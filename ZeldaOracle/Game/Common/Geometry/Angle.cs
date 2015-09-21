using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Geometry {

	public static class Angles {

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
		
		// Return the given direction flipped horizontally over the y-axis.
		public static int FlipHorizontal(int angle) {
			return (Angles.West + Angles.AngleCount - angle) % Angles.AngleCount;
		}
		
		// Return the given direction flipped vertically over the x-axis.
		public static int FlipVertical(int angle) {
			return (AngleCount - angle) % AngleCount;
		}

		// Return a normalized vector representing the given angle.
		public static Vector2F ToVector(int angle) {
			Vector2F vec = Vector2F.Zero;
			if (angle % 8 == SouthEast || angle % 8 == East || angle % 8 == NorthEast)
				vec.X = 1;
			else if (angle % 8 >= NorthWest && angle % 8 <= SouthWest)
				vec.X = -1;
			if (angle % 8 >= NorthEast && angle % 8 <= NorthWest)
				vec.Y = -1;
			else if (angle % 8 >= SouthWest && angle % 8 <= SouthEast)
				vec.Y = 1;
			return vec.Normalized;
		}

		public static int CombineAxisDirections(int directionH, int directionV) {
			if (directionH == Directions.Left)
				return (directionV == Directions.Up ? Angles.UpLeft : Angles.DownLeft);
			return (directionV == Directions.Up ? Angles.UpRight : Angles.DownRight);
		}
	}
}
