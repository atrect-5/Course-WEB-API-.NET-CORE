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
        public const string IncomeType = "INCOME";
        public const string ExpenditureType = "EXPENDITURE";

        /// <summary>
        /// Category identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the category.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Type of the category (income or expenditure).
        /// </summary>
        public required string Type { get; set; }

        /// <summary>
        /// Creator of the category (Null represents global Category)
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// User who create the category (Only appears to him)
        /// </summary>
        public User? User { get; set; }


        /// <summary>
        /// Collection of transactions associated with this category.
        /// </summary>
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
