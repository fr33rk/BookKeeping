using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Logger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PL.BookKeeping.Business.Services
{
    /// <summary>
    /// Why isn't  this on one line
    /// </summary>
    public class DataImporterService : IDataImporterService
    {
        private ITransactionDataService mTransactionDataService;
        private ILogFile mLogFile;

        /// <summary>Initializes a new instance of the <see cref="DataImporterService"/> class.
        /// </summary>
        /// <param name="transactionDataService">The transaction data service.</param>
        public DataImporterService(ITransactionDataService transactionDataService, ILogFile logFile)
        {
            mTransactionDataService = transactionDataService;
            mLogFile = logFile;
        }

        /// <summary>Occurs when a line in a imported file has been processed.</summary>
        public event EventHandler<DataImportedEventArgs> OnDataProcessed;

        private void signalDataProcessed()
        {
            var handler = OnDataProcessed;
            if (handler != null)
            {
                handler(this, new DataImportedEventArgs(mImported, mDuplicate));
            }
        }

        private int mImported;
        private int mDuplicate;

        /// <summary>Import the transaction data from a list of files.
        /// </summary>
        /// <param name="files">The files.</param>
        public IList<Transaction> ImportFiles(IEnumerable<string> files)
        {
            var retValue = new List<Transaction>();

            mImported = 0;
            mDuplicate = 0;

            foreach (var file in files)
            {
                retValue = retValue.Concat(Import(file)).ToList();
            }

            return retValue;
        }

        private IList<Transaction> Import(string fileName)
        {
            var retValue = new List<Transaction>();
            var reader = new StreamReader(fileName);

            // First line is a header...
            reader.ReadLine();
            var sepparators = new string[] { "\",\"" };

            Transaction transaction;

            try
            {
                while (!reader.EndOfStream)
                {
                    var values = reader.ReadLine().ToUpper().Split(sepparators, 9, StringSplitOptions.None);
                    transaction = ProcessLine(values);
                    if (mTransactionDataService.Add(transaction))
                    {
                        mImported++;
                        retValue.Add(transaction);
                    }
                    else
                    {
                        mDuplicate++;
                    }
                    signalDataProcessed();
                }
            }
            catch (Exception e)
            {
                mLogFile.Error(string.Format("Unable to import {0}. The following exception occurred: {1}", fileName, e.Message));
            }

            return retValue;
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

            if (retValue.MutationType == MutationType.Debit)
            {
                retValue.Amount *= -1;
            }

            return retValue;
        }
    }
}