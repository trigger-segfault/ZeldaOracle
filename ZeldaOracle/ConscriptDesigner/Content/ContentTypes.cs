using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConscriptDesigner.Content {
	/// <summary>The types of identifyable content files.</summary>
	public enum ContentTypes {
		/// <summary>Any undefined extension.</summary>
		Unknown = -1,
		/// <summary>No content type.</summary>
		None = 0,
		/// <summary>The root content project</summary>
		Project,
		/// <summary>A folder.</summary>
		Folder,
		/// <summary>Extensions: .conscript</summary>
		Conscript,
		/// <summary>Extensions: .png, .jpg, .gif</summary>
		Image,
		/// <summary>Extensions: .wav</summary>
		Sound,
		/// <summary>Extensions: .fx</summary>
		Shader,
		/// <summary>Extensions: .spritefont</summary>
		SpriteFont
	}
}
