using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Models;
using StitchArtisan.Backend.Repositories;

namespace StitchArtisan.Backend.Services
{
    // Tầng 2: Business Logic Layer - Xử lý nghiệp vụ, gọi xuống Repository
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                FullName = u.FullName ?? string.Empty,
                Email = u.Email,
                Role = u.Role?.RoleName ?? "customer"
            });
        }

        public async Task<UserResponseDto> RegisterAsync(UserRegisterDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new Exception("Email already exists");
            }

            var newUser = new User
            {
                FullName = dto.Name,
                Email = dto.Email,
                // Trong thực tế, cần mã hóa mật khẩu ở đây bằng BCrypt hoặc PBKDF2
                PasswordHash = dto.Password, 
                RoleId = 3 // 3 is customer
            };

            await _userRepository.AddAsync(newUser);

            return new UserResponseDto
            {
                UserId = newUser.UserId,
                FullName = newUser.FullName ?? string.Empty,
                Email = newUser.Email,
                Role = "customer"
            };
        }
        public async Task<UserResponseDto?> LoginAsync(UserLoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            // So sánh mật khẩu (hiện tại so sánh chuỗi trần, thực tế nên dùng BCrypt.Verify)
            if (user == null || user.PasswordHash != dto.Password)
            {
                return null;
            }

            return new UserResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email,
                Role = user.Role?.RoleName ?? "customer"
            };
        }
    }
}
