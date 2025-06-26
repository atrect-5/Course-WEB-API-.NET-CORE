using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// Represents an application user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// User identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the user.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Password of the user.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Collection of money accounts associated with the user.
        /// </summary>
        public ICollection<MoneyAccount>? MoneyAccounts { get; set; }

        /// <summary>
        /// Collection of transactions associated with the user.
        /// </summary>
        public ICollection<Transaction>? Transactions { get; set; }

        /// <summary>
        /// Collection of transfer records associated with the user.
        /// </summary>
        public ICollection<Transfer>? Transfer { get; set; }

        /// <summary>
        /// Collection of Categories made by the user
        /// </summary>
        public ICollection<Category>? Categories { get; set; }
    }
}
