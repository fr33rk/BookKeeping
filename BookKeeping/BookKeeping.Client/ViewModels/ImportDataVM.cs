using Microsoft.Win32;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Services;
using PL.Common.Prism;
using PL.Logger;
using Prism.Commands;
using Prism.Regions;
using Stateless;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace BookKeeping.Client.ViewModels
{
	public class ImportDataVm : ViewModelBase, INavigationAware, IDisposable
	{
		#region Fields

		private readonly IDataImporterService mDataImportService;
		private readonly IDataProcessorService mDataProcessorService;
		private readonly ILogFile mLogFile;
		private readonly BackgroundWorker mImportWorker;
		private readonly BackgroundWorker mProcessorWorker;
		private IList<Transaction> mImportedTransactions;

		#endregion Fields

		#region Constructor(s)

		/// <summary>Initializes a new instance of the <see cref="ImportDataVm"/> class.
		/// </summary>
		/// <param name="dataImportService">The data import service.</param>
		/// <param name="dataProcessorService">The data processor service.</param>
		/// <param name="logFile">The log file.</param>
		public ImportDataVm(IDataImporterService dataImportService, IDataProcessorService dataProcessorService, ILogFile logFile)
		{
			InitializeStateMachine();

			SelectedFiles = new ObservableCollection<string>();

			mDataImportService = dataImportService;
			mDataImportService.OnDataProcessed += DataImportService_OnDataProcessed;
			mLogFile = logFile;
			mDataProcessorService = dataProcessorService;
			mDataProcessorService.OnDataProcessed += DataProcessorService_OnDataProcessed;

			mImportWorker = new BackgroundWorker();
			mImportWorker.DoWork += ImportWorker_DoWork;
			mImportWorker.RunWorkerCompleted += ImportWorker_RunWorkerCompleted;

			mProcessorWorker = new BackgroundWorker();
			mProcessorWorker.DoWork += ProcessorWorker_DoWork;
			mProcessorWorker.RunWorkerCompleted += ProcessorWorker_RunWorkerCompleted;

			mVmStateMachine.Fire(VmTrigger.Start);
		}

		#endregion Constructor(s)

		#region ProcessorWorker

		private void ProcessorWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			mVmStateMachine.Fire(VmTrigger.ProcessingDone);
		}

		private void ProcessorWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			mDataProcessorService.Process(mImportedTransactions);
		}

		private void DataProcessorService_OnDataProcessed(object sender, PL.BookKeeping.Infrastructure.DataProcessedEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				ProcessedTransactions = e.Processed;
				IgnoredTransactions = e.Ignored;
			});
		}

		#endregion ProcessorWorker

		#region ImportWorker

		private void ImportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			mVmStateMachine.Fire(VmTrigger.ImportDone);
		}

		private void ImportWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			mImportedTransactions = mDataImportService.ImportFiles(SelectedFiles);
		}

		private void DataImportService_OnDataProcessed(object sender, PL.BookKeeping.Infrastructure.DataImportedEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				TransactionsImported = e.Imported;
				DuplicateTransactions = e.Duplicate;
			});
		}

		#endregion ImportWorker

		#region Property TransactionsImported

		private int mTransactionsImported;

		public int TransactionsImported
		{
			get => mTransactionsImported;
			set
			{
				mTransactionsImported = value;
				NotifyPropertyChanged();
			}
		}

		#endregion Property TransactionsImported

		#region Property DuplicateTransactions

		private int mDuplicateTransactions;

		public int DuplicateTransactions
		{
			get => mDuplicateTransactions;
			set
			{
				mDuplicateTransactions = value;
				NotifyPropertyChanged();
			}
		}

		#endregion Property DuplicateTransactions

		#region Property ProcessedTransactions

		private int mProccesedTransactions;

		public int ProcessedTransactions
		{
			get => mProccesedTransactions;
			set
			{
				mProccesedTransactions = value;
				NotifyPropertyChanged();
			}
		}

		#endregion Property ProcessedTransactions

		#region Property IgnoredTransactions

		private int mIgnoredTransactions;

		public int IgnoredTransactions
		{
			get => mIgnoredTransactions;
			set
			{
				mIgnoredTransactions = value;
				NotifyPropertyChanged();
			}
		}

		#endregion Property IgnoredTransactions

		#region property SelectedFiles

		public ObservableCollection<string> SelectedFiles { get; private set; }

		#endregion property SelectedFiles

		#region Property ShowWarning

		public Visibility ShowWarning { get; private set; } = Visibility.Collapsed;

		#endregion Property ShowWarning

		#region Command SelectFilesCommand

		/// <summary>Field for the StartMeasurement command.
		/// </summary>
		private DelegateCommand mSelectFilesCommand;

		/// <summary>Gets StartMeasurement command.
		/// </summary>
		[Browsable(false)]
		public DelegateCommand SelectFilesCommand => mSelectFilesCommand
													 // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
													 // is not yet bound to the View, the command is instantiated in a different thread than the
													 // main thread. Prevent this by checking on the SynchronizationContext.
													 ?? (mSelectFilesCommand = System.Threading.SynchronizationContext.Current == null
														 ? null : new DelegateCommand(SelectFiles, CanSelectFiles));

		/// <summary>Starts the measurement of a sample.
		/// </summary>
		private void SelectFiles()
		{
			var dialog = new OpenFileDialog()
			{
				Filter = "Comma separated files (*.csv)|*.csv",
				Multiselect = true,
			};

			if (dialog.ShowDialog().GetValueOrDefault())
			{
				SelectedFiles.Clear();
				foreach (var fileName in dialog.FileNames)
				{
					SelectedFiles.Add(fileName);
					mLogFile.Info($"Selected file for import: {fileName}");
				}
				mVmStateMachine.Fire(VmTrigger.FilesSelected);
			}
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanSelectFiles()
		{
			return mVmStateMachine.IsInState(VmState.SelectingFiles) ||
				mVmStateMachine.IsInState(VmState.WaitingOnImport);
		}

		#endregion Command SelectFilesCommand

		#region Command ImportFilesCommand

		/// <summary>Field for the StartMeasurement command.
		/// </summary>
		private DelegateCommand mImportFilesCommand;

		/// <summary>Gets StartMeasurement command.
		/// </summary>
		[Browsable(false)]
		public DelegateCommand ImportFilesCommand => mImportFilesCommand
													 // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
													 // is not yet bound to the View, the command is instantiated in a different thread than the
													 // main thread. Prevent this by checking on the SynchronizationContext.
													 ?? (mImportFilesCommand = System.Threading.SynchronizationContext.Current == null
														 ? null : new DelegateCommand(ImportFiles, CanImportFiles));

		/// <summary>Starts the measurement of a sample.
		/// </summary>
		private void ImportFiles()
		{
			mVmStateMachine.Fire(VmTrigger.StartImport);

			mImportWorker.RunWorkerAsync();
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanImportFiles()
		{
			return mVmStateMachine.IsInState(VmState.WaitingOnImport);
		}

		#endregion Command ImportFilesCommand

		#region State machine

		private enum VmState
		{
			Idle,
			CheckIfImportIsAllowed,
			SelectingFiles,
			WaitingOnImport,
			Importing,
			Processing,
		};

		private enum VmTrigger
		{
			Start,
			CheckPassed,
			FilesSelected,
			StartImport,
			ImportDone,
			ProcessingDone,
		}

		private StateMachine<VmState, VmTrigger> mVmStateMachine;

		private void InitializeStateMachine()
		{
			mVmStateMachine = new StateMachine<VmState, VmTrigger>(VmState.Idle);

			mVmStateMachine.OnTransitioned((t) =>
			{
				mLogFile.Debug($"ImportDataVM - VMStateMachine, changed state to {t.Destination}");
				SelectFilesCommand.RaiseCanExecuteChanged();
				ImportFilesCommand.RaiseCanExecuteChanged();
			});

			mVmStateMachine.Configure(VmState.Idle)
				.Permit(VmTrigger.Start, VmState.CheckIfImportIsAllowed);

			mVmStateMachine.Configure(VmState.CheckIfImportIsAllowed)
				.OnEntry(CheckIfImportIsAllowed)
				.Permit(VmTrigger.CheckPassed, VmState.SelectingFiles);

			mVmStateMachine.Configure(VmState.SelectingFiles)
				.Permit(VmTrigger.FilesSelected, VmState.WaitingOnImport);

			mVmStateMachine.Configure(VmState.WaitingOnImport)
				.PermitReentry(VmTrigger.FilesSelected)
				.Permit(VmTrigger.StartImport, VmState.Importing);

			mVmStateMachine.Configure(VmState.Importing)
				.OnEntry(() =>
				{
					mLogFile.Info("Started importing selected files");
				})
				.Permit(VmTrigger.ImportDone, VmState.Processing);

			mVmStateMachine.Configure(VmState.Processing)
				.OnEntry(() =>
				{
					mProcessorWorker.RunWorkerAsync();
					mLogFile.Info("Started processing imported transactions.");
				})
				.Permit(VmTrigger.ProcessingDone, VmState.SelectingFiles)
				.OnExit(() =>
				{
					SelectedFiles.Clear();
				});
		}

		private void CheckIfImportIsAllowed()
		{
			if (mDataImportService.IsReadyForImport)
			{
				mVmStateMachine.Fire(VmTrigger.CheckPassed);
			}
			else
			{
				ShowWarning = Visibility.Visible;
			}
		}

		#endregion State machine

		#region Command NavigateBackCommand

		/// <summary>Field for the NavigateBack command.
		/// </summary>
		private DelegateCommand mNavigateBackCommand;

		/// <summary>Gets NavigateBack command.
		/// </summary>
		[Browsable(false)]
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
			//throw new NotImplementedException();
		}

		#endregion INavigationAware

		#region IDisposable

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Finalizes an instance of the <see cref="ImportDataVm"/> class.
		/// </summary>
		~ImportDataVm()
		{
			Dispose(false);
		}

		/// <summary>Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="includeManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool includeManaged)
		{
			if (includeManaged)
			{
				// free managed resources
			}

			try
			{
				if (mImportWorker != null)
				{
					mImportWorker.Dispose();
				}

				if (mProcessorWorker != null)
				{
					mProcessorWorker.Dispose();
				}
			}
			catch (ObjectDisposedException e)
			{
				mLogFile.Error($"~ImportDataVM raised exception: {e}");
			}
		}

		#endregion IDisposable
	}
}