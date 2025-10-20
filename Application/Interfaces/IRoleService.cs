using Core.Entities;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetByNameAsync(string name);
        Task<Role> AddAsync(Role role);
        Task DeleteAsync(int id);
    }
}
