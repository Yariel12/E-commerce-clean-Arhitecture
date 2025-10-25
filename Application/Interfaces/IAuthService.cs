using Application.DTOs;


namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto?> LoginWithGoogleAsync(string googleToken);
    }
}
