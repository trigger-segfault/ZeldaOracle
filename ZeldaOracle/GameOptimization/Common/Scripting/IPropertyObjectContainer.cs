using System.Collections.Generic;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>An object contianing objects with Zelda properties.</summary>
	public interface IPropertyObjectContainer {

		/// <summary>Gets the collection of objects containing Zelda
		/// properties including this one.</summary>
		IEnumerable<IPropertyObject> GetPropertyObjects();
	}
}
