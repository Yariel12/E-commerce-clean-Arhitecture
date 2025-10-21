using Core.Entities;

namespace Core.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(int userId);
        Task AddToCartAsync(int userId, int productId, int quantity);
        Task RemoveFromCartAsync(int userId, int productId);
        Task CheckoutAsync(int userId);  
    }
}
