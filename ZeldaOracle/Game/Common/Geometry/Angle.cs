using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Geometry {

	public static class Angle {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		public const int AngleCount	= 8;
		
		public const int Right		= 0;
		public const int UpRight	= 1;
		public const int Up			= 2;
		public const int UpLeft		= 3;
		public const int Left		= 4;
		public const int DownLeft	= 5;
		public const int Down		= 6;
		public const int DownRight	= 7;
		
		public const int East		= 0;
		public const int NorthEast	= 1;
		public const int North		= 2;
		public const int NorthWest	= 3;
		public const int West		= 4;
		public const int SouthWest	= 5;
		public const int South		= 6;
		public const int SouthEast	= 7;


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		public static bool IsHorizontal(int angle) {
			return (angle % 4 == 0);
		}
		
		public static bool IsVertical(int angle) {
			return (angle % 4 == 2);
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
	}
}
