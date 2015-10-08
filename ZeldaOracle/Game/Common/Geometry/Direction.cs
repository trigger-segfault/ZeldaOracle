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
		
		// Return the given direction flipped horizontally over the y-axis.
		public static int FlipHorizontal(int direction) {
			return (Directions.West + Directions.Count - direction) % Directions.Count;
		}
		
		// Return the given direction flipped vertically over the x-axis.
		public static int FlipVertical(int direction) {
			return (Directions.Count - direction) % Directions.Count;
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

		public static Vector2F ToVector(int direction) {
			direction = direction % 4;
			if (direction == Right)
				return new Vector2F(1.0f, 0.0f);
			else if (direction == Up)
				return new Vector2F(0.0f, -1.0f);
			else if (direction == Left)
				return new Vector2F(-1.0f, 0.0f);
			return new Vector2F(0.0f, 1.0f);
		}

		public static int ToAngle(int direction) {
			return (direction * 2);
		}

		public static int RoundFromRadians(float radians) {
			float halfPi = (float) Math.PI * 0.5f;
			int dir = (int) Math.Round(radians / halfPi);
			return GMath.Wrap(dir, Directions.Count);
		}
	}
}
