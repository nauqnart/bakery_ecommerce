using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Models;

namespace StitchArtisan.Backend.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsAsync(int page, int pageSize, string? search, int? categoryId = null, string? sortBy = null, decimal? minPrice = null, decimal? maxPrice = null);
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(ProductCreateDto dto);
        Task UpdateProductAsync(int id, ProductUpdateDto dto);
        Task DeleteProductAsync(int id);
    }
}
