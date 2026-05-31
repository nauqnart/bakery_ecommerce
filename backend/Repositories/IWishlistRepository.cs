using StitchArtisan.Backend.Models;

namespace StitchArtisan.Backend.Repositories
{
    public interface IWishlistRepository
    {
        Task<IEnumerable<Wishlist>> GetUserWishlistAsync(int userId);
        Task<Wishlist?> GetWishlistItemAsync(int userId, int productId);
        Task AddAsync(Wishlist wishlist);
        Task RemoveAsync(Wishlist wishlist);
    }
}
