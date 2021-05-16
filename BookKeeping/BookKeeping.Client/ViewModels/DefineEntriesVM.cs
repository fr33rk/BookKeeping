using AutoMapper;
using BookKeeping.Client.Models;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.EventAggregatorEvents;
using PL.BookKeeping.Infrastructure.Extensions;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore.Internal;

namespace BookKeeping.Client.ViewModels
{
	/// <summary>Allows CRUD operations on entry definitions.
	/// </summary>
	public class DefineEntriesVM : ViewModelBase, INavigationAware
	{
		#region Fields

		private readonly IEntryDataService mEntryDataService;
		private readonly IProcessingRuleDataService mProcessingRuleDataService;
		private readonly IEventAggregator mEventAggregator;
		private bool mEntriesHaveChanged;

		#endregion Fields

		#region Constructor(s)

		public DefineEntriesVM(IEntryDataService entryDataService, IProcessingRuleDataService processingRuleDataService,
			IEventAggregator eventAggregator)
		{
			mEntryDataService = entryDataService;
			mProcessingRuleDataService = processingRuleDataService;
			mEventAggregator = eventAggregator;
		}

		#endregion Constructor(s)

		#region INavigationAware

		private IRegionNavigationService mNavigationService;

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			mNavigationService = navigationContext.NavigationService;
			mEntriesHaveChanged = false;

			LoadData();
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return false;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
		}

		#endregion INavigationAware

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
			if (mEntriesHaveChanged)
			{
				// Update the entries in the main view
				mEventAggregator.GetEvent<DataChangedEvent>().Publish(new DataChangedEventArgs());
			}

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

		#region Command AddEntryCommand

		/// <summary>Field for the AddEntry command.
		/// </summary>
		private DelegateCommand mAddEntryCommand;

		/// <summary>Gets AddEntry command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand AddEntryCommand => mAddEntryCommand
												  // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
												  // is not yet bound to the View, the command is instantiated in a different thread than the
												  // main thread. Prevent this by checking on the SynchronizationContext.
												  ?? (mAddEntryCommand = System.Threading.SynchronizationContext.Current == null
													  ? null : new DelegateCommand(AddEntry, CanAddEntry));

		/// <summary>
		/// </summary>
		private void AddEntry()
		{
			var newEntry = new Entry();
			newEntry.Description = "Nieuw";
			newEntry.ParentEntry = Mapper.Map<Entry>(SelectedEntry);

			mEntryDataService.Add(newEntry);

			SelectedEntry.ChildEntryVms.Add(Mapper.Map<EntryVm>(newEntry));

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
		public DelegateCommand DeleteEntryCommand => mDeleteEntryCommand
													 // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
													 // is not yet bound to the View, the command is instantiated in a different thread than the
													 // main thread. Prevent this by checking on the SynchronizationContext.
													 ?? (mDeleteEntryCommand = System.Threading.SynchronizationContext.Current == null
														 ? null : new DelegateCommand(DeleteEntry, CanDeleteEntry));

		/// <summary>
		/// </summary>
		private void DeleteEntry()
		{
			// Ask for confirmation.
			var result = MessageBox.Show("Weet je zeker dat je deze post wilt verwijderen?", "Bookkeeping", MessageBoxButton.YesNoCancel);

			if (result == MessageBoxResult.Yes)
			{
				// Delete entry
				var rootInList = GetRootEntryOf(SelectedEntry);

				mEntryDataService.Delete(SelectedEntry.ToEntity());

				if (rootInList != null)
				{
					rootInList.DeleteChild(SelectedEntry);
				}

				mEntriesHaveChanged = true;
			}
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanDeleteEntry()
		{
			if ((SelectedEntry != null) && (SelectedEntry.ChildEntryVms.Count == 0))
			{
				return true;
			}

			return false;
		}

		#endregion Command DeleteEntryCommand

        #region CloneEntryCommand

        private DelegateCommand mCloneEntryCommand;
		
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand CloneEntryCommand => mCloneEntryCommand 
                                                    ?? (mCloneEntryCommand = System.Threading.SynchronizationContext.Current == null
                                                        ? null : new DelegateCommand(CloneEntry, CanCloneEntry));

        private void CloneEntry()
        {
            var parentEntryVm = GetEntryVm(SelectedEntry.ParentEntry);
            if (parentEntryVm == null)
                return;

            var clone = mEntryDataService.Clone(SelectedEntry.ToEntity());
            parentEntryVm.ChildEntryVms.Add(Mapper.Map<EntryVm>(clone));
			mEntriesHaveChanged = true;
		}

        private bool CanCloneEntry()
        {
            return SelectedEntry != null && !EnumerableExtensions.Any(SelectedEntry.ChildEntries);
        }


        #endregion

		#region Property Entries

		public ObservableCollection<EntryVm> Entries { get; set; }

		#endregion Property Entries

		#region Property SelectedEntry

		private EntryVm mSelectedEntry;

		public EntryVm SelectedEntry
		{
			get => mSelectedEntry;
			set
			{
				mSelectedEntry = value;

				LoadAttachedProcessingRules();

				NotifyPropertyChanged();
				NotifyPropertyChanged(nameof(SelectedEntryDescription));
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

				var entry = SelectedEntry.ToEntity();

				mEntryDataService.Update(entry);

				var entryVm = EntryVm.FromEntity(entry);

				var rootInList = GetRootEntryOf(entryVm);

				if (rootInList != null)
				{
					rootInList.UpdateChild(entryVm);
				}

				mEntriesHaveChanged = true;
			}
		}

		#endregion Property SelectedEntryDescription

		#region Property AttachedRules

		public ObservableCollection<ProcessingRuleVm> AttachedProcessingRules { get; private set; } = new ObservableCollection<ProcessingRuleVm>();

		#endregion Property AttachedRules

		#region Helper methods

		private void LoadAttachedProcessingRules()
		{
			if (mSelectedEntry != null)
			{
				AttachedProcessingRules.Clear(); // = new ObservableCollection<ProcessingRuleVM>();

				foreach (var rule in mProcessingRuleDataService.GetByEntry(mSelectedEntry.ToEntity()))
				{
					AttachedProcessingRules.Add(ProcessingRuleVm.FromEntity(rule));
				}
			}
		}

		private void LoadData()
		{
			Entries = new ObservableCollection<EntryVm>();

			foreach (var rootEntry in mEntryDataService.GetRootEntries())
			{
				Entries.Add(Mapper.Map<EntryVm>(rootEntry));
			}
		}

		private EntryVm GetRootEntryOf(EntryVm child)
		{
			foreach (var entryVm in Entries)
			{
				if (entryVm.HasChildNode(child))
				{
					return entryVm;
				}
			}

			return null;
		}

        private EntryVm GetEntryVm(Entry entry)
        {
            return Entries
                .Select(entryVm => entryVm.FindEntryVm(entry))
                .FirstOrDefault(result => result != null);
        }

		#endregion Helper methods
	}
}