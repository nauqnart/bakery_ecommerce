using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StitchArtisan.Backend.Data;
using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Models;
using System.Text.RegularExpressions;

namespace StitchArtisan.Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/category
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.CategoryId,
                    c.Name,
                    c.Description,
                    ProductCount = c.Products.Count(p => !p.IsDeleted)
                })
                .ToListAsync();

            return Ok(new { Success = true, Data = categories });
        }

        // POST /api/category
        [HttpPost]
        [Authorize(Roles = "Admin,admin,Staff,staff")]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { Success = false, Message = "Tên danh mục không được để trống" });

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                ParentId = dto.ParentId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Thêm danh mục thành công", Data = category });
        }

        // PUT /api/category/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,admin,Staff,staff")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound(new { Success = false, Message = "Không tìm thấy danh mục" });

            category.Name = dto.Name;
            category.Description = dto.Description;
            category.ParentId = dto.ParentId;

            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Cập nhật danh mục thành công" });
        }

        // DELETE /api/category/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,admin,Staff,staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null) return NotFound(new { Success = false, Message = "Không tìm thấy danh mục" });

            if (category.Products.Any(p => !p.IsDeleted))
                return BadRequest(new { Success = false, Message = "Không thể xóa danh mục đang có sản phẩm." });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Xóa danh mục thành công" });
        }

        private string GenerateSlug(string phrase)
        {
            string str = phrase.ToLowerInvariant();
            str = Regex.Replace(str, @"\s+", "-");
            str = Regex.Replace(str, @"[^\w\-]+", "");
            str = str.Trim('-');
            return str;
        }
    }
}
