﻿using System.ComponentModel.DataAnnotations;

namespace Dtos.MoneyAccount
{
    public class UpdateMoneyAccountDto
    {
        [Required(ErrorMessage = "El nombre de la cuenta es obligatorio.")]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "El tipo de cuenta es obligatorio.")]
        public required string AccountType { get; set; }

        public decimal? CreditLimit { get; set; }
    }
}
