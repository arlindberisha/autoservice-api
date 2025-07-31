using System.ComponentModel.DataAnnotations;

namespace AutoServiceAPI.DTOs
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength(100)]
        public string OrganisationName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string OwnerFirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string OwnerLastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string OwnerPhone { get; set; } = string.Empty;
    }
}