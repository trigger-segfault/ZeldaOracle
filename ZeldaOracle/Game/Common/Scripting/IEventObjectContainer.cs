using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>An object contianing objects with Zelda properties and events.</summary>
	public interface IEventObjectContainer : IPropertyObjectContainer {

		/// <summary>Gets the collection of objects containing Zelda properties and events including this one.</summary>
		IEnumerable<IEventObject> GetEventObjects();
	}
}
