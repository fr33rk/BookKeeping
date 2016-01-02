using AutoMapper;
using BookKeeping.Client.Models;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using Prism.Commands;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace BookKeeping.Client.ViewModels
{
    public class PeriodSelectionVM
    {
        public PeriodSelectionVM(int? year, string description)
        {
            Year = year;
            Description = description;
        }

        public int? Year { get; private set; }
        public string Description { get; private set; }
    }

    public class DefineRulesVM : ViewModelBase, INavigationAware
    {
        #region Fields

        private const int cIgnoreKey = -1;
        private IRegionNavigationService mNavigationService;
        private IProcessingRuleDataService mProcessingRuleDataService;
        private IEntryDataService mEntryDataService;
        private IPeriodDataService mPeriodDataService;
        private ITransactionDataService mTransactionDataService;

        #endregion Fields

        #region Constructor(s)

        public DefineRulesVM(IProcessingRuleDataService processingRuleDataService, IEntryDataService entryDataService,
            IPeriodDataService periodDataService, ITransactionDataService transactionDataService)
        {
            mProcessingRuleDataService = processingRuleDataService;
            mEntryDataService = entryDataService;
            mPeriodDataService = periodDataService;
            mTransactionDataService = transactionDataService;
            MatchingTransactions = new ObservableCollection<Transaction>();
        }

        #endregion Constructor(s)

        #region INavigationAware

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            mNavigationService = navigationContext.NavigationService;

            loadData();
        }

        #endregion INavigationAware

        #region Command NavigateBackCommand

        /// <summary>Field for the NavigateBack command.
        /// </summary>
        private DelegateCommand mNavigateBackCommand;

        /// <summary>Gets NavigateBack command.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public DelegateCommand NavigateBackCommand
        {
            get
            {
                return this.mNavigateBackCommand
                    // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
                    // is not yet bound to the View, the command is instantiated in a different thread than the
                    // main thread. Prevent this by checking on the SynchronizationContext.
                    ?? (this.mNavigateBackCommand = System.Threading.SynchronizationContext.Current == null
                    ? null : new DelegateCommand(this.NavigateBack, this.CanNavigateBack));
            }
        }

        /// <summary>
        /// </summary>
        private void NavigateBack()
        {
            if (mNavigationService.Journal.CanGoBack)
            {
                mNavigationService.Journal.GoBack();
            }
        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanNavigateBack()
        {
            return true;
        }

        #endregion Command NavigateBackCommand

        #region Command ShowPreviewCommand

        /// <summary>Field for the ShowPreview command.
        /// </summary>
        private DelegateCommand mShowPreviewCommand;

        /// <summary>Gets ShowPreview command.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public DelegateCommand ShowPreviewCommand
        {
            get
            {
                return this.mShowPreviewCommand
                    // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
                    // is not yet bound to the View, the command is instantiated in a different thread than the
                    // main thread. Prevent this by checking on the SynchronizationContext.
                    ?? (this.mShowPreviewCommand = System.Threading.SynchronizationContext.Current == null
                    ? null : new DelegateCommand(this.ShowPreview, this.CanShowPreview));
            }
        }

        /// <summary>
        /// </summary>
        private void ShowPreview()
        {
            MatchingTransactions.Clear();

            if (mSelectedRule != null)
            {
                var transactions = mTransactionDataService.GetAll();

                var result = mSelectedRule.FilterList(ref transactions);
                MatchingTransactions.AddRange(result);

                //MatchingTransactions.AddRange(mTransactionDataService.GetByRule(mSelectedRule.ToEntity(), mSelectedPeriod.Year));
            }
        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanShowPreview()
        {
            return SelectedRule != null;
        }

        #endregion Command ShowPreviewCommand

        #region Property DefinedRules

        private ObservableCollection<ProcessingRuleVM> mDefinedRules;

        public ObservableCollection<ProcessingRuleVM> DefinedRules
        {
            get
            {
                return mDefinedRules;
            }
            private set
            {
                mDefinedRules = value;
                NotifyPropertyChanged();
            }
        }

        #endregion Property DefinedRules

        #region Property SelectedRule

        private ProcessingRuleVM mSelectedRule;

        public ProcessingRuleVM SelectedRule
        {
            get
            {
                return mSelectedRule;
            }
            set
            {
                mSelectedRule = value;

                if (mSelectedRule?.EntryKey != null)
                {
                    SelectedEntry = AvailableEntries.First(e => e.Key == mSelectedRule.EntryKey);
                }
                else
                {
                    // First entry is the 'ignore' option.
                    SelectedEntry = AvailableEntries.First(e => e.Key == cIgnoreKey);
                }
                NotifyPropertyChanged();
            }
        }

        #endregion Property SelectedRule

        #region Property AvailableEntries

        public ObservableCollection<EntryVM> AvailableEntries
        {
            get; private set;
        }

        #endregion Property AvailableEntries

        #region Property SeletedEntry

        private EntryVM mSelectedEntry;

        public EntryVM SelectedEntry
        {
            get
            {
                return mSelectedEntry;
            }
            set
            {
                mSelectedEntry = value;

                if (mSelectedEntry.Key == cIgnoreKey)
                {
                    mSelectedRule.Entry = null;
                }
                else
                {
                    mSelectedRule.Entry = mSelectedEntry.ToEntity();
                }
                NotifyPropertyChanged();
            }
        }

        #endregion Property SeletedEntry

        #region Property SelectedPeriod

        private PeriodSelectionVM mSelectedPeriod;

        public PeriodSelectionVM SelectedPeriod
        {
            get
            {
                return mSelectedPeriod;
            }
            set
            {
                mSelectedPeriod = value;
                NotifyPropertyChanged();
            }
        }

        #endregion Property SelectedPeriod

        #region Property AvailablePeriods

        public ObservableCollection<PeriodSelectionVM> AvailablePeriods { get; private set; }

        #endregion Property AvailablePeriods

        #region Property MatchingRules

        public ObservableCollection<Transaction> MatchingTransactions
        {
            get; private set;
        }

        #endregion Property MatchingRules


        #region Helper methods

        private void loadData()
        {
            mSelectedRule = null;
            DefinedRules = new ObservableCollection<ProcessingRuleVM>();

            foreach (var rule in mProcessingRuleDataService.GetAllSorted())
            {
                DefinedRules.Add(Mapper.Map<ProcessingRuleVM>(rule));
            }

            var unsortedAvailableEntries = new List<EntryVM>();

            // Available entries.
            foreach (var entry in mEntryDataService.Get3rdLevelEntries())
            {
                unsortedAvailableEntries.Add(Mapper.Map<EntryVM>(entry));
            }

            AvailableEntries = new ObservableCollection<EntryVM>(unsortedAvailableEntries.OrderBy(e => e.RouteString));

            AvailableEntries.Insert(0, new EntryVM() { Key = cIgnoreKey, Description = "Negeren" });

            // Available periods
            var availableYears = mPeriodDataService.GetAvailableYears();

            AvailablePeriods = new ObservableCollection<PeriodSelectionVM>();
            AvailablePeriods.Add(new PeriodSelectionVM(null, "Alle"));

            foreach (var year in availableYears)
            {
                AvailablePeriods.Add(new PeriodSelectionVM(year, year.ToString()));
            }
        }

        #endregion Helper methods
    }
}