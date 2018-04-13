using System.Windows.Controls;
using ConscriptDesigner.Anchorables;
using ConscriptDesigner.Content.Xnb;
using ConscriptDesigner.Control;

namespace ConscriptDesigner.Content {
	/// <summary>A content file representing .png, .jpg, and .gif extensions.</summary>
	public class ContentImage : ContentFile {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Conscructs the content image.</summary>
		public ContentImage(string name) :
			base(name)
		{
			TreeViewItem.Source = DesignerImages.ImageFile;
			XmlInfo.ElementName = "Compile";
			XmlInfo.Importer = "TextureImporter";
			XmlInfo.Processor = "TextureProcessor";
		}

		
		//-----------------------------------------------------------------------------
		// Building
		//-----------------------------------------------------------------------------

		/// <summary>Compiles the content file as an xnb asset.</summary>
		public override bool Compile() {
			return PngConverter.Convert(FilePath, OutputFilePath);
		}


		//-----------------------------------------------------------------------------
		// Override Events
		//-----------------------------------------------------------------------------

		/// <summary>Called when the file is opened.</summary>
		protected override void OnOpen() {
			Document = new ImageDisplay(this);
		}

		/// <summary>Called when the file is closed.</summary>
		protected override void OnClose() {
			
		}


		//-----------------------------------------------------------------------------
		// Override Context Menu
		//-----------------------------------------------------------------------------

		/// <summary>Creates the context menu for the tree view item.</summary>
		protected override void CreateContextMenu(ContextMenu menu) {
			AddOpenContextMenuItem(menu);
			AddSeparatorContextMenuItem(menu);
			AddExcludeContextMenuItem(menu);
			AddSeparatorContextMenuItem(menu);
			AddClipboardContextMenuItems(menu, false);
			AddRenameContextMenuItem(menu);
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of the content file.</summary>
		public override ContentTypes ContentType {
			get { return ContentTypes.Image; }
		}

		/// <summary>Gets if the content file should be compiled.</summary>
		public override bool ShouldCompile {
			get {
				return (XmlInfo.Importer == "TextureImporter" && XmlInfo.Processor == "TextureProcessor");
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the document as an image display.</summary>
		private ImageDisplay ImageDisplay {
			get { return Document as ImageDisplay; }
		}
	}
}
