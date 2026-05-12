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
                    StockQty  = v.StockQty,
                    ImageUrl  = v.ImageUrl
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
    }
}
