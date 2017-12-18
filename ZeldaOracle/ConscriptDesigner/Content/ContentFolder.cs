using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ConscriptDesigner.Controls;
using IOPath = System.IO.Path;
using ConscriptDesigner.Control;
using System.ComponentModel;

namespace ConscriptDesigner.Content {
	public class ContentFolder : ContentFile, IComparable {
		
		private Dictionary<string, ContentFile> files;
		
		public ContentFolder(string name) :
			base(name)
		{
			this.files = new Dictionary<string, ContentFile>(StringComparer.OrdinalIgnoreCase);
			TreeViewItem = new FolderTreeViewItem(name, false);
			XmlInfo.ElementName = "Folder";
		}

		int IComparable.CompareTo(object obj) {
			return CompareTo(obj as ContentFile);
		}
		public override int CompareTo(ContentFile file) {
			if (file.IsFolder) {
				return string.Compare(Path, file.Path);
			}
			return string.Compare(Path, file.Path) - 10000;
		}

		public IEnumerable<ContentFile> GetLocalFiles() {
			foreach (var pair in files) {
				yield return pair.Value;
			}
		}

		public IEnumerable<ContentFile> GetAllFiles() {
			return GetAllFiles(this);
		}

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

		public override ContentTypes ContentType {
			get { return ContentTypes.Folder; }
		}

		/*public void Add(ContentFile file) {
			if (files.ContainsKey(file.Name))
				throw new FileAlreadyExistsException(file.Name);
			files.Add(file.Name, file);
			file.Parent = this;
		}

		public void Remove(string name) {
			ContentFile file = Get(name);
			if (file == null)
				throw new FileDoesNotExistException(name);
			files.Remove(name);
			file.Parent = null;
		}

		public ContentFile Get(string name) {
			ContentFile file;
			files.TryGetValue(name, out file);
			return file;
		}

		public bool Contains(string name) {
			return files.ContainsKey(name);
		}

		public void Rename(string name, string newName) {
			ContentFile file = Get(name);
			if (file == null)
				throw new FileDoesNotExistException(name);
			if (files.ContainsKey(newName))
				throw new FileAlreadyExistsException(newName);
			files.Remove(name);
			file.Name = newName;
			files.Add(newName, file);
		}*/

		//-----------------------------------------------------------------------------
		// Override Context Menu
		//-----------------------------------------------------------------------------

		protected override void CreateContextMenu(ContextMenu menu) {
			AddAddContextMenuItem(menu);
			AddSeparatorContextMenuItem(menu);
			AddExcludeContextMenuItem(menu);
			AddSeparatorContextMenuItem(menu);
			AddClipboardContextMenuItems(menu, true);
			AddRenameContextMenuItem(menu);
		}



		public IEnumerable<ContentFile> GetFiles() {
			return files.Values;
		}

		internal Dictionary<string, ContentFile> Files {
			get { return files; }
		}

		public bool IsEmpty {
			get { return files.Count == 0; }
		}
	}
}
