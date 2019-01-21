using CommonServiceLocator;
using PL.BookKeeping.Infrastructure;
using PL.Common.Logger;
using PL.Logger;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using PL.BookKeeping.Infrastructure.EventAggregatorEvents;
using Prism.Events;

namespace BookKeeping.Client
{
	/// <summary>Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
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
		}

		protected override void OnExit(ExitEventArgs e)
		{
			var logFile = Container.Resolve<ILogFile>();
			logFile?.WriteLogEnd();

			base.OnExit(e);
		}

		protected override void InitializeShell(Window shell)
		{
			base.InitializeShell(shell);

			Current.MainWindow = shell;
			Current.MainWindow?.Show();
		}

		protected override Window CreateShell()
		{
			return ServiceLocator.Current.GetInstance<ShellView>();
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			var loggerFacade = CreateLogger();

			containerRegistry.RegisterInstance(typeof(ILogFile), mLogFile);
			containerRegistry.RegisterInstance(typeof(ILoggerFacade), loggerFacade);
		}

		#region CreateModuleCatalog

		protected override IModuleCatalog CreateModuleCatalog()
		{
			var moduleCatalog = base.CreateModuleCatalog();

			AddModuleToCatalog(typeof(PL.BookKeeping.Business.ModuleInit), moduleCatalog);
			AddModuleToCatalog(typeof(ModuleInit), moduleCatalog);

			return moduleCatalog;
		}

		/// <summary>Adds the module to catalog with an unique name (AssemblyQualifiedName).</summary>
		/// <param name="moduleType">Type of the module.</param>
		/// <param name="catalog">The catalog.</param>
		/// Otherwise ModuleInit has to be named differently in each module.
		private void AddModuleToCatalog(Type moduleType, IModuleCatalog catalog)
		{
			var newModuleInfo = new ModuleInfo
			{
				ModuleName = moduleType.AssemblyQualifiedName,
				ModuleType = moduleType.AssemblyQualifiedName,
				InitializationMode = InitializationMode.WhenAvailable
			};
			catalog.AddModule(newModuleInfo);
		}

		#endregion CreateModuleCatalog

		#region LogFile

		private ILogFile mLogFile;

		private ILoggerFacade CreateLogger()
		{
			LogFile.DefaultLogLevel = LogFile.LogLevel.Debug;
			mLogFile = new LogFile("Main", LogFile.LogLevel.Debug, 2097152, false); // Max 2 Mb
			mLogFile.WriteLogStart();

			return new PlLoggerFacade(mLogFile);
		}

		#endregion LogFile

		protected override void InitializeModules()
		{
			base.InitializeModules();

			var eventAggregator = Container.Resolve<IEventAggregator>();
			eventAggregator.GetEvent<ModuleInitializationDoneEvent>().Publish(true);
		}
	}
}