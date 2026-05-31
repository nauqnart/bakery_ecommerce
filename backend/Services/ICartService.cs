using StitchArtisan.Backend.DTOs;

namespace StitchArtisan.Backend.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);
        Task<CartDto> AddItemToCartAsync(int userId, AddToCartDto dto);
        Task<CartDto> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto dto);
        Task<CartDto> RemoveItemFromCartAsync(int userId, int cartItemId);
        Task ClearCartAsync(int userId);
    }
}
