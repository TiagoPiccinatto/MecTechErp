using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Common;

namespace MecTecERP.Domain.Interfaces
{
    public interface IFornecedorRepository : IRepository<Fornecedor>
    {
        Task<bool> ExisteCnpjAsync(string cnpj, int? idExcluir = null);
        Task<bool> ExisteEmailAsync(string email, int? idExcluir = null);
        Task<IEnumerable<Fornecedor>> ObterAtivosAsync();
        Task<IEnumerable<Fornecedor>> ObterPorFiltroAsync(
            string? nome = null,
            string? cnpj = null,
            string? email = null,
            string? cidade = null,
            string? estado = null,
            bool? ativo = null,
            int pagina = 1,
            int tamanhoPagina = 10,
            string ordenarPor = "Nome",
            bool ordenarDesc = false);
        Task<int> ContarPorFiltroAsync(
            string? nome = null,
            string? cnpj = null,
            string? email = null,
            string? cidade = null,
            string? estado = null,
            bool? ativo = null);
        Task<IEnumerable<Fornecedor>> ObterParaSelectAsync();
        Task<bool> PossuiProdutosAsync(int fornecedorId); // Adicionado
    }
}