﻿using Microsoft.EntityFrameworkCore;
using PL.BookKeeping.Business.Services;
using PL.BookKeeping.Business.Services.DataServices;
using PL.BookKeeping.Data;
using PL.BookKeeping.Data.Repositories;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using Prism.Ioc;
using Prism.Modularity;
using Unity;
using Unity.Lifetime;

namespace PL.BookKeeping.Business
{
	public class ModuleInit : IModule
	{
		private readonly IUnityContainer mContainer;

		/// <summary>Initializes a new instance of the <see cref="ModuleInit"/> class.</summary>
		/// <param name="container">The container.</param>
		public ModuleInit(IUnityContainer container)
		{
			mContainer = container;
		}

		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			mContainer.RegisterType<ISettingsService<Settings>, SettingsService>(new ContainerControlledLifetimeManager());
			mContainer.RegisterType<IDbConnectionFactory, MySqlConnectionFactory>(new ContainerControlledLifetimeManager());
			mContainer.RegisterType<IUnitOfWorkFactory, UnitOfWorkFactoryOfT<DataContext>>(new ContainerControlledLifetimeManager());
			mContainer.RegisterType<IAuthorizationService, AuthorizationService>(new ContainerControlledLifetimeManager());

			mContainer.RegisterType<ITransactionDataService, TransactionDataService>(new ContainerControlledLifetimeManager());
			mContainer.RegisterType<IEntryDataService, EntryDataService>(new ContainerControlledLifetimeManager());
			mContainer.RegisterType<IEntryPeriodDataService, EntryPeriodDataService>(new ContainerControlledLifetimeManager());
			mContainer.RegisterType<IPeriodDataService, PeriodDataService>(new ContainerControlledLifetimeManager());
			mContainer.RegisterType<IProcessingRuleDataService, ProcessingRuleDataService>(new ContainerControlledLifetimeManager());

			mContainer.RegisterType<IDataImporterService, DataImporterService>(new ContainerControlledLifetimeManager());
			mContainer.RegisterType<IDataExporterService, DataExporterService>(new ContainerControlledLifetimeManager());
			mContainer.RegisterType<IDataProcessorService, DataProcessorService>(new ContainerControlledLifetimeManager());
		}

		public void OnInitialized(IContainerProvider containerProvider)
		{
			LoadSettings();

			// Migrate the database to the last version.
			var connectionFactory = mContainer.Resolve<MySqlConnectionFactory>();
			var context = (DataContext)System.Activator.CreateInstance(typeof(DataContext), connectionFactory.Create());
			context.Database.Migrate();
		}

		private void LoadSettings()
		{
			var settingService = mContainer.Resolve<ISettingsService<Settings>>();

			settingService.LoadSettings();
		}
	}
}