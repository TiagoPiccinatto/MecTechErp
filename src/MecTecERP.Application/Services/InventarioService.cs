using AutoMapper;
using Microsoft.Extensions.Logging;
using MecTecERP.Application.DTOs;
using MecTecERP.Application.Interfaces;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly IInventarioRepository _inventarioRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<InventarioService> _logger;

        public InventarioService(
            IInventarioRepository inventarioRepository,
            IProdutoRepository produtoRepository,
            IMovimentacaoEstoqueRepository movimentacaoRepository,
            IMapper mapper,
            ILogger<InventarioService> logger)
        {
            _inventarioRepository = inventarioRepository;
            _produtoRepository = produtoRepository;
            _movimentacaoRepository = movimentacaoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RespostaDto<InventarioDto>> ObterPorIdAsync(int id)
        {
            try
            {
                var inventario = await _inventarioRepository.ObterComItensAsync(id);
                if (inventario == null)
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                var inventarioDto = _mapper.Map<InventarioDto>(inventario);
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = true,
                    Dados = inventarioDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter inventário por ID: {Id}", id);
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<PaginacaoDto<InventarioListDto>>> ObterTodosAsync(InventarioFiltroDto filtro)
        {
            try
            {
                var inventarios = await _inventarioRepository.ObterTodosAsync();
                
                // Aplicar filtros
                if (!string.IsNullOrEmpty(filtro.Busca))
                {
                    inventarios = inventarios.Where(i => 
                        i.Descricao.Contains(filtro.Busca, StringComparison.OrdinalIgnoreCase) ||
                        i.Observacoes.Contains(filtro.Busca, StringComparison.OrdinalIgnoreCase));
                }

                if (filtro.Status.HasValue)
                {
                    inventarios = inventarios.Where(i => i.Status == filtro.Status.Value);
                }

                if (filtro.DataInicio.HasValue)
                {
                    inventarios = inventarios.Where(i => i.DataCriacao >= filtro.DataInicio.Value);
                }

                if (filtro.DataFim.HasValue)
                {
                    inventarios = inventarios.Where(i => i.DataCriacao <= filtro.DataFim.Value);
                }

                var totalItens = inventarios.Count();
                
                // Ordenação
                if (!string.IsNullOrEmpty(filtro.OrdenarPor))
                {
                    switch (filtro.OrdenarPor.ToLower())
                    {
                        case "descricao":
                            inventarios = inventarios.OrderBy(i => i.Descricao);
                            break;
                        case "status":
                            inventarios = inventarios.OrderBy(i => i.Status);
                            break;
                        case "datacriacao":
                            inventarios = inventarios.OrderBy(i => i.DataCriacao);
                            break;
                        default:
                            inventarios = inventarios.OrderByDescending(i => i.Id);
                            break;
                    }
                }
                else
                {
                    inventarios = inventarios.OrderByDescending(i => i.Id);
                }

                // Paginação
                var inventariosPaginados = inventarios
                    .Skip((filtro.Pagina - 1) * filtro.ItensPorPagina)
                    .Take(filtro.ItensPorPagina)
                    .ToList();

                var inventariosDto = _mapper.Map<List<InventarioListDto>>(inventariosPaginados);

                var paginacao = new PaginacaoDto<InventarioListDto>
                {
                    Itens = inventariosDto,
                    TotalItens = totalItens,
                    Pagina = filtro.Pagina,
                    ItensPorPagina = filtro.ItensPorPagina,
                    TotalPaginas = (int)Math.Ceiling((double)totalItens / filtro.ItensPorPagina)
                };

                return new RespostaDto<PaginacaoDto<InventarioListDto>>
                {
                    Sucesso = true,
                    Dados = paginacao
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os inventários");
                return new RespostaDto<PaginacaoDto<InventarioListDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Dados = new PaginacaoDto<InventarioListDto>
                    {
                        Itens = new List<InventarioListDto>(),
                        TotalItens = 0,
                        Pagina = filtro.Pagina,
                        ItensPorPagina = filtro.ItensPorPagina,
                        TotalPaginas = 0
                    }
                };
            }
        }

        public async Task<RespostaDto<InventarioDto>> CriarAsync(InventarioCreateDto dto)
        {
            try
            {
                // Verificar se já existe inventário aberto
                var inventarioAberto = await _inventarioRepository.ObterInventarioAbertoAsync();
                if (inventarioAberto != null)
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Já existe um inventário aberto. Finalize ou cancele antes de criar um novo."
                    };
                }

                var inventario = _mapper.Map<Inventario>(dto);
                inventario.DataCriacao = DateTime.Now;
                inventario.Status = StatusInventario.Aberto;

                // Criar itens do inventário com base nos produtos ativos
                var produtos = await _produtoRepository.ObterTodosAtivosAsync();
                foreach (var produto in produtos)
                {
                    var item = new InventarioItem
                    {
                        ProdutoId = produto.Id,
                        EstoqueAtual = produto.EstoqueAtual,
                        EstoqueContado = 0,
                        Divergencia = -produto.EstoqueAtual,
                        ValorUnitario = produto.PrecoVenda,
                        ValorTotal = 0
                    };
                    inventario.Itens.Add(item);
                }

                await _inventarioRepository.AdicionarAsync(inventario);
                await _inventarioRepository.SalvarAsync();

                var inventarioDto = _mapper.Map<InventarioDto>(inventario);
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = true,
                    Dados = inventarioDto,
                    Mensagem = "Inventário criado com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar inventário");
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<InventarioDto>> AtualizarAsync(int id, InventarioUpdateDto dto)
        {
            try
            {
                var inventario = await _inventarioRepository.ObterPorIdAsync(id);
                if (inventario == null)
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                if (inventario.Status != StatusInventario.Aberto)
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Apenas inventários abertos podem ser atualizados"
                    };
                }

                _mapper.Map(dto, inventario);
                inventario.DataAtualizacao = DateTime.Now;

                _inventarioRepository.Atualizar(inventario);
                await _inventarioRepository.SalvarAsync();

                var inventarioDto = _mapper.Map<InventarioDto>(inventario);
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = true,
                    Dados = inventarioDto,
                    Mensagem = "Inventário atualizado com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar inventário: {Id}", id);
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto> ExcluirAsync(int id)
        {
            try
            {
                var inventario = await _inventarioRepository.ObterPorIdAsync(id);
                if (inventario == null)
                {
                    return new RespostaDto
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                if (inventario.Status != StatusInventario.Aberto)
                {
                    return new RespostaDto
                    {
                        Sucesso = false,
                        Mensagem = "Apenas inventários abertos podem ser excluídos"
                    };
                }

                _inventarioRepository.Remover(inventario);
                await _inventarioRepository.SalvarAsync();

                return new RespostaDto
                {
                    Sucesso = true,
                    Mensagem = "Inventário excluído com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir inventário: {Id}", id);
                return new RespostaDto
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<InventarioDto>> IniciarInventarioAsync(int id)
        {
            try
            {
                var inventario = await _inventarioRepository.ObterPorIdAsync(id);
                if (inventario == null)
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                if (inventario.Status != StatusInventario.Aberto)
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Apenas inventários abertos podem ser iniciados"
                    };
                }

                inventario.Status = StatusInventario.EmAndamento;
                inventario.DataInicio = DateTime.Now;

                _inventarioRepository.Atualizar(inventario);
                await _inventarioRepository.SalvarAsync();

                var inventarioDto = _mapper.Map<InventarioDto>(inventario);
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = true,
                    Dados = inventarioDto,
                    Mensagem = "Inventário iniciado com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar inventário: {Id}", id);
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<InventarioDto>> FinalizarInventarioAsync(int id)
        {
            try
            {
                var inventario = await _inventarioRepository.ObterComItensAsync(id);
                if (inventario == null)
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                if (inventario.Status != StatusInventario.EmAndamento)
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Apenas inventários em andamento podem ser finalizados"
                    };
                }

                inventario.Status = StatusInventario.Finalizado;
                inventario.DataFinalizacao = DateTime.Now;

                // Processar divergências e criar movimentações
                await ProcessarDivergenciasInternoAsync(inventario);

                _inventarioRepository.Atualizar(inventario);
                await _inventarioRepository.SalvarAsync();

                var inventarioDto = _mapper.Map<InventarioDto>(inventario);
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = true,
                    Dados = inventarioDto,
                    Mensagem = "Inventário finalizado com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao finalizar inventário: {Id}", id);
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<InventarioDto>> CancelarInventarioAsync(int id)
        {
            try
            {
                var inventario = await _inventarioRepository.ObterPorIdAsync(id);
                if (inventario == null)
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                if (inventario.Status == StatusInventario.Finalizado)
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Inventários finalizados não podem ser cancelados"
                    };
                }

                inventario.Status = StatusInventario.Cancelado;
                inventario.DataCancelamento = DateTime.Now;

                _inventarioRepository.Atualizar(inventario);
                await _inventarioRepository.SalvarAsync();

                var inventarioDto = _mapper.Map<InventarioDto>(inventario);
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = true,
                    Dados = inventarioDto,
                    Mensagem = "Inventário cancelado com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar inventário: {Id}", id);
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<InventarioItemDto>> AtualizarItemAsync(int inventarioId, InventarioItemUpdateDto dto)
        {
            try
            {
                var inventario = await _inventarioRepository.ObterComItensAsync(inventarioId);
                if (inventario == null)
                {
                    return new RespostaDto<InventarioItemDto>
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                if (inventario.Status != StatusInventario.EmAndamento)
                {
                    return new RespostaDto<InventarioItemDto>
                    {
                        Sucesso = false,
                        Mensagem = "Apenas itens de inventários em andamento podem ser atualizados"
                    };
                }

                var item = inventario.Itens.FirstOrDefault(i => i.ProdutoId == dto.ProdutoId);
                if (item == null)
                {
                    return new RespostaDto<InventarioItemDto>
                    {
                        Sucesso = false,
                        Mensagem = "Item não encontrado no inventário"
                    };
                }

                item.EstoqueContado = dto.EstoqueContado;
                item.Divergencia = dto.EstoqueContado - item.EstoqueAtual;
                item.ValorTotal = item.Divergencia * item.ValorUnitario;
                item.Observacoes = dto.Observacoes;

                _inventarioRepository.Atualizar(inventario);
                await _inventarioRepository.SalvarAsync();

                var itemDto = _mapper.Map<InventarioItemDto>(item);
                return new RespostaDto<InventarioItemDto>
                {
                    Sucesso = true,
                    Dados = itemDto,
                    Mensagem = "Item atualizado com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar item do inventário: {InventarioId}", inventarioId);
                return new RespostaDto<InventarioItemDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<List<InventarioItemDto>>> AtualizarItensAsync(int inventarioId, List<InventarioItemUpdateDto> itens)
        {
            try
            {
                var inventario = await _inventarioRepository.ObterComItensAsync(inventarioId);
                if (inventario == null)
                {
                    return new RespostaDto<List<InventarioItemDto>>
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                if (inventario.Status != StatusInventario.EmAndamento)
                {
                    return new RespostaDto<List<InventarioItemDto>>
                    {
                        Sucesso = false,
                        Mensagem = "Apenas itens de inventários em andamento podem ser atualizados"
                    };
                }

                var itensAtualizados = new List<InventarioItemDto>();

                foreach (var itemDto in itens)
                {
                    var item = inventario.Itens.FirstOrDefault(i => i.ProdutoId == itemDto.ProdutoId);
                    if (item != null)
                    {
                        item.EstoqueContado = itemDto.EstoqueContado;
                        item.Divergencia = itemDto.EstoqueContado - item.EstoqueAtual;
                        item.ValorTotal = item.Divergencia * item.ValorUnitario;
                        item.Observacoes = itemDto.Observacoes;

                        itensAtualizados.Add(_mapper.Map<InventarioItemDto>(item));
                    }
                }

                _inventarioRepository.Atualizar(inventario);
                await _inventarioRepository.SalvarAsync();

                return new RespostaDto<List<InventarioItemDto>>
                {
                    Sucesso = true,
                    Dados = itensAtualizados,
                    Mensagem = $"{itensAtualizados.Count} itens atualizados com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar itens do inventário: {InventarioId}", inventarioId);
                return new RespostaDto<List<InventarioItemDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<List<InventarioItemDto>>> ObterItensAsync(int inventarioId)
        {
            try
            {
                var inventario = await _inventarioRepository.ObterComItensAsync(inventarioId);
                if (inventario == null)
                {
                    return new RespostaDto<List<InventarioItemDto>>
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                var itensDto = _mapper.Map<List<InventarioItemDto>>(inventario.Itens);
                return new RespostaDto<List<InventarioItemDto>>
                {
                    Sucesso = true,
                    Dados = itensDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter itens do inventário: {InventarioId}", inventarioId);
                return new RespostaDto<List<InventarioItemDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<List<InventarioItemDto>>> ObterItensComDivergenciaAsync(int inventarioId)
        {
            try
            {
                var inventario = await _inventarioRepository.ObterComItensAsync(inventarioId);
                if (inventario == null)
                {
                    return new RespostaDto<List<InventarioItemDto>>
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                var itensComDivergencia = inventario.Itens.Where(i => i.Divergencia != 0).ToList();
                var itensDto = _mapper.Map<List<InventarioItemDto>>(itensComDivergencia);
                
                return new RespostaDto<List<InventarioItemDto>>
                {
                    Sucesso = true,
                    Dados = itensDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter itens com divergência: {InventarioId}", inventarioId);
                return new RespostaDto<List<InventarioItemDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<List<InventarioItemDto>>> ObterItensPendentesAsync(int inventarioId)
        {
            try
            {
                var inventario = await _inventarioRepository.ObterComItensAsync(inventarioId);
                if (inventario == null)
                {
                    return new RespostaDto<List<InventarioItemDto>>
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                var itensPendentes = inventario.Itens.Where(i => i.EstoqueContado == 0 && i.EstoqueAtual > 0).ToList();
                var itensDto = _mapper.Map<List<InventarioItemDto>>(itensPendentes);
                
                return new RespostaDto<List<InventarioItemDto>>
                {
                    Sucesso = true,
                    Dados = itensDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter itens pendentes: {InventarioId}", inventarioId);
                return new RespostaDto<List<InventarioItemDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<InventarioResumoDto>> ObterResumoAsync()
        {
            try
            {
                var inventarios = await _inventarioRepository.ObterTodosAsync();
                
                var resumo = new InventarioResumoDto
                {
                    TotalInventarios = inventarios.Count(),
                    InventariosAbertos = inventarios.Count(i => i.Status == StatusInventario.Aberto),
                    InventariosEmAndamento = inventarios.Count(i => i.Status == StatusInventario.EmAndamento),
                    InventariosFinalizados = inventarios.Count(i => i.Status == StatusInventario.Finalizado),
                    InventariosCancelados = inventarios.Count(i => i.Status == StatusInventario.Cancelado)
                };

                return new RespostaDto<InventarioResumoDto>
                {
                    Sucesso = true,
                    Dados = resumo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter resumo de inventários");
                return new RespostaDto<InventarioResumoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<InventarioDto>> ObterInventarioAbertoAsync()
        {
            try
            {
                var inventario = await _inventarioRepository.ObterInventarioAbertoAsync();
                if (inventario == null)
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Nenhum inventário aberto encontrado"
                    };
                }

                var inventarioDto = _mapper.Map<InventarioDto>(inventario);
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = true,
                    Dados = inventarioDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter inventário aberto");
                return new RespostaDto<InventarioDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<bool>> ExisteInventarioAbertoAsync()
        {
            try
            {
                var inventario = await _inventarioRepository.ObterInventarioAbertoAsync();
                return new RespostaDto<bool>
                {
                    Sucesso = true,
                    Dados = inventario != null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se existe inventário aberto");
                return new RespostaDto<bool>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<bool>> PodeIniciarNovoInventarioAsync()
        {
            try
            {
                var inventarioAberto = await _inventarioRepository.ObterInventarioAbertoAsync();
                var podeIniciar = inventarioAberto == null;
                
                return new RespostaDto<bool>
                {
                    Sucesso = true,
                    Dados = podeIniciar,
                    Mensagem = podeIniciar ? "Pode iniciar novo inventário" : "Existe inventário aberto"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se pode iniciar novo inventário");
                return new RespostaDto<bool>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto> ProcessarDivergenciasAsync(int inventarioId, bool aplicarAjustes = false)
        {
            try
            {
                var inventario = await _inventarioRepository.ObterComItensAsync(inventarioId);
                if (inventario == null)
                {
                    return new RespostaDto
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                if (aplicarAjustes)
                {
                    await ProcessarDivergenciasInternoAsync(inventario);
                }

                return new RespostaDto
                {
                    Sucesso = true,
                    Mensagem = aplicarAjustes ? "Divergências processadas com sucesso" : "Divergências analisadas"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar divergências: {InventarioId}", inventarioId);
                return new RespostaDto
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<decimal>> CalcularValorDivergenciasAsync(int inventarioId)
        {
            try
            {
                var inventario = await _inventarioRepository.ObterComItensAsync(inventarioId);
                if (inventario == null)
                {
                    return new RespostaDto<decimal>
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                var valorDivergencias = inventario.Itens.Sum(i => Math.Abs(i.ValorTotal));
                
                return new RespostaDto<decimal>
                {
                    Sucesso = true,
                    Dados = valorDivergencias
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular valor das divergências: {InventarioId}", inventarioId);
                return new RespostaDto<decimal>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<List<MovimentacaoEstoqueListDto>>> ObterMovimentacoesInventarioAsync(int inventarioId)
        {
            try
            {
                var movimentacoes = await _movimentacaoRepository.ObterPorInventarioIdAsync(inventarioId);
                var movimentacoesDto = _mapper.Map<List<MovimentacaoEstoqueListDto>>(movimentacoes);
                
                return new RespostaDto<List<MovimentacaoEstoqueListDto>>
                {
                    Sucesso = true,
                    Dados = movimentacoesDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter movimentações do inventário: {InventarioId}", inventarioId);
                return new RespostaDto<List<MovimentacaoEstoqueListDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<ExportacaoDto>> ExportarInventarioAsync(int inventarioId, string formato = "excel")
        {
            try
            {
                var inventario = await _inventarioRepository.ObterComItensAsync(inventarioId);
                if (inventario == null)
                {
                    return new RespostaDto<ExportacaoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                // Implementar lógica de exportação
                var exportacao = new ExportacaoDto
                {
                    NomeArquivo = $"Inventario_{inventario.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.{formato}",
                    Formato = formato,
                    DataGeracao = DateTime.Now,
                    Tamanho = 0 // Será calculado após gerar o arquivo
                };

                return new RespostaDto<ExportacaoDto>
                {
                    Sucesso = true,
                    Dados = exportacao,
                    Mensagem = "Exportação gerada com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exportar inventário: {InventarioId}", inventarioId);
                return new RespostaDto<ExportacaoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<ExportacaoDto>> ExportarDivergenciasAsync(int inventarioId, string formato = "excel")
        {
            try
            {
                var inventario = await _inventarioRepository.ObterComItensAsync(inventarioId);
                if (inventario == null)
                {
                    return new RespostaDto<ExportacaoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Inventário não encontrado"
                    };
                }

                // Implementar lógica de exportação das divergências
                var exportacao = new ExportacaoDto
                {
                    NomeArquivo = $"Divergencias_Inventario_{inventario.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.{formato}",
                    Formato = formato,
                    DataGeracao = DateTime.Now,
                    Tamanho = 0 // Será calculado após gerar o arquivo
                };

                return new RespostaDto<ExportacaoDto>
                {
                    Sucesso = true,
                    Dados = exportacao,
                    Mensagem = "Exportação de divergências gerada com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exportar divergências: {InventarioId}", inventarioId);
                return new RespostaDto<ExportacaoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        private async Task ProcessarDivergenciasInternoAsync(Inventario inventario)
        {
            foreach (var item in inventario.Itens.Where(i => i.Divergencia != 0))
            {
                var produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
                if (produto != null)
                {
                    // Criar movimentação de ajuste
                    var movimentacao = new MovimentacaoEstoque
                    {
                        ProdutoId = item.ProdutoId,
                        InventarioId = inventario.Id,
                        Tipo = item.Divergencia > 0 ? TipoMovimentacao.Entrada : TipoMovimentacao.Saida,
                        Quantidade = Math.Abs(item.Divergencia),
                        Valor = 0, // Ajustes não têm valor
                        DataMovimentacao = DateTime.Now,
                        Observacoes = $"Ajuste de inventário - ID: {inventario.Id}"
                    };

                    await _movimentacaoRepository.AdicionarAsync(movimentacao);

                    // Atualizar estoque do produto
                    var produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
                    if (produto != null)
                    {
                        produto.EstoqueAtual = item.EstoqueContado;
                        produto.AtualizarStatusEstoque();
                        await _produtoRepository.AtualizarAsync(produto);
                    }
                }
            }

            await _movimentacaoRepository.SalvarAsync();
            await _produtoRepository.SalvarAsync();
        }
    }
}