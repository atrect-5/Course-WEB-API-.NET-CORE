using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class TransferRepository : ITransferService
    {
        public bool Add(Transfer model)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transfer> GetTransferByDateRange(int userId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Transfer GetTransferById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transfer> GetTransfersByUserId(int userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transfer> GetTransfersReceivedByIdAccount(int moneyAccountId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transfer> GetTransfersSentByIdAccount(int moneyAccountId)
        {
            throw new NotImplementedException();
        }

        public Transfer Update(Transfer model)
        {
            throw new NotImplementedException();
        }
    }
}
