namespace Dtos.Statistics
{
    public class FinancialSummaryDto
    {
        /// <summary>
        /// The total available balance across all non-credit accounts.
        /// </summary>
        public decimal TotalCashBalance { get; set; }

        /// <summary>
        /// The total credit limit across all credit accounts.
        /// </summary>
        public decimal TotalCreditLimit { get; set; }

        /// <summary>
        /// The total used credit, which is the sum of balances on all credit accounts.
        /// </summary>
        public decimal TotalCreditUsed { get; set; }
    }
}
