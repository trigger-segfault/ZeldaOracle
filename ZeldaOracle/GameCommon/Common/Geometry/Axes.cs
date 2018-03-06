using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>Enumeration methods for the X and Y axes.</summary>
	public static class Axes {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The X axis.</summary>
		public const int X		= 0;
		/// <summary>The Y axis.</summary>
		public const int Y		= 1;
		/// <summary>The number of axes.</summary>
		public const int Count	= 2;


		//-----------------------------------------------------------------------------
		// Functions
		//-----------------------------------------------------------------------------

		/// <summary>Get the opposite of the given axis (X vs Y).</summary>
		public static int GetOpposite(int axis) {
			return (1 - axis);
		}

		/// <summary>Get the direction from the origin to a position along an axis. For
		/// example, the direction to +1 along the X-Axis would be Direction.Right.
		/// </summary>
		public static Direction GetDirection(int axis, float positionOnAxis) {
			if (axis == Axes.X)
				return (positionOnAxis >= 0.0f ? Direction.Right : Direction.Left);
			else
				return (positionOnAxis >= 0.0f ? Direction.Down : Direction.Up);
		}

	}
}
