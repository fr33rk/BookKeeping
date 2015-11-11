using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Win32;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Services;
using PL.Common.Prism;
using PL.Logger;
using Prism.Commands;
using Prism.Regions;
using Stateless;

namespace BookKeeping.Client.ViewModels
{
    public class ImportDataVM : ViewModelBase
    {
        private IDataImporterService mDataImportService;
        private IDataProcessorService mDataProcessorService;
        private ILogFile mLogFile;
        private BackgroundWorker mImportWorker;
        private BackgroundWorker mProcessorWorker;
        private IList<Transaction> mImportedTransactions;

        public ImportDataVM(IRegionManager regionManager, IDataImporterService dataImportService, IDataProcessorService dataProcessorService, ILogFile logFile)
        {
            InitializeStateMachine();

            SelectedFiles = new ObservableCollection<string>();

            mDataImportService = dataImportService;
            mDataImportService.OnDataProcessed += DataImportService_OnDataProcessed;
            mLogFile = logFile;
            mDataProcessorService = dataProcessorService;

            mImportWorker = new BackgroundWorker();
            mImportWorker.DoWork += ImportWorker_DoWork;
            mImportWorker.RunWorkerCompleted += ImportWorker_RunWorkerCompleted;

            mProcessorWorker = new BackgroundWorker();
            mProcessorWorker.DoWork += ProcessorWorker_DoWork;
            mProcessorWorker.RunWorkerCompleted += ProcessorWorker_RunWorkerCompleted;
        }

        #region ProcessorWorker

        private void ProcessorWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mVMStateMachine.Fire(VMTrigger.ProcessingDone);
        }

        private void ProcessorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            mDataProcessorService.Process(mImportedTransactions);
        }

        #endregion ProcessorWorker

        #region ImportWorker

        private void ImportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mVMStateMachine.Fire(VMTrigger.ImportDone);
        }

        private void ImportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            mDataImportService.ImportFiles(SelectedFiles);
        }

        private void DataImportService_OnDataProcessed(object sender, PL.BookKeeping.Infrastructure.DataImportedEventArgs e)
        {
            TransactionsImported = e.Imported;
            DuplicateTransactions = e.Duplicate;
        }

        #endregion ImportWorker

        #region Property TransactionsImported

        private int mTransactionsImported;

        public int TransactionsImported
        {
            get
            {
                return mTransactionsImported;
            }
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
            get
            {
                return mDuplicateTransactions;
            }
            set
            {
                mDuplicateTransactions = value;
                NotifyPropertyChanged();
            }
        }

        #endregion Property DuplicateTransactions

        #region property SelectedFiles

        public ObservableCollection<string> SelectedFiles { get; private set; }

        #endregion property SelectedFiles

        #region Command SelectFilesCommand

        /// <summary>Field for the StartMeasurement command.
        /// </summary>
        private DelegateCommand mSelectFilesCommand;

        /// <summary>Gets StartMeasurement command.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public DelegateCommand SelectFilesCommand
        {
            get
            {
                return this.mSelectFilesCommand
                    // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
                    // is not yet bound to the View, the command is instantiated in a different thread than the
                    // main thread. Prevent this by checking on the SynchronizationContext.
                    ?? (this.mSelectFilesCommand = System.Threading.SynchronizationContext.Current == null
                    ? null : new DelegateCommand(this.SelectFiles, this.CanSelectFiles));
            }
        }

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
                    mLogFile.Info(string.Format("Selected file for import: {0}", fileName));
                }
                mVMStateMachine.Fire(VMTrigger.FilesSelected);
            }
        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanSelectFiles()
        {
            return mVMStateMachine.IsInState(VMState.SelectingFiles) ||
                mVMStateMachine.IsInState(VMState.WaitingOnImport);
        }

        #endregion Command SelectFilesCommand

        #region Command ImportFilesCommand

        /// <summary>Field for the StartMeasurement command.
        /// </summary>
        private DelegateCommand mImportFilesCommand;

        /// <summary>Gets StartMeasurement command.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public DelegateCommand ImportFilesCommand
        {
            get
            {
                return this.mImportFilesCommand
                    // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
                    // is not yet bound to the View, the command is instantiated in a different thread than the
                    // main thread. Prevent this by checking on the SynchronizationContext.
                    ?? (this.mImportFilesCommand = System.Threading.SynchronizationContext.Current == null
                    ? null : new DelegateCommand(this.ImportFiles, this.CanImportFiles));
            }
        }

        /// <summary>Starts the measurement of a sample.
        /// </summary>
        private void ImportFiles()
        {
            mVMStateMachine.Fire(VMTrigger.StartImport);

            mImportWorker.RunWorkerAsync();
        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanImportFiles()
        {
            return mVMStateMachine.IsInState(VMState.WaitingOnImport);
        }

        #endregion Command ImportFilesCommand

        #region State machine

        private enum VMState
        {
            SelectingFiles,
            WaitingOnImport,
            Imporing,
            Processing,
        };

        private enum VMTrigger
        {
            FilesSelected,
            StartImport,
            ImportDone,
            ProcessingDone,
        }

        private StateMachine<VMState, VMTrigger> mVMStateMachine;

        private void InitializeStateMachine()
        {
            mVMStateMachine = new StateMachine<VMState, VMTrigger>(VMState.SelectingFiles);

            mVMStateMachine.OnTransitioned((t) =>
            {
                SelectFilesCommand.RaiseCanExecuteChanged();
                ImportFilesCommand.RaiseCanExecuteChanged();
            });

            mVMStateMachine.Configure(VMState.SelectingFiles)
                .Permit(VMTrigger.FilesSelected, VMState.WaitingOnImport);

            mVMStateMachine.Configure(VMState.WaitingOnImport)
                .Permit(VMTrigger.StartImport, VMState.Imporing);

            mVMStateMachine.Configure(VMState.Imporing)
                .OnEntry(() =>
                {
                    mLogFile.Info("Started importing selected files");
                })
                .Permit(VMTrigger.ImportDone, VMState.Processing);

            mVMStateMachine.Configure(VMState.Processing)
                .OnEntry(() =>
                {
                    mLogFile.Info("Started processing imported transactions.");
                })
                .Permit(VMTrigger.ProcessingDone, VMState.SelectingFiles)
                .OnExit(() =>
                {
                    SelectedFiles.Clear();
                });
        }

        #endregion State machine
    }
}