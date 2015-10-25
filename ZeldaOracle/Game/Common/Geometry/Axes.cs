using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Geometry {

	// Enumeration methods for the X and Y axes.
	public static class Axes {

		public const int X		= 0;
		public const int Y		= 1;
		public const int Count	= 2;
		

		// Get the opposite of the given axis (X vs Y).
		public static int GetOpposite(int axis) {
			return (1 - axis);
		}


	}
}
