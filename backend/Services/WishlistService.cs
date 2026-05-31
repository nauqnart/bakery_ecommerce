using StitchArtisan.Backend.Models;
using StitchArtisan.Backend.Repositories;

namespace StitchArtisan.Backend.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<IEnumerable<Wishlist>> GetUserWishlistAsync(int userId)
        {
            return await _wishlistRepository.GetUserWishlistAsync(userId);
        }

        public async Task<bool> ToggleWishlistAsync(int userId, int productId)
        {
            var existing = await _wishlistRepository.GetWishlistItemAsync(userId, productId);
            if (existing != null)
            {
                await _wishlistRepository.RemoveAsync(existing);
                return false; // Trả về false nghĩa là đã xóa (un-hearted)
            }
            else
            {
                await _wishlistRepository.AddAsync(new Wishlist
                {
                    UserId = userId,
                    ProductId = productId
                });
                return true; // Trả về true nghĩa là đã thêm (hearted)
            }
        }
    }
}
