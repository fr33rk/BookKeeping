using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using Prism.Commands;
using Prism.Regions;
using Stateless;

namespace BookKeeping.Client.ViewModels
{
	public class ReApplyRulesVM : ViewModelBase, INavigationAware, IDataErrorInfo
	{
		#region Fields

		private IRegionNavigationService mNavigationService;
		private ITransactionDataService mTransactionDataService;
		private IDataProcessorService mDataProcessorService;
		private IEntryPeriodDataService mEntryPeriodDataService;
		private IPeriodDataService mPeriodDataService;
		private IList<Transaction> mSelectedTransactions;

		#endregion Fields

		#region Constructor(s)

		public ReApplyRulesVM(IRegionNavigationService navigationService, ITransactionDataService transactionDataService,
			IEntryPeriodDataService entryPeriodDataService, IPeriodDataService periodDataService, IDataProcessorService dataProcessorService)
		{
			InitializeStateMachine();

			mNavigationService = navigationService;
			mTransactionDataService = transactionDataService;
			mDataProcessorService = dataProcessorService;
			mEntryPeriodDataService = entryPeriodDataService;
			mPeriodDataService = periodDataService;

			mDataProcessorService.OnDataProcessed += DataProcessorService_OnDataProcessed;
		}

		private void DataProcessorService_OnDataProcessed(object sender, PL.BookKeeping.Infrastructure.DataProcessedEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				ProcessedCount = e.Processed;
				IgnoredCount = e.Ignored;
			});
		}

		#endregion Constructor(s)

		#region INavigationAware

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			//
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			mNavigationService = navigationContext.NavigationService;

			if (mVMStateMachine.IsInState(VMState.Idle))
			{
				mVMStateMachine.Fire(VMTrigger.Start);
			}
		}

		#endregion INavigationAware

		#region IDateErrorInfor

		public string Error
		{
			get
			{
				return null;
			}
		}

		public string this[string columnName]
		{
			get
			{
				string result = null;

				if ((columnName == "EndDate") || (columnName == "StartDate"))
				{
					if (EndDate <= StartDate)
					{
						result = "Periode kan niet eerder eindigen dan dat deze start";
					}
				}

				return result;
			}
		}

		#endregion IDateErrorInfor

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

		#region Command ReApplyRulesCommand

		/// <summary>Field for the ReApplyRules command.
		/// </summary>
		private DelegateCommand mReApplyRulesCommand;

		/// <summary>Gets ReApplyRules command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand ReApplyRulesCommand
		{
			get
			{
				return this.mReApplyRulesCommand
					// Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
					// is not yet bound to the View, the command is instantiated in a different thread than the
					// main thread. Prevent this by checking on the SynchronizationContext.
					?? (this.mReApplyRulesCommand = System.Threading.SynchronizationContext.Current == null
					? null : new DelegateCommand(this.ReApplyRules, this.CanReApplyRules));
			}
		}

		/// <summary>
		/// </summary>
		private void ReApplyRules()
		{
			// TODO Should check if entered data is valid. There should be some kind of ValidateableViewModelBase. See https://msdn.microsoft.com/en-us/library/txtwdysk.aspx
			mVMStateMachine.Fire(VMTrigger.LoadTransactions);
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanReApplyRules()
		{
			return mVMStateMachine.IsInState(VMState.WaitingOnInput);
		}

		#endregion Command ReApplyRulesCommand

		#region Property ReApplyToAll

		private bool mReApplyToAll;

		public bool ReApplyToAll
		{
			get
			{
				return mReApplyToAll;
			}
			set
			{
				if (mReApplyToAll != value)
				{
					mReApplyToAll = value;
					NotifyPropertyChanged();
				}
			}
		}

		#endregion Property ReApplyToAll

		#region Property StartDate

		private DateTime mStartDate;

		public DateTime StartDate
		{
			get
			{
				return mStartDate;
			}
			set
			{
				if (mStartDate != value)
				{
					mStartDate = value;
					NotifyPropertyChanged();
				}
			}
		}

		#endregion Property StartDate

		#region Property EndDate

		private DateTime mEndDate;

		public DateTime EndDate
		{
			get
			{
				return mEndDate;
			}
			set
			{
				if (mEndDate != value)
				{
					// We get the beginning of the day (00:00). but we need the end (23:59).
					mEndDate = value.AddDays(1).AddTicks(-1);
					NotifyPropertyChanged();
				}
			}
		}

		#endregion Property EndDate

		#region Property TransactionCount

		private int mTransactionCount;

		public int TransactionCount
		{
			get
			{
				return mTransactionCount;
			}
			set
			{
				if (mTransactionCount != value)
				{
					mTransactionCount = value;
					NotifyPropertyChanged();
				}
			}
		}

		#endregion Property TransactionCount

		#region Property ProcessedCount

		private int mProcessedCount;

		public int ProcessedCount
		{
			get
			{
				return mProcessedCount;
			}
			set
			{
				mProcessedCount = value;
				NotifyPropertyChanged();
			}
		}

		#endregion Property ProcessedCount

		#region Property IgnoredCount

		private int mIgnoredCount;

		public int IgnoredCount
		{
			get
			{
				return mIgnoredCount;
			}
			set
			{
				mIgnoredCount = value;
				NotifyPropertyChanged();
			}
		}

		#endregion Property IgnoredCount

		#region StateMachine

		private enum VMState
		{
			Idle,
			WaitingOnInput,
			LoadingTransactions,
			ReapplingRules,
			RecalculatingTotalAmounts,
		}

		private enum VMTrigger
		{
			Start,
			WaitOnInput,
			LoadTransactions,
			ReApplyRules,
			RecalculateTotalAmounts,
			ReApplyingDone
		}

		private StateMachine<VMState, VMTrigger> mVMStateMachine;

		private void InitializeStateMachine()
		{
			mVMStateMachine = new StateMachine<VMState, VMTrigger>(VMState.Idle);

			mVMStateMachine.Configure(VMState.Idle)
				.Permit(VMTrigger.Start, VMState.WaitingOnInput);

			mVMStateMachine.Configure(VMState.WaitingOnInput)
				.OnEntryFrom(VMTrigger.Start, () => initialize())
				.Permit(VMTrigger.LoadTransactions, VMState.LoadingTransactions);

			mVMStateMachine.Configure(VMState.LoadingTransactions)
				.OnEntry(() => loadTransactions())
				.Permit(VMTrigger.ReApplyRules, VMState.ReapplingRules);

			mVMStateMachine.Configure(VMState.ReapplingRules)
				.OnEntry(() => reApplyRules())
				.Permit(VMTrigger.RecalculateTotalAmounts, VMState.RecalculatingTotalAmounts);

			mVMStateMachine.Configure(VMState.RecalculatingTotalAmounts)
				.OnEntry(() => reCalculateTotalAmounts())
				.Permit(VMTrigger.ReApplyingDone, VMState.WaitingOnInput);
		}

		#endregion StateMachine

		#region Helper functions

		private void reCalculateTotalAmounts()
		{
			Task.Factory.StartNew(() =>
			{
				if (ReApplyToAll)
				{
					var startDate = mSelectedTransactions.Min(t => t.Date);
					var endDate = mSelectedTransactions.Max(t => t.Date);

					mEntryPeriodDataService.ReCalculateTotalAmounts(startDate, endDate);					
				}
				else
				{
					mEntryPeriodDataService.ReCalculateTotalAmounts(StartDate, EndDate);
				}

				mVMStateMachine.Fire(VMTrigger.ReApplyingDone);				
			});
		}

		private void reApplyRules()
		{
			Task.Factory.StartNew(() =>
			{
				mDataProcessorService.Process(mSelectedTransactions);

				mVMStateMachine.Fire(VMTrigger.RecalculateTotalAmounts);
			});
		}

		private void loadTransactions()
		{
			Task.Factory.StartNew(() =>
			{
				if (ReApplyToAll)
				{
					mSelectedTransactions = mTransactionDataService.GetAll();
				}
				else
				{
					mSelectedTransactions = mTransactionDataService.GetOfPeriod(StartDate, EndDate);
				}

				Application.Current.Dispatcher.Invoke(() => TransactionCount = mSelectedTransactions.Count);

				mVMStateMachine.Fire(VMTrigger.ReApplyRules);
			});
		}

		private void initialize()
		{
			// Initialize the period.
			var lastMonth = DateTime.Now.AddMonths(-1);
			StartDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
			EndDate = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));

			EndDate = EndDate.AddDays(1).AddTicks(-1);

			ReApplyToAll = true;
		}

		#endregion Helper functions
	}
}