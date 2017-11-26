using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Scripting {
	/**<summary>An object contianing objects with Zelda properties.</summary>*/
	public interface IPropertyObjectContainer : IPropertyObject {

		/**<summary>Gets the collection of objects containing Zelda properties including this one.</summary>*/
		IEnumerable<IPropertyObject> GetPropertyObjects();
	}
}
