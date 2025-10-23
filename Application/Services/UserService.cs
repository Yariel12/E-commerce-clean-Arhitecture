using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly string _jwtSecret;
        private readonly int _jwtExpireMinutes;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, string jwtSecret, int jwtExpireMinutes = 60)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _jwtSecret = jwtSecret;
            _jwtExpireMinutes = jwtExpireMinutes;
        }

        public async Task<UserResponseDto?> GetCurrentUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role?.Name ?? "User"
            };
        }

        public async Task<UserCreatedDto?> RegisterAsync(UserRegisterDto dto)
        {
            if (!IsValidEmail(dto.Email))
                throw new ArgumentException("El formato del email no es válido.");

            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null) return null;

            var defaultRole = await _roleRepository.GetByNameAsync("Admin");
            if (defaultRole == null)
                throw new Exception("El rol 'Admin' no existe en la base de datos.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = defaultRole
            };

            await _userRepository.AddAsync(user);

            return new UserCreatedDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role?.Name ?? "User"
            };
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserResponseDto?> LoginAsync(UserLoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

            return MapToResponse(user);
        }

        private UserResponseDto MapToResponse(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role?.Name ?? "User",
                Token = GenerateToken(user)
            };
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "ProyectSX",
                audience: "ProyectSX",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtExpireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}