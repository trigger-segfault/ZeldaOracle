using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOPath = System.IO.Path;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ZeldaOracleBuilder.Util;

namespace ZeldaOracleBuilder.Content {
	/// <summary>The base file for all content project files.</summary>
	public class ContentFile {

		/// <summary>The parent of the content file.</summary>
		private ContentFolder parent;
		/// <summary>The name of the content file.</summary>
		private string name;

		/// <summary>The XML info of the content file.</summary>
		private ContentXmlInfo xmlInfo;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Conscructs the content file.</summary>
		public ContentFile(string name) {
			this.name = name;
			this.parent = null;
			this.xmlInfo = new ContentXmlInfo();
			XmlInfo.ElementName = "None";
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


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the filename of the content file.</summary>
		public string Name {
			get { return name; }
			set { name = value; }
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
						return IOPath.Combine(Program.OutputContentDirectory, parent.Path, outptutFileName);
				}
				else if (IsRoot) {
					return Program.OutputContentDirectory;
				}
				return null;
			}
		}

		/// <summary>Gets or sets the XML info of the content file.
		/// This should only be set by ContentRoot.</summary>
		internal ContentXmlInfo XmlInfo {
			get { return xmlInfo; }
			set { xmlInfo = value; }
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
	}
}
