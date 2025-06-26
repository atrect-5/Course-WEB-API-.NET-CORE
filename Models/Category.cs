using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// Represents a transaction's category (store, house, transfer, etc).
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Category identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the category.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Type of the category (income or expenditure).
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Collection of transactions associated with this category.
        /// </summary>
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
