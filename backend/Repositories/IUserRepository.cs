using StitchArtisan.Backend.Models;

namespace StitchArtisan.Backend.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<(IEnumerable<User>, int)> GetPagedAsync(int page, int pageSize);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<User?> GetByResetTokenAsync(string token);
    }
}
