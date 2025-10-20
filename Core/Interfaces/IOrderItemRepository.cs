using Core.Entities;

namespace Core.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<OrderItem?> GetByIdAsync(int id);
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
        Task AddAsync(OrderItem item);
        Task UpdateAsync(OrderItem item);
        Task DeleteAsync(int id);
    }
}
