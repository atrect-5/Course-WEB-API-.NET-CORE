using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// Represents a user's money account (cash, debit, or credit).
    /// </summary>
    public class MoneyAccount
    {
        public int Id { get; set; }
        /// <summary>
        /// Name of the account.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Type of account: Cash, Debit, or Credit.
        /// </summary>
        public required string AccountType { get; set; }

        /// <summary>
        /// Current balance of the account.
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Credit limit (only for credit accounts).
        /// </summary>
        public decimal? CreditLimit { get; set; }

        /// <summary>
        /// Id of the user who owns the account.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// User who owns the account.
        /// </summary>
        public required User User { get; set; }

        /// <summary>
        /// Transactions associated with this account.
        /// </summary>
        public ICollection<Transaction>? Transactions { get; set; }

        /// <summary>
        /// Transfers sent from this account.
        /// </summary>
        public ICollection<Transfer>? TransfersSent { get; set; }

        /// <summary>
        /// Transfers received by this account.
        /// </summary>
        public ICollection<Transfer>? TransfersReceived { get; set; }

    }
}
