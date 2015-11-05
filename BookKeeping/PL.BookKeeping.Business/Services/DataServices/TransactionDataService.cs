using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Logger;
using System.Collections.Generic;
using System.Linq;

namespace PL.BookKeeping.Business.Services.DataServices
{
    public class TransactionDataService : BaseTraceableObjectDataServiceOfT<Transaction>, ITransactionDataService
    {
        private ILogFile mLogFile;

        public TransactionDataService(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService, ILogFile logFile)
            : base(uowFactory, authorizationService)
        {
            mLogFile = logFile;
        }

        new public bool Add(Transaction entity)
        {
            bool add = true;

            // Check if the transaction is already in the database.
            var currentTransaction = GetByFingerpint(entity.FingerPrint);

            foreach (var transaction in currentTransaction)
            {
                if (entity.IsEqual(transaction))
                {
                    add = false;
                    break;
                }
            }

            if (add)
            {
                base.Add(entity);
            }
            else
            {
                mLogFile.Info(string.Format("Found duplicate transaction: {0}", entity.ToString()));
            }

            return add;
        }

        public IList<Transaction> GetByFingerpint(int fingerPrint)
        {
            using (var unitOfWork = this.mUOWFactory.Create())
            {
                var qry = unitOfWork.GetRepository<Transaction>().GetQuery()
                    .Where(t => t.FingerPrint == fingerPrint);

                qry = CompleteQry(qry);

                return qry.ToList();
            }
        }
    }
}