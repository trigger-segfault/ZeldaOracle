using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracleBuilder.Content {
	/// <summary>A content file representing .fx extension.</summary>
	public class ContentShader : ContentFile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Conscructs the content shader.</summary>
		public ContentShader(string name) :
			base(name)
		{
			XmlInfo.ElementName = "Compile";
			XmlInfo.Importer = "EffectImporter";
			XmlInfo.Processor = "EffectProcessor";
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the type of the content file.</summary>
		public override ContentTypes ContentType {
			get { return ContentTypes.Shader; }
		}
	}
}
