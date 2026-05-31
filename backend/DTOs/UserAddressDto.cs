using System.ComponentModel.DataAnnotations;

namespace StitchArtisan.Backend.DTOs
{
    public class UserAddressCreateDto
    {
        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string AddressLine { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;
    }

    public class UserAddressUpdateDto
    {
        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string AddressLine { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;
    }
}
