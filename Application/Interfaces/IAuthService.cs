using Core.DTOs;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto?> LoginWithGoogleAsync(string googleToken);
    }
}
