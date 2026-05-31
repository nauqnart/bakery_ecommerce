using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StitchArtisan.Backend.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("category_id")]
        public int? CategoryId { get; set; }

        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Column("slug")]
        [StringLength(191)]
        public string Slug { get; set; } = string.Empty;

        [Column("base_description")]
        public string? BaseDescription { get; set; }

        [Column("tier_variations_json")]
        public string? TierVariationsJson { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}
