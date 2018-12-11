using BookKeeping.Client.Models;
using BookKeeping.Client.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BookKeeping.Client.Views
{
	/// <summary>Interaction logic for DefineEntitiesView.xaml
	/// </summary>
	public partial class DefineEntriesView : UserControl
	{
		public static readonly DependencyProperty SelectedEntry =
			DependencyProperty.Register("SelectedEntry", typeof(EntryVm),
			typeof(DefineEntriesView), new PropertyMetadata(default(EntryVm)));

		public DefineEntriesView(DefineEntriesVM vm)
		{
			InitializeComponent();

			DataContext = vm;
		}

		private void TextBlock_GotFocus(object sender, RoutedEventArgs e)
		{
		}

		private void Grid_Loaded(object sender, RoutedEventArgs e)
		{
			var binding = new Binding("SelectedEntry");
			binding.Mode = BindingMode.OneWayToSource;
			SetBinding(SelectedEntry, binding);
		}

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			SetValue(SelectedEntry, e.NewValue);
		}
	}
}