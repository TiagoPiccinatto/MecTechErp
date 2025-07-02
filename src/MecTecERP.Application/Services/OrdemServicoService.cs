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

                // Usar o método do repositório que aplica filtros e paginação no banco
                // O filtro.OrdenarPor já está como "DataEntrada" por padrão na interface do repositório
                var ordensServico = await _ordemServicoRepository.ObterPorFiltroAsync(
                    filtro.Numero,
                    filtro.ClienteId,
                    filtro.VeiculoId,
                    filtro.Status,
                    filtro.DataEntradaInicio, // DTO já usa DataEntradaInicio
                    filtro.DataEntradaFim,   // DTO já usa DataEntradaFim
                    filtro.Pagina,
                    filtro.ItensPorPagina,
                    filtro.OrdenarPor,
                    filtro.OrdemDecrescente
                );
                
                var totalItens = await _ordemServicoRepository.ContarPorFiltroAsync(
                    filtro.Numero,
                    filtro.ClienteId,
                    filtro.VeiculoId,
                    filtro.Status,
                    filtro.DataEntradaInicio,
                    filtro.DataEntradaFim
                );

                // O mapeamento para OrdemServicoListDto já deve lidar com ClienteNome, VeiculoPlaca, etc.
                // Se o repositório ObterPorFiltroAsync não trouxer Cliente/Veiculo, precisaremos de outro método
                // ou ajustar o mapeamento para carregar essas informações se necessário para a lista.
                // O IOrdemServicoRepository.ObterPorFiltroAsync não especifica joins.
                // Vamos assumir por agora que o mapeamento pode precisar de ajustes ou o serviço enriquecerá o DTO.
                var ordensServicoDto = _mapper.Map<List<OrdemServicoListDto>>(ordensServico);
                
                // Para ClienteNome e VeiculoPlaca em OrdemServicoListDto, se não vierem do repo com join:
                // foreach (var osDto in ordensServicoDto) {
                //     var osEntidade = ordensServico.FirstOrDefault(os => os.Id == osDto.Id);
                //     if (osEntidade?.Cliente != null) osDto.ClienteNome = osEntidade.Cliente.NomeRazaoSocial;
                //     if (osEntidade?.Veiculo != null) osDto.VeiculoPlaca = osEntidade.Veiculo.Placa;
                // }


                var paginacao = new PaginacaoDto<OrdemServicoListDto>(ordensServicoDto, totalItens, filtro.Pagina, filtro.ItensPorPagina);

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
                // DataEntrada é definida com default na entidade ou pode ser explicitamente setada aqui se necessário.
                // O DTO OrdemServicoCreateDto já define o Status inicial como Protocolo.
                // ordemServico.Status = StatusOrdemServico.Protocolo; // Ajustado no DTO ou aqui.
                // ValorTotal será calculado com base nos itens.

                // Adicionar itens e fotos ANTES de adicionar a OS principal se o ID da OS for FK nos itens/fotos
                // Ou DEPOIS se os itens/fotos precisarem do ID da OS.
                // O BaseRepository.AdicionarAsync retorna a entidade com ID.

                var osAdicionada = await _ordemServicoRepository.AdicionarAsync(ordemServico);

                // Salvar Itens (Exemplo, precisa de OrdemServicoItemRepository ou lógica no OrdemServicoRepository)
                if (ordemServicoCreateDto.Itens != null && ordemServicoCreateDto.Itens.Any())
                {
                    foreach (var itemDto in ordemServicoCreateDto.Itens)
                    {
                        var item = _mapper.Map<OrdemServicoItem>(itemDto);
                        item.OrdemServicoId = osAdicionada.Id;
                        // await _ordemServicoItemRepository.AdicionarAsync(item); // Se existir repo de item
                        // Ou adicionar à coleção e salvar a OS agregada (se EF Core)
                        // Com Dapper, geralmente se insere separadamente.
                        // Por ora, o _ordemServicoRepository não tem AddItem.
                        // Esta lógica precisará ser implementada (ex: _ordemServicoRepository.AdicionarItemAsync(osAdicionada.Id, item))
                    }
                }
                // Salvar Fotos (similar aos itens)

                // Recalcular valor total após adicionar itens
                // osAdicionada.ValorTotal = CalcularValorTotalDosItens(osAdicionada.Id); // Método a ser criado
                // await _ordemServicoRepository.AtualizarAsync(osAdicionada);


                var ordemServicoDto = _mapper.Map<OrdemServicoDto>(osAdicionada);
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
                // DataAtualizacao é tratada pelo BaseRepository.AtualizarAsync

                await _ordemServicoRepository.AtualizarAsync(ordemServico);

                // Lógica para atualizar/adicionar/remover itens
                // Se itemDto.Id existe, atualiza. Se não, adiciona.
                // Itens não presentes no DTO que existem no banco podem ser removidos.
                // Exemplo simplificado:
                if (ordemServicoUpdateDto.Itens != null)
                {
                    // Obter itens existentes do banco para esta OS
                    // var itensExistentes = await _ordemServicoItemRepository.ObterPorOsIdAsync(id);
                    // foreach (var itemDto in ordemServicoUpdateDto.Itens)
                    // {
                    //    if (itemDto.Id > 0) // Atualizar
                    //        var itemDb = itensExistentes.FirstOrDefault(i => i.Id == itemDto.Id);
                    //        _mapper.Map(itemDto, itemDb);
                    //        // await _ordemServicoItemRepository.AtualizarAsync(itemDb);
                    //    else // Novo
                    //        var novoItem = _mapper.Map<OrdemServicoItem>(itemDto);
                    //        novoItem.OrdemServicoId = id;
                    //        // await _ordemServicoItemRepository.AdicionarAsync(novoItem);
                    // }
                    // // Remover itens que não estão no DTO mas estão no banco
                }

                // Lógica para adicionar/remover fotos
                if (ordemServicoUpdateDto.NovasFotos != null && ordemServicoUpdateDto.NovasFotos.Any())
                {
                    // foreach (var fotoDto in ordemServicoUpdateDto.NovasFotos) { /* adicionar foto */ }
                }
                if (ordemServicoUpdateDto.FotosParaRemoverIds != null && ordemServicoUpdateDto.FotosParaRemoverIds.Any())
                {
                    // foreach (var fotoId in ordemServicoUpdateDto.FotosParaRemoverIds) { /* remover foto */ }
                }

                // Recalcular valor total
                // ordemServico.ValorTotal = await CalcularValorTotalInternoAsync(id);
                // await _ordemServicoRepository.AtualizarAsync(ordemServico); // Salvar o total atualizado

                var osAtualizada = await _ordemServicoRepository.ObterCompletaAsync(id); // Re-buscar para obter estado atualizado com itens/fotos
                var ordemServicoDto = _mapper.Map<OrdemServicoDto>(osAtualizada);
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

                // Verificar se a ordem pode ser excluída (ex: apenas Protocolo ou Orcamento)
                if (ordemServico.Status != StatusOrdemServico.Protocolo && ordemServico.Status != StatusOrdemServico.Orcamento)
                {
                    return new RespostaDto<bool>
                    {
                        Sucesso = false,
                        Mensagem = "Apenas ordens de serviço em status Protocolo ou Orçamento podem ser excluídas."
                    };
                }
                // Adicionar lógica para remover itens e fotos associados, ou garantir que o DB faça isso em cascata.

                await _ordemServicoRepository.RemoverAsync(id);

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

                var ordensServico = await _ordemServicoRepository.ObterTodosComClienteVeiculoAsync(); // Este método pode ser ineficiente. Considerar um método de repo específico para select list.
                var selectList = ordensServico
                    .Where(os => os.Status != StatusOrdemServico.Finalizado && os.Status != StatusOrdemServico.Cancelado) // Ex: OS não finalizadas/canceladas
                    .Select(os => new SelectItemDto
                    {
                        Value = os.Id.ToString(),
                        Text = $"{os.Numero} - {os.Cliente?.NomeRazaoSocial} ({os.Veiculo?.Placa})" // Ajustado para NomeRazaoSocial
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

        public async Task<RespostaDto<OrdemServicoDto>> FecharOrdemServicoAsync(int id, string solucaoAplicada, decimal? desconto = null)
        {
            try
            {
                _logger.LogInformation("Fechando ordem de serviço. ID: {Id}", id);

                var ordemServico = await _ordemServicoRepository.ObterCompletaAsync(id); // Usar ObterCompleta para ter itens se necessário para validação
                if (ordemServico == null)
                {
                    return RespostaDto<OrdemServicoDto>.Erro("Ordem de serviço não encontrada");
                }

                // Validar se a OS pode ser fechada (ex: status Aprovado ou Execucao)
                if (ordemServico.Status != StatusOrdemServico.Aprovado && ordemServico.Status != StatusOrdemServico.Execucao)
                {
                    return RespostaDto<OrdemServicoDto>.Erro($"Apenas ordens de serviço com status Aprovado ou Execução podem ser finalizadas. Status atual: {ordemServico.Status}");
                }

                ordemServico.Status = StatusOrdemServico.Finalizado; // Novo status
                ordemServico.DiagnosticoTecnico = solucaoAplicada; // Campo correto
                ordemServico.DataConclusao = DateTime.UtcNow; // Campo correto

                if (desconto.HasValue)
                {
                    ordemServico.ValorDesconto = desconto.Value;
                    // O ValorTotal deve ser recalculado aqui ou ser uma propriedade calculada na entidade/DTO.
                    // Por ora, assumindo que o DTO e a entidade refletem a necessidade de recalcular.
                    // Se os itens já têm seus totais, o ValorTotal da OS seria a soma dos itens - ValorDesconto.
                    // Esta lógica de cálculo do ValorTotal precisa ser robusta.
                }
                // É importante recalcular o ValorTotal da OS aqui, considerando todos os itens e o desconto.
                // Ex: ordemServico.ValorTotal = await CalcularValorTotalInternoAsync(id) - (ordemServico.ValorDesconto);

                await _ordemServicoRepository.AtualizarAsync(ordemServico);

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

        public async Task<RespostaDto<OrdemServicoDto>> ReabrirOrdemServicoAsync(int id) // Assinatura da interface
        {
            try
            {
                _logger.LogInformation("Reabrindo ordem de serviço. ID: {Id}", id);

                var ordemServico = await _ordemServicoRepository.ObterCompletaAsync(id);
                if (ordemServico == null)
                {
                    return RespostaDto<OrdemServicoDto>.Erro("Ordem de serviço não encontrada");
                }

                // Permitir reabrir se estiver Finalizada ou Cancelada
                if (ordemServico.Status != StatusOrdemServico.Finalizado && ordemServico.Status != StatusOrdemServico.Cancelado)
                {
                    return RespostaDto<OrdemServicoDto>.Erro($"Apenas ordens de serviço Finalizadas ou Canceladas podem ser reabertas. Status atual: {ordemServico.Status}");
                }

                // Definir um status apropriado para reabertura, por exemplo, 'Aprovado' ou 'Execucao'
                // ou um novo status 'Reaberta'. Por simplicidade, vamos para 'Execucao'.
                ordemServico.Status = StatusOrdemServico.Execucao;
                ordemServico.DataConclusao = null;
                // Adicionar uma observação sobre a reabertura
                ordemServico.ObservacoesInternas = $"{ordemServico.ObservacoesInternas}\nReaberta em {DateTime.UtcNow:dd/MM/yyyy HH:mm}.".Trim();


                // Recalcular valor total se necessário, embora geralmente não mude ao reabrir.
                // Se o desconto foi aplicado ao fechar, pode ser necessário revertê-lo ou zerá-lo.
                // ordemServico.ValorDesconto = 0; // Exemplo
                // ordemServico.ValorTotal = await CalcularValorTotalInternoAsync(id) - ordemServico.ValorDesconto;


                await _ordemServicoRepository.AtualizarAsync(ordemServico);

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

        public async Task<RespostaDto<bool>> AdicionarItemAsync(int ordemServicoId, OrdemServicoItemCreateDto itemDto)
        {
            try
            {
                var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(ordemServicoId);
                if (ordemServico == null)
                {
                    return RespostaDto<bool>.Erro("Ordem de serviço não encontrada");
                }

                if (ordemServico.Status != StatusOrdemServico.Fechada)
                {
                    return RespostaDto<bool>.Erro("Apenas ordens de serviço fechadas podem ser reabertas");
                }

                ordemServico.Status = StatusOrdemServico.Aberta;
                ordemServico.DataFechamento = null;
                if (!string.IsNullOrEmpty(motivo))
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

                // Permitir adicionar itens se OS não estiver Finalizada ou Cancelada
                if (ordemServico.Status == StatusOrdemServico.Finalizado || ordemServico.Status == StatusOrdemServico.Cancelado)
                {
                    return RespostaDto<bool>.Erro($"Não é possível adicionar itens a uma OS com status {ordemServico.Status}.");
                }

                // Validação do ProdutoId no itemDto
                if (itemDto.ProdutoId.HasValue && itemDto.ProdutoId > 0)
                {
                    var produto = await _produtoRepository.ObterPorIdAsync(itemDto.ProdutoId.Value);
                    if (produto == null)
                    {
                        return RespostaDto<bool>.Erro("Produto não encontrado para o item.");
                    }
                } else if (itemDto.Tipo == TipoItemOrdemServico.Peca) // Se for Peça, ProdutoId é obrigatório
                {
                     return RespostaDto<bool>.Erro("ProdutoId é obrigatório para itens do tipo Peça.");
                }


                var item = _mapper.Map<OrdemServicoItem>(itemDto);
                item.OrdemServicoId = ordemServicoId;
                // ValorTotal deve ser calculado ou vir do DTO, mas geralmente é Qtd * VlrUnit
                // item.ValorTotal = item.Quantidade * item.ValorUnitario; // Se não vier do DTO

                // Assumindo que _ordemServicoRepository.AdicionarItemAsync existe e funciona.
                // await _ordemServicoRepository.AdicionarItemAsync(item);
                // await _ordemServicoRepository.SalvarAsync(); // Removido

                // Recalcular valor total da ordem
                // await RecalcularEAtualizarValorTotalOSAsync(ordemServicoId);

                return RespostaDto<bool>.Sucesso(true, "Item adicionado com sucesso (simulado - repo não implementado)");
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
                // var item = await _ordemServicoRepository.ObterItemPorIdAsync(itemId); // Precisa existir no repo
                // if (item == null || item.OrdemServicoId != ordemServicoId)
                // {
                //     return RespostaDto<bool>.Erro("Item não encontrado na ordem de serviço especificada");
                // }

                var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(ordemServicoId);
                if (ordemServico == null || ordemServico.Status == StatusOrdemServico.Finalizado || ordemServico.Status == StatusOrdemServico.Cancelado)
                {
                    return RespostaDto<bool>.Erro($"Não é possível atualizar itens de uma OS com status {ordemServico?.Status}.");
                }

                // Validação do ProdutoId no itemDto
                if (itemDto.ProdutoId.HasValue && itemDto.ProdutoId > 0)
                {
                    var produto = await _produtoRepository.ObterPorIdAsync(itemDto.ProdutoId.Value);
                    if (produto == null)
                    {
                        return RespostaDto<bool>.Erro("Produto não encontrado para o item.");
                    }
                } else if (itemDto.Tipo == TipoItemOrdemServico.Peca)
                {
                     return RespostaDto<bool>.Erro("ProdutoId é obrigatório para itens do tipo Peça.");
                }

                // _mapper.Map(itemDto, item);
                // item.ValorTotal = item.Quantidade * item.ValorUnitario;

                // _ordemServicoRepository.AtualizarItem(item); // Precisa existir no repo
                // await _ordemServicoRepository.SalvarAsync(); // Removido

                // Recalcular valor total da ordem
                // await RecalcularEAtualizarValorTotalOSAsync(ordemServicoId);

                return RespostaDto<bool>.Sucesso(true, "Item atualizado com sucesso (simulado - repo não implementado)");
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

                // Permitir remover itens se OS não estiver Finalizada ou Cancelada
                if (ordemServico.Status == StatusOrdemServico.Finalizado || ordemServico.Status == StatusOrdemServico.Cancelado)
                {
                    return RespostaDto<bool>.Erro($"Não é possível remover itens de uma OS com status {ordemServico.Status}.");
                }

                // Estornar estoque se necessário (a lógica de estorno parece ok, mas depende de CriarAsync no repo de movimentação)
                if (item.ProdutoId.HasValue && item.ProdutoId.Value > 0) // Adicionado > 0
                {
                    // Criar MovimentacaoEstoque e registrar
                    var movimentacaoEstorno = new MovimentacaoEstoque
                    {
                        ProdutoId = item.ProdutoId.Value,
                        Tipo = TipoMovimentacaoEstoque.Entrada, // Estorno é uma entrada
                        Quantidade = item.Quantidade,
                        Observacoes = $"Estorno - Remoção de item da OS {ordemServico.Numero ?? ordemServicoId.ToString()}",
                        DataMovimentacao = DateTime.UtcNow,
                        // EstoqueAnterior e EstoquePosterior seriam calculados pelo serviço de movimentação ou aqui.
                        // UsuarioCriacao seria atribuído pelo BaseRepository.
                    };
                    // await _movimentacaoRepository.AdicionarAsync(movimentacaoEstorno); // ou um método de serviço específico
                }

                // _ordemServicoRepository.RemoverItem(item); // Precisa existir no repo
                // await _ordemServicoRepository.SalvarAsync(); // Removido

                // Recalcular valor total
                // await RecalcularEAtualizarValorTotalOSAsync(ordemServicoId);

                _logger.LogInformation("Item removido com sucesso. OrdemServicoId: {OrdemServicoId}, ItemId: {ItemId}", ordemServicoId, itemId);
                return RespostaDto<bool>.Sucesso(true, "Item removido com sucesso (simulado - repo não implementado)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover item da ordem de serviço. OrdemServicoId: {OrdemServicoId}, ItemId: {ItemId}", ordemServicoId, itemId);
                return RespostaDto<bool>.Erro("Erro interno do servidor");
            }
        }

        private async Task RecalcularEAtualizarValorTotalOSAsync(int ordemServicoId) // Renomeado e com atualização
        {
            try
            {
                // var itens = await _ordemServicoRepository.ObterItensPorOrdemServicoIdAsync(ordemServicoId); // Precisa existir
                // decimal valorItens = itens.Sum(i => i.ValorTotal);

                var os = await _ordemServicoRepository.ObterPorIdAsync(ordemServicoId);
                if (os != null)
                {
                    // os.ValorServicos = itens.Where(i => i.Tipo == TipoItemOrdemServico.Servico).Sum(i => i.ValorTotal);
                    // os.ValorPecas = itens.Where(i => i.Tipo == TipoItemOrdemServico.Peca).Sum(i => i.ValorTotal);
                    // os.ValorTotal = os.ValorServicos + os.ValorPecas - os.ValorDesconto;
                    // await _ordemServicoRepository.AtualizarAsync(os);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recalcular e atualizar valor total da ordem de serviço: {OrdemServicoId}", ordemServicoId);
                throw; // Re-throw para que a transação (se houver) possa ser revertida.
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