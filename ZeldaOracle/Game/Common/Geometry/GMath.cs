using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace ZeldaOracle.Common.Geometry {
/** <summary>
* A static class for advanced game-related mathematical functions and calculations.
* </summary> */
public static class GMath {
	
	//========== CONSTANTS ===========
	#region Constants

	/** <summary> Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π. </summary> */
	public const double Pi = 3.14159265358979323846;
	/** <summary> Represents the natural logarithmic base, specified by the constant, e. </summary> */
	public const double E = 2.71828182845904523536;
	
	/** <summary> Quarter of an angle in either radians or degrees. </summary> */
	public static double QuarterAngle {
		get { return (usingDegrees ? 90.0 : Pi / 2.0); }
	}
	/** <summary> Half of an angle in either radians or degrees. </summary> */
	public static double HalfAngle {
		get { return (usingDegrees ? 180.0 : Pi); }
	}
	/** <summary> Three quarters of an angle in either radians or degrees. </summary> */
	public static double ThreeFourthsAngle {
		get { return (usingDegrees ? 270.0 : Pi * 1.5); }
	}
	/** <summary> A full angle in either radians or degrees. </summary> */
	public static double FullAngle {
		get { return (usingDegrees ? 360.0 : Pi * 2.0); }
	}

	#endregion
	//========== VARIABLES ===========
	#region Variables

	/** <summary> If true, all trigonometric functions will use degrees instead of radians. </summary> */
	private static bool usingDegrees = true;

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Sets if all trigonometric functions will use degrees instead of radians. </summary> */
	public static bool UsingDegrees {
		get { return usingDegrees; }
		set { usingDegrees = value; }
	}

	#endregion
	//============ BASIC =============
	#region Basic
	//--------------------------------
	#region Max

	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static byte Max(byte a, byte b) {
		return Math.Max(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static sbyte Max(sbyte a, sbyte b) {
		return Math.Max(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static short Max(short a, short b) {
		return Math.Max(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static ushort Max(ushort a, ushort b) {
		return Math.Max(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static int Max(int a, int b) {
		return Math.Max(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static uint Max(uint a, uint b) {
		return Math.Max(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static long Max(long a, long b) {
		return Math.Max(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static float Max(ulong a, ulong b) {
		return Math.Max(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static float Max(float a, float b) {
		return Math.Max(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static double Max(double a, double b) {
		return Math.Max(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static decimal Max(decimal a, decimal b) {
		return Math.Max(a, b);
	}
	
	/** <summary> Returns the larger coordinates of the two specified vectors. </summary> */
	public static Vector2F Max(Vector2F a, Vector2F b) {
		return new Vector2F(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
	}
	/** <summary> Returns the larger coordinates of the two specified vectors. </summary> */
	public static Vector2F Max(double aX, double aY, Vector2F b) {
		return new Vector2F(Math.Max(aX, b.X), Math.Max(aY, b.Y));
	}
	/** <summary> Returns the larger coordinates of the two specified vectors. </summary> */
	public static Vector2F Max(Vector2F a, double bX, double bY) {
		return new Vector2F(Math.Max(a.X, bX), Math.Max(a.Y, bY));
	}
	/** <summary> Returns the larger coordinates of the two specified vectors. </summary> */
	public static Vector2F Max(double aX, double aY, double bX, double bY) {
		return new Vector2F(Math.Max(aX, bX), Math.Max(aY, bY));
	}

	/** <summary> Returns the larger coordinates of the two specified points. </summary> */
	public static Point2I Max(Point2I a, Point2I b) {
		return new Point2I(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
	}
	/** <summary> Returns the larger coordinates of the two specified points. </summary> */
	public static Point2I Max(int aX, int aY, Point2I b) {
		return new Point2I(Math.Max(aX, b.X), Math.Max(aY, b.Y));
	}
	/** <summary> Returns the larger coordinates of the two specified points. </summary> */
	public static Point2I Max(Point2I a, int bX, int bY) {
		return new Point2I(Math.Max(a.X, bX), Math.Max(a.Y, bY));
	}
	/** <summary> Returns the larger coordinates of the two specified points. </summary> */
	public static Point2I Max(int aX, int aY, int bX, int bY) {
		return new Point2I(Math.Max(aX, bX), Math.Max(aY, bY));
	}

	#endregion
	//--------------------------------
	#region Min

	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static byte Min(byte a, byte b) {
		return Math.Min(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static sbyte Min(sbyte a, sbyte b) {
		return Math.Min(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static short Min(short a, short b) {
		return Math.Min(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static ushort Min(ushort a, ushort b) {
		return Math.Min(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static int Min(int a, int b) {
		return Math.Min(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static uint Min(uint a, uint b) {
		return Math.Min(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static long Min(long a, long b) {
		return Math.Min(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static float Min(ulong a, ulong b) {
		return Math.Min(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static float Min(float a, float b) {
		return Math.Min(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static double Min(double a, double b) {
		return Math.Min(a, b);
	}
	/** <summary> Returns the larger of the two specified numbers. </summary> */
	public static decimal Min(decimal a, decimal b) {
		return Math.Min(a, b);
	}

	/** <summary> Returns the smallest coordinates of the two specified vectors. </summary> */
	public static Vector2F Min(Vector2F a, Vector2F b) {
		return new Vector2F(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
	}
	/** <summary> Returns the smallest coordinates of the two specified vectors. </summary> */
	public static Vector2F Min(double aX, double aY, Vector2F b) {
		return new Vector2F(Math.Min(aX, b.X), Math.Min(aY, b.Y));
	}
	/** <summary> Returns the smallest coordinates of the two specified vectors. </summary> */
	public static Vector2F Min(Vector2F a, double bX, double bY) {
		return new Vector2F(Math.Min(a.X, bX), Math.Min(a.Y, bY));
	}
	/** <summary> Returns the smallest coordinates of the two specified vectors. </summary> */
	public static Vector2F Min(double aX, double aY, double bX, double bY) {
		return new Vector2F(Math.Min(aX, bX), Math.Min(aY, bY));
	}
	
	/** <summary> Returns the smallest coordinates of the two specified points. </summary> */
	public static Point2I Min(Point2I a, Point2I b) {
		return new Point2I(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
	}
	/** <summary> Returns the smallest coordinates of the two specified points. </summary> */
	public static Point2I Min(int aX, int aY, Point2I b) {
		return new Point2I(Math.Min(aX, b.X), Math.Min(aY, b.Y));
	}
	/** <summary> Returns the smallest coordinates of the two specified points. </summary> */
	public static Point2I Min(Point2I a, int bX, int bY) {
		return new Point2I(Math.Min(a.X, bX), Math.Min(a.Y, bY));
	}
	/** <summary> Returns the smallest coordinates of the two specified points. </summary> */
	public static Point2I Min(int aX, int aY, int bX, int bY) {
		return new Point2I(Math.Min(aX, bX), Math.Min(aY, bY));
	}

	#endregion
	//--------------------------------
	#region Clamp

	/** <summary> Restricts the value to be within the specified range. </summary> */
	public static byte Clamp(byte value, byte min, byte max) {
		return Math.Max(min, Math.Min(max, value));
	}
	/** <summary> Restricts the value to be within the specified range. </summary> */
	public static sbyte Clamp(sbyte value, sbyte min, sbyte max) {
		return Math.Max(min, Math.Min(max, value));
	}
	/** <summary> Restricts the value to be within the specified range. </summary> */
	public static short Clamp(short value, short min, short max) {
		return Math.Max(min, Math.Min(max, value));
	}
	/** <summary> Restricts the value to be within the specified range. </summary> */
	public static ushort Clamp(ushort value, ushort min, ushort max) {
		return Math.Max(min, Math.Min(max, value));
	}
	/** <summary> Restricts the value to be within the specified range. </summary> */
	public static int Clamp(int value, int min, int max) {
		return Math.Max(min, Math.Min(max, value));
	}
	/** <summary> Restricts the value to be within the specified range. </summary> */
	public static uint Clamp(uint value, uint min, uint max) {
		return Math.Max(min, Math.Min(max, value));
	}
	/** <summary> Restricts the value to be within the specified range. </summary> */
	public static long Clamp(long value, long min, long max) {
		return Math.Max(min, Math.Min(max, value));
	}
	/** <summary> Restricts the value to be within the specified range. </summary> */
	public static ulong Clamp(ulong value, ulong min, ulong max) {
		return Math.Max(min, Math.Min(max, value));
	}
	/** <summary> Restricts the value to be within the specified range. </summary> */
	public static float Clamp(float value, float min, float max) {
		return Math.Max(min, Math.Min(max, value));
	}
	/** <summary> Restricts the value to be within the specified range. </summary> */
	public static double Clamp(double value, double min, double max) {
		return Math.Max(min, Math.Min(max, value));
	}
	/** <summary> Restricts the value to be within the specified range. </summary> */
	public static decimal Clamp(decimal value, decimal min, decimal max) {
		return Math.Max(min, Math.Min(max, value));
	}

	/** <summary> Restricts the vector to be within the specified range. </summary> */
	public static Vector2F Clamp(Vector2F value, Vector2F min, Vector2F max) {
		return new Vector2F(Math.Max(min.X, Math.Min(max.X, value.X)), Math.Max(min.Y, Math.Min(max.Y, value.Y)));
	}
	/** <summary> Restricts the vector to be within the specified range. </summary> */
	public static Vector2F Clamp(Vector2F value, double minX, double minY, Vector2F max) {
		return new Vector2F(Math.Max(minX, Math.Min(max.X, value.X)), Math.Max(minY, Math.Min(max.Y, value.Y)));
	}
	/** <summary> Restricts the vector to be within the specified range. </summary> */
	public static Vector2F Clamp(Vector2F value, Vector2F min, double maxX, double maxY) {
		return new Vector2F(Math.Max(min.X, Math.Min(maxX, value.X)), Math.Max(min.Y, Math.Min(maxY, value.Y)));
	}
	/** <summary> Restricts the vector to be within the specified range. </summary> */
	public static Vector2F Clamp(Vector2F value, double minX, double minY, double maxX, double maxY) {
		return new Vector2F(Math.Max(minX, Math.Min(maxX, value.X)), Math.Max(minY, Math.Min(maxY, value.Y)));
	}

	/** <summary> Restricts the point to be within the specified range. </summary> */
	public static Point2I Clamp(Point2I value, Point2I min, Point2I max) {
		return new Point2I(Math.Max(min.X, Math.Min(max.X, value.X)), Math.Max(min.Y, Math.Min(max.Y, value.Y)));
	}
	/** <summary> Restricts the point to be within the specified range. </summary> */
	public static Point2I Clamp(Point2I value, int minX, int minY, Point2I max) {
		return new Point2I(Math.Max(minX, Math.Min(max.X, value.X)), Math.Max(minY, Math.Min(max.Y, value.Y)));
	}
	/** <summary> Restricts the point to be within the specified range. </summary> */
	public static Point2I Clamp(Point2I value, Point2I min, int maxX, int maxY) {
		return new Point2I(Math.Max(min.X, Math.Min(maxX, value.X)), Math.Max(min.Y, Math.Min(maxY, value.Y)));
	}
	/** <summary> Restricts the point to be within the specified range. </summary> */
	public static Point2I Clamp(Point2I value, int minX, int minY, int maxX, int maxY) {
		return new Point2I(Math.Max(minX, Math.Min(maxX, value.X)), Math.Max(minY, Math.Min(maxY, value.Y)));
	}

	#endregion
	//--------------------------------
	#region Sign

	/** <summary> Returns a value indicating the sign of the specified number. </summary> */
	public static sbyte Sign(sbyte a) {
		return (sbyte)Math.Sign(a);
	}
	/** <summary> Returns a value indicating the sign of the specified number. </summary> */
	public static short Sign(short a) {
		return (short)Math.Sign(a);
	}
	/** <summary> Returns a value indicating the sign of the specified number. </summary> */
	public static int Sign(int a) {
		return Math.Sign(a);
	}
	/** <summary> Returns a value indicating the sign of the specified number. </summary> */
	public static long Sign(long a) {
		return (long)Math.Sign(a);
	}
	/** <summary> Returns a value indicating the sign of the specified number. </summary> */
	public static float Sign(float a) {
		return (float)Math.Sign(a);
	}
	/** <summary> Returns a value indicating the sign of the specified number. </summary> */
	public static double Sign(double a) {
		return (double)Math.Sign(a);
	}
	/** <summary> Returns a value indicating the sign of the specified number. </summary> */
	public static decimal Sign(decimal a) {
		return (decimal)Math.Sign(a);
	}

	/** <summary> Returns the coordinates indicating the sign of the specified vector. </summary> */
	public static Vector2F Sign(Vector2F a) {
		return new Vector2F(Math.Sign(a.X), Math.Sign(a.Y));
	}
	/** <summary> Returns the coordinates indicating the sign of the specified vector. </summary> */
	public static Vector2F Sign(double aX, double aY) {
		return new Vector2F(Math.Sign(aX), Math.Sign(aY));
	}

	/** <summary> Returns the coordinates indicating the sign of the specified point. </summary> */
	public static Point2I Sign(Point2I a) {
		return new Point2I(Math.Sign(a.X), Math.Sign(a.Y));
	}
	/** <summary> Returns the coordinates indicating the sign of the specified point. </summary> */
	public static Point2I Sign(int aX, int aY) {
		return new Point2I(Math.Sign(aX), Math.Sign(aY));
	}

	#endregion
	//--------------------------------
	#region Abs

	/** <summary> Returns the absolute value of the specified number. </summary> */
	public static sbyte Abs(sbyte a) {
		return Math.Abs(a);
	}
	/** <summary> Returns the absolute value of the specified number. </summary> */
	public static short Abs(short a) {
		return Math.Abs(a);
	}
	/** <summary> Returns the absolute value of the specified number. </summary> */
	public static int Abs(int a) {
		return Math.Abs(a);
	}
	/** <summary> Returns the absolute value of the specified number. </summary> */
	public static long Abs(long a) {
		return Math.Abs(a);
	}
	/** <summary> Returns the absolute value of the specified number. </summary> */
	public static float Abs(float a) {
		return Math.Abs(a);
	}
	/** <summary> Returns the absolute value of the specified number. </summary> */
	public static double Abs(double a) {
		return Math.Abs(a);
	}
	/** <summary> Returns the absolute value of the specified number. </summary> */
	public static decimal Abs(decimal a) {
		return Math.Abs(a);
	}

	/** <summary> Returns the absolute coordinates of the specified vector. </summary> */
	public static Vector2F Abs(Vector2F a) {
		return new Vector2F(Math.Abs(a.X), Math.Abs(a.Y));
	}
	/** <summary> Returns the absolute coordinates of the specified vector. </summary> */
	public static Vector2F Abs(double aX, double aY) {
		return new Vector2F(Math.Abs(aX), Math.Abs(aY));
	}

	/** <summary> Returns the absolute coordinates of the specified point. </summary> */
	public static Point2I Abs(Point2I a) {
		return new Point2I(Math.Abs(a.X), Math.Abs(a.Y));
	}
	/** <summary> Returns the absolute coordinates of the specified point. </summary> */
	public static Point2I Abs(int aX, int aY) {
		return new Point2I(Math.Abs(aX), Math.Abs(aY));
	}

	#endregion
	//--------------------------------
	#region Plusmod

	/** <summary> Returns the positive modulus of the specified number. </summary> */
	public static sbyte Plusmod(sbyte a, sbyte mod) {
		sbyte value = (sbyte)(a % mod);
		return (value < 0 ? (sbyte)(value + mod) : value);
	}
	/** <summary> Returns the positive modulus of the specified number. </summary> */
	public static short Plusmod(short a, short mod) {
		short value = (short)(a % mod);
		return (value < 0 ? (short)(value + mod) : value);
	}
	/** <summary> Returns the positive modulus of the specified number. </summary> */
	public static int Plusmod(int a, int mod) {
		int value = a % mod;
		return (value < 0 ? value + mod : value);
	}
	/** <summary> Returns the positive modulus of the specified number. </summary> */
	public static long Plusmod(long a, long mod) {
		long value = a % mod;
		return (value < 0L ? value + mod : value);
	}
	/** <summary> Returns the positive modulus of the specified number. </summary> */
	public static float Plusmod(float a, float mod) {
		float value = a % mod;
		return (value < 0.0f ? value + mod : value);
	}
	/** <summary> Returns the positive modulus of the specified number. </summary> */
	public static double Plusmod(double a, double mod) {
		double value = a % mod;
		return (value < 0.0d ? value + mod : value);
	}
	/** <summary> Returns the positive modulus of the specified number. </summary> */
	public static decimal Plusmod(decimal a, decimal mod) {
		decimal value = a % mod;
		return (value < 0.0m ? value + mod : value);
	}

	/** <summary> Returns the positive modulus coordinates of the specified vector. </summary> */
	public static Vector2F Plusmod(Vector2F a, double mod) {
		Vector2F value = a % mod;
		return new Vector2F(value.X < 0.0d ? value.X + mod : value.X,
							value.Y < 0.0d ? value.Y + mod : value.Y);
	}
	/** <summary> Returns the positive modulus coordinates of the specified vector. </summary> */
	public static Vector2F Plusmod(Vector2F a, Vector2F mod) {
		Vector2F value = a % mod;
		return new Vector2F(value.X < 0.0d ? value.X + mod.X : value.X,
							value.Y < 0.0d ? value.Y + mod.Y : value.Y);
	}
	/** <summary> Returns the positive modulus coordinates of the specified point. </summary> */
	public static Point2I Plusmod(Point2I a, int mod) {
		Point2I value = a % mod;
		return new Point2I( value.X < 0 ? value.X + mod : value.X,
							value.Y < 0 ? value.Y + mod : value.Y);
	}
	/** <summary> Returns the positive modulus coordinates of the specified point. </summary> */
	public static Point2I Plusmod(Point2I a, Point2I mod) {
		Point2I value = a % mod;
		return new Point2I( value.X < 0 ? value.X + mod.X : value.X,
							value.Y < 0 ? value.Y + mod.Y : value.Y);
	}

	#endregion
	//--------------------------------
	#region IsInt

	/** <summary> Returns true if the specified number is an integer. </summary> */
	public static bool IsInt(float a) {
		return (a % 1 == 0);
	}
	/** <summary> Returns true if the specified number is an integer. </summary> */
	public static bool IsInt(double a) {
		return (a % 1 == 0);
	}
	/** <summary> Returns true if the specified number is an integer. </summary> */
	public static bool IsInt(decimal a) {
		return (a % 1 == 0);
	}

	/** <summary> Returns true if the specified coordinates are integers. </summary> */
	public static bool IsInt(Vector2F a) {
		return ((a.X % 1 == 0) && (a.Y % 1 == 0));
	}
	/** <summary> Returns true if the specified coordinates are integers. </summary> */
	public static bool IsInt(double aX, double aY) {
		return ((aX % 1 == 0) && (aY % 1 == 0));
	}

	#endregion
	//--------------------------------
	#endregion
	//=========== ROUNDING ===========
	#region Rounding
	//--------------------------------
	#region Floor

	/** <summary> Returns the largest integer less than or equal to the specified number. </summary> */
	public static float Floor(float a) {
		return (float)Math.Floor(a);
	}
	/** <summary> Returns the largest integer less than or equal to the specified number, with the base unit. </summary> */
	public static float Floor(float a, float unit) {
		return (float)Math.Floor(a / unit) * unit;
	}
	/** <summary> Returns the largest integer less than or equal to the specified number. </summary> */
	public static double Floor(double a) {
		return Math.Floor(a);
	}
	/** <summary> Returns the largest integer less than or equal to the specified number, with the base unit. </summary> */
	public static double Floor(double a, double unit) {
		return Math.Floor(a / unit) * unit;
	}
	/** <summary> Returns the largest integer less than or equal to the specified number. </summary> */
	public static decimal Floor(decimal a) {
		#if WINDOWS
		return Math.Floor(a);
		#else
		return (decimal)Math.Floor((double)a);
		#endif
	}
	/** <summary> Returns the largest integer less than or equal to the specified number, with the base unit. </summary> */
	public static decimal Floor(decimal a, decimal unit) {
		#if WINDOWS
		return Math.Floor(a / unit) * unit;
		#else
		return (decimal)(Math.Floor((double)a / (double)unit) * (double)unit);
		#endif
	}

	/** <summary> Returns the largest point less than or equal to the specified vector. </summary> */
	public static Vector2F Floor(Vector2F a) {
		return new Vector2F(Math.Floor(a.X), Math.Floor(a.Y));
	}
	/** <summary> Returns the largest point less than or equal to the specified vector, with the base unit. </summary> */
	public static Vector2F Floor(Vector2F a, double unit) {
		return new Vector2F(Math.Floor(a.X / unit) * unit, Math.Floor(a.Y / unit) * unit);
	}
	/** <summary> Returns the largest point less than or equal to the specified vector, with the base unit. </summary> */
	public static Vector2F Floor(Vector2F a, Vector2F unit) {
		return new Vector2F(Math.Floor(a.X / unit.X) * unit.X, Math.Floor(a.Y / unit.Y) * unit.Y);
	}

	#endregion
	//--------------------------------
	#region Ceiling

	/** <summary> Returns the smallest integer greater than or equal to the specified number. </summary> */
	public static float Ceiling(float a) {
		return (float)Math.Ceiling(a);
	}
	/** <summary> Returns the smallest integer greater than or equal to the specified number, with the base unit. </summary> */
	public static float Ceiling(float a, float unit) {
		return (float)Math.Ceiling(a / unit) * unit;
	}
	/** <summary> Returns the smallest integer greater than or equal to the specified number. </summary> */
	public static double Ceiling(double a) {
		return Math.Ceiling(a);
	}
	/** <summary> Returns the smallest integer greater than or equal to the specified number, with the base unit. </summary> */
	public static double Ceiling(double a, double unit) {
		return Math.Ceiling(a / unit) * unit;
	}
	/** <summary> Returns the smallest integer greater than or equal to the specified number. </summary> */
	public static decimal Ceiling(decimal a) {
		#if WINDOWS
		return Math.Ceiling(a);
		#else
		return (decimal)Math.Ceiling((double)a);
		#endif
	}
	/** <summary> Returns the smallest integer greater than or equal to the specified number, with the base unit. </summary> */
	public static decimal Ceiling(decimal a, decimal unit) {
		#if WINDOWS
		return Math.Ceiling(a / unit) * unit;
		#else
		return (decimal)(Math.Ceiling((double)a / (double)unit) * (double)unit);
		#endif
	}

	/** <summary> Returns the smallest point greater than or equal to the specified vector. </summary> */
	public static Vector2F Ceiling(Vector2F a) {
		return new Vector2F(Math.Ceiling(a.X), Math.Ceiling(a.Y));
	}
	/** <summary> Returns the smallest point greater than or equal to the specified vector, with the base unit. </summary> */
	public static Vector2F Ceiling(Vector2F a, double unit) {
		return new Vector2F(Math.Ceiling(a.X / unit) * unit, Math.Ceiling(a.Y / unit) * unit);
	}
	/** <summary> Returns the smallest point greater than or equal to the specified vector, with the base unit. </summary> */
	public static Vector2F Ceiling(Vector2F a, Vector2F unit) {
		return new Vector2F(Math.Ceiling(a.X / unit.X) * unit.X, Math.Ceiling(a.Y / unit.Y) * unit.Y);
	}
	
	#endregion
	//--------------------------------
	#region Round

	/** <summary> Rounds the specified number to the nearest integral value. </summary> */
	public static float Round(float a) {
		return (float)Math.Round(a);
	}
	/** <summary> Rounds the specified number to the nearest integral value, with the base unit. </summary> */
	public static float Round(float a, float unit) {
		return (float)Math.Round(a / unit) * unit;
	}
	/** <summary> Rounds the specified number to the nearest integral value. </summary> */
	public static double Round(double a) {
		return Math.Round(a);
	}
	/** <summary> Rounds the specified number to the nearest integral value, with the base unit. </summary> */
	public static double Round(double a, double unit) {
		return Math.Round(a / unit) * unit;
	}
	/** <summary> Rounds the specified number to the nearest integral value. </summary> */
	public static decimal Round(decimal a) {
		return Math.Round(a);
	}
	/** <summary> Rounds the specified number to the nearest integral value, with the base unit. </summary> */
	public static decimal Round(decimal a, decimal unit) {
		return Math.Round(a / unit) * unit;
	}

	/** <summary> Rounds the specified vector to the nearest integral coordinates. </summary> */
	public static Vector2F Round(Vector2F a) {
		return new Vector2F(Math.Round(a.X), Math.Round(a.Y));
	}
	/** <summary> Rounds the specified vector to the nearest integral coordinates, with the base unit. </summary> */
	public static Vector2F Round(Vector2F a, double unit) {
		return new Vector2F(Math.Round(a.X / unit) * unit, Math.Round(a.Y / unit) * unit);
	}
	/** <summary> Rounds the specified vector to the nearest integral coordinates, with the base unit. </summary> */
	public static Vector2F Round(Vector2F a, Vector2F unit) {
		return new Vector2F(Math.Round(a.X / unit.X) * unit.X, Math.Round(a.Y / unit.Y) * unit.Y);
	}

	#endregion
	//--------------------------------
	#region Truncate

	/** <summary> Rounds the specified value to the nearest integer towards zero. </summary> */
	public static float Truncate(float a) {
		#if WINDOWS
		return (float)Math.Truncate(a);
		#else
		return (a > 0.0f ? (float)Math.Floor(  a) :
						   (float)Math.Ceiling(a));
		#endif
	}
	/** <summary> Rounds the specified value to the nearest integer towards zero, with the base unit. </summary> */
	public static float Truncate(float a, float unit) {
		#if WINDOWS
		return (float)Math.Truncate(a / unit) * unit;
		#else
		return (a > 0.0f ? (float)Math.Floor(  a / unit) * unit :
						   (float)Math.Ceiling(a / unit) * unit);
		#endif
	}
	/** <summary> Rounds the specified value to the nearest integer towards zero. </summary> */
	public static double Truncate(double a) {
		#if WINDOWS
		return Math.Truncate(a);
		#else
		return (a > 0.0d ? Math.Floor(  a) :
						   Math.Ceiling(a));
		#endif
	}
	/** <summary> Rounds the specified value to the nearest integer towards zero, with the base unit. </summary> */
	public static double Truncate(double a, double unit) {
		#if WINDOWS
		return Math.Truncate(a / unit) * unit;
		#else
		return (a > 0.0d ? Math.Floor(  a / unit) * unit :
						   Math.Ceiling(a / unit) * unit);
		#endif
	}
	/** <summary> Rounds the specified value to the nearest integer towards zero. </summary> */
	public static decimal Truncate(decimal a) {
		#if WINDOWS
		return Math.Truncate(a);
		#else
		return (a > 0.0m ? (decimal)Math.Floor(  (double)a) :
						   (decimal)Math.Ceiling((double)a));
		#endif
	}
	/** <summary> Rounds the specified value to the nearest integer towards zero, with the base unit. </summary> */
	public static decimal Truncate(decimal a, decimal unit) {
		#if WINDOWS
		return Math.Truncate(a / unit) * unit;
		#else
		return (a > 0.0m ? (decimal)(Math.Floor(  (double)a / (double)unit) * (double)unit) :
						   (decimal)(Math.Ceiling((double)a / (double)unit) * (double)unit));
		#endif
	}

	/** <summary> Rounds the specified vector to the nearest point towards zero. </summary> */
	public static Vector2F Truncate(Vector2F a) {
		#if WINDOWS
		return new Vector2F(Math.Truncate(a.X), Math.Truncate(a.Y));
		#else
		return new Vector2D(a.X > 0.0d ? Math.Floor(a.X) : Math.Ceiling(a.X),
							a.Y > 0.0d ? Math.Floor(a.Y) : Math.Ceiling(a.Y));
		#endif
	}
	/** <summary> Rounds the specified vector to the nearest point towards zero, with the base unit. </summary> */
	public static Vector2F Truncate(Vector2F a, double unit) {
		#if WINDOWS
		return new Vector2F(Math.Truncate(a.X / unit) * unit,
							Math.Truncate(a.Y / unit) * unit);
		#else
		return new Vector2D(a.X > 0.0d ? Math.Floor(a.X / unit) * unit : Math.Ceiling(a.X / unit) * unit,
							a.Y > 0.0d ? Math.Floor(a.Y / unit) * unit : Math.Ceiling(a.Y / unit) * unit);
		#endif
	}
	/** <summary> Rounds the specified vector to the nearest point towards zero, with the base unit. </summary> */
	public static Vector2F Truncate(Vector2F a, Vector2F unit) {
		#if WINDOWS
		return new Vector2F(Math.Truncate(a.X / unit.X) * unit.X,
							Math.Truncate(a.Y / unit.Y) * unit.Y);
		#else
		return new Vector2D(a.X > 0.0d ? Math.Floor(a.X / unit.X) * unit.X : Math.Ceiling(a.X / unit.X) * unit.X,
							a.Y > 0.0d ? Math.Floor(a.Y / unit.Y) * unit.Y : Math.Ceiling(a.Y / unit.Y) * unit.Y);
		#endif
	}
	
	#endregion
	//--------------------------------
	#region Atruncate

	/** <summary> Rounds the specified value to the farthest integer from zero. </summary> */
	public static float Atruncate(float a) {
		return (a < 0.0f ? (float)Math.Floor(a) : (float)Math.Ceiling(a));
	}
	/** <summary> Rounds the specified value to the farthest integer from zero, with the base unit. </summary> */
	public static float Atruncate(float a, float unit) {
		return (a < 0.0f ? (float)Math.Floor(a / unit) * unit : (float)Math.Ceiling(a / unit) * unit);
	}
	/** <summary> Rounds the specified value to the farthest integer from zero. </summary> */
	public static double Atruncate(double a) {
		return (a < 0.0d ? Math.Floor(a) : Math.Ceiling(a));
	}
	/** <summary> Rounds the specified value to the farthest integer from zero, with the base unit. </summary> */
	public static double Atruncate(double a, double unit) {
		return (a < 0.0d ? Math.Floor(a / unit) * unit : Math.Ceiling(a / unit) * unit);
	}
	/** <summary> Rounds the specified value to the farthest integer from zero. </summary> */
	public static decimal Atruncate(decimal a) {
		#if WINDOWS
		return (a < 0.0m ? Math.Floor(a) : Math.Ceiling(a));
		#else
		return (a < 0.0m ? (decimal)Math.Floor((double)a) : (decimal)Math.Ceiling((double)a));
		#endif
	}
	/** <summary> Rounds the specified value to the farthest integer from zero, with the base unit. </summary> */
	public static decimal Atruncate(decimal a, decimal unit) {
		#if WINDOWS
		return (a < 0.0m ? Math.Floor(a / unit) * unit : Math.Ceiling(a / unit) * unit);
		#else
		return (a < 0.0m ? (decimal)(Math.Floor(  (double)a / (double)unit) * (double)unit) :
						   (decimal)(Math.Ceiling((double)a / (double)unit) * (double)unit));
		#endif
	}

	/** <summary> Rounds the specified vector to the farthest point from zero. </summary> */
	public static Vector2F Atruncate(Vector2F a) {
		return new Vector2F(a.X < 0.0d ? Math.Floor(a.X) : Math.Ceiling(a.X),
							a.Y < 0.0d ? Math.Floor(a.Y) : Math.Ceiling(a.Y));
	}
	/** <summary> Rounds the specified vector to the farthest point from zero, with the base unit. </summary> */
	public static Vector2F Atruncate(Vector2F a, double unit) {
		return new Vector2F(a.X < 0.0d ? Math.Floor(a.X / unit) * unit : Math.Ceiling(a.X / unit) * unit,
							a.Y < 0.0d ? Math.Floor(a.Y / unit) * unit : Math.Ceiling(a.Y / unit) * unit);
	}
	/** <summary> Rounds the specified vector to the farthest point from zero, with the base unit. </summary> */
	public static Vector2F Atruncate(Vector2F a, Vector2F unit) {
		return new Vector2F(a.X < 0.0d ? Math.Floor(a.X / unit.X) * unit.X : Math.Ceiling(a.X / unit.X) * unit.X,
							a.Y < 0.0d ? Math.Floor(a.Y / unit.Y) * unit.Y : Math.Ceiling(a.Y / unit.Y) * unit.Y);
	}

	#endregion
	//--------------------------------
	#endregion
	//========== EXPONENTS ===========
	#region Exponents
	//--------------------------------
	#region Pow

	/** <summary> Returns the specified number raised to the specified power. </summary> */
	public static int Pow(int x, int y) {
		return (int)Math.Pow(x, y);
	}
	/** <summary> Returns the specified number raised to the specified power. </summary> */
	public static uint Pow(uint x, uint y) {
		return (uint)Math.Pow(x, y);
	}
	/** <summary> Returns the specified number raised to the specified power. </summary> */
	public static long Pow(long x, long y) {
		return (long)Math.Pow(x, y);
	}
	/** <summary> Returns the specified number raised to the specified power. </summary> */
	public static ulong Pow(ulong x, ulong y) {
		return (ulong)Math.Pow(x, y);
	}
	/** <summary> Returns the specified number raised to the specified power. </summary> */
	public static float Pow(float x, float y) {
		return (float)Math.Pow(x, y);
	}
	/** <summary> Returns the specified number raised to the specified power. </summary> */
	public static double Pow(double x, double y) {
		return Math.Pow(x, y);
	}

	/** <summary> Returns the specified vector raised to the specified power. </summary> */
	public static Vector2F Pow(Vector2F x, double y) {
		return new Vector2F(Math.Pow(x.X, y), Math.Pow(x.Y, y));
	}
	/** <summary> Returns the specified vector raised to the specified power. </summary> */
	public static Vector2F Pow(Vector2F x, Vector2F y) {
		return new Vector2F(Math.Pow(x.X, y.X), Math.Pow(x.Y, y.Y));
	}
	/** <summary> Returns the specified point raised to the specified power. </summary> */
	public static Point2I Pow(Point2I x, int y) {
		return new Point2I((int)Math.Pow(x.X, y), (int)Math.Pow(x.Y, y));
	}
	/** <summary> Returns the specified point raised to the specified power. </summary> */
	public static Point2I Pow(Point2I x, Point2I y) {
		return new Point2I((int)Math.Pow(x.X, y.X), (int)Math.Pow(x.Y, y.Y));
	}

	#endregion
	//--------------------------------
	#region Root

	/** <summary> Returns a specified number lowered to the specified root. </summary> */
	public static float Root(float x, float y) {
		return (float)Math.Pow(x, 1.0f / y);
	}
	/** <summary> Returns a specified number lowered to the specified root. </summary> */
	public static double Root(double x, double y) {
		return Math.Pow(x, 1.0d / y);
	}

	/** <summary> Returns a specified vector lowered to the specified root. </summary> */
	public static Vector2F Root(Vector2F x, double y) {
		return new Vector2F(Math.Pow(x.X, 1.0d / y), Math.Pow(x.Y, 1.0d / y));
	}
	/** <summary> Returns a specified vector lowered to the specified root. </summary> */
	public static Vector2F Root(Vector2F x, Vector2F y) {
		return new Vector2F(Math.Pow(x.X, 1.0d / y.X), Math.Pow(x.Y, 1.0d / y.Y));
	}

	#endregion
	//--------------------------------
	#region Sqrt

	/** <summary> Returns the square root of a specified number. </summary> */
	public static float Sqrt(float x) {
		return (float)Math.Sqrt(x);
	}
	/** <summary> Returns the square root of a specified number. </summary> */
	public static double Sqrt(double x) {
		return Math.Sqrt(x);
	}

	/** <summary> Returns the square root of a specified vector. </summary> */
	public static Vector2F Sqrt(Vector2F x) {
		return new Vector2F(Math.Sqrt(x.X), Math.Sqrt(x.Y));
	}

	#endregion
	//--------------------------------
	#region Cbrt

	/** <summary> Returns the cube root of a specified number. </summary> */
	public static float Cbrt(float x) {
		return (float)Math.Pow(x, 1.0f / 3.0f);
	}
	/** <summary> Returns the cube root of a specified number. </summary> */
	public static double Cbrt(double x) {
		return Math.Pow(x, 1.0d / 3.0d);
	}

	/** <summary> Returns the cube root of a specified vector. </summary> */
	public static Vector2F Cbrt(Vector2F x) {
		return new Vector2F(Math.Pow(x.X, 1.0d / 3.0d), Math.Pow(x.Y, 1.0d / 3.0d));
	}

	#endregion
	//--------------------------------
	#region Exp

	/** <summary> Returns e raised to the specified power. </summary> */
	public static float Exp(float x) {
		return (float)Math.Exp(x);
	}
	/** <summary> Returns e raised to the specified power. </summary> */
	public static double Exp(double x) {
		return Math.Exp(x);
	}

	/** <summary> Returns e raised to the specified power coordinates. </summary> */
	public static Vector2F Exp(Vector2F x) {
		return new Vector2F(Math.Exp(x.X), Math.Exp(x.Y));
	}

	#endregion
	//--------------------------------
	#endregion
	//========== LOGARITHM ===========
	#region Logarithm
	//--------------------------------
	#region Log e

	/** <summary> Returns the natural (base e) logarithm of the specified number. </summary> */
	public static float Log(float d) {
		return (float)Math.Log(d);
	}
	/** <summary> Returns the natural (base e) logarithm of the specified number. </summary> */
	public static double Log(double d) {
		return Math.Log(d);
	}

	/** <summary> Returns the natural (base e) logarithm of the specified vector. </summary> */
	public static Vector2F Log(Vector2F d) {
		return new Vector2F(Math.Log(d.X), Math.Log(d.Y));
	}

	#endregion
	//--------------------------------
	#region Log (base)

	/** <summary> Returns the specified base logarithm of the specified number. </summary> */
	public static float Log(float a, float newBase) {
		return (float)Math.Log(a, newBase);
	}
	/** <summary> Returns the specified base logarithm of the specified number. </summary> */
	public static double Log(double a, double newBase) {
		return Math.Log(a, newBase);
	}

	/** <summary> Returns the specified base logarithm of the specified vector. </summary> */
	public static Vector2F Log(Vector2F a, double newBase) {
		return new Vector2F(Math.Log(a.X, newBase), Math.Log(a.Y, newBase));
	}
	/** <summary> Returns the specified base logarithm of the specified vector. </summary> */
	public static Vector2F Log(Vector2F a, Vector2F newBase) {
		return new Vector2F(Math.Log(a.X, newBase.X), Math.Log(a.Y, newBase.Y));
	}

	#endregion
	//--------------------------------
	#region Log 10

	/** <summary> Returns the base 10 logarithm of the specified number. </summary> */
	public static float Log10(float d) {
		return (float)Math.Log10(d);
	}
	/** <summary> Returns the base 10 logarithm of the specified number. </summary> */
	public static double Log10(double d) {
		return Math.Log10(d);
	}

	/** <summary> Returns the base 10 logarithm of the specified vector. </summary> */
	public static Vector2F Log10(Vector2F d) {
		return new Vector2F(Math.Log10(d.X), Math.Log10(d.Y));
	}

	#endregion
	//--------------------------------
	#endregion
	//============ ANGLES ============
	#region Angles
	//--------------------------------
	#region Convert Angle

	/** <summary> Converts the given angle into degrees if UsingDegrees is true. </summary> */
	public static float ConvertToDegrees(float angle) {
		return (usingDegrees ? (angle / (float)Pi * 180.0f) : angle);
	}
	/** <summary> Converts the given angle into degrees if UsingDegrees is true. </summary> */
	public static double ConvertToDegrees(double angle) {
		return (usingDegrees ? (angle / Pi * 180.0) : angle);
	}
	/** <summary> Converts the given angle into radians if UsingDegrees is true. </summary> */
	public static float ConvertToRadians(float angle) {
		return (usingDegrees ? (angle / 180.0f * (float)Pi) : angle);
	}
	/** <summary> Converts the given angle into radians if UsingDegrees is true. </summary> */
	public static double ConvertToRadians(double angle) {
		return (usingDegrees ? (angle / 180.0 * Pi) : angle);
	}
	/** <summary> Converts the specified angle from radians into degrees. </summary> */
	public static float ToDegrees(float angle) {
		return angle / (float)Pi * 180.0f;
	}
	/** <summary> Converts the specified angle from radians into degrees. </summary> */
	public static double ToDegrees(double angle) {
		return angle / Pi * 180.0;
	}
	/** <summary> Converts the specified angle from degrees into radians. </summary> */
	public static float ToRadians(float angle) {
		return angle / 180.0f * (float)Pi;
	}
	/** <summary> Converts the specified angle from degrees into radians. </summary> */
	public static double ToRadians(double angle) {
		return angle / 180.0 * Pi;
	}

	#endregion
	//--------------------------------
	#region Modulus Angle

	/** <summary> Returns the absolute angle as (0, 360]. </summary> */
	public static float Plusdir(float angle) {
		return Plusmod(angle, (float)FullAngle);
	}
	/** <summary> Returns the absolute angle as (0, 360]. </summary> */
	public static double Plusdir(double angle) {
		return Plusmod(angle, FullAngle);
	}
	/** <summary> Returns the absolute angle as [0, 360). </summary> */
	public static float Plusdir2(float angle) {
		float value = Plusmod(angle, (float)FullAngle);
		if (value == 0.0f)
			return (float)FullAngle;
		return value;
	}
	/** <summary> Returns the absolute angle as [0, 360). </summary> */
	public static double Plusdir2(double angle) {
		double value = Plusmod(angle, FullAngle);
		if (value == 0.0d)
			return FullAngle;
		return value;
	}
	/** <summary> Returns the absolute angle as [-180, 180). </summary> */
	public static float Plusdir3(float angle) {
		float value = Plusmod(angle, (float)FullAngle);
		if (value > (float)HalfAngle)
			return value - (float)FullAngle;
		return value;
	}
	/** <summary> Returns the absolute angle as [-180, 180). </summary> */
	public static double Plusdir3(double angle) {
		double value = Plusmod(angle, FullAngle);
		if (value > HalfAngle)
			return value - FullAngle;
		return value;
	}
	/** <summary> Returns the change in direction between two angles. </summary> */
	public static float DeltaDirection(float source, float destination) {
		float dir = Plusdir(destination - source);
		if (dir > (float)HalfAngle)
			return dir - (float)FullAngle;
		return dir;
	}
	/** <summary> Returns the change in direction between two angles. </summary> */
	public static double DeltaDirection(double source, double destination) {
		double dir = Plusdir(destination - source);
		if (dir > HalfAngle)
			return dir - FullAngle;
		return dir;
	}

	#endregion
	//--------------------------------
	#endregion
	//========= TRIGONOMETRY =========
	#region Trigonometry
	//--------------------------------
	#region Trigonometric

	/** <summary> Returns the sine of the specified angle. </summary> */
	public static float Sin(float a) {
		return (float)Math.Sin(ConvertToRadians(a));
	}
	/** <summary> Returns the sine of the specified angle. </summary> */
	public static double Sin(double a) {
		return Math.Sin(ConvertToRadians(a));
	}
	/** <summary> Returns the cosine of the specified angle. </summary> */
	public static float Cos(float a) {
		return (float)Math.Cos(ConvertToRadians(a));
	}
	/** <summary> Returns the cosine of the specified angle. </summary> */
	public static double Cos(double a) {
		return Math.Cos(ConvertToRadians(a));
	}
	/** <summary> Returns the tangent of the specified angle. </summary> */
	public static float Tan(float a) {
		return (float)Math.Tan(ConvertToRadians(a));
	}
	/** <summary> Returns the tangent of the specified angle. </summary> */
	public static double Tan(double a) {
		return Math.Tan(ConvertToRadians(a));
	}

	#endregion
	//--------------------------------
	#region Arc Trigonometric

	/** <summary> Returns the angle whose sine is the specified number. </summary> */
	public static float Asin(float d) {
		return ConvertToDegrees((float)Math.Asin(d));
	}
	/** <summary> Returns the angle whose sine is the specified number. </summary> */
	public static double Asin(double d) {
		return ConvertToDegrees(Math.Asin(d));
	}

	/** <summary> Returns the angle whose cosine is the specified number. </summary> */
	public static float Acos(float d) {
		return ConvertToDegrees((float)Math.Acos(d));
	}
	/** <summary> Returns the angle whose cosine is the specified number. </summary> */
	public static double Acos(double d) {
		return ConvertToDegrees(Math.Acos(d));
	}

	/** <summary> Returns the angle whose tangent is the specified number. </summary> */
	public static float Atan(float d) {
		return ConvertToDegrees((float)Math.Atan(d));
	}
	/** <summary> Returns the angle whose tangent is the specified number. </summary> */
	public static double Atan(double d) {
		return ConvertToDegrees(Math.Atan(d));
	}

	/** <summary> Returns the angle whose tangent is the quotient of two specified numbers. </summary> */
	public static float Atan2(float y, float x) {
		return ConvertToDegrees((float)Math.Atan2(y, x));
	}
	/** <summary> Returns the angle whose tangent is the quotient of two specified numbers. </summary> */
	public static double Atan2(double y, double x) {
		return ConvertToDegrees(Math.Atan2(y, x));
	}

	#endregion
	//--------------------------------
	#region Hyperbolic

	/** <summary> Returns the hyperbolic sine of the specified angle. </summary> */
	public static float Sinh(float a) {
		return (float)Math.Sinh(a);
	}
	/** <summary> Returns the hyperbolic sine of the specified angle. </summary> */
	public static double Sinh(double a) {
		return Math.Sinh(a);
	}

	/** <summary> Returns the hyperbolic cosine of the specified angle. </summary> */
	public static float Cosh(float a) {
		return (float)Math.Cosh(a);
	}
	/** <summary> Returns the hyperbolic cosine of the specified angle. </summary> */
	public static double Cosh(double a) {
		return Math.Cosh(a);
	}

	/** <summary> Returns the hyperbolic tangent of the specified angle. </summary> */
	public static float Tanh(float a) {
		return (float)Math.Tanh(a);
	}
	/** <summary> Returns the hyperbolic tangent of the specified angle. </summary> */
	public static double Tanh(double a) {
		return Math.Tanh(a);
	}

	#endregion
	//--------------------------------
	#endregion
}
} // End namespace
