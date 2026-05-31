using StitchArtisan.Backend.DTOs;

namespace StitchArtisan.Backend.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<(IEnumerable<UserResponseDto> Users, int TotalCount)> GetPagedUsersAsync(int page, int pageSize);
        Task<UserResponseDto> RegisterAsync(UserRegisterDto dto);
        Task<UserResponseDto?> LoginAsync(UserLoginDto dto);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
        Task<UserResponseDto?> GoogleLoginAsync(GoogleLoginDto dto);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
    }
}
