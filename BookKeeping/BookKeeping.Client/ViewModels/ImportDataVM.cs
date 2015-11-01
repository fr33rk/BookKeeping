using Microsoft.Practices.Unity;
using Microsoft.Win32;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Common.Prism;
using Prism.Commands;
using Prism.Regions;
using Prism.Unity;
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

        //public ImportDataVM(IDataImporterService dataImportService)
        //{
        //    mDataImportService = dataImportService;

            
        //}

        public ImportDataVM(IRegionManager regionManager, IUnityContainer test)
        {
            SelectedFiles = new ObservableCollection<string>();
            mDataImportService = test.TryResolve<IDataImporterService>();
        }


        #region property SelectedFiles

        public ObservableCollection<string> SelectedFiles { get; private set; }

        #endregion

        #region Command JustDoItCommand

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
                }

                mDataImportService.ImportFiles(SelectedFiles);
            }
        }

        /// <summary>Determines whether the StartMeasurement command can be executed.
        /// </summary>
        private bool CanSelectFiles()
        {
            return true;
        }

        #endregion Command JustDoItCommand
    }
}
