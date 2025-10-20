using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAllByUserIdAsync(int userId);
        Task<AddressDto?> GetByIdAsync(int id);

        Task<AddressDto> AddAsync(AddressDto addressDto);

        Task<AddressDto> UpdateAsync(AddressDto addressDto);

        Task DeleteAsync(int id);
    }
}
