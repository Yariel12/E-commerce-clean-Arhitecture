using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al obtener los productos.", ex);
            }
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return null;

                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener el producto con ID {id}.", ex);
            }
        }

        public async Task AddAsync(ProductDto productDto)
        {
            try
            {
               
                var products = await _productRepository.GetAllAsync();
                var existingProduct = products
                    .Where(p => p.Name.ToLower() == productDto.Name.ToLower());

                if (existingProduct.Any())
                    throw new InvalidOperationException($"Ya existe un producto con el nombre '{productDto.Name}'.");

                var product = _mapper.Map<Product>(productDto);
                await _productRepository.AddAsync(product);
            }
            catch (InvalidOperationException dupEx)
            {
                throw new ApplicationException(dupEx.Message);
            }
            catch (DbUpdateException dbEx)
            {
                throw new ApplicationException("Error al guardar el producto en la base de datos.", dbEx);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al agregar un producto.", ex);
            }
        }

        public async Task UpdateAsync(ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                await _productRepository.UpdateAsync(product);
            }
            catch (KeyNotFoundException)
            {
                throw new ApplicationException($"No se encontró el producto con ID {productDto.Id} para actualizar.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al actualizar el producto.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _productRepository.DeleteAsync(id);
            }
            catch (KeyNotFoundException)
            {
                throw new ApplicationException($"No se encontró el producto con ID {id} para eliminar.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al eliminar el producto.", ex);
            }
        }

        public async Task<PagedResult<ProductDto>> GetPagedAsync(int page, int limit, string? search)
        {
            try
            {
                var query = await _productRepository.GetAllAsQueryableAsync();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));
                }

                var total = query.Count();

                var items = query
                    .OrderByDescending(p => p.Id)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToList();

                return new PagedResult<ProductDto>
                {
                    Data = _mapper.Map<IEnumerable<ProductDto>>(items),
                    Total = total,
                    Page = page,
                    Limit = limit
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al obtener los productos paginados.", ex);
            }
        }

    }
}
