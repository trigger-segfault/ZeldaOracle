using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOPath = System.IO.Path;
using System.ComponentModel;

namespace ZeldaOracleBuilder.Content {
	public class ContentFolder : ContentFile {

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
			XmlInfo.ElementName = "Folder";
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

		/// <summary>Enumerates all files within this and all subfolders.</summary>
		public IEnumerable<ContentFile> GetAllFiles() {
			return GetAllFiles(this);
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
