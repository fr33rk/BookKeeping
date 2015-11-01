﻿using BookKeeping.Client.Views;
using PL.BookKeeping.Infrastructure;
using PL.Common.Prism;
using Prism.Commands;
using Prism.Regions;

namespace BookKeeping.Client.ViewModels
{
    public class MainVM : ViewModelBase
    {
        IRegionManager mRegionManager;

        public MainVM(IRegionManager regionManager)
        {
            mRegionManager = regionManager;
        }

        #region Command JustDoItCommand

        /// <summary>Field for the StartMeasurement command.
        /// </summary>
        private DelegateCommand mJustDoItCommand;

        /// <summary>Gets StartMeasurement command.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public DelegateCommand JustDoItCommand
        {
            get
            {
                return this.mJustDoItCommand
                    // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
                    // is not yet bound to the View, the command is instantiated in a different thread than the
                    // main thread. Prevent this by checking on the SynchronizationContext.
                    ?? (this.mJustDoItCommand = System.Threading.SynchronizationContext.Current == null
                    ? null : new DelegateCommand(this.JustDoIt, this.CanStartJustDoIt));
            }
        }

        /// <summary>Starts the measurement of a sample.
        /// </summary>
        private void JustDoIt()
        {
            mRegionManager.RequestNavigate(RegionNames.MainRegion, typeof(ImportDataView).FullName);
        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanStartJustDoIt()
        {
            return true;
        }

        #endregion Command JustDoItCommand
    }
}