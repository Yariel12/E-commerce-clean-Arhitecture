using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;

        public OrderItemService(IOrderItemRepository orderItemRepository, IMapper mapper)
        {
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public async Task<OrderItemDto?> GetByIdAsync(int id)
        {
            var item = await _orderItemRepository.GetByIdAsync(id);
            return _mapper.Map<OrderItemDto>(item);
        }

        public async Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(int orderId)
        {
            var items = await _orderItemRepository.GetByOrderIdAsync(orderId);
            return _mapper.Map<IEnumerable<OrderItemDto>>(items);
        }

        public async Task AddAsync(OrderItemDto orderItemDto)
        {
            var entity = _mapper.Map<OrderItem>(orderItemDto);
            await _orderItemRepository.AddAsync(entity);
        }

        public async Task UpdateAsync(OrderItemDto orderItemDto)
        {
            var entity = _mapper.Map<OrderItem>(orderItemDto);
            await _orderItemRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _orderItemRepository.DeleteAsync(id);
        }
    }
}
