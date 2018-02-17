using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Geometry {

	// Possible Names:
	// - WindingOrder
	// - RotationDirection
	// - RotateDirection
	// - RotDirection
	// - TurnDirection
	// "rotation_direction"

	// Currently used in:
	// - Direction
	// - Angle
	// - GMath
	// - PlayerSwingState
	// - PlayerSpinSwordState
	// - PlayerSeedShooterState
	// - PlayerMagicBoomerangState
	// - RoomStateTurnstile
	// - TileTurnstile

	// The direction of rotation.
	public enum WindingOrder {
		CounterClockwise	= 0,
		Clockwise			= 1,
	}

	// Enumeration values and methods for integers representing one of eight
	// 45 degree angle intervals in a circle.
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
		
		public static int Add(int angle, int addAmount, WindingOrder windingOrder) {
			if (windingOrder == WindingOrder.Clockwise)
				angle -= addAmount;
			else
				angle += addAmount;
			return GMath.Wrap(angle, Angles.AngleCount);
		}
		
		public static int Subtract(int angle, int subtractAmount, WindingOrder windingOrder) {
			if (windingOrder == WindingOrder.Clockwise)
				angle += subtractAmount;
			else
				angle -= subtractAmount;
			return GMath.Wrap(angle, Angles.AngleCount);
		}

		public static int Reverse(int angle) {
			return ((angle + Angles.AngleCount / 2) % Angles.AngleCount);
		}

		// Return the modular distance from one angle to another using the given winding order.
		public static int GetAngleDistance(int startAngle, int endAngle, WindingOrder windingOrder) {
			if (windingOrder == WindingOrder.Clockwise) {
				if (endAngle > startAngle)
					return (startAngle + Angles.AngleCount - endAngle);
				else
					return (startAngle - endAngle);
			}
			else {
				if (endAngle < startAngle)
					return (endAngle + Angles.AngleCount - startAngle);
				else
					return (endAngle - startAngle);
			}
		}

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
			return (Angles.AngleCount - angle) % Angles.AngleCount;
		}

		// Return a normalized vector representing the given angle.
		public static Vector2F ToVector(int angle, bool normalize = true) {
			angle %= Angles.AngleCount;
			Vector2F vec = Vector2F.Zero;
			if (angle == SouthEast || angle == East || angle == NorthEast)
				vec.X = 1;
			else if (angle >= NorthWest && angle <= SouthWest)
				vec.X = -1;
			if (angle >= NorthEast && angle <= NorthWest)
				vec.Y = -1;
			else if (angle >= SouthWest && angle <= SouthEast)
				vec.Y = 1;
			return (normalize ? vec.Normalized : vec);
		}

		// Return a point the given angle.
		public static Point2I ToPoint(int angle) {
			angle %= Angles.AngleCount;
			Point2I point = Point2I.Zero;
			if (angle == SouthEast || angle == East || angle == NorthEast)
				point.X = 1;
			else if (angle >= NorthWest && angle <= SouthWest)
				point.X = -1;
			if (angle >= NorthEast && angle <= NorthWest)
				point.Y = -1;
			else if (angle >= SouthWest && angle <= SouthEast)
				point.Y = 1;
			return point;
		}

		public static int CombineAxisDirections(int directionH, int directionV) {
			if (directionH == Directions.Left)
				return (directionV == Directions.Up ? Angles.UpLeft : Angles.DownLeft);
			return (directionV == Directions.Up ? Angles.UpRight : Angles.DownRight);
		}
				
		public static int NearestFromVector(Vector2F vector) {
			return RoundFromRadians(vector.Direction);
		}

		public static int RoundFromRadians(float radians) {
			int angle = (int) GMath.Round(radians / GMath.EighthAngle);
			return GMath.Wrap(angle, Angles.AngleCount);
		}
		
		public static bool TryParse(string value, bool ignoreCase, out int result) {
			if (Directions.TryParse(value, ignoreCase, out result)) {
				result = Directions.ToAngle(result);
				return true;
			}
			if (ignoreCase)
				value = value.ToLower();
			if (value == "upright" || value == "rightup" || value == "northeast")
				result = Angles.NorthEast;
			else if (value == "upleft" || value == "leftup" || value == "northwest")
				result = Angles.NorthWest;
			else if (value == "downleft" || value == "leftdown" || value == "southwest")
				result = Angles.SouthWest;
			else if (value == "downright" || value == "rightdown" || value == "southeast")
				result = Angles.SouthEast;
			else {
				result = -1;
				return false;
			}
			return true;
		}
	}
}
