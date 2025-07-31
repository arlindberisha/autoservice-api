namespace AutoServiceAPI.DTOs
{
    public class AuthResponse
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string OrganisationId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public OrganisationDto? Organisation { get; set; }
    }

    public class OrganisationDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionDueDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}