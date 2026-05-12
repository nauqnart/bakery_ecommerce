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
        private readonly AppDbContext _context;

        public OrderService(IOrderRepository orderRepository, AppDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync() => await _orderRepository.GetAllAsync();

        public async Task<Order?> GetOrderByIdAsync(int id) => await _orderRepository.GetByIdAsync(id);

        public async Task<Order> CreateOrderAsync(OrderCreateDto dto)
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
                TaxAmount = Math.Round(totalPrice * 0.10m, 2),
                TotalAmount = Math.Round(totalPrice * 1.10m, 2),
                PaymentStatus = "unpaid"
            };
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            await _orderRepository.UpdateStatusAsync(orderId, status);
        }
    }
}
