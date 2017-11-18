using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ZeldaEditor.Control;
using ZeldaEditor.Controls;
using ZeldaEditor.Util;

namespace ZeldaEditor.Controls {
	public class FolderTreeViewItem : ImageTreeViewItem {

		public FolderTreeViewItem() : this(null, false) {

		}
		public FolderTreeViewItem(string name, bool expanded) {
			Header = name;
			IsExpanded = expanded;
			UpdateImage();
		}

		private void UpdateImage() {
			Source = (IsExpanded ? EditorImages.FolderBlueOpen : EditorImages.FolderBlueClosed);
		}

		protected override void OnExpanded(RoutedEventArgs e) {
			base.OnExpanded(e);
			UpdateImage();
		}
		protected override void OnCollapsed(RoutedEventArgs e) {
			base.OnCollapsed(e);
			UpdateImage();
		}
	}
}
