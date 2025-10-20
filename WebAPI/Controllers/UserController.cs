using Core.DTOs;
using Infrastructure.Services;
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
