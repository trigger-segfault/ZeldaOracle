using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZeldaEditor.Controls {
	public class ImageTreeViewItem : TreeViewItem {

		private static ContextMenu NullContextMenu;

		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.RegisterAttached(
			"Source", typeof(ImageSource), typeof(ImageTreeViewItem));

		public ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
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
