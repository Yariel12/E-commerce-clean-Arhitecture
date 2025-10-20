using Core.Entities;

namespace Core.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetByNameAsync(string name);
        Task AddAsync(Role role);
        Task DeleteAsync(int id);
    }
}
