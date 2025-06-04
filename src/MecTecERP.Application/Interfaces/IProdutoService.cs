using MecTecERP.Application.DTOs;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.Interfaces;

public interface IProdutoService
{
    Task<RespostaDto<PaginacaoDto<ProdutoListDto>>> ObterTodosAsync(FiltroBaseDto filtro);
    Task<RespostaDto<ProdutoDto>> ObterPorIdAsync(int id);
    Task<RespostaDto<ProdutoDto>> CriarAsync(ProdutoCreateDto dto);
    Task<RespostaDto<ProdutoDto>> AtualizarAsync(int id, ProdutoUpdateDto dto);
    Task<RespostaDto> ExcluirAsync(int id);
    Task<RespostaDto> AtivarDesativarAsync(int id);
    Task<RespostaDto<List<SelectItemDto>>> ObterParaSelectAsync();
    Task<RespostaDto<List<ProdutoSelectDto>>> ObterParaSelectComDetalhesAsync();
    Task<RespostaDto<bool>> ExisteAsync(int id);
    Task<RespostaDto<bool>> CodigoExisteAsync(string codigo, int? idExcluir = null);
    Task<RespostaDto<bool>> CodigoBarrasExisteAsync(string codigoBarras, int? idExcluir = null);
    Task<RespostaDto<ProdutoDto>> ObterPorCodigoAsync(string codigo);
    Task<RespostaDto<ProdutoDto>> ObterPorCodigoBarrasAsync(string codigoBarras);
    Task<RespostaDto<List<ProdutoListDto>>> ObterPorCategoriaAsync(int categoriaId);
    Task<RespostaDto<List<ProdutoListDto>>> ObterPorFornecedorAsync(int fornecedorId);
    Task<RespostaDto<List<ProdutoListDto>>> ObterComEstoqueBaixoAsync();
    Task<RespostaDto<List<ProdutoListDto>>> ObterComEstoqueZeradoAsync();
    Task<RespostaDto<List<ProdutoListDto>>> ObterPorStatusEstoqueAsync(StatusEstoque status);
    Task<RespostaDto<decimal>> CalcularValorTotalEstoqueAsync();
    Task<RespostaDto<decimal>> CalcularValorEstoquePorCategoriaAsync(int categoriaId);
    Task<RespostaDto<decimal>> CalcularValorEstoquePorFornecedorAsync(int fornecedorId);
    Task<RespostaDto<ExportacaoDto>> ExportarAsync(FiltroBaseDto filtro, string formato = "excel");
    Task<RespostaDto<string>> GerarProximoCodigoAsync();
}