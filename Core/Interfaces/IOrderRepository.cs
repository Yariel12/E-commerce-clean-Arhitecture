using Core.Entities;

namespace Core.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetByAsync(int id);
        Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
        Task AddAsync(Order order);
    }
}
