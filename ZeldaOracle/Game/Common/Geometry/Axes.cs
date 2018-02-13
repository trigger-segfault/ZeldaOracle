using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>Enumeration methods for the X and Y axes.</summary>
	public static class Axes {

		/// <summary>The X axis.</summary>
		public const int X		= 0;
		/// <summary>The Y axis.</summary>
		public const int Y		= 1;
		/// <summary>The number of axes.</summary>
		public const int Count	= 2;


		/// <summary>Get the opposite of the given axis (X vs Y).</summary>
		public static int GetOpposite(int axis) {
			return (1 - axis);
		}
	}
}
