using System.ComponentModel.DataAnnotations;

namespace AutoServiceAPI.Models
{
    public class Bill
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [MaxLength(20)]
        public string BillNumber { get; set; } = string.Empty;
        
        [Required]
        public string ClientId { get; set; } = string.Empty;
        
        [Required]
        public string CarId { get; set; } = string.Empty;
        
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Subtotal { get; set; }
        
        public decimal? DiscountValue { get; set; }
        
        [MaxLength(20)]
        public string? DiscountType { get; set; } // "percentage" or "fixed"
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        public string CreatedById { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Client Client { get; set; } = null!;
        public virtual Car Car { get; set; } = null!;
        public virtual User CreatedBy { get; set; } = null!;
        public virtual ICollection<BillService> BillServices { get; set; } = new List<BillService>();
    }
} 