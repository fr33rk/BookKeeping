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

        public override Transaction AttachEntities(IUnitOfWork unitOfWork, Transaction entity)
        {
            if (entity.EntryPeriod != null)
            {
                var periods = unitOfWork.GetRepository<EntryPeriod>();
                entity.EntryPeriod = periods.FirstOrDefault(e => e.Key == entity.EntryPeriod.Key);
            }
            return base.AttachEntities(unitOfWork, entity);
        }

        public IEnumerable<Transaction> GetByEntryPeriod(EntryPeriod entryPeriod)
        {
            if (entryPeriod != null)
            {
                using (var unitOfWork = this.mUOWFactory.Create())
                {
                    var repository = unitOfWork.GetRepository<Transaction>();
                    var retValue = repository.GetQuery()
                        .Where(e => e.EntryPeriodKey == entryPeriod.Key);

                    return retValue.ToList();
                }
            }

            return null;
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

        public void ResetPeriodEntryLinks()
        {
            using (var unitOfWork = this.mUOWFactory.Create())
            {
                unitOfWork.GetRepository<Transaction>().ExecuteProcedure("RESET_TRANSACTIONS", new object[0]);
            }
        }
    }
}