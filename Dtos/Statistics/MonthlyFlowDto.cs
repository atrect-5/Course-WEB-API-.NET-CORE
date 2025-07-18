using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Statistics
{
    public class MonthlyFlowDto
    {
        /// <summary>
        /// The year of the summary.
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// The month of the summary (1-12).
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// The total income for the month.
        /// </summary>
        public decimal TotalIncome { get; set; }
        /// <summary>
        /// The total expenses for the month.
        /// </summary>
        public decimal TotalExpenses { get; set; }
        /// <summary>
        /// The net cash flow (TotalIncome - TotalExpenses).
        /// </summary>
        public decimal NetFlow => TotalIncome - TotalExpenses;
    }
}
