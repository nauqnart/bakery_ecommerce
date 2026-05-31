using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StitchArtisan.Backend.Models
{
    [Table("product_variants")]
    public class ProductVariant
    {
        [Key]
        [Column("variant_id")]
        public int VariantId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("sku")]
        [StringLength(100)]
        public string Sku { get; set; } = string.Empty;

        [Column("variant_name")]
        [StringLength(200)]
        public string VariantName { get; set; } = "Mặc định";

        [Column("tier_values_json")]
        public string? TierValuesJson { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("stock_qty")]
        public int StockQty { get; set; } = 0;

        [Column("image_url")]
        [StringLength(512)]
        public string? ImageUrl { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}
