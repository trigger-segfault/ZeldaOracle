using System;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>A static class for advanced game-related mathematical functions and
	/// calculations.</summary>
	public static partial class GMath {

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

		// Sign (Integer) -------------------------------------------------------------

		/// <summary>Returns a value indicating the sign of the specified number.
		/// Returns an integer.</summary>
		public static int SignI(float a) {
			return Math.Sign(a);
		}

		/// <summary>Returns a value indicating the sign of the specified number.
		/// Returns an integer.</summary>
		public static int SignI(double a) {
			return Math.Sign(a);
		}

		/// <summary>Returns a value indicating the sign of the specified number.
		/// Returns an integer.</summary>
		public static int SignI(decimal a) {
			return Math.Sign(a);
		}

		/// <summary>Returns the coordinates indicating the sign of the specified
		/// vector. Returns a point.</summary>
		public static Point2I SignI(Vector2F a) {
			return new Point2I(Math.Sign(a.X), Math.Sign(a.Y));
		}

		/// <summary>Returns the coordinates indicating the sign of the specified
		/// vector. Returns a point.</summary>
		public static Point2I SignI(float aX, float aY) {
			return new Point2I(Math.Sign(aX), Math.Sign(aY));
		}

		// Sign2 ----------------------------------------------------------------------

		/// <summary>Returns a value indicating the sign of the specified number.
		/// Returns 1 if the value is zero.</summary>
		public static sbyte Sign2(sbyte a) {
			sbyte sign = (sbyte) Math.Sign(a);
			return (sign == 0 ? (sbyte) 1 : sign);
		}

		/// <summary>Returns a value indicating the sign of the specified number.
		/// Returns 1 if the value is zero.</summary>
		public static short Sign2(short a) {
			short sign = (short) Math.Sign(a);
			return (sign == 0 ? (short) 1 : sign);
		}

		/// <summary>Returns a value indicating the sign of the specified number.
		/// Returns 1 if the value is zero.</summary>
		public static int Sign2(int a) {
			int sign = Math.Sign(a);
			return (sign == 0 ? 1 : sign);
		}

		/// <summary>Returns a value indicating the sign of the specified number.
		/// Returns 1 if the value is zero.</summary>
		public static long Sign2(long a) {
			long sign = Math.Sign(a);
			return (sign == 0 ? 1 : sign);
		}

		/// <summary>Returns a value indicating the sign of the specified number.
		/// Returns 1 if the value is zero.</summary>
		public static float Sign2(float a) {
			float sign = Math.Sign(a);
			return (sign == 0 ? 1.0f : sign);
		}

		/// <summary>Returns a value indicating the sign of the specified number.
		/// Returns 1 if the value is zero.</summary>
		public static double Sign2(double a) {
			double sign = Math.Sign(a);
			return (sign == 0 ? 1.0d : sign);
		}

		/// <summary>Returns a value indicating the sign of the specified number.
		/// Returns 1 if the value is zero.</summary>
		public static decimal Sign2(decimal a) {
			decimal sign = Math.Sign(a);
			return (sign == 0 ? 1.0m : sign);
		}

		/// <summary>Returns the coordinates indicating the sign of the specified
		/// vector. Returns 1 if the value is zero.</summary>
		public static Vector2F Sign2(Vector2F a) {
			float signX = Math.Sign(a.X);
			float signY = Math.Sign(a.Y);
			return new Vector2F((signX == 0 ? 1f : signX),
								(signY == 0 ? 1f : signY));
		}

		/// <summary>Returns the coordinates indicating the sign of the specified
		/// vector. Returns 1 if the value is zero.</summary>
		public static Vector2F Sign2(float aX, float aY) {
			float signX = Math.Sign(aX);
			float signY = Math.Sign(aY);
			return new Vector2F((signX == 0 ? 1f : signX),
								(signY == 0 ? 1f : signY));
		}

		/// <summary>Returns the coordinates indicating the sign of the specified
		/// point. Returns 1 if the value is zero.</summary>
		public static Point2I Sign2(Point2I a) {
			int signX = Math.Sign(a.X);
			int signY = Math.Sign(a.Y);
			return new Point2I(	(signX == 0 ? 1 : signX),
								(signY == 0 ? 1 : signY));
		}

		/// <summary>Returns the coordinates indicating the sign of the specified
		/// point. Returns 1 if the value is zero.</summary>
		public static Point2I Sign2(int aX, int aY) {
			int signX = Math.Sign(aX);
			int signY = Math.Sign(aY);
			return new Point2I(	(signX == 0 ? 1 : signX),
								(signY == 0 ? 1 : signY));
		}

		// Sign2 (Integer) ------------------------------------------------------------

		/// <summary>Returns a value indicating the sign of the specified number.
		/// Returns 1 if the value is zero and returns an integer.</summary>
		public static int Sign2I(float a) {
			int sign = Math.Sign(a);
			return (sign == 0 ? 1 : sign);
		}

		/// <summary>Returns a value indicating the sign of the specified number.
		/// Returns 1 if the value is zero and returns an integer.</summary>
		public static int Sign2I(double a) {
			int sign = Math.Sign(a);
			return (sign == 0 ? 1 : sign);
		}

		/// <summary>Returns a value indicating the sign of the specified number.
		/// Returns 1 if the value is zero and returns an integer.</summary>
		public static int Sign2I(decimal a) {
			int sign = Math.Sign(a);
			return (sign == 0 ? 1 : sign);
		}

		/// <summary>Returns the coordinates indicating the sign of the specified
		/// vector. Returns 1 if the value is zero and returns a point.</summary>
		public static Point2I Sign2I(Vector2F a) {
			int signX = Math.Sign(a.X);
			int signY = Math.Sign(a.Y);
			return new Point2I((signX == 0 ? 1 : signX),
								(signY == 0 ? 1 : signY));
		}

		/// <summary>Returns the coordinates indicating the sign of the specified
		/// vector. Returns 1 if the value is zero and returns a point.</summary>
		public static Point2I Sign2I(float aX, float aY) {
			int signX = Math.Sign(aX);
			int signY = Math.Sign(aY);
			return new Point2I((signX == 0 ? 1 : signX),
								(signY == 0 ? 1 : signY));
		}
	}
}
