using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Models;

namespace StitchArtisan.Backend.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(ProductCreateDto dto);
        Task UpdateProductAsync(int id, ProductUpdateDto dto);
        Task DeleteProductAsync(int id);
    }
}
