using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConscriptDesigner.Content.Xnb;
using ConscriptDesigner.Control;
using Microsoft.Xna.Framework.Audio;

namespace ConscriptDesigner.Content {
	public class ContentSound : ContentFile {
		
		private SoundEffect soundEffect;

		public ContentSound(string name) :
			base(name) {
			this.soundEffect = null;
			TreeViewItem.Source = DesignerImages.SoundFile;
			XmlInfo.ElementName = "Compile";
			XmlInfo.Importer = "WavImporter";
			XmlInfo.Processor = "SoundEffectProcessor";
		}

		public override bool ShouldCompile {
			get {
				return (XmlInfo.Importer == "WavImporter" && XmlInfo.Processor == "SoundEffectProcessor");
			}
		}

		public override bool Compile() {
			return WavConverter.Convert(FilePath, OutputFilePath);
		}

		public override ContentTypes ContentType {
			get { return ContentTypes.Sound; }
		}

		public SoundEffect SoundEffect {
			get { return soundEffect; }
		}
	}
}
