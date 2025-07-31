using Microsoft.EntityFrameworkCore;
using AutoServiceAPI.Data;

namespace AutoServiceAPI.Services
{
    public class BillNumberService : IBillNumberService
    {
        private readonly AutoServiceDbContext _context;

        public BillNumberService(AutoServiceDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateNextBillNumberAsync()
        {
            var currentYear = DateTime.UtcNow.Year;
            var yearPrefix = currentYear.ToString();
            
            // Get the last bill number for the current year
            var lastBill = await _context.Bills
                .Where(b => b.BillNumber.StartsWith(yearPrefix))
                .OrderByDescending(b => b.BillNumber)
                .FirstOrDefaultAsync();

            int nextSequence = 1;
            if (lastBill != null)
            {
                // Extract the sequence number from the last bill number (format: YYYY-NNNN)
                var lastSequencePart = lastBill.BillNumber.Substring(5); // Remove "YYYY-" part
                if (int.TryParse(lastSequencePart, out int lastSequence))
                {
                    nextSequence = lastSequence + 1;
                }
            }

            return $"{yearPrefix}-{nextSequence:D4}"; // Format: 2025-0001
        }
    }
}