using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Models;

namespace StitchArtisan.Backend.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(OrderCreateDto dto);
        Task UpdateOrderStatusAsync(int orderId, string status);
    }
}
