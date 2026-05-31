using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StitchArtisan.Backend.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; } = 3;

        [Column("email")]
        [StringLength(191)]
        public string Email { get; set; } = string.Empty;

        [Column("password_hash")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("full_name")]
        [StringLength(255)]
        public string? FullName { get; set; }

        [Column("phone")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("reset_password_token")]
        [StringLength(255)]
        public string? ResetPasswordToken { get; set; }

        [Column("reset_password_expiry")]
        public DateTime? ResetPasswordExpiry { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }

        public ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    }
}
