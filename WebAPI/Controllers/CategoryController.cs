using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _categoryService.AddAsync(categoryDto);
            return CreatedAtAction(nameof(GetById), new { id = categoryDto.Id }, categoryDto);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] CategoryDto categoryDto)
        {
            categoryDto.Id = id;

            await _categoryService.UpdateAsync(categoryDto);

            var updated = await _categoryService.GetByIdAsync(id);
            if (updated == null)
                return NotFound(new { message = "No se encontró la categoría actualizada." });

            return Ok(new { message = "Producto actualizado correctamente" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            await _categoryService.DeleteAsync(id);
            return Ok(new { message = "Category deleted successfully" });
        }

    }
}
