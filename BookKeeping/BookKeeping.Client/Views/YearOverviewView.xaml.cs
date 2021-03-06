﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BookKeeping.Client.Views
{
	/// <summary>
	/// Interaction logic for YearOverviewView.xaml
	/// </summary>
	public partial class YearOverviewView : UserControl
	{
		public static readonly DependencyProperty SelectedColumn =
			DependencyProperty.Register("SelectedColumn", typeof(int),
			typeof(YearOverviewView), new PropertyMetadata(default(int)));

		public YearOverviewView()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			var binding = new Binding("SelectedColumn");
			binding.Mode = BindingMode.OneWayToSource;
			SetBinding(SelectedColumn, binding);
		}

		private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
		{
			if (EntryPeriodsGrid.CurrentCell.Column != null)
			{
				SetValue(SelectedColumn, EntryPeriodsGrid.CurrentCell.Column.DisplayIndex);
			}
		}
	}
}