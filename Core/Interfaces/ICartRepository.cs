using Core.Entities;

namespace Core.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task AddToCartAsync(int userId, int productId, int quantity);
        Task UpdateCartAsync(int userId, ICollection<CartItem> items); 
        Task ClearCartAsync(int userId);
    }
}
