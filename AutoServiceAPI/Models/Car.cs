using System.ComponentModel.DataAnnotations;

namespace AutoServiceAPI.Models
{
    public class Car
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [MaxLength(50)]
        public string Make { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Model { get; set; } = string.Empty;
        
        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; }
        
        [MaxLength(30)]
        public string? Color { get; set; }
        
        [Required]
        [MaxLength(17)]
        public string Vin { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? LicensePlate { get; set; }
        
        [MaxLength(50)]
        public string? InsuranceNumber { get; set; }
        
        [Range(0, 999999)]
        public int? Mileage { get; set; }
        
        [MaxLength(50)]
        public string? EngineType { get; set; }
        
        [MaxLength(30)]
        public string? Transmission { get; set; }
        
        [Range(1, 10)]
        public int? NumberOfDoors { get; set; }
        
        [Required]
        public string ClientId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Client Client { get; set; } = null!;
        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
    }
} 