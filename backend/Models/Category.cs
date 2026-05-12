using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StitchArtisan.Backend.Models
{
    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("parent_id")]
        public int? ParentId { get; set; }

        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Column("slug")]
        [StringLength(191)]
        public string Slug { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [ForeignKey("ParentId")]
        public Category? ParentCategory { get; set; }
        
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
