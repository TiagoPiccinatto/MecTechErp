using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Common;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Domain.Interfaces
{
    public interface IMovimentacaoEstoqueRepository : IRepository<MovimentacaoEstoque>
    {
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorProdutoAsync(int produtoId);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorInventarioAsync(int inventarioId);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorTipoAsync(TipoMovimentacao tipo);
        Task<decimal> CalcularSaldoProdutoAsync(int produtoId, DateTime? dataLimite = null);
        Task<IEnumerable<MovimentacaoEstoque>> ObterUltimasMovimentacoesAsync(int quantidade = 10);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorFiltroAsync(
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            int? produtoId = null,
            int? inventarioId = null,
            TipoMovimentacao? tipo = null,
            int pagina = 1,
            int tamanhoPagina = 10,
            string ordenarPor = "DataMovimentacao",
            bool ordenarDesc = true);
        Task<int> ContarPorFiltroAsync(
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            int? produtoId = null,
            int? inventarioId = null,
            TipoMovimentacao? tipo = null);
        Task<IEnumerable<object>> ObterRelatorioMovimentacaoAsync(
            DateTime dataInicio,
            DateTime dataFim,
            int? produtoId = null,
            int? categoriaId = null,
            TipoMovimentacao? tipo = null);
    }
}