using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ConscriptDesigner.Control;

namespace ConscriptDesigner.Controls {
	public class FolderTreeViewItem : ImageTreeViewItem {

		public FolderTreeViewItem() : this(null, false) {

		}
		public FolderTreeViewItem(string name, bool expanded) {
			Header = name;
			IsExpanded = expanded;
			UpdateImage();
		} 

		private void UpdateImage() {
			Source = (IsExpanded ? DesignerImages.FolderOpen : DesignerImages.FolderClosed);
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
