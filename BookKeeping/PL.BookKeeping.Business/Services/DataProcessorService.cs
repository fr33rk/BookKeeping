using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.EventAggregatorEvents;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Logger;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PL.BookKeeping.Business.Services
{
    internal class DataProcessorService : IDataProcessorService
    {
        private readonly ILogFile mLogFile;
        private readonly IEntryPeriodDataService mEntryPeriodDataService;
        private readonly IProcessingRuleDataService mProcessingRulesDataService;
        private readonly IPeriodDataService mPeriodDataService;
        private readonly ITransactionDataService mTransactionDataService;
        private IList<ProcessingRule> mProcessingRules;
        private IList<EntryPeriod> mEntryPeriods;
        private readonly IEventAggregator mEventAggregator;

        private int mTransactionProcessedCount;
        private int mTransactionsIgnoredCount;

        public DataProcessorService(ILogFile logFile, IEntryPeriodDataService entryPeriodDataService,
            IProcessingRuleDataService processingRuleDataService, IPeriodDataService periodDataService,
            ITransactionDataService transactionDataService, IEventAggregator eventAggregator)
        {
            mLogFile = logFile;
            mProcessingRulesDataService = processingRuleDataService;
            mEntryPeriodDataService = entryPeriodDataService;
            mPeriodDataService = periodDataService;
            mTransactionDataService = transactionDataService;
            mEventAggregator = eventAggregator;
        }

        public void Process(IList<Transaction> transactions)
        {
            bool processed;

            // Prepare for processing by loading the entries and current periods.
            initializeForProcessing();

            if ((mProcessingRules != null) && (transactions != null))
            {
                var changedYears = new List<int>();

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

                                // Update the parent (total) entries
                                updateParentTotals(entryPeriod, transaction.Amount);

                                // We're done.
                                mLogFile.Info(
	                                $"Transaction: {transaction} is added to entry {entryPeriod.Entry.Description}, period {entryPeriod.Period}, due to rule {rule.Priority.ToString()}");

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

                    if (processed)
                    {
                        if (!changedYears.Contains(transaction.Date.Year))
                        {
                            changedYears.Add(transaction.Date.Year);
                        }
                    }
                    else
                    {
                        mLogFile.Warning($"There is no rule to process transaction: {transaction}");
                    }
                }
                mEventAggregator.GetEvent<DataChangedEvent>().Publish(new DataChangedEventArgs(changedYears));
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

        private void updateParentTotals(EntryPeriod entryPeriod, decimal amount)
        {
            if (entryPeriod.Entry.ParentEntryKey.HasValue)
            {
                var parentEntryPeriod = mEntryPeriods.Where(ep => (ep.EntryKey == entryPeriod.Entry.ParentEntryKey) && (ep.Period.Key == entryPeriod.Period.Key)).FirstOrDefault();

                if (parentEntryPeriod != null)
                {
                    parentEntryPeriod.TotalAmount += amount;
                    mEntryPeriodDataService.Update(parentEntryPeriod);

                    updateParentTotals(parentEntryPeriod, amount);
                }
                else
                {
                    throw new Exception($"Parent entry period of {entryPeriod.Key} does not exist");
                }
            }
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