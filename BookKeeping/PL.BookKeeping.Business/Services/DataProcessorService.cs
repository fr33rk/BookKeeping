using System.Collections.Generic;
using System.Linq;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Logger;
using System;
using PL.BookKeeping.Infrastructure;

namespace PL.BookKeeping.Business.Services
{
    internal class DataProcessorService : IDataProcessorService
    {
        private ILogFile mLogFile;
        private IEntryPeriodDataService mEntryPeriodDataService;
        private IProcessingRuleDataService mProcessingRulesDataService;
        private IPeriodDataService mPeriodDataService;
        private ITransactionDataService mTransactionDataService;
        private IList<ProcessingRule> mProcessingRules;
        private IList<EntryPeriod> mEntryPeriods;

        private int mTransactionProcessedCount;
        private int mTransactionsIgnoredCount;

        public DataProcessorService(ILogFile logFile, IEntryPeriodDataService entryPeriodDataService,
            IProcessingRuleDataService processingRuleDataService, IPeriodDataService periodDataService,
            ITransactionDataService transactionDataService)
        {
            mLogFile = logFile;
            mProcessingRulesDataService = processingRuleDataService;
            mEntryPeriodDataService = entryPeriodDataService;
            mPeriodDataService = periodDataService;
            mTransactionDataService = transactionDataService;
        }

        public void Process(IList<Transaction> transactions)
        {
            bool processed;

            // Prepare for processing by loading the entries and current periods.
            initializeForProcessing();

            if (mProcessingRules != null)
            {
                foreach (var transaction in transactions)
                {
                    processed = false;
                    foreach (var rule in mProcessingRules)
                    {
                        if (rule.AppliesTo(transaction))
                        {
                            // When the entry is null then the transaction should be ignored.
                            if (rule.Entry != null)
                            {
                                var entryPeriod = getEntryPeriodForTransaction(transaction, rule);

                                if (entryPeriod == null)
                                {
                                    // A new period and entry-period combination need to be created first.
                                    mPeriodDataService.AddUsingTransactionDate(transaction.Date);
                                    // Reload the list of entry-date combinations.
                                    getEntryPeriodList();
                                    // Now get the brand new EntryPeriod combination.
                                    entryPeriod = getEntryPeriodForTransaction(transaction, rule);
                                }

                                transaction.EntryPeriodKey = entryPeriod.Key;
                                transaction.EntryPeriod = entryPeriod;

                                entryPeriod.TotalAmount += transaction.Amount;

                                mTransactionDataService.Update(transaction);
                                mEntryPeriodDataService.Update(entryPeriod);

                                // We're done.
                                mLogFile.Info(string.Format("Transaction: {0} is added to entry {1}, period {2}, due to rule {3}", transaction.ToString(), entryPeriod.Entry.Description, entryPeriod.Period.ToString(), rule.Priority.ToString()));

                                mTransactionProcessedCount++;                                
                            }
                            else
                            {
                                mTransactionsIgnoredCount++;
                            }

                            processed = true;
                            signalDataProcessed();

                            break;
                        }
                    }

                    if (!processed)
                    {
                        mLogFile.Warning(string.Format("There is no rule to process transaction: {0}", transaction.ToString()));
                    }
                }
            }
        }

        private void initializeForProcessing()
        {
            mTransactionProcessedCount = 0;
            mTransactionsIgnoredCount = 0;
            mProcessingRules = mProcessingRulesDataService.GetAllSorted();
            getEntryPeriodList();
        }

        /// <summary>(Re-)load all entry/period combinations from the database.</summary>
        private void getEntryPeriodList()
        {
            mEntryPeriods = mEntryPeriodDataService.GetAll(true);
        }

        private EntryPeriod getEntryPeriodForTransaction(Transaction transaction, ProcessingRule rule)
        {
            return mEntryPeriods.Where(p => (p.Entry.Key == rule.Entry.Key) &&
                                            (p.Period.PeriodStart <= transaction.Date) &&
                                            (p.Period.PeriodEnd > transaction.Date))
                                .FirstOrDefault();
        }



        #region Event DataProcessed

        /// <summary>Occurs when a transaction had been processed. </summary>
        public event EventHandler<DataProcessedEventArgs> OnDataProcessed;

        /// <summary>Signals to raise the OnDataProcessed event.</summary>
        private void signalDataProcessed()
        {
            var handler = OnDataProcessed;

            if (handler != null)
            {
                handler(this, new DataProcessedEventArgs(mTransactionProcessedCount, mTransactionsIgnoredCount));
            }
        }

        #endregion Event DataProcessed

    }
}