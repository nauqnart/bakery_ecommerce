using Microsoft.EntityFrameworkCore;
using StitchArtisan.Backend.Data;
using StitchArtisan.Backend.Models;

namespace StitchArtisan.Backend.Repositories
{
    // Tầng 3: Data Access Layer - Giao tiếp với Database thật (EF Core)
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.Include(u => u.Role).Where(u => !u.IsDeleted).ToListAsync();
        }

        public async Task<(IEnumerable<User>, int)> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Users.Include(u => u.Role).Where(u => !u.IsDeleted).AsQueryable();
            var totalCount = await query.CountAsync();
            var users = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (users, totalCount);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id && !u.IsDeleted);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByResetTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ResetPasswordToken == token);
        }
    }
}
