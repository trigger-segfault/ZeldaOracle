using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>An object contianing a collection of Zelda properties.</summary>
	public interface IPropertyObject {

		/// <summary>Gets the collection of Zelda properties.</summary>
		Properties Properties { get; }
	}
	/// <summary>An object contianing a collection of Zelda properties.</summary>
	public interface IPropertyObject2 {

		/// <summary>Gets the collection of Zelda properties.</summary>
		Properties2 Properties { get; }
	}
}
