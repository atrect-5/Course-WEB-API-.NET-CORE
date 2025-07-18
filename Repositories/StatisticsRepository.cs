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

            IQueryable<CategorySummaryDto> query = _context.Transactions
                .Where(t =>
                    t.UserId == userId &&
                    t.Category.Type == categoryType &&
                    t.Date.HasValue &&
                    t.Date.Value >= startDate
                    && t.Date.Value <= endDate)
                .GroupBy(t => new { t.CategoryId, t.Category.Name })
                .Select(g => new CategorySummaryDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(s => s.TotalAmount);

            if (limit.HasValue && limit.Value > 0)
                query = query.Take(limit.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<MonthlyFlowDto>> GetMonthlyCashFlowAsync(int userId, int numberOfMonths)
        {
            var startDate = DateTime.UtcNow.AddMonths(-numberOfMonths + 1);
            var firstDayOfStartMonth = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0, DateTimeKind.Utc); // Para que cuente el mes completo del mes de inicio

            var monthlyData = await _context.Transactions
                .Where(t => t.UserId == userId && t.Date.HasValue && t.Date >= firstDayOfStartMonth)
                .GroupBy(t => new { t.Date!.Value.Year, t.Date.Value.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalIncome = g.Where(t => t.Category.Type == "INCOME").Sum(t => t.Amount),
                    TotalExpenses = g.Where(t => t.Category.Type == "EXPENDITURE").Sum(t => t.Amount)
                })
                .ToListAsync();

            var result = new List<MonthlyFlowDto>();

            // Se rellenan los meses sin datos con 0
            for (int i = 0; i < numberOfMonths; i++)
            {
                var targetDate = DateTime.UtcNow.AddMonths(-i);
                var existingMonthData = monthlyData.FirstOrDefault(m => m.Year == targetDate.Year && m.Month == targetDate.Month);

                result.Add(new MonthlyFlowDto
                {
                    Year = targetDate.Year,
                    Month = targetDate.Month,
                    TotalIncome = existingMonthData?.TotalIncome ?? 0,
                    TotalExpenses = existingMonthData?.TotalExpenses ?? 0
                });
            }

            return result.OrderBy(r => r.Year).ThenBy(r => r.Month);
        }
    }
}
