using System;
using ZeldaOracle.Common.Scripting.Internal;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>The possible raw data types for some sort of variable.</summary>
	[Serializable]
	public enum VarType : short {
		/// <summary>Currently unsupported.</summary>
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
		Color,
	}

	/// <summary>The list types for use with var types.</summary>
	[Serializable]
	public enum ListType : short {
		Single = 0,
		Array,
		List,
	}

	/// <summary>Extensions for the var type and list type enums.</summary>
	public static class VarExtensions {

		/// <summary>Convert a VarType to a System.Type.</summary>
		public static Type ToType(this VarType varType) {
			return VarBase.VarTypeToType(varType);
		}

		/// <summary>Convert a VarType to a System.Type.</summary>
		public static Type ToType(this VarType varType, ListType listType) {
			return VarBase.VarTypeToType(varType, listType);
		}
	}
}
