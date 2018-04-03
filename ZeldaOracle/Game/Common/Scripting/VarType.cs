using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>The possible raw data types for some sort of variable.</summary>
	[Serializable]
	public enum VarType : short {
		Custom = 0,
		String,
		Integer,
		Float,
		Boolean,
		Point,
		Vector,
		RangeI,
		RangeF,
		RectangleI,
		RectangleF,
	}

	/// <summary>The list types for use with var types.</summary>
	[Serializable]
	public enum ListType : short {
		Single = 0,
		Array,
		List,
	}
}
