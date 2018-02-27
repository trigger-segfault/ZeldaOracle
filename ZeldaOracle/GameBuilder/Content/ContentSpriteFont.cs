using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracleBuilder.Content {
	/// <summary>A content file representing .spritefont extension.</summary>
	public class ContentSpriteFont : ContentFile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Conscructs the content sprite font.</summary>
		public ContentSpriteFont(string name) :
			base(name)
		{
			XmlInfo.ElementName = "Compile";
			XmlInfo.Importer = "FontDescriptionImporter";
			XmlInfo.Processor = "FontDescriptionProcessor";
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of the content file.</summary>
		public override ContentTypes ContentType {
			get { return ContentTypes.SpriteFont; }
		}
	}
}
