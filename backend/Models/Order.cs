using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StitchArtisan.Backend.Models
{
    [Table("orders")]
    public class Order
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("order_status")]
        [StringLength(50)]
        public string OrderStatus { get; set; } = "pending";

        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        [Column("shipping_address")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Column("note")]
        public string? Note { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("payment_method")]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "cod";

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Invoice? Invoice { get; set; }
    }

    [Table("order_items")]
    public class OrderItem
    {
        [Key]
        [Column("order_item_id")]
        public int OrderItemId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("variant_id")]
        public int VariantId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price_at_purchase")]
        public decimal PriceAtPurchase { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [ForeignKey("VariantId")]
        public ProductVariant? Variant { get; set; }
    }
}
