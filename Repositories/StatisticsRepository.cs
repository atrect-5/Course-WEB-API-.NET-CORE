using Data;
using Dtos.Statistics;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Repositories
{
    public class StatisticsRepository(ProjectDBContext context) : IStatisticsService
    {
        private readonly ProjectDBContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<FinancialSummaryDto?> GetSummaryByUserIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
                return null;

            var summary = await _context.MoneyAccounts
                .Where(ma => ma.UserId == userId)
                .GroupBy(ma => ma.AccountType)
                .Select(group => new FinancialSummaryDto
                {
                    TotalCashBalance = group.Key != "CREDIT" ? group.Sum(ma => ma.Balance) : 0,
                    TotalCreditLimit = group.Key == "CREDIT" ? group.Sum(ma => ma.CreditLimit ?? 0) : 0,
                    TotalCreditUsed = group.Key == "CREDIT" ? group.Sum(ma => ma.Balance) : 0
                })
                .ToListAsync();

            return new FinancialSummaryDto
            {
                TotalCashBalance = summary.Sum(s => s.TotalCashBalance),
                TotalCreditLimit = summary.Sum(s => s.TotalCreditLimit),
                TotalCreditUsed = summary.Sum(s => s.TotalCreditUsed)
            };
        }

        public async Task<IEnumerable<CategorySummaryDto>> GetSummaryByCategoryAsync(int userId, string categoryType, DateTime startDate, DateTime endDate, int? limit = null)
        {
            if (categoryType != "INCOME" && categoryType != "EXPENDITURE")
                throw new ArgumentException("El tipo de categoría debe ser 'INCOME' o 'EXPENDITURE'.", nameof(categoryType));

            var query = await _context.Transactions
                .Where(t =>
                    t.UserId == userId &&
                    t.Category.Type == categoryType &&
                    t.Date >= startDate &&
                    t.Date <= endDate)
                .GroupBy(t => new { t.CategoryId, t.Category.Name })
                .Select(g => new CategorySummaryDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(s => s.TotalAmount)
                .ToListAsync();

            return query;
        }
    }
}
