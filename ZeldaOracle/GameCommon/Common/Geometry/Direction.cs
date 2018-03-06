using System;
using System.Collections.Generic;

namespace ZeldaOracle.Common.Geometry {

	/// <summary>The direction of rotation, either Clockwise or Counter-Clockwise. Note
	/// that angles in this game increase in the clockwise direction, where zero
	/// represents right.
	/// </summary>
	public enum WindingOrder {
		CounterClockwise = 0,
		Clockwise = 1,
	}


	/// <summary>Struct used to represent an axis-aligned direction.</summary>
	[Serializable]
	public struct Direction {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The direction pointing right (east).</summary>
		public static readonly Direction Right = new Direction(0);
		/// <summary>The direction pointing up (north).</summary>
		public static readonly Direction Up = new Direction(1);
		/// <summary>The direction pointing left (west).</summary>
		public static readonly Direction Left = new Direction(2);
		/// <summary>The direction pointing down (south).</summary>
		public static readonly Direction Down = new Direction(3);

		/// <summary>The direction pointing right (east).</summary>
		public static readonly Direction East = new Direction(0);
		/// <summary>The direction pointing up (north).</summary>
		public static readonly Direction North = new Direction(1);
		/// <summary>The direction pointing left (west).</summary>
		public static readonly Direction West = new Direction(2);
		/// <summary>The direction pointing down (south).</summary>
		public static readonly Direction South = new Direction(3);
		
		/// <summary>An invalid direction.</summary>
		public static readonly Direction Invalid = new Direction(-1);

		/// <summary>The total number of unique directions.</summary>
		public const int Count = 4;

		/// <summary>Iterate all four directions.</summary>
		public static IEnumerable<Direction> Range {
			get {
				for (int index = 0; index < Direction.Count; index++)
					yield return new Direction(index);
			}
		}
		

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The index of the direction, from 0 to 3. 0 is right, 1 is up, 2
		/// is left, and 3 is down. Directions increase counter-clockwise. If this
		/// value is not between 0 and 3, then the direction is considered invalid.
		/// </summary>
		private int index;
		

		//-----------------------------------------------------------------------------
		// Constructors / Factory Functions
		//-----------------------------------------------------------------------------

		/// <summary>Create a direction from an index (0 to 3).</summary>
		public Direction(int index) {
			this.index = index;
		}

		/// <summary>Return the nearest axis-aligned direction from a vector.</summary>
		public static Direction FromVector(Vector2F vector) {
			// Cheap algorithm for turning a vector into an axis-aligned direction
			if (GMath.Abs(vector.X) >= GMath.Abs(vector.Y)) {
				if (vector.X < 0)
					return Direction.Left;
				else if (vector.X > 0)
					return Direction.Right;
				else
					return Direction.Invalid; // Zero-length vector
			}
			else if (vector.Y < 0)
				return Direction.Up;
			else
				return Direction.Down;
		}

		/// <summary>Return the nearest axis-aligned direction from a point.</summary>
		public static Direction FromPoint(Point2I point) {
			if (GMath.Abs(point.X) >= GMath.Abs(point.Y)) {
				if (point.X < 0)
					return Direction.Left;
				else if (point.X > 0)
					return Direction.Right;
				else
					return Direction.Invalid; // Zero-length point
			}
			else if (point.Y < 0)
				return Direction.Up;
			else
				return Direction.Down;
		}
		
		/// <summary>Return the nearest axis-aligned direction from an angle in
		/// radians.</summary>
		public static Direction FromRadians(float radians) {
			return new Direction(GMath.Wrap(
				(int) GMath.Round(radians / GMath.QuarterPi), 4));
		}
		

		//-----------------------------------------------------------------------------
		// Unary Operations
		//-----------------------------------------------------------------------------

		/// <summary>Return the opposite direction.</summary>
		public Direction Reverse() {
			return new Direction((index + 2) % 4);
		}

		/// <summary>Return the direction flipped horizontally over the y-axis.
		/// </summary>
		public Direction FlipHorizontal() {
			return new Direction((6 - index) % 4);
		}
		
		/// <summary>Return the direction flipped vertically over the x-axis.
		/// </summary>
		public Direction FlipVertical() {
			return new Direction((4 - index) % 4);
		}

		
		//-----------------------------------------------------------------------------
		// Static Unary Operations
		//-----------------------------------------------------------------------------

		/// <summary>Return the direction as is (this operator does nothing).</summary>
		public static Direction operator +(Direction a) {
			return a;
		}

		/// <summary>Return the opposite direction.</summary>
		public static Direction operator -(Direction a) {
			return a.Reverse();
		}

		/// <summary>Rotate the direction once, clockwise.</summary>
		public static Direction operator ++(Direction a) {
			return new Direction((a.index + 1) % 4);
		}

		/// <summary>Rotate the direction once, counter-clockwise.</summary>
		public static Direction operator --(Direction a) {
			return new Direction((a.index + 3) % 4);
		}
		
		
		//-----------------------------------------------------------------------------
		// Binary Operations
		//-----------------------------------------------------------------------------

		/// <summary>Rotate the direction either clockwise or counter-clockwise.
		/// </summary>
		public Direction Rotate(int amount,
			WindingOrder windingOrder = WindingOrder.CounterClockwise)
		{
			if (windingOrder == WindingOrder.Clockwise)
				amount = -amount;
			return new Direction(GMath.Wrap(index + amount, 4));
		}
		
		/// <summary>Return the nearest distance between this direction and another. A
		/// positive distance is counter-clockwise while a negative distance is
		/// clockwise.</summary>
		public int NearestDistanceTo(Direction other) {
			return Direction.NearestDistance(this, other);
		}
		
		/// <summary>Return the distance from one angle to another when traveling in
		/// the given winding order. The result will always be positive.</summary>
		public int DistanceTo(Direction other, WindingOrder windingOrder) {
			return Direction.Distance(this, other, windingOrder);
		}
		

		//-----------------------------------------------------------------------------
		// Static Binary Operations
		//-----------------------------------------------------------------------------

		/// <summary>Rotate the direction counter-clockwise by the given amount.
		/// </summary>
		public static Direction operator +(Direction direction, int amount) {
			return direction.Rotate(amount, WindingOrder.CounterClockwise);
		}
		
		/// <summary>Rotate the direction clockwise by the given amount.</summary>
		public static Direction operator -(Direction direction, int amount) {
			return direction.Rotate(amount, WindingOrder.Clockwise);
		}
		
		/// <summary>Return the nearest distance between two directions. A positive
		/// distance is counter-clockwise while a negative distance is clockwise.
		/// </summary>
		public static int NearestDistance(Direction from, Direction to) {
			int clockwiseDistance = Direction.Distance(
				from, to, WindingOrder.Clockwise);
			int counterClockwiseDistance = Direction.Distance(
				from, to, WindingOrder.CounterClockwise);
			if (clockwiseDistance < counterClockwiseDistance)
				return -clockwiseDistance;
			else
				return counterClockwiseDistance;
		}

		/// <summary>Return the distance from one angle to another when traveling in
		/// the given winding order. The result will always be positive.</summary>
		public static int Distance(Direction from, Direction to,
			WindingOrder windingOrder)
		{
			if (windingOrder == WindingOrder.Clockwise) {
				if (from.index > to.index)
					return (4 + from.index - to.index);
				else
					return (from.index - to.index);
			}
			else {
				if (to.index < from.index)
					return (4 + to.index - from.index);
				else
					return (to.index - from.index);
			}
		}

		
		//-----------------------------------------------------------------------------
		// Implicit Conversions
		//-----------------------------------------------------------------------------

		/// <summary>Convert an integer to a direction.</summary>
		public static implicit operator Direction(int index) {
			return new Direction(index);
		}

		/// <summary>Convert a direction to an integer.</summary>
		public static implicit operator int(Direction direction) {
			return direction.index;
		}

		/// <summary>Return the string representation of the direction (right, up,
		/// left, or down).</summary>
		public override string ToString() {
			if (index == 0)
				return "right";
			if (index == 1)
				return "up";
			if (index == 2)
				return "left";
			if (index == 3)
				return "down";
			return "invalid";
		}

		
		//-----------------------------------------------------------------------------
		// Explicit Conversions
		//-----------------------------------------------------------------------------

		/// <summary>Return an Angle struct that represents this direction.</summary>
		public Angle ToAngle() {
			return new Angle(index * 2);
		}

		/// <summary>Return the direction's angle in radians.</summary>
		public float ToRadians() {
			return (index * GMath.HalfPi);
		}

		/// <summary>Return the direction as a polar vector with the given magnitude.
		/// </summary>
		public Vector2F ToVector(float magnitude = 1.0f) {
			if (index == 0)
				return new Vector2F(magnitude, 0); // right
			else if (index == 1)
				return new Vector2F(0, -magnitude); // up
			else if (index == 2)
				return new Vector2F(-magnitude, 0); // left
			else if (index == 3)
				return new Vector2F(0, magnitude); // down
			else
				return Vector2F.Zero; // invalid
		}

		/// <summary>Return the direction as a point with the given magnitude.
		/// </summary>
		public Point2I ToPoint(int magnitude = 1) {
			if (index == 0)
				return new Point2I(magnitude, 0); // right
			else if (index == 1)
				return new Point2I(0, -magnitude); // up
			else if (index == 2)
				return new Point2I(-magnitude, 0); // left
			else if (index == 3)
				return new Point2I(0, magnitude); // down
			else
				return Point2I.Zero; // invalid
		}
		
		/// <summary>Try to parse a direction from a string. Returns true if the parse
		/// was successful.</summary>
		public static bool TryParse(string value, bool ignoreCase,
			out Direction result)
		{
			if (ignoreCase)
				value = value.ToLower();
			if (value == "right" || value == "east")
				result = Direction.Right;
			else if (value == "left" || value == "west")
				result = Direction.Left;
			else if (value == "up" || value == "north")
				result = Direction.Up;
			else if (value == "down" || value == "south")
				result = Direction.Down;
			else {
				result = Direction.Invalid;
				return false;
			}
			return true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Return the index of the direction (0 to 3).</summary>
		public int Index {
			get { return index; }
		}

		/// <summary>Return true if this is a valid axis-aligned direction.</summary>
		public bool IsValid {
			get { return (index >= 0 && index < 4); }
		}

		/// <summary>Return the axis parallel with this direction (0 for X, 1 for Y).</summary>
		public int Axis {
			get {  return (index % 2); }
		}

		/// <summary>Return the axis perpendicular to this direction (0 for X, 1 for Y).</summary>
		public int PerpendicularAxis {
			get {  return (1 - (index % 2)); }
		}

		/// <summary>Return true if the direction is horizontal (left or right).
		/// </summary>
		public bool IsHorizontal {
			get { return ((index % 2) == 0); }
		}

		/// <summary>Return true if the direction is vertical (up or down).</summary>
		public bool IsVertical {
			get { return ((index % 2) == 1); }
		}
	}
	


	/*

	public struct Orientation {

		private int index;
		private int count;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Orientation(int index, int count) {
			this.index = index;
			this.count = count;
		}

		
		//-----------------------------------------------------------------------------
		// Implicit Conversions
		//-----------------------------------------------------------------------------

		public static implicit operator Orientation(int index) {
			return new Orientation(index, 4);
		}

		public static implicit operator int(Orientation orientation) {
			return orientation.index;
		}

		
		//-----------------------------------------------------------------------------
		// Unary Operators
		//-----------------------------------------------------------------------------

		public static Orientation operator -(Orientation a) {
			return a.Reverse();
		}
		

		//-----------------------------------------------------------------------------
		// Binary Operators
		//-----------------------------------------------------------------------------

		public static Orientation operator +(Orientation a, Orientation b) {
			return new Orientation((a.index + b.index) % a.count, a.count);
		}

		public static Orientation operator -(Orientation a, Orientation b) {
			return new Orientation(GMath.Wrap(a.index - b.index, a.count), a.count);
		}

		/// <summary>Return the orientation in radians.</summary>
		public int ToAxis() {
			if (index == 0 || index * 2 == count)
				return 0;
			else if (index * 4 == count || index * 4 == (count * 3))
				return 0;
			else
				return -1;
		}

		/// <summary>Return the orientation in radians.</summary>
		public float ToRadians() {
			return (index / (float) count) * GMath.TwoPi;
		}

		/// <summary>Return the orientation as a unit vector.</summary>
		public Vector2F ToVector() {
			float radians = ToRadians();
			return new Vector2F(GMath.Cos(radians), -GMath.Sin(radians));
		}

		/// <summary>Return the orientation flipped horizontally over the y-axis.
		/// </summary>
		public Orientation FlipHorizontal() {
			return new Orientation((((count * 3) / 2) - index) % count, count);
		}
		
		/// <summary>Return the orientation flipped vertically over the x-axis.
		/// </summary>
		public Orientation FlipVertical() {
			return new Orientation((count - index) % count, count);
		}

		public Orientation Reverse() {
			return new Orientation((index + (count / 2)) % count, count);
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
			return RoundFromRadians(vector.Direction, numAngles);
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
			return GMath.Wrap(direction, Direction.Count);
		}

		
		// Return the opposite of the given direction.
		public static int Reverse(int direction) {
			return ((direction + Direction.Count / 2) % Direction.Count);
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
			return (Directions.West + Direction.Count - direction) % Direction.Count;
		}
		
		// Return the given direction flipped vertically over the x-axis.
		public static int FlipVertical(int direction) {
			return (Direction.Count - direction) % Direction.Count;
		}
		
		// Return a unit vector as a point in the given direction.
		public static Point2I ToPoint(int direction) {
			direction = direction % Direction.Count;
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
			direction = direction % Direction.Count;
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
				return Direction.Right;
			if (point.Y < 0)
				return Direction.Up;
			if (point.X < 0)
				return Direction.Left;
			if (point.Y > 0)
				return Direction.Down;
			return -1;
		}
		
		public static int NearestFromVector(Vector2F vector) {
			// Cheap algorithm for turning a vector into an axis-aligned direction.
			if (GMath.Abs(vector.X) >= GMath.Abs(vector.Y)) {
				if (vector.X < 0)
					return Direction.Left;
				else if (vector.X > 0)
					return Direction.Right;
				else
					return -1;
			}
			else if (vector.Y < 0)
				return Direction.Up;
			else
				return Direction.Down;
		}

		public static int RoundFromRadians(float radians) {
			int dir = (int) GMath.Round(radians / GMath.QuarterAngle);
			return GMath.Wrap(dir, Direction.Count);
		}

		public static string ToString(int direction) {
			if (direction == Direction.Right)
				return "right";
			if (direction == Direction.Left)
				return "left";
			if (direction == Direction.Up)
				return "up";
			if (direction == Direction.Down)
				return "down";
			return "invalid";
		}
		
		public static bool TryParse(string value, bool ignoreCase, out int result) {
			if (ignoreCase)
				value = value.ToLower();
			if (value == "right" || value == "east")
				result = Direction.Right;
			else if (value == "left" || value == "west")
				result = Direction.Left;
			else if (value == "up" || value == "north")
				result = Direction.Up;
			else if (value == "down" || value == "south")
				result = Direction.Down;
			else {
				result = Direction.Invalid;
				return false;
			}
			return true;
		}

		public static bool TryParse(string value, bool ignoreCase, out Direction result) {
			if (ignoreCase)
				value = value.ToLower();
			if (value == "right" || value == "east")
				result = Direction.Right;
			else if (value == "left" || value == "west")
				result = Direction.Left;
			else if (value == "up" || value == "north")
				result = Direction.Up;
			else if (value == "down" || value == "south")
				result = Direction.Down;
			else {
				result = Direction.Invalid;
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
