using StitchArtisan.Backend.Models;

namespace StitchArtisan.Backend.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<(IEnumerable<Product>, int)> GetPagedAsync(int page, int pageSize, string? search, int? categoryId = null, string? sortBy = null, decimal? minPrice = null, decimal? maxPrice = null);
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task RemoveVariantsAsync(IEnumerable<ProductVariant> variants);
    }
}
