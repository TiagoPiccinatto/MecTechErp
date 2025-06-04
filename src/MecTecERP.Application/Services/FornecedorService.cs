using AutoMapper;
using MecTecERP.Application.DTOs;
using MecTecERP.Application.Interfaces;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MecTecERP.Application.Services;

public class FornecedorService : IFornecedorService
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<FornecedorService> _logger;

    public FornecedorService(
        IFornecedorRepository fornecedorRepository,
        IMapper mapper,
        ILogger<FornecedorService> logger)
    {
        _fornecedorRepository = fornecedorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<RespostaDto<PaginacaoDto<FornecedorListDto>>> ObterTodosAsync(FiltroBaseDto filtro)
    {
        try
        {
            _logger.LogInformation("Obtendo fornecedores com filtros: {@Filtro}", filtro);

            var fornecedores = await _fornecedorRepository.ObterTodosAsync(
                filtro.Nome,
                filtro.Cnpj,
                filtro.Email,
                filtro.Cidade,
                filtro.Estado,
                filtro.Ativo,
                filtro.Pagina,
                filtro.ItensPorPagina,
                filtro.OrdenarPor,
                filtro.OrdemDecrescente);

            var total = await _fornecedorRepository.ContarAsync(
                filtro.Nome,
                filtro.Cnpj,
                filtro.Email,
                filtro.Cidade,
                filtro.Estado,
                filtro.Ativo);

            var fornecedoresDto = _mapper.Map<List<FornecedorListDto>>(fornecedores);

            var paginacao = new PaginacaoDto<FornecedorListDto>
            {
                Itens = fornecedoresDto,
                Total = total,
                Pagina = filtro.Pagina,
                ItensPorPagina = filtro.ItensPorPagina,
                TotalPaginas = (int)Math.Ceiling((double)total / filtro.ItensPorPagina)
            };

            return RespostaDto<PaginacaoDto<FornecedorListDto>>.Sucesso(paginacao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter fornecedores");
            return RespostaDto<PaginacaoDto<FornecedorListDto>>.Erro("Erro interno do servidor ao obter fornecedores");
        }
    }

    public async Task<RespostaDto<FornecedorDto>> ObterPorIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Obtendo fornecedor por ID: {Id}", id);

            var fornecedor = await _fornecedorRepository.ObterPorIdAsync(id);
            if (fornecedor == null)
                return RespostaDto<FornecedorDto>.Erro("Fornecedor não encontrado");

            var fornecedorDto = _mapper.Map<FornecedorDto>(fornecedor);
            return RespostaDto<FornecedorDto>.Sucesso(fornecedorDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter fornecedor por ID: {Id}", id);
            return RespostaDto<FornecedorDto>.Erro("Erro interno do servidor ao obter fornecedor");
        }
    }

    public async Task<RespostaDto<FornecedorDto>> ObterPorCnpjAsync(string cnpj)
    {
        try
        {
            _logger.LogInformation("Obtendo fornecedor por CNPJ: {Cnpj}", cnpj);

            var fornecedor = await _fornecedorRepository.ObterPorCnpjAsync(cnpj);
            if (fornecedor == null)
                return RespostaDto<FornecedorDto>.Erro("Fornecedor não encontrado");

            var fornecedorDto = _mapper.Map<FornecedorDto>(fornecedor);
            return RespostaDto<FornecedorDto>.Sucesso(fornecedorDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter fornecedor por CNPJ: {Cnpj}", cnpj);
            return RespostaDto<FornecedorDto>.Erro("Erro interno do servidor ao obter fornecedor");
        }
    }

    public async Task<RespostaDto<List<SelectItemDto>>> ObterParaSelectAsync()
    {
        try
        {
            _logger.LogInformation("Obtendo fornecedores para select");

            var fornecedores = await _fornecedorRepository.ObterAtivosAsync();
            var selectItems = fornecedores.Select(f => new SelectItemDto
            {
                Value = f.Id.ToString(),
                Text = f.Nome
            }).ToList();

            return RespostaDto<List<SelectItemDto>>.Sucesso(selectItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter fornecedores para select");
            return RespostaDto<List<SelectItemDto>>.Erro("Erro interno do servidor ao obter fornecedores");
        }
    }

    public async Task<RespostaDto<FornecedorDto>> CriarAsync(FornecedorCreateDto fornecedorDto)
    {
        try
        {
            _logger.LogInformation("Criando novo fornecedor: {@Fornecedor}", fornecedorDto);

            // Verificar se já existe fornecedor com o mesmo CNPJ
            if (await _fornecedorRepository.ExistePorCnpjAsync(fornecedorDto.Cnpj))
            {
                return RespostaDto<FornecedorDto>.Erro($"Já existe um fornecedor com o CNPJ '{fornecedorDto.Cnpj}'");
            }

            // Verificar se já existe fornecedor com o mesmo email
            if (!string.IsNullOrEmpty(fornecedorDto.Email) && 
                await _fornecedorRepository.ExistePorEmailAsync(fornecedorDto.Email))
            {
                return RespostaDto<FornecedorDto>.Erro($"Já existe um fornecedor com o email '{fornecedorDto.Email}'");
            }

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorDto);
            fornecedor.DataCriacao = DateTime.Now;
            fornecedor.Ativo = true;

            var fornecedorCriado = await _fornecedorRepository.AdicionarAsync(fornecedor);
            
            _logger.LogInformation("Fornecedor criado com sucesso. ID: {Id}", fornecedorCriado.Id);
            
            var resultado = _mapper.Map<FornecedorDto>(fornecedorCriado);
            return RespostaDto<FornecedorDto>.Sucesso(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar fornecedor: {@Fornecedor}", fornecedorDto);
            return RespostaDto<FornecedorDto>.Erro("Erro interno do servidor ao criar fornecedor");
        }
    }

    public async Task<RespostaDto<FornecedorDto>> AtualizarAsync(int id, FornecedorUpdateDto fornecedorDto)
    {
        try
        {
            _logger.LogInformation("Atualizando fornecedor: {@Fornecedor}", fornecedorDto);

            var fornecedor = await _fornecedorRepository.ObterPorIdAsync(id);
            if (fornecedor == null)
            {
                return RespostaDto<FornecedorDto>.Erro($"Fornecedor com ID {id} não encontrado");
            }

            // Verificar se já existe outro fornecedor com o mesmo CNPJ
            if (await _fornecedorRepository.ExistePorCnpjAsync(fornecedorDto.Cnpj, id))
            {
                return RespostaDto<FornecedorDto>.Erro($"Já existe outro fornecedor com o CNPJ '{fornecedorDto.Cnpj}'");
            }

            // Verificar se já existe outro fornecedor com o mesmo email
            if (!string.IsNullOrEmpty(fornecedorDto.Email) && 
                await _fornecedorRepository.ExistePorEmailAsync(fornecedorDto.Email, id))
            {
                return RespostaDto<FornecedorDto>.Erro($"Já existe outro fornecedor com o email '{fornecedorDto.Email}'");
            }

            _mapper.Map(fornecedorDto, fornecedor);
            fornecedor.DataAtualizacao = DateTime.Now;

            var fornecedorAtualizado = await _fornecedorRepository.AtualizarAsync(fornecedor);
            
            _logger.LogInformation("Fornecedor atualizado com sucesso. ID: {Id}", fornecedorAtualizado.Id);
            
            var resultado = _mapper.Map<FornecedorDto>(fornecedorAtualizado);
            return RespostaDto<FornecedorDto>.Sucesso(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar fornecedor: {@Fornecedor}", fornecedorDto);
            return RespostaDto<FornecedorDto>.Erro("Erro interno do servidor ao atualizar fornecedor");
        }
    }

    public async Task<RespostaDto> ExcluirAsync(int id)
    {
        try
        {
            _logger.LogInformation("Excluindo fornecedor. ID: {Id}", id);

            var fornecedor = await _fornecedorRepository.ObterPorIdAsync(id);
            if (fornecedor == null)
            {
                return RespostaDto.Erro("Fornecedor não encontrado");
            }

            // Verificar se o fornecedor possui produtos associados
            if (await _fornecedorRepository.PossuiProdutosAsync(id))
            {
                return RespostaDto.Erro("Não é possível excluir o fornecedor pois ele possui produtos associados");
            }

            var resultado = await _fornecedorRepository.ExcluirAsync(id);
            
            if (resultado)
            {
                _logger.LogInformation("Fornecedor excluído com sucesso. ID: {Id}", id);
                return RespostaDto.Sucesso("Fornecedor excluído com sucesso");
            }
            
            return RespostaDto.Erro("Falha ao excluir fornecedor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir fornecedor. ID: {Id}", id);
            return RespostaDto.Erro("Erro interno do servidor ao excluir fornecedor");
        }
    }

    public async Task<RespostaDto> AtivarDesativarAsync(int id)
    {
        try
        {
            _logger.LogInformation("Alterando status do fornecedor. ID: {Id}", id);

            var fornecedor = await _fornecedorRepository.ObterPorIdAsync(id);
            if (fornecedor == null)
            {
                return RespostaDto.Erro("Fornecedor não encontrado");
            }

            fornecedor.Ativo = !fornecedor.Ativo;
            fornecedor.DataAtualizacao = DateTime.Now;

            await _fornecedorRepository.AtualizarAsync(fornecedor);
            
            var status = fornecedor.Ativo ? "ativado" : "desativado";
            _logger.LogInformation("Fornecedor {Status} com sucesso. ID: {Id}", status, id);
            
            return RespostaDto.Sucesso($"Fornecedor {status} com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do fornecedor. ID: {Id}", id);
            return RespostaDto.Erro("Erro interno do servidor ao alterar status do fornecedor");
        }
    }

    public async Task<RespostaDto<bool>> ExisteAsync(int id)
    {
        try
        {
            var existe = await _fornecedorRepository.ExisteAsync(id);
            return RespostaDto<bool>.Sucesso(existe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do fornecedor. ID: {Id}", id);
            return RespostaDto<bool>.Erro("Erro interno do servidor ao verificar existência do fornecedor");
        }
    }

    public async Task<RespostaDto<bool>> CnpjExisteAsync(string cnpj, int? idExcluir = null)
    {
        try
        {
            var existe = await _fornecedorRepository.ExistePorCnpjAsync(cnpj, idExcluir);
            return RespostaDto<bool>.Sucesso(existe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do fornecedor por CNPJ: {Cnpj}", cnpj);
            return RespostaDto<bool>.Erro("Erro interno do servidor ao verificar CNPJ");
        }
    }

    public async Task<RespostaDto<bool>> EmailExisteAsync(string email, int? idExcluir = null)
    {
        try
        {
            var existe = await _fornecedorRepository.ExistePorEmailAsync(email, idExcluir);
            return RespostaDto<bool>.Sucesso(existe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do fornecedor por email: {Email}", email);
            return RespostaDto<bool>.Erro("Erro interno do servidor ao verificar email");
        }
    }

    public async Task<RespostaDto<ExportacaoDto>> ExportarAsync(FiltroBaseDto filtro, string formato = "excel")
    {
        try
        {
            _logger.LogInformation("Exportando fornecedores. Formato: {Formato}", formato);

            var fornecedores = await _fornecedorRepository.ObterTodosAsync(
                filtro.Nome,
                filtro.Cnpj,
                filtro.Email,
                filtro.Cidade,
                filtro.Estado,
                filtro.Ativo,
                1,
                int.MaxValue,
                filtro.OrdenarPor,
                filtro.OrdemDecrescente);

            var fornecedoresDto = _mapper.Map<List<FornecedorListDto>>(fornecedores);

            var nomeArquivo = $"fornecedores_{DateTime.Now:yyyyMMdd_HHmmss}";

            var exportacao = new ExportacaoDto
            {
                NomeArquivo = nomeArquivo,
                Formato = formato.ToUpper(),
                Dados = fornecedoresDto,
                DataGeracao = DateTime.Now
            };

            return RespostaDto<ExportacaoDto>.Sucesso(exportacao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exportar fornecedores");
            return RespostaDto<ExportacaoDto>.Erro("Erro interno do servidor ao exportar fornecedores");
        }
    }
}