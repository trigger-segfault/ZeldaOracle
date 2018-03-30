using System;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>A static class for advanced game-related mathematical functions and
	/// calculations.</summary>
	public static partial class GMath {
		
		//-----------------------------------------------------------------------------
		// Basic
		//-----------------------------------------------------------------------------

		// Max ------------------------------------------------------------------------

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static byte Max(byte a, byte b) {
			return Math.Max(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static sbyte Max(sbyte a, sbyte b) {
			return Math.Max(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static short Max(short a, short b) {
			return Math.Max(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static ushort Max(ushort a, ushort b) {
			return Math.Max(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static int Max(int a, int b) {
			return Math.Max(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static uint Max(uint a, uint b) {
			return Math.Max(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static long Max(long a, long b) {
			return Math.Max(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static float Max(ulong a, ulong b) {
			return Math.Max(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static float Max(float a, float b) {
			return Math.Max(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static double Max(double a, double b) {
			return Math.Max(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static decimal Max(decimal a, decimal b) {
			return Math.Max(a, b);
		}
	
		/// <summary>Returns the larger coordinates of the two specified vectors.</summary>
		public static Vector2F Max(Vector2F a, Vector2F b) {
			return new Vector2F(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
		}

		/// <summary>Returns the larger coordinates of the two specified vectors.</summary>
		public static Vector2F Max(float aX, float aY, Vector2F b) {
			return new Vector2F(Math.Max(aX, b.X), Math.Max(aY, b.Y));
		}

		/// <summary>Returns the larger coordinates of the two specified vectors.</summary>
		public static Vector2F Max(Vector2F a, float bX, float bY) {
			return new Vector2F(Math.Max(a.X, bX), Math.Max(a.Y, bY));
		}

		/// <summary>Returns the larger coordinates of the two specified vectors.</summary>
		public static Vector2F Max(float aX, float aY, float bX, float bY) {
			return new Vector2F(Math.Max(aX, bX), Math.Max(aY, bY));
		}

		/// <summary>Returns the larger coordinates of the two specified points.</summary>
		public static Point2I Max(Point2I a, Point2I b) {
			return new Point2I(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
		}

		/// <summary>Returns the larger coordinates of the two specified points.</summary>
		public static Point2I Max(int aX, int aY, Point2I b) {
			return new Point2I(Math.Max(aX, b.X), Math.Max(aY, b.Y));
		}

		/// <summary>Returns the larger coordinates of the two specified points.</summary>
		public static Point2I Max(Point2I a, int bX, int bY) {
			return new Point2I(Math.Max(a.X, bX), Math.Max(a.Y, bY));
		}

		/// <summary>Returns the larger coordinates of the two specified points.</summary>
		public static Point2I Max(int aX, int aY, int bX, int bY) {
			return new Point2I(Math.Max(aX, bX), Math.Max(aY, bY));
		}
		
		// Min ------------------------------------------------------------------------

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static byte Min(byte a, byte b) {
			return Math.Min(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static sbyte Min(sbyte a, sbyte b) {
			return Math.Min(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static short Min(short a, short b) {
			return Math.Min(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static ushort Min(ushort a, ushort b) {
			return Math.Min(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static int Min(int a, int b) {
			return Math.Min(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static uint Min(uint a, uint b) {
			return Math.Min(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static long Min(long a, long b) {
			return Math.Min(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static float Min(ulong a, ulong b) {
			return Math.Min(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static float Min(float a, float b) {
			return Math.Min(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static double Min(double a, double b) {
			return Math.Min(a, b);
		}

		/// <summary>Returns the larger of the two specified numbers.</summary>
		public static decimal Min(decimal a, decimal b) {
			return Math.Min(a, b);
		}

		/// <summary>Returns the smallest coordinates of the two specified vectors.</summary>
		public static Vector2F Min(Vector2F a, Vector2F b) {
			return new Vector2F(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
		}

		/// <summary>Returns the smallest coordinates of the two specified vectors.</summary>
		public static Vector2F Min(float aX, float aY, Vector2F b) {
			return new Vector2F(Math.Min(aX, b.X), Math.Min(aY, b.Y));
		}

		/// <summary>Returns the smallest coordinates of the two specified vectors.</summary>
		public static Vector2F Min(Vector2F a, float bX, float bY) {
			return new Vector2F(Math.Min(a.X, bX), Math.Min(a.Y, bY));
		}

		/// <summary>Returns the smallest coordinates of the two specified vectors.</summary>
		public static Vector2F Min(float aX, float aY, float bX, float bY) {
			return new Vector2F(Math.Min(aX, bX), Math.Min(aY, bY));
		}
	
		/// <summary>Returns the smallest coordinates of the two specified points.</summary>
		public static Point2I Min(Point2I a, Point2I b) {
			return new Point2I(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
		}

		/// <summary>Returns the smallest coordinates of the two specified points.</summary>
		public static Point2I Min(int aX, int aY, Point2I b) {
			return new Point2I(Math.Min(aX, b.X), Math.Min(aY, b.Y));
		}

		/// <summary>Returns the smallest coordinates of the two specified points.</summary>
		public static Point2I Min(Point2I a, int bX, int bY) {
			return new Point2I(Math.Min(a.X, bX), Math.Min(a.Y, bY));
		}

		/// <summary>Returns the smallest coordinates of the two specified points.</summary>
		public static Point2I Min(int aX, int aY, int bX, int bY) {
			return new Point2I(Math.Min(aX, bX), Math.Min(aY, bY));
		}
		
		// Clamp ----------------------------------------------------------------------

		/// <summary>Restricts the value to be within the specified range.</summary>
		public static byte Clamp(byte value, byte min, byte max) {
			return Math.Max(min, Math.Min(max, value));
		}

		/// <summary>Restricts the value to be within the specified range.</summary>
		public static sbyte Clamp(sbyte value, sbyte min, sbyte max) {
			return Math.Max(min, Math.Min(max, value));
		}

		/// <summary>Restricts the value to be within the specified range.</summary>
		public static short Clamp(short value, short min, short max) {
			return Math.Max(min, Math.Min(max, value));
		}

		/// <summary>Restricts the value to be within the specified range.</summary>
		public static ushort Clamp(ushort value, ushort min, ushort max) {
			return Math.Max(min, Math.Min(max, value));
		}

		/// <summary>Restricts the value to be within the specified range.</summary>
		public static int Clamp(int value, int min, int max) {
			return Math.Max(min, Math.Min(max, value));
		}

		/// <summary>Restricts the value to be within the specified range.</summary>
		public static int Clamp(int value, RangeI range) {
			return Math.Max(range.Min, Math.Min(range.Max, value));
		}

		/// <summary>Restricts the value to be within the specified range.</summary>
		public static uint Clamp(uint value, uint min, uint max) {
			return Math.Max(min, Math.Min(max, value));
		}

		/// <summary>Restricts the value to be within the specified range.</summary>
		public static long Clamp(long value, long min, long max) {
			return Math.Max(min, Math.Min(max, value));
		}

		/// <summary>Restricts the value to be within the specified range.</summary>
		public static ulong Clamp(ulong value, ulong min, ulong max) {
			return Math.Max(min, Math.Min(max, value));
		}

		/// <summary>Restricts the value to be within the specified range.</summary>
		public static float Clamp(float value, float min, float max) {
			return Math.Max(min, Math.Min(max, value));
		}

		/// <summary>Restricts the value to be within the specified range.</summary>
		public static float Clamp(float value, RangeF range) {
			return Math.Max(range.Min, Math.Min(range.Max, value));
		}

		/// <summary>Restricts the value to be within the specified range.</summary>
		public static double Clamp(double value, double min, double max) {
			return Math.Max(min, Math.Min(max, value));
		}

		/// <summary>Restricts the value to be within the specified range.</summary>
		public static decimal Clamp(decimal value, decimal min, decimal max) {
			return Math.Max(min, Math.Min(max, value));
		}

		/// <summary>Restricts the vector to be within the specified range.</summary>
		public static Vector2F Clamp(Vector2F value, Vector2F min, Vector2F max) {
			return new Vector2F(Math.Max(min.X, Math.Min(max.X, value.X)),
								Math.Max(min.Y, Math.Min(max.Y, value.Y)));
		}

		/// <summary>Restricts the vector to be within the specified range.</summary>
		public static Vector2F Clamp(Vector2F value, float minX, float minY, Vector2F max) {
			return new Vector2F(Math.Max(minX, Math.Min(max.X, value.X)),
								Math.Max(minY, Math.Min(max.Y, value.Y)));
		}

		/// <summary>Restricts the vector to be within the specified range.</summary>
		public static Vector2F Clamp(Vector2F value, Vector2F min, float maxX, float maxY) {
			return new Vector2F(Math.Max(min.X, Math.Min(maxX, value.X)),
								Math.Max(min.Y, Math.Min(maxY, value.Y)));
		}

		/// <summary>Restricts the vector to be within the specified range.</summary>
		public static Vector2F Clamp(Vector2F value, float minX, float minY, float maxX, float maxY) {
			return new Vector2F(Math.Max(minX, Math.Min(maxX, value.X)),
								Math.Max(minY, Math.Min(maxY, value.Y)));
		}

		/// <summary>Restricts the point to be within the specified range.</summary>
		public static Point2I Clamp(Point2I value, Point2I min, Point2I max) {
			return new Point2I(Math.Max(min.X, Math.Min(max.X, value.X)),
								Math.Max(min.Y, Math.Min(max.Y, value.Y)));
		}

		/// <summary>Restricts the point to be within the specified range.</summary>
		public static Point2I Clamp(Point2I value, int minX, int minY, Point2I max) {
			return new Point2I(Math.Max(minX, Math.Min(max.X, value.X)),
								Math.Max(minY, Math.Min(max.Y, value.Y)));
		}

		/// <summary>Restricts the point to be within the specified range.</summary>
		public static Point2I Clamp(Point2I value, Point2I min, int maxX, int maxY) {
			return new Point2I( Math.Max(min.X, Math.Min(maxX, value.X)),
								Math.Max(min.Y, Math.Min(maxY, value.Y)));
		}

		/// <summary>Restricts the point to be within the specified range.</summary>
		public static Point2I Clamp(Point2I value, int minX, int minY, int maxX, int maxY) {
			return new Point2I( Math.Max(minX, Math.Min(maxX, value.X)),
								Math.Max(minY, Math.Min(maxY, value.Y)));
		}

		/// <summary>Clamp the vector within a rectangular area.</summary>
		public static Vector2F Clamp(Vector2F value, Rectangle2F area) {
			return new Vector2F(Math.Max(area.Left, Math.Min(area.Right,  value.X)),
								Math.Max(area.Top,  Math.Min(area.Bottom, value.Y)));
		}

		/// <summary>Clamp the point within a rectangular area.</summary>
		public static Point2I Clamp(Point2I value, Rectangle2I area) {
			return new Point2I( Math.Max(area.Left, Math.Min(area.Right,  value.X)),
								Math.Max(area.Top,  Math.Min(area.Bottom, value.Y)));
		}
		
		// Wrapping -------------------------------------------------------------------
		// (modulus that also works as wrapping for negative numbers)

		/// <summary>Returns the positive modulus of the specified number.</summary>
		public static sbyte Wrap(sbyte a, sbyte mod) {
			sbyte value = (sbyte)(a % mod);
			return (value < 0 ? (sbyte)(value + mod) : value);
		}

		/// <summary>Returns the positive modulus of the specified number.</summary>
		public static short Wrap(short a, short mod) {
			short value = (short)(a % mod);
			return (value < 0 ? (short)(value + mod) : value);
		}

		/// <summary>Returns the positive modulus of the specified number.</summary>
		public static int Wrap(int a, int mod) {
			int value = a % mod;
			return (value < 0 ? value + mod : value);
		}

		/// <summary>Returns the positive modulus of the specified number.</summary>
		public static long Wrap(long a, long mod) {
			long value = a % mod;
			return (value < 0L ? value + mod : value);
		}

		/// <summary>Returns the positive modulus of the specified number.</summary>
		public static float Wrap(float a, float mod) {
			float value = a % mod;
			return (value < 0.0f ? value + mod : value);
		}

		/// <summary>Returns the positive modulus of the specified number.</summary>
		public static double Wrap(double a, double mod) {
			double value = a % mod;
			return (value < 0.0d ? value + mod : value);
		}

		/// <summary>Returns the positive modulus of the specified number.</summary>
		public static decimal Wrap(decimal a, decimal mod) {
			decimal value = a % mod;
			return (value < 0.0m ? value + mod : value);
		}

		/// <summary>Returns the positive modulus coordinates of the specified vector.</summary>
		public static Vector2F Wrap(Vector2F a, float mod) {
			Vector2F value = a % mod;
			return new Vector2F(value.X < 0.0d ? value.X + mod : value.X,
								value.Y < 0.0d ? value.Y + mod : value.Y);
		}

		/// <summary>Returns the positive modulus coordinates of the specified vector.</summary>
		public static Vector2F Wrap(Vector2F a, Vector2F mod) {
			Vector2F value = a % mod;
			return new Vector2F(value.X < 0.0d ? value.X + mod.X : value.X,
								value.Y < 0.0d ? value.Y + mod.Y : value.Y);
		}

		/// <summary>Returns the positive modulus coordinates of the specified point.</summary>
		public static Point2I Wrap(Point2I a, int mod) {
			Point2I value = a % mod;
			return new Point2I( value.X < 0 ? value.X + mod : value.X,
								value.Y < 0 ? value.Y + mod : value.Y);
		}

		/// <summary>Returns the positive modulus coordinates of the specified point.</summary>
		public static Point2I Wrap(Point2I a, Point2I mod) {
			Point2I value = a % mod;
			return new Point2I( value.X < 0 ? value.X + mod.X : value.X,
								value.Y < 0 ? value.Y + mod.Y : value.Y);
		}
	}
}
