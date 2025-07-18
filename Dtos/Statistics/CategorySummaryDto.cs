using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Statistics
{
    public class CategorySummaryDto
    {
        /// <summary>
        /// The ID of the category.
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// The name of the category.
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;
        /// <summary>
        /// The total amount for this category (can be income or expenditure) for the given period.
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
