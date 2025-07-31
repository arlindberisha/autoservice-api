using System.ComponentModel.DataAnnotations;

namespace AutoServiceAPI.DTOs
{
    public class UpdateOrganisationRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Location { get; set; }
    }
}