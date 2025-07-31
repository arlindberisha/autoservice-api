namespace AutoServiceAPI.Services
{
    public interface IBillNumberService
    {
        Task<string> GenerateNextBillNumberAsync();
    }
}