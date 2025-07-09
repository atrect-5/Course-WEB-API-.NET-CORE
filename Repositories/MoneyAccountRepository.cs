using Data;
using Dtos.MoneyAccount;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;

namespace Repositories
{
    public class MoneyAccountRepository(ProjectDBContext context) : IMoneyAccountService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        public async Task<MoneyAccountDto> AddAsync(CreateMoneyAccountDto model)
        {
            ArgumentNullException.ThrowIfNull(model);
            _ = await _dbContext.Users.FindAsync(model.UserId)
                ?? throw new ArgumentException($"No se encontró un usuario con el ID {model.UserId}.", nameof(model));

            var moneyAccount = new MoneyAccount
            {
                Name = model.Name,
                AccountType = model.AccountType,
                Balance = model.Balance,
                CreditLimit = model.CreditLimit,
                UserId = model.UserId
            };
            ValidateAndPrepareAccount(moneyAccount);

            await _dbContext.MoneyAccounts.AddAsync(moneyAccount);
            await _dbContext.SaveChangesAsync();

            return MapToDto(moneyAccount);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var account = await _dbContext.MoneyAccounts.FindAsync(id);
            if (account is null)
                return false;

            _dbContext.MoneyAccounts.Remove(account);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<MoneyAccountDto?> GetMoneyAccountByIdAsync(int id)
        {
            var account = await _dbContext.MoneyAccounts.FindAsync(id);
            if (account is null)
                return null;
            
            return MapToDto(account);
        }

        public async Task<IEnumerable<MoneyAccountDto>> GetMoneyAccountsByUserIdAsync(int userId, string? nameFilter = null, string? typeFilter = null)
        {
            var query = _dbContext.MoneyAccounts.Where(ma => ma.UserId == userId);

            // Si se proporciona un filtro de nombre, lo aplicamos a la consulta.
            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(ma => ma.Name.ToLower().Contains(nameFilter.ToLower()));

            // Si se proporciona un filtro de tipo, lo aplicamos también.
            if (!string.IsNullOrWhiteSpace(typeFilter))
                query = query.Where(ma => ma.AccountType.ToLower().Equals(typeFilter.ToLower()));

            return await query.Select(ma => MapToDto(ma)).ToListAsync();
        }

        public async Task<MoneyAccountDto?> UpdateAsync(int id, UpdateMoneyAccountDto  model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var accountInDb = await _dbContext.MoneyAccounts.FindAsync(id);
            if (accountInDb is null)
                return null; 

            accountInDb.Name = model.Name;
            accountInDb.AccountType = model.AccountType;
            accountInDb.CreditLimit = model.CreditLimit;

            ValidateAndPrepareAccount(accountInDb);
            await _dbContext.SaveChangesAsync();
            return MapToDto(accountInDb);
        }

        /// <summary>
        /// Validates the account type and prepares the account model for further processing.
        /// </summary>
        /// <param name="model">The <see cref="MoneyAccount"/> instance to validate and prepare.  The account type must be one of the valid
        /// account types defined in <see cref="MoneyAccount.ValidAccountTypes"/>.</param>
        /// <exception cref="ArgumentException">Thrown if the account type is invalid or if the credit limit is null for accounts of type 'CREDIT'.</exception>
        private static void ValidateAndPrepareAccount(MoneyAccount model)
        {
            model.AccountType = model.AccountType.ToUpper();

            if (!MoneyAccount.ValidAccountTypes.Contains(model.AccountType))
            {
                string validTypesString = string.Join("', '", MoneyAccount.ValidAccountTypes);
                throw new ArgumentException($"El tipo de cuenta debe ser uno de los siguientes: '{validTypesString}'.", nameof(model));
            }

            if (model.AccountType != "CREDIT")
            {
                model.CreditLimit = null;
            }
            else if (model.CreditLimit is null)
            {
                throw new ArgumentException("El límite de crédito no puede ser nulo para cuentas de tipo 'CREDIT'.", nameof(model));
            }
        }
        
        /// <summary>
        /// Maps a <see cref="MoneyAccount"/> instance to a <see cref="MoneyAccountDto"/> instance.
        /// </summary>
        /// <param name="account">The <see cref="MoneyAccount"/> object to map. Must not be <see langword="null"/>.</param>
        /// <returns>A <see cref="MoneyAccountDto"/> object containing the mapped data from the specified <see
        /// cref="MoneyAccount"/>.</returns>
        private static MoneyAccountDto MapToDto(MoneyAccount account)
        {
            return new MoneyAccountDto
            {
                Id = account.Id,
                Name = account.Name,
                AccountType = account.AccountType,
                Balance = account.Balance,
                CreditLimit = account.CreditLimit,
                UserId = account.UserId
            };
        }
    }
}
