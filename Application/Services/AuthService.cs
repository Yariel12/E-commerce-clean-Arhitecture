using Application.Interfaces;
using Application.DTOs;
using Core.Entities;
using Core.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpireMinutes;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));

            _jwtSecret = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key not configured");
            _jwtIssuer = configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("Jwt:Issuer not configured");
            _jwtAudience = configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("Jwt:Audience not configured");
            _jwtExpireMinutes = configuration.GetValue<int>("Jwt:ExpireMinutes", 60);
        }

        public async Task<UserResponseDto?> LoginWithGoogleAsync(string googleToken)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);
            }
            catch (Exception)
            {
                throw new ArgumentException("Token de Google inválido.");
            }

            var user = await _userRepository.GetByEmailAsync(payload.Email);
            if (user == null)
            {
                var defaultRole = await _roleRepository.GetByNameAsync("User");
                if (defaultRole == null)
                    throw new Exception("El rol 'User' no existe en la base de datos.");

                user = new User
                {
                    Username = payload.Name ?? payload.Email,
                    Email = payload.Email,
                    PasswordHash = "",
                    Role = defaultRole
                };
                await _userRepository.AddAsync(user);
            }

            var response = MapToResponsee(user);
            response.Token = GenerateJwtToken(user);
            return response;
        }

        private UserResponseDto MapToResponsee(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role?.Name
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtExpireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}