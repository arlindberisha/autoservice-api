namespace AutoServiceAPI.DTOs
{
    public class CarDto
    {
        public string Id { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string? Color { get; set; }
        public string Vin { get; set; } = string.Empty;
        public string? LicensePlate { get; set; }
        public string? InsuranceNumber { get; set; }
        public int? Mileage { get; set; }
        public string? EngineType { get; set; }
        public string? Transmission { get; set; }
        public int? NumberOfDoors { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCarRequest
    {
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string? Color { get; set; }
        public string Vin { get; set; } = string.Empty;
        public string? LicensePlate { get; set; }
        public string? InsuranceNumber { get; set; }
        public int? Mileage { get; set; }
        public string? EngineType { get; set; }
        public string? Transmission { get; set; }
        public int? NumberOfDoors { get; set; }
        public string ClientId { get; set; } = string.Empty;
    }

    public class UpdateCarRequest
    {
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string? Color { get; set; }
        public string Vin { get; set; } = string.Empty;
        public string? LicensePlate { get; set; }
        public string? InsuranceNumber { get; set; }
        public int? Mileage { get; set; }
        public string? EngineType { get; set; }
        public string? Transmission { get; set; }
        public int? NumberOfDoors { get; set; }
        public string ClientId { get; set; } = string.Empty;
    }
}