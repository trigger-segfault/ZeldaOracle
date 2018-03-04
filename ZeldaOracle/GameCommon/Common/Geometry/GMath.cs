using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>A static class for advanced game-related mathematical functions and calculations.</summary>
	public static class GMath {
		
		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>Represents the ratio of the circumference of a circle to its diameter,
		/// specified by the constant, pi.</summary>
		public const float Pi = 3.14159265358979323846f;
		/// <summary>Represents the natural logarithmic base, specified by the constant, e.</summary>
		public const float E = 2.71828182845904523536f;
		
		/// <summary>Returns twice the value of pi.</summary>
		public const float TwoPi = Pi * 2f;
		/// <summary>Returns half the value of pi.</summary>
		public const float HalfPi = Pi * 0.5f;
		/// <summary>Returns a quarter of the value of pi.</summary>
		public const float QuarterPi = Pi * 0.25f;

		/// <summary>A full angle in radians.</summary>
		public const float FullAngle = TwoPi;
		/// <summary>Three quarters of a full angle in radians.</summary>
		public const float ThreeFourthsAngle = Pi + HalfPi;
		/// <summary>Half of a full angle in radians.</summary>
		public const float HalfAngle = Pi;
		/// <summary>Quarter of a full angle in radians.</summary>
		public const float QuarterAngle = HalfPi;
		/// <summary>Eighth of a full angle in radians.</summary>
		public const float EighthAngle = QuarterPi;
		
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
			return new Point2I(Math.Max(min.X, Math.Min(maxX, value.X)),
								Math.Max(min.Y, Math.Min(maxY, value.Y)));
		}

		/// <summary>Restricts the point to be within the specified range.</summary>
		public static Point2I Clamp(Point2I value, int minX, int minY, int maxX, int maxY) {
			return new Point2I(Math.Max(minX, Math.Min(maxX, value.X)),
								Math.Max(minY, Math.Min(maxY, value.Y)));
		}

		/// <summary>Clamp the vector within a rectangular area.</summary>
		public static Vector2F Clamp(Vector2F value, Rectangle2F area) {
			return new Vector2F(
				Clamp(value.X, area.Left, area.Right),
				Clamp(value.Y, area.Top, area.Bottom));
		}

		/// <summary>Clamp the point within a rectangular area.</summary>
		public static Point2I Clamp(Point2I value, Rectangle2I area) {
			return new Point2I(
				Clamp(value.X, area.Left, area.Right),
				Clamp(value.Y, area.Top, area.Bottom));
		}
		
		// Sign -----------------------------------------------------------------------

		/// <summary>Returns a value indicating the sign of the specified number.</summary>
		public static sbyte Sign(sbyte a) {
			return (sbyte) Math.Sign(a);
		}

		/// <summary>Returns a value indicating the sign of the specified number.</summary>
		public static short Sign(short a) {
			return (short) Math.Sign(a);
		}

		/// <summary>Returns a value indicating the sign of the specified number.</summary>
		public static int Sign(int a) {
			return Math.Sign(a);
		}

		/// <summary>Returns a value indicating the sign of the specified number.</summary>
		public static long Sign(long a) {
			return Math.Sign(a);
		}

		/// <summary>Returns a value indicating the sign of the specified number.</summary>
		public static float Sign(float a) {
			return Math.Sign(a);
		}

		/// <summary>Returns a value indicating the sign of the specified number.</summary>
		public static double Sign(double a) {
			return Math.Sign(a);
		}

		/// <summary>Returns a value indicating the sign of the specified number.</summary>
		public static decimal Sign(decimal a) {
			return Math.Sign(a);
		}

		/// <summary>Returns the coordinates indicating the sign of the specified
		/// vector.</summary>
		public static Vector2F Sign(Vector2F a) {
			return new Vector2F(Math.Sign(a.X), Math.Sign(a.Y));
		}

		/// <summary>Returns the coordinates indicating the sign of the specified
		/// vector.</summary>
		public static Vector2F Sign(float aX, float aY) {
			return new Vector2F(Math.Sign(aX), Math.Sign(aY));
		}

		/// <summary>Returns the coordinates indicating the sign of the specified
		/// point.</summary>
		public static Point2I Sign(Point2I a) {
			return new Point2I(Math.Sign(a.X), Math.Sign(a.Y));
		}

		/// <summary>Returns the coordinates indicating the sign of the specified
		/// point.</summary>
		public static Point2I Sign(int aX, int aY) {
			return new Point2I(Math.Sign(aX), Math.Sign(aY));
		}
		
		// Abs ------------------------------------------------------------------------

		/// <summary>Returns the absolute value of the specified number.</summary>
		public static sbyte Abs(sbyte a) {
			return Math.Abs(a);
		}

		/// <summary>Returns the absolute value of the specified number.</summary>
		public static short Abs(short a) {
			return Math.Abs(a);
		}

		/// <summary>Returns the absolute value of the specified number.</summary>
		public static int Abs(int a) {
			return Math.Abs(a);
		}

		/// <summary>Returns the absolute value of the specified number.</summary>
		public static long Abs(long a) {
			return Math.Abs(a);
		}

		/// <summary>Returns the absolute value of the specified number.</summary>
		public static float Abs(float a) {
			return Math.Abs(a);
		}

		/// <summary>Returns the absolute value of the specified number.</summary>
		public static double Abs(double a) {
			return Math.Abs(a);
		}

		/// <summary>Returns the absolute value of the specified number.</summary>
		public static decimal Abs(decimal a) {
			return Math.Abs(a);
		}

		/// <summary>Returns the absolute coordinates of the specified vector.</summary>
		public static Vector2F Abs(Vector2F a) {
			return new Vector2F(Math.Abs(a.X), Math.Abs(a.Y));
		}

		/// <summary>Returns the absolute coordinates of the specified vector.</summary>
		public static Vector2F Abs(float aX, float aY) {
			return new Vector2F(Math.Abs(aX), Math.Abs(aY));
		}

		/// <summary>Returns the absolute coordinates of the specified point.</summary>
		public static Point2I Abs(Point2I a) {
			return new Point2I(Math.Abs(a.X), Math.Abs(a.Y));
		}

		/// <summary>Returns the absolute coordinates of the specified point.</summary>
		public static Point2I Abs(int aX, int aY) {
			return new Point2I(Math.Abs(aX), Math.Abs(aY));
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

		// IsInt ----------------------------------------------------------------------

		/// <summary>Returns true if the specified number is an integer.</summary>
		public static bool IsInt(float a) {
			return (a % 1 == 0);
		}

		/// <summary>Returns true if the specified number is an integer.</summary>
		public static bool IsInt(double a) {
			return (a % 1 == 0);
		}

		/// <summary>Returns true if the specified number is an integer.</summary>
		public static bool IsInt(decimal a) {
			return (a % 1 == 0);
		}

		/// <summary>Returns true if the specified coordinates are integers.</summary>
		public static bool IsInt(Vector2F a) {
			return ((a.X % 1 == 0) && (a.Y % 1 == 0));
		}

		/// <summary>Returns true if the specified coordinates are integers.</summary>
		public static bool IsInt(float aX, float aY) {
			return ((aX % 1 == 0) && (aY % 1 == 0));
		}


		//-----------------------------------------------------------------------------
		// Rounding
		//-----------------------------------------------------------------------------

		// Floor ----------------------------------------------------------------------

		/// <summary>Returns the largest integer less than or equal to the specified
		/// number.</summary>
		public static float Floor(float a) {
			return (float) Math.Floor(a);
		}

		/// <summary>Returns the largest integer less than or equal to the specified
		/// number, with the base unit.</summary>
		public static float Floor(float a, float unit) {
			return (float) Math.Floor(a / unit) * unit;
		}

		/// <summary>Returns the largest integer less than or equal to the specified
		/// number.</summary>
		public static double Floor(double a) {
			return Math.Floor(a);
		}

		/// <summary>Returns the largest integer less than or equal to the specified
		/// number, with the base unit.</summary>
		public static double Floor(double a, double unit) {
			return Math.Floor(a / unit) * unit;
		}

		/// <summary>Returns the largest integer less than or equal to the specified
		/// number.</summary>
		public static decimal Floor(decimal a) {
			return Math.Floor(a);
		}

		/// <summary>Returns the largest integer less than or equal to the specified
		/// number, with the base unit.</summary>
		public static decimal Floor(decimal a, decimal unit) {
			return Math.Floor(a / unit) * unit;
		}

		/// <summary>Returns the largest point less than or equal to the specified
		/// vector.</summary>
		public static Vector2F Floor(Vector2F a) {
			return new Vector2F((float) Math.Floor(a.X), (float) Math.Floor(a.Y));
		}

		/// <summary>Returns the largest point less than or equal to the specified
		/// vector, with the base unit.</summary>
		public static Vector2F Floor(Vector2F a, float unit) {
			return new Vector2F((float) Math.Floor(a.X / unit) * unit,
								(float) Math.Floor(a.Y / unit) * unit);
		}
		/// <summary>Returns the largest point less than or equal to the specified
		/// vector, with the base unit.</summary>
		public static Vector2F Floor(Vector2F a, Vector2F unit) {
			return new Vector2F((float) Math.Floor(a.X / unit.X) * unit.X,
								(float) Math.Floor(a.Y / unit.Y) * unit.Y);
		}
		
		// Ceiling --------------------------------------------------------------------
		
		/// <summary>Returns the smallest integer greater than or equal to the
		/// specified number.</summary>
		public static float Ceiling(float a) {
			return (float) Math.Ceiling(a);
		}

		/// <summary>Returns the smallest integer greater than or equal to the
		/// specified number, with the base unit.</summary>
		public static float Ceiling(float a, float unit) {
			return (float) Math.Ceiling(a / unit) * unit;
		}

		/// <summary>Returns the smallest integer greater than or equal to the
		/// specified number.</summary>
		public static double Ceiling(double a) {
			return Math.Ceiling(a);
		}

		/// <summary>Returns the smallest integer greater than or equal to the
		/// specified number, with the base unit.</summary>
		public static double Ceiling(double a, double unit) {
			return Math.Ceiling(a / unit) * unit;
		}

		/// <summary>Returns the smallest integer greater than or equal to the
		/// specified number.</summary>
		public static decimal Ceiling(decimal a) {
			return Math.Ceiling(a);
		}

		/// <summary>Returns the smallest integer greater than or equal to the
		/// specified number, with the base unit.</summary>
		public static decimal Ceiling(decimal a, decimal unit) {
			return Math.Ceiling(a / unit) * unit;
		}

		/// <summary>Returns the smallest point greater than or equal to the
		/// specified vector.</summary>
		public static Vector2F Ceiling(Vector2F a) {
			return new Vector2F((float) Math.Ceiling(a.X), (float) Math.Ceiling(a.Y));
		}

		/// <summary>Returns the smallest point greater than or equal to the specified
		/// vector, with the base unit.</summary>
		public static Vector2F Ceiling(Vector2F a, float unit) {
			return new Vector2F((float) Math.Ceiling(a.X / unit) * unit,
								(float) Math.Ceiling(a.Y / unit) * unit);
		}

		/// <summary>Returns the smallest point greater than or equal to the specified
		/// vector, with the base unit.</summary>
		public static Vector2F Ceiling(Vector2F a, Vector2F unit) {
			return new Vector2F((float) Math.Ceiling(a.X / unit.X) * unit.X,
								(float) Math.Ceiling(a.Y / unit.Y) * unit.Y);
		}

		// Round ----------------------------------------------------------------------

		/// <summary>Rounds the specified number to the nearest integral value.</summary>
		public static float Round(float a) {
			return (float) Math.Round(a);
		}

		/// <summary>Rounds the specified number to the nearest integral value, with
		/// the base unit.</summary>
		public static float Round(float a, float unit) {
			return (float) Math.Round(a / unit) * unit;
		}

		/// <summary>Rounds the specified number to the nearest integral value.</summary>
		public static double Round(double a) {
			return Math.Round(a);
		}

		/// <summary>Rounds the specified number to the nearest integral value, with
		/// the base unit.</summary>
		public static double Round(double a, double unit) {
			return Math.Round(a / unit) * unit;
		}

		/// <summary>Rounds the specified number to the nearest integral value.</summary>
		public static decimal Round(decimal a) {
			return Math.Round(a);
		}

		/// <summary>Rounds the specified number to the nearest integral value, with
		/// the base unit.</summary>
		public static decimal Round(decimal a, decimal unit) {
			return Math.Round(a / unit) * unit;
		}

		/// <summary>Rounds the specified vector to the nearest integral coordinates.</summary>
		public static Vector2F Round(Vector2F a) {
			return new Vector2F((float) Math.Round(a.X), (float) Math.Round(a.Y));
		}

		/// <summary>Rounds the specified vector to the nearest integral coordinates,
		/// with the base unit.</summary>
		public static Vector2F Round(Vector2F a, float unit) {
			return new Vector2F((float) Math.Round(a.X / unit) * unit,
								(float) Math.Round(a.Y / unit) * unit);
		}

		/// <summary>Rounds the specified vector to the nearest integral coordinates,
		/// with the base unit.</summary>
		public static Vector2F Round(Vector2F a, Vector2F unit) {
			return new Vector2F((float) Math.Round(a.X / unit.X) * unit.X,
								(float) Math.Round(a.Y / unit.Y) * unit.Y);
		}
		
		// Truncate -------------------------------------------------------------------

		/// <summary>Rounds the specified value to the nearest integer towards zero.</summary>
		public static float Truncate(float a) {
			return (float) Math.Truncate(a);
		}

		/// <summary>Rounds the specified value to the nearest integer towards zero,
		/// with the base unit.</summary>
		public static float Truncate(float a, float unit) {
			return (float) Math.Truncate(a / unit) * unit;
		}

		/// <summary>Rounds the specified value to the nearest integer towards zero.</summary>
		public static double Truncate(double a) {
			return Math.Truncate(a);
		}

		/// <summary>Rounds the specified value to the nearest integer towards zero,
		/// with the base unit.</summary>
		public static double Truncate(double a, double unit) {
			return Math.Truncate(a / unit) * unit;
		}

		/// <summary>Rounds the specified value to the nearest integer towards zero.</summary>
		public static decimal Truncate(decimal a) {
			return Math.Truncate(a);
		}

		/// <summary>Rounds the specified value to the nearest integer towards zero,
		/// with the base unit.</summary>
		public static decimal Truncate(decimal a, decimal unit) {
			return Math.Truncate(a / unit) * unit;
		}

		/// <summary>Rounds the specified vector to the nearest point towards zero.</summary>
		public static Vector2F Truncate(Vector2F a) {
			return new Vector2F((float) Math.Truncate(a.X), (float) Math.Truncate(a.Y));
		}

		/// <summary>Rounds the specified vector to the nearest point towards zero,
		/// with the base unit.</summary>
		public static Vector2F Truncate(Vector2F a, float unit) {
			return new Vector2F((float) Math.Truncate(a.X / unit) * unit,
								(float) Math.Truncate(a.Y / unit) * unit);
		}

		/// <summary>Rounds the specified vector to the nearest point towards zero,
		/// with the base unit.</summary>
		public static Vector2F Truncate(Vector2F a, Vector2F unit) {
			return new Vector2F((float) Math.Truncate(a.X / unit.X) * unit.X,
								(float) Math.Truncate(a.Y / unit.Y) * unit.Y);
		}
		
		// Atruncate ------------------------------------------------------------------

		/// <summary>Rounds the specified value to the farthest integer from zero.</summary>
		public static float Atruncate(float a) {
			return (a < 0.0f ? (float) Math.Floor(a) : (float) Math.Ceiling(a));
		}

		/// <summary>Rounds the specified value to the farthest integer from zero, with
		/// the base unit.</summary>
		public static float Atruncate(float a, float unit) {
			return (a < 0.0f ?	(float) Math.Floor(a / unit) * unit :
								(float) Math.Ceiling(a / unit) * unit);
		}

		/// <summary>Rounds the specified value to the farthest integer from zero.</summary>
		public static double Atruncate(double a) {
			return (a < 0.0d ? Math.Floor(a) : Math.Ceiling(a));
		}

		/// <summary>Rounds the specified value to the farthest integer from zero, with
		/// the base unit.</summary>
		public static double Atruncate(double a, double unit) {
			return (a < 0.0d ? Math.Floor(a / unit) * unit : Math.Ceiling(a / unit) * unit);
		}

		/// <summary>Rounds the specified value to the farthest integer from zero.</summary>
		public static decimal Atruncate(decimal a) {
			return (a < 0.0m ? Math.Floor(a) : Math.Ceiling(a));
		}

		/// <summary>Rounds the specified value to the farthest integer from zero, with
		/// the base unit.</summary>
		public static decimal Atruncate(decimal a, decimal unit) {
			return (a < 0.0m ?  Math.Floor(a / unit) * unit :
								Math.Ceiling(a / unit) * unit);
		}

		/// <summary>Rounds the specified vector to the farthest point from zero.</summary>
		public static Vector2F Atruncate(Vector2F a) {
			return new Vector2F(a.X < 0.0f ? (float) Math.Floor(a.X) :
											 (float) Math.Ceiling(a.X),
								a.Y < 0.0f ? (float) Math.Floor(a.Y) :
											 (float) Math.Ceiling(a.Y));
		}

		/// <summary>Rounds the specified vector to the farthest point from zero, with
		/// the base unit.</summary>
		public static Vector2F Atruncate(Vector2F a, float unit) {
			return new Vector2F(a.X < 0.0f ? (float) Math.Floor(a.X / unit) * unit :
											 (float) Math.Ceiling(a.X / unit) * unit,
								a.Y < 0.0f ? (float) Math.Floor(a.Y / unit) * unit :
											 (float) Math.Ceiling(a.Y / unit) * unit);
		}

		/// <summary>Rounds the specified vector to the farthest point from zero, with
		/// the base unit.</summary>
		public static Vector2F Atruncate(Vector2F a, Vector2F unit) {
			return new Vector2F(a.X < 0.0f ? (float) Math.Floor(a.X / unit.X) * unit.X :
											 (float) Math.Ceiling(a.X / unit.X) * unit.X,
								a.Y < 0.0f ? (float) Math.Floor(a.Y / unit.Y) * unit.Y :
											 (float) Math.Ceiling(a.Y / unit.Y) * unit.Y);
		}


		//-----------------------------------------------------------------------------
		// Exponents
		//-----------------------------------------------------------------------------

		// Pow ------------------------------------------------------------------------

		/// <summary>Returns the specified number raised to the specified power.</summary>
		public static int Pow(int x, int y) {
			return (int) Math.Pow(x, y);
		}

		/// <summary>Returns the specified number raised to the specified power.</summary>
		public static uint Pow(uint x, uint y) {
			return (uint) Math.Pow(x, y);
		}

		/// <summary>Returns the specified number raised to the specified power.</summary>
		public static long Pow(long x, long y) {
			return (long) Math.Pow(x, y);
		}

		/// <summary>Returns the specified number raised to the specified power.</summary>
		public static ulong Pow(ulong x, ulong y) {
			return (ulong) Math.Pow(x, y);
		}

		/// <summary>Returns the specified number raised to the specified power.</summary>
		public static float Pow(float x, float y) {
			return (float) Math.Pow(x, y);
		}

		/// <summary>Returns the specified number raised to the specified power.</summary>
		public static double Pow(double x, double y) {
			return Math.Pow(x, y);
		}

		/// <summary>Returns the specified vector raised to the specified power.</summary>
		public static Vector2F Pow(Vector2F x, float y) {
			return new Vector2F((float) Math.Pow(x.X, y), (float) Math.Pow(x.Y, y));
		}

		/// <summary>Returns the specified vector raised to the specified power.</summary>
		public static Vector2F Pow(Vector2F x, Vector2F y) {
			return new Vector2F((float) Math.Pow(x.X, y.X), (float) Math.Pow(x.Y, y.Y));
		}

		/// <summary>Returns the specified point raised to the specified power.</summary>
		public static Point2I Pow(Point2I x, int y) {
			return new Point2I((int) Math.Pow(x.X, y), (int) Math.Pow(x.Y, y));
		}

		/// <summary>Returns the specified point raised to the specified power.</summary>
		public static Point2I Pow(Point2I x, Point2I y) {
			return new Point2I((int) Math.Pow(x.X, y.X), (int) Math.Pow(x.Y, y.Y));
		}

		// Root -----------------------------------------------------------------------

		/// <summary>Returns a specified number lowered to the specified root.</summary>
		public static float Root(float x, float y) {
			return (float) Math.Pow(x, 1.0f / y);
		}

		/// <summary>Returns a specified number lowered to the specified root.</summary>
		public static double Root(double x, double y) {
			return Math.Pow(x, 1.0d / y);
		}

		/// <summary>Returns a specified vector lowered to the specified root.</summary>
		public static Vector2F Root(Vector2F x, float y) {
			return new Vector2F((float) Math.Pow(x.X, 1.0d / y),
								(float) Math.Pow(x.Y, 1.0d / y));
		}

		/// <summary>Returns a specified vector lowered to the specified root.</summary>
		public static Vector2F Root(Vector2F x, Vector2F y) {
			return new Vector2F((float) Math.Pow(x.X, 1.0d / y.X),
								(float) Math.Pow(x.Y, 1.0d / y.Y));
		}

		// Sqrt -----------------------------------------------------------------------

		/// <summary>Returns the square root of a specified number.</summary>
		public static float Sqrt(float x) {
			return (float) Math.Sqrt(x);
		}

		/// <summary>Returns the square root of a specified number.</summary>
		public static double Sqrt(double x) {
			return Math.Sqrt(x);
		}

		/// <summary>Returns the square root of a specified vector.</summary>
		public static Vector2F Sqrt(Vector2F x) {
			return new Vector2F((float) Math.Sqrt(x.X), (float) Math.Sqrt(x.Y));
		}

		// Cbrt -----------------------------------------------------------------------

		/// <summary>Returns the cube root of a specified number.</summary>
		public static float Cbrt(float x) {
			return (float) Math.Pow(x, 1.0f / 3.0f);
		}

		/// <summary>Returns the cube root of a specified number.</summary>
		public static double Cbrt(double x) {
			return Math.Pow(x, 1.0d / 3.0d);
		}

		/// <summary>Returns the cube root of a specified vector.</summary>
		public static Vector2F Cbrt(Vector2F x) {
			return new Vector2F((float) Math.Pow(x.X, 1.0f / 3.0f),
								(float) Math.Pow(x.Y, 1.0f / 3.0f));
		}

		// Exp ------------------------------------------------------------------------

		/// <summary>Returns e raised to the specified power.</summary>
		public static float Exp(float x) {
			return (float) Math.Exp(x);
		}

		/// <summary>Returns e raised to the specified power.</summary>
		public static double Exp(double x) {
			return Math.Exp(x);
		}

		/// <summary>Returns e raised to the specified power coordinates.</summary>
		public static Vector2F Exp(Vector2F x) {
			return new Vector2F((float) Math.Exp(x.X), (float) Math.Exp(x.Y));
		}
		

		//-----------------------------------------------------------------------------
		// Logarithm
		//-----------------------------------------------------------------------------

		// Log e ----------------------------------------------------------------------

		/// <summary>Returns the natural (base e) logarithm of the specified number.</summary>
		public static float Log(float d) {
			return (float) Math.Log(d);
		}

		/// <summary>Returns the natural (base e) logarithm of the specified number.</summary>
		public static double Log(double d) {
			return Math.Log(d);
		}

		/// <summary>Returns the natural (base e) logarithm of the specified vector.</summary>
		public static Vector2F Log(Vector2F d) {
			return new Vector2F((float) Math.Log(d.X), (float) Math.Log(d.Y));
		}
		
		// Log (base) -----------------------------------------------------------------

		/// <summary>Returns the specified base logarithm of the specified number.</summary>
		public static float Log(float a, float newBase) {
			return (float) Math.Log(a, newBase);
		}

		/// <summary>Returns the specified base logarithm of the specified number.</summary>
		public static double Log(double a, double newBase) {
			return Math.Log(a, newBase);
		}

		/// <summary>Returns the specified base logarithm of the specified vector.</summary>
		public static Vector2F Log(Vector2F a, float newBase) {
			return new Vector2F((float) Math.Log(a.X, newBase),
								(float) Math.Log(a.Y, newBase));
		}

		/// <summary>Returns the specified base logarithm of the specified vector.</summary>
		public static Vector2F Log(Vector2F a, Vector2F newBase) {
			return new Vector2F((float) Math.Log(a.X, newBase.X),
								(float) Math.Log(a.Y, newBase.Y));
		}

		// Pow 10 ---------------------------------------------------------------------

		/// <summary>Returns the base 10 logarithm of the specified number.</summary>
		public static float Log10(float d) {
			return (float) Math.Log10(d);
		}

		/// <summary>Returns the base 10 logarithm of the specified number.</summary>
		public static double Log10(double d) {
			return Math.Log10(d);
		}

		/// <summary>Returns the base 10 logarithm of the specified vector.</summary>
		public static Vector2F Log10(Vector2F d) {
			return new Vector2F((float) Math.Log10(d.X), (float) Math.Log10(d.Y));
		}


		//-----------------------------------------------------------------------------
		// Angles
		//-----------------------------------------------------------------------------
		
		// Convert Angle --------------------------------------------------------------
		
		/// <summary>Converts the specified angle from radians into degrees.</summary>
		public static float ToDegrees(float radians) {
			return radians / Pi * 180f;
		}

		/// <summary>Converts the specified angle from radians into degrees.</summary>
		public static double ToDegrees(double radians) {
			return radians / Pi * 180.0;
		}

		/// <summary>Converts the specified angle from degrees into radians.</summary>
		public static float ToRadians(float degrees) {
			return degrees / 180f * Pi;
		}

		/// <summary>Converts the specified angle from degrees into radians.</summary>
		public static double ToRadians(double degrees) {
			return degrees / 180.0 * Pi;
		}

		// Modulus Angle --------------------------------------------------------------
		
		/// <summary>Returns the absolute angle as (0, 360].</summary>
		public static float Plusdir(float angle) {
			return Wrap(angle, FullAngle);
		}

		/// <summary>Returns the absolute angle as (0, 360].</summary>
		public static double Plusdir(double angle) {
			return Wrap(angle, FullAngle);
		}

		/// <summary>Returns the absolute angle as [0, 360).</summary>
		public static float Plusdir2(float angle) {
			float value = Wrap(angle, FullAngle);
			if (value == 0.0f)
				return FullAngle;
			return value;
		}

		/// <summary>Returns the absolute angle as [0, 360).</summary>
		public static double Plusdir2(double angle) {
			double value = Wrap(angle, FullAngle);
			if (value == 0.0d)
				return FullAngle;
			return value;
		}
		
		/// <summary>Returns the absolute angle as [-180, 180).</summary>
		public static float Plusdir3(float angle) {
			float value = Wrap(angle, FullAngle);
			if (value > HalfAngle)
				return value - FullAngle;
			return value;
		}

		/// <summary>Returns the absolute angle as [-180, 180).</summary>
		public static double Plusdir3(double angle) {
			double value = Wrap(angle, FullAngle);
			if (value > HalfAngle)
				return value - FullAngle;
			return value;
		}
		
		/// <summary>Return the modular distance from one angle to another using the
		/// given winding order.</summary>
		public static float DeltaDirection(float source, float destination,
			WindingOrder windingOrder = WindingOrder.CounterClockwise)
		{
			source = Plusdir(source);
			destination = Plusdir(destination);
			if (windingOrder == WindingOrder.Clockwise) {
				if (destination > source)
					return (source + GMath.FullAngle - destination);
				else
					return (source - destination);
			}
			else {
				if (destination < source)
					return (destination + GMath.FullAngle - source);
				else
					return (destination - source);
			}
		}

		/// <summary>Returns the change in direction between two angles.</summary>
		public static double DeltaDirection(double source, double destination,
			WindingOrder windingOrder = WindingOrder.CounterClockwise)
		{
			source = Plusdir(source);
			destination = Plusdir(destination);
			if (windingOrder == WindingOrder.Clockwise) {
				if (destination > source)
					return (source + GMath.FullAngle - destination);
				else
					return (source - destination);
			}
			else {
				if (destination < source)
					return (destination + GMath.FullAngle - source);
				else
					return (destination - source);
			}
		}


		//-----------------------------------------------------------------------------
		// Trigonometry
		//-----------------------------------------------------------------------------

		// Trigonometric --------------------------------------------------------------
		
		/// <summary>Returns the sine of the specified angle.</summary>
		public static float Sin(float a) {
			return (float) Math.Sin(a);
		}

		/// <summary>Returns the sine of the specified angle.</summary>
		public static double Sin(double a) {
			return Math.Sin(a);
		}

		/// <summary>Returns the cosine of the specified angle.</summary>
		public static float Cos(float a) {
			return (float) Math.Cos(a);
		}

		/// <summary>Returns the cosine of the specified angle.</summary>
		public static double Cos(double a) {
			return Math.Cos(a);
		}

		/// <summary>Returns the tangent of the specified angle.</summary>
		public static float Tan(float a) {
			return (float) Math.Tan(a);
		}

		/// <summary>Returns the tangent of the specified angle.</summary>
		public static double Tan(double a) {
			return Math.Tan(a);
		}
		
		// Arc Trigonometric ----------------------------------------------------------

		/// <summary>Returns the angle whose sine is the specified number.</summary>
		public static float Asin(float d) {
			return (float) Math.Asin(d);
		}

		/// <summary>Returns the angle whose sine is the specified number.</summary>
		public static double Asin(double d) {
			return Math.Asin(d);
		}

		/// <summary>Returns the angle whose cosine is the specified number.</summary>
		public static float Acos(float d) {
			return (float) Math.Acos(d);
		}

		/// <summary>Returns the angle whose cosine is the specified number.</summary>
		public static double Acos(double d) {
			return Math.Acos(d);
		}

		/// <summary>Returns the angle whose tangent is the specified number.</summary>
		public static float Atan(float d) {
			return (float) Math.Atan(d);
		}

		/// <summary>Returns the angle whose tangent is the specified number.</summary>
		public static double Atan(double d) {
			return Math.Atan(d);
		}
		
		/// <summary>Returns the angle whose tangent is the quotient of two specified
		/// numbers.</summary>
		public static float Atan2(float y, float x) {
			return (float) Math.Atan2(y, x);
		}

		/// <summary>Returns the angle whose tangent is the quotient of two specified
		/// numbers.</summary>
		public static double Atan2(double y, double x) {
			return Math.Atan2(y, x);
		}
		
		// Hyperbolic -----------------------------------------------------------------

		/// <summary>Returns the hyperbolic sine of the specified angle.</summary>
		public static float Sinh(float a) {
			return (float) Math.Sinh(a);
		}

		/// <summary>Returns the hyperbolic sine of the specified angle.</summary>
		public static double Sinh(double a) {
			return Math.Sinh(a);
		}

		/// <summary>Returns the hyperbolic cosine of the specified angle.</summary>
		public static float Cosh(float a) {
			return (float) Math.Cosh(a);
		}

		/// <summary>Returns the hyperbolic cosine of the specified angle.</summary>
		public static double Cosh(double a) {
			return Math.Cosh(a);
		}

		/// <summary>Returns the hyperbolic tangent of the specified angle.</summary>
		public static float Tanh(float a) {
			return (float) Math.Tanh(a);
		}

		/// <summary>Returns the hyperbolic tangent of the specified angle.</summary>
		public static double Tanh(double a) {
			return Math.Tanh(a);
		}


		//-----------------------------------------------------------------------------
		// Graphics
		//-----------------------------------------------------------------------------

		/*public static Point2I FlipAndRotate(Point2I drawOffset, Flip flip, Rotation rotation, Point2I size, Rectangle2I bounds) {
			return FlipAndRotate(drawOffset, flip, rotation, (Vector2F) (bounds.Size - size) / 2f - bounds.Point);
		}

		public static Point2I FlipAndRotate(Point2I drawOffset, Flip flip, Rotation rotation, Vector2F origin) {
			if (flip == Flip.None && rotation == Rotation.None)
				return drawOffset;
			Vector2F tempDrawOffset = drawOffset - origin;
			switch (rotation) {
			case Rotation.Clockwise90:
				tempDrawOffset = new Vector2F(-tempDrawOffset.Y, tempDrawOffset.X); break;
			case Rotation.Clockwise180:
				tempDrawOffset = -tempDrawOffset; break;
			case Rotation.Clockwise270:
				tempDrawOffset = new Vector2F(tempDrawOffset.Y, -tempDrawOffset.X); break;
			}
			if (flip.HasFlag(Flip.Horizontal))
				tempDrawOffset.X = -tempDrawOffset.X;
			if (flip.HasFlag(Flip.Vertical))
				tempDrawOffset.Y = -tempDrawOffset.Y;

			return (Point2I) GMath.Floor(tempDrawOffset + origin);
		}*/

		/// <summary>Returns the corrected angle for use with XNA drawing functions.</summary>
		public static float CorrectAngle(float angle) {
			return GMath.Wrap(-angle, FullAngle);
		}
	}
}
