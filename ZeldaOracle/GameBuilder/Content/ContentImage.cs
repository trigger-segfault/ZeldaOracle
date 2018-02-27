using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracleBuilder.Content.Xnb;

namespace ZeldaOracleBuilder.Content {
	/// <summary>A content file representing .png, .jpg, and .gif extensions.</summary>
	public class ContentImage : ContentFile {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Conscructs the content image.</summary>
		public ContentImage(string name) :
			base(name)
		{
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
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of the content file.</summary>
		public override ContentTypes ContentType {
			get { return ContentTypes.Image; }
		}

		/// <summary>Gets if the content file should be compiled.</summary>
		public override bool ShouldCompile {
			get {
				return (XmlInfo.Importer == "TextureImporter" &&
					XmlInfo.Processor == "TextureProcessor");
			}
		}
	}
}
