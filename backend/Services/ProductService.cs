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

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsAsync(int page, int pageSize, string? search, int? categoryId = null, string? sortBy = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            return await _productRepository.GetPagedAsync(page, pageSize, search, categoryId, sortBy, minPrice, maxPrice);
        }

        public async Task<Product?> GetProductByIdAsync(int id) => await _productRepository.GetByIdAsync(id);

        public async Task<Product> CreateProductAsync(ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                BaseDescription = dto.BaseDescription,
                CategoryId = dto.CategoryId,
                Slug = dto.Name.ToLower().Replace(" ", "-"),
                TierVariationsJson = dto.TierVariations.Any() ? System.Text.Json.JsonSerializer.Serialize(dto.TierVariations) : null,
                Variants = dto.Variants.Select(v => new ProductVariant
                {
                    VariantName = v.VariantName,
                    Sku = v.Sku,
                    Price = v.Price,
                    StockQty = v.StockQuantity,
                    ImageUrl = v.ImageUrl ?? dto.ImageUrl,
                    TierValuesJson = v.TierValues.Any() ? System.Text.Json.JsonSerializer.Serialize(v.TierValues) : null
                }).ToList()
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
            product.TierVariationsJson = dto.TierVariations.Any() ? System.Text.Json.JsonSerializer.Serialize(dto.TierVariations) : null;
            
            // Update existing and add new variants
            foreach (var vDto in dto.Variants)
            {
                var existing = product.Variants.FirstOrDefault(v => v.VariantId == vDto.VariantId);
                var valuesJson = vDto.TierValues.Any() ? System.Text.Json.JsonSerializer.Serialize(vDto.TierValues) : null;
                if (existing != null)
                {
                    existing.VariantName = vDto.VariantName;
                    existing.Sku = vDto.Sku;
                    existing.Price = vDto.Price;
                    existing.StockQty = vDto.StockQuantity;
                    existing.TierValuesJson = valuesJson;
                    if (!string.IsNullOrEmpty(vDto.ImageUrl)) existing.ImageUrl = vDto.ImageUrl;
                }
                else
                {
                    product.Variants.Add(new ProductVariant
                    {
                        VariantName = vDto.VariantName,
                        Sku = vDto.Sku,
                        Price = vDto.Price,
                        StockQty = vDto.StockQuantity,
                        ImageUrl = vDto.ImageUrl,
                        TierValuesJson = valuesJson
                    });
                }
            }

            // Remove deleted variants
            var incomingIds = dto.Variants.Where(v => v.VariantId.HasValue).Select(v => v.VariantId.Value).ToList();
            var toRemove = product.Variants.Where(v => v.VariantId != 0 && !incomingIds.Contains(v.VariantId)).ToList();
            if (toRemove.Any())
            {
                await _productRepository.RemoveVariantsAsync(toRemove);
            }

            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }
    }
}
