using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<Cart> GetCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return cart ?? new Cart { UserId = userId, Items = new List<CartItem>() };
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            var cart = await GetCartAsync(userId);
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new Exception("Producto no encontrado.");

            if (product.Stock < quantity)
                throw new Exception("No hay suficiente stock.");

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Product = product 
                });
            }

            await _cartRepository.UpdateCartAsync(userId, cart.Items);
        }

        public async Task RemoveFromCartAsync(int userId, int productId)
        {
            var cart = await GetCartAsync(userId);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                cart.Items.Remove(item);
                await _cartRepository.UpdateCartAsync(userId, cart.Items);
            }
        }

        public async Task CheckoutAsync(int userId)
        {
            var cart = await GetCartAsync(userId);

            if (!cart.Items.Any())
                throw new Exception("El carrito está vacío.");

            await _cartRepository.ClearCartAsync(userId);
        }
    }
}
