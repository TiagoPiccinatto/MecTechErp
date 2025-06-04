using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Common;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Domain.Interfaces
{
    public interface IInventarioRepository : IRepository<Inventario>
    {
        Task<IEnumerable<Inventario>> ObterPorStatusAsync(StatusInventario status);
        Task<IEnumerable<Inventario>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<Inventario?> ObterComItensAsync(int id);
        Task<bool> ExisteInventarioAbertoAsync();
        Task<IEnumerable<Inventario>> ObterPorFiltroAsync(
            string? descricao = null,
            StatusInventario? status = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            int pagina = 1,
            int tamanhoPagina = 10,
            string ordenarPor = "DataCriacao",
            bool ordenarDesc = true);
        Task<int> ContarPorFiltroAsync(
            string? descricao = null,
            StatusInventario? status = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null);
        Task<IEnumerable<object>> ObterRelatorioInventarioAsync(int inventarioId);
        Task<IEnumerable<object>> ObterRelatorioDivergenciasAsync(int inventarioId);
    }
}