﻿using System;
using System.Collections.Generic;
using System.Linq;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Logger;

namespace PL.BookKeeping.Business.Services
{
    class DataProcessorService
    {
        private ILogFile mLogFile;
        private ITransactionDataService mTransactionDataService;
        //private IEntryDataService mEntryDataService;
        private IPeriodDataService mPeriodDataService;
        //private IEntryPeriodDataService mEntryPeriodDataService;
        private IList<ProcessingRule> mProcessingRules;
        private IList<EntryPeriod> mEntryPeriods;


        public DataProcessorService(ILogFile logFile, ITransactionDataService transactionService, IPeriodDataService periodDataService)
        {
            mLogFile = logFile;
            mTransactionDataService = transactionService;
            //mEntryDataService = entryDataService;
            mPeriodDataService = periodDataService;
        }

        public void Process(IList<Transaction> transactions)
        {
            bool added;

            // Prepare for processing by loading the entries and current periods.
            initializeForProcessing();

            foreach (var transaction in transactions)
            {
                added = false;
                foreach (var rule in mProcessingRules)
                {
                    if (rule.AppliesTo(transaction))
                    {
                        var period = getEntryPeriodForTransaction(transaction, rule);

                        if (period == null)
                        {
                            // A new period and entry-period combination need to be created first.
                            mPeriodDataService.AddUsingTransactionDate(transaction.Date);
                            // Reload the list of entry-date combinations.
                            getEntryPeriodList();
                            // Now get the brand new EntryPeriod combination.
                            period = getEntryPeriodForTransaction(transaction, rule);
                        }

                        period.Transactions.Add(transaction);

                        // We're done.
                        mLogFile.Info(string.Format("Transaction: {0} is added to entry {1}, period {2}, due to rule {3}", transaction.ToString(), period.Entry.Description, period.Period.ToString()));
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    mLogFile.Warning(string.Format("There is no rule to process transaction: {0}", transaction.ToString()));
                }
            }
        }

        private void initializeForProcessing()
        {
            throw new NotImplementedException();
        }

        /// <summary>(Re-)load all entry/period combinations from the database.</summary>        
        private void getEntryPeriodList()
        {
            throw new NotImplementedException();
        }

        private EntryPeriod getEntryPeriodForTransaction(Transaction transaction, ProcessingRule rule)
        {
            return mEntryPeriods.Where(p => (p.Entry.Key == rule.Entry.Key) &&
                                            (p.Period.PeriodStart >= transaction.Date) &&
                                            (p.Period.PeriodEnd < transaction.Date))
                                .FirstOrDefault();
        }


    }
}