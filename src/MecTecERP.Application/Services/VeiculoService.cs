using AutoMapper;
using Microsoft.Extensions.Logging;
using MecTecERP.Application.DTOs;
using MecTecERP.Application.Interfaces;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;

namespace MecTecERP.Application.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IOrdemServicoRepository _ordemServicoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<VeiculoService> _logger;

        public VeiculoService(
            IVeiculoRepository veiculoRepository,
            IClienteRepository clienteRepository,
            IOrdemServicoRepository ordemServicoRepository,
            IMapper mapper,
            ILogger<VeiculoService> logger)
        {
            _veiculoRepository = veiculoRepository;
            _clienteRepository = clienteRepository;
            _ordemServicoRepository = ordemServicoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RespostaDto<VeiculoDto>> ObterPorIdAsync(int id)
        {
            try
            {
                var veiculo = await _veiculoRepository.ObterComClienteAsync(id);
                if (veiculo == null)
                {
                    return RespostaDto<VeiculoDto>.Erro("Veículo não encontrado");
                }

                var veiculoDto = _mapper.Map<VeiculoDto>(veiculo);
                return RespostaDto<VeiculoDto>.Sucesso(veiculoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter veículo por ID: {Id}", id);
                return RespostaDto<VeiculoDto>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<PaginacaoDto<VeiculoListDto>>> ObterTodosAsync(VeiculoFiltroDto filtro)
        {
            try
            {
                // Usar o método do repositório que aplica filtros e paginação no banco
                var veiculos = await _veiculoRepository.ObterPorFiltroAsync(
                    filtro.Placa,
                    filtro.Modelo,
                    filtro.Marca,
                    filtro.ClienteId,
                    filtro.Ativo,
                    filtro.Pagina,
                    filtro.ItensPorPagina,
                    filtro.OrdenarPor, // Repositório já trata "ano" -> "anofabricacao", "cliente" -> "c.NomeRazaoSocial"
                    filtro.OrdemDecrescente);
                
                var totalItens = await _veiculoRepository.ContarPorFiltroAsync(
                    filtro.Placa,
                    filtro.Modelo,
                    filtro.Marca,
                    filtro.ClienteId,
                    filtro.Ativo);

                var veiculosDto = _mapper.Map<List<VeiculoListDto>>(veiculos);
                
                // O VeiculoListDto precisa ter AnoFabricacao e AnoModelo se quisermos exibi-los.
                // O filtro no DTO usa AnoFabricacaoInicio e AnoFabricacaoFim.
                // O repositório foi ajustado para ordenar por 'anofabricacao'.

                var paginacao = new PaginacaoDto<VeiculoListDto>(veiculosDto, totalItens, filtro.Pagina, filtro.ItensPorPagina);

                return RespostaDto<PaginacaoDto<VeiculoListDto>>.Sucesso(paginacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os veículos");
                return RespostaDto<PaginacaoDto<VeiculoListDto>>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<VeiculoDto>> CriarAsync(VeiculoCreateDto veiculoCreateDto)
        {
            try
            {
                // Verificar se o cliente existe
                var cliente = await _clienteRepository.ObterPorIdAsync(veiculoCreateDto.ClienteId);
                if (cliente == null)
                {
                    return RespostaDto<VeiculoDto>.Erro("Cliente não encontrado");
                }

                // Verificar se a placa já existe
                var veiculoExistente = await _veiculoRepository.ObterPorPlacaAsync(veiculoCreateDto.Placa);
                if (veiculoExistente != null)
                {
                    return RespostaDto<VeiculoDto>.Erro("Já existe um veículo com esta placa");
                }

                var veiculo = _mapper.Map<Veiculo>(veiculoCreateDto);
                // DataCriacao e Ativo são definidos pelo BaseRepository.AdicionarAsync
                // veiculo.DataCadastro = DateTime.Now; // Removido
                // veiculo.Ativo = true; // Removido

                var veiculoAdicionado = await _veiculoRepository.AdicionarAsync(veiculo);
                // await _veiculoRepository.SalvarAsync(); // Removido - Dapper executa imediatamente

                var veiculoDto = _mapper.Map<VeiculoDto>(veiculoAdicionado);
                return RespostaDto<VeiculoDto>.Sucesso(veiculoDto, "Veículo criado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar veículo");
                return RespostaDto<VeiculoDto>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<VeiculoDto>> AtualizarAsync(int id, VeiculoUpdateDto veiculoUpdateDto)
        {
            try
            {
                var veiculo = await _veiculoRepository.ObterPorIdAsync(id);
                if (veiculo == null)
                {
                    return RespostaDto<VeiculoDto>.Erro("Veículo não encontrado");
                }

                // Verificar se o cliente existe
                var cliente = await _clienteRepository.ObterPorIdAsync(veiculoUpdateDto.ClienteId);
                if (cliente == null)
                {
                    return RespostaDto<VeiculoDto>.Erro("Cliente não encontrado");
                }

                // Verificar se a placa já existe em outro veículo
                var veiculoExistente = await _veiculoRepository.ObterPorPlacaAsync(veiculoUpdateDto.Placa);
                if (veiculoExistente != null && veiculoExistente.Id != id)
                {
                    return RespostaDto<VeiculoDto>.Erro("Já existe um veículo com esta placa");
                }

                _mapper.Map(veiculoUpdateDto, veiculo);
                // DataAtualizacao é definido pelo BaseRepository.AtualizarAsync
                // veiculo.DataAtualizacao = DateTime.Now; // Removido

                await _veiculoRepository.AtualizarAsync(veiculo); // Usar AtualizarAsync
                // await _veiculoRepository.SalvarAsync(); // Removido

                var veiculoDto = _mapper.Map<VeiculoDto>(veiculo); // Mapear após AtualizarAsync pode ser útil se AtualizarAsync modificar a entidade
                return RespostaDto<VeiculoDto>.Sucesso(veiculoDto, "Veículo atualizado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar veículo: {Id}", id);
                return RespostaDto<VeiculoDto>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<bool>> ExcluirAsync(int id)
        {
            try
            {
                var veiculo = await _veiculoRepository.ObterPorIdAsync(id);
                if (veiculo == null)
                {
                    return RespostaDto<bool>.Erro("Veículo não encontrado");
                }

                // Verificar se existem ordens de serviço vinculadas
                var ordensServico = await _ordemServicoRepository.ObterPorVeiculoIdAsync(id);
                if (ordensServico.Any())
                {
                    return RespostaDto<bool>.Erro("Não é possível excluir o veículo pois existem ordens de serviço vinculadas");
                }

                await _veiculoRepository.RemoverAsync(id); // Usar RemoverAsync(id)
                // await _veiculoRepository.SalvarAsync(); // Removido

                return RespostaDto<bool>.Sucesso(true, "Veículo excluído com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir veículo: {Id}", id);
                return RespostaDto<bool>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<bool>> ExistePlacaAsync(string placa, int? idExcluir = null)
        {
            try
            {
                var veiculo = await _veiculoRepository.ObterPorPlacaAsync(placa);
                if (veiculo == null) 
                    return RespostaDto<bool>.Sucesso(false);
                
                var existe = idExcluir == null || veiculo.Id != idExcluir;
                return RespostaDto<bool>.Sucesso(existe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se placa existe: {Placa}", placa);
                return RespostaDto<bool>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<SelectItemDto>>> ObterSelectListAsync()
        {
            try
            {
                var veiculos = await _veiculoRepository.ObterTodosComClienteAsync();
                var selectList = veiculos
                    .Where(v => v.Ativo)
                    .Select(v => new SelectItemDto
                    {
                        Value = v.Id.ToString(),
                        Text = $"{v.Placa} - {v.Marca} {v.Modelo} ({v.Cliente?.NomeRazaoSocial})" // Ajustado para NomeRazaoSocial
                    })
                    .OrderBy(x => x.Text)
                    .ToList();
                    
                return RespostaDto<List<SelectItemDto>>.Sucesso(selectList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter select list de veículos");
                return RespostaDto<List<SelectItemDto>>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<VeiculoListDto>>> ObterPorClienteAsync(int clienteId)
        {
            try
            {
                var veiculos = await _veiculoRepository.ObterPorClienteIdAsync(clienteId);
                var veiculosDto = _mapper.Map<List<VeiculoListDto>>(veiculos);
                return RespostaDto<List<VeiculoListDto>>.Sucesso(veiculosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter veículos por cliente: {ClienteId}", clienteId);
                return RespostaDto<List<VeiculoListDto>>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<VeiculoListDto>>> ObterPorPlacaAsync(string placa)
        {
            try
            {
                var veiculo = await _veiculoRepository.ObterPorPlacaAsync(placa);
                var veiculosDto = veiculo != null ? 
                    new List<VeiculoListDto> { _mapper.Map<VeiculoListDto>(veiculo) } : 
                    new List<VeiculoListDto>();
                return RespostaDto<List<VeiculoListDto>>.Sucesso(veiculosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter veículo por placa: {Placa}", placa);
                return RespostaDto<List<VeiculoListDto>>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<OrdemServicoListDto>>> ObterOrdensServicoAsync(int veiculoId)
        {
            try
            {
                var ordensServico = await _ordemServicoRepository.ObterPorVeiculoIdAsync(veiculoId);
                var ordensServicoDto = _mapper.Map<List<OrdemServicoListDto>>(ordensServico);
                return RespostaDto<List<OrdemServicoListDto>>.Sucesso(ordensServicoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter ordens de serviço do veículo: {VeiculoId}", veiculoId);
                return RespostaDto<List<OrdemServicoListDto>>.Erro("Erro interno do servidor");
            }
        }
    }
}