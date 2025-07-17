using Dtos.Statistics;

namespace Services
{
    public interface IFinancialSummaryService
    {
        /// <summary>
        /// Calculates and retrieves the financial summary for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A DTO containing the user's financial summary.</returns>
        Task<FinancialSummaryDto?> GetSummaryByUserIdAsync(int userId);
    }
}
