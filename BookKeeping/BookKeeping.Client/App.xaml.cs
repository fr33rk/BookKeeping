using PL.Logger;
using Prism.Unity;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace BookKeeping.Client
{
	/// <summary>Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		private DefaultBootstrapper mBootstrapper;

		protected override void OnStartup(StartupEventArgs e)
		{
			// Set the correct culture...
			FrameworkElement.LanguageProperty.OverrideMetadata(
				typeof(FrameworkElement),
				new FrameworkPropertyMetadata(
					XmlLanguage.GetLanguage(
						CultureInfo.CurrentCulture.IetfLanguageTag)));

			base.OnStartup(e);

			// Set the current user interface culture to the specific culture Russian
			System.Threading.Thread.CurrentThread.CurrentUICulture =
				new CultureInfo("en");

			// Configure Bootstrapper
			mBootstrapper = new DefaultBootstrapper();
			mBootstrapper.Run();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			var logFile = mBootstrapper.Container.TryResolve<ILogFile>();
			logFile?.WriteLogEnd();

			base.OnExit(e);
		}
	}
}