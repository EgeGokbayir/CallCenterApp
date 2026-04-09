using System.Linq.Expressions;

namespace Utilities.Generics
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T entity);
        Task CreateManyAsync(IEnumerable<T> entities);

        Task<T?> ReadByKey(object entityKey);
        Task<T?> FindFirstAsync(Expression<Func<T, bool>>? expression = null);
        Task<IEnumerable<T>> ReadManyAsync(Expression<Func<T, bool>>? expression = null, params string[] includes);

        Task UpdateAsync(T entity);
        Task UpdateManyAsync(IEnumerable<T> entities);
        Task UpdateManyAsync(Expression<Func<T, bool>>? expression = null);

        Task DeleteAsync(T entity);
        Task DeleteManyAsync(IEnumerable<T> entities);
        Task DeleteManyAsync(Expression<Func<T, bool>>? expression = null);

        Task<int> CountAsync(Expression<Func<T, bool>>? expression = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>>? expression = null);
    }
}