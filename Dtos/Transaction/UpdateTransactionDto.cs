using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Transaction
{
    /// <summary>
    /// DTO for updating a transaction.
    /// </summary>
    public class UpdateTransactionDto
    {
        [Required]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
        public decimal Amount { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int MoneyAccountId { get; set; }
    }
}
