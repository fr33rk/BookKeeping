using Prism.Logging;
using System.Windows;

namespace BookKeeping.Client
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView(ILoggerFacade logger)
        {
            InitializeComponent();

            logger.Log("Starting shell view...", Category.Debug, Priority.Low);
        }
    }
}