using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ConscriptDesigner.Controls {
	public class ImageTreeViewItem : TreeViewItem {

		private static ContextMenu NullContextMenu;

		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.RegisterAttached(
			"Source", typeof(ImageSource), typeof(ImageTreeViewItem));

		public ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		public static readonly DependencyProperty ImageOpacityProperty =
			DependencyProperty.RegisterAttached(
			"ImageOpacity", typeof(double), typeof(ImageTreeViewItem),
				new FrameworkPropertyMetadata(1.0));

		public double ImageOpacity {
			get { return (double) GetValue(ImageOpacityProperty); }
			set { SetValue(ImageOpacityProperty, value); }
		}

		public static readonly DependencyProperty IsHighlightedProperty =
			DependencyProperty.RegisterAttached(
			"IsHighlighted", typeof(bool), typeof(ImageTreeViewItem),
				new FrameworkPropertyMetadata(false));

		public bool IsHighlighted {
			get { return (bool) GetValue(IsHighlightedProperty); }
			set { SetValue(IsHighlightedProperty, value); }
		}

		static ImageTreeViewItem() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageTreeViewItem),
					   new FrameworkPropertyMetadata(typeof(ImageTreeViewItem)));

			NullContextMenu = new ContextMenu();
			NullContextMenu.Visibility = Visibility.Hidden;
		}

		public ImageTreeViewItem() : this(null, null, false) {

		}
		public ImageTreeViewItem(ImageSource source, string name, bool expanded) {
			Source = source;
			Header = name;
			IsExpanded = expanded;
			ContextMenu = NullContextMenu;
		}
	}
}
