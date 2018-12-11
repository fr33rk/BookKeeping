using System.Collections.ObjectModel;
using System.Linq;
using BookKeeping.Client.Views;
using Microsoft.Practices.Unity;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.EventAggregatorEvents;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace BookKeeping.Client.ViewModels
{
	public class MainVm : ViewModelBase, INavigationAware, IRegionMemberLifetime
	{
		#region Fields

		private readonly IRegionManager mRegionManager;
		private readonly IPeriodDataService mPeriodDataService;
		private readonly IUnityContainer mContainer;
		private readonly IDataExporterService mDataExporterService;

		#endregion Fields

		#region Constructor(s)

		public MainVm(IRegionManager regionManager, IPeriodDataService periodDataService, IEventAggregator eventAggregator,
			IUnityContainer unityContainer, IDataExporterService dataExporterService)
		{
			mRegionManager = regionManager;
			mPeriodDataService = periodDataService;
			var localEventAggregator = eventAggregator;
			mContainer = unityContainer;
			mDataExporterService = dataExporterService;

			localEventAggregator.GetEvent<ModuleInitializationDoneEvent>().Subscribe(InitializationDoneAction);
			localEventAggregator.GetEvent<DataChangedEvent>().Subscribe(DataChangedAction, ThreadOption.UIThread);
			localEventAggregator.GetEvent<OptionsChangedEvent>().Subscribe(OptionsChangedAction, ThreadOption.UIThread);
		}

		#endregion Constructor(s)

		#region Helper methods

		private void DataChangedAction(DataChangedEventArgs obj)
		{
			LoadData();
		}

		private void InitializationDoneAction(bool value)
		{
			LoadData();
		}

		private void OptionsChangedAction(Settings newSettings)
		{
			LoadData();
		}

		private void LoadData()
		{
			AvailableYears = new ObservableCollection<YearOverviewVm>();

			var availableYears = mPeriodDataService.GetAvailableYears();
			foreach (var year in availableYears)
			{
				mAvailableYears.Add(mContainer.Resolve<YearOverviewVm>(new ParameterOverride("year", year)));
			}

			SelectLatestYear();
		}

		private void SelectLatestYear()
		{
			SelectedYear = mAvailableYears.Last();
		}

		#endregion Helper methods

		#region Property AvailableYears

		private ObservableCollection<YearOverviewVm> mAvailableYears;

		public ObservableCollection<YearOverviewVm> AvailableYears
		{
			get => mAvailableYears;
			set
			{
				mAvailableYears = value;
				NotifyPropertyChanged();
			}
		}

		#endregion Property AvailableYears

		#region Property SelectedYear

		private YearOverviewVm mSelectedYear;

		public YearOverviewVm SelectedYear
		{
			get => mSelectedYear;
			set
			{
				mSelectedYear = value;
				NotifyPropertyChanged();
			}
		}

		#endregion Property SelectedYear

		#region Command JustDoItCommand

		/// <summary>Field for the StartMeasurement command.
		/// </summary>
		private DelegateCommand mJustDoItCommand;

		/// <summary>Gets StartMeasurement command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand JustDoItCommand => mJustDoItCommand
												  // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
												  // is not yet bound to the View, the command is instantiated in a different thread than the
												  // main thread. Prevent this by checking on the SynchronizationContext.
												  ?? (mJustDoItCommand = System.Threading.SynchronizationContext.Current == null
													  ? null : new DelegateCommand(JustDoIt, CanStartJustDoIt));

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

		#region Command ExportDataCommand

		private DelegateCommand mExportDataCommand;

		[System.ComponentModel.Browsable(false)]
		public DelegateCommand ExportDataCommand => mExportDataCommand
													// Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
													// is not yet bound to the View, the command is instantiated in a different thread than the
													// main thread. Prevent this by checking on the SynchronizationContext.
													?? (mExportDataCommand = System.Threading.SynchronizationContext.Current == null
														? null
														: new DelegateCommand(ExportData, CanExportData));

		private void ExportData()
		{
			mDataExporterService.Export(@"d:\Database.json");
		}

		private bool CanExportData()
		{
			return true;
		}

		#endregion Command ExportDataCommand

		#region Command DefineEntriesCommand

		/// <summary>Field for the StartMeasurement command.
		/// </summary>
		private DelegateCommand mDefineEntriesCommand;

		/// <summary>Gets StartMeasurement command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand DefineEntriesCommand => mDefineEntriesCommand
													   // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
													   // is not yet bound to the View, the command is instantiated in a different thread than the
													   // main thread. Prevent this by checking on the SynchronizationContext.
													   ?? (mDefineEntriesCommand = System.Threading.SynchronizationContext.Current == null
														   ? null : new DelegateCommand(DefineEntries, CanDefineEntries));

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
		public DelegateCommand DefineRulesCommand => mDefineRulesCommand
													 // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
													 // is not yet bound to the View, the command is instantiated in a different thread than the
													 // main thread. Prevent this by checking on the SynchronizationContext.
													 ?? (mDefineRulesCommand = System.Threading.SynchronizationContext.Current == null
														 ? null : new DelegateCommand(DefineRules, CanDefineRules));

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

		#region Command ReApplyRulesCommand

		/// <summary>Field for the ReApplyRules command.
		/// </summary>
		private DelegateCommand mReApplyRulesCommand;

		/// <summary>Gets ReApplyRules command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand ReApplyRulesCommand => mReApplyRulesCommand
													  // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
													  // is not yet bound to the View, the command is instantiated in a different thread than the
													  // main thread. Prevent this by checking on the SynchronizationContext.
													  ?? (mReApplyRulesCommand = System.Threading.SynchronizationContext.Current == null
														  ? null : new DelegateCommand(ReApplyRules, CanReApplyRules));

		/// <summary>
		/// </summary>
		private void ReApplyRules()
		{
			mRegionManager.RequestNavigate(RegionNames.MainRegion, typeof(ReApplyRulesView).FullName);

			//mTransactionDataService.ResetPeriodEntryLinks();

			//mDataProcessorService.Process(mTransactionDataService.GetAll());

			//loadData();
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanReApplyRules()
		{
			return true;
		}

		#endregion Command ReApplyRulesCommand

		#region Command SearchCommand

		/// <summary>Field for the Search command.
		/// </summary>
		private DelegateCommand mSearchCommand;

		/// <summary>Gets Search command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand SearchCommand => mSearchCommand
												// Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
												// is not yet bound to the View, the command is instantiated in a different thread than the
												// main thread. Prevent this by checking on the SynchronizationContext.
												?? (mSearchCommand = System.Threading.SynchronizationContext.Current == null
													? null : new DelegateCommand(Search, CanSearch));

		/// <summary>
		/// </summary>
		private void Search()
		{
			mRegionManager.RequestNavigate(RegionNames.MainRegion, typeof(GlobalSearchView).FullName);
		}

		/// <summary>Determines whether the Search command can be executed.
		/// </summary>
		private bool CanSearch()
		{
			return true;
		}

		#endregion Command SearchCommand

		#region Command EditOptionsCommand

		/// <summary>Field for the EditOptions command.
		/// </summary>
		private DelegateCommand mEditOptionsCommand;

		/// <summary>Gets EditOptions command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand EditOptionsCommand => mEditOptionsCommand
												// Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
												// is not yet bound to the View, the command is instantiated in a different thread than the
												// main thread. Prevent this by checking on the SynchronizationContext.
												?? (mEditOptionsCommand = System.Threading.SynchronizationContext.Current == null
													? null : new DelegateCommand(EditOptions, CanEditOptions));

		/// <summary>
		/// </summary>
		private void EditOptions()
		{
			mRegionManager.RequestNavigate(RegionNames.MainRegion, typeof(EditOptionsView).FullName);
		}

		/// <summary>Determines whether the EditOptions command can be executed.
		/// </summary>
		private bool CanEditOptions()
		{
			return true;
		}

		#endregion Command EditOptionsCommand

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

		public bool KeepAlive => true;

		#endregion IRegionMemberLifetime
	}
}