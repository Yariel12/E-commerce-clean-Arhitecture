using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>()
                };

                _context.Carts.Add(cart);
            }

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
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartAsync(int userId, ICollection<CartItem> items)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = items.ToList()
                };
                _context.Carts.Add(cart);
            }
            else
            {
                var existingItems = cart.Items.ToList();
                foreach (var existingItem in existingItems)
                {
                    if (!items.Any(i => i.ProductId == existingItem.ProductId))
                    {
                        _context.CartItems.Remove(existingItem);
                    }
                }

                foreach (var newItem in items)
                {
                    var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == newItem.ProductId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity = newItem.Quantity;
                    }
                    else
                    {
                        cart.Items.Add(new CartItem
                        {
                            ProductId = newItem.ProductId,
                            Quantity = newItem.Quantity
                        });
                    }
                }

                _context.Carts.Update(cart);
            }

            await _context.SaveChangesAsync();
        }


        public async Task ClearCartAsync(int userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateCartAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

    }
}
