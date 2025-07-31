namespace AutoServiceAPI.DTOs
{
    public class BillDto
    {
        public string Id { get; set; } = string.Empty;
        public string BillNumber { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string CarId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<ServicePerformedDto> ServicesPerformed { get; set; } = new List<ServicePerformedDto>();
        public decimal Subtotal { get; set; }
        public DiscountDto? Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public string CreatedById { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ServicePerformedDto
    {
        public string ServiceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class DiscountDto
    {
        public string Type { get; set; } = string.Empty; // "percentage" or "fixed"
        public decimal Value { get; set; }
    }

    public class CreateBillRequest
    {
        public string ClientId { get; set; } = string.Empty;
        public string CarId { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public List<ServicePerformedDto> ServicesPerformed { get; set; } = new List<ServicePerformedDto>();
        public DiscountDto? Discount { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateBillRequest
    {
        public string ClientId { get; set; } = string.Empty;
        public string CarId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<ServicePerformedDto> ServicesPerformed { get; set; } = new List<ServicePerformedDto>();
        public DiscountDto? Discount { get; set; }
        public string? Notes { get; set; }
    }
}