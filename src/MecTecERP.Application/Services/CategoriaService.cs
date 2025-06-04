using AutoMapper;
using MecTecERP.Application.DTOs;
using MecTecERP.Application.Interfaces;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MecTecERP.Application.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoriaService> _logger;

    public CategoriaService(
        ICategoriaRepository categoriaRepository,
        IMapper mapper,
        ILogger<CategoriaService> logger)
    {
        _categoriaRepository = categoriaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<RespostaDto<PaginacaoDto<CategoriaListDto>>> ObterTodosAsync(FiltroBaseDto filtro)
    {
        try
        {
            _logger.LogInformation("Obtendo categorias com filtros: {@Filtro}", filtro);

            var categorias = await _categoriaRepository.ObterTodosAsync(
                filtro.Nome,
                filtro.Ativo,
                filtro.Pagina,
                filtro.ItensPorPagina,
                filtro.OrdenarPor,
                filtro.OrdemDecrescente);

            var total = await _categoriaRepository.ContarAsync(filtro.Nome, filtro.Ativo);

            var categoriasDto = _mapper.Map<List<CategoriaListDto>>(categorias);

            var resultado = new PaginacaoDto<CategoriaListDto>
            {
                Itens = categoriasDto,
                Total = total,
                Pagina = filtro.Pagina,
                ItensPorPagina = filtro.ItensPorPagina,
                TotalPaginas = (int)Math.Ceiling((double)total / filtro.ItensPorPagina)
            };

            return RespostaDto<PaginacaoDto<CategoriaListDto>>.Sucesso(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter categorias");
            throw;
        }
    }

    public async Task<RespostaDto<CategoriaDto>> ObterPorIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Obtendo categoria por ID: {Id}", id);

            var categoria = await _categoriaRepository.ObterPorIdAsync(id);
            if (categoria == null)
            {
                return RespostaDto<CategoriaDto>.Erro("Categoria não encontrada");
            }

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
            return RespostaDto<CategoriaDto>.Sucesso(categoriaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter categoria por ID: {Id}", id);
            return RespostaDto<CategoriaDto>.Erro($"Erro interno: {ex.Message}");
        }
    }

    public async Task<RespostaDto<List<SelectItemDto>>> ObterParaSelectAsync()
    {
        try
        {
            _logger.LogInformation("Obtendo categorias para select");

            var categorias = await _categoriaRepository.ObterAtivasAsync();
            var selectItems = categorias.Select(c => new SelectItemDto
            {
                Value = c.Id.ToString(),
                Text = c.Nome
            }).ToList();

            return RespostaDto<List<SelectItemDto>>.Sucesso(selectItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter categorias para select");
            return RespostaDto<List<SelectItemDto>>.Erro($"Erro interno: {ex.Message}");
        }
    }

    public async Task<RespostaDto<CategoriaDto>> CriarAsync(CategoriaCreateDto categoriaDto)
    {
        try
        {
            _logger.LogInformation("Criando nova categoria: {@Categoria}", categoriaDto);

            // Verificar se já existe categoria com o mesmo nome
            if (await _categoriaRepository.ExistePorNomeAsync(categoriaDto.Nome))
            {
                return RespostaDto<CategoriaDto>.Erro($"Já existe uma categoria com o nome '{categoriaDto.Nome}'");
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);
            categoria.DataCriacao = DateTime.Now;
            categoria.Ativo = true;

            var categoriaCriada = await _categoriaRepository.AdicionarAsync(categoria);
            
            _logger.LogInformation("Categoria criada com sucesso. ID: {Id}", categoriaCriada.Id);
            
            var resultado = _mapper.Map<CategoriaDto>(categoriaCriada);
            return RespostaDto<CategoriaDto>.Sucesso(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar categoria: {@Categoria}", categoriaDto);
            return RespostaDto<CategoriaDto>.Erro($"Erro interno: {ex.Message}");
        }
    }

    public async Task<RespostaDto<CategoriaDto>> AtualizarAsync(int id, CategoriaUpdateDto categoriaDto)
    {
        try
        {
            _logger.LogInformation("Atualizando categoria: {@Categoria}", categoriaDto);

            var categoria = await _categoriaRepository.ObterPorIdAsync(id);
            if (categoria == null)
            {
                return RespostaDto<CategoriaDto>.Erro($"Categoria com ID {id} não encontrada");
            }

            // Verificar se já existe outra categoria com o mesmo nome
            if (await _categoriaRepository.ExistePorNomeAsync(categoriaDto.Nome, id))
            {
                return RespostaDto<CategoriaDto>.Erro($"Já existe outra categoria com o nome '{categoriaDto.Nome}'");
            }

            _mapper.Map(categoriaDto, categoria);
            categoria.DataAtualizacao = DateTime.Now;

            var categoriaAtualizada = await _categoriaRepository.AtualizarAsync(categoria);
            
            _logger.LogInformation("Categoria atualizada com sucesso. ID: {Id}", categoriaAtualizada.Id);
            
            var resultado = _mapper.Map<CategoriaDto>(categoriaAtualizada);
            return RespostaDto<CategoriaDto>.Sucesso(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar categoria: {@Categoria}", categoriaDto);
            return RespostaDto<CategoriaDto>.Erro($"Erro interno: {ex.Message}");
        }
    }

    public async Task<RespostaDto> ExcluirAsync(int id)
    {
        try
        {
            _logger.LogInformation("Excluindo categoria. ID: {Id}", id);

            var categoria = await _categoriaRepository.ObterPorIdAsync(id);
            if (categoria == null)
            {
                return RespostaDto.Erro("Categoria não encontrada");
            }

            // Verificar se a categoria possui produtos associados
            if (await _categoriaRepository.PossuiProdutosAsync(id))
            {
                return RespostaDto.Erro("Não é possível excluir a categoria pois ela possui produtos associados");
            }

            var resultado = await _categoriaRepository.ExcluirAsync(id);
            
            if (resultado)
            {
                _logger.LogInformation("Categoria excluída com sucesso. ID: {Id}", id);
                return RespostaDto.Sucesso("Categoria excluída com sucesso");
            }
            
            return RespostaDto.Erro("Falha ao excluir categoria");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir categoria. ID: {Id}", id);
            return RespostaDto.Erro($"Erro interno: {ex.Message}");
        }
    }

    public async Task<RespostaDto> AtivarDesativarAsync(int id)
    {
        try
        {
            _logger.LogInformation("Alterando status da categoria. ID: {Id}", id);

            var categoria = await _categoriaRepository.ObterPorIdAsync(id);
            if (categoria == null)
            {
                return RespostaDto.Erro("Categoria não encontrada");
            }

            categoria.Ativo = !categoria.Ativo;
            categoria.DataAtualizacao = DateTime.Now;

            await _categoriaRepository.AtualizarAsync(categoria);
            
            var status = categoria.Ativo ? "ativada" : "desativada";
            _logger.LogInformation("Categoria {Status} com sucesso. ID: {Id}", status, id);
            
            return RespostaDto.Sucesso($"Categoria {status} com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status da categoria. ID: {Id}", id);
            return RespostaDto.Erro($"Erro interno: {ex.Message}");
        }
    }

    public async Task<RespostaDto<bool>> ExisteAsync(int id)
    {
        try
        {
            var existe = await _categoriaRepository.ExisteAsync(id);
            return RespostaDto<bool>.Sucesso(existe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência da categoria. ID: {Id}", id);
            return RespostaDto<bool>.Erro($"Erro interno: {ex.Message}");
        }
    }

    public async Task<RespostaDto<bool>> NomeExisteAsync(string nome, int? idExcluir = null)
    {
        try
        {
            var existe = await _categoriaRepository.ExistePorNomeAsync(nome, idExcluir);
            return RespostaDto<bool>.Sucesso(existe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência da categoria por nome: {Nome}", nome);
            return RespostaDto<bool>.Erro($"Erro interno: {ex.Message}");
        }
    }

    public async Task<RespostaDto<ExportacaoDto>> ExportarAsync(FiltroBaseDto filtro, string formato = "excel")
    {
        try
        {
            _logger.LogInformation("Exportando categorias. Formato: {Formato}", formato);

            var categorias = await _categoriaRepository.ObterTodosAsync(
                filtro.Nome,
                filtro.Ativo,
                1,
                int.MaxValue,
                filtro.OrdenarPor,
                filtro.OrdemDecrescente);

            var categoriasDto = _mapper.Map<List<CategoriaListDto>>(categorias);

            var nomeArquivo = $"categorias_{DateTime.Now:yyyyMMdd_HHmmss}";

            var resultado = new ExportacaoDto
            {
                NomeArquivo = nomeArquivo,
                Formato = formato.ToUpper(),
                Dados = categoriasDto,
                DataGeracao = DateTime.Now
            };

            return RespostaDto<ExportacaoDto>.Sucesso(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exportar categorias");
            return RespostaDto<ExportacaoDto>.Erro($"Erro interno: {ex.Message}");
        }
    }
}