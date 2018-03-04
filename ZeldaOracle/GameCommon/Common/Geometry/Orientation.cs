using System;

namespace ZeldaOracle.Common.Geometry {

	/// <summary>Struct used to represent an axis-aligned or diagonal angle.</summary>
	[Serializable]
	public struct Orientation {
		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The index of the orientation, from 0 to 7. 0 is right, 1 is up-right, 2
		/// is up, etc.. Angles increase counter-clockwise. If this value is not
		/// between 0 and 7, then the angle is considered invalid.</summary>
		private int index;

		private int count;
		

		//-----------------------------------------------------------------------------
		// Constructors / Factory Functions
		//-----------------------------------------------------------------------------
		
		/// <summary>Create an orientation struct with the given orientation 'index'
		/// out of a possible 'count' of orientations.</summary>
		public Orientation(int index, int count) {
			this.index = index;
			this.count = count;
		}

		/// <summary>Return the nearest angle from a vector.</summary>
		public static Orientation FromVector(Vector2F vector, int orientationCount) {
			return FromRadians(GMath.Atan2(-vector.Y, vector.X), orientationCount);
		}

		/// <summary>Return the nearest angle from a point.</summary>
		public static Orientation FromPoint(Point2I point, int orientationCount) {
			return FromRadians(GMath.Atan2(
				(float) -point.Y, (float) point.X), orientationCount);
		}
		
		/// <summary>Return the nearest angle from an angle in radians.</summary>
		public static Orientation FromRadians(float radians, int orientationCount) {
			return new Orientation(
				GMath.Wrap((int) GMath.Round(
					(radians * orientationCount) / GMath.TwoPi),
					orientationCount),
				orientationCount);
		}
		

		//-----------------------------------------------------------------------------
		// Unary Operations
		//-----------------------------------------------------------------------------

		/// <summary>Return the opposite angle.</summary>
		public Orientation Reverse() {
			if (count % 2 == 0)
				return new Orientation(index + (count / 2) % count, count);
			else
				return Invalid;
		}

		/// <summary>Return the angle flipped horizontally over the y-axis.
		/// </summary>
		public Orientation FlipHorizontal() {
			if (count % 2 == 0)
				return new Orientation((((count * 3) / 2) - index) % count, count);
			else
				return Invalid;
		}
		
		/// <summary>Return the angle flipped vertically over the x-axis.
		/// </summary>
		public Orientation FlipVertical() {
			return new Orientation((count - index) % count, count);
		}

		
		//-----------------------------------------------------------------------------
		// Static Unary Operations
		//-----------------------------------------------------------------------------

		/// <summary>Return the orientation as is (this operator does nothing).
		/// </summary>
		public static Orientation operator +(Orientation a) {
			return a;
		}

		/// <summary>Return the opposite orientation.</summary>
		public static Orientation operator -(Orientation a) {
			return a.Reverse();
		}

		/// <summary>Rotate the orientation once, clockwise.</summary>
		public static Orientation operator ++(Orientation a) {
			return new Orientation((a.index + 1) % a.count, a.count);
		}

		/// <summary>Rotate the orientation once, counter-clockwise.</summary>
		public static Orientation operator --(Orientation a) {
			return new Orientation((a.index + 1) % a.count, a.count);
		}
		
		
		//-----------------------------------------------------------------------------
		// Binary Operations
		//-----------------------------------------------------------------------------

		/// <summary>Rotate the orientation either clockwise or counter-clockwise.
		/// </summary>
		public Orientation Rotate(int amount, WindingOrder windingOrder) {
			if (windingOrder == WindingOrder.Clockwise)
				amount = -amount;
			return new Orientation(GMath.Wrap(index + amount, count), count);
		}
		
		/*
		/// <summary>Return the nearest distance between this angle and another. A
		/// positive distance is counter-clockwise while a negative distance is
		/// clockwise.</summary>
		public int NearestDistanceTo(Angle other) {
			return Angle.NearestDistance(this, other);
		}
		
		/// <summary>Return the nearest distance between this angle and another.
		/// This will return a positive distance, and set the winding order output
		/// variable to direction-of-rotation to rotate that distance to get to the
		/// destination angle.</summary>
		public int NearestDistanceTo(Angle other, out WindingOrder windingOrder) {
			int signedDistance = Angle.NearestDistance(this, other);
			if (signedDistance < 0) {
				windingOrder = WindingOrder.Clockwise;
				return -signedDistance;
			}
			else {
				windingOrder = WindingOrder.CounterClockwise;
				return signedDistance;
			}
		}
		
		/// <summary>Return the distance from one angle to another when traveling in
		/// the given winding order. The result will always be positive.</summary>
		public int DistanceTo(Angle other, WindingOrder windingOrder) {
			return Angle.Distance(this, other, windingOrder);
		}
		*/
		

		//-----------------------------------------------------------------------------
		// Static Binary Operations
		//-----------------------------------------------------------------------------

		/// <summary>Rotate the orientation counter-clockwise by the given amount.
		/// </summary>
		public static Orientation operator +(Orientation orientation, int amount) {
			return orientation.Rotate(amount, WindingOrder.CounterClockwise);
		}
		
		/// <summary>Rotate the orientation clockwise by the given amount.</summary>
		public static Orientation operator -(Orientation orientation, int amount) {
			return orientation.Rotate(amount, WindingOrder.Clockwise);
		}
		
		/*
		/// <summary>Return the nearest distance between two angles. A positive
		/// distance is counter-clockwise while a negative distance is clockwise.
		/// </summary>
		public static int NearestDistance(Angle from, Angle to) {
			int clockwiseDistance = Angle.Distance(
				from, to, WindingOrder.Clockwise);
			int counterClockwiseDistance = Angle.Distance(
				from, to, WindingOrder.CounterClockwise);
			if (clockwiseDistance < counterClockwiseDistance)
				return -clockwiseDistance;
			else
				return counterClockwiseDistance;
		}

		/// <summary>Return the distance from one angle to another when traveling in
		/// the given winding order. The result will always be positive.</summary>
		public static int Distance(Angle from, Angle to,
			WindingOrder windingOrder)
		{
			if (windingOrder == WindingOrder.Clockwise) {
				if (to.index > from.index)
					return (8 + from.index - to.index) % 8;
				else
					return (from.index - to.index);
			}
			else {
				if (from.index > to.index)
					return (8 + to.index - from.index) % 8;
				else
					return (to.index - from.index);
			}
		}
		*/

		
		//-----------------------------------------------------------------------------
		// Implicit Conversions
		//-----------------------------------------------------------------------------

		/// <summary>Return the string representation of the angle (right, up,
		/// left, or down).</summary>
		public override string ToString() {
			if (index == 0)
				return "east";
			else if (index == 1)
				return "northeast";
			else if (index == 2)
				return "north";
			else if (index == 3)
				return "northwest";
			else if (index == 4)
				return "west";
			else if (index == 5)
				return "southwest";
			else if (index == 6)
				return "south";
			else if (index == 7)
				return "southeast";
			return "invalid";
		}

		
		//-----------------------------------------------------------------------------
		// Explicit Conversions
		//-----------------------------------------------------------------------------

		/// <summary>Convert an orientation to an integer.</summary>
		public static explicit operator int(Orientation orientation) {
			return orientation.index;
		}

		/// <summary>Convert the orientation to a direction. If the orientation is not
		/// axis aligned, then this will return an invalid direction.</summary>
		public Direction ToDirection() {
			if ((index * Direction.Count) % count == 0)
				return new Direction((index * Direction.Count) / count);
			else
				return Direction.Invalid;
		}

		/// <summary>Convert the orientation to an angle. If the orientation is not
		/// axis-aligned or diagonal, then this will return an invalid direction.
		/// </summary>
		public Angle ToAngle() {
			if ((index * Angle.Count) % count == 0)
				return new Angle((index * Angle.Count) / count);
			else
				return Angle.Invalid;
		}

		/// <summary>Return the orientation's angle in radians.</summary>
		public float ToRadians() {
			return (index * GMath.TwoPi) / count;
		}

		/// <summary>Return the orientation as a polar vector with the given magnitude.
		/// </summary>
		public Vector2F ToVector(float magnitude = 1.0f) {
			float radians = ToRadians();
			return new Vector2F(GMath.Cos(radians), -GMath.Sin(radians)) * magnitude;
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Return the orientation index.</summary>
		public int Index {
			get { return index; }
		}

		/// <summary>Return the number of possible orientations.</summary>
		public int Count {
			get { return count; }
		}

		/// <summary>Return true if this is a valid orientation.</summary>
		public bool IsValid {
			get { return (index >= 0 && index < count); }
		}

		/// <summary>Return true if the orientation is horizontal (left or right).
		/// </summary>
		public bool IsHorizontal {
			get {
				return (index == 0 || index * 2 == count);
			}
		}

		/// <summary>Return true if the orientation is vertical (up or down).</summary>
		public bool IsVertical {
			get { return (index * 2 == count || index * 4 == count * 3); }
		}

		/// <summary>Return true if the orientation is axis-aligned.</summary>
		public bool IsAxisAligned {
			get { return ((index * Direction.Count) % count == 0); }
		}

		/// <summary>Return the invalid orientation.</summary>
		public Orientation Invalid {
			get { return new Orientation(-1, count); }
		}
	}
}
