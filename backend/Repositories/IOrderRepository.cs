using StitchArtisan.Backend.Models;

namespace StitchArtisan.Backend.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
        Task AddAsync(Order order);
        Task UpdateStatusAsync(int orderId, string status);
    }
}
