using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Common;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Domain.Interfaces
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<bool> ExisteCodigoAsync(string codigo, int? idExcluir = null);
        Task<bool> ExisteCodigoBarrasAsync(string codigoBarras, int? idExcluir = null);
        Task<string> ObterProximoCodigoAsync();
        Task<IEnumerable<Produto>> ObterAtivosAsync();
        Task<IEnumerable<Produto>> ObterPorCategoriaAsync(int categoriaId);
        Task<IEnumerable<Produto>> ObterPorFornecedorAsync(int fornecedorId);
        Task<IEnumerable<Produto>> ObterComEstoqueBaixoAsync();
        Task<IEnumerable<Produto>> ObterComEstoqueZeradoAsync();
        Task<IEnumerable<Produto>> ObterPorStatusEstoqueAsync(StatusEstoque status);
        Task<decimal> CalcularValorTotalEstoqueAsync();
        Task<decimal> CalcularValorEstoquePorCategoriaAsync(int categoriaId);
        Task<decimal> CalcularValorEstoquePorFornecedorAsync(int fornecedorId);
        Task<IEnumerable<Produto>> ObterPorFiltroAsync(
            string? nome = null,
            string? codigo = null,
            string? codigoBarras = null,
            int? categoriaId = null,
            int? fornecedorId = null,
            decimal? precoVendaMin = null,
            decimal? precoVendaMax = null,
            StatusEstoque? statusEstoque = null,
            bool? ativo = null,
            int pagina = 1,
            int tamanhoPagina = 10,
            string ordenarPor = "Nome",
            bool ordenarDesc = false);
        Task<int> ContarPorFiltroAsync(
            string? nome = null,
            string? codigo = null,
            string? codigoBarras = null,
            int? categoriaId = null,
            int? fornecedorId = null,
            decimal? precoVendaMin = null,
            decimal? precoVendaMax = null,
            StatusEstoque? statusEstoque = null,
            bool? ativo = null);
        Task<IEnumerable<Produto>> ObterParaSelectAsync();
    }
}