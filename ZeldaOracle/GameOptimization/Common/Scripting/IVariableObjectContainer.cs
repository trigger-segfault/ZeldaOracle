using System.Collections.Generic;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>An object contianing objects with Zelda variables.</summary>
	public interface IVariableObjectContainer {

		/// <summary>Gets the collection of objects containing Zelda
		/// variables including this one.</summary>
		IEnumerable<IVariableObject> GetVariableObjects();
	}
}
