using System.ComponentModel.DataAnnotations;

namespace StitchArtisan.Backend.DTOs
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal MinOrderValue { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class CouponCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string DiscountType { get; set; } = "percent"; // "percent" or "fixed"

        [Required]
        public decimal DiscountValue { get; set; }

        public decimal MinOrderValue { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }
    }

    public class CouponApplyDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;
        public decimal OrderTotal { get; set; }
    }
}
