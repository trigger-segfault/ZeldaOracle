using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>An object contianing a collection of Zelda properties and events.</summary>
	public interface IEventObject : IPropertyObject {

		/// <summary>Gets the collection of Zelda events.</summary>
		EventCollection Events { get; }
	}
}
