using BookKeeping.Client.ViewModels;
using System.Windows.Controls;

namespace BookKeeping.Client.Views
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class EditOptionsView : UserControl
	{
		public EditOptionsView(EditOptionsVm viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}