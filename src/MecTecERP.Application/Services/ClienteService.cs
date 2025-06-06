using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MecTecERP.Application.DTOs;
using MecTecERP.Application.Interfaces;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;

namespace MecTecERP.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IOrdemServicoRepository _ordemServicoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(
            IClienteRepository clienteRepository,
            IVeiculoRepository veiculoRepository,
            IOrdemServicoRepository ordemServicoRepository,
            IMapper mapper,
            ILogger<ClienteService> logger)
        {
            _clienteRepository = clienteRepository;
            _veiculoRepository = veiculoRepository;
            _ordemServicoRepository = ordemServicoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RespostaDto<PaginacaoDto<ClienteListDto>>> ObterTodosAsync(ClienteFiltroDto filtro)
        {
            try
            {
                var clientes = await _clienteRepository.ObterComFiltroAsync(
                    filtro.Nome,
                    filtro.CpfCnpj,
                    filtro.Email,
                    filtro.Telefone,
                    filtro.Cidade,
                    filtro.Ativo,
                    filtro.Pagina,
                    filtro.ItensPorPagina,
                    filtro.OrdenarPor,
                    filtro.OrdemDecrescente);

                var total = await _clienteRepository.ContarComFiltroAsync(
                    filtro.Nome,
                    filtro.CpfCnpj,
                    filtro.Email,
                    filtro.Telefone,
                    filtro.Cidade,
                    filtro.Ativo);

                var dtos = _mapper.Map<List<ClienteListDto>>(clientes);
                var paginacao = new PaginacaoDto<ClienteListDto>(dtos, total, filtro.Pagina, filtro.ItensPorPagina);

                return new RespostaDto<PaginacaoDto<ClienteListDto>>(true, "Clientes obtidos com sucesso", paginacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter clientes");
                return new RespostaDto<PaginacaoDto<ClienteListDto>>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<ClienteDto>> ObterPorIdAsync(int id)
        {
            try
            {
                var cliente = await _clienteRepository.ObterPorIdAsync(id);
                if (cliente == null)
                {
                    return new RespostaDto<ClienteDto>(false, "Cliente não encontrado");
                }
                
                var dto = _mapper.Map<ClienteDto>(cliente);
                return new RespostaDto<ClienteDto>(true, "Cliente obtido com sucesso", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter cliente por ID: {Id}", id);
                return new RespostaDto<ClienteDto>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<ClienteDto>> CriarAsync(ClienteCreateDto dto)
        {
            try
            {
                // Validar se CPF/CNPJ já existe
                if (!string.IsNullOrEmpty(dto.CpfCnpj))
                {
                    var existeCpfCnpj = await ExisteCpfCnpjAsync(dto.CpfCnpj);
                    if (existeCpfCnpj.Dados)
                    {
                        return new RespostaDto<ClienteDto>(false, "CPF/CNPJ já cadastrado");
                    }
                }

                // Validar se email já existe
                if (!string.IsNullOrEmpty(dto.Email))
                {
                    var existeEmail = await ExisteEmailAsync(dto.Email);
                    if (existeEmail.Dados)
                    {
                        return new RespostaDto<ClienteDto>(false, "Email já cadastrado");
                    }
                }

                var cliente = _mapper.Map<Cliente>(dto);
                cliente.DataCadastro = DateTime.Now;
                cliente.Ativo = true;
                
                var novoCliente = await _clienteRepository.AdicionarAsync(cliente);
                
                var resultado = _mapper.Map<ClienteDto>(novoCliente);
                _logger.LogInformation("Cliente criado com sucesso. ID: {Id}", novoCliente.Id);
                
                return new RespostaDto<ClienteDto>(true, "Cliente criado com sucesso", resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente");
                return new RespostaDto<ClienteDto>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<ClienteDto>> AtualizarAsync(int id, ClienteUpdateDto dto)
        {
            try
            {
                var cliente = await _clienteRepository.ObterPorIdAsync(id);
                if (cliente == null)
                {
                    return new RespostaDto<ClienteDto>(false, "Cliente não encontrado");
                }

                // Validar se CPF/CNPJ já existe (exceto para o próprio cliente)
                if (!string.IsNullOrEmpty(dto.CpfCnpj) && dto.CpfCnpj != cliente.CpfCnpj)
                {
                    var existeCpfCnpj = await ExisteCpfCnpjAsync(dto.CpfCnpj, id);
                    if (existeCpfCnpj.Dados)
                    {
                        return new RespostaDto<ClienteDto>(false, "CPF/CNPJ já cadastrado");
                    }
                }

                // Validar se email já existe (exceto para o próprio cliente)
                if (!string.IsNullOrEmpty(dto.Email) && dto.Email != cliente.Email)
                {
                    var existeEmail = await ExisteEmailAsync(dto.Email, id);
                    if (existeEmail.Dados)
                    {
                        return new RespostaDto<ClienteDto>(false, "Email já cadastrado");
                    }
                }

                _mapper.Map(dto, cliente);
                await _clienteRepository.AtualizarAsync(cliente);
                
                var resultado = _mapper.Map<ClienteDto>(cliente);
                return new RespostaDto<ClienteDto>(true, "Cliente atualizado com sucesso", resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente: {Id}", id);
                return new RespostaDto<ClienteDto>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<bool>> ExcluirAsync(int id)
        {
            try
            {
                _logger.LogInformation("Excluindo cliente: {Id}", id);
                
                var cliente = await _clienteRepository.ObterPorIdAsync(id);
                if (cliente == null)
                {
                    return RespostaDto<bool>.Erro("Cliente não encontrado");
                }

                // Verificar se cliente possui veículos ou ordens de serviço
                var possuiVeiculos = await _veiculoRepository.ExistePorClienteAsync(id);
                var possuiOrdens = await _ordemServicoRepository.ExistePorClienteAsync(id);

                if (possuiVeiculos || possuiOrdens)
                {
                    // Inativar ao invés de excluir
                    cliente.Ativo = false;
                    await _clienteRepository.AtualizarAsync(cliente);
                    return RespostaDto<bool>.Sucesso(true, "Cliente inativado com sucesso (possui vínculos)");
                }

                await _clienteRepository.ExcluirAsync(id);
                return RespostaDto<bool>.Sucesso(true, "Cliente excluído com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir cliente: {Id}", id);
                return RespostaDto<bool>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<bool>> ExisteCpfCnpjAsync(string cpfCnpj, int? clienteIdExcluir = null)
        {
            try
            {
                var existe = await _clienteRepository.ExisteCpfCnpjAsync(cpfCnpj, clienteIdExcluir);
                return new RespostaDto<bool>(true, "Verificação realizada com sucesso", existe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar CPF/CNPJ: {CpfCnpj}", cpfCnpj);
                return new RespostaDto<bool>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<bool>> ExisteEmailAsync(string email, int? clienteIdExcluir = null)
        {
            try
            {
                var existe = await _clienteRepository.ExisteEmailAsync(email, clienteIdExcluir);
                return new RespostaDto<bool>(true, "Verificação realizada com sucesso", existe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar email: {Email}", email);
                return new RespostaDto<bool>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<SelectItemDto>>> ObterSelectListAsync()
        {
            try
            {
                _logger.LogInformation("Obtendo lista de seleção de clientes");
                var clientes = await _clienteRepository.ObterSelectListAsync(true);
                return RespostaDto<List<SelectItemDto>>.Sucesso(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter select list de clientes");
                return RespostaDto<List<SelectItemDto>>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<ClienteListDto>>> ObterPorNomeAsync(string nome)
        {
            try
            {
                _logger.LogInformation("Obtendo clientes por nome: {Nome}", nome);
                
                if (string.IsNullOrWhiteSpace(nome))
                {
                    return RespostaDto<List<ClienteListDto>>.Erro("Nome é obrigatório");
                }

                var clientes = await _clienteRepository.ObterTodosAsync();
                var clientesFiltrados = clientes.Where(c => c.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase)).ToList();
                var dtos = _mapper.Map<List<ClienteListDto>>(clientesFiltrados);
                
                return RespostaDto<List<ClienteListDto>>.Sucesso(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter clientes por nome: {Nome}", nome);
                return RespostaDto<List<ClienteListDto>>.Erro("Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<VeiculoListDto>>> ObterVeiculosAsync(int clienteId)
        {
            try
            {
                var veiculos = await _veiculoRepository.ObterPorClienteAsync(clienteId);
                var dtos = _mapper.Map<List<VeiculoListDto>>(veiculos);
                return new RespostaDto<List<VeiculoListDto>>(true, "Veículos obtidos com sucesso", dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter veículos do cliente: {ClienteId}", clienteId);
                return new RespostaDto<List<VeiculoListDto>>(false, "Erro interno do servidor");
            }
        }

        public async Task<RespostaDto<List<OrdemServicoListDto>>> ObterOrdensServicoAsync(int clienteId)
        {
            try
            {
                var ordens = await _ordemServicoRepository.ObterPorClienteAsync(clienteId);
                var dtos = _mapper.Map<List<OrdemServicoListDto>>(ordens);
                return new RespostaDto<List<OrdemServicoListDto>>(true, "Ordens de serviço obtidas com sucesso", dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter ordens de serviço do cliente: {ClienteId}", clienteId);
                return new RespostaDto<List<OrdemServicoListDto>>(false, "Erro interno do servidor");
            }
        }
    }
}