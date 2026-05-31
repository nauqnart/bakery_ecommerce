using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StitchArtisan.Backend.Services;
using StitchArtisan.Backend.DTOs;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace StitchArtisan.Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration;
        private readonly StitchArtisan.Backend.Data.AppDbContext _context;
        private readonly IEmailService _emailService;

        public PaymentController(IOrderService orderService, IConfiguration configuration, StitchArtisan.Backend.Data.AppDbContext context, IEmailService emailService)
        {
            _orderService = orderService;
            _configuration = configuration;
            _context = context;
            _emailService = emailService;
        }

        [HttpGet("vietqr/{orderId}")]
        public async Task<IActionResult> GetVietQrCode(int orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound(new { Success = false, Message = "Order not found" });

            if (order.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Get bank info from appsettings or use defaults
            var bankId = _configuration["Payment:VietQR:BankId"] ?? "MB";
            var accountNo = _configuration["Payment:VietQR:AccountNo"] ?? "0123456789";
            var template = _configuration["Payment:VietQR:Template"] ?? "compact";
            var accountName = _configuration["Payment:VietQR:AccountName"] ?? "CAKE SHOP";

            var amount = (int)order.TotalPrice;
            var addInfo = $"DH{orderId}";

            // URL format from VietQR.io
            var qrUrl = $"https://img.vietqr.io/image/{bankId}-{accountNo}-{template}.png?amount={amount}&addInfo={Uri.EscapeDataString(addInfo)}&accountName={Uri.EscapeDataString(accountName)}";

            return Ok(new { Success = true, QrUrl = qrUrl });
        }

        [HttpPost("webhook/sepay")]
        [AllowAnonymous]
        public async Task<IActionResult> SePayWebhook([FromBody] SePayWebhookDto payload)
        {
            try
            {
                var expectedToken = _configuration["Payment:SePay:WebhookToken"];
                if (!string.IsNullOrEmpty(expectedToken))
                {
                    if (!Request.Headers.TryGetValue("Authorization", out var authHeader)) 
                        return Unauthorized(new { success = false, message = "Missing Authorization header" });
                    
                    var token = authHeader.ToString().Replace("Apikey ", "").Replace("Bearer ", "").Trim();
                    if (token != expectedToken) 
                        return Unauthorized(new { success = false, message = "Invalid Token" });
                }

                if (payload.transferType != "in")
                    return Ok(new { success = true, message = "Not an incoming transfer" });

                if (string.IsNullOrEmpty(payload.content))
                    return Ok(new { success = true, message = "Empty content" });

                var match = Regex.Match(payload.content, @"DH(\d+)");
                if (match.Success)
                {
                    int orderId = int.Parse(match.Groups[1].Value);
                    
                    var order = await _orderService.GetOrderByIdAsync(orderId);
                    if (order != null && order.OrderStatus == "pending")
                    {
                        if (payload.transferAmount >= order.TotalPrice)
                        {
                            var invoice = _context.Invoices.FirstOrDefault(i => i.OrderId == orderId);
                            if (invoice != null)
                            {
                                invoice.PaymentStatus = "paid";
                                invoice.PaymentMethod = "bank";
                                invoice.PaidAt = DateTime.UtcNow;
                                await _context.SaveChangesAsync();
                            }
                            await _orderService.UpdateOrderStatusAsync(orderId, "processing");

                            // Send notification email
                            var userEmail = _context.Users.FirstOrDefault(u => u.UserId == order.UserId)?.Email;
                            if (!string.IsNullOrEmpty(userEmail))
                            {
                                string subject = $"Hearth & Harvest - Đã nhận thanh toán đơn #{order.OrderId}";
                                string body = $@"
                                    <h2>Cảm ơn bạn đã thanh toán!</h2>
                                    <p>Chúng tôi đã nhận được khoản thanh toán <strong>{payload.transferAmount:N0} VNĐ</strong> cho đơn hàng <strong>#{order.OrderId}</strong>.</p>
                                    <p>Đơn hàng của bạn đã được chuyển sang trạng thái <em>Đang xử lý</em>.</p>
                                    <p>Hearth & Harvest sẽ sớm chuẩn bị bánh và giao cho bạn.</p>
                                ";
                                _ = _emailService.SendEmailAsync(userEmail, subject, body);
                            }

                            // Send notification to Admin
                            string adminBody = $@"
                                <h2>Đơn hàng #{order.OrderId} vừa thanh toán thành công qua chuyển khoản!</h2>
                                <p>Số tiền: <strong>{payload.transferAmount:N0} VNĐ</strong></p>
                                <p>Vui lòng tiến hành chuẩn bị đơn hàng.</p>
                            ";
                            _ = _emailService.SendEmailAsync("admin@hearthandharvest.com", $"[Hệ thống] Đơn #{order.OrderId} đã thanh toán", adminBody);

                            return Ok(new { success = true, message = "Order updated to processing and paid" });
                        }
                        else 
                        {
                            return Ok(new { success = true, message = "Partial payment received" });
                        }
                    }
                }
                
                return Ok(new { success = true, message = "Webhook received but no matching pending order found" });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
