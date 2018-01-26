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
using ConscriptDesigner.Util;

namespace ConscriptDesigner.Content {
	/// <summary>The base file for all content project files.</summary>
	public class ContentFile : IComparable {

		/// <summary>The parent of the content file.</summary>
		private ContentFolder parent;
		/// <summary>The name of the content file.</summary>
		private string name;
		/// <summary>True if the content file is cut.</summary>
		private bool cut;
		/// <summary>Overrides if the file is modified.</summary>
		private bool modifiedOverride;
		/// <summary>The time the content file was last modified at in UTC.</summary>
		private DateTime lastModified;

		/// <summary>The XML info of the content file.</summary>
		private ContentXmlInfo xmlInfo;
		/// <summary>The tree view item of the content file.</summary>
		private ImageTreeViewItem treeViewItem;
		/// <summary>The document of the content file.</summary>
		private ContentFileDocument document;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Conscructs the content file.</summary>
		public ContentFile(string name) {
			this.name = name;
			this.parent = null;
			this.xmlInfo = new ContentXmlInfo();
			TreeViewItem = new ImageTreeViewItem(DesignerImages.File, name, false);
			XmlInfo.ElementName = "None";
			this.document = null;
			this.cut = false;
			this.modifiedOverride = false;
			this.lastModified = new DateTime();
		}


		//-----------------------------------------------------------------------------
		// IComparable Overloads
		//-----------------------------------------------------------------------------

		/// <summary>Compares the content files to one another.</summary>
		int IComparable.CompareTo(object obj) {
			return CompareTo(obj as ContentFile);
		}

		/// <summary>Compares the content files to one another.</summary>
		public virtual int CompareTo(ContentFile file) {
			if (file.IsFolder) {
				return AlphanumComparator.Compare(Path, file.Path, true) + 10000;
			}
			return AlphanumComparator.Compare(Path, file.Path, true);
		}


		//-----------------------------------------------------------------------------
		// File Modified
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the file has been modified outside of the designer.</summary>
		public bool IsFileOutdated() {
			bool outdated = false;
			if (!IsFolder) {
				if (File.Exists(FilePath))
					outdated = File.GetLastWriteTimeUtc(FilePath) != lastModified;
			}
			else if (IsRoot) {
				if (File.Exists(((ContentRoot) this).ProjectFile))
					outdated = File.GetLastWriteTimeUtc(((ContentRoot) this).ProjectFile) != lastModified;
			}
			if (outdated)
				OnFileOutdated();
			return outdated;
		}

		/// <summary>Updates the last modified time for the file.</summary>
		public void UpdateLastModified() {
			if (!IsFolder) {
				lastModified = File.GetLastWriteTimeUtc(FilePath);
			}
			else if (IsRoot) {
				lastModified = File.GetLastWriteTimeUtc(((ContentRoot) this).ProjectFile);
			}
		}


		//-----------------------------------------------------------------------------
		// Building
		//-----------------------------------------------------------------------------

		/// <summary>Compiles the content file as an xnb asset.</summary>
		public virtual bool Compile() {
			return false;
		}


		//-----------------------------------------------------------------------------
		// ContentRoot-Exclusive
		//-----------------------------------------------------------------------------
		
		/// <summary>Updates the files' XML info for saving.
		/// This should only be called by ContentRoot.</summary>
		internal void UpdateXmlInfo() {
			XmlInfo.Include = Path.Replace('/', '\\');
			if (!IsFolder)
				xmlInfo.Name = IOPath.GetFileNameWithoutExtension(name);
			else
				XmlInfo.Include += "\\";
		}


		//-----------------------------------------------------------------------------
		// Actions
		//-----------------------------------------------------------------------------

		/// <summary>Opens the content file.</summary>
		public bool Open(bool silentFail, bool activate = true) {
			if (IsOpen) {
				// Focus on the already-existing anchorable
				if (activate)
					Document.IsActive = true;
				return true;
			}
			else {
				try {
					UpdateLastModified();
					OnOpen();
					if (IsOpen && activate)
						document.IsActive = true;
					return true;
				}
				catch (Exception ex) {
					if (!silentFail)
						DesignerControl.ShowExceptionMessage(ex, "open", name);
					if (IsOpen)
						document.ForceClose();
					return false;
				}
			}
		}

		/// <summary>Closes the content file.</summary>
		public void Close(bool forceClose = false) {
			if (IsOpen) {
				if (forceClose)
					document.ForceClose();
				else
					document.Close();
				// OnClose() will be called by anchorable event
			}
		}

		/// <summary>Reloads the content file.</summary>
		public bool Reload(bool silentFail) {
			try {
				if (IsOpen) {
					OnReload();
					//if (IsOpen)
					//	document.IsActive = true;
					UpdateLastModified();
					modifiedOverride = false;
					return true;
				}
				else {
					return Open(silentFail);
				}
			}
			catch (Exception ex) {
				if (!silentFail)
					DesignerControl.ShowExceptionMessage(ex, "reload", name);
				if (IsOpen)
					document.ForceClose();
				return false;
			}
		}

		/// <summary>Saves the content file.</summary>
		public bool Save(bool silentFail) {
			try {
				OnSave();
				UpdateLastModified();
				modifiedOverride = false;
				return true;
			}
			catch (Exception ex) {
				if (!silentFail)
					DesignerControl.ShowExceptionMessage(ex, "save", name);
				if (IsOpen)
					document.ForceClose();
				return false;
			}
		}
		
		/// <summary>Undoes the last action.</summary>
		public void Undo() {
			if (IsOpen) {
				OnUndo();
			}
		}

		/// <summary>Redoes the last action.</summary>
		public void Redo() {
			if (IsOpen) {
				OnRedo();
			}
		}


		//-----------------------------------------------------------------------------
		// Virtual Events
		//-----------------------------------------------------------------------------

		/// <summary>Called when the file is opened.</summary>
		protected virtual void OnOpen() { }

		/// <summary>Called when the file is closed.</summary>
		protected virtual void OnClose() { }

		/// <summary>Called when the file is reloaded.</summary>
		protected virtual void OnReload() { }

		/// <summary>Called when the file is saved.</summary>
		protected virtual void OnSave() { }

		/// <summary>Called when the file is deleted.</summary>
		protected virtual void OnDelete() { }

		/// <summary>Called when the file is moved.</summary>
		protected virtual void OnMove() { }

		/// <summary>Called when the file is renamed.</summary>
		protected virtual void OnRename() {
			if (IsOpen) {
				Document.Title = name;
			}
		}

		/// <summary>Called during undo.</summary>
		protected virtual void OnUndo() { }

		/// <summary>Called during redo.</summary>
		protected virtual void OnRedo() { }

		/// <summary>Called when the modified override is changed.</summary>
		protected virtual void OnModifiedChanged() { }
		
		/// <summary>Called when the file becomes outdated.</summary>
		protected virtual void OnFileOutdated() { }


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Called when the anchorable is closed.</summary>
		private void OnAnchorableClosed(object sender, EventArgs e) {
			document = null;
			OnClose();
		}

		/// <summary>Called when the context menu is opening to update if paste is enabled.</summary>
		private void OnContextMenuOpening(object sender, ContextMenuEventArgs e) {
			foreach (object item in treeViewItem.ContextMenu.Items) {
				if (item is MenuItem) {
					if (((MenuItem) item).Tag as string == "Paste") {
						((MenuItem) item).IsEnabled = Root.CanPaste;
					}
				}
			}
		}

		/// <summary>Called when the tree view item is double clicked.</summary>
		private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
			// Double click event is glitchy. Make sure we're not opening the file more than once.
			if (e.Source is TreeViewItem && ((TreeViewItem) e.Source).IsSelected) {
				Open(false);
			}
		}


		//-----------------------------------------------------------------------------
		// Virtual Context Menu
		//-----------------------------------------------------------------------------

		/// <summary>Creates the context menu for the tree view item.</summary>
		protected virtual void CreateContextMenu(ContextMenu menu) {
			AddExcludeContextMenuItem(menu);
			AddSeparatorContextMenuItem(menu);
			AddClipboardContextMenuItems(menu, false);
			AddRenameContextMenuItem(menu);
		}


		//-----------------------------------------------------------------------------
		// Context Menu Creators
		//-----------------------------------------------------------------------------

		/// <summary>Adds a separator to the context menu.</summary>
		protected void AddSeparatorContextMenuItem(ContextMenu menu) {
			menu.Items.Add(new Separator());
		}

		/// <summary>Adds an add menu to the context menu.</summary>
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

		/// <summary>Adds an open file item to the context menu.</summary>
		protected void AddOpenContextMenuItem(ContextMenu menu) {
			ImageMenuItem item = new ImageMenuItem(DesignerImages.Open, "Open");
			item.Click += delegate { Open(false); };
			menu.Items.Add(item);
		}

		/// <summary>Adds an exclude file item to the context menu.</summary>
		protected void AddExcludeContextMenuItem(ContextMenu menu) {
			ImageMenuItem item = new ImageMenuItem(DesignerImages.ContentProjectRemove, "Exclude From Project");
			item.Click += delegate { Root.Exclude(Path); };
			menu.Items.Add(item);
		}

		/// <summary>Adds clipboard items to the context menu.</summary>
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

		/// <summary>Adds a paste item to the context menu.</summary>
		protected void AddPasteContextMenuItem(ContextMenu menu) {
			ImageMenuItem item = new ImageMenuItem(DesignerImages.Paste, "Paste");
			item.Tag = "Paste";
			item.Click += delegate { Root.RequestPaste(Path); };
			menu.Items.Add(item);
		}

		/// <summary>Adds a rename item to the context menu.</summary>
		protected void AddRenameContextMenuItem(ContextMenu menu) {
			ImageMenuItem item = new ImageMenuItem(DesignerImages.Rename, "Rename");
			item.Click += delegate { Root.RequestRename(Path); };
			menu.Items.Add(item);
		}
		

		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of the content file.</summary>
		public virtual ContentTypes ContentType {
			get { return ContentTypes.Unknown; }
		}

		/// <summary>Gets if the content file should be compiled.</summary>
		public virtual bool ShouldCompile {
			get { return false; }
		}

		/// <summary>Gets if the content file should be copied to the content folder as is.</summary>
		public virtual bool ShouldCopyToOutput {
			get { return (XmlInfo.CopyToOutputDirectory == "PreserveNewest" || XmlInfo.CopyToOutputDirectory == "Always"); }
		}

		/// <summary>Gets if the file is modified.</summary>
		protected virtual bool IsModifiedInternal {
			get { return false; }
		}

		/// <summary>Gets if the content file can undo any actions.</summary>
		public virtual bool CanUndo {
			get { return false; }
		}

		/// <summary>Gets if the content file can redo any actions.</summary>
		public virtual bool CanRedo {
			get { return false; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the filename of the content file.</summary>
		public string Name {
			get { return name; }
			set {
				name = value;
				treeViewItem.Header = value;
				OnRename();
			}
		}

		/// <summary>Gets the project-local path of the content file.</summary>
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

		/// <summary>Gets the project-local directory of the content file.</summary>
		public string Directory {
			get {
				if (parent != null)
					return parent.Path;
				else if (IsRoot)
					return "";
				throw new Exception("Content file is not in the file system!");
			}
		}

		/// <summary>Gets the full file path of the content file.</summary>
		public string FilePath {
			get {
				if (parent != null)
					return IOPath.Combine(parent.FilePath, name).Replace('\\', '/');
				else if (IsRoot)
					return ((ContentRoot) this).ProjectDirectory;
				return name;
			}
		}

		/// <summary>Gets the full directory path of the content file.</summary>
		public string FileDirectory {
			get {
				if (parent != null)
					return parent.FilePath;
				else if (IsRoot)
					return IOPath.GetDirectoryName(((ContentRoot) this).ProjectDirectory);
				throw new Exception("Content file is not in the file system!");
			}
		}

		/// <summary>Gets the output filename of the content file.
		/// Used for when copied to the designer's content folder.</summary>
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

		/// <summary>Gets the output file path of the content file.
		/// Used for when copied to the designer's content folder.</summary>
		public string OutputFilePath {
			get {
				if (parent != null) {
					string outptutFileName = OutputFileName;
					if (outptutFileName != null)
						return IOPath.Combine(DesignerControl.DesignerContentDirectory, parent.Path, outptutFileName);
				}
				else if (IsRoot) {
					return DesignerControl.DesignerContentDirectory;
				}
				return null;
			}
		}

		/// <summary>Gets or sets the tree view item for the content file.
		/// This should only be set in the constructor.</summary>
		public ImageTreeViewItem TreeViewItem {
			get { return treeViewItem; }
			protected set {
				treeViewItem = value;
				treeViewItem.Tag = this;
				treeViewItem.Items.SortDescriptions.Clear();
				treeViewItem.Items.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Ascending));
				treeViewItem.MouseDoubleClick += OnMouseDoubleClick;
				treeViewItem.ContextMenu = new ContextMenu();
				CreateContextMenu(treeViewItem.ContextMenu);
				treeViewItem.ContextMenuOpening += OnContextMenuOpening;
			}
		}

		/// <summary>Gets or sets the anchorable document of the content file.</summary>
		public ContentFileDocument Document {
			get { return document; }
			set {
				if (document != null) {
					document.Closed -= OnAnchorableClosed;
					document.ForceClose();
				}
				document = value;
				if (document != null) {
					document.Closed += OnAnchorableClosed;
					DesignerControl.DockDocument(document);
				}
			}
		}

		/// <summary>Gets or sets the XML info of the content file.
		/// This should only be set by ContentRoot.</summary>
		internal ContentXmlInfo XmlInfo {
			get { return xmlInfo; }
			set { xmlInfo = value; }
		}

		/// <summary>Gets if the file is open.</summary>
		public bool IsOpen {
			get { return document != null; }
		}

		/// <summary>Gets if this file is a folder.</summary>
		public bool IsFolder {
			get { return this is ContentFolder; }
		}

		/// <summary>Gets if this file is the root project folder.</summary>
		public bool IsRoot {
			get { return this is ContentRoot; }
		}

		/// <summary>Gets or sets the parent of the content file.
		/// The setter should only be called by ContentRoot.</summary>
		public ContentFolder Parent {
			get { return parent; }
			internal set {
				bool moved = (parent != null && value != null) ;
				parent = value;
				if (moved)
					OnMove();
			}
		}

		/// <summary>Gets the root container of the content file.</summary>
		public ContentRoot Root {
			get {
				if (parent != null)
					return parent.Root;
				if (IsRoot)
					return (ContentRoot) this;
				return null;
			}
		}

		/// <summary>Gets or sets if the file has been cut in the project explorer.
		/// The setter should only be called by ContentRoot.</summary>
		public bool IsCut {
			get { return cut; }
			internal set {
				cut = value;
				treeViewItem.ImageOpacity = (cut ? 0.5 : 1.0);
			}
		}

		/// <summary>Gets or sets when the file was last modified.</summary>
		/*public DateTime LastModified {
			get { return lastModified; }
			internal set { lastModified = value; }
		}*/

		/// <summary>Gets or sets and overrides if the file is modified.</summary>
		public bool IsModifiedOverride {
			get { return modifiedOverride; }
			set {
				if (value != modifiedOverride) {
					modifiedOverride = value;
					OnModifiedChanged();
				}
			}
		}

		/// <summary>Overrides if the file is modified.</summary>
		public bool IsModified {
			get { return modifiedOverride || IsModifiedInternal; }
		}
	}
}
