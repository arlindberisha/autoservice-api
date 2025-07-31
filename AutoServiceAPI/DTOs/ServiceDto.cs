namespace AutoServiceAPI.DTOs
{
    public class ServiceDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateServiceRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateServiceRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}