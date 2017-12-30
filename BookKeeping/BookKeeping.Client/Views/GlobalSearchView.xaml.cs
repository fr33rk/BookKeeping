using System.Windows.Controls;
using BookKeeping.Client.ViewModels;
using PL.Common.Prism;

namespace BookKeeping.Client.Views
{
	/// <summary>
	/// Interaction logic for GlobalSearchView.xaml
	/// </summary>
	public partial class GlobalSearchView : UserControl
	{
		public GlobalSearchView(GlobalSearchVM viewModel)
		{
			InitializeComponent();

			DataContext = viewModel;
		}
	}
}