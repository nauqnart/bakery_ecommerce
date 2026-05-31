using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StitchArtisan.Backend.Models
{
    [Table("user_addresses")]
    public class UserAddress
    {
        [Key]
        [Column("user_address_id")]
        public int UserAddressId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("full_name")]
        [StringLength(255)]
        public string FullName { get; set; } = string.Empty;

        [Column("phone")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Column("address_line")]
        [StringLength(500)]
        public string AddressLine { get; set; } = string.Empty;

        [Column("is_default")]
        public bool IsDefault { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        [JsonIgnore]
        public User? User { get; set; }
    }
}
