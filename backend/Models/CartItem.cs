using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StitchArtisan.Backend.Models
{
    [Table("cart_items")]
    public class CartItem
    {
        [Key]
        [Column("cart_item_id")]
        public int CartItemId { get; set; }

        [Column("cart_id")]
        public int CartId { get; set; }

        [Column("variant_id")]
        public int VariantId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CartId")]
        public Cart? Cart { get; set; }

        [ForeignKey("VariantId")]
        public ProductVariant? Variant { get; set; }
    }
}
