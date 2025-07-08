using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// Represents a financial transaction made by a user.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Transaction identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Amount of the transaction.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Date of the transaction.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Description of the transaction.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Id of the user who made the transaction.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// User who made the transaction.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Id of the category of the transaction.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Category of the transaction.
        /// </summary>
        public Category Category { get; set; } = null!;

        /// <summary>
        /// Id of the money account used in the transaction.
        /// </summary>
        public int MoneyAccountId { get; set; }

        /// <summary>
        /// Money account used in the transaction.
        /// </summary>
        public MoneyAccount MoneyAccount { get; set; } = null!;

        /// <summary>
        /// Optional. Foreign key for the Transfer.
        /// </summary>
        public int? TransferId { get; set; }

        /// <summary>
        /// Optional. The transfer associated with this transaction (Only when the transaction was made by a transfer).
        /// </summary>
        public Transfer? Transfer { get; set; }
    }
}
