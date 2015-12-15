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
using System.Linq;
using System.Windows;

namespace BookKeeping.Client.ViewModels
{
    public class DefineEntriesVM : ViewModelBase, INavigationAware
    {
        private IEntryDataService mEntryDataService;
        private IRegionManager mRegionManager;
        private IEventAggregator mEventAggregator;
        private bool mEntriesHaveChanged;

        public DefineEntriesVM(IEntryDataService entryDataService, IRegionManager regionManager,
            IEventAggregator eventAggregator)
        {
            mEntryDataService = entryDataService;
            mRegionManager = regionManager;
            mEventAggregator = eventAggregator;
        }

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
            //throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
        }

        #endregion INavigationAware

        public ObservableCollection<EntryVM> Entries { get; set; }

        private void loadData()
        {
            //Entries = new ObservableCollection<Entry>(mEntryDataService.GetRootEntries());
            Entries = new ObservableCollection<EntryVM>();

            foreach (var rootEntry in mEntryDataService.GetRootEntries())
            {
                Entries.Add(Mapper.Map<EntryVM>(rootEntry));
            }
        }

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

                var entry = SelectedEntry.ToEntry();

                // Get the root before the update. This will clear the root node.
                var root = entry.RootEntry();

                var rootInList = Entries.FirstOrDefault(e => e.Key == root.Key);

                mEntryDataService.Update(entry);

                if (rootInList != null)
                {
                     rootInList.UpdateChild(EntryVM.FromEntry(entry));
                }

                mEntriesHaveChanged = true;
            }
        }

        #endregion Property SelectedEntryDescription

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
                mEntryDataService.Delete(Mapper.Map<Entry>(SelectedEntry));
            }

            mEntriesHaveChanged = true;
        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanDeleteEntry()
        {
            if ((SelectedEntry != null) && (SelectedEntry.ChildEntries == null))
            {
                return true;
            }

            return false;
        }

        #endregion Command DeleteEntryCommand
    }
}