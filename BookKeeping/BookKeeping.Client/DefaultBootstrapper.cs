using Microsoft.Practices.Unity;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.EventAggregatorEvents;
using PL.Logger;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;
using PL.Common.Logger;

namespace BookKeeping.Client
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	internal class DefaultBootstrapper : UnityBootstrapper
	{
		protected override void InitializeShell()
		{
			base.InitializeShell();

			Application.Current.MainWindow = (Window)Shell;
			Application.Current.MainWindow?.Show();
		}

		protected override DependencyObject CreateShell()
		{
			return Container.TryResolve<ShellView>();
		}

		protected override void ConfigureContainer()
		{
			base.ConfigureContainer();
			Container.RegisterInstance(mLogFile);
		}

		protected override void ConfigureModuleCatalog()
		{
			base.ConfigureModuleCatalog();

			AddModuleToCatalog(typeof(PL.BookKeeping.Business.ModuleInit), ModuleCatalog);
			AddModuleToCatalog(typeof(ModuleInit), ModuleCatalog);
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

		private ILogFile mLogFile;

		protected override ILoggerFacade CreateLogger()
		{
			LogFile.DefaultLogLevel = LogFile.LogLevel.Debug;
			mLogFile = new LogFile("Main", LogFile.LogLevel.Debug, 2097152, false); // Max 2 Mb
			mLogFile.WriteLogStart();

			return new PlLoggerFacade(mLogFile);
		}

		protected override void InitializeModules()
		{
			base.InitializeModules();

			var eventAggregator = Container.TryResolve<IEventAggregator>();
			eventAggregator.GetEvent<ModuleInitializationDoneEvent>().Publish(true);
		}
	}
}