using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using ZeldaWpf.Controls;
using ZeldaWpf.Util;

namespace ConscriptDesigner.Content {
	public class ContentFolder : ContentFile, IComparable {

		/// <summary>The collection of files in the folder.</summary>
		private Dictionary<string, ContentFile> files;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Conscructs the content folder.</summary>
		public ContentFolder(string name) :
			base(name)
		{
			this.files = new Dictionary<string, ContentFile>(StringComparer.OrdinalIgnoreCase);
			TreeViewItem = new FolderManillaVTreeViewItem(name, false);
			XmlInfo.ElementName = "Folder";
		}


		//-----------------------------------------------------------------------------
		// IComparable Overloads
		//-----------------------------------------------------------------------------

		/// <summary>Compares the content files to one another.</summary>
		int IComparable.CompareTo(object obj) {
			return CompareTo(obj as ContentFile);
		}

		/// <summary>Compares the content files to one another.</summary>
		public override int CompareTo(ContentFile file) {
			if (file.IsFolder) {
				return AlphanumComparator.Compare(Path, file.Path, true);
			}
			return AlphanumComparator.Compare(Path, file.Path, true) - 10000;
		}
		

		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Enumerates all files within this folder only.</summary>
		public IEnumerable<ContentFile> GetLocalFiles() {
			foreach (var pair in files) {
				yield return pair.Value;
			}
		}

		/// <summary>Enumerates all files within this folder only in order.</summary>
		public IEnumerable<ContentFile> GetLocalOrderedFiles() {
			foreach (object item in TreeViewItem.Items) {
				yield return (ContentFile) ((ImageTreeViewItem) item).Tag;
			}
		}

		/// <summary>Enumerates all files within this and all subfolders.</summary>
		public IEnumerable<ContentFile> GetAllFiles() {
			return GetAllFiles(this);
		}

		/// <summary>Enumerates all files within this and all subfolders in order.</summary>
		public IEnumerable<ContentFile> GetAllOrderedFiles() {
			return GetAllOrderedFiles(this);
		}

		/// <summary>Gets the index of the specified content file.</summary>
		public int IndexOfFile(ContentFile file) {
			return TreeViewItem.Items.IndexOf(file.TreeViewItem);
		}

		/// <summary>Gets the file at the specified index in the ordered list.</summary>
		public ContentFile GetFileAt(int index) {
			return (ContentFile) ((ImageTreeViewItem) TreeViewItem.Items[index]).Tag;
		}

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Updates the sorting of the folder.</summary>
		public void Resort() {
			TreeViewItem.Items.SortDescriptions.Clear();
			TreeViewItem.Items.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Ascending));
			TreeViewItem.Items.Refresh();
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Enumerates all files within this and all subfolders.</summary>
		private static IEnumerable<ContentFile> GetAllFiles(ContentFolder folder) {
			foreach (var pair in folder.Files) {
				ContentFile file = pair.Value;
				yield return file;
				if (file.IsFolder) {
					foreach (ContentFile subFile in GetAllFiles((ContentFolder) file)) {
						yield return subFile;
					}
				}
			}
		}

		/// <summary>Enumerates all files within this and all subfolders in order.</summary>
		private static IEnumerable<ContentFile> GetAllOrderedFiles(ContentFolder folder) {
			foreach (object item in folder.TreeViewItem.Items) {
				ContentFile file = (ContentFile) ((ImageTreeViewItem) item).Tag;
				yield return file;
				if (file.IsFolder) {
					foreach (ContentFile subFile in GetAllOrderedFiles((ContentFolder) file)) {
						yield return subFile;
					}
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Override Context Menu
		//-----------------------------------------------------------------------------

		/// <summary>Creates the context menu for the tree view item.</summary>
		protected override void CreateContextMenu(ContextMenu menu) {
			AddAddContextMenuItem(menu);
			AddSeparatorContextMenuItem(menu);
			AddExcludeContextMenuItem(menu);
			AddSeparatorContextMenuItem(menu);
			AddClipboardContextMenuItems(menu, true);
			AddRenameContextMenuItem(menu);
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the content type of the file.</summary>
		public override ContentTypes ContentType {
			get { return ContentTypes.Folder; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The collection of files in the folder. This should only be accessed by ContentRoot.</summary>
		internal Dictionary<string, ContentFile> Files {
			get { return files; }
		}

		/// <summary>Gets the number of files in this folder only.</summary>
		public int LocalFileCount {
			get { return files.Count; }
		}

		/// <summary>Gets if the folder is empty.</summary>
		public bool IsEmpty {
			get { return files.Count == 0; }
		}
	}
}
