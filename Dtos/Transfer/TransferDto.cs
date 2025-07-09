using Dtos.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Transfer
{
    /// <summary>
    /// DTO for returning a transfer.
    /// </summary>
    public class TransferDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
        public int MoneyAccountSendId { get; set; }
        public int MoneyAccountReceiveId { get; set; }

        /// <summary>
        /// The two transactions (income and expenditure) that make up this transfer.
        /// </summary>
        public ICollection<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
    }
}
