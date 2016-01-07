using AutoMapper;
using BookKeeping.Client.Models;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.EntityExtensions;
using PL.BookKeeping.Infrastructure.EventAggregatorEvents;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows;

namespace BookKeeping.Client.ViewModels
{
    /// <summary>Allows CRUD operations on entry definitions.
    /// </summary>
    public class DefineEntriesVM : ViewModelBase, INavigationAware
    {
        #region Fields

        private IEntryDataService mEntryDataService;
        private IProcessingRuleDataService mProcessingRuleDataService;
        private IRegionManager mRegionManager;
        private IEventAggregator mEventAggregator;
        private bool mEntriesHaveChanged;

        #endregion Fields

        #region Constructor(s)

        public DefineEntriesVM(IEntryDataService entryDataService, IRegionManager regionManager,
            IProcessingRuleDataService processingRuleDataService, IEventAggregator eventAggregator)
        {
            mEntryDataService = entryDataService;
            mRegionManager = regionManager;
            mProcessingRuleDataService = processingRuleDataService;
            mEventAggregator = eventAggregator;
        }

        #endregion Constructor(s)

        #region INavigationAware

        private IRegionNavigationService navigationService;

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            navigationService = navigationContext.NavigationService;
            mEntriesHaveChanged = false;

            loadData();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
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
            if (mEntriesHaveChanged)
            {
                // Update the entries in the main view
                mEventAggregator.GetEvent<DataChangedEvent>().Publish(new DataChangedEventArgs());
            }

            if (navigationService.Journal.CanGoBack)
            {
                navigationService.Journal.GoBack();
            }
        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanNavigateBack()
        {
            return true;
        }

        #endregion Command NavigateBackCommand

        #region Command AddEntryCommand

        /// <summary>Field for the AddEntry command.
        /// </summary>
        private DelegateCommand mAddEntryCommand;

        /// <summary>Gets AddEntry command.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public DelegateCommand AddEntryCommand
        {
            get
            {
                return this.mAddEntryCommand
                    // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
                    // is not yet bound to the View, the command is instantiated in a different thread than the
                    // main thread. Prevent this by checking on the SynchronizationContext.
                    ?? (this.mAddEntryCommand = System.Threading.SynchronizationContext.Current == null
                    ? null : new DelegateCommand(this.AddEntry, this.CanAddEntry));
            }
        }

        /// <summary>
        /// </summary>
        private void AddEntry()
        {
            var newEntry = new Entry();
            newEntry.Description = "Nieuw";
            newEntry.ParentEntry = Mapper.Map<Entry>(SelectedEntry);

            mEntryDataService.Add(newEntry);

            SelectedEntry.ChildEntryVms.Add(Mapper.Map<EntryVM>(newEntry));

            mEntriesHaveChanged = true;
        }

        /// <summary>Determines whether the AddEntry command can be executed.
        /// </summary>
        private bool CanAddEntry()
        {
            if ((SelectedEntry != null) && ((Mapper.Map<Entry>(SelectedEntry)).Level() < 3))
            {
                return true;
            }

            return false;
        }

        #endregion Command AddEntryCommand

        #region Command DeleteEntryCommand

        /// <summary>Field for the DeleteEntry command.
        /// </summary>
        private DelegateCommand mDeleteEntryCommand;

        /// <summary>Gets DeleteEntry command.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public DelegateCommand DeleteEntryCommand
        {
            get
            {
                return this.mDeleteEntryCommand
                    // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
                    // is not yet bound to the View, the command is instantiated in a different thread than the
                    // main thread. Prevent this by checking on the SynchronizationContext.
                    ?? (this.mDeleteEntryCommand = System.Threading.SynchronizationContext.Current == null
                    ? null : new DelegateCommand(this.DeleteEntry, this.CanDeleteEntry));
            }
        }

        /// <summary>
        /// </summary>
        private void DeleteEntry()
        {
            // Ask for confirmation.
            var result = MessageBox.Show("Weet je zeker dat je deze post wilt verwijderen?", "Bookkeeping", MessageBoxButton.YesNoCancel);

            if (result == MessageBoxResult.Yes)
            {
                // Delete entry
                var rootInList = GetRootEntryOf(SelectedEntry);

                mEntryDataService.Delete(SelectedEntry.ToEntity());

                if (rootInList != null)
                {
                    rootInList.DeleteChild(SelectedEntry);
                }

                mEntriesHaveChanged = true;
            }
        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanDeleteEntry()
        {
            if ((SelectedEntry != null) && (SelectedEntry.ChildEntryVms.Count == 0))
            {
                return true;
            }

            return false;
        }

        #endregion Command DeleteEntryCommand

        #region Property Entries

        public ObservableCollection<EntryVM> Entries { get; set; }

        #endregion Property Entries

        #region Property SelectedEntry

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

                loadAttachedProcessingRules();

                NotifyPropertyChanged();
                NotifyPropertyChanged("SelectedEntryDescription");
                AddEntryCommand.RaiseCanExecuteChanged();
                DeleteEntryCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion Property SelectedEntry

        #region Property SelectedEntryDescription

        public string SelectedEntryDescription
        {
            get
            {
                if (SelectedEntry != null)
                {
                    return SelectedEntry.Description;
                }
                return "";
            }
            set
            {
                SelectedEntry.Description = value;

                var entry = SelectedEntry.ToEntity();

                mEntryDataService.Update(entry);

                var entryVm = EntryVM.FromEntity(entry);

                var rootInList = GetRootEntryOf(entryVm);

                if (rootInList != null)
                {
                    rootInList.UpdateChild(entryVm);
                }

                mEntriesHaveChanged = true;
            }
        }

        #endregion Property SelectedEntryDescription

        #region Property AttachedRules

        public ObservableCollection<ProcessingRuleVM> AttachedProcessingRules { get; private set; } = new ObservableCollection<ProcessingRuleVM>();

        #endregion Property AttachedRules

        #region Helper methods

        private void loadAttachedProcessingRules()
        {
            if (mSelectedEntry != null)
            {
                AttachedProcessingRules.Clear(); // = new ObservableCollection<ProcessingRuleVM>();

                foreach (var rule in mProcessingRuleDataService.GetByEntry(mSelectedEntry.ToEntity()))
                {
                    AttachedProcessingRules.Add(ProcessingRuleVM.FromEntity(rule));
                }
            }
        }

        private void loadData()
        {
            Entries = new ObservableCollection<EntryVM>();

            foreach (var rootEntry in mEntryDataService.GetRootEntries())
            {
                Entries.Add(Mapper.Map<EntryVM>(rootEntry));
            }
        }

        private EntryVM GetRootEntryOf(EntryVM child)
        {
            EntryVM retValue = null;

            foreach (var entryVm in Entries)
            {
                if (entryVm.HasChildNode(child))
                {
                    return entryVm;
                }
            }

            return retValue;
        }

        #endregion Helper methods
    }
}