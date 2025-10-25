using Core.Entities;
using System.Linq.Expressions;

namespace Core.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);

        Task<IEnumerable<Product>> FindByConditionAsync(Expression<Func<Product, bool>> expression);

        Task<IQueryable<Product>> GetAllAsQueryableAsync();

    }
}
