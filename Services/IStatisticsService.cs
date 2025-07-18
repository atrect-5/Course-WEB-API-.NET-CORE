using Dtos.Statistics;

namespace Services
{
    public interface IStatisticsService
    {
        /// <summary>
        /// Calculates a summary of category data (income or spending) for a specific user within a date range.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="categoryType">The type of the category (INCOME or EXPENDITURE).</param>
        /// <param name="startDate">The start date of the period.</param>
        /// <param name="endDate">The end date of the period.</param>
        /// <returns>A list of categories with their corresponding total spending.</returns>
        Task<IEnumerable<CategorySummaryDto>> GetSummaryByCategoryAsync(int userId, string categoryType, DateTime startDate, DateTime endDate, int? limit = null);

        /// <summary>
        /// Calculates and retrieves the financial summary for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A DTO containing the user's financial summary.</returns>
        Task<FinancialSummaryDto?> GetSummaryByUserIdAsync(int userId);
    }
}
