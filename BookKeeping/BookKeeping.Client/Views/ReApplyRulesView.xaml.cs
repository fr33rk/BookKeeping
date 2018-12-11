using BookKeeping.Client.ViewModels;
using System.Windows.Controls;

namespace BookKeeping.Client.Views
{
	/// <summary>
	/// Interaction logic for ReApplyRulesView.xaml
	/// </summary>
	public partial class ReApplyRulesView : UserControl
	{
		public ReApplyRulesView(ReApplyRulesVM vm)
		{
			InitializeComponent();

			DataContext = vm;
		}
	}
}