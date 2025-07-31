using System.ComponentModel.DataAnnotations;

namespace AutoServiceAPI.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = string.Empty; // "Organisation Owner" or "Employee"
        
        [Required]
        public string OrganisationId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Organisation Organisation { get; set; } = null!;
        public virtual ICollection<Bill> BillsCreated { get; set; } = new List<Bill>();
    }
} 