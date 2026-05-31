using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StitchArtisan.Backend.Data;
using StitchArtisan.Backend.DTOs;
using StitchArtisan.Backend.Services;

namespace StitchArtisan.Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly AppDbContext _context;
        private readonly IPhotoService _photoService;

        public ProductController(IProductService productService, AppDbContext context, IPhotoService photoService)
        {
            _productService = productService;
            _context = context;
            _photoService = photoService;
        }

        // GET /api/product
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] int? categoryId = null, [FromQuery] string? sortBy = null, [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null)
        {
            var result = await _productService.GetPagedProductsAsync(page, pageSize, search, categoryId, sortBy, minPrice, maxPrice);
            return Ok(new { Success = true, Data = result.Products, TotalCount = result.TotalCount, Page = page, PageSize = pageSize });
        }

        // GET /api/product/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound(new { Success = false, Message = "Product not found" });
            return Ok(new { Success = true, Data = product });
        }
        [AllowAnonymous]
        [HttpGet("generate-sku")]
        public async Task<IActionResult> GenerateSku([FromQuery] string name, [FromQuery] string? variantName)
        {
            var baseSku = GetAbbreviation(name);
            if (!string.IsNullOrWhiteSpace(variantName) && variantName != "Mặc định")
            {
                baseSku += "-" + GetAbbreviation(variantName);
            }

            if (string.IsNullOrWhiteSpace(baseSku))
            {
                return Ok(new { Success = true, Sku = "" });
            }

            baseSku = baseSku.ToUpper();
            
            // Check DB
            string finalSku = baseSku;
            int counter = 1;
            while (await _context.ProductVariants.AnyAsync(v => v.Sku == finalSku))
            {
                finalSku = baseSku + counter.ToString();
                counter++;
            }

            return Ok(new { Success = true, Sku = finalSku });
        }

        private string GetAbbreviation(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            
            // Remove diacritics
            var normalized = input.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();
            foreach (var c in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            var withoutMarks = sb.ToString().Replace('đ', 'd').Replace('Đ', 'D');

            var words = withoutMarks.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
            var result = "";
            foreach (var word in words)
            {
                var chars = word.Where(char.IsLetterOrDigit).ToArray();
                if (chars.Length > 0)
                {
                    result += chars[0];
                }
            }
            return result;
        }

        [Authorize(Roles = "Admin,admin,Staff,staff")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            try
            {
                var product = await _productService.CreateProductAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, new { Success = true, Data = product });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { Success = false, Message = "Mã sản phẩm (SKU) đã tồn tại. Vui lòng chọn mã khác." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,admin,Staff,staff")]
        [HttpPost("with-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateWithImage([FromForm] ProductCreateDto dto, IFormFile? image)
        {
            try
            {
                if (image != null)
                {
                    var uploadResult = await _photoService.AddPhotoAsync(image);
                    if (uploadResult.Error != null)
                        return BadRequest(new { Success = false, Message = uploadResult.Error.Message });
                    
                    dto.ImageUrl = uploadResult.SecureUrl.ToString();
                }

                var product = await _productService.CreateProductAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, new { Success = true, Data = product });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { Success = false, Message = "Mã sản phẩm (SKU) đã tồn tại. Vui lòng chọn mã khác." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,admin,Staff,staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            try
            {
                await _productService.UpdateProductAsync(id, dto);
                return Ok(new { Success = true, Message = "Product updated successfully" });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { Success = false, Message = "Mã sản phẩm (SKU) đã tồn tại. Vui lòng chọn mã khác." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // POST /api/product/{id}/upload-image  — upload / thay ảnh cho tất cả variant
        [Authorize(Roles = "Admin,admin,Staff,staff")]
        [HttpPost("{id}/upload-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(int id, IFormFile image)
        {
            var variants = await _context.ProductVariants
                .Where(v => v.ProductId == id).ToListAsync();

            if (!variants.Any())
                return NotFound(new { Success = false, Message = "Product or variant not found" });

            var uploadResult = await _photoService.AddPhotoAsync(image);
            if (uploadResult.Error != null)
                return BadRequest(new { Success = false, Message = uploadResult.Error.Message });

            // Áp dụng ảnh mới cho tất cả các biến thể
            foreach (var v in variants)
            {
                v.ImageUrl = uploadResult.SecureUrl.ToString();
            }
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, ImageUrl = variants[0].ImageUrl });
        }

        // DELETE /api/product/{id}
        [Authorize(Roles = "Admin,admin,Staff,staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try 
            {
                await _productService.DeleteProductAsync(id);
                return Ok(new { Success = true, Message = "Xóa sản phẩm thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}

