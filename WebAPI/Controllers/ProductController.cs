using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Producto no encontrado" });

            return Ok(product);
        }


        [Authorize(Roles = "Admin,Seller")]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var products = await _productService.GetAllAsync();
            if (products.Any(p => p.Name.Equals(productDto.Name, StringComparison.OrdinalIgnoreCase)))
                return Conflict(new { message = "Ya existe un producto con ese nombre." });

            var categoryExists = await _categoryService.ExistsAsync(productDto.CategoryId);
            if (!categoryExists)
                return BadRequest(new { message = "La categoría especificada no existe." });

            await _productService.AddAsync(productDto);
            return Ok(new { message = "Producto agregado correctamente." });
        }

        // 🔐 Solo Admin puede actualizar productos
        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] ProductDto productDto)
        {
            if (id != productDto.Id)
                return BadRequest(new { message = "El ID del producto no coincide con el cuerpo de la solicitud." });

            var existingProduct = await _productService.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound(new { message = $"No se encontró el producto con ID {id}." });

            await _productService.UpdateAsync(productDto);
            return Ok(new { message = "Producto actualizado correctamente." });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = $"No se encontró el producto con ID {id}." });

            await _productService.DeleteAsync(id);
            return Ok(new { message = "Producto eliminado correctamente." });
        }
    }
}
