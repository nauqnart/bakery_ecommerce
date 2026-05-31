using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StitchArtisan.Backend.Data;
using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Models;
using System.Security.Claims;

namespace StitchArtisan.Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserAddressController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserAddressController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyAddresses()
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var addresses = await _context.UserAddresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();

            return Ok(new { Success = true, Data = addresses });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserAddressCreateDto dto)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var addressCount = await _context.UserAddresses.CountAsync(a => a.UserId == userId);
            bool isFirstAddress = addressCount == 0;

            // If it's the first address, or they checked IsDefault, handle the default state
            bool setAsDefault = isFirstAddress || dto.IsDefault;

            if (setAsDefault && !isFirstAddress)
            {
                var existingDefault = await _context.UserAddresses.FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
                if (existingDefault != null)
                {
                    existingDefault.IsDefault = false;
                }
            }

            var address = new UserAddress
            {
                UserId = userId,
                FullName = dto.FullName,
                Phone = dto.Phone,
                AddressLine = dto.AddressLine,
                IsDefault = setAsDefault
            };

            await _context.UserAddresses.AddAsync(address);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Data = address });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserAddressUpdateDto dto)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var address = await _context.UserAddresses.FirstOrDefaultAsync(a => a.UserAddressId == id && a.UserId == userId);
            if (address == null) return NotFound(new { Success = false, Message = "Address not found" });

            address.FullName = dto.FullName;
            address.Phone = dto.Phone;
            address.AddressLine = dto.AddressLine;
            
            if (dto.IsDefault && !address.IsDefault)
            {
                var existingDefault = await _context.UserAddresses.FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
                if (existingDefault != null)
                {
                    existingDefault.IsDefault = false;
                }
                address.IsDefault = true;
            }

            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Data = address });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var address = await _context.UserAddresses.FirstOrDefaultAsync(a => a.UserAddressId == id && a.UserId == userId);
            if (address == null) return NotFound(new { Success = false, Message = "Address not found" });

            bool wasDefault = address.IsDefault;

            _context.UserAddresses.Remove(address);
            await _context.SaveChangesAsync();

            if (wasDefault)
            {
                var nextAddress = await _context.UserAddresses.Where(a => a.UserId == userId).FirstOrDefaultAsync();
                if (nextAddress != null)
                {
                    nextAddress.IsDefault = true;
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new { Success = true, Message = "Address deleted successfully" });
        }

        [HttpPut("{id}/set-default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var address = await _context.UserAddresses.FirstOrDefaultAsync(a => a.UserAddressId == id && a.UserId == userId);
            if (address == null) return NotFound(new { Success = false, Message = "Address not found" });

            if (!address.IsDefault)
            {
                var existingDefault = await _context.UserAddresses.FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
                if (existingDefault != null)
                {
                    existingDefault.IsDefault = false;
                }
                address.IsDefault = true;
                await _context.SaveChangesAsync();
            }

            return Ok(new { Success = true, Data = address });
        }
    }
}
