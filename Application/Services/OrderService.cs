using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _orderRepository.GetByAsync(id);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task AddAsync(OrderDto orderDto)
        {
            // 🔹 Mapear el DTO a la entidad Order
            var order = _mapper.Map<Order>(orderDto);
            order.OrderDate = DateTime.UtcNow;

            decimal totalAmount = 0;

            // 🔹 Validar y completar los datos de los productos
            var cleanItems = new List<OrderItem>();
            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"El producto con ID {item.ProductId} no existe.");

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = item.Quantity,
                    Price = product.Price   
                };

                totalAmount += orderItem.Price * orderItem.Quantity;
                cleanItems.Add(orderItem);
            }

            order.OrderItems = cleanItems;

            order.TotalAmount = totalAmount;

            await _orderRepository.AddAsync(order);
        }
    }
}
