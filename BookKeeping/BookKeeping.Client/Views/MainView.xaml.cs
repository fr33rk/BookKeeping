using BookKeeping.Client.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BookKeeping.Client.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView(MainVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}