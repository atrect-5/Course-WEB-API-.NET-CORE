﻿using System.ComponentModel.DataAnnotations;

namespace Dtos.MoneyAccount
{
    public class CreateMoneyAccountDto
    {
        [Required(ErrorMessage = "El nombre de la cuenta es obligatorio.")]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "El tipo de cuenta es obligatorio.")]
        public required string AccountType { get; set; }

        public decimal Balance { get; set; } = 0;

        public decimal? CreditLimit { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public int UserId { get; set; }
    }
}
