using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConscriptDesigner.Control;

namespace ConscriptDesigner.Content {
	public class ContentShader : ContentFile {
		
		public ContentShader(string name) :
			base(name) {
			TreeViewItem.Source = DesignerImages.ShaderFile;
			XmlInfo.ElementName = "Compile";
			XmlInfo.Importer = "EffectImporter";
			XmlInfo.Processor = "EffectProcessor";
		}

		public override ContentTypes ContentType {
			get { return ContentTypes.Shader; }
		}
	}
}
