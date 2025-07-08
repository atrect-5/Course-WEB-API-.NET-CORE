namespace Dtos.MoneyAccount
{
    public class MoneyAccountDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string AccountType { get; set; }
        public decimal Balance { get; set; }
        public decimal? CreditLimit { get; set; }
        public int UserId { get; set; }
    }
}
