using StitchArtisan.Backend.DTOs;

namespace StitchArtisan.Backend.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> RegisterAsync(UserRegisterDto dto);
        Task<UserResponseDto?> LoginAsync(UserLoginDto dto);
    }
}
