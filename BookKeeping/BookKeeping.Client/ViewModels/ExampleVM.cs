using PL.Common.Prism;
using PL.Logger;
using Prism.Commands;
using Prism.Events;

namespace BookKeeping.Client.ViewModels
{
	public class ExampleVM : ViewModelBase
	{
		private readonly ILogFile mLogFile;

		private readonly IEventAggregator mEventAggregator;

		public ExampleVM(ILogFile logFile, IEventAggregator eventAggregator)
		{
			mLogFile = logFile;
			mEventAggregator = eventAggregator;

			// mEventAggregator.GetEvent<StatusChangedEvent>().Subscribe(this.StatusChanged, ThreadOption.UIThread);
		}

		#region Command ExampleCommand

		/// <summary>Field for the StartMeasurement command.
		/// </summary>
		private DelegateCommand ExampleCommand;

		/// <summary>Gets StartMeasurement command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand StartMeasurementCommand => ExampleCommand
														  // Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
														  // is not yet bound to the View, the command is instantiated in a different thread than the
														  // main thread. Prevent this by checking on the SynchronizationContext.
														  ?? (ExampleCommand = System.Threading.SynchronizationContext.Current == null
															  ? null : new DelegateCommand(StartExample, CanStartExample));

		/// <summary>Starts the measurement of a sample.
		/// </summary>
		private void StartExample()
		{
			// Do something
		}

		/// <summary>Determines whether the StartMeasurement command can be executed.
		/// </summary>
		private bool CanStartExample()
		{
			return true;
		}

		#endregion Command ExampleCommand
	}
}