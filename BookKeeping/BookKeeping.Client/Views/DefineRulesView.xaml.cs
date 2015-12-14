using BookKeeping.Client.ViewModels;
using System.Windows.Controls;

namespace BookKeeping.Client.Views
{
    /// <summary>
    /// Interaction logic for DefineRulesView.xaml
    /// </summary>
    public partial class DefineRulesView : UserControl
    {
        public DefineRulesView(DefineRulesVM vm)
        {
            InitializeComponent();

            DataContext = vm;
        }
    }
}