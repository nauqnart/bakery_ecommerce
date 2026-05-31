using StitchArtisan.Backend.Data;
using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Models;
using StitchArtisan.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace StitchArtisan.Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, AppDbContext context, IEmailService emailService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _context = context;
            _emailService = emailService;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync() => await _orderRepository.GetAllAsync();

        public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedOrdersAsync(int page, int pageSize)
        {
            return await _orderRepository.GetPagedAsync(page, pageSize);
        }

        public async Task<IEnumerable<Order>> GetMyOrdersAsync(int userId)
        {
            return await _orderRepository.GetByUserIdAsync(userId);
        }

        public async Task<Order?> GetOrderByIdAsync(int id) => await _orderRepository.GetByIdAsync(id);

        public async Task<Order> CreateOrderAsync(OrderCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                decimal totalPrice = 0;
                var orderItems = new List<OrderItem>();

                foreach (var item in dto.Items)
                {
                    // Tìm variant và kiểm tra tồn kho
                    var variant = await _context.ProductVariants
                        .Include(v => v.Product)
                        .FirstOrDefaultAsync(v => v.VariantId == item.VariantId);

                    if (variant == null)
                        throw new Exception($"Variant {item.VariantId} not found");
                    if (variant.StockQty < item.Quantity)
                        throw new Exception($"Insufficient stock for {variant.Product?.Name ?? "product"}");

                    // Reserve stock
                    variant.StockQty -= item.Quantity;

                    totalPrice += variant.Price * item.Quantity;
                    orderItems.Add(new OrderItem
                    {
                        VariantId = variant.VariantId,
                        Quantity = item.Quantity,
                        PriceAtPurchase = variant.Price
                    });
                }

                var order = new Order
                {
                    UserId = dto.UserId,
                    ShippingAddress = dto.ShippingAddress,
                    Note = dto.Note,
                    PaymentMethod = dto.PaymentMethod,
                    TotalPrice = totalPrice,
                    OrderStatus = "pending",
                    Items = orderItems
                };

                await _orderRepository.AddAsync(order);

                // Auto-generate invoice cho COD
                var invoice = new Invoice
                {
                    OrderId = order.OrderId,
                    InvoiceNumber = $"INV-{DateTime.UtcNow:yyyy}-{order.OrderId:D4}",
                    Subtotal = totalPrice,
                    TaxAmount = 0,
                    TotalAmount = totalPrice,
                    PaymentStatus = "unpaid"
                };
                await _context.Invoices.AddAsync(invoice);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Fire and forget email notification to Customer
                var userEmail = _context.Users.FirstOrDefault(u => u.UserId == order.UserId)?.Email;
                if (!string.IsNullOrEmpty(userEmail))
                {
                    string emailBody = $@"
                        <h2>Cảm ơn bạn đã đặt hàng tại Hearth & Harvest!</h2>
                        <p>Mã đơn hàng của bạn: <strong>#{order.OrderId}</strong></p>
                        <p>Tổng giá trị: <strong>{order.TotalPrice:N0} VNĐ</strong></p>
                        <p>Địa chỉ giao hàng: {order.ShippingAddress}</p>
                        <p>Trạng thái: <em>Đang chờ xử lý</em></p>
                        <br/>
                        <p>Chúng tôi sẽ liên hệ với bạn trong thời gian sớm nhất.</p>
                    ";
                    _ = _emailService.SendEmailAsync(userEmail, $"Xác nhận đơn hàng #{order.OrderId} từ Hearth & Harvest", emailBody);
                }

                // Fire and forget email notification to Admin
                string adminEmailBody = $@"
                    <h2>Có đơn hàng mới: #{order.OrderId}</h2>
                    <p>Khách hàng: {userEmail ?? "Khách vô danh"}</p>
                    <p>Tổng giá trị: <strong>{order.TotalPrice:N0} VNĐ</strong></p>
                    <p>Phương thức: <strong>{order.PaymentMethod}</strong></p>
                    <p>Vui lòng đăng nhập hệ thống để xử lý.</p>
                ";
                _ = _emailService.SendEmailAsync("admin@hearthandharvest.com", $"[Hệ thống] Đơn hàng mới #{order.OrderId}", adminEmailBody);

                return order;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Order not found");

            // State Machine Validation
            var validTransitions = new Dictionary<string, List<string>>
            {
                { "pending", new List<string> { "processing", "cancelled" } },
                { "processing", new List<string> { "shipped", "cancelled" } },
                { "shipped", new List<string> { "delivered", "cancelled" } },
                { "delivered", new List<string>() }, // Terminal state
                { "cancelled", new List<string>() }  // Terminal state
            };

            var currentStatus = order.OrderStatus ?? "pending";

            if (validTransitions.TryGetValue(currentStatus, out var allowed))
            {
                if (!allowed.Contains(status) && status != currentStatus)
                {
                    throw new Exception($"Invalid status transition from {currentStatus} to {status}");
                }
            }

            if (status == "cancelled")
            {
                await _orderRepository.CancelOrderAsync(orderId);
            }
            else
            {
                await _orderRepository.UpdateStatusAsync(orderId, status);
            }

            // Send notification email
            if (status != currentStatus)
            {
                var userEmail = _context.Users.FirstOrDefault(u => u.UserId == order.UserId)?.Email;
                if (!string.IsNullOrEmpty(userEmail))
                {
                    string subject = "";
                    string body = "";
                    
                    if (status == "shipped")
                    {
                        subject = $"Đơn hàng #{order.OrderId} đang trên đường giao!";
                        body = $@"
                            <h2>Hearth & Harvest đang giao bánh đến bạn!</h2>
                            <p>Đơn hàng <strong>#{order.OrderId}</strong> của bạn đã được chuyển cho đơn vị vận chuyển.</p>
                            <p>Vui lòng chú ý điện thoại để nhận hàng nhé.</p>
                        ";
                    }
                    else if (status == "delivered")
                    {
                        subject = $"Giao hàng thành công đơn #{order.OrderId}";
                        body = $@"
                            <h2>Đơn hàng đã được giao thành công!</h2>
                            <p>Đơn hàng <strong>#{order.OrderId}</strong> của bạn đã được giao đến nơi.</p>
                            <p>Chúc bạn ngon miệng và có những trải nghiệm tuyệt vời cùng Hearth & Harvest!</p>
                        ";
                    }
                    else if (status == "cancelled")
                    {
                        subject = $"Đơn hàng #{order.OrderId} đã bị hủy";
                        body = $@"
                            <h2>Đơn hàng bị hủy</h2>
                            <p>Đơn hàng <strong>#{order.OrderId}</strong> của bạn đã bị hủy.</p>
                            <p>Nếu bạn đã thanh toán, vui lòng liên hệ hotline để được hoàn tiền. Bạn luôn có thể đặt lại đơn mới trên website của chúng tôi.</p>
                        ";
                    }

                    if (!string.IsNullOrEmpty(body))
                    {
                        _ = _emailService.SendEmailAsync(userEmail, subject, body);
                    }
                }
            }
        }
    }
}
