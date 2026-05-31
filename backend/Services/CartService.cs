using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Models;
using StitchArtisan.Backend.Repositories;

namespace StitchArtisan.Backend.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _cartRepository.CreateCartAsync(cart);
            }
            return cart;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            return MapToDto(cart);
        }

        public async Task<CartDto> AddItemToCartAsync(int userId, AddToCartDto dto)
        {
            var cart = await GetOrCreateCartAsync(userId);
            
            var existingItem = cart.Items.FirstOrDefault(i => i.VariantId == dto.VariantId);
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                await _cartRepository.UpdateCartItemAsync(existingItem);
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.CartId,
                    VariantId = dto.VariantId,
                    Quantity = dto.Quantity
                };
                await _cartRepository.AddCartItemAsync(newItem);
            }

            return await GetCartAsync(userId); // Reload cart to get variants and products
        }

        public async Task<CartDto> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto dto)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) throw new Exception("Cart not found");

            var item = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
            if (item == null) throw new Exception("Item not found in cart");

            if (dto.Quantity <= 0)
            {
                await _cartRepository.RemoveCartItemAsync(item);
            }
            else
            {
                item.Quantity = dto.Quantity;
                await _cartRepository.UpdateCartItemAsync(item);
            }

            return await GetCartAsync(userId);
        }

        public async Task<CartDto> RemoveItemFromCartAsync(int userId, int cartItemId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) throw new Exception("Cart not found");

            var item = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
            if (item == null) throw new Exception("Item not found in cart");

            await _cartRepository.RemoveCartItemAsync(item);

            return await GetCartAsync(userId);
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart != null)
            {
                await _cartRepository.ClearCartAsync(cart.CartId);
            }
        }

        private CartDto MapToDto(Cart cart)
        {
            return new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    CartItemId = i.CartItemId,
                    VariantId = i.VariantId,
                    ProductName = i.Variant?.Product?.Name ?? "Unknown Product",
                    Sku = i.Variant?.Sku ?? string.Empty,
                    ImageUrl = !string.IsNullOrEmpty(i.Variant?.ImageUrl) ? i.Variant.ImageUrl : (i.Variant?.Product?.Variants?.FirstOrDefault(v => !string.IsNullOrEmpty(v.ImageUrl))?.ImageUrl ?? string.Empty),
                    Price = i.Variant?.Price ?? 0,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
    }
}
