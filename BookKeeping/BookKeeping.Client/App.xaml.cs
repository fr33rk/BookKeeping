using PL.Logger;
using Prism.Unity;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

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
            // Set the correct culture...
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                        CultureInfo.CurrentCulture.IetfLanguageTag)));

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