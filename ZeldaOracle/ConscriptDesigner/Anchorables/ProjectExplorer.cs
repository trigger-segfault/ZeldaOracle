using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using ConscriptDesigner.Content;
using ConscriptDesigner.Control;
using ConscriptDesigner.Controls;

namespace ConscriptDesigner.Anchorables {
	/// <summary>Extensions for use with the project explorer.</summary>
	public static class ProjectExplorerExtensions {
		/// <summary>Gets the content file from the tree view item's tag.</summary>
		public static ContentFile File(this ImageTreeViewItem treeViewItem) {
			return treeViewItem.Tag as ContentFile;
		}
	}

	public class ProjectExplorer : RequestCloseAnchorable, ICommandAnchorable {
		
		/// <summary>The tree view to display the content project files.</summary>
		private DraggableImageTreeView treeView;
		/// <summary>The content project loaded in the explorer.</summary>
		private ContentRoot project;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the project explorer.</summary>
		public ProjectExplorer() {
			Border border = CreateBorder();
			this.treeView = new DraggableImageTreeView();
			this.treeView.CanDragItem += OnCanDragItem;
			this.treeView.CanDropItem += OnCanDropItem;
			this.treeView.DropItem += OnDropItem;
			this.treeView.BorderThickness = new Thickness(0);
			border.Child = this.treeView;

			Closed += OnAnchorableClosed;

			Title = "Project Explorer";
			Content = border;
		}

		/// <summary>Clears the items from the project explorer tree view.</summary>
		public void Clear() {
			// Make sure the Project's tree view item no longer has a parent
			treeView.Items.Clear();
		}


		//-----------------------------------------------------------------------------
		// XML Serialization
		//-----------------------------------------------------------------------------

		public override void ReadXml(XmlReader reader) {
			DesignerControl.MainWindow.ProjectExplorer = this;
			Project = DesignerControl.Project;
			if (reader.MoveToAttribute("ExpandedFolders")) {
				string expandedFolders = reader.Value;
				string[] folders = expandedFolders.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string folder in folders) {
					ContentFolder file = project.GetFolder(folder);
					if (file != null)
						file.TreeViewItem.IsExpanded = true;
				}
			}
			base.ReadXml(reader);
		}
		
		public override void WriteXml(XmlWriter writer) {
			base.WriteXml(writer);
			string expandedFolders = "";
			foreach (ContentFile file in project.GetAllFiles()) {
				if (file.IsFolder && file.TreeViewItem.IsExpanded)
					expandedFolders += file.Path + "|";
			}
			writer.WriteAttributeString("ExpandedFolders", expandedFolders);
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Called to force cleanup during close.</summary>
		private void OnAnchorableClosed(object sender, EventArgs e) {
			Clear();
		}

		/// <summary>Called to confirm if the item can be dragged.</summary>
		private void OnCanDragItem(object sender, TreeViewCanDragEventArgs e) {
			e.CanDrag = !e.Item.File().IsRoot;
		}

		/// <summary>Called to confirm if the item can be dropped.</summary>
		private void OnCanDropItem(object sender, TreeViewCanDropEventArgs e) {
			e.CanDrop = true;
		}

		/// <summary>Called to drop the item.</summary>
		private void OnDropItem(object sender, TreeViewDropEventArgs e) {
			ContentFile target = e.Target.File();
			if (!target.IsFolder)
				target = target.Parent;

			if (e.IsFileDrop) {
				List<string> files = new List<string>();
				Project.RequestDrop(e.Files, target.Path);
			}
			else {
				ContentFile source = e.Item.File();
				Project.RequestMove(source.Path, target.Path);
				e.Item.IsSelected = true;
			}
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Sets the contained project of the explorer.</summary>
		public ContentRoot Project {
			get { return project; }
			set {
				treeView.Items.Clear();
				if (value != null) {
					treeView.Items.Add(value.TreeViewItem);
				}
				project = value;
			}
		}


		//-----------------------------------------------------------------------------
		// Command Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Cuts the selected file.</summary>
		public void Cut() {
			FrameworkElement selectedItem = treeView.SelectedItem as FrameworkElement;
			if (selectedItem != null) {
				ContentFile file = selectedItem.Tag as ContentFile;
				if (file != null)
					project.Cut(file.Path);
			}
		}

		/// <summary>Copies the selected file.</summary>
		public void Copy() {
			FrameworkElement selectedItem = treeView.SelectedItem as FrameworkElement;
			if (selectedItem != null) {
				ContentFile file = selectedItem.Tag as ContentFile;
				if (file != null)
					project.Copy(file.Path);
			}
		}

		/// <summary>Pastes from the clipboard into the selected directory.</summary>
		public void Paste() {
			FrameworkElement selectedItem = treeView.SelectedItem as FrameworkElement;
			if (selectedItem != null) {
				ContentFile file = selectedItem.Tag as ContentFile;
				if (file != null) {
					if (!file.IsFolder)
						file = file.Parent;
					project.RequestPaste(file.Path);
				}
			}
		}

		/// <summary>Performs the delete command.</summary>
		public void Delete() {
			FrameworkElement selectedItem = treeView.SelectedItem as FrameworkElement;
			if (selectedItem != null) {
				ContentFile file = selectedItem.Tag as ContentFile;
				if (file != null && !file.IsRoot) {
					project.RequestDelete(file.Path);
				}
			}
		}

		/// <summary>Performs the undo command.</summary>
		public void Undo() {

		}

		/// <summary>Performs the redo command.</summary>
		public void Redo() {

		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the project explorer can cut.</summary>
		public bool CanCut {
			// Handled by text editor
			get {
				FrameworkElement selectedItem = treeView.SelectedItem as FrameworkElement;
				if (selectedItem == null) return false;
				ContentFile file = selectedItem.Tag as ContentFile;
				return (file != null ? !file.IsRoot : false);
			}
		}

		/// <summary>Returns true if the project explorer can copy.</summary>
		public bool CanCopy {
			// Handled by text editor
			get {
				FrameworkElement selectedItem = treeView.SelectedItem as FrameworkElement;
				if (selectedItem == null) return false;
				ContentFile file = selectedItem.Tag as ContentFile;
				return (file != null ? !file.IsRoot : false);
			}
		}

		/// <summary>Returns true if the project explorer can paste.</summary>
		public bool CanPaste {
			// Handled by text editor
			get {
				FrameworkElement selectedItem = treeView.SelectedItem as FrameworkElement;
				if (selectedItem == null) return false;
				ContentFile file = selectedItem.Tag as ContentFile;
				return (file != null && project.CanPaste);
			}
		}

		/// <summary>Returns true if the project explorer can delete.</summary>
		public bool CanDelete {
			// Handled by text editor
			get {
				FrameworkElement selectedItem = treeView.SelectedItem as FrameworkElement;
				if (selectedItem == null) return false;
				ContentFile file = selectedItem.Tag as ContentFile;
				return (file != null ? !file.IsRoot : false);
			}
		}

		/// <summary>Returns true if the project explorer can undo.</summary>
		public bool CanUndo {
			get { return false; }
		}

		/// <summary>Returns true if the project explorer can redo.</summary>
		public bool CanRedo {
			get { return false; }
		}
	}
}
