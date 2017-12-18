using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConscriptDesigner.Control;

namespace ConscriptDesigner.Content {
	class ContentSpriteFont : ContentFile {

		public ContentSpriteFont(string name) :
			base(name) {
			TreeViewItem.Source = DesignerImages.SpriteFontFile;
			XmlInfo.ElementName = "Compile";
			XmlInfo.Importer = "FontDescriptionImporter";
			XmlInfo.Processor = "FontDescriptionProcessor";
		}

		public override ContentTypes ContentType {
			get { return ContentTypes.SpriteFont; }
		}
	}
}
