using StitchArtisan.Backend.Models;

namespace StitchArtisan.Backend.Services
{
    public interface IWishlistService
    {
        Task<IEnumerable<Wishlist>> GetUserWishlistAsync(int userId);
        Task<bool> ToggleWishlistAsync(int userId, int productId);
    }
}
