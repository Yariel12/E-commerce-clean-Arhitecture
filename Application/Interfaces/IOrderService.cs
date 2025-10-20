using Application.DTOs;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto?> GetByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetByUserIdAsync(int userId);
        Task AddAsync(OrderDto orderDto);
    }
}
