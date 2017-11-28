using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>An enumeration for different alignments.</summary>
	[Flags]
	public enum Align {

		Center		= 0,
		Left		= 1,
		Right		= 2,
		Top			= 4,
		Bottom		= 8,
		TopLeft		= Top | Left,
		TopRight	= Top | Right,
		BottomLeft	= Bottom | Left,
		BottomRight	= Bottom | Right,

		Int = 16

	}
} // end namespace
