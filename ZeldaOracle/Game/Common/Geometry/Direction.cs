using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Geometry {
	
	/*[Flags]
	public enum DirectionMask {
		None	= 0x0,
		Right	= 0x1,
		Up		= 0x2,
		Left	= 0x4,
		Down	= 0x8,
	}*/
	
	// Ideas for a smarter orientation method
	/*
	public enum DirectionEnum {
		Right	= 0,
		Up		= 1,
		Left	= 2,
		Down	= 3,
		East	= 0,
		North	= 1,
		West	= 2,
		South	= 3,
	}

	public enum AngleEnum {
		Right	= 0,
		Up		= 1,
		Left	= 2,
		Down	= 3,
		East	= 0,
		North	= 1,
		West	= 2,
		South	= 3,
	}

	public struct Orientation {
		private int index;
		private int count;

		public Orientation(int index, int count) {
			this.index = index;
			this.count = count;
		}

		//public static implicit operator Orientation(int index) {
			//return new Orientation(index, 4);
		//}
		public static implicit operator Orientation(DirectionEnum direction) {
			return new Orientation((int) direction, 4);
		}
		public static implicit operator Orientation(AngleEnum angle) {
			return new Orientation((int) angle, 8);
		}

		// Return a unit vector in the given direction.
		public Vector2F ToVector() {
			float radians = (index / (float) count) * GMath.DoublePi;
			return new Vector2F(GMath.Cos(radians), -GMath.Sin(radians));
		}

		// Return the given direction flipped horizontally over the y-axis.
		public void FlipHorizontal() {
			int west = ((count * 4) / 3);
			index = (west + count - index) % count;
		}
		
		// Return the given direction flipped vertically over the x-axis.
		public void FlipVertical() {
			index = (count - index) % count;
		}
	}
	*/

	public enum OrientationStyle {
		None,		// Not-applicable
		Direction,	// 4 directions
		Angle,		// 8 angles
	};

	public static class Orientations {
		

		// Return a unit vector in the given direction.
		public static Vector2F ToVector(int orientation, OrientationStyle style) {
			if (style == OrientationStyle.Direction)
				return Directions.ToVector(orientation);
			else if (style == OrientationStyle.Angle)
				return Angles.ToVector(orientation);
			return Vector2F.Zero;
		}

		public static Vector2F ToVector(int angle, int numAngles) {
			float radians = (angle / (float) numAngles) * GMath.FullAngle;
			return Vector2F.FromPolar(radians);
		}

		public static int RoundFromRadians(float radians, int numAngles) {
			int angle = (int) GMath.Round((radians * numAngles) / GMath.FullAngle);
			return GMath.Wrap(angle, numAngles);
		}

		public static int NearestFromVector(Vector2F vector, int numAngles) {
			float radians = GMath.Atan2(-vector.Y, vector.X);
			return RoundFromRadians(radians, numAngles);
		}
		
		public static int Add(int angle, int addAmount, WindingOrder windingOrder, int numAngles) {
			if (windingOrder == WindingOrder.Clockwise)
				angle -= addAmount;
			else
				angle += addAmount;
			return GMath.Wrap(angle, numAngles);
		}
		
		public static int Subtract(int angle, int subtractAmount,
			WindingOrder windingOrder, int numAngles)
		{
			if (windingOrder == WindingOrder.Clockwise)
				angle += subtractAmount;
			else
				angle -= subtractAmount;
			return GMath.Wrap(angle, numAngles);
		}

		public static int Reverse(int angle, int numAngles) {
			return ((angle + (numAngles / 2)) % numAngles);
		}
		
		public static int GetNearestAngleDistance(int startAngle, int endAngle, int numAngles) {
			int cwDist = GetAngleDistance(startAngle, endAngle, WindingOrder.Clockwise, numAngles);
			int ccwDist = GetAngleDistance(startAngle, endAngle, WindingOrder.CounterClockwise, numAngles);
			if (cwDist < ccwDist)
				return -cwDist;
			else
				return ccwDist;
		}

		/// <summary>Return the modular distance from one angle to another using
		/// the given winding order.</summary>
		public static int GetAngleDistance(int startAngle, int endAngle,
			WindingOrder windingOrder, int numAngles)
		{
			startAngle = GMath.Wrap(startAngle, numAngles);
			endAngle = GMath.Wrap(endAngle, numAngles);
			if (windingOrder == WindingOrder.Clockwise) {
				if (endAngle > startAngle)
					return (startAngle + numAngles - endAngle);
				else
					return (startAngle - endAngle);
			}
			else {
				if (endAngle < startAngle)
					return (endAngle + numAngles - startAngle);
				else
					return (endAngle - startAngle);
			}
		}
	}


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
		
		public static int Add(int direction, int addAmount, WindingOrder windingOrder) {
			if (windingOrder == WindingOrder.Clockwise)
				direction -= addAmount;
			else
				direction += addAmount;
			return GMath.Wrap(direction, Directions.Count);
		}

		
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

		/// <summary>Return the given direction flipped horizontally over the x and y axis.</summary>
		public static int Flip(int direction) {
			if (IsHorizontal(direction))
				return FlipHorizontal(direction);
			else if (IsVertical(direction))
				return FlipVertical(direction);
			return direction;
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
				if (vector.X >= GMath.Abs(vector.Y))
					return Directions.Right;
				else if (vector.Y < 0)
					return Directions.Up;
				else
					return Directions.Down;
			}
			else {
				if (-vector.X >= GMath.Abs(vector.Y))
					return Directions.Left;
				else if (vector.Y < 0)
					return Directions.Up;
				else
					return Directions.Down;
			}
		}

		public static int RoundFromRadians(float radians) {
			int dir = (int) GMath.Round(radians / GMath.QuarterAngle);
			return GMath.Wrap(dir, Directions.Count);
		}

		public static string ToString(int direction) {
			if (direction == Directions.Right)
				return "right";
			if (direction == Directions.Left)
				return "left";
			if (direction == Directions.Up)
				return "up";
			if (direction == Directions.Down)
				return "down";
			return "error";
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
		
		/*public static DirectionMask GetDirectionBit(int direction) {
			if (direction >= 0 && direction < Count)
				return (DirectionMask) (1 << direction);
			return DirectionMask.None;
		}*/
	}
}
