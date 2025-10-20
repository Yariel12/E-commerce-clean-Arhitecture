using Application.DTOs;

namespace Application.Interfaces
{
    public interface IOrderItemService
    {
        Task<OrderItemDto?> GetByIdAsync(int id);
        Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(int orderId);
        Task AddAsync(OrderItemDto orderItemDto);
        Task UpdateAsync(OrderItemDto orderItemDto);
        Task DeleteAsync(int id);
    }
}
