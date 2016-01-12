using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Common.Scripting {

	// An object that contains Zelda properties.
	public interface IEventObject {

		ObjectEventCollection Events { get; }
	}
}
