using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConscriptDesigner.Content;
using ConscriptDesigner.Control;
using Xceed.Wpf.AvalonDock.Layout;

namespace ConscriptDesigner.Anchorables {
	/// <summary>The document used for content files.</summary>
	public abstract class ContentFileDocument : RequestCloseDocument, IContentFileContainer {

		/// <summary>The content file for the content file document.</summary>
		private ContentFile file;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the content file document.</summary>
		public ContentFileDocument(ContentFile file) {
			this.file = file;
		}
		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the content file for the content file document.</summary>
		public ContentFile File {
			get { return file; }
		}
	}
}
