using AutoMapper;
using BookKeeping.Client.Models;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.Services;
using PL.Common.Prism;
using Prism.Commands;
using Prism.Regions;

namespace BookKeeping.Client.ViewModels
{
	public class EditOptionsVm : ViewModelBase, INavigationAware
	{
		#region Fields

		private readonly ISettingsService<Settings> mSettingsService;

		#endregion Fields

		#region Constructor(s)

		public EditOptionsVm(ISettingsService<Settings> settingsService)
		{
			mSettingsService = settingsService;
			WrappedModel = Mapper.Map<OptionsVm>(mSettingsService.Settings);
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

		#region Property WrappedModel

		public OptionsVm WrappedModel { get; }

		#endregion Property WrappedModel

		#region Command CancelCommand

		/// <summary>Field for the Cancel command.
		/// </summary>
		private DelegateCommand mCancelCommand;

		/// <summary>Gets Cancel command.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public DelegateCommand CancelCommand => mCancelCommand
												// Reflection is used to call ChangeCanExecute on the command. Therefore, when the command
												// is not yet bound to the View, the command is instantiated in a different thread than the
												// main thread. Prevent this by checking on the SynchronizationContext.
												?? (mCancelCommand = System.Threading.SynchronizationContext.Current == null
													? null : new DelegateCommand(Cancel, CanCancel));

		/// <summary>
		/// </summary>
		private void Cancel()
		{
			if (mNavigationService.Journal.CanGoBack)
			{
				mNavigationService.Journal.GoBack();
			}
		}

		/// <summary>Determines whether the Cancel command can be executed.
		/// </summary>
		private bool CanCancel()
		{
			return true;
		}

		#endregion Command CancelCommand

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
			mSettingsService.Settings = Mapper.Map<Settings>(WrappedModel);
			mSettingsService.SaveSettings();
			if (mNavigationService.Journal.CanGoBack)
			{
				mNavigationService.Journal.GoBack();
			}
		}

		/// <summary>Determines whether the NavigateBack command can be executed.
		/// </summary>
		private bool CanNavigateBack()
		{
			return true;
		}

		#endregion Command NavigateBackCommand
	}
}