using ConscriptDesigner.Control;

namespace ConscriptDesigner.Content {
	/// <summary>A content file representing .fx extension.</summary>
	public class ContentShader : ContentFile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Conscructs the content shader.</summary>
		public ContentShader(string name) :
			base(name)
		{
			TreeViewItem.Source = DesignerImages.ShaderFile;
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
