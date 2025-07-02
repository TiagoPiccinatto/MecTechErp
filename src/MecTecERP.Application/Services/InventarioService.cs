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
                // Usar o método do repositório que aplica filtros e paginação no banco
                // Assumindo que InventarioFiltroDto tem Descricao, Status, DataInicio, DataFim
                // e os campos de FiltroBaseDto (Pagina, ItensPorPagina, OrdenarPor, OrdemDecrescente)
                var inventarios = await _inventarioRepository.ObterPorFiltroAsync(
                    filtro.Descricao, // Usando filtro.Descricao se existir no DTO
                    filtro.Status,
                    filtro.DataInicio, // Renomeado no repo para DataCriacaoInicio ou similar
                    filtro.DataFim,   // Renomeado no repo para DataCriacaoFim ou similar
                    filtro.Pagina,
                    filtro.ItensPorPagina,
                    filtro.OrdenarPor,
                    filtro.OrdemDecrescente);
                
                var totalItens = await _inventarioRepository.ContarPorFiltroAsync(
                    filtro.Descricao,
                    filtro.Status,
                    filtro.DataInicio,
                    filtro.DataFim);

                var inventariosDto = _mapper.Map<List<InventarioListDto>>(inventarios);

                var paginacao = new PaginacaoDto<InventarioListDto>(inventariosDto, totalItens, filtro.Pagina, filtro.ItensPorPagina);
                
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

                var inventario = _mapper.Map<Inventario>(dto); // Status já vem como Planejado do MappingProfile
                // DataCriacao é definida pelo BaseRepository.AdicionarAsync

                // Criar itens do inventário com base nos produtos ativos
                var produtos = await _produtoRepository.ObterAtivosAsync(); // Método em IProdutoRepository
                foreach (var produto in produtos)
                {
                    var item = new InventarioItem
                    {
                        ProdutoId = produto.Id,
                        EstoqueSistema = produto.EstoqueAtual, // Usar EstoqueSistema conforme entidade InventarioItem
                        EstoqueContado = 0, // Inicialmente 0 ou null se preferir
                        // Diferenca é calculada na entidade
                        // Observacoes e UsuarioContagem seriam preenchidos durante a contagem
                    };
                    // A propriedade ValorUnitario e ValorTotal não existem em InventarioItem.
                    // O valor da diferença seria calculado com base no PrecoCusto do produto.
                    inventario.Itens.Add(item);
                }

                var inventarioAdicionado = await _inventarioRepository.AdicionarAsync(inventario);
                // SalvarAsync removido

                var inventarioDto = _mapper.Map<InventarioDto>(inventarioAdicionado);
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

                // Permitir atualização apenas se Planejado ou EmAndamento (conforme método PodeSerEditado da entidade)
                if (!inventario.PodeSerEditado())
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = $"Apenas inventários com status Planejado ou Em Andamento podem ser atualizados. Status atual: {inventario.Status}"
                    };
                }

                _mapper.Map(dto, inventario);
                // DataAtualizacao é tratada pelo BaseRepository

                await _inventarioRepository.AtualizarAsync(inventario);
                // SalvarAsync removido

                var inventarioDto = _mapper.Map<InventarioDto>(inventario); // Mapear a instância atualizada
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

                // Permitir exclusão apenas se Planejado (antes de iniciar contagem ou qualquer trabalho significativo)
                if (inventario.Status != StatusInventario.Planejado)
                {
                    return new RespostaDto
                    {
                        Sucesso = false,
                        Mensagem = $"Apenas inventários com status Planejado podem ser excluídos. Status atual: {inventario.Status}"
                    };
                }
                // Adicionar lógica para remover itens de inventário associados, ou garantir que DB faça em cascata.

                await _inventarioRepository.RemoverAsync(id); // Usar RemoverAsync(id)
                // SalvarAsync removido

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

                // Usar o método da entidade para validação e mudança de status
                // if (inventario.Status != StatusInventario.Planejado) // Entidade tem PodeSerIniciado()
                if (!inventario.PodeSerIniciado())
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = $"Inventário não pode ser iniciado. Status atual: {inventario.Status}"
                    };
                }

                inventario.Iniciar(); // Método da entidade já define Status e DataInicio, e chama MarcarComoAtualizado
                // inventario.Status = StatusInventario.EmAndamento; // Feito pela entidade
                // inventario.DataInicio = DateTime.Now; // Feito pela entidade

                await _inventarioRepository.AtualizarAsync(inventario);
                // SalvarAsync removido

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
                await ProcessarDivergenciasInternoAsync(inventario); // Este método precisa ser transacional

                // Usar método da entidade para finalizar
                // if (inventario.Status != StatusInventario.EmAndamento) // Entidade tem PodeSerFinalizado()
                if(!inventario.PodeSerFinalizado())
                {
                     return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = $"Inventário não pode ser finalizado. Status atual: {inventario.Status} ou não possui itens."
                    };
                }
                inventario.Finalizar(); // Método da entidade já define Status e DataFinalizacao

                await _inventarioRepository.AtualizarAsync(inventario);
                // SalvarAsync removido

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

                // Usar método da entidade para cancelar
                // if (inventario.Status == StatusInventario.Finalizado) // Entidade tem PodeSerCancelado()
                if(!inventario.PodeSerCancelado())
                {
                    return new RespostaDto<InventarioDto>
                    {
                        Sucesso = false,
                        Mensagem = $"Inventário não pode ser cancelado. Status atual: {inventario.Status}"
                    };
                }
                inventario.Cancelar(); // Método da entidade já define Status e DataCancelamento (se existir na entidade)
                                      // A entidade Inventario não tem DataCancelamento, mas MarcarComoAtualizado é chamado.

                await _inventarioRepository.AtualizarAsync(inventario);
                // SalvarAsync removido

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

                var item = inventario.Itens.FirstOrDefault(i => i.ProdutoId == dto.ProdutoId && i.Id == dto.Id); // Usar Id do item se fornecido pelo DTO
                if (item == null && dto.Id > 0) // Tentar buscar pelo Id se não encontrado pelo ProdutoId na coleção já carregada
                {
                    // item = await _inventarioRepository.ObterItemPorIdAsync(dto.Id); // Necessitaria de um método no repo
                    // Esta lógica de buscar item individualmente pode ser complexa se não estiver na coleção 'inventario.Itens'
                }
                if (item == null) // Se ainda não encontrou
                {
                    // Se o item não está na coleção do inventário carregado, pode ser um erro ou um novo item.
                    // Para atualização, o item DEVE existir. O DTO InventarioItemUpdateDto deve ter o Id do item.
                    return new RespostaDto<InventarioItemDto>
                    {
                        Sucesso = false,
                        Mensagem = "Item não encontrado no inventário para o ProdutoId ou ItemId fornecido."
                    };
                }

                // Usar o método da entidade para registrar a contagem, se ele fizer mais do que apenas setar propriedades.
                // A entidade InventarioItem tem: public void RegistrarContagem(decimal estoqueContado, string? usuario = null, string? observacoes = null)
                // Assumindo que o usuário logado será pego de um contexto de usuário no serviço real.
                item.RegistrarContagem(dto.EstoqueContado, usuario: null, observacoes: dto.Observacoes);
                // item.EstoqueContado = dto.EstoqueContado; // Feito por RegistrarContagem
                // item.Observacoes = dto.Observacoes; // Feito por RegistrarContagem

                // Não é necessário chamar _inventarioRepository.Atualizar(inventario) para cada item.
                // A atualização do inventário (que contém os itens) será feita uma vez se múltiplos itens forem atualizados,
                // ou após todas as manipulações de itens.
                // Se este método AtualizarItemAsync for para um único item, então sim, salvar o inventário.
                await _inventarioRepository.AtualizarAsync(inventario); // Salva o inventário com o item atualizado
                // SalvarAsync removido

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
                    // Assumindo que InventarioItemUpdateDto tem Id para identificar o item a ser atualizado.
                    var item = inventario.Itens.FirstOrDefault(i => i.Id == itemDto.Id);
                    if (item != null)
                    {
                        // Usar o método da entidade para registrar a contagem
                        item.RegistrarContagem(itemDto.EstoqueContado, usuario: null, observacoes: itemDto.Observacoes);
                        // item.EstoqueContado = itemDto.EstoqueContado;
                        // item.Observacoes = itemDto.Observacoes;

                        itensAtualizados.Add(_mapper.Map<InventarioItemDto>(item));
                    }
                    // else: Logar ou tratar item não encontrado se necessário.
                }

                if(itensAtualizados.Any()){ // Só salvar se algo foi de fato modificado na coleção
                    await _inventarioRepository.AtualizarAsync(inventario);
                     // SalvarAsync removido
                }

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
                    // Usar RegistrarMovimentacaoAsync do repo que já deve lidar com a atualização do produto e ser transacional por si.
                    // A entidade MovimentacaoEstoque precisa de EstoqueAnterior e EstoquePosterior.
                    // EstoqueAnterior seria item.EstoqueSistema. EstoquePosterior seria item.EstoqueContado.
                    var movimentacaoAjuste = new MovimentacaoEstoque
                    {
                        ProdutoId = item.ProdutoId,
                        InventarioId = inventario.Id, // Associar ao inventário
                        Tipo = TipoMovimentacaoEstoque.Inventario, // Usar tipo específico para ajuste de inventário
                        Quantidade = item.Diferenca, // Diferença já é Contado - Sistema
                        EstoqueAnterior = item.EstoqueSistema,
                        EstoquePosterior = item.EstoqueContado,
                        ValorUnitario = produto?.PrecoCusto ?? 0, // Usar PrecoCusto do produto
                        DataMovimentacao = DateTime.UtcNow,
                        Observacoes = $"Ajuste de inventário ID: {inventario.Id}. Contado: {item.EstoqueContado}, Sistema: {item.EstoqueSistema}."
                        // UsuarioCriacao será definido pelo BaseRepository
                    };

                    // Este método já deve atualizar o produto.EstoqueAtual para item.EstoqueContado e salvar a movimentação.
                    await _movimentacaoRepository.RegistrarMovimentacaoAsync(movimentacaoAjuste);
                }
            }
            // SalvarAsync removidos, pois RegistrarMovimentacaoAsync deve ser completo.
        }
    }
}