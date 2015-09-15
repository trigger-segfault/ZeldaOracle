using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Geometry {


	public static class Directions {
		
		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		public const int Count	= 4;

		public const int Right	= 0;
		public const int Up		= 1;
		public const int Left	= 2;
		public const int Down	= 3;

		public const int East	= 0;
		public const int North	= 1;
		public const int West	= 2;
		public const int South	= 3;
		
		
		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------
		
		public static bool IsHorizontal(int direction) {
			return (direction % 2 == 0);
		}
		
		public static bool IsVertical(int direction) {
			return (direction % 2 == 1);
		}

		public static Point2I ToPoint(int direction) {
			direction = direction % 4;
			if (direction == Right)
				return new Point2I(1, 0);
			else if (direction == Up)
				return new Point2I(0, -1);
			else if (direction == Left)
				return new Point2I(-1, 0);
			return new Point2I(0, 1);
		}

		public static int ToAngle(int direction) {
			return (direction * 2);
		}
	}
}
