﻿using Microsoft.Practices.Unity;
using PL.BookKeeping.Business.Services;
using PL.BookKeeping.Business.Services.DataServices;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PL.BookKeeping.Data.Repositories;
using PL.BookKeeping.Data;

namespace PL.BookKeeping.Business
{
    public class ModuleInit : IModule
    {
        //private readonly IRegionManager mRegionManager;

        private readonly IUnityContainer mContainer;

        /// <summary>Initializes a new instance of the <see cref="ModuleInit"/> class.</summary>
        /// <param name="container">The container.</param>
        /// <param name="regionManager">The region manager.</param>
        public ModuleInit(IUnityContainer container)
        {
            this.mContainer = container;            
        }

        public void Initialize()
        {
            mContainer.RegisterType<IUnitOfWorkFactory, UnitOfWorkFactoryOfT<DataContext>>(new ContainerControlledLifetimeManager());
            mContainer.RegisterType<IAuthorizationService, AuthorizationService>(new ContainerControlledLifetimeManager());
            mContainer.RegisterType<ITransactionDataService, TransactionDataService>(new ContainerControlledLifetimeManager());
            mContainer.RegisterType<IDataImporterService, DataImporterService>(new ContainerControlledLifetimeManager());
        }
    }
}