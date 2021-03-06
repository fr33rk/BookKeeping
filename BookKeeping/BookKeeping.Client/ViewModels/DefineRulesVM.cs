﻿using AutoMapper;
using BookKeeping.Client.Models;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Extensions;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using PL.Logger;
using Prism.Commands;
using Prism.Regions;
using Stateless;
using Stateless.Graph;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;

namespace BookKeeping.Client.ViewModels
{
	public class PeriodSelectionVM
	{
		public PeriodSelectionVM(int? year, string description)
		{
			Year = year;
			Description = description;
		}

		public int? Year { get; private set; }
		public string Description { get; private set; }
	}

	public class DefineRulesVM : ViewModelBase, INavigationAware
	{
		#region Fields

		private const int cIgnoreKey = -1;
		private IRegionNavigationService mNavigationService;
		private readonly IProcessingRuleDataService mProcessingRuleDataService;
		private readonly IEntryDataService mEntryDataService;
		private readonly IPeriodDataService mPeriodDataService;
		private readonly ITransactionDataService mTransactionDataService;
		private readonly ILogFile mLogFile;
		private ProcessingRuleVm mSavedProcessingRuleVM;

		#endregion Fields

		#region Constructor(s)

		public DefineRulesVM(IProcessingRuleDataService processingRuleDataService, IEntryDataService entryDataService,
			IPeriodDataService periodDataService, ITransactionDataService transactionDataService, ILogFile logFile)
		{
			mProcessingRuleDataService = processingRuleDataService;
			mEntryDataService = entryDataService;
			mPeriodDataService = periodDataService;
			mTransactionDataService = transactionDataService;
			mLogFile = logFile;

			MatchingTransactions = new ObservableCollection<Transaction>();

			InitializeStateMachine();

			mLogFile.Debug($"DefineRulesVM.MainStm: {UmlDotGraph.Format(mMainStm.GetInfo())}");
		}

		#endregion Constructor(s)

		#region INavigationAware

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return false;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			mNavigationService = navigationContext.NavigationService;

			mMainStm.Fire(Trigger.Start);
		}

		#endregion INavigationAware

		#region Command AddEntryCommand

		/// <summary>Field for the AddRule command.
		/// </summary>
		private DelegateCommand mAddRuleCommand;

		/// <summary>Gets AddRule command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand AddRuleCommand => mAddRuleCommand
												 // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
												 // is not yet bound to the View, the command is instantiated in a different thread than the
												 // main thread. Prevent this by checking on the SynchronizationContext.
												 ?? (mAddRuleCommand = System.Threading.SynchronizationContext.Current == null
													 ? null : new DelegateCommand(AddRule, CanAddRule));

		/// <summary>
		/// </summary>
		private void AddRule()
		{
			mMainStm.Fire(Trigger.AddRule);
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanAddRule()
		{
			return mMainStm.IsInState(State.DoSomething);
		}

		#endregion Command AddEntryCommand

		#region Command DeleteRuleCommand

		/// <summary>Field for the DeleteRule command.
		/// </summary>
		private DelegateCommand mDeleteRuleCommand;

		/// <summary>Gets DeleteRule command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand DeleteRuleCommand => mDeleteRuleCommand
													// Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
													// is not yet bound to the View, the command is instantiated in a different thread than the
													// main thread. Prevent this by checking on the SynchronizationContext.
													?? (mDeleteRuleCommand = System.Threading.SynchronizationContext.Current == null
														? null : new DelegateCommand(DeleteRule, CanDeleteRule));

		/// <summary>
		/// </summary>
		private void DeleteRule()
		{
			mMainStm.Fire(Trigger.DeleteRule);
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanDeleteRule()
		{
			return mMainStm.IsInState(State.DoSomething);
		}

		#endregion Command DeleteRuleCommand

		#region Command EditRuleCommand

		/// <summary>Field for the EditRule command.
		/// </summary>
		private DelegateCommand mEditRuleCommand;

		/// <summary>Gets EditRule command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand EditRuleCommand => mEditRuleCommand
												  // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
												  // is not yet bound to the View, the command is instantiated in a different thread than the
												  // main thread. Prevent this by checking on the SynchronizationContext.
												  ?? (mEditRuleCommand = System.Threading.SynchronizationContext.Current == null
													  ? null : new DelegateCommand(EditRule, CanEditRule));

		/// <summary>
		/// </summary>
		private void EditRule()
		{
			mMainStm.Fire(Trigger.EditRule);
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanEditRule()
		{
			return mMainStm.IsInState(State.DoSomething);
		}

		#endregion Command EditRuleCommand

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
			return mMainStm.IsInState(State.DoSomething);
		}

		#endregion Command NavigateBackCommand

		#region Command ShowPreviewCommand

		/// <summary>Field for the ShowPreview command.
		/// </summary>
		private DelegateCommand mShowPreviewCommand;

		/// <summary>Gets ShowPreview command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand ShowPreviewCommand => mShowPreviewCommand
													 // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
													 // is not yet bound to the View, the command is instantiated in a different thread than the
													 // main thread. Prevent this by checking on the SynchronizationContext.
													 ?? (mShowPreviewCommand = System.Threading.SynchronizationContext.Current == null
														 ? null : new DelegateCommand(ShowPreview, CanShowPreview));

		/// <summary>
		/// </summary>
		private void ShowPreview()
		{
			MatchingTransactions.Clear();
			if (mSelectedRule == null)
			{
				return;
			}

			var previewedRule = Mapper.Map<ProcessingRule>(mSelectedRule);
			var transactions = mTransactionDataService.GetAll();

			foreach (var transaction in transactions)
			{
				if (previewedRule.AppliesTo(transaction))
				{
					MatchingTransactions.Add(transaction);
				}
			}
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanShowPreview()
		{
			return SelectedRule != null;
		}

		#endregion Command ShowPreviewCommand

		#region Command MoveUpCommand

		/// <summary>Field for the MoveUp command.
		/// </summary>
		private DelegateCommand mMoveUpCommand;

		/// <summary>Gets MoveUp command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand MoveUpCommand => mMoveUpCommand
												// Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
												// is not yet bound to the View, the command is instantiated in a different thread than the
												// main thread. Prevent this by checking on the SynchronizationContext.
												?? (mMoveUpCommand = System.Threading.SynchronizationContext.Current == null
													? null : new DelegateCommand(MoveUp, CanMoveUp));

		/// <summary>
		/// </summary>
		private void MoveUp()
		{
			var swapThisVM = SelectedRule;
			var withThatVM = DefinedRules[DefinedRules.IndexOf(SelectedRule) - 1];

			swap(swapThisVM, withThatVM);
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanMoveUp()
		{
			return mMainStm.IsInState(State.DoSomething)
				&& (mSelectedRule != null)
				&& (DefinedRules.FirstOrDefault()?.Key != mSelectedRule.Key);
		}

		#endregion Command MoveUpCommand

		#region Command MoveDownCommand

		/// <summary>Field for the MoveDown command.
		/// </summary>
		private DelegateCommand mMoveDownCommand;

		/// <summary>Gets MoveDown command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand MoveDownCommand => mMoveDownCommand
												  // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
												  // is not yet bound to the View, the command is instantiated in a different thread than the
												  // main thread. Prevent this by checking on the SynchronizationContext.
												  ?? (mMoveDownCommand = System.Threading.SynchronizationContext.Current == null
													  ? null : new DelegateCommand(MoveDown, CanMoveDown));

		/// <summary>
		/// </summary>
		private void MoveDown()
		{
			var swapThisVM = SelectedRule;
			var withThatVM = DefinedRules[DefinedRules.IndexOf(SelectedRule) + 1];
			swap(swapThisVM, withThatVM);
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanMoveDown()
		{
			return mMainStm.IsInState(State.DoSomething)
				&& (mSelectedRule != null)
				&& (DefinedRules.LastOrDefault()?.Key != mSelectedRule.Key);
		}

		#endregion Command MoveDownCommand

		#region Command CancelAddEditCommand

		/// <summary>Field for the CancelAddEdit command.
		/// </summary>
		private DelegateCommand mCancelAddEditCommand;

		/// <summary>Gets CancelAddEdit command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand CancelAddEditCommand => mCancelAddEditCommand
													   // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
													   // is not yet bound to the View, the command is instantiated in a different thread than the
													   // main thread. Prevent this by checking on the SynchronizationContext.
													   ?? (mCancelAddEditCommand = System.Threading.SynchronizationContext.Current == null
														   ? null : new DelegateCommand(CancelAddEdit, CanCancelAddEdit));

		/// <summary>
		/// </summary>
		private void CancelAddEdit()
		{
			mMainStm.Fire(Trigger.CancelAddEdit);
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanCancelAddEdit()
		{
			return mMainStm.IsInState(State.AddEditRule);
		}

		#endregion Command CancelAddEditCommand

		#region Command SaveRuleCommand

		/// <summary>Field for the SaveRule command.
		/// </summary>
		private DelegateCommand mSaveRuleCommand;

		/// <summary>Gets SaveRule command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand SaveRuleCommand => mSaveRuleCommand
												  // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
												  // is not yet bound to the View, the command is instantiated in a different thread than the
												  // main thread. Prevent this by checking on the SynchronizationContext.
												  ?? (mSaveRuleCommand = System.Threading.SynchronizationContext.Current == null
													  ? null : new DelegateCommand(SaveRule, CanSaveRule));

		/// <summary>
		/// </summary>
		private void SaveRule()
		{
			mMainStm.Fire(Trigger.SaveAddEdit);
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanSaveRule()
		{
			return mMainStm.IsInState(State.AddEditRule);
		}

		#endregion Command SaveRuleCommand

		#region Property DefinedRules

		private ObservableCollection<ProcessingRuleVm> mDefinedRules;

		public ObservableCollection<ProcessingRuleVm> DefinedRules
		{
			get => mDefinedRules;
			private set
			{
				mDefinedRules = value;
				NotifyPropertyChanged();
			}
		}

		#endregion Property DefinedRules

		#region Property SelectedRule

		private ProcessingRuleVm mSelectedRule;

		public ProcessingRuleVm SelectedRule
		{
			get => mSelectedRule;
			set
			{
				mSelectedRule = value;

				if (mSelectedRule != null)
				{
					if (mSelectedRule?.EntryKey != null)
					{
						SelectedEntry = AvailableEntries.First(e => e.Key == mSelectedRule.EntryKey);
					}
					else
					{
						// First entry is the 'ignore' option.
						SelectedEntry = AvailableEntries.First(e => e.Key == cIgnoreKey);
					}
				}
				else
				{
					SelectedEntry = null;
				}
				NotifyPropertyChanged();
			}
		}

		#endregion Property SelectedRule

		#region Property AvailableEntries

		public ObservableCollection<EntryVm> AvailableEntries
		{
			get; private set;
		}

		#endregion Property AvailableEntries

		#region Property SeletedEntry

		private EntryVm mSelectedEntry;

		public EntryVm SelectedEntry
		{
			get => mSelectedEntry;
			set
			{
				mSelectedEntry = value;

				if (mSelectedRule != null)
				{
					if (mSelectedEntry.Key == cIgnoreKey)
					{
						mSelectedRule.Entry = null;
					}
					else
					{
						mSelectedRule.Entry = mSelectedEntry.ToEntity();
					}
				}
				NotifyPropertyChanged();
			}
		}

		#endregion Property SeletedEntry

		#region Property SelectedPeriod

		private PeriodSelectionVM mSelectedPeriod;

		public PeriodSelectionVM SelectedPeriod
		{
			get => mSelectedPeriod;
			set
			{
				mSelectedPeriod = value;
				NotifyPropertyChanged();
			}
		}

		#endregion Property SelectedPeriod

		#region Property AvailablePeriods

		public ObservableCollection<PeriodSelectionVM> AvailablePeriods { get; private set; }

		#endregion Property AvailablePeriods

		#region Property Matching Transactions

		public ObservableCollection<Transaction> MatchingTransactions
		{
			get; private set;
		}

		#endregion Property Matching Transactions

		#region Property IsSelectionChangeAllowed

		public bool IsSelectionChangeAllowed => mMainStm.IsInState(State.DoSomething);

		#endregion Property IsSelectionChangeAllowed

		#region Property IsEditing

		public bool IsEditing => mMainStm.IsInState(State.AddEditRule);

		#endregion Property IsEditing

		#region Helper methods

		private void loadData()
		{
			mSelectedRule = null;
			DefinedRules = new ObservableCollection<ProcessingRuleVm>();

			foreach (var rule in mProcessingRuleDataService.GetAllSorted())
			{
				DefinedRules.Add(Mapper.Map<ProcessingRuleVm>(rule));
			}

			var unsortedAvailableEntries = new List<EntryVm>();

			// Available entries.
			foreach (var entry in mEntryDataService.Get3rdLevelEntries())
			{
				unsortedAvailableEntries.Add(Mapper.Map<EntryVm>(entry));
			}

			AvailableEntries = new ObservableCollection<EntryVm>(unsortedAvailableEntries.OrderBy(e => e.RouteString));

			AvailableEntries.Insert(0, new EntryVm() { Key = cIgnoreKey, Description = "Negeren" });

			// Available periods
			var availableYears = mPeriodDataService.GetAvailableYears();

			AvailablePeriods = new ObservableCollection<PeriodSelectionVM>();
			AvailablePeriods.Add(new PeriodSelectionVM(null, "Alle"));

			SelectedPeriod = AvailablePeriods.First();

			foreach (var year in availableYears)
			{
				AvailablePeriods.Add(new PeriodSelectionVM(year, year.ToString()));
			}
			SelectedRule = DefinedRules?.FirstOrDefault();

			mMainStm.Fire(Trigger.RulesLoaded);
		}

		private void startAddNewRule()
		{
			mSavedProcessingRuleVM = mSelectedRule;

			var newRule = new ProcessingRuleVm();
			var index = DefinedRules.IndexOf(SelectedRule);
			newRule.Priority = index + 1;
			DefinedRules.Insert(index, newRule);
			SelectedRule = newRule;
		}

		private void startEditRule()
		{
			mSavedProcessingRuleVM = mSelectedRule;

			// Reload the rule from the database so that it is managed by EF.
			//SelectedRule = SelectedRule.Clone();
			SelectedRule = ProcessingRuleVm.FromEntity(mProcessingRuleDataService.GetByKey(SelectedRule.Key, true));
		}

		private void saveRule()
		{
			var saved = false;

			// nullify empty strings
			var ruleToSave = SelectedRule.NullifyEmptyStrings().ToEntity();
			ProcessingRuleVm ruleToReplace = null;

			if (SelectedRule.Key == 0)
			{
				// Add the rule in the database
				saved = mProcessingRuleDataService.Add(ruleToSave);
				if (saved)
				{
					ruleToReplace = SelectedRule;
				}
			}
			else
			{
				// Update the rule
				saved = mProcessingRuleDataService.Update(ruleToSave);
				if (saved)
				{
					ruleToReplace = mSavedProcessingRuleVM;
				}
			}

			if (saved)
			{
				// Replace the element in the DefinedRules to update the list in the view.
				var savedRuleVM = ProcessingRuleVm.FromEntity(ruleToSave);
				DefinedRules.Replace(ruleToReplace, savedRuleVM);
				SelectedRule = ProcessingRuleVm.FromEntity(ruleToSave);
				mMainStm.Fire(Trigger.SaveDone);
			}
			else
			{
				MessageBox.Show("Er is een fout opgetreden bij het opslaan van de regel definitie. Zie de log voor meer informatie");
				mMainStm.Fire(Trigger.SaveFailed);
			}
		}

		private void restoreRule()
		{
			DefinedRules.Remove(SelectedRule);
			SelectedRule = mSavedProcessingRuleVM;
			MatchingTransactions.Clear();

			mMainStm.Fire(Trigger.RestoreDone);
		}

		private void askPermissionToDelete()
		{
			// Ask for confirmation.
			var result = MessageBox.Show("Weet je zeker dat je deze regel wilt verwijderen?", "Bookkeeping", MessageBoxButton.YesNoCancel);

			if (result == MessageBoxResult.Yes)
			{
				mMainStm.Fire(Trigger.ConinueDelete);
			}
			else
			{
				mMainStm.Fire(Trigger.AbortDelete);
			}
		}

		private void deleteRule()
		{
			mProcessingRuleDataService.Delete(SelectedRule.ToEntity());

			DefinedRules.Remove(SelectedRule);

			mMainStm.Fire(Trigger.DeleteDone);
		}

		private void swap(ProcessingRuleVm swapThisVM, ProcessingRuleVm withThatVM)
		{
			var swapThis = SelectedRule.ToEntity();
			var withThat = withThatVM.ToEntity();

			// Update the database.
			mProcessingRuleDataService.SwapByPriority(swapThis, withThat);

			var newSwapThisVM = ProcessingRuleVm.FromEntity(swapThis);
			var newWithThatVM = ProcessingRuleVm.FromEntity(withThat);

			// Replace the VM's
			DefinedRules.Replace(swapThisVM, newSwapThisVM);
			DefinedRules.Replace(withThatVM, newWithThatVM);

			// Also update the list.
			DefinedRules.Swap(newSwapThisVM, newWithThatVM);

			SelectedRule = newSwapThisVM;
		}

		#endregion Helper methods

		#region State machine

		private enum State
		{
			Idle,
			LoadingCurrentRules,
			DoSomething,
			AddEditRule,
			AddEditRuleDone,
			SavingRule,
			RestoringRule,
			StartDeletingRule,
			AskingPermissionToDelete,
			DeletingRule,
		}

		private enum Trigger
		{
			Start,
			RulesLoaded,
			AddRule,
			EditRule,
			CancelAddEdit,
			SaveAddEdit,
			SaveDone,
			RestoreDone,
			AddEditDone,
			DeleteRule,
			AbortDelete,
			ConinueDelete,
			DeleteDone,
			SaveFailed,
		}

		private StateMachine<State, Trigger> mMainStm;

		private void InitializeStateMachine()
		{
			mMainStm = new StateMachine<State, Trigger>(State.Idle);

			mMainStm.OnTransitioned((t) =>
				{
					mLogFile.Debug($"PeriodSelectionVM - VMStateMachine, changed state to {t.Destination}");
					NotifyPropertyChanged();
					NotifyPropertyChanged("IsSelectionChangeAllowed");
					NotifyPropertyChanged("IsEditing");
				});

			mMainStm.Configure(State.Idle)
				.Permit(Trigger.Start, State.LoadingCurrentRules);

			mMainStm.Configure(State.LoadingCurrentRules)
				.OnEntry(() => loadData())
				.Permit(Trigger.RulesLoaded, State.DoSomething);

			mMainStm.Configure(State.DoSomething)
				.Permit(Trigger.AddRule, State.AddEditRule)
				.PermitIf(Trigger.EditRule, State.AddEditRule, () => mSelectedRule != null)
				.Permit(Trigger.DeleteRule, State.StartDeletingRule);

			// Add/edit rule sub state machine
			mMainStm.Configure(State.AddEditRule)
				.OnEntryFrom(Trigger.AddRule, () => startAddNewRule())
				.OnEntryFrom(Trigger.EditRule, () => startEditRule())
				.Permit(Trigger.SaveAddEdit, State.SavingRule)
				.Permit(Trigger.CancelAddEdit, State.RestoringRule);

			mMainStm.Configure(State.AddEditRuleDone)
				.OnEntry(() => mMainStm.Fire(Trigger.AddEditDone))
				.Permit(Trigger.AddEditDone, State.DoSomething);

			mMainStm.Configure(State.SavingRule)
				.SubstateOf(State.AddEditRule)
				.OnEntry(() => saveRule())
				.Permit(Trigger.SaveDone, State.AddEditRuleDone)
				.Permit(Trigger.SaveFailed, State.RestoringRule);

			mMainStm.Configure(State.RestoringRule)
				.SubstateOf(State.AddEditRule)
				.OnEntry(() => restoreRule())
				.Permit(Trigger.RestoreDone, State.AddEditRuleDone);

			// Delete
			mMainStm.Configure(State.StartDeletingRule)
				.OnEntry(() => askPermissionToDelete())
				.Permit(Trigger.ConinueDelete, State.DeletingRule)
				.Permit(Trigger.AbortDelete, State.DoSomething);

			mMainStm.Configure(State.DeletingRule)
				.OnEntry(() => deleteRule())
				.Permit(Trigger.DeleteDone, State.DoSomething);
		}

		#endregion State machine
	}
}