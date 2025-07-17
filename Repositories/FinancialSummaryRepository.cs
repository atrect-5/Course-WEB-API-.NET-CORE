using Data;
using Dtos.Statistics;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Repositories
{
    public class FinancialSummaryRepository(ProjectDBContext context) : IFinancialSummaryService
    {
        private readonly ProjectDBContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<FinancialSummaryDto?> GetSummaryByUserIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync();
            if (user is null)
                return null;

            var userAccounts = _context.MoneyAccounts.Where(ma => ma.UserId == userId);

            var summary = new FinancialSummaryDto
            {
                TotalCashBalance = await userAccounts
                    .Where(ma => ma.AccountType != "CREDIT")
                    .SumAsync(ma => ma.Balance),

                TotalCreditLimit = await userAccounts
                    .Where(ma => ma.AccountType == "CREDIT")
                    .SumAsync(ma => ma.CreditLimit ?? 0),

                TotalCreditUsed = await userAccounts
                    .Where(ma => ma.AccountType == "CREDIT")
                    .SumAsync(ma => ma.Balance)
            };

            return summary;
        }
    }
}
