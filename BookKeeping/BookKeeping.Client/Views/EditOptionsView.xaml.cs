using System.Windows.Controls;
using BookKeeping.Client.ViewModels;

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