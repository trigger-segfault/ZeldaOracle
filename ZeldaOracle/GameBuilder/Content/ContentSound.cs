using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracleBuilder.Content.Xnb;

namespace ZeldaOracleBuilder.Content {
	/// <summary>A content file representing .wav extension.</summary>
	public class ContentSound : ContentFile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Conscructs the content sound.</summary>
		public ContentSound(string name) :
			base(name)
		{
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
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of the content file.</summary>
		public override ContentTypes ContentType {
			get { return ContentTypes.Sound; }
		}

		/// <summary>Gets if the content file should be compiled.</summary>
		public override bool ShouldCompile {
			get {
				return (XmlInfo.Importer == "WavImporter" &&
					XmlInfo.Processor == "SoundEffectProcessor");
			}
		}
	}
}
