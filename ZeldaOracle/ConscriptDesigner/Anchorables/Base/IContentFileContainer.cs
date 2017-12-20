using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConscriptDesigner.Content;

namespace ConscriptDesigner.Anchorables {
	/// <summary>An interface for classes that contain a content file.</summary>
	public interface IContentFileContainer {

		/// <summary>Gets the content file of the container.</summary>
		ContentFile File { get; }
	}
}
