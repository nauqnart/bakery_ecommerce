using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Models;

namespace StitchArtisan.Backend.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedOrdersAsync(int page, int pageSize);
        Task<IEnumerable<Order>> GetMyOrdersAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(OrderCreateDto dto);
        Task UpdateOrderStatusAsync(int orderId, string status);
    }
}
