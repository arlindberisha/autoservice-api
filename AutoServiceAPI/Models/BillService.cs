using System.ComponentModel.DataAnnotations;

namespace AutoServiceAPI.Models
{
    public class BillService
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string BillId { get; set; } = string.Empty;
        
        [Required]
        public string ServiceId { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string ServiceName { get; set; } = string.Empty;
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        // Navigation properties
        public virtual Bill Bill { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
    }
} 