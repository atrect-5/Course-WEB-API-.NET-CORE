using Data;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class MoneyAccountRepository(ProjectDBContext context) : IMoneyAccountService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        public bool Add(MoneyAccount model)
        {
            ArgumentNullException.ThrowIfNull(model);
            _ = _dbContext.Users.Find(model.UserId) 
                ?? throw new ArgumentException($"No se encontró un usuario con el ID {model.UserId}.", nameof(model));
            
            ValidateAndPrepareAccount(model);
            
            _dbContext.MoneyAccounts.Add(model);
            _dbContext.SaveChanges();

            return true;
        }

        public bool Delete(int id)
        {
            var account = _dbContext.MoneyAccounts.Find(id);
            if (account is null)
            {
                return false;
            }

            _dbContext.MoneyAccounts.Remove(account);
            _dbContext.SaveChanges();
            return true;
        }

        public MoneyAccount? GetMoneyAccountById(int id) => 
            _dbContext.MoneyAccounts.Find(id);

        public IEnumerable<MoneyAccount> GetMoneyAccountsByUserId(int userId, string? nameFilter = null, string? typeFilter = null)
        {
            var query = _dbContext.MoneyAccounts.Where(ma => ma.UserId == userId);

            // Si se proporciona un filtro de nombre, lo aplicamos a la consulta.
            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                query = query.Where(ma => ma.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Si se proporciona un filtro de tipo, lo aplicamos también.
            if (!string.IsNullOrWhiteSpace(typeFilter))
            {
                query = query.Where(ma => ma.AccountType.Equals(typeFilter, StringComparison.OrdinalIgnoreCase));
            }

            return query.ToList();
        }

        public MoneyAccount? Update(MoneyAccount model)
        {
            ArgumentNullException.ThrowIfNull(model);
            
            ValidateAndPrepareAccount(model);
            
            var accountInDb = _dbContext.MoneyAccounts.Find(model.Id);
            if (accountInDb is null)
            {
                return null; // El controlador manejará esto como un 404 Not Found.
            }

            accountInDb.Name = model.Name;
            accountInDb.AccountType = model.AccountType;
            accountInDb.Balance = model.Balance;
            accountInDb.CreditLimit = model.CreditLimit;

            _dbContext.SaveChanges();
            return accountInDb;
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
    }
}
