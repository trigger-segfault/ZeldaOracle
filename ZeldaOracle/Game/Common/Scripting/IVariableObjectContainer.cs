using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>An object contianing objects with Zelda variables.</summary>
	public interface IVariableObjectContainer {

		/// <summary>Gets the collection of objects containing Zelda
		/// variables including this one.</summary>
		IEnumerable<IVariableObject> GetVariableObjects();
	}
}
