using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StitchArtisan.Backend.Data;
using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Services;

namespace StitchArtisan.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _context;

        public UserController(IUserService userService, AppDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        // GET /api/user
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new { Success = true, Data = users });
        }

        // GET /api/user/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound(new { Success = false, Message = "User not found" });
            return Ok(new { Success = true, Data = new UserResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role?.RoleName ?? "customer",
                IsActive = user.IsActive
            }});
        }

        // POST /api/user/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            try
            {
                var newUser = await _userService.RegisterAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = newUser.UserId }, new { Success = true, Data = newUser });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // POST /api/user/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var user = await _userService.LoginAsync(dto);
            if (user == null)
                return Unauthorized(new { Success = false, Message = "Email hoặc mật khẩu không chính xác." });
            return Ok(new { Success = true, Data = user });
        }

        // PUT /api/user/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { Success = false, Message = "User not found" });

            user.FullName = dto.FullName;
            user.Phone    = dto.Phone;
            user.RoleId   = dto.RoleId;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { Success = true, Message = "User updated" });
        }

        // DELETE /api/user/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { Success = false, Message = "User not found" });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { Success = true, Message = "User deleted" });
        }

        // PATCH /api/user/{id}/toggle-active — bật/tắt tài khoản nhanh
        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { Success = false, Message = "User not found" });

            user.IsActive  = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { Success = true, IsActive = user.IsActive });
        }
    }
}
