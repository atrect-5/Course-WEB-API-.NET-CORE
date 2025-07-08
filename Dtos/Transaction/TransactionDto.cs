using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Transaction
{
    /// <summary>
    /// DTO for returning a new transaction.
    /// </summary>
    public class TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int MoneyAccountId { get; set; }
        public int? TransferId { get; set; }
    }
}
