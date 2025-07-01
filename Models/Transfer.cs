using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// Represents a transfer between two money accounts.
    /// </summary>
    public class Transfer
    {
        // NOTA: Agregar que al hacer transferencia se debe restar el monto de la cuenta origen y sumarlo a la cuenta destino (Creando las transacciones)

        /// <summary>
        /// Transfer identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Amount transferred.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Date of the transfer.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Description of the transfer.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Id of the user who made the transfer.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// User who made the transfer.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Id of the receiving money account.
        /// </summary>
        public int MoneyAccountReceiveId { get; set; }

        /// <summary>
        /// Receiving money account.
        /// </summary>
        public MoneyAccount MoneyAccountReceive { get; set; } = null!;

        /// <summary>
        /// Id of the sending money account.
        /// </summary>
        public int MoneyAccountSendId { get; set; }

        /// <summary>
        /// Sending money account.
        /// </summary>
        public MoneyAccount MoneyAccountSend { get; set; } = null!;

        /// <summary>
        /// The two transactions (income and expenditure) that make up this transfer.
        /// </summary>
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
