using AutoMapper;
using Microsoft.Extensions.Logging;
using MecTecERP.Application.DTOs;
using MecTecERP.Application.Interfaces;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.Services
{
    public class OrdemServicoService : IOrdemServicoService
    {
        private readonly IOrdemServicoRepository _ordemServicoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrdemServicoService> _logger;

        public OrdemServicoService(
            IOrdemServicoRepository ordemServicoRepository,
            IClienteRepository clienteRepository,
            IVeiculoRepository veiculoRepository,
            IProdutoRepository produtoRepository,
            IMovimentacaoEstoqueRepository movimentacaoRepository,
            IMapper mapper,
            ILogger<OrdemServicoService> logger)
        {
            _ordemServicoRepository = ordemServicoRepository;
            _clienteRepository = clienteRepository;
            _veiculoRepository = veiculoRepository;
            _produtoRepository = produtoRepository;
            _movimentacaoRepository = movimentacaoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RespostaDto<OrdemServicoDto>> ObterPorIdAsync(int id)
        {
            try
            {
                var ordemServico = await _ordemServicoRepository.ObterCompletaAsync(id);
                if (ordemServico == null)
                {
                    return new RespostaDto<OrdemServicoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Ordem de serviço não encontrada"
                    };
                }

                var ordemServicoDto = _mapper.Map<OrdemServicoDto>(ordemServico);
                return new RespostaDto<OrdemServicoDto>
                {
                    Sucesso = true,
                    Dados = ordemServicoDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter ordem de serviço por ID: {Id}", id);
                return new RespostaDto<OrdemServicoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<PaginacaoDto<OrdemServicoListDto>>> ObterTodosAsync(OrdemServicoFiltroDto filtro)
        {
            try
            {
                _logger.LogInformation("Obtendo todas as ordens de serviço com filtros");

                var ordensServico = await _ordemServicoRepository.ObterTodosComClienteVeiculoAsync();
                
                // Aplicar filtros
                if (!string.IsNullOrEmpty(filtro.Busca))
                {
                    ordensServico = ordensServico.Where(os => 
                        os.Numero.Contains(filtro.Busca, StringComparison.OrdinalIgnoreCase) ||
                        os.Descricao.Contains(filtro.Busca, StringComparison.OrdinalIgnoreCase) ||
                        (os.Cliente != null && os.Cliente.Nome.Contains(filtro.Busca, StringComparison.OrdinalIgnoreCase)) ||
                        (os.Veiculo != null && os.Veiculo.Placa.Contains(filtro.Busca, StringComparison.OrdinalIgnoreCase)));
                }

                if (filtro.ClienteId.HasValue)
                {
                    ordensServico = ordensServico.Where(os => os.ClienteId == filtro.ClienteId.Value);
                }

                if (filtro.VeiculoId.HasValue)
                {
                    ordensServico = ordensServico.Where(os => os.VeiculoId == filtro.VeiculoId.Value);
                }

                if (!string.IsNullOrEmpty(filtro.Numero))
                {
                    ordensServico = ordensServico.Where(os => os.Numero.Contains(filtro.Numero, StringComparison.OrdinalIgnoreCase));
                }

                if (filtro.Status.HasValue)
                {
                    ordensServico = ordensServico.Where(os => os.Status == filtro.Status.Value);
                }

                if (filtro.DataInicio.HasValue)
                {
                    ordensServico = ordensServico.Where(os => os.DataAbertura >= filtro.DataInicio.Value);
                }

                if (filtro.DataFim.HasValue)
                {
                    ordensServico = ordensServico.Where(os => os.DataAbertura <= filtro.DataFim.Value);
                }

                var totalItens = ordensServico.Count();
                
                // Ordenação
                if (!string.IsNullOrEmpty(filtro.OrdenarPor))
                {
                    switch (filtro.OrdenarPor.ToLower())
                    {
                        case "numero":
                            ordensServico = ordensServico.OrderBy(os => os.Numero);
                            break;
                        case "cliente":
                            ordensServico = ordensServico.OrderBy(os => os.Cliente?.Nome);
                            break;
                        case "veiculo":
                            ordensServico = ordensServico.OrderBy(os => os.Veiculo?.Placa);
                            break;
                        case "status":
                            ordensServico = ordensServico.OrderBy(os => os.Status);
                            break;
                        case "dataabertura":
                            ordensServico = ordensServico.OrderBy(os => os.DataAbertura);
                            break;
                        case "valortotal":
                            ordensServico = ordensServico.OrderBy(os => os.ValorTotal);
                            break;
                        default:
                            ordensServico = ordensServico.OrderByDescending(os => os.Id);
                            break;
                    }
                }
                else
                {
                    ordensServico = ordensServico.OrderByDescending(os => os.Id);
                }

                // Paginação
                var ordensServicoPaginadas = ordensServico
                    .Skip((filtro.Pagina - 1) * filtro.ItensPorPagina)
                    .Take(filtro.ItensPorPagina)
                    .ToList();

                var ordensServicoDto = _mapper.Map<List<OrdemServicoListDto>>(ordensServicoPaginadas);

                var paginacao = new PaginacaoDto<OrdemServicoListDto>
                {
                    Itens = ordensServicoDto,
                    TotalItens = totalItens,
                    Pagina = filtro.Pagina,
                    ItensPorPagina = filtro.ItensPorPagina,
                    TotalPaginas = (int)Math.Ceiling((double)totalItens / filtro.ItensPorPagina)
                };

                return RespostaDto<PaginacaoDto<OrdemServicoListDto>>.Sucesso(paginacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todas as ordens de serviço");
                var paginacaoVazia = new PaginacaoDto<OrdemServicoListDto>
                {
                    Itens = new List<OrdemServicoListDto>(),
                    TotalItens = 0,
                    Pagina = filtro.Pagina,
                    ItensPorPagina = filtro.ItensPorPagina,
                    TotalPaginas = 0
                };
                return RespostaDto<PaginacaoDto<OrdemServicoListDto>>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<OrdemServicoDto>> CriarAsync(OrdemServicoCreateDto ordemServicoCreateDto)
        {
            try
            {
                // Verificar se o cliente existe
                var cliente = await _clienteRepository.ObterPorIdAsync(ordemServicoCreateDto.ClienteId);
                if (cliente == null)
                {
                    return new RespostaDto<OrdemServicoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Cliente não encontrado"
                    };
                }

                // Verificar se o veículo existe
                var veiculo = await _veiculoRepository.ObterPorIdAsync(ordemServicoCreateDto.VeiculoId);
                if (veiculo == null)
                {
                    return new RespostaDto<OrdemServicoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Veículo não encontrado"
                    };
                }

                // Verificar se o número já existe
                var ordemExistente = await _ordemServicoRepository.ObterPorNumeroAsync(ordemServicoCreateDto.Numero);
                if (ordemExistente != null)
                {
                    return new RespostaDto<OrdemServicoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Já existe uma ordem de serviço com este número"
                    };
                }

                var ordemServico = _mapper.Map<OrdemServico>(ordemServicoCreateDto);
                ordemServico.DataAbertura = DateTime.Now;
                ordemServico.Status = StatusOrdemServico.Aberta;
                ordemServico.ValorTotal = 0;

                await _ordemServicoRepository.AdicionarAsync(ordemServico);
                await _ordemServicoRepository.SalvarAsync();

                var ordemServicoDto = _mapper.Map<OrdemServicoDto>(ordemServico);
                return new RespostaDto<OrdemServicoDto>
                {
                    Sucesso = true,
                    Dados = ordemServicoDto,
                    Mensagem = "Ordem de serviço criada com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar ordem de serviço");
                return new RespostaDto<OrdemServicoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<OrdemServicoDto>> AtualizarAsync(int id, OrdemServicoUpdateDto ordemServicoUpdateDto)
        {
            try
            {
                var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(id);
                if (ordemServico == null)
                {
                    return new RespostaDto<OrdemServicoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Ordem de serviço não encontrada"
                    };
                }

                // Verificar se o cliente existe
                var cliente = await _clienteRepository.ObterPorIdAsync(ordemServicoUpdateDto.ClienteId);
                if (cliente == null)
                {
                    return new RespostaDto<OrdemServicoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Cliente não encontrado"
                    };
                }

                // Verificar se o veículo existe
                var veiculo = await _veiculoRepository.ObterPorIdAsync(ordemServicoUpdateDto.VeiculoId);
                if (veiculo == null)
                {
                    return new RespostaDto<OrdemServicoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Veículo não encontrado"
                    };
                }

                // Verificar se o número já existe em outra ordem
                var ordemExistente = await _ordemServicoRepository.ObterPorNumeroAsync(ordemServicoUpdateDto.Numero);
                if (ordemExistente != null && ordemExistente.Id != id)
                {
                    return new RespostaDto<OrdemServicoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Já existe outra ordem de serviço com este número"
                    };
                }

                _mapper.Map(ordemServicoUpdateDto, ordemServico);
                ordemServico.DataAtualizacao = DateTime.Now;

                _ordemServicoRepository.Atualizar(ordemServico);
                await _ordemServicoRepository.SalvarAsync();

                var ordemServicoDto = _mapper.Map<OrdemServicoDto>(ordemServico);
                return new RespostaDto<OrdemServicoDto>
                {
                    Sucesso = true,
                    Dados = ordemServicoDto,
                    Mensagem = "Ordem de serviço atualizada com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar ordem de serviço: {Id}", id);
                return new RespostaDto<OrdemServicoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<bool>> ExcluirAsync(int id)
        {
            try
            {
                var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(id);
                if (ordemServico == null)
                {
                    return new RespostaDto<bool>
                    {
                        Sucesso = false,
                        Mensagem = "Ordem de serviço não encontrada"
                    };
                }

                // Verificar se a ordem pode ser excluída (apenas se estiver aberta)
                if (ordemServico.Status != StatusOrdemServico.Aberta)
                {
                    return new RespostaDto<bool>
                    {
                        Sucesso = false,
                        Mensagem = "Apenas ordens de serviço abertas podem ser excluídas"
                    };
                }

                _ordemServicoRepository.Remover(ordemServico);
                await _ordemServicoRepository.SalvarAsync();

                return new RespostaDto<bool>
                {
                    Sucesso = true,
                    Dados = true,
                    Mensagem = "Ordem de serviço excluída com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir ordem de serviço: {Id}", id);
                return new RespostaDto<bool>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<bool>> ExisteNumeroAsync(string numero, int? idExcluir = null)
        {
            try
            {
                _logger.LogInformation("Verificando se número existe: {Numero}", numero);

                var ordemServico = await _ordemServicoRepository.ObterPorNumeroAsync(numero);
                if (ordemServico == null)
                {
                    return RespostaDto<bool>.Sucesso(false);
                }
                
                bool existe = idExcluir == null || ordemServico.Id != idExcluir;
                return RespostaDto<bool>.Sucesso(existe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se número existe: {Numero}", numero);
                return RespostaDto<bool>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<SelectItemDto>>> ObterSelectListAsync()
        {
            try
            {
                _logger.LogInformation("Obtendo select list de ordens de serviço");

                var ordensServico = await _ordemServicoRepository.ObterTodosComClienteVeiculoAsync();
                var selectList = ordensServico
                    .Where(os => os.Status == StatusOrdemServico.Aberta)
                    .Select(os => new SelectItemDto
                    {
                        Value = os.Id.ToString(),
                        Text = $"{os.Numero} - {os.Cliente?.Nome} ({os.Veiculo?.Placa})"
                    })
                    .OrderBy(x => x.Text)
                    .ToList();
                
                return RespostaDto<List<SelectItemDto>>.Sucesso(selectList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter select list de ordens de serviço");
                return RespostaDto<List<SelectItemDto>>.Erro("Erro interno do servidor");
            }
        }

        public async Task<IEnumerable<OrdemServicoDto>> ObterPorClienteIdAsync(int clienteId)
        {
            try
            {
                var ordensServico = await _ordemServicoRepository.ObterPorClienteIdAsync(clienteId);
                return _mapper.Map<IEnumerable<OrdemServicoDto>>(ordensServico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter ordens de serviço por cliente: {ClienteId}", clienteId);
                throw;
            }
        }

        public async Task<IEnumerable<OrdemServicoDto>> ObterPorVeiculoIdAsync(int veiculoId)
        {
            try
            {
                var ordensServico = await _ordemServicoRepository.ObterPorVeiculoIdAsync(veiculoId);
                return _mapper.Map<IEnumerable<OrdemServicoDto>>(ordensServico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter ordens de serviço por veículo: {VeiculoId}", veiculoId);
                throw;
            }
        }

        public async Task<RespostaDto<List<OrdemServicoListDto>>> ObterPorStatusAsync(StatusOrdemServico status)
        {
            try
            {
                _logger.LogInformation("Buscando ordens de serviço por status: {Status}", status);

                var ordensServico = await _ordemServicoRepository.ObterPorStatusAsync(status);
                var ordensServicoDto = _mapper.Map<List<OrdemServicoListDto>>(ordensServico);
                
                return RespostaDto<List<OrdemServicoListDto>>.Sucesso(ordensServicoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter ordens de serviço por status: {Status}", status);
                return RespostaDto<List<OrdemServicoListDto>>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<OrdemServicoListDto>>> ObterPorNumeroAsync(string numero)
        {
            try
            {
                _logger.LogInformation("Buscando ordens de serviço por número: {Numero}", numero);

                var ordensServico = await _ordemServicoRepository.ObterPorFiltroAsync(new OrdemServicoFiltroDto
                {
                    Numero = numero
                });

                var ordensServicoDto = _mapper.Map<List<OrdemServicoListDto>>(ordensServico);
                
                return RespostaDto<List<OrdemServicoListDto>>.Sucesso(ordensServicoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter ordens de serviço por número: {Numero}", numero);
                return RespostaDto<List<OrdemServicoListDto>>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<OrdemServicoDto>> FecharAsync(int id, string observacoes = "")
        {
            try
            {
                var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(id);
                if (ordemServico == null)
                {
                    return new RespostaDto<OrdemServicoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Ordem de serviço não encontrada"
                    };
                }

                if (ordemServico.Status != StatusOrdemServico.Aberta)
                {
                    return new RespostaDto<OrdemServicoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Apenas ordens de serviço abertas podem ser fechadas"
                    };
                }

                ordemServico.Status = StatusOrdemServico.Fechada;
                ordemServico.DataFechamento = DateTime.Now;
                if (!string.IsNullOrEmpty(observacoes))
                {
                    ordemServico.Observacoes = observacoes;
                }

                _ordemServicoRepository.Atualizar(ordemServico);
                await _ordemServicoRepository.SalvarAsync();

                var ordemServicoDto = _mapper.Map<OrdemServicoDto>(ordemServico);
                return new RespostaDto<OrdemServicoDto>
                {
                    Sucesso = true,
                    Dados = ordemServicoDto,
                    Mensagem = "Ordem de serviço fechada com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fechar ordem de serviço: {Id}", id);
                return new RespostaDto<OrdemServicoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<OrdemServicoDto>> ReabrirAsync(int id, string motivo = "")
        {
            try
            {
                var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(id);
                if (ordemServico == null)
                {
                    return new RespostaDto<OrdemServicoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Ordem de serviço não encontrada"
                    };
                }

                if (ordemServico.Status != StatusOrdemServico.Fechada)
                {
                    return new RespostaDto<OrdemServicoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Apenas ordens de serviço fechadas podem ser reabertas"
                    };
                }

                ordemServico.Status = StatusOrdemServico.Aberta;
                ordemServico.DataFechamento = null;
                if (!string.IsNullOrEmpty(motivo))
                {
                    ordemServico.Observacoes += $"\n\nReaberta em {DateTime.Now:dd/MM/yyyy HH:mm}: {motivo}";
                }

                _ordemServicoRepository.Atualizar(ordemServico);
                await _ordemServicoRepository.SalvarAsync();

                var ordemServicoDto = _mapper.Map<OrdemServicoDto>(ordemServico);
                return new RespostaDto<OrdemServicoDto>
                {
                    Sucesso = true,
                    Dados = ordemServicoDto,
                    Mensagem = "Ordem de serviço reaberta com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao reabrir ordem de serviço: {Id}", id);
                return new RespostaDto<OrdemServicoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public async Task<RespostaDto<bool>> AdicionarItemAsync(int ordemServicoId, OrdemServicoItemCreateDto itemDto)
        {
            try
            {
                var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(ordemServicoId);
                if (ordemServico == null)
                {
                    return RespostaDto<bool>.Erro("Ordem de serviço não encontrada");
                }

                if (ordemServico.Status != StatusOrdemServico.Aberta)
                {
                    return RespostaDto<bool>.Erro("Apenas ordens de serviço abertas podem ter itens adicionados");
                }

                var produto = await _produtoRepository.ObterPorIdAsync(itemDto.ProdutoId);
                if (produto == null)
                {
                    return RespostaDto<bool>.Erro("Produto não encontrado");
                }

                var item = _mapper.Map<OrdemServicoItem>(itemDto);
                item.OrdemServicoId = ordemServicoId;
                item.ValorTotal = item.Quantidade * item.ValorUnitario;

                await _ordemServicoRepository.AdicionarItemAsync(item);
                await _ordemServicoRepository.SalvarAsync();

                // Recalcular valor total da ordem
                await RecalcularValorTotalAsync(ordemServicoId);

                return RespostaDto<bool>.Sucesso(true, "Item adicionado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar item à ordem de serviço: {OrdemServicoId}", ordemServicoId);
                return RespostaDto<bool>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<bool>> AtualizarItemAsync(int ordemServicoId, int itemId, OrdemServicoItemUpdateDto itemDto)
        {
            try
            {
                var item = await _ordemServicoRepository.ObterItemPorIdAsync(itemId);
                if (item == null || item.OrdemServicoId != ordemServicoId)
                {
                    return RespostaDto<bool>.Erro("Item não encontrado na ordem de serviço especificada");
                }

                var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(ordemServicoId);
                if (ordemServico?.Status != StatusOrdemServico.Aberta)
                {
                    return RespostaDto<bool>.Erro("Apenas itens de ordens de serviço abertas podem ser atualizados");
                }

                _mapper.Map(itemDto, item);
                item.ValorTotal = item.Quantidade * item.ValorUnitario;

                _ordemServicoRepository.AtualizarItem(item);
                await _ordemServicoRepository.SalvarAsync();

                // Recalcular valor total da ordem
                await RecalcularValorTotalAsync(ordemServicoId);

                return RespostaDto<bool>.Sucesso(true, "Item atualizado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar item: {ItemId}", itemId);
                return RespostaDto<bool>.Erro("Erro interno do servidor");
            }
        }



        public async Task<RespostaDto<decimal>> CalcularValorTotalAsync(int ordemServicoId)
        {
            try
            {
                var itens = await _ordemServicoRepository.ObterItensPorOrdemServicoIdAsync(ordemServicoId);
                var valorTotal = itens.Sum(i => i.ValorTotal);
                return RespostaDto<decimal>.Sucesso(valorTotal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular valor total da ordem de serviço: {OrdemServicoId}", ordemServicoId);
                return RespostaDto<decimal>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<bool>> RemoverItemAsync(int ordemServicoId, int itemId)
        {
            try
            {
                _logger.LogInformation("Removendo item da ordem de serviço. OrdemServicoId: {OrdemServicoId}, ItemId: {ItemId}", ordemServicoId, itemId);

                var item = await _ordemServicoRepository.ObterItemPorIdAsync(itemId);
                if (item == null || item.OrdemServicoId != ordemServicoId)
                {
                    return RespostaDto<bool>.Erro("Item não encontrado na ordem de serviço especificada");
                }

                // Verificar se a ordem de serviço permite remoção
                var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(ordemServicoId);
                if (ordemServico == null)
                {
                    return RespostaDto<bool>.Erro("Ordem de serviço não encontrada");
                }

                if (ordemServico.Status == StatusOrdemServico.Finalizada)
                {
                    return RespostaDto<bool>.Erro("Não é possível remover itens de uma ordem de serviço finalizada");
                }

                // Estornar estoque se necessário
                if (item.ProdutoId.HasValue)
                {
                    await _movimentacaoRepository.CriarAsync(new MovimentacaoEstoque
                    {
                        ProdutoId = item.ProdutoId.Value,
                        TipoMovimentacao = TipoMovimentacao.Entrada,
                        Quantidade = item.Quantidade,
                        Observacao = $"Estorno - Remoção de item da OS {ordemServicoId}",
                        DataMovimentacao = DateTime.Now
                    });
                }

                _ordemServicoRepository.RemoverItem(item);
                await _ordemServicoRepository.SalvarAsync();

                // Recalcular valor total
                await RecalcularValorTotalAsync(ordemServicoId);

                _logger.LogInformation("Item removido com sucesso. OrdemServicoId: {OrdemServicoId}, ItemId: {ItemId}", ordemServicoId, itemId);
                return RespostaDto<bool>.Sucesso(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover item da ordem de serviço. OrdemServicoId: {OrdemServicoId}, ItemId: {ItemId}", ordemServicoId, itemId);
                return RespostaDto<bool>.Erro("Erro interno do servidor");
            }
        }

        private async Task<decimal> CalcularValorTotalInternoAsync(int ordemServicoId)
        {
            try
            {
                var itens = await _ordemServicoRepository.ObterItensPorOrdemServicoIdAsync(ordemServicoId);
                return itens.Sum(i => i.ValorTotal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular valor total da ordem de serviço: {OrdemServicoId}", ordemServicoId);
                throw;
            }
        }

        public async Task<RespostaDto<OrdemServicoDto>> FecharOrdemServicoAsync(int id, string solucaoAplicada, decimal? desconto = null)
        {
            try
            {
                _logger.LogInformation("Fechando ordem de serviço. ID: {Id}", id);

                var ordemServico = await _ordemServicoRepository.ObterCompletaAsync(id);
                if (ordemServico == null)
                {
                    return RespostaDto<OrdemServicoDto>.Erro("Ordem de serviço não encontrada");
                }

                if (ordemServico.Status != StatusOrdemServico.Aberta)
                {
                    return RespostaDto<OrdemServicoDto>.Erro("Apenas ordens de serviço abertas podem ser fechadas");
                }

                ordemServico.Status = StatusOrdemServico.Finalizada;
                ordemServico.SolucaoAplicada = solucaoAplicada;
                ordemServico.DataFinalizacao = DateTime.Now;
                
                if (desconto.HasValue)
                {
                    ordemServico.Desconto = desconto.Value;
                    ordemServico.ValorTotal -= desconto.Value;
                }

                _ordemServicoRepository.Atualizar(ordemServico);
                await _ordemServicoRepository.SalvarAsync();

                var ordemServicoDto = _mapper.Map<OrdemServicoDto>(ordemServico);
                _logger.LogInformation("Ordem de serviço fechada com sucesso. ID: {Id}", id);
                
                return RespostaDto<OrdemServicoDto>.Sucesso(ordemServicoDto, "Ordem de serviço fechada com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fechar ordem de serviço. ID: {Id}", id);
                return RespostaDto<OrdemServicoDto>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<OrdemServicoDto>> ReabrirOrdemServicoAsync(int id)
        {
            try
            {
                _logger.LogInformation("Reabrindo ordem de serviço. ID: {Id}", id);

                var ordemServico = await _ordemServicoRepository.ObterCompletaAsync(id);
                if (ordemServico == null)
                {
                    return RespostaDto<OrdemServicoDto>.Erro("Ordem de serviço não encontrada");
                }

                if (ordemServico.Status != StatusOrdemServico.Finalizada)
                {
                    return RespostaDto<OrdemServicoDto>.Erro("Apenas ordens de serviço finalizadas podem ser reabertas");
                }

                ordemServico.Status = StatusOrdemServico.Aberta;
                ordemServico.SolucaoAplicada = null;
                ordemServico.DataFinalizacao = null;
                ordemServico.Desconto = 0;

                // Recalcular valor total
                await RecalcularValorTotalAsync(id);

                _ordemServicoRepository.Atualizar(ordemServico);
                await _ordemServicoRepository.SalvarAsync();

                var ordemServicoDto = _mapper.Map<OrdemServicoDto>(ordemServico);
                _logger.LogInformation("Ordem de serviço reaberta com sucesso. ID: {Id}", id);
                
                return RespostaDto<OrdemServicoDto>.Sucesso(ordemServicoDto, "Ordem de serviço reaberta com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao reabrir ordem de serviço. ID: {Id}", id);
                return RespostaDto<OrdemServicoDto>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<OrdemServicoListDto>>> ObterPorClienteAsync(int clienteId)
        {
            try
            {
                _logger.LogInformation("Buscando ordens de serviço por cliente: {ClienteId}", clienteId);

                var ordensServico = await _ordemServicoRepository.ObterPorClienteAsync(clienteId);
                var ordensServicoDto = _mapper.Map<List<OrdemServicoListDto>>(ordensServico);
                
                return RespostaDto<List<OrdemServicoListDto>>.Sucesso(ordensServicoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter ordens de serviço por cliente: {ClienteId}", clienteId);
                return RespostaDto<List<OrdemServicoListDto>>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<OrdemServicoListDto>>> ObterPorVeiculoAsync(int veiculoId)
        {
            try
            {
                _logger.LogInformation("Buscando ordens de serviço por veículo: {VeiculoId}", veiculoId);

                var ordensServico = await _ordemServicoRepository.ObterPorVeiculoAsync(veiculoId);
                var ordensServicoDto = _mapper.Map<List<OrdemServicoListDto>>(ordensServico);
                
                return RespostaDto<List<OrdemServicoListDto>>.Sucesso(ordensServicoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter ordens de serviço por veículo: {VeiculoId}", veiculoId);
                return RespostaDto<List<OrdemServicoListDto>>.Erro("Erro interno do servidor");
            }
        }

        private async Task RecalcularValorTotalAsync(int ordemServicoId)
        {
            try
            {
                var valorTotal = await CalcularValorTotalInternoAsync(ordemServicoId);
                var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(ordemServicoId);
                if (ordemServico != null)
                {
                    ordemServico.ValorTotal = valorTotal;
                    _ordemServicoRepository.Atualizar(ordemServico);
                    await _ordemServicoRepository.SalvarAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recalcular valor total da ordem de serviço: {OrdemServicoId}", ordemServicoId);
                throw;
            }
        }
    }
}