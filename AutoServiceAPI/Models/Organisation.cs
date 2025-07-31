using System.ComponentModel.DataAnnotations;

namespace AutoServiceAPI.Models
{
    public class Organisation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string? Location { get; set; }
        
        public DateTime SubscriptionStartDate { get; set; } = DateTime.UtcNow;
        
        public DateTime SubscriptionDueDate { get; set; } = DateTime.UtcNow.AddYears(1);
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
} 