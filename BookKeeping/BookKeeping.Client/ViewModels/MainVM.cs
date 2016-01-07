using BookKeeping.Client.Views;
using Microsoft.Practices.Unity;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.EventAggregatorEvents;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace BookKeeping.Client.ViewModels
{
    public class MainVM : ViewModelBase, INavigationAware, IRegionMemberLifetime
    {
        private IRegionManager mRegionManager;
        private IPeriodDataService mPeriodDataService;
        private IEventAggregator mEventAggregator;
        private IUnityContainer mContainer;

        public MainVM(IRegionManager regionManager, IPeriodDataService periodDataService, IEventAggregator eventAggregator,
            IUnityContainer unityContainer)
        {
            mRegionManager = regionManager;
            mPeriodDataService = periodDataService;
            mEventAggregator = eventAggregator;
            mContainer = unityContainer;

            mEventAggregator.GetEvent<ModuleInitializationDoneEvent>().Subscribe(InitializationDoneAction);
            mEventAggregator.GetEvent<DataChangedEvent>().Subscribe(DataChangedAction, ThreadOption.UIThread);
        }

        private void DataChangedAction(DataChangedEventArgs obj)
        {
            loadData();
        }

        private void InitializationDoneAction(bool value)
        {
            loadData();
        }

        private void loadData()
        {
            AvailableYears = new ObservableCollection<YearOverviewVM>();

            var availableYears = mPeriodDataService.GetAvailableYears();
            foreach (var year in availableYears)
            {
                mAvailableYears.Add(mContainer.Resolve<YearOverviewVM>(new ResolverOverride[]
                                                                       {
                                                                           new ParameterOverride("year", year),
                                                                       })
                                   );
            }
        }

        private ObservableCollection<YearOverviewVM> mAvailableYears;

        public ObservableCollection<YearOverviewVM> AvailableYears
        {
            get
            {
                return mAvailableYears;
            }
            set
            {
                mAvailableYears = value;
                NotifyPropertyChanged();
            }
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

        #region Command DefineEntriesCommand

        /// <summary>Field for the StartMeasurement command.
        /// </summary>
        private DelegateCommand mDefineEntriesCommand;

        /// <summary>Gets StartMeasurement command.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public DelegateCommand DefineEntriesCommand
        {
            get
            {
                return this.mDefineEntriesCommand
                    // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
                    // is not yet bound to the View, the command is instantiated in a different thread than the
                    // main thread. Prevent this by checking on the SynchronizationContext.
                    ?? (this.mDefineEntriesCommand = System.Threading.SynchronizationContext.Current == null
                    ? null : new DelegateCommand(this.DefineEntries, this.CanDefineEntries));
            }
        }

        /// <summary>Starts the measurement of a sample.
        /// </summary>
        private void DefineEntries()
        {
            mRegionManager.RequestNavigate(RegionNames.MainRegion, typeof(DefineEntriesView).FullName);
        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanDefineEntries()
        {
            return true;
        }

        #endregion Command DefineEntriesCommand

        #region Command DefineRulesCommand

        /// <summary>Field for the DefineRules command.
        /// </summary>
        private DelegateCommand mDefineRulesCommand;

        /// <summary>Gets DefineRules command.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public DelegateCommand DefineRulesCommand
        {
            get
            {
                return this.mDefineRulesCommand
                    // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
                    // is not yet bound to the View, the command is instantiated in a different thread than the
                    // main thread. Prevent this by checking on the SynchronizationContext.
                    ?? (this.mDefineRulesCommand = System.Threading.SynchronizationContext.Current == null
                    ? null : new DelegateCommand(this.DefineRules, this.CanDefineRules));
            }
        }

        /// <summary>
        /// </summary>
        private void DefineRules()
        {
            mRegionManager.RequestNavigate(RegionNames.MainRegion, typeof(DefineRulesView).FullName);
        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanDefineRules()
        {
            return true;
        }

        #endregion Command DefineRulesCommand

        #region INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion INavigationAware

        #region IRegionMemberLifetime

        public bool KeepAlive
        {
            get
            {
                return true;
            }
        }

        #endregion IRegionMemberLifetime
    }
}