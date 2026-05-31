using Microsoft.EntityFrameworkCore;
using StitchArtisan.Backend.Data;
using StitchArtisan.Backend.Models;

namespace StitchArtisan.Backend.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Variants).Where(p => !p.IsDeleted).ToListAsync();
        }

        public async Task<(IEnumerable<Product>, int)> GetPagedAsync(int page, int pageSize, string? search, int? categoryId = null, string? sortBy = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var query = _context.Products.Include(p => p.Variants).Where(p => !p.IsDeleted).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search) || (p.BaseDescription != null && p.BaseDescription.Contains(search)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Variants.Any(v => v.Price >= minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Variants.Any(v => v.Price <= maxPrice.Value));
            }

            switch (sortBy)
            {
                case "hot":
                    query = query.OrderByDescending(p => p.ProductId);
                    break;
                case "price_asc":
                    query = query.OrderBy(p => p.Variants.Min(v => v.Price));
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Variants.Max(v => v.Price));
                    break;
                case "newest":
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(p => p.ProductId);
                    break;
            }

            var totalCount = await query.CountAsync();
            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (products, totalCount);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Variants).FirstOrDefaultAsync(p => p.ProductId == id && !p.IsDeleted);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            // Entity is already tracked by GetByIdAsync, just save changes
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product != null)
            {
                var variantIds = product.Variants.Select(v => v.VariantId).ToList();

                // Check if any variant is in an order
                var hasOrders = await _context.OrderItems.AnyAsync(oi => variantIds.Contains(oi.VariantId));
                
                // Delete related cart items (we don't want customers checking out deleted products)
                var cartItems = await _context.CartItems.Where(ci => variantIds.Contains(ci.VariantId)).ToListAsync();
                if (cartItems.Any()) _context.CartItems.RemoveRange(cartItems);

                // Delete related wishlists
                var wishlists = await _context.Wishlists.Where(w => w.ProductId == id).ToListAsync();
                if (wishlists.Any()) _context.Wishlists.RemoveRange(wishlists);

                if (hasOrders)
                {
                    // Soft delete
                    product.IsDeleted = true;
                }
                else
                {
                    // Hard delete

                    _context.ProductVariants.RemoveRange(product.Variants);
                    _context.Products.Remove(product);
                }
                
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveVariantsAsync(IEnumerable<ProductVariant> variants)
        {
            var variantIds = variants.Select(v => v.VariantId).ToList();

            // Check if any variant is in an order
            var hasOrders = await _context.OrderItems.AnyAsync(oi => variantIds.Contains(oi.VariantId));
            if (hasOrders)
            {
                throw new InvalidOperationException("Không thể xóa biến thể vì đã có đơn hàng đặt mua biến thể này.");
            }

            // Remove from carts first to avoid FK violation
            var cartItems = await _context.CartItems.Where(ci => variantIds.Contains(ci.VariantId)).ToListAsync();
            if (cartItems.Any())
            {
                _context.CartItems.RemoveRange(cartItems);
            }

            _context.ProductVariants.RemoveRange(variants);
            await _context.SaveChangesAsync();
        }
    }
}
