using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PL.Logger;
using Prism.Unity;

namespace BookKeeping.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private DefaultBootstrapper mBootStrapper;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set the current user interface culture to the specific culture Russian
            System.Threading.Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo("en");

            // Configure Bootstrapper
            mBootStrapper = new DefaultBootstrapper();
            mBootStrapper.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var logFile = mBootStrapper.Container.TryResolve<ILogFile>();
            if (logFile != null)
            {
                logFile.WriteLogEnd();
            }

            base.OnExit(e);
        }
    }
}
