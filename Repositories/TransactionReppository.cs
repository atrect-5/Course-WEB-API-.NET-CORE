using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class TransactionReppository : ITransactionService
    {
        public bool Add(Transaction model)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transaction> GetTransactionByCategoryId(int userId, int categoryId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transaction> GetTransactionByDateRange(int userId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Transaction GetTransactionById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transaction> GetTransactionByMoneyAccountId(int moneyAccountId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transaction> GetTransactionsByUserId(int userId)
        {
            throw new NotImplementedException();
        }

        public Transaction Update(Transaction model)
        {
            throw new NotImplementedException();
        }
    }
}
