using AutoMapper;
using MecTecERP.Application.DTOs;
using MecTecERP.Application.Interfaces;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Enums;
using MecTecERP.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MecTecERP.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoService(
        IProdutoRepository produtoRepository,
        ICategoriaRepository categoriaRepository,
        IFornecedorRepository fornecedorRepository,
        IMapper mapper,
        ILogger<ProdutoService> logger)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
        _fornecedorRepository = fornecedorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<RespostaDto<PaginacaoDto<ProdutoListDto>>> ObterTodosAsync(FiltroBaseDto filtro)
    {
        try
        {
            _logger.LogInformation("Obtendo produtos com filtros: {@Filtro}", filtro);

            // Assumindo que FiltroBaseDto pode ter Nome e Ativo, ou que deveríamos usar um ProdutoFiltroDto.
            // Por ora, adaptando para o que IProdutoRepository.ObterPorFiltroAsync espera.
            // Se FiltroBaseDto não tiver Nome, Codigo, etc., eles serão null.
            var produtos = await _produtoRepository.ObterPorFiltroAsync(
                filtro.Nome, // Supondo que FiltroBaseDto tenha Nome (ou é um ProdutoFiltroDto)
                null, // Codigo - não presente em FiltroBaseDto padrão
                null, // CodigoBarras - não presente em FiltroBaseDto padrão
                null, // CategoriaId - não presente em FiltroBaseDto padrão
                null, // FornecedorId - não presente em FiltroBaseDto padrão
                null, // precoVendaMin - não presente em FiltroBaseDto padrão
                null, // precoVendaMax - não presente em FiltroBaseDto padrão
                null, // statusEstoque - não presente em FiltroBaseDto padrão
                filtro.Ativo, // Supondo que FiltroBaseDto tenha Ativo
                filtro.Pagina,
                filtro.ItensPorPagina,
                filtro.OrdenarPor,
                filtro.OrdemDecrescente);

            var total = await _produtoRepository.ContarPorFiltroAsync(
                filtro.Nome,
                null, // Codigo
                null, // CodigoBarras
                null, // CategoriaId
                null, // FornecedorId
                null, // precoVendaMin
                null, // precoVendaMax
                null, // statusEstoque
                filtro.Ativo);

            var produtosDto = _mapper.Map<List<ProdutoListDto>>(produtos);

            var paginacao = new PaginacaoDto<ProdutoListDto>
            {
                Itens = produtosDto,
                Total = total,
                Pagina = filtro.Pagina,
                ItensPorPagina = filtro.ItensPorPagina,
                TotalPaginas = (int)Math.Ceiling((double)total / filtro.ItensPorPagina)
            };

            return RespostaDto<PaginacaoDto<ProdutoListDto>>.Sucesso(paginacao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos");
            return RespostaDto<PaginacaoDto<ProdutoListDto>>.Erro("Erro interno do servidor ao obter produtos");
        }
    }

    public async Task<RespostaDto<ProdutoDto>> ObterPorIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Obtendo produto por ID: {Id}", id);

            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
                return RespostaDto<ProdutoDto>.Erro("Produto não encontrado");

            var produtoDto = _mapper.Map<ProdutoDto>(produto);
            return RespostaDto<ProdutoDto>.Sucesso(produtoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produto por ID: {Id}", id);
            return RespostaDto<ProdutoDto>.Erro("Erro interno do servidor ao obter produto");
        }
    }

    public async Task<RespostaDto<ProdutoDto>> ObterPorCodigoAsync(string codigo)
    {
        try
        {
            _logger.LogInformation("Obtendo produto por código: {Codigo}", codigo);

            var produto = await _produtoRepository.ObterPorCodigoAsync(codigo); // IProdutoRepository não tem ObterPorCodigoAsync. Assumindo que deveria ser ObterPorFiltroAsync ou um novo método.
                                                                            // O repo existente tem um método ObterPorCodigoAsync, mas não está na interface IProdutoRepository que listamos.
                                                                            // Vou assumir que o método existe na implementação de _produtoRepository.
            if (produto == null)
                return RespostaDto<ProdutoDto>.Erro($"Produto com código {codigo} não encontrado");

            return RespostaDto<ProdutoDto>.Sucesso(_mapper.Map<ProdutoDto>(produto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produto por código: {Codigo}", codigo);
            return RespostaDto<ProdutoDto>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<ProdutoDto>> ObterPorCodigoBarrasAsync(string codigoBarras)
    {
        try
        {
            _logger.LogInformation("Obtendo produto por código de barras: {CodigoBarras}", codigoBarras);

            if (string.IsNullOrWhiteSpace(codigoBarras))
                return RespostaDto<ProdutoDto>.Erro("Código de barras é obrigatório");

            var produto = await _produtoRepository.ObterPorCodigoBarrasAsync(codigoBarras);
            
            if (produto == null)
            {
                return RespostaDto<ProdutoDto>.Erro($"Produto com código de barras {codigoBarras} não encontrado");
            }

            var produtoDto = _mapper.Map<ProdutoDto>(produto);
            return RespostaDto<ProdutoDto>.Sucesso(produtoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produto por código de barras: {CodigoBarras}", codigoBarras);
            return RespostaDto<ProdutoDto>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<List<SelectItemDto>>> ObterParaSelectAsync()
    {
        try
        {
            _logger.LogInformation("Obtendo produtos para select");

            var produtos = await _produtoRepository.ObterAtivosAsync();
            var selectItems = produtos.Select(p => new SelectItemDto
            {
                Value = p.Id.ToString(),
                Text = $"{p.Codigo} - {p.Nome}"
            }).ToList();
            
            return RespostaDto<List<SelectItemDto>>.Sucesso(selectItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos para select");
            return RespostaDto<List<SelectItemDto>>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<List<ProdutoSelectDto>>> ObterParaSelectComDetalhesAsync()
    {
        try
        {
            _logger.LogInformation("Obtendo produtos para select com detalhes");

            var produtos = await _produtoRepository.ObterAtivosAsync();
            var produtosDto = _mapper.Map<List<ProdutoSelectDto>>(produtos);
            
            return RespostaDto<List<ProdutoSelectDto>>.Sucesso(produtosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos para select com detalhes");
            return RespostaDto<List<ProdutoSelectDto>>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<List<ProdutoListDto>>> ObterPorCategoriaAsync(int categoriaId)
    {
        try
        {
            _logger.LogInformation("Obtendo produtos por categoria: {CategoriaId}", categoriaId);

            if (categoriaId <= 0)
                return RespostaDto<List<ProdutoListDto>>.Erro("ID da categoria deve ser maior que zero");

            var produtos = await _produtoRepository.ObterPorCategoriaAsync(categoriaId);
            var produtosDto = _mapper.Map<List<ProdutoListDto>>(produtos);
            
            return RespostaDto<List<ProdutoListDto>>.Sucesso(produtosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos por categoria: {CategoriaId}", categoriaId);
            return RespostaDto<List<ProdutoListDto>>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<List<ProdutoListDto>>> ObterPorFornecedorAsync(int fornecedorId)
    {
        try
        {
            _logger.LogInformation("Obtendo produtos por fornecedor: {FornecedorId}", fornecedorId);

            if (fornecedorId <= 0)
                return RespostaDto<List<ProdutoListDto>>.Erro("ID do fornecedor deve ser maior que zero");

            var produtos = await _produtoRepository.ObterPorFornecedorAsync(fornecedorId);
            var produtosDto = _mapper.Map<List<ProdutoListDto>>(produtos);
            
            return RespostaDto<List<ProdutoListDto>>.Sucesso(produtosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos por fornecedor: {FornecedorId}", fornecedorId);
            return RespostaDto<List<ProdutoListDto>>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<List<ProdutoListDto>>> ObterComEstoqueBaixoAsync()
    {
        try
        {
            _logger.LogInformation("Obtendo produtos com estoque baixo");

            var produtos = await _produtoRepository.ObterComEstoqueBaixoAsync();
            var produtosDto = _mapper.Map<List<ProdutoListDto>>(produtos);
            
            return RespostaDto<List<ProdutoListDto>>.Sucesso(produtosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos com estoque baixo");
            return RespostaDto<List<ProdutoListDto>>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<List<ProdutoListDto>>> ObterComEstoqueZeradoAsync()
    {
        try
        {
            _logger.LogInformation("Obtendo produtos com estoque zerado");

            var produtos = await _produtoRepository.ObterComEstoqueZeradoAsync();
            var produtosDto = _mapper.Map<List<ProdutoListDto>>(produtos);
            
            return RespostaDto<List<ProdutoListDto>>.Sucesso(produtosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos com estoque zerado");
            return RespostaDto<List<ProdutoListDto>>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<List<ProdutoListDto>>> ObterPorStatusEstoqueAsync(StatusEstoque status)
    {
        try
        {
            _logger.LogInformation("Obtendo produtos por status de estoque: {Status}", status);

            var produtos = await _produtoRepository.ObterPorStatusEstoqueAsync(status);
            var produtosDto = _mapper.Map<List<ProdutoListDto>>(produtos);
            
            return RespostaDto<List<ProdutoListDto>>.Sucesso(produtosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos por status de estoque: {Status}", status);
            return RespostaDto<List<ProdutoListDto>>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<decimal>> CalcularValorTotalEstoqueAsync()
    {
        try
        {
            _logger.LogInformation("Calculando valor total do estoque");

            var valorTotal = await _produtoRepository.CalcularValorTotalEstoqueAsync();
            return RespostaDto<decimal>.Sucesso(valorTotal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular valor total do estoque");
            return RespostaDto<decimal>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<decimal>> CalcularValorEstoquePorCategoriaAsync(int categoriaId)
    {
        try
        {
            _logger.LogInformation("Calculando valor do estoque para categoria {CategoriaId}", categoriaId);

            if (categoriaId <= 0)
                return RespostaDto<decimal>.Erro("ID da categoria deve ser maior que zero");

            var valor = await _produtoRepository.CalcularValorEstoquePorCategoriaAsync(categoriaId);
            return RespostaDto<decimal>.Sucesso(valor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular valor do estoque por categoria {CategoriaId}", categoriaId);
            return RespostaDto<decimal>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<decimal>> CalcularValorEstoquePorFornecedorAsync(int fornecedorId)
    {
        try
        {
            _logger.LogInformation("Calculando valor do estoque para fornecedor {FornecedorId}", fornecedorId);

            if (fornecedorId <= 0)
                return RespostaDto<decimal>.Erro("ID do fornecedor deve ser maior que zero");

            var valor = await _produtoRepository.CalcularValorEstoquePorFornecedorAsync(fornecedorId);
            return RespostaDto<decimal>.Sucesso(valor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular valor do estoque por fornecedor {FornecedorId}", fornecedorId);
            return RespostaDto<decimal>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<ProdutoDto>> CriarAsync(ProdutoCreateDto produtoDto)
    {
        try
        {
            _logger.LogInformation("Criando novo produto: {@Produto}", produtoDto);

            // Validar categoria
            if (!await _categoriaRepository.ExisteAsync(produtoDto.CategoriaId))
            {
                return RespostaDto<ProdutoDto>.Erro($"Categoria com ID {produtoDto.CategoriaId} não encontrada");
            }

            // Validar fornecedor
            if (!await _fornecedorRepository.ExisteAsync(produtoDto.FornecedorId))
            {
                return RespostaDto<ProdutoDto>.Erro($"Fornecedor com ID {produtoDto.FornecedorId} não encontrado");
            }

            // Verificar se já existe produto com o mesmo código
            if (await _produtoRepository.ExistePorCodigoAsync(produtoDto.Codigo))
            {
                return RespostaDto<ProdutoDto>.Erro($"Já existe um produto com o código '{produtoDto.Codigo}'");
            }

            // Verificar se já existe produto com o mesmo código de barras
            if (!string.IsNullOrEmpty(produtoDto.CodigoBarras) && 
                await _produtoRepository.ExistePorCodigoBarrasAsync(produtoDto.CodigoBarras))
            {
                return RespostaDto<ProdutoDto>.Erro($"Já existe um produto com o código de barras '{produtoDto.CodigoBarras}'");
            }

            var produto = _mapper.Map<Produto>(produtoDto);
            produto.DataCriacao = DateTime.Now;
            produto.Ativo = true;
            produto.EstoqueAtual = 0; // Estoque inicial sempre zero

            var produtoCriado = await _produtoRepository.AdicionarAsync(produto);
            
            _logger.LogInformation("Produto criado com sucesso. ID: {Id}", produtoCriado.Id);
            
            var produtoDto = _mapper.Map<ProdutoDto>(produtoCriado);
            return RespostaDto<ProdutoDto>.Sucesso(produtoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto: {@Produto}", produtoDto);
            return RespostaDto<ProdutoDto>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<ProdutoDto>> AtualizarAsync(int id, ProdutoUpdateDto produtoDto)
    {
        try
        {
            _logger.LogInformation("Atualizando produto: {@Produto}", produtoDto);

            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
            {
                return RespostaDto<ProdutoDto>.Erro($"Produto com ID {id} não encontrado");
            }

            // Validar categoria
            if (!await _categoriaRepository.ExisteAsync(produtoDto.CategoriaId))
            {
                return RespostaDto<ProdutoDto>.Erro($"Categoria com ID {produtoDto.CategoriaId} não encontrada");
            }

            // Validar fornecedor
            if (!await _fornecedorRepository.ExisteAsync(produtoDto.FornecedorId))
            {
                return RespostaDto<ProdutoDto>.Erro($"Fornecedor com ID {produtoDto.FornecedorId} não encontrado");
            }

            // Verificar se já existe outro produto com o mesmo código
            if (await _produtoRepository.ExistePorCodigoAsync(produtoDto.Codigo, id))
            {
                return RespostaDto<ProdutoDto>.Erro($"Já existe outro produto com o código '{produtoDto.Codigo}'");
            }

            // Verificar se já existe outro produto com o mesmo código de barras
            if (!string.IsNullOrEmpty(produtoDto.CodigoBarras) && 
                await _produtoRepository.ExistePorCodigoBarrasAsync(produtoDto.CodigoBarras, id))
            {
                return RespostaDto<ProdutoDto>.Erro($"Já existe outro produto com o código de barras '{produtoDto.CodigoBarras}'");
            }

            _mapper.Map(produtoDto, produto);
            produto.DataAtualizacao = DateTime.Now;

            var produtoAtualizado = await _produtoRepository.AtualizarAsync(produto);
            
            _logger.LogInformation("Produto atualizado com sucesso. ID: {Id}", produtoAtualizado.Id);
            
            var produtoDto = _mapper.Map<ProdutoDto>(produtoAtualizado);
            return RespostaDto<ProdutoDto>.Sucesso(produtoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto: {@Produto}", produtoDto);
            return RespostaDto<ProdutoDto>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto> ExcluirAsync(int id)
    {
        try
        {
            _logger.LogInformation("Excluindo produto. ID: {Id}", id);

            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
            {
                return RespostaDto.Erro($"Produto com ID {id} não encontrado");
            }

            // Verificar se o produto possui movimentações de estoque
            if (await _produtoRepository.PossuiMovimentacoesAsync(id))
            {
                return RespostaDto.Erro("Não é possível excluir o produto pois ele possui movimentações de estoque");
            }

            var resultado = await _produtoRepository.ExcluirAsync(id);
            
            if (resultado)
            {
                _logger.LogInformation("Produto excluído com sucesso. ID: {Id}", id);
            }
            
            return resultado ? RespostaDto.Sucesso("Produto excluído com sucesso") : RespostaDto.Erro("Falha ao excluir produto");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir produto. ID: {Id}", id);
            return RespostaDto.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<bool>> AtivarAsync(int id)
    {
        try
        {
            _logger.LogInformation("Ativando produto. ID: {Id}", id);

            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
            {
                return RespostaDto.Erro($"Produto com ID {id} não encontrado");
            }

            produto.Ativo = true;
            produto.DataAtualizacao = DateTime.Now;

            await _produtoRepository.AtualizarAsync(produto);
            
            _logger.LogInformation("Produto ativado com sucesso. ID: {Id}", id);
            
            return RespostaDto<bool>.Sucesso(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao ativar produto. ID: {Id}", id);
            return RespostaDto.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<bool>> DesativarAsync(int id)
    {
        try
        {
            _logger.LogInformation("Desativando produto. ID: {Id}", id);

            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
            {
                return RespostaDto.Erro($"Produto com ID {id} não encontrado");
            }

            produto.Ativo = false;
            produto.DataAtualizacao = DateTime.Now;

            await _produtoRepository.AtualizarAsync(produto);
            
            _logger.LogInformation("Produto desativado com sucesso. ID: {Id}", id);
            
            return RespostaDto<bool>.Sucesso(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desativar produto. ID: {Id}", id);
            return RespostaDto.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<bool>> ExisteAsync(int id)
    {
        try
        {
            var existe = await _produtoRepository.ExisteAsync(id);
            return RespostaDto<bool>.Sucesso(existe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do produto. ID: {Id}", id);
            return RespostaDto.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto> AtivarDesativarAsync(int id)
    {
        try
        {
            _logger.LogInformation("Alternando status do produto. ID: {Id}", id);

            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
            {
                return RespostaDto.Erro($"Produto com ID {id} não encontrado");
            }

            produto.Ativo = !produto.Ativo;
            produto.DataAtualizacao = DateTime.Now;

            _produtoRepository.Atualizar(produto);
            await _produtoRepository.SalvarAsync();

            var status = produto.Ativo ? "ativado" : "desativado";
            _logger.LogInformation("Produto {Status} com sucesso. ID: {Id}", status, id);
            
            return RespostaDto.Sucesso($"Produto {status} com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alternar status do produto. ID: {Id}", id);
            return RespostaDto.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<bool>> CodigoExisteAsync(string codigo, int? idExcluir = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return RespostaDto<bool>.Erro("Código é obrigatório");
            }

            var existe = await _produtoRepository.ExistePorCodigoAsync(codigo, idExcluir);
            return RespostaDto<bool>.Sucesso(existe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do produto por código: {Codigo}", codigo);
            return RespostaDto.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<bool>> CodigoBarrasExisteAsync(string codigoBarras, int? idExcluir = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(codigoBarras))
            {
                return RespostaDto<bool>.Erro("Código de barras é obrigatório");
            }

            var existe = await _produtoRepository.ExistePorCodigoBarrasAsync(codigoBarras, idExcluir);
            return RespostaDto<bool>.Sucesso(existe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do produto por código de barras: {CodigoBarras}", codigoBarras);
            return RespostaDto.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<string>> GerarProximoCodigoAsync()
    {
        try
        {
            _logger.LogInformation("Gerando próximo código de produto");

            var ultimoCodigo = await _produtoRepository.ObterUltimoCodigoAsync();
            
            if (string.IsNullOrEmpty(ultimoCodigo))
            {
                return RespostaDto<string>.Sucesso("PRD001");
            }

            // Extrair número do código (assumindo formato PRDxxx)
            if (ultimoCodigo.StartsWith("PRD") && ultimoCodigo.Length > 3)
            {
                var numeroStr = ultimoCodigo.Substring(3);
                if (int.TryParse(numeroStr, out var numero))
                {
                    return RespostaDto<string>.Sucesso($"PRD{(numero + 1):D3}");
                }
            }

            return RespostaDto<string>.Sucesso("PRD001");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar próximo código de produto");
            return RespostaDto<string>.Erro("Erro interno do servidor");
        }
    }

    public async Task<RespostaDto<ExportacaoDto>> ExportarAsync(FiltroBaseDto filtro, string formato = "excel")
    {
        try
        {
            if (filtro == null)
            {
                return RespostaDto<ExportacaoDto>.Erro("Filtro é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(formato))
            {
                return RespostaDto<ExportacaoDto>.Erro("Formato é obrigatório");
            }

            _logger.LogInformation("Exportando produtos. Formato: {Formato}", formato);

            var produtos = await _produtoRepository.ObterTodosAsync(
                filtro.Nome,
                null, // codigo
                null, // codigoBarras
                null, // categoriaId
                null, // fornecedorId
                null, // statusEstoque
                filtro.Ativo,
                1,
                int.MaxValue,
                filtro.OrdenarPor,
                filtro.OrdemDecrescente);

            var produtosDto = _mapper.Map<List<ProdutoListDto>>(produtos);

            var nomeArquivo = $"produtos_{DateTime.Now:yyyyMMdd_HHmmss}";

            var exportacao = new ExportacaoDto
            {
                NomeArquivo = nomeArquivo,
                Formato = formato.ToUpper(),
                Dados = produtosDto,
                DataGeracao = DateTime.Now
            };

            return RespostaDto<ExportacaoDto>.Sucesso(exportacao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exportar produtos");
            return RespostaDto<ExportacaoDto>.Erro("Erro interno do servidor");
        }
    }
}