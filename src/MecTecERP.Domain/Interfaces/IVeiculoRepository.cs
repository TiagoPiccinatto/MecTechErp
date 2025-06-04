using MecTecERP.Domain.Common;
using MecTecERP.Domain.Entities;

namespace MecTecERP.Domain.Interfaces
{
    public interface IVeiculoRepository : IRepository<Veiculo>
    {
        Task<bool> ExistePlacaAsync(string placa, int? excludeId = null);
        Task<Veiculo?> ObterPorPlacaAsync(string placa);
        Task<Veiculo?> ObterPorPlacaComClienteAsync(string placa);
        Task<Veiculo?> ObterComClienteAsync(int id);
        Task<IEnumerable<Veiculo>> ObterTodosComClienteAsync();
        Task<IEnumerable<Veiculo>> ObterAtivosAsync();
        Task<IEnumerable<Veiculo>> ObterAtivosComClienteAsync();
        Task<IEnumerable<Veiculo>> ObterPorClienteAsync(int clienteId);
        Task<bool> PossuiOrdensServicoAsync(int veiculoId);
        Task<IEnumerable<Veiculo>> ObterPorFiltroAsync(
            string? placa = null,
            string? modelo = null,
            string? marca = null,
            int? clienteId = null,
            bool? ativo = null,
            int pagina = 1,
            int tamanhoPagina = 10,
            string ordenarPor = "Placa",
            bool ordenarDesc = false);
        Task<int> ContarPorFiltroAsync(
            string? placa = null,
            string? modelo = null,
            string? marca = null,
            int? clienteId = null,
            bool? ativo = null);
        Task<IEnumerable<Veiculo>> ObterParaSelectAsync(int? clienteId = null);
    }
}