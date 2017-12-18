using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ConscriptDesigner.Content {
	public static class ContentExtensions {
		public static ContentFile File(this TreeViewItem treeViewItem) {
			return treeViewItem.Tag as ContentFile;
		}
	}
}
