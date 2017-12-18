using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ConscriptDesigner.Controls;
using ConscriptDesigner.Control;
using IOPath = System.IO.Path;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;
using ConscriptDesigner.Anchorables;

namespace ConscriptDesigner.Content {
	public class ContentFile : IComparable {

		private ContentFolder parent;
		private string name;
		private ContentXmlInfo xmlInfo;
		private ImageTreeViewItem treeViewItem;
		private RequestCloseAnchorable anchorable;

		private DateTime lastModified;

		private bool cut;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ContentFile(string name) {
			this.name = name;
			this.parent = null;
			this.xmlInfo = new ContentXmlInfo();
			TreeViewItem = new ImageTreeViewItem(DesignerImages.File, name, false);
			XmlInfo.ElementName = "None";
			this.anchorable = null;
			this.cut = false;
		}


		//-----------------------------------------------------------------------------
		// IComparable Overloads
		//-----------------------------------------------------------------------------

		int IComparable.CompareTo(object obj) {
			return CompareTo(obj as ContentFile);
		}
		public virtual int CompareTo(ContentFile file) {
			if (file.IsFolder) {
				return string.Compare(Path, file.Path) + 10000;
			}
			return string.Compare(Path, file.Path);
		}

		public virtual bool Compile() {
			return false;
		}

		public void Resort() {
			treeViewItem.Items.SortDescriptions.Clear();
			treeViewItem.Items.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Ascending));
			treeViewItem.Items.Refresh();
		}

		private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (e.Source is TreeViewItem && ((TreeViewItem) e.Source).IsSelected) {
				Open();
			}
		}

		private static object GetParent(DependencyObject obj, Type expectedType) {
			var parent = VisualTreeHelper.GetParent(obj);
			while (parent != null && parent.GetType() != expectedType)
				parent = VisualTreeHelper.GetParent(parent);

			return parent;
		}

		public void UpdateXmlInfo() {
			XmlInfo.Include = Path.Replace('/', '\\');
			if (!IsFolder)
				xmlInfo.Name = IOPath.GetFileNameWithoutExtension(name);
			else
				XmlInfo.Include += "\\";
		}

		public void Open() {
			if (IsOpen) {
				// Focus on the already-existing anchorable
				Anchorable.IsActive = true;
			}
			else {
				try {
					OnOpen();
					if (IsOpen)
						Anchorable.IsActive = true;
				}
				catch (Exception ex) {
					DesignerControl.ShowExceptionMessage(ex, "open", name);
					if (Anchorable != null)
						Anchorable.ForceClose();
				}
			}
		}

		public void Close(bool forceClose = false) {
			if (IsOpen) {
				if (forceClose)
					anchorable.ForceClose();
				else
					anchorable.Close();
				// OnClose() will be called by anchorable event
			}
		}

		//-----------------------------------------------------------------------------
		// Virtual Events
		//-----------------------------------------------------------------------------

		protected virtual void OnOpen() {

		}

		protected virtual void OnClose() {

		}

		protected virtual void OnDelete() {

		}

		protected virtual void OnMove() {

		}

		protected virtual void OnRename() {
			if (IsOpen) {
				Anchorable.Title = name;
			}
		}

		private void OnAnchorableClosed(object sender, EventArgs e) {
			anchorable = null;
			OnClose();
		}

		private void OnContextMenuOpening(object sender, ContextMenuEventArgs e) {
			foreach (object item in treeViewItem.ContextMenu.Items) {
				if (item is MenuItem) {
					if (((MenuItem) item).Tag as string == "Paste") {
						((MenuItem) item).IsEnabled = Root.CanPaste;
					}
				}
			}
		}
		
		//-----------------------------------------------------------------------------
		// Virtual Context Menu
		//-----------------------------------------------------------------------------

		protected virtual void CreateContextMenu(ContextMenu menu) {
			AddExcludeContextMenuItem(menu);
			AddSeparatorContextMenuItem(menu);
			AddClipboardContextMenuItems(menu, false);
			AddRenameContextMenuItem(menu);
		}


		//-----------------------------------------------------------------------------
		// Context Menu
		//-----------------------------------------------------------------------------

		protected void AddSeparatorContextMenuItem(ContextMenu menu) {
			menu.Items.Add(new Separator());
		}

		protected void AddAddContextMenuItem(ContextMenu menu) {
			ImageMenuItem item = new ImageMenuItem(DesignerImages.Plus, "Add");
			menu.Items.Add(item);

			ImageMenuItem subItem = new ImageMenuItem(DesignerImages.ConscriptFileAdd, "New Conscript...");
			subItem.Click += delegate { Root.NewConscript(Path); };
			item.Items.Add(subItem);
			subItem = new ImageMenuItem(DesignerImages.FileAdd, "Add Existing...");
			subItem.Click += delegate { Root.AddExisting(Path); };
			item.Items.Add(subItem);
			subItem = new ImageMenuItem(DesignerImages.FolderAdd, "New Folder...");
			subItem.Click += delegate { Root.NewFolder(Path); };
			item.Items.Add(subItem);
		}

		protected void AddOpenContextMenuItem(ContextMenu menu) {
			ImageMenuItem item = new ImageMenuItem(DesignerImages.Open, "Open");
			item.Click += delegate { Open(); };
			menu.Items.Add(item);
		}

		protected void AddExcludeContextMenuItem(ContextMenu menu) {
			ImageMenuItem item = new ImageMenuItem(DesignerImages.ContentProjectRemove, "Exclude From Project");
			item.Click += delegate { Root.Exclude(Path); };
			menu.Items.Add(item);
		}

		protected void AddClipboardContextMenuItems(ContextMenu menu, bool includePaste) {
			ImageMenuItem item = new ImageMenuItem(DesignerImages.Cut, "Cut");
			item.Click += delegate { Root.Cut(Path); };
			menu.Items.Add(item);
			item = new ImageMenuItem(DesignerImages.Copy, "Copy");
			item.Click += delegate { Root.Copy(Path); };
			menu.Items.Add(item);
			if (includePaste) {
				AddPasteContextMenuItem(menu);
			}
			item = new ImageMenuItem(DesignerImages.Delete, "Delete");
			item.Click += delegate { Root.RequestDelete(Path); };
			menu.Items.Add(item);
		}
		protected void AddPasteContextMenuItem(ContextMenu menu) {
			ImageMenuItem item = new ImageMenuItem(DesignerImages.Paste, "Paste");
			item.Tag = "Paste";
			item.Click += delegate { Root.RequestPaste(Path); };
			menu.Items.Add(item);
		}
		protected void AddRenameContextMenuItem(ContextMenu menu) {
			ImageMenuItem item = new ImageMenuItem(DesignerImages.Rename, "Rename");
			item.Click += delegate { Root.RequestRename(Path); };
			menu.Items.Add(item);
		}


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		public virtual ContentTypes ContentType {
			get { return ContentTypes.Unknown; }
		}
		
		public virtual bool ShouldCompile {
			get { return false; }
		}

		public virtual bool ShouldCopyToOutput {
			get { return (XmlInfo.CopyToOutputDirectory == "PreserveNewest" || XmlInfo.CopyToOutputDirectory == "Always"); }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string Name {
			get { return name; }
			set {
				name = value;
				treeViewItem.Header = value;
				OnRename();
			}
		}

		public string Path {
			get {
				if (parent != null) {
					if (parent.IsRoot)
						return name;
					return IOPath.Combine(parent.Path, name).Replace('\\', '/');
				}
				else if (IsRoot)
					return "";
				throw new Exception("Content file is not in the file system!");
			}
		}
		public string Directory {
			get {
				if (parent != null)
					return parent.Path;
				else if (IsRoot)
					return "";
				throw new Exception("Content file is not in the file system!");
			}
		}

		public string FilePath {
			get {
				if (parent != null)
					return IOPath.Combine(parent.FilePath, name).Replace('\\', '/');
				else if (IsRoot)
					return ((ContentRoot) this).ProjectDirectory;
				return name;
			}
		}

		public string FileDirectory {
			get {
				if (parent != null)
					return parent.FilePath;
				else if (IsRoot)
					return IOPath.GetDirectoryName(((ContentRoot) this).ProjectDirectory);
				throw new Exception("Content file is not in the file system!");
			}
		}

		public string OutputFileName {
			get {
				if (XmlInfo.ElementName == "Compile")
					return IOPath.ChangeExtension(name, ".xnb");
				else if (XmlInfo.ElementName == "Folder")
					return name;
				else if (ShouldCopyToOutput)
					return name;
				return null;
			}
		}

		public string OutputFilePath {
			get {
				if (parent != null)
					return IOPath.Combine(DesignerControl.DesignerContentDirectory, parent.Path, OutputFileName);
				else if (IsRoot)
					return DesignerControl.DesignerContentDirectory;
				return null;
			}
		}

		public ImageTreeViewItem TreeViewItem {
			get { return treeViewItem; }
			protected set {
				treeViewItem = value;
				treeViewItem.Tag = this;
				treeViewItem.Items.SortDescriptions.Clear();
				treeViewItem.Items.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Ascending));
				treeViewItem.Items.IsLiveSorting = true;
				treeViewItem.MouseDoubleClick += OnMouseDoubleClick;
				//treeViewItem.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
				treeViewItem.ContextMenu = new ContextMenu();
				CreateContextMenu(treeViewItem.ContextMenu);
				treeViewItem.ContextMenuOpening += OnContextMenuOpening;
			}
		}

		public RequestCloseAnchorable Anchorable {
			get { return anchorable; }
			set {
				if (anchorable != null) {
					anchorable.Closed -= OnAnchorableClosed;
				}
				anchorable = value;
				if (anchorable != null) {
					anchorable.Closed += OnAnchorableClosed;
				}
			}
		}

		public ContentXmlInfo XmlInfo {
			get { return xmlInfo; }
			set { xmlInfo = value; }
		}

		public bool IsOpen {
			get { return anchorable != null; }
		}

		public bool IsFolder {
			get { return this is ContentFolder; }
		}

		public bool IsRoot {
			get { return this is ContentRoot; }
		}

		public ContentFolder Parent {
			get { return parent; }
			set {
				parent = value;
				OnMove();
			}
		}

		public ContentRoot Root {
			get {
				if (parent != null)
					return parent.Root;
				if (IsRoot)
					return (ContentRoot) this;
				return null;
			}
		}

		public bool IsCut {
			get { return cut; }
			set {
				cut = value;
				treeViewItem.ImageOpacity = (cut ? 0.5 : 1.0);
			}
		}
	}
}
