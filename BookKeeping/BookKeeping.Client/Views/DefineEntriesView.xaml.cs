using BookKeeping.Client.ViewModels;
using PL.BookKeeping.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookKeeping.Client.Views
{
    /// <summary>Interaction logic for DefineEntitiesView.xaml
    /// </summary>
    public partial class DefineEntriesView : UserControl
    {
        public static readonly DependencyProperty SelectedEntry =
            DependencyProperty.Register("SelectedEntry", typeof(Entry),
            typeof(DefineEntriesView), new PropertyMetadata(default(Entry)));

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
            Binding binding = new Binding("SelectedEntry");
            binding.Mode = BindingMode.OneWayToSource;
            SetBinding(SelectedEntry, binding);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetValue(SelectedEntry, e.NewValue);            
        }
    }
}
