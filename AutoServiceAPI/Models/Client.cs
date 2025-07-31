using System.ComponentModel.DataAnnotations;

namespace AutoServiceAPI.Models
{
    public class Client
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
    }
} 