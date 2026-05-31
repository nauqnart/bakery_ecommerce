using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Models;
using StitchArtisan.Backend.Repositories;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;

namespace StitchArtisan.Backend.Services
{
    // Tầng 2: Business Logic Layer - Xử lý nghiệp vụ, gọi xuống Repository
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository, IConfiguration configuration, IEmailService emailService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                FullName = u.FullName ?? string.Empty,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role?.RoleName ?? "customer",
                IsActive = u.IsActive
            });
        }

        public async Task<(IEnumerable<UserResponseDto> Users, int TotalCount)> GetPagedUsersAsync(int page, int pageSize)
        {
            var result = await _userRepository.GetPagedAsync(page, pageSize);
            var usersDto = result.Item1.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                FullName = u.FullName ?? string.Empty,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role?.RoleName ?? "customer",
                IsActive = u.IsActive
            });
            return (usersDto, result.Item2);
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), 
                RoleId = 3 // 3 is customer
            };

            await _userRepository.AddAsync(newUser);

            return new UserResponseDto
            {
                UserId = newUser.UserId,
                FullName = newUser.FullName ?? string.Empty,
                Email = newUser.Email,
                Role = "customer",
                IsActive = newUser.IsActive
            };
        }
        public async Task<UserResponseDto?> LoginAsync(UserLoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return null;
            }

            if (!user.IsActive)
            {
                throw new Exception("ACCOUNT_LOCKED");
            }

            var token = GenerateJwtToken(user);

            return new UserResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email,
                Role = user.Role?.RoleName ?? "customer",
                Token = token
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var rawRole = user.Role?.RoleName ?? "customer";
            var normalizedRole = string.IsNullOrEmpty(rawRole) ? "Customer" : char.ToUpper(rawRole[0]) + rawRole.Substring(1).ToLower();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, normalizedRole)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) return; // Don't reveal that the user does not exist

            var resetToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
            user.ResetPasswordToken = resetToken;
            user.ResetPasswordExpiry = DateTime.UtcNow.AddHours(1);

            await _userRepository.UpdateAsync(user);

            var resetLink = $"http://localhost:5173/reset-password?token={Uri.EscapeDataString(resetToken)}";
            
            var emailBody = $@"
                <h3>Yêu cầu đặt lại mật khẩu</h3>
                <p>Bạn (hoặc ai đó) vừa yêu cầu đặt lại mật khẩu cho tài khoản này.</p>
                <p>Vui lòng click vào link bên dưới để đặt lại mật khẩu. Link này sẽ hết hạn trong 1 giờ.</p>
                <a href='{resetLink}'>Đặt lại mật khẩu</a>
                <br/>
                <p>Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>";

            await _emailService.SendEmailAsync(user.Email, "Hearth & Harvest - Đặt lại mật khẩu", emailBody);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userRepository.GetByResetTokenAsync(dto.Token);
            if (user == null || user.ResetPasswordExpiry < DateTime.UtcNow)
            {
                throw new Exception("Token không hợp lệ hoặc đã hết hạn");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordExpiry = null;

            await _userRepository.UpdateAsync(user);
        }

        public async Task<UserResponseDto?> GoogleLoginAsync(GoogleLoginDto dto)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(dto.Credential);
            
            var user = await _userRepository.GetByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new User
                {
                    FullName = payload.Name,
                    Email = payload.Email,
                    PasswordHash = "", 
                    RoleId = 3, 
                    IsActive = true
                };
                await _userRepository.AddAsync(user);
            }
            else if (!user.IsActive)
            {
                throw new Exception("ACCOUNT_LOCKED");
            }

            var token = GenerateJwtToken(user);
            return new UserResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email,
                Role = user.Role?.RoleName ?? "customer",
                Token = token
            };
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            // For Google accounts, they might not have a password hash yet. 
            if (!string.IsNullOrEmpty(user.PasswordHash) && !BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                throw new Exception("Mật khẩu hiện tại không đúng");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _userRepository.UpdateAsync(user);
        }
    }
}
