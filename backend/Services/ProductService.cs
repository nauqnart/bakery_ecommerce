using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Models;
using StitchArtisan.Backend.Repositories;

namespace StitchArtisan.Backend.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync() => await _productRepository.GetAllAsync();

        public async Task<Product?> GetProductByIdAsync(int id) => await _productRepository.GetByIdAsync(id);

        public async Task<Product> CreateProductAsync(ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                BaseDescription = dto.BaseDescription,
                CategoryId = dto.CategoryId,
                Slug = dto.Name.ToLower().Replace(" ", "-"),
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Sku = dto.Sku,
                        Price = dto.Price,
                        StockQty = dto.StockQuantity,
                        ImageUrl = dto.ImageUrl
                    }
                }
            };
            
            await _productRepository.AddAsync(product);
            return product;
        }

        public async Task UpdateProductAsync(int id, ProductUpdateDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) throw new Exception("Product not found");

            product.Name = dto.Name;
            product.BaseDescription = dto.BaseDescription;
            product.CategoryId = dto.CategoryId;
            
            var variant = product.Variants.FirstOrDefault();
            if (variant != null)
            {
                variant.Sku = dto.Sku;
                variant.Price = dto.Price;
                variant.StockQty = dto.StockQuantity;
                variant.ImageUrl = dto.ImageUrl;
            }

            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }
    }
}
