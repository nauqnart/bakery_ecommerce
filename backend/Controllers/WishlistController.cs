using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StitchArtisan.Backend.Services;
using System.Security.Claims;

namespace StitchArtisan.Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private int GetUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdStr, out int userId) ? userId : 0;
        }

        // GET /api/wishlist
        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var wishlists = await _wishlistService.GetUserWishlistAsync(userId);
            return Ok(new { Success = true, Data = wishlists });
        }

        // POST /api/wishlist/toggle
        [HttpPost("toggle")]
        public async Task<IActionResult> Toggle([FromBody] WishlistToggleRequest req)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var isAdded = await _wishlistService.ToggleWishlistAsync(userId, req.ProductId);
            return Ok(new { Success = true, IsAdded = isAdded });
        }
    }

    public class WishlistToggleRequest
    {
        public int ProductId { get; set; }
    }
}
