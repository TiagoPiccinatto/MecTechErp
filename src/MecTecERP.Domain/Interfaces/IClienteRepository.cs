using MecTecERP.Domain.Common;
using MecTecERP.Domain.Entities;

namespace MecTecERP.Domain.Interfaces
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        Task<bool> ExisteCpfCnpjAsync(string cpfCnpj, int? excludeId = null);
        Task<bool> ExisteEmailAsync(string email, int? excludeId = null);
        Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj);
        Task<Cliente?> ObterPorEmailAsync(string email);
        Task<IEnumerable<Cliente>> ObterAtivosAsync();
        Task<bool> PossuiOrdensServicoAsync(int clienteId);
        Task<IEnumerable<Cliente>> ObterPorFiltroAsync(
            string? nome = null,
            string? cpfCnpj = null,
            string? email = null,
            string? telefone = null,
            string? cidade = null,
            bool? ativo = null,
            int pagina = 1,
            int tamanhoPagina = 10,
            string ordenarPor = "Nome",
            bool ordenarDesc = false);
        Task<int> ContarPorFiltroAsync(
            string? nome = null,
            string? cpfCnpj = null,
            string? email = null,
            string? telefone = null,
            string? cidade = null,
            bool? ativo = null);
        Task<IEnumerable<Cliente>> ObterParaSelectAsync();
    }
}