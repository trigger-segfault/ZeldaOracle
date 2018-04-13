using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ZeldaWpf.Resources;

namespace ZeldaWpf.Controls {
	/// <summary>An image tree view item that displays a folder.</summary>
	public class FolderTreeViewItem : ImageTreeViewItem {
		/// <summary>Constructs the base folder tree view item.</summary>
		public FolderTreeViewItem(FolderColor color, Orientation orientation) {
			SourceExpanded = WpfImages.GetFolder(color, orientation, true);
			SourceCollapsed = WpfImages.GetFolder(color, orientation, false);
			UpdateExpandedSource();
		}

		/// <summary>Constructs the base folder tree view item.</summary>
		public FolderTreeViewItem(FolderColor color, Orientation orientation,
			object header, bool expanded) : base(null, header, expanded)
		{
			SourceExpanded = WpfImages.GetFolder(color, orientation, true);
			SourceCollapsed = WpfImages.GetFolder(color, orientation, false);
			UpdateExpandedSource();
		}
	}

	/// <summary>An image tree view item that displays a blue horizontal folder.</summary>
	public class FolderBlueHTreeViewItem : FolderTreeViewItem {
		/// <summary>Constructs an empty folder.</summary>
		public FolderBlueHTreeViewItem()
			: base(FolderColor.Blue, Orientation.Horizontal)
		{
		}

		/// <summary>Constructs a folder with the name and expanded state.</summary>
		public FolderBlueHTreeViewItem(object header, bool expanded)
			: base(FolderColor.Blue, Orientation.Horizontal, header, expanded)
		{
		}
	}

	/// <summary>An image tree view item that displays a blue vertical folder.</summary>
	public class FolderBlueVTreeViewItem : FolderTreeViewItem {
		/// <summary>Constructs an empty folder.</summary>
		public FolderBlueVTreeViewItem()
			: base(FolderColor.Blue, Orientation.Vertical)
		{
		}

		/// <summary>Constructs a folder with the name and expanded state.</summary>
		public FolderBlueVTreeViewItem(object header, bool expanded)
			: base(FolderColor.Blue, Orientation.Vertical, header, expanded)
		{
		}
	}

	/// <summary>An image tree view item that displays a manilla horizontal folder.</summary>
	public class FolderManillaHTreeViewItem : FolderTreeViewItem {
		/// <summary>Constructs an empty folder.</summary>
		public FolderManillaHTreeViewItem()
			: base(FolderColor.Manilla, Orientation.Horizontal)
		{
		}

		/// <summary>Constructs a folder with the name and expanded state.</summary>
		public FolderManillaHTreeViewItem(object header, bool expanded)
			: base(FolderColor.Manilla, Orientation.Horizontal, header, expanded)
		{
		}
	}

	/// <summary>An image tree view item that displays a manilla vertical folder.</summary>
	public class FolderManillaVTreeViewItem : FolderTreeViewItem {
		/// <summary>Constructs an empty folder.</summary>
		public FolderManillaVTreeViewItem()
			: base(FolderColor.Manilla, Orientation.Vertical)
		{
		}

		/// <summary>Constructs a folder with the name and expanded state.</summary>
		public FolderManillaVTreeViewItem(object header, bool expanded)
			: base(FolderColor.Manilla, Orientation.Vertical, header, expanded)
		{
		}
	}
}
