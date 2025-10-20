using Application.Interfaces;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
            => await _roleRepository.GetAllAsync();

        public async Task<Role?> GetByIdAsync(int id)
            => await _roleRepository.GetByIdAsync(id);

        public async Task<Role?> GetByNameAsync(string name)
            => await _roleRepository.GetByNameAsync(name);

        public async Task<Role> AddAsync(Role role)
        {
            var existing = await _roleRepository.GetByNameAsync(role.Name);
            if (existing != null)
                throw new Exception("Ese rol ya existe.");

            await _roleRepository.AddAsync(role);
            return role;
        }

        public async Task DeleteAsync(int id)
            => await _roleRepository.DeleteAsync(id);
    }
}
