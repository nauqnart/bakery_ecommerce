using Microsoft.EntityFrameworkCore;
using StitchArtisan.Backend.Data;
using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Repositories;

namespace StitchArtisan.Backend.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrderRepository    _orderRepo;
        private readonly IProductRepository  _productRepo;
        private readonly IUserRepository     _userRepo;
        private readonly AppDbContext        _context;

        public DashboardService(
            IOrderRepository orderRepo,
            IProductRepository productRepo,
            IUserRepository userRepo,
            AppDbContext context)
        {
            _orderRepo   = orderRepo;
            _productRepo = productRepo;
            _userRepo    = userRepo;
            _context     = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var orders   = (await _orderRepo.GetAllAsync()).ToList();
            var products = (await _productRepo.GetAllAsync()).ToList();
            var users    = (await _userRepo.GetAllAsync()).ToList();

            var completed = orders.Where(o =>
                o.OrderStatus == "delivered" || o.OrderStatus == "completed").ToList();

            var totalRevenue  = completed.Sum(o => o.TotalPrice);
            var totalOrders   = orders.Count;
            var pendingOrders = orders.Count(o => o.OrderStatus == "pending");
            var avgOrder      = totalOrders > 0 ? orders.Average(o => o.TotalPrice) : 0;

            // 5 đơn hàng gần nhất
            var recentOrders = orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new RecentOrderDto
                {
                    OrderId       = o.OrderId,
                    CustomerName  = o.User?.FullName ?? "Khách",
                    TotalPrice    = o.TotalPrice,
                    OrderStatus   = o.OrderStatus,
                    PaymentStatus = o.Invoice?.PaymentStatus ?? "unpaid",
                    CreatedAt     = o.CreatedAt
                }).ToList();

            // Sản phẩm sắp hết (stockQty <= 5)
            var lowStock = await _context.ProductVariants
                .Include(v => v.Product)
                .Where(v => v.StockQty <= 5)
                .OrderBy(v => v.StockQty)
                .Take(5)
                .Select(v => new LowStockProductDto
                {
                    ProductId = v.ProductId,
                    Name      = v.Product != null ? v.Product.Name : "—",
                    Sku       = v.Sku,
                    StockQty  = v.StockQty,
                    ImageUrl  = v.ImageUrl ?? (v.Product != null ? v.Product.Variants.Where(x => x.ImageUrl != null && x.ImageUrl != "").Select(x => x.ImageUrl).FirstOrDefault() : null)
                })
                .ToListAsync();

            return new DashboardSummaryDto
            {
                TotalRevenue    = totalRevenue,
                TotalOrders     = totalOrders,
                PendingOrders   = pendingOrders,
                TotalProducts   = products.Count,
                LowStockCount   = lowStock.Count,
                ActiveCustomers = users.Count(u => u.IsActive && u.RoleId == 3),
                AvgOrderValue   = Math.Round(avgOrder, 0),
                RecentOrders    = recentOrders,
                LowStockProducts= lowStock
            };
        }

        public async Task<object> GetRevenueChartAsync()
        {
            var orders = await _context.Orders
                .Where(o => o.OrderStatus == "delivered" || o.OrderStatus == "completed")
                .ToListAsync();

            // Group by Day for the last 7 days (or simply by Date)
            var revenueByDate = orders
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Revenue = g.Sum(o => o.TotalPrice)
                })
                .OrderBy(x => x.Date)
                .Take(7)
                .ToList();

            return new {
                Labels = revenueByDate.Select(r => r.Date).ToList(),
                Data = revenueByDate.Select(r => r.Revenue).ToList()
            };
        }

        public async Task<object> GetTopProductsChartAsync()
        {
            var topProducts = await _context.OrderItems
                .Include(oi => oi.Variant)
                .ThenInclude(v => v.Product)
                .GroupBy(oi => oi.Variant.Product.Name)
                .Select(g => new {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToListAsync();

            return new {
                Labels = topProducts.Select(p => p.ProductName).ToList(),
                Data = topProducts.Select(p => p.QuantitySold).ToList()
            };
        }

        public async Task<object> GetBottomProductsChartAsync()
        {
            var bottomProducts = await _context.OrderItems
                .Include(oi => oi.Variant)
                .ThenInclude(v => v.Product)
                .GroupBy(oi => oi.Variant.Product.Name)
                .Select(g => new {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(oi => oi.Quantity)
                })
                .OrderBy(x => x.QuantitySold)
                .Take(5)
                .ToListAsync();

            return new {
                Labels = bottomProducts.Select(p => p.ProductName).ToList(),
                Data = bottomProducts.Select(p => p.QuantitySold).ToList()
            };
        }

        public async Task<object> GetChartDataAsync()
        {
            var orders = await _context.Orders
                .Where(o => o.OrderStatus == "delivered" || o.OrderStatus == "completed")
                .ToListAsync();

            var revenueByDate = orders
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new {
                    Date = g.Key.ToString("dd-MM-yyyy"),
                    Revenue = g.Sum(o => o.TotalPrice)
                })
                .OrderBy(x => x.Date)
                .Take(7)
                .ToList();

            var categories = await _context.OrderItems
                .Include(oi => oi.Variant)
                .ThenInclude(v => v.Product)
                .ThenInclude(p => p.Category)
                .GroupBy(oi => oi.Variant.Product.Category != null ? oi.Variant.Product.Category.Name : "Khác")
                .Select(g => new {
                    CategoryName = g.Key,
                    QuantitySold = g.Sum(oi => oi.Quantity)
                })
                .ToListAsync();

            return new {
                Revenue = new {
                    Labels = revenueByDate.Select(r => r.Date).ToList(),
                    Data = revenueByDate.Select(r => r.Revenue).ToList()
                },
                Category = new {
                    Labels = categories.Select(c => c.CategoryName).ToList(),
                    Data = categories.Select(c => c.QuantitySold).ToList()
                }
            };
        }
    }
}
