using System.Security.Claims;
using Application.DTOs;
using System.IdentityModel.Tokens.Jwt;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }


    [Authorize] 
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (userIdClaim == null)
                return Unauthorized(new { message = "No se pudo obtener el ID del usuario." });

            if (!int.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "ID de usuario inválido." });

            var user = await _userService.GetCurrentUserAsync(userId);
            if (user == null)
            return NotFound(new { message = "Usuario no encontrado." });

        return Ok(user);
    }


    [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            var user = await _userService.RegisterAsync(dto);
            if (user == null) return BadRequest(new { message = "User already exists" });
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _userService.LoginAsync(dto);
            if (user == null) return Unauthorized(new { message = "Invalid credentials" });
            return Ok(user);
        }
    }
}
