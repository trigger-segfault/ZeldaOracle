using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZeldaEditor.Controls {
	/// <summary>A listview item for use with the history window.</summary>
	public class HistoryListViewItem : ListViewItem {

		//-----------------------------------------------------------------------------
		// Dependency Properties
		//-----------------------------------------------------------------------------

		/// <summary>The dependency property for the action icon.</summary>
		public static readonly DependencyProperty ActionIconProperty =
			DependencyProperty.RegisterAttached(
			"ActionIcon", typeof(ImageSource), typeof(HistoryListViewItem));

		/// <summary>Gets or sets the action icon that represents this history item.</summary>
		public ImageSource ActionIcon {
			get { return (ImageSource)GetValue(ActionIconProperty); }
			set { SetValue(ActionIconProperty, value); }
		}

		/// <summary>The dependency property for the action name.</summary>
		public static readonly DependencyProperty ActionNameProperty =
			DependencyProperty.RegisterAttached(
			"ActionName", typeof(string), typeof(HistoryListViewItem),
				new FrameworkPropertyMetadata(""));

		/// <summary>Gets or sets the action name that represents this history item.</summary>
		public string ActionName {
			get { return (string)GetValue(ActionNameProperty); }
			set { SetValue(ActionNameProperty, value); }
		}

		/// <summary>The dependency property for if the action has been undone.</summary>
		public static readonly DependencyProperty IsUndoneProperty =
			DependencyProperty.RegisterAttached(
			"IsUndone", typeof(bool), typeof(HistoryListViewItem),
				new FrameworkPropertyMetadata(false));

		/// <summary>Gets or sets the action has been undone (visual only).</summary>
		public bool IsUndone {
			get { return (bool)GetValue(IsUndoneProperty); }
			set { SetValue(IsUndoneProperty, value); }
		}


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the history list view item default style.</summary>
		static HistoryListViewItem() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(HistoryListViewItem),
					   new FrameworkPropertyMetadata(typeof(HistoryListViewItem)));
		}
	}
}
