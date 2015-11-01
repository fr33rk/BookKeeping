using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.BookKeeping.Business.Services.DataServices
{
    public class TransactionDataService : BaseTraceableObjectDataServiceOfT<Transaction>, ITransactionDataService
    {
        public TransactionDataService(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService)
			: base(uowFactory, authorizationService)
		{

        }
    }
}
