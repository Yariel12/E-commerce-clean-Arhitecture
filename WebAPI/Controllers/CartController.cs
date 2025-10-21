using Application.Services;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/cart
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<Cart>> GetCart()
        {
            int userId = GetCurrentUserId(); 
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<ActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            int userId = GetCurrentUserId();
            await _cartService.AddToCartAsync(userId, request.ProductId, request.Quantity);
            return NoContent();
        }

        [Authorize]
        [HttpPost("remove")]
        public async Task<ActionResult> RemoveFromCart([FromBody] RemoveFromCartRequest request)
        {
            int userId = GetCurrentUserId();
            await _cartService.RemoveFromCartAsync(userId, request.ProductId);
            return NoContent();
        }

        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout()
        {
            int userId = GetCurrentUserId();
            await _cartService.CheckoutAsync(userId);
            return NoContent();
        }

        private int GetCurrentUserId()
        {
            // Aquí depende de tu auth (JWT, Identity, etc.)
            // Por ejemplo, si tienes JWT: int.Parse(User.FindFirst("id").Value)
            return 1; // temporal para pruebas
        }
    }

    // DTOs para requests
    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class RemoveFromCartRequest
    {
        public int ProductId { get; set; }
    }
}
