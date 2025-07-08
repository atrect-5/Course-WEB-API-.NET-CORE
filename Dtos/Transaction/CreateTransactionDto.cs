using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Transaction
{
    /// <summary>
    /// DTO for creating a new transaction.
    /// </summary>
    public class CreateTransactionDto
    {
        [Required]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
        public decimal Amount { get; set; }
        public DateTime? Date { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int MoneyAccountId { get; set; }
    }
}
