﻿using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using Prism.Commands;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace BookKeeping.Client.ViewModels
{
    public class DefineEntriesVM : ViewModelBase, INavigationAware
    {
        private IEntryDataService mEntryDataSeervice;
        private IRegionManager mRegionManager;

        public DefineEntriesVM(IEntryDataService entryDataService, IRegionManager regionManager)
        {
            mEntryDataSeervice = entryDataService;
            mRegionManager = regionManager;
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

        private IRegionNavigationService navigationService;

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            navigationService = navigationContext.NavigationService;

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

        #endregion Command NavigateBackCommand



        public ObservableCollection<Entry> Entries { get; set; }

        private void loadData()
        {
            Entries = new ObservableCollection<Entry>(mEntryDataSeervice.GetAllSorted());
        }


        #region Property SelectedEntry

        private Entry mSelectedEntry;

        public Entry SelectedEntry
        {
            get
            {
                return mSelectedEntry;
            }
            set
            {
                mSelectedEntry = value;               
                NotifyPropertyChanged();
                AddEntryCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion Property SelectedEntry


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
            SelectedEntry.ChildEntries.Add(new Entry() { Description="New"});
        }

        /// <summary>Determines whether the AddEntry command can be executed.
        /// </summary>
        private bool CanAddEntry()
        {
            if (SelectedEntry != null)
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

        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanDeleteEntry()
        {
            return true;
        }

        #endregion Command DeleteEntryCommand				


    }
}