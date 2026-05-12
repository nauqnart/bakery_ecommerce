using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StitchArtisan.Backend.Models
{
    [Table("invoices")]
    public class Invoice
    {
        [Key]
        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("invoice_number")]
        [StringLength(100)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Column("billing_name")]
        [StringLength(255)]
        public string? BillingName { get; set; }

        [Column("billing_email")]
        [StringLength(191)]
        public string? BillingEmail { get; set; }

        [Column("tax_code")]
        [StringLength(50)]
        public string? TaxCode { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("tax_amount")]
        public decimal TaxAmount { get; set; } = 0;

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("payment_status")]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "unpaid";

        [Column("payment_method")]
        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
    }
}
