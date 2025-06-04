using System.Linq.Expressions;

namespace MecTecERP.Domain.Common
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> ObterPorIdAsync(int id);
        Task<IEnumerable<T>> ObterTodosAsync();
        Task<IEnumerable<T>> ObterPorCondicaoAsync(Expression<Func<T, bool>> condicao);
        Task<T?> ObterPrimeiroAsync(Expression<Func<T, bool>> condicao);
        Task<bool> ExisteAsync(Expression<Func<T, bool>> condicao);
        Task<int> ContarAsync(Expression<Func<T, bool>>? condicao = null);
        Task<T> AdicionarAsync(T entidade);
        Task<IEnumerable<T>> AdicionarVariosAsync(IEnumerable<T> entidades);
        Task AtualizarAsync(T entidade);
        Task AtualizarVariosAsync(IEnumerable<T> entidades);
        Task RemoverAsync(T entidade);
        Task RemoverAsync(int id);
        Task RemoverVariosAsync(IEnumerable<T> entidades);
        Task<bool> SalvarMudancasAsync();
    }
}