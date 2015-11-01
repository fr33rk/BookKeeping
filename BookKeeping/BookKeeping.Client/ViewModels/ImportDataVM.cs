using Microsoft.Practices.Unity;
using Microsoft.Win32;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using PL.Logger;
using Prism.Commands;
using Prism.Regions;
using Prism.Unity;
using Stateless;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookKeeping.Client.ViewModels
{
    public class ImportDataVM : ViewModelBase
    {

        private IDataImporterService mDataImportService;
        private ILogFile mLogFile;

        public ImportDataVM(IRegionManager regionManager, IDataImporterService dataImportService, ILogFile logFile)
        {
            InitializeStateMachine();

            SelectedFiles = new ObservableCollection<string>();

            mDataImportService = dataImportService;
            mLogFile = logFile;
        }


        #region property SelectedFiles

        public ObservableCollection<string> SelectedFiles { get; private set; }

        #endregion

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
            }
        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanSelectFiles()
        {
            return mVMStateMachine.IsInState(VMState.SelectingFiles);
        }

        #endregion

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
            mDataImportService.ImportFiles(SelectedFiles);

        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanImportFiles()
        {
            return mVMStateMachine.IsInState(VMState.SelectingFiles);
        }

        #endregion

        #region State machine

        private enum VMState
        {
            SelectingFiles,
            Imporing,
            Done,
        };

        private enum VMTrigger
        {
            StartImport,
            ImportDone,
            
        }

        private StateMachine<VMState, VMTrigger> mVMStateMachine;

        private void InitializeStateMachine()
        {
            mVMStateMachine = new StateMachine<VMState, VMTrigger>(VMState.SelectingFiles);

            mVMStateMachine.Configure(VMState.SelectingFiles)
                .Permit(VMTrigger.StartImport, VMState.Imporing);

            mVMStateMachine.Configure(VMState.Imporing)
                .OnEntry(() => mLogFile.Info("Started importing selected files"))
                .Permit(VMTrigger.ImportDone, VMState.Done);
        }

        #endregion
    }
}
