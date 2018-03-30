using System;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>A static class for advanced game-related mathematical functions and
	/// calculations.</summary>
	public static partial class GMath {
		
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
			return new Vector2F((float) Math.Pow(x.X, y),
								(float) Math.Pow(x.Y, y));
		}

		/// <summary>Returns the specified vector raised to the specified power.</summary>
		public static Vector2F Pow(Vector2F x, Vector2F y) {
			return new Vector2F((float) Math.Pow(x.X, y.X),
								(float) Math.Pow(x.Y, y.Y));
		}

		/// <summary>Returns the specified point raised to the specified power.</summary>
		public static Point2I Pow(Point2I x, int y) {
			return new Point2I(	(int) Math.Pow(x.X, y),
								(int) Math.Pow(x.Y, y));
		}

		/// <summary>Returns the specified point raised to the specified power.</summary>
		public static Point2I Pow(Point2I x, Point2I y) {
			return new Point2I(	(int) Math.Pow(x.X, y.X),
								(int) Math.Pow(x.Y, y.Y));
		}

		// Root -----------------------------------------------------------------------

		/// <summary>Returns a specified number lowered to the specified root.</summary>
		public static float Root(float x, float y) {
			return (float) Math.Pow(x, 1.0d / y);
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
			return new Vector2F((float) Math.Sqrt(x.X),
								(float) Math.Sqrt(x.Y));
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
			return new Vector2F((float) Math.Pow(x.X, 1.0d / 3.0d),
								(float) Math.Pow(x.Y, 1.0d / 3.0d));
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
			return new Vector2F((float) Math.Exp(x.X),
								(float) Math.Exp(x.Y));
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
			return new Vector2F((float) Math.Log(d.X),
								(float) Math.Log(d.Y));
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
			return new Vector2F((float) Math.Log10(d.X),
								(float) Math.Log10(d.Y));
		}
		
	}
}
