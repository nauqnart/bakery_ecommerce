using StitchArtisan.Backend.Models;

namespace StitchArtisan.Backend.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<(IEnumerable<Order>, int)> GetPagedAsync(int page, int pageSize);
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
        Task AddAsync(Order order);
        Task UpdateStatusAsync(int orderId, string status);
        Task UpdateAsync(Order order);
        Task CancelOrderAsync(int orderId);
        Task<int> GetTotalCountAsync();
    }
}
