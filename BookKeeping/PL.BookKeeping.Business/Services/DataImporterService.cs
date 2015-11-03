using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PL.BookKeeping.Entities;
using System.IO;
using System.Globalization;

namespace PL.BookKeeping.Business.Services
{
    public class DataImporterService: IDataImporterService
    {
        private ITransactionDataService mTransactionDataService;

        /// <summary>Initializes a new instance of the <see cref="DataImporterService"/> class.
        /// </summary>
        /// <param name="transactionDataService">The transaction data service.</param>
        public DataImporterService(ITransactionDataService transactionDataService)
        {
            mTransactionDataService = transactionDataService;
        }

        /// <summary>Import the transaction data from a list of files.
        /// </summary>
        /// <param name="files">The files.</param>
        public void ImportFiles(IEnumerable<string>files)
        {
            foreach (var file in files)
            {
                Import(file);
            }
        }

        private bool Import(string fileName)
        {
            var reader = new StreamReader(fileName);

            // First line is a header...
            reader.ReadLine();
            var sepparators = new string[] { "\",\"" };

            Transaction transaction;

            while (!reader.EndOfStream)
            {
                var values = reader.ReadLine().Split(sepparators, 9, StringSplitOptions.None);
                transaction = ProcessLine(values);
                mTransactionDataService.Add(transaction);
            }
            return false;
        }

        private Transaction ProcessLine(string[] values)
        {
            var retValue = new Transaction();

            for (int i = 0; i < values.Count(); i++)
            {
                values[i] = values[i].Replace("\"", "");
            }

            retValue.Date = DateTime.ParseExact(values[0], "yyyyMMdd", CultureInfo.InvariantCulture);
            retValue.Name = values[1];
            retValue.Account = values[2];
            retValue.CounterAccount = values[3];
            retValue.Code = values[4];
            retValue.MutationType = values[5].ToUpper() == "AF" ? MutationType.Debit : MutationType.Credit;
            retValue.Amount = Convert.ToDecimal(values[6]);
            retValue.MutationKind = values[7];
            retValue.Remarks = values[8];

            return retValue;
        }
    }
}
