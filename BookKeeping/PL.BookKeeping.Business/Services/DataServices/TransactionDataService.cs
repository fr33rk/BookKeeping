using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using System.Collections.Generic;
using System.Linq;

namespace PL.BookKeeping.Business.Services.DataServices
{
    public class TransactionDataService : BaseTraceableObjectDataServiceOfT<Transaction>, ITransactionDataService
    {
        public TransactionDataService(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService)
            : base(uowFactory, authorizationService)
        {
        }

        public override void Add(Transaction entity)
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