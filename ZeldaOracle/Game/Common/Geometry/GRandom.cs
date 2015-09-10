using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace ZeldaOracle.Common.Geometry {
/** <summary>
* A static class for advanced random number generation.
* </summary> */
public static class GRandom {
	
	//========== VARIABLES ===========

	/** <summary> The class used to randomly generate numbers. </summary> */
	private static Random random = new Random();

	//============ RANDOM ============
	
	// Seed

	/** <summary> Sets the new seed of the random number generator. </summary> */
	public static void SetSeed(int seed) {
		random = new Random(seed);
	}
	
	// Bool

	/** <summary> Returns true or false at random. </summary> */
	public static bool NextBool() {
		return random.Next(2) == 1;
	}
	/** <summary> Returns true or false at random with the specified ratio of getting true. </summary> */
	public static bool NextBool(float ratio) {
		return (ratio == 0.0f ? false : (float)random.NextDouble() <= ratio);
	}
	/** <summary> Returns true or false at random with the specified ratio of getting true. </summary> */
	public static bool NextBool(double ratio) {
		return (ratio == 0.0d ? false : random.NextDouble() <= ratio);
	}

	// Data

	/** <summary> Fills the array with random bytes. </summary> */
	public static void NextBytes(byte[] data) {
		random.NextBytes(data);
	}

	// Byte

	/** <summary> Returns a nonnegative random byte. </summary> */
	public static byte NextByte() {
		return (byte)random.Next((int)byte.MaxValue + 1);
	}
	/** <summary> Returns a nonnegative random byte less than the specified maximum. </summary> */
	public static byte NextByte(byte maxValue) {
		return (byte)random.Next(maxValue);
	}
	/** <summary> Returns a nonnegative random byte within the specified range. </summary> */
	public static byte NextByte(byte minValue, byte maxValue) {
		return (byte)random.Next(minValue, maxValue);
	}

	// SByte

	/** <summary> Returns a nonnegative random signed byte. </summary> */
	public static sbyte NextSByte() {
		return (sbyte)random.Next((int)sbyte.MaxValue + 1);
	}
	/** <summary> Returns a nonnegative random signed byte less than the specified maximum. </summary> */
	public static sbyte NextSByte(sbyte maxValue) {
		return (sbyte)random.Next(maxValue);
	}
	/** <summary> Returns a random signed byte within the specified range. </summary> */
	public static sbyte NextSByte(sbyte minValue, sbyte maxValue) {
		return (sbyte)random.Next(minValue, maxValue);
	}

	// Integer

	/** <summary> Returns a nonnegative random integer. </summary> */
	public static int NextInt() {
		return random.Next();
	}
	/** <summary> Returns a nonnegative random integer less than the specified maximum. </summary> */
	public static int NextInt(int maxValue) {
		return random.Next(maxValue);
	}
	/** <summary> Returns a random integer within the specified range. </summary> */
	public static int NextInt(int minValue, int maxValue) {
		return random.Next(minValue, maxValue);
	}

	// Float

	/** <summary> Returns a random float between 0 and 1. </summary> */
	public static float NextFloat() {
		return (float)random.NextDouble();
	}
	/** <summary> Returns a nonnegative random float less than or equal to the specified maximum. </summary> */
	public static float NextFloat(float maxValue) {
		return (float)random.NextDouble() * maxValue;
	}
	/** <summary> Returns a random float within the specified range. </summary> */
	public static float NextFloat(float minValue, float maxValue) {
		return minValue + (float)random.NextDouble() * (maxValue - minValue);
	}

	// Double

	/** <summary> Returns a random double between 0 and 1. </summary> */
	public static double NextDouble() {
		return random.NextDouble();
	}
	/** <summary> Returns a nonnegative random double less than or equal to the specified maximum. </summary> */
	public static double NextDouble(double maxValue) {
		return random.NextDouble() * maxValue;
	}
	/** <summary> Returns a random double within the specified range. </summary> */
	public static double NextDouble(double minValue, double maxValue) {
		return minValue + random.NextDouble() * (maxValue - minValue);
	}
	/** <summary> Returns a random double within the specified range. </summary> */
	public static double NextDouble(RangeF range) {
		return range.Min + random.NextDouble() * (range.Max - range.Min);
	}

	// Vector

	/** <summary> Returns a random vector between (0, 0) and (1, 1). </summary> */
	public static Vector2F NextVector() {
		return new Vector2F((float)random.NextDouble(), (float)random.NextDouble());
	}
	/** <summary> Returns a nonnegative random vector less than or equal to the specified maximum. </summary> */
	public static Vector2F NextVector(float maxValue) {
		return new Vector2F((float)random.NextDouble() * maxValue, (float)random.NextDouble() * maxValue);
	}
	/** <summary> Returns a nonnegative random vector less than or equal to the specified maximum. </summary> */
	public static Vector2F NextVector(float maxX, float maxY) {
		return new Vector2F((float)random.NextDouble() * maxX, (float)random.NextDouble() * maxY);
	}
	/** <summary> Returns a nonnegative random vector less than or equal to the specified maximum. </summary> */
	public static Vector2F NextVector(Vector2F maxPoint) {
		return new Vector2F((float)random.NextDouble(), (float)random.NextDouble()) * maxPoint;
	}

	// Point

	/** <summary> Returns a random point. </summary> */
	public static Point2I NextPoint() {
		return new Point2I(random.Next(), random.Next());
	}
	/** <summary> Returns a nonnegative random point less than the specified maximum. </summary> */
	public static Point2I NextPoint(int maxValue) {
		return new Point2I(random.Next(maxValue), random.Next(maxValue));
	}
	/** <summary> Returns a nonnegative random point less than the specified maximum. </summary> */
	public static Point2I NextPoint(int maxX, int maxY) {
		return new Point2I(random.Next(maxX), random.Next(maxY));
	}
	/** <summary> Returns a nonnegative random point less than the specified maximum. </summary> */
	public static Point2I NextPoint(Point2I maxPoint) {
		return new Point2I(random.Next(maxPoint.X), random.Next(maxPoint.Y));
	}

}
} // End namespace
