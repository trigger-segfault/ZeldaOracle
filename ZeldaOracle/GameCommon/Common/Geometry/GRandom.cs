using System;
using System.Collections.Generic;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>A static class for advanced random number generation.</summary>
	public static class GRandom {
		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The class used to randomly generate numbers.</summary>
		private static Random random = new Random();


		//-----------------------------------------------------------------------------
		// Random
		//-----------------------------------------------------------------------------

		// Seed -----------------------------------------------------------------------

		/// <summary>Sets the new seed of the random number generator.</summary>
		public static void SetSeed(int seed) {
			random = new Random(seed);
		}

		// Bool -----------------------------------------------------------------------

		/// <summary>Returns true or false at random.</summary>
		public static bool NextBool() {
			return random.Next(2) == 1;
		}

		/// <summary>Returns true or false at random with the specified ratio of
		/// getting true.</summary>
		public static bool NextBool(float ratio) {
			return (ratio == 0f ? false : (float)random.NextDouble() <= ratio);
		}

		/// <summary>Returns true or false at random with the specified ratio of
		/// getting true.</summary>
		public static bool NextBool(double ratio) {
			return (ratio == 0d ? false : random.NextDouble() <= ratio);
		}

		// Data -----------------------------------------------------------------------

		/// <summary>Fills the array with random bytes.</summary>
		public static byte[] NextBytes(byte[] data) {
			random.NextBytes(data);
			return data;
		}

		/// <summary>Returns an array of the specified length with random bytes.</summary>
		public static byte[] NextBytes(int length) {
			byte[] data = new byte[length];
			random.NextBytes(data);
			return data;
		}

		// Byte -----------------------------------------------------------------------

		/// <summary>Returns a nonnegative random byte.</summary>
		public static byte NextByte() {
			return (byte) random.Next(byte.MaxValue + 1);
		}

		/// <summary>Returns a nonnegative random byte less than the specified
		/// maximum.</summary>
		public static byte NextByte(byte maxValue) {
			return (byte) random.Next(maxValue);
		}

		/// <summary>Returns a nonnegative random byte within the specified range.</summary>
		public static byte NextByte(byte minValue, byte maxValue) {
			return (byte) random.Next(minValue, maxValue);
		}

		// SByte ----------------------------------------------------------------------

		/// <summary>Returns a nonnegative random signed byte.</summary>
		public static sbyte NextSByte() {
			return (sbyte) random.Next((int)sbyte.MaxValue + 1);
		}

		/// <summary>Returns a nonnegative random signed byte less than the specified
		/// maximum.</summary>
		public static sbyte NextSByte(sbyte maxValue) {
			return (sbyte) random.Next(maxValue);
		}

		/// <summary>Returns a random signed byte within the specified range.</summary>
		public static sbyte NextSByte(sbyte minValue, sbyte maxValue) {
			return (sbyte) random.Next(minValue, maxValue);
		}

		// Integer --------------------------------------------------------------------

		/// <summary>Returns a nonnegative random integer.</summary>
		public static int NextInt() {
			return random.Next();
		}

		/// <summary>Returns a nonnegative random integer less than the specified
		/// maximum.</summary>
		public static int NextInt(int maxValue) {
			return random.Next(maxValue);
		}

		/// <summary>Returns a random integer within the specified range.</summary>
		public static int NextInt(int minValue, int maxValue) {
			return random.Next(minValue, maxValue);
		}

		/// <summary>Returns a random integer within the specified range.</summary>
		public static int NextInt(RangeI range) {
			return random.Next(range.Min, range.Max);
		}

		// Float ----------------------------------------------------------------------

		/// <summary>Returns a random float between 0 and 1.</summary>
		public static float NextFloat() {
			return (float) random.NextDouble();
		}

		/// <summary>Returns a nonnegative random float less than or equal to the
		/// specified maximum.</summary>
		public static float NextFloat(float maxValue) {
			return (float) random.NextDouble() * maxValue;
		}

		/// <summary>Returns a random float within the specified range.</summary>
		public static float NextFloat(float minValue, float maxValue) {
			return minValue + (float) random.NextDouble() * (maxValue - minValue);
		}

		/// <summary>Returns a random integer within the specified range.</summary>
		public static float NextFloat(RangeF range) {
			return range.Min + (float) random.NextDouble() * range.Range;
		}

		// Double ---------------------------------------------------------------------

		/// <summary>Returns a random double between 0 and 1.</summary>
		public static double NextDouble() {
			return random.NextDouble();
		}

		/// <summary>Returns a nonnegative random double less than or equal to the
		/// specified maximum.</summary>
		public static double NextDouble(double maxValue) {
			return random.NextDouble() * maxValue;
		}

		/// <summary>Returns a random double within the specified range.</summary>
		public static double NextDouble(double minValue, double maxValue) {
			return minValue + random.NextDouble() * (maxValue - minValue);
		}

		/// <summary>Returns a random double within the specified range.</summary>
		public static double NextDouble(RangeF range) {
			return range.Min + random.NextDouble() * range.Range;
		}

		// Vector ---------------------------------------------------------------------

		/// <summary>Returns a random vector between (0, 0) and (1, 1).</summary>
		public static Vector2F NextVector() {
			return new Vector2F((float)random.NextDouble(),
								(float)random.NextDouble());
		}

		/// <summary>Returns a nonnegative random vector less than or equal to the
		/// specified maximum.</summary>
		public static Vector2F NextVector(float maxValue) {
			return new Vector2F((float)random.NextDouble() * maxValue,
								(float)random.NextDouble() * maxValue);
		}

		/// <summary>Returns a nonnegative random vector less than or equal to the
		/// specified maximum.</summary>
		public static Vector2F NextVector(float maxX, float maxY) {
			return new Vector2F((float)random.NextDouble() * maxX,
								(float)random.NextDouble() * maxY);
		}

		/// <summary>Returns a nonnegative random vector less than or equal to the
		/// specified maximum.</summary>
		public static Vector2F NextVector(Vector2F maxPoint) {
			return new Vector2F((float)random.NextDouble(),
								(float)random.NextDouble())
									* maxPoint;
		}

		/// <summary>Returns a random vector within the rectangle.</summary>
		public static Vector2F NextVector(Rectangle2F bounds) {
			return bounds.Point + new Vector2F(	(float) random.NextDouble(),
												(float) random.NextDouble())
													* bounds.Size;
		}

		// Point ----------------------------------------------------------------------

		/// <summary>Returns a random point.</summary>
		public static Point2I NextPoint() {
			return new Point2I(random.Next(), random.Next());
		}

		/// <summary>Returns a nonnegative random point less than the specified
		/// maximum.</summary>
		public static Point2I NextPoint(int maxValue) {
			return new Point2I(random.Next(maxValue), random.Next(maxValue));
		}

		/// <summary>Returns a nonnegative random point less than the specified
		/// maximum.</summary>
		public static Point2I NextPoint(int maxX, int maxY) {
			return new Point2I(random.Next(maxX), random.Next(maxY));
		}

		/// <summary>Returns a nonnegative random point less than the specified
		/// maximum.</summary>
		public static Point2I NextPoint(Point2I maxPoint) {
			return new Point2I(random.Next(maxPoint.X), random.Next(maxPoint.Y));
		}

		/// <summary>Returns a random point within the rectangle.</summary>
		public static Point2I NextPoint(Rectangle2I bounds) {
			return bounds.Point + new Point2I(	random.Next(bounds.Size.X),
												random.Next(bounds.Size.Y));
		}

		// Orientations ---------------------------------------------------------------

		public static Direction NextDirection() {
			return (Direction) NextInt(Direction.Count);
		}

		public static Angle NextAngle() {
			return (Angle) NextInt(Angle.Count);
		}


		// Containers -----------------------------------------------------------------

		/// <summary>Choose a random element from a list.</summary>
		public static T Choose<T>(List<T> list) {
			if (list.Count == 0)
				throw new IndexOutOfRangeException();
			return list[NextInt(list.Count)];
		}
		
		/// <summary>Choose a random element from an array.</summary>
		public static T Choose<T>(params T[] options) {
			if (options.Length == 0)
				throw new IndexOutOfRangeException();
			return options[NextInt(options.Length)];
		}
	}
}
