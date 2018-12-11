using BookKeeping.Client.ViewModels;
using System.Windows.Controls;

namespace BookKeeping.Client.Views
{
	/// <summary>
	/// Interaction logic for ImportDataView.xaml
	/// </summary>
	public partial class ImportDataView : UserControl
	{
		public ImportDataView(ImportDataVM vm)
		{
			InitializeComponent();

			DataContext = vm;
		}
	}
}