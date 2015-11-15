using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
