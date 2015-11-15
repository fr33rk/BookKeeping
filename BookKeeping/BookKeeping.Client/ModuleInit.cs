using BookKeeping.Client.Views;
using Microsoft.Practices.Unity;
using PL.BookKeeping.Infrastructure;
using Prism.Modularity;
using Prism.Regions;

namespace BookKeeping.Client
{
    class ModuleInit : IModule
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
            mContainer.RegisterType<object, ImportDataView> (typeof(ImportDataView).FullName);
            mContainer.RegisterType<object, DefineEntriesView>(typeof(DefineEntriesView).FullName);

            mRegionManager.RequestNavigate(RegionNames.MainRegion, typeof(MainView).FullName);
        }      
    }
}
