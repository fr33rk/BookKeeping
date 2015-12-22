using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using BookKeeping.Client.Models;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using Prism.Commands;
using Prism.Regions;

namespace BookKeeping.Client.ViewModels
{
	public class DefineRulesVM : ViewModelBase, INavigationAware
	{
		#region Fields

		private IRegionNavigationService mNavigationService;
		private IProcessingRuleDataService mProcessingRuleDataService;
		private IEntryDataService mEntryDataService;

		#endregion Fields

		#region Constructor(s)

		public DefineRulesVM(IProcessingRuleDataService processingRuleDataService, IEntryDataService entryDataService)
		{
			mProcessingRuleDataService = processingRuleDataService;
			mEntryDataService = entryDataService;
		}

		#endregion Constructor(s)

		#region INavigationAware

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
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

		private void loadData()
		{
			DefinedRules = new ObservableCollection<ProcessingRuleVM>();

			foreach (var rule in mProcessingRuleDataService.GetAllSorted())
			{
				DefinedRules.Add(Mapper.Map<ProcessingRuleVM>(rule));
			}

			var unsortedAvailableEntries = new List<EntryVM>();

			foreach (var entry in mEntryDataService.Get3rdLevelEntries())
			{
				unsortedAvailableEntries.Add(Mapper.Map<EntryVM>(entry));
			}

			AvailableEntries = new ObservableCollection<EntryVM>(unsortedAvailableEntries.OrderBy(e => e.RouteString));
		}
	}
}