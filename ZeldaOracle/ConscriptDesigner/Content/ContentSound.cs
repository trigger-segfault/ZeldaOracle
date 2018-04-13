using ConscriptDesigner.Content.Xnb;
using ConscriptDesigner.Control;

namespace ConscriptDesigner.Content {
	/// <summary>A content file representing .wav extension.</summary>
	public class ContentSound : ContentFile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Conscructs the content sound.</summary>
		public ContentSound(string name) :
			base(name)
		{
			TreeViewItem.Source = DesignerImages.SoundFile;
			XmlInfo.ElementName = "Compile";
			XmlInfo.Importer = "WavImporter";
			XmlInfo.Processor = "SoundEffectProcessor";
		}


		//-----------------------------------------------------------------------------
		// Building
		//-----------------------------------------------------------------------------

		/// <summary>Compiles the content file as an xnb asset.</summary>
		public override bool Compile() {
			return WavConverter.Convert(FilePath, OutputFilePath);
		}


		//-----------------------------------------------------------------------------
		// Override Events
		//-----------------------------------------------------------------------------

		/// <summary>Called when the file is opened.</summary>
		protected override void OnOpen() {
			DesignerControl.PlaySound(this);
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of the content file.</summary>
		public override ContentTypes ContentType {
			get { return ContentTypes.Sound; }
		}

		/// <summary>Gets if the content file should be compiled.</summary>
		public override bool ShouldCompile {
			get {
				return (XmlInfo.Importer == "WavImporter" && XmlInfo.Processor == "SoundEffectProcessor");
			}
		}
	}
}
