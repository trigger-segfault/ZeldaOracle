using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZeldaEditor.Controls {
	public class HistoryListViewItem : ListViewItem {

		public static readonly DependencyProperty ActionIconProperty =
			DependencyProperty.RegisterAttached(
			"ActionIcon", typeof(ImageSource), typeof(HistoryListViewItem));

		public ImageSource ActionIcon {
			get { return (ImageSource)GetValue(ActionIconProperty); }
			set { SetValue(ActionIconProperty, value); }
		}


		public static readonly DependencyProperty ActionNameProperty =
			DependencyProperty.RegisterAttached(
			"ActionName", typeof(string), typeof(HistoryListViewItem),
				new FrameworkPropertyMetadata(""));

		public string ActionName {
			get { return (string)GetValue(ActionNameProperty); }
			set { SetValue(ActionNameProperty, value); }
		}


		public static readonly DependencyProperty IsUndoneProperty =
			DependencyProperty.RegisterAttached(
			"IsUndone", typeof(bool), typeof(HistoryListViewItem),
				new FrameworkPropertyMetadata(false));

		public bool IsUndone {
			get { return (bool)GetValue(IsUndoneProperty); }
			set { SetValue(IsUndoneProperty, value); }
		}

		static HistoryListViewItem() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(HistoryListViewItem),
					   new FrameworkPropertyMetadata(typeof(HistoryListViewItem)));
		}
	}
}
