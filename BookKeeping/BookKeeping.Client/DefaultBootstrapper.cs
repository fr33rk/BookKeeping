﻿using System;
using System.Windows;
using Microsoft.Practices.Unity;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Business;
using PL.Logger;
using Prism.Logging;
using Prism.Modularity;
using Prism.Unity;

namespace BookKeeping.Client
{
    class DefaultBootstrapper : UnityBootstrapper
    {
        protected override void InitializeShell()
        {
            base.InitializeShell();

            App.Current.MainWindow = (Window)Shell;
            App.Current.MainWindow.Show();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.TryResolve<ShellView>();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterInstance<ILogFile>(mLogFile);
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            // Load business unit first, because the services are used in other modules.
            //AddModuleToCatalog(typeof(PL.BookKeeping.Business.ModuleInit), this.ModuleCatalog);

            AddModuleToCatalog(typeof(PL.BookKeeping.Business.ModuleInit), this.ModuleCatalog);
            AddModuleToCatalog(typeof(BookKeeping.Client.ModuleInit), this.ModuleCatalog);
        }

        /// <summary>Adds the module to catalog with an unique name (AssemblyQualifiedName).</summary>
        /// <param name="moduleType">Type of the module.</param>
        /// <param name="catalog">The catalog.</param>
        /// Otherwise ModuleInit has to be named differently in each module.
        private void AddModuleToCatalog(Type moduleType, IModuleCatalog catalog)
        {
            ModuleInfo NewModuleInfo = new ModuleInfo();
            NewModuleInfo.ModuleName = moduleType.AssemblyQualifiedName;
            NewModuleInfo.ModuleType = moduleType.AssemblyQualifiedName;
            NewModuleInfo.InitializationMode = InitializationMode.WhenAvailable;
            catalog.AddModule(NewModuleInfo);
        }

        private ILogFile mLogFile;

        protected override ILoggerFacade CreateLogger()
        {
            LogFile.DefaultLogLevel = LogFile.LogLevel.Debug;
            mLogFile = new LogFile("Main");
            mLogFile.WriteLogStart();

            return new PlLoggerFacade(mLogFile);
        }
    }
}
