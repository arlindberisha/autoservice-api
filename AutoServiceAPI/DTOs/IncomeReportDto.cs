namespace AutoServiceAPI.DTOs
{
    public class IncomeReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalIncome { get; set; }
        public int TotalBills { get; set; }
        public decimal TotalDiscountGiven { get; set; }
    }
}