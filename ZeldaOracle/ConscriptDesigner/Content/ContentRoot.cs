using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConscriptDesigner.Controls;
using ConscriptDesigner.Control;
using System.IO;
using IOPath = System.IO.Path;
using IODirectory = System.IO.Directory;
using System.Xml;
using System.Windows.Controls;
using System.Windows;
using ConscriptDesigner.Windows;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Windows.Input;
using ConscriptDesigner.Util;
using ZeldaOracle.Common.Util;

namespace ConscriptDesigner.Content {
	/// <summary>The root content folder that handles the content project and all operations.</summary>
	public class ContentRoot : ContentFolder {
		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The path to the content project file.</summary>
		private string contentFile;
		/// <summary>True if the project file has been modified and needs to be saved.</summary>
		private bool projectModified;
		/// <summary>The currently cut file in the project explorer.</summary>
		private ContentFile cutFile;
		/// <summary>True if the current files were directly copied from the project explorer.</summary>
		private bool copiedFromExplorer;
		/// <summary>The loaded XML document for the project file.</summary>
		private XmlDocument xmlDoc;
		/// <summary>The loaded XML project file root element.</summary>
		private XmlElement xmlProject;
		/// <summary>The namespace manager for accessing elements in the XML document.</summary>
		private XmlNamespaceManager xmlns;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the root content folder.</summary>
		public ContentRoot() :
			base("ContentProject.contentproj")
		{
			this.projectModified = false;
			this.contentFile = "";
			this.cutFile = null;
			this.copiedFromExplorer = false;

			this.xmlDoc = null;
			this.xmlProject = null;
			this.xmlns = null;

			TreeViewItem = new ImageTreeViewItem(DesignerImages.ContentProject, "ContentProject.contentproj", true);
			ClipboardHelper.ClipboardChanged += OnClipboardChanged;
		}

		/// <summary>Cleans up the content root events.</summary>
		public void Cleanup() {
			ClipboardHelper.ClipboardChanged -= OnClipboardChanged;
		}


		//-----------------------------------------------------------------------------
		// Loading
		//-----------------------------------------------------------------------------

		/// <summary>Loads the content project from the XML file.</summary>
		public void LoadContentProject(string path) {
			Files.Clear();
			TreeViewItem.Items.Clear();
			Name = IOPath.GetFileName(path);
			contentFile = path;
			try {
				xmlDoc = new XmlDocument();
				xmlDoc.Load(path);
				xmlns = new XmlNamespaceManager(xmlDoc.NameTable);
				xmlns.AddNamespace("ns", ContentXmlInfo.XmlNamespace);

				xmlProject = xmlDoc.SelectSingleNode("/ns:Project", xmlns) as XmlElement;
				if (xmlProject == null)
					throw new InvalidProjectFileException(Name);


				ForEachContentElement((XmlElement item) => {
					ContentXmlInfo xmlInfo = new ContentXmlInfo();
					xmlInfo.Read(item);

					string include = PathHelper.TrimEnd(xmlInfo.Include);
					string directory = IOPath.GetDirectoryName(include);
					ContentFolder parent = EnsureContentFolderExists(directory);
					ContentFile file = CreateContentFileFromXml(xmlInfo);

					parent.Files.Add(file.Name, file);
					parent.TreeViewItem.Items.Add(file.TreeViewItem);
					file.Parent = parent;
					parent.Resort();

					return false;
				});

				bool someMissing = false;

				foreach (ContentFile file in GetAllFiles()) {
					try {
						file.UpdateLastModified();
					}
					catch (Exception) {
						//file.IsMissing = true;
						someMissing = true;
					}
				}

				if (someMissing) {
					TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
						"Some project files could not be found!", "Missing Files");
				}

				UpdateLastModified();
			}
			catch (Exception ex) {
				Name = "ContentProject.contentproj";
				contentFile = "";
				throw ex;
			}
		}

		/// <summary>Saves the content project from the XML file.</summary>
		public void SaveContentProject() {
			// Remove previous content definitions then redefine them
			ForEachContentElement((XmlElement item) => {
				return true;
			});

			// Order the files by type for convenience
			List<ContentFile> folders = new List<ContentFile>();
			List<ContentFile> conscripts = new List<ContentFile>();
			List<ContentFile> images = new List<ContentFile>();
			List<ContentFile> sounds = new List<ContentFile>();
			List<ContentFile> shaders = new List<ContentFile>();
			List<ContentFile> spriteFonts = new List<ContentFile>();
			List<ContentFile> unknown = new List<ContentFile>();

			foreach (ContentFile file in GetAllFiles()) {
				file.UpdateXmlInfo();
				switch (file.ContentType) {
				case ContentTypes.Folder:
					if (((ContentFolder) file).IsEmpty)
						folders.Add(file);
					break;
				case ContentTypes.Conscript: conscripts.Add(file); break;
				case ContentTypes.Image: images.Add(file); break;
				case ContentTypes.Sound: sounds.Add(file); break;
				case ContentTypes.Shader: shaders.Add(file); break;
				case ContentTypes.SpriteFont: spriteFonts.Add(file); break;
				case ContentTypes.Unknown: unknown.Add(file); break;
				}
			}

			folders.Sort();
			conscripts.Sort();
			images.Sort();
			sounds.Sort();
			shaders.Sort();
			spriteFonts.Sort();
			unknown.Sort();

			xmlProject.AppendChild(CreateItemGroupElement(folders));
			xmlProject.AppendChild(CreateItemGroupElement(conscripts));
			xmlProject.AppendChild(CreateItemGroupElement(images));
			xmlProject.AppendChild(CreateItemGroupElement(sounds));
			xmlProject.AppendChild(CreateItemGroupElement(shaders));
			xmlProject.AppendChild(CreateItemGroupElement(spriteFonts));
			xmlProject.AppendChild(CreateItemGroupElement(unknown));

			xmlDoc.Save(contentFile);
			projectModified = false;

			UpdateLastModified();
		}

		/// <summary>Enumerates through every valid item ground element. Return true to remove the item.</summary>
		private void ForEachContentElement(Func<XmlElement, bool> function) {
			XmlNodeList itemGroups = xmlProject.SelectNodes("ns:ItemGroup", xmlns);
			for (int i = 0; i < itemGroups.Count; i++) {
				XmlElement itemGroup = itemGroups[i] as XmlElement;
				if (itemGroup == null) continue;
				XmlNodeList childNodes = itemGroup.ChildNodes;
				for (int j = 0; j < childNodes.Count; j++) {
					XmlElement item = childNodes[j] as XmlElement;
					if (item == null) continue;
					if (ContentXmlInfo.IsContentElement(item)) {
						if (function(item)) {
							itemGroup.RemoveChild(item);
							j--;
						}
					}
				}
				if (childNodes.Count == 0) {
					xmlProject.RemoveChild(itemGroup);
					//i--;
				}
			}
		}

		/// <summary>Creates an ItemGroup element from the list of files.</summary>
		private XmlElement CreateItemGroupElement(IEnumerable<ContentFile> files) {
			XmlElement itemGroup = xmlDoc.CreateElement("", "ItemGroup", ContentXmlInfo.XmlNamespace);
			foreach (ContentFile file in files) {
				file.UpdateXmlInfo();
				itemGroup.AppendChild(file.XmlInfo.Write(xmlDoc));
			}
			return itemGroup;
		}

		/// <summary>Creates a content file from the XML info.</summary>
		private static ContentFile CreateContentFileFromXml(ContentXmlInfo xmlInfo) {
			ContentFile file;
			if (xmlInfo.ElementName == "Folder") {
				string folderName = IOPath.GetFileName(PathHelper.TrimEnd(xmlInfo.Include));
				file = new ContentFolder(folderName);
			}
			else {
				file = CreateContentFileFromPath(xmlInfo.Include);
			}
			file.XmlInfo = xmlInfo;
			return file;
		}

		/// <summary>Creates a content file from the file path.</summary>
		private static ContentFile CreateContentFileFromPath(string path) {
			string ext = IOPath.GetExtension(path).ToLower();
			string name = IOPath.GetFileName(path);
			if (IODirectory.Exists(path)) {
				return new ContentFolder(name);
			}
			switch (ext) {
			case ".conscript":
				return new ContentScript(name);
			case ".png":
			case ".jpg":
			case ".gif":
				return new ContentImage(name);
			case ".wav":
				return new ContentSound(name);
			case ".fx":
				return new ContentShader(name);
			case ".spritefont":
				return new ContentSpriteFont(name);
			default:
				return new ContentFile(name);
			}
		}

		/// <summary>Ensures the content folder exists.</summary>
		private ContentFolder EnsureContentFolderExists(string path) {
			FixPath(ref path);
			if (string.IsNullOrEmpty(path))
				return this;
			int index = path.IndexOf('/');
			string directory, name;
			ContentFolder parent = this;
			ContentFolder newDir;
			while (index != -1) {
				directory = path.Substring(0, index);
				name = IOPath.GetFileName(directory);
				if (!parent.Files.ContainsKey(name)) {
					newDir = new ContentFolder(name);
					parent.Files.Add(name, newDir);
					parent.TreeViewItem.Items.Add(newDir.TreeViewItem);

					newDir.Parent = parent;
					parent.Resort();
				}
				parent = parent.Files[name] as ContentFolder;
				index = path.IndexOf('/', index + 1);
			}
			name = path.Substring(path.LastIndexOf('/') + 1);
			if (!parent.Files.ContainsKey(name)) {
				newDir = new ContentFolder(name);
				parent.Files.Add(name, newDir);
				parent.TreeViewItem.Items.Add(newDir.TreeViewItem);

				newDir.Parent = parent;
				parent.Resort();
			}
			return parent.Files[name] as ContentFolder;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Clear the cut file when the clipboard changes.</summary>
		private void OnClipboardChanged(object sender, EventArgs e) {
			// Only clear the cut file if the clipboard is no longer empty
			if (!ClipboardHelper.IsEmpty()) {
				if (cutFile != null)
					cutFile.IsCut = false;
				cutFile = null;
				copiedFromExplorer = false;
			}
		}
		

		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the project contains the file.</summary>
		public bool Contains(string path) {
			return Get(path) != null;
		}

		/// <summary>Gets the project-contained file.</summary>
		public ContentFile Get(string path) {
			FixPath(ref path);
			if (path == "")
				return this;
			return GetFile(this, path);
		}

		/// <summary>Gets the project-contained folder.</summary>
		public ContentFolder GetFolder(string path) {
			FixPath(ref path);
			if (path == "")
				return this;
			return GetFile(this, path) as ContentFolder;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Includes the file in the project.</summary>
		public void Include(string filePath, string newDirectory, bool catchExceptions = false) {
			string name = IOPath.GetFileName(filePath);
			ContentFolder parent = GetFolder(newDirectory);
			if (parent == null)
				throw new DirectoryDoesNotExistException(newDirectory);
			if (parent.Files.ContainsKey(name))
				throw new FileAlreadyExistsException(name);

			string newPath = IOPath.Combine(parent.Path, name);
			string newFilePath = IOPath.Combine(parent.FilePath, name);

			//if (File.Exists(newFilePath) || IODirectory.Exists(newFilePath))
			//	throw new FileAlreadyExistsException(name);

			if (!PathHelper.IsPathTheSame(filePath, newFilePath)) {
				try {
					PathHelper.CopyFileOrDirectory(filePath, newFilePath, true);
				}
				catch (Exception ex) {
					if (catchExceptions)
						DesignerControl.ShowExceptionMessage(ex, "copy", name);
					else
						throw ex;
					return;
				}
			}
			ContentFile file =  CreateContentFileFromPath(filePath);
			parent.Files.Add(file.Name, file);
			parent.TreeViewItem.Items.Add(file.TreeViewItem);

			file.Parent = parent;
			parent.Resort();
			projectModified = true;
		}

		/// <summary>Excludes the file from the project.</summary>
		public void Exclude(string path) {
			string name = IOPath.GetFileName(path);
			ContentFile file = Get(path);
			if (file == null)
				throw new FileDoesNotExistException(name);
			file.Close();
			file.Parent.Files.Remove(name);
			file.Parent.TreeViewItem.Items.Remove(file.TreeViewItem);

			file.Parent = null;
			projectModified = true;
		}

		/// <summary>Replaces the existing file with a new file.</summary>
		public void Replace(string filePath, string newPath, bool catchExceptions = false) {
			string name = IOPath.GetFileName(newPath);
			string newDirectory = IOPath.GetDirectoryName(newPath);
			string newFilePath = IOPath.Combine(ProjectDirectory, newPath);
			ContentFolder parent = GetFolder(newDirectory);
			if (parent == null)
				throw new DirectoryDoesNotExistException(newDirectory);
			if (!parent.Files.ContainsKey(name))
				throw new FileDoesNotExistException(name);

			bool isFolder = IODirectory.Exists(filePath);
			if (isFolder != IODirectory.Exists(newFilePath)) {
				if (!catchExceptions)
					throw new DirectoryFileMismatchException(IOPath.GetFileName(filePath), name);
				TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
					"Cannot replace '" + name + "' due to directory/file mismatch!", "Replace Failed");
			}

			try {
				PathHelper.CopyFileOrDirectory(filePath, newFilePath, true);
			}
			catch (Exception ex) {
				if (catchExceptions)
					DesignerControl.ShowExceptionMessage(ex, "copy", name);
				else
					throw ex;
				return;
			}

			ContentFile oldFile = parent.Files[name];
			oldFile.Close();
			parent.Files.Remove(name);
			parent.TreeViewItem.Items.Remove(oldFile.TreeViewItem);
			oldFile.Parent = null;

			ContentFile file =  CreateContentFileFromPath(filePath);
			parent.Files.Add(file.Name, file);
			parent.TreeViewItem.Items.Add(file.TreeViewItem);

			file.Parent = parent;
			parent.Resort();
			projectModified = true;
		}

		/// <summary>Moves the file to the new directory.</summary>
		public void Move(string path, string newDirectory, bool catchExceptions = false) {
			FixPath(ref path, ref newDirectory);
			string name = IOPath.GetFileName(path);
			string newPath = IOPath.Combine(newDirectory, name);
			ContentFile file = Get(path);
			if (file == null)
				throw new FileDoesNotExistException(name);
			string filePath = file.FilePath;
			ContentFolder parent = GetFolder(newDirectory);
			if (parent == null)
				throw new DirectoryDoesNotExistException(newDirectory);
			string newFilePath = IOPath.Combine(parent.FilePath, name);
			if (parent.Files.ContainsKey(name))
				throw new FileAlreadyExistsException(name);
			if (file.IsFolder && IsSubdirectory(file.Path, parent.Path))
				throw new DirectoryIsSubdirectoryException(file.Name, parent.Name);

			try {
				if (file.IsFolder)
					IODirectory.Move(filePath, newFilePath);
				else
					File.Move(filePath, newFilePath);
			}
			catch (Exception ex) {
				if (catchExceptions)
					DesignerControl.ShowExceptionMessage(ex, "move", name);
				else
					throw ex;
				return;
			}

			file.Parent.Files.Remove(file.Name);
			parent.Files.Add(file.Name, file);

			file.Parent.TreeViewItem.Items.Remove(file.TreeViewItem);
			parent.TreeViewItem.Items.Add(file.TreeViewItem);

			file.Parent = parent;
			parent.Resort();

			projectModified = true;
		}

		/// <summary>Renames the file to the new name.</summary>
		public void Rename(string path, string newName, bool catchExceptions = false) {
			FixPath(ref path);
			string name = IOPath.GetFileName(path);
			ContentFile file = Get(path);
			if (file == null)
				throw new FileDoesNotExistException(name);
			string filePath = file.FilePath;
			string newPath = IOPath.Combine(IOPath.GetDirectoryName(path), newName);
			string newFilePath = IOPath.Combine(file.FileDirectory, newName);
			if (file.Parent.Files.ContainsKey(newName))
				throw new FileAlreadyExistsException(newName);

			try {
				PathHelper.MoveFileOrDirectory(filePath, newFilePath);
				/*if (file.IsFolder)
					IODirectory.Move(filePath, newFilePath);
				else
					File.Move(filePath, newFilePath);*/
			}
			catch (Exception ex) {
				if (catchExceptions)
					DesignerControl.ShowExceptionMessage(ex, "rename", name);
				return;
			}

			file.Parent.Files.Remove(name);
			file.Parent.Files.Add(newName, file);
			file.Parent.Resort();

			projectModified = true;
		}

		/// <summary>Cuts the file.</summary>
		public void Cut(string path) {
			FixPath(ref path);
			Clipboard.Clear();
			string name = IOPath.GetFileName(path);
			if (cutFile != null)
				cutFile.IsCut = false;
			ContentFile file = Get(path);
			if (file == null)
				throw new FileDoesNotExistException(name);
			cutFile = file;
			file.IsCut = true;
			copiedFromExplorer = false;
			CommandManager.InvalidateRequerySuggested();
		}

		/// <summary>Copies the file.</summary>
		public void Copy(string path) {
			if (cutFile != null)
				cutFile.IsCut = false;
			string name = IOPath.GetFileName(path);
			ContentFile file = Get(path);
			if (file == null)
				throw new FileDoesNotExistException(name);
			StringCollection files = new StringCollection();
			files.Add(file.FilePath);
			Clipboard.SetFileDropList(files);
			copiedFromExplorer = true;
			CommandManager.InvalidateRequerySuggested();
		}


		//-----------------------------------------------------------------------------
		// Dialog Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Requests the user to create a new conscript file in the directory.</summary>
		public void NewConscript(string directory) {
			string name = RenameFileWindow.Show(DesignerControl.MainWindow, "Add",
				"New Conscript", "conscript.conscript", directory, this);
			if (name != null) {
				string filePath = IOPath.Combine(ProjectDirectory, directory, name);
				try {
					using (File.Create(filePath)) { }
				}
				catch (Exception ex) {
					DesignerControl.ShowExceptionMessage(ex, "create", name);
					return;
				}

				ContentFolder parent = GetFolder(directory);
				if (parent == null)
					throw new DirectoryDoesNotExistException(parent.Path);
				if (parent.Files.ContainsKey(name))
					throw new FileAlreadyExistsException(name);
				ContentScript file = new ContentScript(name);
				parent.Files.Add(name, file);
				parent.TreeViewItem.Items.Add(file.TreeViewItem);
				file.Parent = parent;
				parent.Resort();
				file.TreeViewItem.IsSelected = true;
				projectModified = true;
			}
		}

		/// <summary>Requests the user to add an existing file into the directory.</summary>
		public void AddExisting(string directory) {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Content Files|*.conscript;*.png;*.jpg;*.gif;*.wav;*.fx|" +
				"Conscript Files|*.conscript|" +
				"Image Files|*.png;*.jpg;*.gif|" + 
				"Sound Files|*.wav|" +
				"Shader Files|*.fx|" +
				"Sprite Font Files|*.spritefont|" +
				"All files|*.*";
			dialog.FilterIndex = 0;
			dialog.CheckFileExists = true;
			dialog.Title = "Add Existing File";
			var result = dialog.ShowDialog(DesignerControl.MainWindow);
			if (result.HasValue && result.Value) {
				string filePath = dialog.FileName;
				string name = IOPath.GetFileName(filePath);
				string newFilePath = IOPath.Combine(ProjectDirectory, directory, name);

				ContentFolder parent = GetFolder(directory);
				if (parent == null)
					throw new DirectoryDoesNotExistException(parent.Path);
				if (parent.Files.ContainsKey(name)) {
					TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
						"A project file with the name '" + name + "' already exists!", "File Already Exists");
					return;
				}
				else if (!PathHelper.IsPathTheSame(filePath, newFilePath)) {
					if (File.Exists(filePath)) {
						TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
							"A file with the name '" + name + "' already exists!", "File Already Exists");
						return;
					}
					else if (IODirectory.Exists(filePath)) {
						TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
							"A directory with the name '" + name + "' already exists!", "File Already Exists");
						return;
					}
				}

				Include(filePath, directory, true);
				projectModified = true;
			}
		}

		/// <summary>Requests the user to create a new folder in the directory.</summary>
		public void NewFolder(string directory) {
			string name = RenameFileWindow.Show(DesignerControl.MainWindow, "Add",
				"New Folder", "Folder", directory, this);
			if (name != null) {
				string filePath = IOPath.Combine(ProjectDirectory, directory, name);
				try {
					IODirectory.CreateDirectory(filePath);
				}
				catch (Exception ex) {
					DesignerControl.ShowExceptionMessage(ex, "create", name);
					return;
				}

				ContentFolder parent = GetFolder(directory);
				if (parent == null)
					throw new DirectoryDoesNotExistException(parent.Path);
				if (parent.Files.ContainsKey(name))
					throw new FileAlreadyExistsException(name);
				ContentFolder file = new ContentFolder(name);
				parent.Files.Add(name, file);
				parent.TreeViewItem.Items.Add(file.TreeViewItem);
				file.Parent = parent;
				parent.Resort();
				file.TreeViewItem.IsSelected = true;
				projectModified = true;
			}
		}

		/// <summary>Requests the user to paste the clipboard's file drop or cut file.</summary>
		public void RequestPaste(string directory) {
			ContentFolder parent = GetFolder(directory);
			if (parent == null)
				throw new DirectoryDoesNotExistException(parent.Path);
			if (cutFile != null) {
				if (cutFile == parent) {
					TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
						"Cannot paste cut folder '" + cutFile.Name + "' into itself!", "Paste Failed");
					return;
				}
				else if (cutFile.Parent == parent) {
					// Do nothing
				}
				else if (cutFile.IsFolder && IsSubdirectory(cutFile.Path, directory)) {
					TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
						"Cannot paste cut folder '" + cutFile.Name + "' into a subdirectory of itself!", "Paste Failed");
					return;
				}
				else {
					string filePath = cutFile.FilePath;
					string newFilePath = IOPath.Combine(parent.FilePath, cutFile.Name);
					try {
						PathHelper.MoveFileOrDirectory(filePath, newFilePath);
					}
					catch (Exception ex) {
						DesignerControl.ShowExceptionMessage(ex, "paste", cutFile.Name);
						return;
					}
				}

				cutFile.Parent.Files.Remove(cutFile.Name);
				cutFile.Parent.TreeViewItem.Items.Remove(cutFile.TreeViewItem);
				parent.Files.Add(cutFile.Name, cutFile);
				parent.TreeViewItem.Items.Add(cutFile.TreeViewItem);
				cutFile.Parent = parent;
				parent.Resort();

				cutFile.IsCut = false;
				cutFile = null;
				projectModified = true;
			}
			else if (Clipboard.ContainsData(DataFormats.FileDrop)) {
				var files = Clipboard.GetFileDropList();
				List<string> filesFinal = new List<string>();
				foreach (string file in files) {
					filesFinal.Add(file);
				}
				RequestDrop(filesFinal, directory);
			}
			CommandManager.InvalidateRequerySuggested();
		}

		/// <summary>Requests the user to drop/paste the list of files.</summary>
		public void RequestDrop(IEnumerable<string> files, string directory) {
			ContentFolder folder = GetFolder(directory);
			if (folder == null)
				throw new DirectoryDoesNotExistException(directory);
			
			List<string> filesToReplace = new List<string>();
			List<string> foldersToMerge = new List<string>();
			List<string> filesToInclude = new List<string>();
			List<string> filesToCopyName = new List<string>();
			foreach (string file in files) {
				string name = IOPath.GetFileName(file);
				string newPath = IOPath.Combine(folder.Path, name);
				string newFilePath = IOPath.Combine(folder.FilePath, name);
				bool isFolder = IODirectory.Exists(file);
				if (PathHelper.IsPathTheSame(file, newFilePath)) {
					if (!Contains(newPath))
						filesToInclude.Add(file);
					else if (copiedFromExplorer)
						filesToCopyName.Add(file);
				}
				else if (File.Exists(newFilePath)) {
					if (!isFolder)
						filesToReplace.Add(file);
					else {
						TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning, "Cannot include directory '" +
							name + "' because a file with that name already exists!", "Include Failed");
						return;
					}
				}
				else if (IODirectory.Exists(newFilePath)) {
					if (isFolder)
						foldersToMerge.Add(file);
					else {
						TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning, "Cannot include file '" +
							name + "' because a directory with that name already exists!", "Include Failed");
						return;
					}
				}
				else {
					filesToInclude.Add(file);
				}
			}

			MessageBoxResult replaceResult = MessageBoxResult.No;
			MessageBoxResult mergeResult = MessageBoxResult.No;

			if (filesToReplace.Any()) {
				replaceResult = TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
						"Some files have the same name as files already in this directory. " +
						"Would you like to replace them?", "Replace Files", MessageBoxButton.YesNoCancel);
			}

			if (foldersToMerge.Any() && replaceResult != MessageBoxResult.Cancel) {
				mergeResult = TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
						"Some folders have the same name as folders in this directory. " +
						"Would you like to merge them?", "Merge Folders", MessageBoxButton.YesNoCancel);
			}

			if (replaceResult != MessageBoxResult.Cancel && mergeResult != MessageBoxResult.Cancel) {
				if (replaceResult == MessageBoxResult.Yes) {
					foreach (string file in filesToReplace) {
						string newPath = IOPath.Combine(folder.Path, IOPath.GetFileName(file));
						try {
							if (Contains(newPath))
								Replace(file, newPath);
							else
								Include(file, folder.Path);
						}
						catch (Exception ex) {
							DesignerControl.ShowExceptionMessage(ex, "replace", IOPath.GetFileName(file));
							return;
						}
					}
				}

				if (mergeResult == MessageBoxResult.Yes) {
					foreach (string file in foldersToMerge) {
						string newPath = IOPath.Combine(folder.Path, IOPath.GetFileName(file));
						try {
							if (Contains(newPath))
								Replace(file, newPath);
							else
								Include(file, folder.Path);
						}
						catch (Exception ex) {
							DesignerControl.ShowExceptionMessage(ex, "merge", IOPath.GetFileName(file));
							return;
						}
					}
				}

				foreach (string file in filesToCopyName) {
					try {
						string newFile = PathHelper.GetCopyName(file);
						PathHelper.CopyFileOrDirectory(file, newFile, false);
						Include(newFile, folder.Path);
					}
					catch (Exception ex) {
						DesignerControl.ShowExceptionMessage(ex, "copy", IOPath.GetFileName(file));
						return;
					}
				}

				foreach (string file in filesToInclude) {
					try {
						Include(file, folder.Path);
					}
					catch (Exception ex) {
						DesignerControl.ShowExceptionMessage(ex, "copy", IOPath.GetFileName(file));
						return;
					}
				}
			}
			CommandManager.InvalidateRequerySuggested();
		}

		/// <summary>Requests the user to delete the file.</summary>
		public void RequestDelete(string path) {
			var result = TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
				"Are you sure you want to delete '" + IOPath.GetFileName(path) + "'?", "Delete File",
				MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes) {
				try {
					ContentFile file = Get(path);
					file.Close(true);
					string filePath = file.FilePath;
					file.Parent.Files.Remove(file.Name);
					file.Parent.TreeViewItem.Items.Remove(file.TreeViewItem);
					file.Parent = null;
					projectModified = true;

					if (IODirectory.Exists(filePath))
						IODirectory.Delete(filePath, true);
					else
						File.Delete(filePath);
				}
				catch (Exception ex) {
					DesignerControl.ShowExceptionMessage(ex, "delete", IOPath.GetFileName(path));
				}
			}
		}

		/// <summary>Requests the user to rename the file.</summary>
		public void RequestRename(string path) {
			string oldName = IOPath.GetFileName(path);
			string directory = IOPath.GetDirectoryName(path);
			string newName = RenameFileWindow.Show(DesignerControl.MainWindow, "Rename",
				"Rename File", oldName, directory, this);
			if (newName != null) {

				ContentFile file = Get(path);
				if (file == null)
					throw new FileDoesNotExistException(oldName);

				if (string.Compare(oldName, newName, true) == 0) {
					file.Name = newName;
				}
				else {
					string filePath = IOPath.Combine(ProjectDirectory, directory, oldName);
					string newFilePath = IOPath.Combine(ProjectDirectory, directory, newName);
					try {
						PathHelper.MoveFileOrDirectory(filePath, newFilePath);
					}
					catch (Exception ex) {
						DesignerControl.ShowExceptionMessage(ex, "rename", oldName);
						return;
					}
				}
				file.Parent.Files.Remove(oldName);
				file.Name = newName;
				file.Parent.Files.Add(newName, file);
				file.Parent.Resort();
				file.TreeViewItem.IsSelected = true;
				projectModified = true;
			}
		}
		
		/// <summary>Moves the file to the new directory and shows error messages when needed.</summary>
		public void RequestMove(string path, string newDirectory) {
			FixPath(ref path, ref newDirectory);
			string name = IOPath.GetFileName(path);
			string newPath = IOPath.Combine(newDirectory, name);
			ContentFile file = Get(path);
			if (file == null)
				throw new FileDoesNotExistException(name);
			string filePath = file.FilePath;
			ContentFolder parent = GetFolder(newDirectory);
			if (parent == file.Parent)
				return;
			if (parent == null)
				throw new DirectoryDoesNotExistException(newDirectory);
			string newFilePath = IOPath.Combine(parent.FilePath, name);
			if (parent.Files.ContainsKey(name)) {
				TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
						"A content file with the same name already exists in this directory!",
						"Content File Already Exists");
				return;
			}
			else if (PathHelper.Exists(newFilePath)) {
				TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
						"A file with the same name already exists in this directory!",
						"File Already Exists");
				return;
			}
			if (file.IsFolder && IsSubdirectory(file.Path, parent.Path))
				throw new DirectoryIsSubdirectoryException(file.Name, parent.Name);

			try {
				if (file.IsFolder)
					IODirectory.Move(filePath, newFilePath);
				else
					File.Move(filePath, newFilePath);
			}
			catch (Exception ex) {
				DesignerControl.ShowExceptionMessage(ex, "move", name);
				return;
			}

			file.Parent.Files.Remove(file.Name);
			parent.Files.Add(file.Name, file);

			file.Parent.TreeViewItem.Items.Remove(file.TreeViewItem);
			parent.TreeViewItem.Items.Add(file.TreeViewItem);

			file.Parent = parent;
			parent.Resort();

			projectModified = true;
		}

		//-----------------------------------------------------------------------------
		// Override Context Menu
		//-----------------------------------------------------------------------------

		/// <summary>Creates the context menu for the tree view item.</summary>
		protected override void CreateContextMenu(ContextMenu menu) {
			AddAddContextMenuItem(menu);
			AddSeparatorContextMenuItem(menu);
			AddPasteContextMenuItem(menu);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Fixes the path separators for Get operations.</summary>
		private static void FixPath(ref string path) {
			path = path.Replace('\\', '/');
		}

		/// <summary>Fixes the path separators for Get operations.</summary>
		private static void FixPath(ref string path1, ref string path2) {
			path1 = path1.Replace('\\', '/');
			path2 = path2.Replace('\\', '/');
		}

		/// <summary>Returns true if the directory contains the subdirectory.</summary>
		private static bool IsSubdirectory(string directory, string subdirectory) {
			FixPath(ref directory, ref subdirectory);
			return subdirectory.StartsWith(directory, StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>Searches for the file with the specified path recursively.</summary>
		private static ContentFile GetFile(ContentFolder folder, string path) {
			int index = path.IndexOf('/');
			string name = (index != -1 ? path.Substring(0, index) : path);
			string nextPath = (index != -1 ? path.Substring(index + 1) : "");
			ContentFile file;
			folder.Files.TryGetValue(name, out file);
			if (file != null && index != -1) {
				// We need to go deeper!
				if (file.IsFolder) {
					return GetFile((ContentFolder) file, nextPath);
				}
				return null;
			}
			// Return either null or the file result
			return file;
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of the content file.</summary>
		public override ContentTypes ContentType {
			get { return ContentTypes.Project; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if paste can be used in the project explorer.</summary>
		public bool CanPaste {
			get { return Clipboard.ContainsFileDropList() || cutFile != null; }
		}

		/// <summary>Gets the currently cut file.</summary>
		public ContentFile CutFile {
			get { return cutFile; }
		}

		/// <summary>Gets or sets if the project is modified.</summary>
		public bool IsProjectModified {
			get { return projectModified; }
			set { projectModified = value; }
		}

		/// <summary>Gets the project file.</summary>
		public string ProjectFile {
			get { return contentFile; }
		}

		/// <summary>Gets the project root folder.</summary>
		public string ProjectDirectory {
			get { return IOPath.GetDirectoryName(contentFile); }
		}
	}
}
