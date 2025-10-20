using Application.DTOs;

namespace Application.Interfaces
{
        public interface ICategoryService
        {
            Task<IEnumerable<CategoryDto>> GetAllAsync();
            Task<CategoryDto?> GetByIdAsync(int id);
            Task AddAsync(CategoryDto categoryDto);
            Task UpdateAsync(CategoryDto categoryDto);
            Task DeleteAsync(int id);
            Task<bool> ExistsAsync(int id); 

    }
}
