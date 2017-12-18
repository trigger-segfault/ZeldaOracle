using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using ConscriptDesigner.Anchorables;
using ConscriptDesigner.Content.Xnb;
using ConscriptDesigner.Control;
using ConscriptDesigner.Windows;
using Microsoft.Xna.Framework.Graphics;

namespace ConscriptDesigner.Content {
	public class ContentImage : ContentFile {

		private Texture2D texture;
		private ImageDisplay imageDisplay;
		
		public ContentImage(string name) :
			base(name) {
			this.texture = null;
			TreeViewItem.Source = DesignerImages.ImageFile;
			XmlInfo.ElementName = "Compile";
			XmlInfo.Importer = "TextureImporter";
			XmlInfo.Processor = "TextureProcessor";
		}

		public override bool ShouldCompile {
			get {
				return (XmlInfo.Importer == "TextureImporter" && XmlInfo.Processor == "TextureProcessor");
			}
		}


		public override bool Compile() {
			return PngConverter.Convert(FilePath, OutputFilePath);
		}

		public override ContentTypes ContentType {
			get { return ContentTypes.Image; }
		}

		public Texture2D Texture {
			get { return texture; }
		}

		protected override void OnOpen() {
			Anchorable = DesignerControl.CreateDocumentAnchorable();
			Anchorable.Title = Name;
			imageDisplay = new ImageDisplay(this, Anchorable);
			Anchorable.Content = imageDisplay;
			Anchorable.IconSource = DesignerImages.ImageFile;
		}

		protected override void OnRename() {
			if (IsOpen) {
				Anchorable.Title = Name;
			}
		}

		protected override void OnClose() {
			imageDisplay = null;
		}

		//-----------------------------------------------------------------------------
		// Override Context Menu
		//-----------------------------------------------------------------------------

		protected override void CreateContextMenu(ContextMenu menu) {
			AddOpenContextMenuItem(menu);
			AddSeparatorContextMenuItem(menu);
			AddExcludeContextMenuItem(menu);
			AddSeparatorContextMenuItem(menu);
			AddClipboardContextMenuItems(menu, false);
			AddRenameContextMenuItem(menu);
		}
	}
}
