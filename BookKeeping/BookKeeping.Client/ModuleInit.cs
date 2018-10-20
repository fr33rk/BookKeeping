using AutoMapper;
using BookKeeping.Client.Models;
using BookKeeping.Client.ViewModels;
using BookKeeping.Client.Views;
using Microsoft.Practices.Unity;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure;
using Prism.Modularity;
using Prism.Regions;

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
            this.mContainer = container;
            this.mRegionManager = regionManager;
        }

        public void Initialize()
        {
            mContainer.RegisterType<object, MainView>(typeof(MainView).FullName);
            mContainer.RegisterType<object, ImportDataView>(typeof(ImportDataView).FullName);
            mContainer.RegisterType<object, DefineEntriesView>(typeof(DefineEntriesView).FullName);
            mContainer.RegisterType<object, DefineRulesView>(typeof(DefineRulesView).FullName);
			mContainer.RegisterType<object, ReApplyRulesView>(typeof(ReApplyRulesView).FullName);
	        mContainer.RegisterType<object, GlobalSearchView>(typeof(GlobalSearchView).FullName);
		        
            mRegionManager.RequestNavigate(RegionNames.MainRegion, typeof(MainView).FullName);

	        Mapper.Initialize(config => {
		        config.CreateMap<ProcessingRule, ProcessingRuleVM>();
		        config.CreateMap<Entry, EntryVM>();
		        config.CreateMap<EntryVM, Entry>();
		        config.CreateMap<ProcessingRule, ProcessingRuleVM>();
		        config.CreateMap<ProcessingRuleVM, ProcessingRule>();
		        config.CreateMap<GlobalSearchView, GlobalSearchVm>();
				config.CreateMap<GlobalSearchVm, GlobalSearchView>();
	        });


        }
    }
}