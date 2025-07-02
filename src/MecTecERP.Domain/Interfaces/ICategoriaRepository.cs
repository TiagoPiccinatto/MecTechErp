using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Common;

namespace MecTecERP.Domain.Interfaces
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<bool> ExisteNomeAsync(string nome, int? idExcluir = null);
        Task<IEnumerable<Categoria>> ObterAtivasAsync();
        Task<IEnumerable<Categoria>> ObterPorFiltroAsync(
            string? nome = null,
            bool? ativa = null,
            int pagina = 1,
            int tamanhoPagina = 10,
            string ordenarPor = "Nome",
            bool ordenarDesc = false);
        Task<int> ContarPorFiltroAsync(
            string? nome = null,
            bool? ativa = null);
        Task<IEnumerable<Categoria>> ObterParaSelectAsync();
        Task<bool> PossuiProdutosAsync(int categoriaId); // Adicionado
    }
}