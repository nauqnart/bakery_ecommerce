using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StitchArtisan.Backend.Data;
using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Services;

namespace StitchArtisan.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        // Các định dạng ảnh được phép
        private static readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public ProductController(IProductService productService, AppDbContext context, IWebHostEnvironment env)
        {
            _productService = productService;
            _context = context;
            _env = env;
        }

        // GET /api/product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(new { Success = true, Data = products });
        }

        // GET /api/product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound(new { Success = false, Message = "Product not found" });
            return Ok(new { Success = true, Data = product });
        }

        // POST /api/product
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, new { Success = true, Data = product });
        }

        // POST /api/product/with-image  — tạo sản phẩm + upload ảnh cùng lúc (multipart/form-data)
        [HttpPost("with-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateWithImage([FromForm] ProductCreateDto dto, IFormFile? image)
        {
            // Upload ảnh trước (nếu có)
            if (image != null)
            {
                var uploadResult = await SaveImageAsync(image);
                if (uploadResult.Error != null)
                    return BadRequest(new { Success = false, Message = uploadResult.Error });
                dto.ImageUrl = uploadResult.Url!;
            }

            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, new { Success = true, Data = product });
        }

        // PUT /api/product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            try
            {
                await _productService.UpdateProductAsync(id, dto);
                return Ok(new { Success = true, Message = "Product updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // POST /api/product/{id}/upload-image  — upload / thay ảnh cho variant đầu tiên
        [HttpPost("{id}/upload-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(int id, IFormFile image)
        {
            // 1. Validate file
            var uploadResult = await SaveImageAsync(image);
            if (uploadResult.Error != null)
                return BadRequest(new { Success = false, Message = uploadResult.Error });

            // 2. Tìm variant đầu tiên của product, cập nhật ImageUrl
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.ProductId == id);

            if (variant == null)
                return NotFound(new { Success = false, Message = "Product or variant not found" });

            // 3. Xóa ảnh cũ nếu tồn tại trên server
            DeleteOldImage(variant.ImageUrl);

            variant.ImageUrl = uploadResult.Url!;
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, ImageUrl = variant.ImageUrl });
        }

        // DELETE /api/product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            return Ok(new { Success = true, Message = "Product deleted successfully" });
        }

        // ─── Helper methods ───────────────────────────────────────────────────────

        private async Task<(string? Url, string? Error)> SaveImageAsync(IFormFile file)
        {
            // Kiểm tra kích thước
            if (file.Length > MaxFileSizeBytes)
                return (null, $"File vượt quá giới hạn {MaxFileSizeBytes / 1024 / 1024} MB");

            // Kiểm tra phần mở rộng
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(ext))
                return (null, $"Định dạng không được phép. Chỉ chấp nhận: {string.Join(", ", _allowedExtensions)}");

            // Tạo thư mục nếu chưa có
            var uploadDir = Path.Combine(_env.WebRootPath, "images", "products");
            Directory.CreateDirectory(uploadDir);

            // Tên file unique để tránh trùng
            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadDir, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Trả về URL tương đối để lưu DB
            var url = $"/images/products/{fileName}";
            return (url, null);
        }

        private void DeleteOldImage(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;
            var oldPath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);
        }
    }
}

