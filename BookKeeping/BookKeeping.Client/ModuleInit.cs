using AutoMapper;
using BookKeeping.Client.Models;
using BookKeeping.Client.ViewModels;
using BookKeeping.Client.Views;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Unity;

namespace BookKeeping.Client
{
	internal class ModuleInit : IModule
	{
		private readonly IRegionManager mRegionManager;

		private readonly IUnityContainer mContainer;

		/// <summary>Initializes a new instance of the <see cref="ModuleInit"/> class.</summary>
		/// <param name="container">The container.</param>
		/// <param name="regionManager">The region manager.</param>
		public ModuleInit(IUnityContainer container, IRegionManager regionManager)
		{
			mContainer = container;
			mRegionManager = regionManager;
		}

		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			mContainer.RegisterType<object, MainView>(typeof(MainView).FullName);
			mContainer.RegisterType<object, ImportDataView>(typeof(ImportDataView).FullName);
			mContainer.RegisterType<object, DefineEntriesView>(typeof(DefineEntriesView).FullName);
			mContainer.RegisterType<object, DefineRulesView>(typeof(DefineRulesView).FullName);
			mContainer.RegisterType<object, ReApplyRulesView>(typeof(ReApplyRulesView).FullName);
			mContainer.RegisterType<object, GlobalSearchView>(typeof(GlobalSearchView).FullName);
			mContainer.RegisterType<object, EditOptionsView>(typeof(EditOptionsView).FullName);
		}

		public void OnInitialized(IContainerProvider containerProvider)
		{
			mRegionManager.RequestNavigate(RegionNames.MainRegion, typeof(MainView).FullName);

			Mapper.Initialize(config =>
			{
				config.CreateMap<ProcessingRule, ProcessingRuleVm>();
				config.CreateMap<Entry, EntryVm>();
				config.CreateMap<EntryVm, Entry>();
				config.CreateMap<ProcessingRule, ProcessingRuleVm>();
				config.CreateMap<ProcessingRuleVm, ProcessingRule>();
				config.CreateMap<GlobalSearchView, GlobalSearchVm>();
				config.CreateMap<GlobalSearchVm, GlobalSearchView>();
				config.CreateMap<Settings, OptionsVm>();
			});
		}
	}
}