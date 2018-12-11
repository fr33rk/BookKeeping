using BookKeeping.Client.ViewModels;
using System.Windows.Controls;

namespace BookKeeping.Client.Views
{
	/// <summary>
	/// Interaction logic for GlobalSearchView.xaml
	/// </summary>
	public partial class GlobalSearchView : UserControl
	{
		public GlobalSearchView(GlobalSearchVm viewModel)
		{
			InitializeComponent();

			DataContext = viewModel;
		}
	}
}