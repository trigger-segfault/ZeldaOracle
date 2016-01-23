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

		
		// Return the opposite of the given direction.
		public static int Reverse(int direction) {
			return ((direction + 2) % 4);
		}
		
		// Return true if the given direction is horizontal (left or right).
		public static bool IsHorizontal(int direction) {
			return (direction % 2 == 0);
		}
		
		// Return true if the given direction is vertical (up or down).
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
		
		// Return a unit vector as a point in the given direction.
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

		// Return a unit vector in the given direction.
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

		// Return the angle representation of the given direction.
		public static int ToAngle(int direction) {
			return (direction * 2);
		}

		// Return the axis that the given direction is aligned on.
		public static int ToAxis(int direction) {
			return (direction % 2);
		}

		public static int FromPoint(Point2I point) {
			if (point.X > 0)
				return Directions.Right;
			if (point.Y < 0)
				return Directions.Up;
			if (point.X < 0)
				return Directions.Left;
			if (point.Y > 0)
				return Directions.Down;
			return -1;
		}
		
		public static int NearestFromVector(Vector2F vector) {
			// Cheap algorithm for turning a vector into an axis-aligned direction.
			if (vector.X > 0) {
				if (vector.X >= Math.Abs(vector.Y))
					return Directions.Right;
				else if (vector.Y < 0)
					return Directions.Up;
				else
					return Directions.Down;
			}
			else {
				if (-vector.X >= Math.Abs(vector.Y))
					return Directions.Left;
				else if (vector.Y < 0)
					return Directions.Up;
				else
					return Directions.Down;
			}
		}

		public static int RoundFromRadians(float radians) {
			int dir = (int) Math.Round(radians / GMath.HalfPi);
			return GMath.Wrap(dir, Directions.Count);
		}
		
		public static bool TryParse(string value, bool ignoreCase, out int result) {
			if (ignoreCase)
				value = value.ToLower();
			if (value == "right" || value == "east")
				result = Directions.Right;
			else if (value == "left" || value == "west")
				result = Directions.Left;
			else if (value == "up" || value == "north")
				result = Directions.Up;
			else if (value == "down" || value == "south")
				result = Directions.Down;
			else {
				result = -1;
				return false;
			}
			return true;
		}
	}
}
