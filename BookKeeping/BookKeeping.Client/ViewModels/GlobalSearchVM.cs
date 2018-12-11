using System.Collections.ObjectModel;
using BookKeeping.Client.Models;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using Prism.Commands;
using Prism.Regions;

namespace BookKeeping.Client.ViewModels
{
	public class GlobalSearchVm : ViewModelBase, INavigationAware
	{
		#region Fields

		private readonly ITransactionDataService mTransactionDataService;

		#endregion Fields

		#region Constructor(s)

		public GlobalSearchVm(ITransactionDataService transactionDataService)
		{
			mTransactionDataService = transactionDataService;
			MatchingTransactions = new ObservableCollection<Transaction>();
			SelectedRule = new ProcessingRuleVm();
		}

		#endregion Constructor(s)

		#region INavigationAware

		private IRegionNavigationService mNavigationService;

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			mNavigationService = navigationContext.NavigationService;
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return false;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
		}

		#endregion INavigationAware

		#region SelectedRule

		private ProcessingRuleVm mSelectedRule;

		public ProcessingRuleVm SelectedRule
		{
			get => mSelectedRule;
			private set
			{
				mSelectedRule = value;
				NotifyPropertyChanged();
			}
		}

		#endregion SelectedRule

		#region Property Matching Transactions

		public ObservableCollection<Transaction> MatchingTransactions
		{
			get;
		}

		#endregion Property Matching Transactions

		#region Command NavigateBackCommand

		/// <summary>Field for the NavigateBack command.
		/// </summary>
		private DelegateCommand mNavigateBackCommand;

		/// <summary>Gets NavigateBack command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand NavigateBackCommand => mNavigateBackCommand
													  // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
													  // is not yet bound to the View, the command is instantiated in a different thread than the
													  // main thread. Prevent this by checking on the SynchronizationContext.
													  ?? (mNavigateBackCommand = System.Threading.SynchronizationContext.Current == null
														  ? null : new DelegateCommand(NavigateBack, CanNavigateBack));

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

		#region Command ClearCommand

		/// <summary>Field for the Clear command.
		/// </summary>
		private DelegateCommand mClearCommand;

		/// <summary>Gets Clear command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand ClearCommand => mClearCommand
												// Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
												// is not yet bound to the View, the command is instantiated in a different thread than the
												// main thread. Prevent this by checking on the SynchronizationContext.
												?? (mClearCommand = System.Threading.SynchronizationContext.Current == null
													? null : new DelegateCommand(Clear, CanClear));

		/// <summary>
		/// </summary>
		private void Clear()
		{
			SelectedRule = new ProcessingRuleVm();
		}

		/// <summary>Determines whether the Clear command can be executed.
		/// </summary>
		private bool CanClear()
		{
			return true;
		}

		#endregion Command ClearCommand

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
			MatchingTransactions.Clear();

			if (mSelectedRule != null)
			{
				var transactions = mTransactionDataService.GetAll();

				var result = mSelectedRule.FilterList(ref transactions, null);

				MatchingTransactions.AddRange(result);
			}
		}

		/// <summary>Determines whether the Search command can be executed.
		/// </summary>
		private bool CanSearch()
		{
			return true;
		}

		#endregion Command SearchCommand
	}
}