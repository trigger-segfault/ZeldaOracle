using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IOPath = System.IO.Path;
using IODirectory = System.IO.Directory;
using System.Xml;
using System.Windows;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Windows.Input;
using ZeldaOracleBuilder.Util;

namespace ZeldaOracleBuilder.Content {
	/// <summary>The root content folder that handles the content project and all operations.</summary>
	public class ContentRoot : ContentFolder {
		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The path to the content project file.</summary>
		private string contentFile;
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
			this.contentFile = "";

			this.xmlDoc = null;
			this.xmlProject = null;
			this.xmlns = null;
		}


		//-----------------------------------------------------------------------------
		// Loading
		//-----------------------------------------------------------------------------

		/// <summary>Loads the content project from the XML file.</summary>
		public void LoadContentProject(string path) {
			Files.Clear();
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
					file.Parent = parent;

					return false;
				});
			}
			catch (Exception ex) {
				Name = "ContentProject.contentproj";
				contentFile = "";
				throw ex;
			}
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
			//case ".conscript":
			//	return new ContentScript(name);
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

					newDir.Parent = parent;
				}
				parent = parent.Files[name] as ContentFolder;
				index = path.IndexOf('/', index + 1);
			}
			name = path.Substring(path.LastIndexOf('/') + 1);
			if (!parent.Files.ContainsKey(name)) {
				newDir = new ContentFolder(name);
				parent.Files.Add(name, newDir);

				newDir.Parent = parent;
			}
			return parent.Files[name] as ContentFolder;
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
