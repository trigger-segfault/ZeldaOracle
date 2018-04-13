using System;
using System.Xml;
using ConscriptDesigner.Content;
using ConscriptDesigner.Control;

namespace ConscriptDesigner.Anchorables {
	[Serializable]
	/// <summary>The document used for content files.</summary>
	public abstract class ContentFileDocument : RequestCloseDocument, IContentFileContainer {

		/// <summary>The content file for the content file document.</summary>
		private ContentFile file;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the content file document for serialization.</summary>
		public ContentFileDocument() {
			this.file = null;
		}

		/// <summary>Constructs the content file document.</summary>
		public ContentFileDocument(ContentFile file) {
			this.file = file;
		}


		//-----------------------------------------------------------------------------
		// Abstract Methods
		//-----------------------------------------------------------------------------

		/// <summary>Loads the file and completes setup.</summary>
		protected void LoadFile(ContentFile file) {
			this.file = file;
			OnLoadFile(file);
		}

		/// <summary>Completes setup after loading the content file.</summary>
		protected abstract void OnLoadFile(ContentFile file);


		//-----------------------------------------------------------------------------
		// XML Serialization
		//-----------------------------------------------------------------------------

		public override void ReadXml(XmlReader reader) {
			if (reader.MoveToAttribute("ContentPath")) {
				string contentPath = reader.Value;
				file = DesignerControl.Project.Get(contentPath);
				if (file != null && file.Open(true)) {
					LoadFile(file);
					base.ReadXml(reader);
					return;
				}
			}
			base.ReadXml(reader);
			Close();
		}

		public override void WriteXml(XmlWriter writer) {
			base.WriteXml(writer);
			writer.WriteAttributeString("ContentPath", File.Path);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the content file for the content file document.</summary>
		public ContentFile File {
			get { return file; }
			//protected set { file = value; }
		}

		/// <summary>A temporary path for xml serialization.</summary>
		//public string ContentPath { get; set; }
	}
}
