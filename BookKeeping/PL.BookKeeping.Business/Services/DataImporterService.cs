using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PL.BookKeeping.Entities;

namespace PL.BookKeeping.Business.Services
{
    public class DataImporterService: IDataImporterService
    {
        private ITransactionDataService mTransactionDataService;

        public DataImporterService(ITransactionDataService transactionDataService)
        {
            mTransactionDataService = transactionDataService;
        }

        public void ImportFiles(IEnumerable<string>files)
        {
            var t = new Transaction();
            mTransactionDataService.Add(t);
        }
    }
}
