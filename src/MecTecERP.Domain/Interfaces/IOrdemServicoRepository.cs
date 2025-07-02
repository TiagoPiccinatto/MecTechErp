using MecTecERP.Domain.Common;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Domain.Interfaces
{
    public interface IOrdemServicoRepository : IRepository<OrdemServico>
    {
        Task<string> ObterProximoNumeroAsync();
        Task<OrdemServico?> ObterCompletaAsync(int id);
        Task<IEnumerable<OrdemServico>> ObterTodasCompletasAsync();
        Task<IEnumerable<OrdemServico>> ObterPorStatusAsync(StatusOrdemServico status);
        Task<IEnumerable<OrdemServico>> ObterPorClienteAsync(int clienteId);
        Task<IEnumerable<OrdemServico>> ObterPorVeiculoAsync(int veiculoId);
        Task<IEnumerable<OrdemServico>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<OrdemServico>> ObterPorFiltroAsync(
            string? numero = null,
            int? clienteId = null,
            int? veiculoId = null,
            StatusOrdemServico? status = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            int pagina = 1,
            int tamanhoPagina = 10,
            string ordenarPor = "DataEntrada", // Alterado de DataAbertura para DataEntrada
            bool ordenarDesc = true);
        Task<int> ContarPorFiltroAsync(
            string? numero = null,
            int? clienteId = null,
            int? veiculoId = null,
            StatusOrdemServico? status = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null);
        Task<decimal> ObterFaturamentoPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<int> ObterQuantidadePorStatusAsync(StatusOrdemServico status);
        Task<IEnumerable<OrdemServico>> ObterMaisRecentesAsync(int quantidade = 10);
    }
}