using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StitchArtisan.Backend.Data;
using StitchArtisan.Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace StitchArtisan.Backend.Services
{
    public class OrderCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderCleanupService> _logger;

        public OrderCleanupService(IServiceProvider serviceProvider, ILogger<OrderCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Order Cleanup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredOrdersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing order cleanup.");
                }

                // Run every 30 minutes
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }

        private async Task CleanupExpiredOrdersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var orderRepo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var expiryTime = DateTime.UtcNow.AddHours(-24);

            var expiredOrders = await context.Orders
                .Where(o => o.OrderStatus == "pending" && o.PaymentMethod == "bank" && o.CreatedAt < expiryTime)
                .ToListAsync();

            if (expiredOrders.Any())
            {
                _logger.LogInformation($"Found {expiredOrders.Count} expired pending bank orders to cancel.");
                foreach (var order in expiredOrders)
                {
                    await orderRepo.CancelOrderAsync(order.OrderId);

                    // Send cancellation email
                    var userEmail = await context.Users
                        .Where(u => u.UserId == order.UserId)
                        .Select(u => u.Email)
                        .FirstOrDefaultAsync();

                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        string subject = $"Đơn hàng #{order.OrderId} đã bị hủy tự động";
                        string body = $@"
                            <h2>Hủy đơn do quá hạn thanh toán</h2>
                            <p>Đơn hàng <strong>#{order.OrderId}</strong> của bạn đặt qua phương thức Chuyển khoản đã bị hủy tự động do chúng tôi chưa nhận được thanh toán sau 24 giờ.</p>
                            <p>Nếu bạn đã thanh toán, vui lòng liên hệ ngay với Hotline của Hearth & Harvest để được hỗ trợ.</p>
                            <p>Cảm ơn bạn đã quan tâm đến sản phẩm của tiệm!</p>
                        ";
                        _ = emailService.SendEmailAsync(userEmail, subject, body);
                    }
                }
                _logger.LogInformation("Successfully cancelled expired orders.");
            }
        }
    }
}
