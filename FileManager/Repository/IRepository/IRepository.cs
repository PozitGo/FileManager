using System.Linq.Expressions;

namespace FileManager.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null);
        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null);
        Task CreateAsync(T entity);
        Task Delete(T entity);
    }
}
