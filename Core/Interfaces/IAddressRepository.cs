using Core.Entities;

namespace Core.Interfaces
{
    public interface IAddressRepository
    {
        Task<Address?> GetByAsync(int id);
        Task<IEnumerable<Address>> GetByUserIdAsync(int userId);
        Task<Address> AddAsync(Address address);
        Task<Address> UpdateAsync(Address address);
        Task DeleteAsync(int id);
    }
}
