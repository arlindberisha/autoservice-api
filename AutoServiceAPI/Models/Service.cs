using System.ComponentModel.DataAnnotations;

namespace AutoServiceAPI.Models
{
    public class Service
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<BillService> BillServices { get; set; } = new List<BillService>();
    }
} 