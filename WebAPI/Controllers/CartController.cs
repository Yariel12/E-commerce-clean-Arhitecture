using Application.Services;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // ✅ GET: api/cart
        [HttpGet]
        public async Task<ActionResult<Cart>> GetCart()
        {
            int userId = GetCurrentUserId();
            var cart = await _cartService.GetCartAsync(userId);

            if (cart == null)
                return NotFound(new { message = "El carrito está vacío o no existe." });

            return Ok(cart);
        }

        // ✅ POST: api/cart/add
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                await _cartService.AddToCartAsync(userId, request.ProductId, request.Quantity);
                return Ok(new { message = "Producto agregado al carrito correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ✅ POST: api/cart/remove
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                await _cartService.RemoveFromCartAsync(userId, request.ProductId);
                return Ok(new { message = "🗑 Producto eliminado del carrito." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ✅ POST: api/cart/checkout
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                int userId = GetCurrentUserId();
                await _cartService.CheckoutAsync(userId);
                return Ok(new { message = "💰 Compra finalizada correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("id")?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new Exception("No se pudo obtener el ID del usuario autenticado.");

            return int.Parse(userIdClaim);
        }
    }

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
