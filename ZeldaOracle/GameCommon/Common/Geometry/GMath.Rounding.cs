using System;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>A static class for advanced game-related mathematical functions and
	/// calculations.</summary>
	public static partial class GMath {

		//-----------------------------------------------------------------------------
		// Rounding (Normal Return)
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
			return new Vector2F((float) Math.Floor(a.X),
								(float) Math.Floor(a.Y));
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
			return new Vector2F((float) Math.Ceiling(a.X),
								(float) Math.Ceiling(a.Y));
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
			return new Vector2F((float) Math.Round(a.X),
								(float) Math.Round(a.Y));
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
			return new Vector2F((float) Math.Truncate(a.X),
								(float) Math.Truncate(a.Y));
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
			return (a < 0.0f ?	(float) Math.Floor(  a / unit) * unit :
								(float) Math.Ceiling(a / unit) * unit);
		}

		/// <summary>Rounds the specified value to the farthest integer from zero.</summary>
		public static double Atruncate(double a) {
			return (a < 0.0d ? Math.Floor(a) : Math.Ceiling(a));
		}

		/// <summary>Rounds the specified value to the farthest integer from zero, with
		/// the base unit.</summary>
		public static double Atruncate(double a, double unit) {
			return (a < 0.0d ?	Math.Floor(  a / unit) * unit :
								Math.Ceiling(a / unit) * unit);
		}

		/// <summary>Rounds the specified value to the farthest integer from zero.</summary>
		public static decimal Atruncate(decimal a) {
			return (a < 0.0m ? Math.Floor(a) : Math.Ceiling(a));
		}

		/// <summary>Rounds the specified value to the farthest integer from zero, with
		/// the base unit.</summary>
		public static decimal Atruncate(decimal a, decimal unit) {
			return (a < 0.0m ?  Math.Floor(  a / unit) * unit :
								Math.Ceiling(a / unit) * unit);
		}

		/// <summary>Rounds the specified vector to the farthest point from zero.</summary>
		public static Vector2F Atruncate(Vector2F a) {
			return new Vector2F(a.X < 0.0f ? (float) Math.Floor(  a.X) :
											 (float) Math.Ceiling(a.X),
								a.Y < 0.0f ? (float) Math.Floor(  a.Y) :
											 (float) Math.Ceiling(a.Y));
		}

		/// <summary>Rounds the specified vector to the farthest point from zero, with
		/// the base unit.</summary>
		public static Vector2F Atruncate(Vector2F a, float unit) {
			return new Vector2F(a.X < 0.0f ? (float) Math.Floor(  a.X / unit)
												* unit :
											 (float) Math.Ceiling(a.X / unit)
												* unit,
								a.Y < 0.0f ? (float) Math.Floor(  a.Y / unit)
												* unit :
											 (float) Math.Ceiling(a.Y / unit)
												* unit);
		}

		/// <summary>Rounds the specified vector to the farthest point from zero, with
		/// the base unit.</summary>
		public static Vector2F Atruncate(Vector2F a, Vector2F unit) {
			return new Vector2F(a.X < 0.0f ? (float) Math.Floor(  a.X / unit.X)
												* unit.X :
											 (float) Math.Ceiling(a.X / unit.X)
												* unit.X,
								a.Y < 0.0f ? (float) Math.Floor(  a.Y / unit.Y)
												* unit.Y :
											 (float) Math.Ceiling(a.Y / unit.Y)
												* unit.Y);
		}


		//-----------------------------------------------------------------------------
		// Rounding (Integer Return)
		//-----------------------------------------------------------------------------

		// Floor ----------------------------------------------------------------------

		/// <summary>Returns the largest integer less than or equal to the specified
		/// number. Returns an integer.</summary>
		public static int FloorI(float a) {
			return (int) Math.Floor(a);
		}
		
		/// <summary>Returns the largest integer less than or equal to the specified
		/// number, with the base unit. Returns an integer.</summary>
		public static float FloorI(float a, int unit) {
			return (int) Math.Floor(a / unit) * unit;
		}

		/// <summary>Returns the largest integer less than or equal to the specified
		/// number. Returns an integer.</summary>
		public static int FloorI(double a) {
			return (int) Math.Floor(a);
		}

		/// <summary>Returns the largest integer less than or equal to the specified
		/// number, with the base unit. Returns an integer.</summary>
		public static int FloorI(double a, int unit) {
			return (int) Math.Floor(a / unit) * unit;
		}

		/// <summary>Returns the largest integer less than or equal to the specified
		/// number. Returns an integer.</summary>
		public static int FloorI(decimal a) {
			return (int) Math.Floor(a);
		}

		/// <summary>Returns the largest integer less than or equal to the specified
		/// number, with the base unit. Returns an integer.</summary>
		public static int FloorI(decimal a, int unit) {
			return (int) Math.Floor(a / unit) * unit;
		}

		/// <summary>Returns the largest point less than or equal to the specified
		/// vector. Returns a point</summary>
		public static Point2I FloorI(Vector2F a) {
			return new Point2I(	(int) Math.Floor(a.X),
								(int) Math.Floor(a.Y));
		}
		
		/// <summary>Returns the largest point less than or equal to the specified
		/// vector, with the base unit. Returns a point.</summary>
		public static Point2I FloorI(Vector2F a, int unit) {
			return new Point2I(	(int) Math.Floor(a.X / unit) * unit,
								(int) Math.Floor(a.Y / unit) * unit);
		}

		/// <summary>Returns the largest point less than or equal to the specified
		/// vector, with the base unit. Returns a point.</summary>
		public static Point2I FloorI(Vector2F a, Point2I unit) {
			return new Point2I(	(int) Math.Floor(a.X / unit.X) * unit.X,
								(int) Math.Floor(a.Y / unit.Y) * unit.Y);
		}

		// Ceiling --------------------------------------------------------------------

		/// <summary>Returns the smallest integer greater than or equal to the
		/// specified number. Returns an integer</summary>
		public static int CeilingI(float a) {
			return (int) Math.Ceiling(a);
		}


		/// <summary>Returns the smallest integer greater than or equal to the
		/// specified number, with the base unit. Returns an integer.</summary>
		public static int CeilingI(float a, int unit) {
			return (int) Math.Ceiling(a / unit) * unit;
		}

		/// <summary>Returns the smallest integer greater than or equal to the
		/// specified number. Returns an integer</summary>
		public static int CeilingI(double a) {
			return (int) Math.Ceiling(a);
		}

		/// <summary>Returns the smallest integer greater than or equal to the
		/// specified number, with the base unit. Returns an integer.</summary>
		public static int CeilingI(double a, int unit) {
			return (int) Math.Ceiling(a / unit) * unit;
		}

		/// <summary>Returns the smallest integer greater than or equal to the
		/// specified number. Returns an integer</summary>
		public static int CeilingI(decimal a) {
			return (int) Math.Ceiling(a);
		}

		/// <summary>Returns the smallest integer greater than or equal to the
		/// specified number, with the base unit. Returns an integer.</summary>
		public static int CeilingI(decimal a, int unit) {
			return (int) Math.Ceiling(a / unit) * unit;
		}

		/// <summary>Returns the smallest point greater than or equal to the
		/// specified vector. Returns a point.</summary>
		public static Point2I CeilingI(Vector2F a) {
			return new Point2I(	(int) Math.Ceiling(a.X),
								(int) Math.Ceiling(a.Y));
		}

		/// <summary>Returns the smallest point greater than or equal to the specified
		/// vector, with the base unit. Returns a point.</summary>
		public static Point2I CeilingI(Vector2F a, int unit) {
			return new Point2I(	(int) Math.Ceiling(a.X / unit) * unit,
								(int) Math.Ceiling(a.Y / unit) * unit);
		}

		/// <summary>Returns the smallest point greater than or equal to the specified
		/// vector, with the base unit. Returns a point.</summary>
		public static Point2I CeilingI(Vector2F a, Point2I unit) {
			return new Point2I(	(int) Math.Ceiling(a.X / unit.X) * unit.X,
								(int) Math.Ceiling(a.Y / unit.Y) * unit.Y);
		}

		// Round ----------------------------------------------------------------------

		/// <summary>Rounds the specified number to the nearest integral value.
		/// Returns an integer.</summary>
		public static int RoundI(float a) {
			return (int) Math.Round(a);
		}

		/// <summary>Rounds the specified number to the nearest integral value,
		/// with the base unit. Returns an integer.</summary>
		public static int RoundI(float a, int unit) {
			return (int) Math.Round(a / unit) * unit;
		}

		/// <summary>Rounds the specified number to the nearest integral value.
		/// Returns an integer.</summary>
		public static int RoundI(double a) {
			return (int) Math.Round(a);
		}

		/// <summary>Rounds the specified number to the nearest integral value,
		/// with the base unit. Returns an integer.</summary>
		public static int RoundI(double a, int unit) {
			return (int) Math.Round(a / unit) * unit;
		}

		/// <summary>Rounds the specified number to the nearest integral value.
		/// Returns an integer.</summary>
		public static int RoundI(decimal a) {
			return (int) Math.Round(a);
		}

		/// <summary>Rounds the specified number to the nearest integral value,
		/// with the base unit. Returns an integer.</summary>
		public static int RoundI(decimal a, int unit) {
			return (int) Math.Round(a / unit) * unit;
		}

		/// <summary>Rounds the specified vector to the nearest integral coordinates.
		/// Returns a point.</summary>
		public static Point2I RoundI(Vector2F a) {
			return new Point2I(	(int) Math.Round(a.X),
								(int) Math.Round(a.Y));
		}

		/// <summary>Rounds the specified vector to the nearest integral coordinates,
		/// with the base unit. Returns a point.</summary>
		public static Point2I RoundI(Vector2F a, int unit) {
			return new Point2I(	(int) Math.Round(a.X / unit) * unit,
								(int) Math.Round(a.Y / unit) * unit);
		}

		/// <summary>Rounds the specified vector to the nearest integral coordinates,
		/// with the base unit. Returns a point.</summary>
		public static Point2I RoundI(Vector2F a, Point2I unit) {
			return new Point2I(	(int) Math.Round(a.X / unit.X) * unit.X,
								(int) Math.Round(a.Y / unit.Y) * unit.Y);
		}
		
		// Truncate -------------------------------------------------------------------

		/// <summary>Rounds the specified value to the nearest integer towards zero.
		/// Returns an integer.</summary>
		public static int TruncateI(float a) {
			return (int) Math.Truncate(a);
		}

		/// <summary>Rounds the specified value to the nearest integer towards zero,
		/// with the base unit. Returns an integer.</summary>
		public static int TruncateI(float a, int unit) {
			return (int) Math.Truncate(a / unit) * unit;
		}

		/// <summary>Rounds the specified value to the nearest integer towards zero.
		/// Returns an integer.</summary>
		public static int TruncateI(double a) {
			return (int) Math.Truncate(a);
		}

		/// <summary>Rounds the specified value to the nearest integer towards zero,
		/// with the base unit. Returns an integer.</summary>
		public static int TruncateI(double a, int unit) {
			return (int) Math.Truncate(a / unit) * unit;
		}

		/// <summary>Rounds the specified value to the nearest integer towards zero.
		/// Returns an integer.</summary>
		public static int TruncateI(decimal a) {
			return (int) Math.Truncate(a);
		}

		/// <summary>Rounds the specified value to the nearest integer towards zero,
		/// with the base unit. Returns an integer.</summary>
		public static int TruncateI(decimal a, int unit) {
			return (int) Math.Truncate(a / unit) * unit;
		}

		/// <summary>Rounds the specified vector to the nearest point towards zero.
		/// Returns a point.</summary>
		public static Point2I TruncateI(Vector2F a) {
			return new Point2I(	(int) Math.Truncate(a.X),
								(int) Math.Truncate(a.Y));
		}

		/// <summary>Rounds the specified vector to the nearest point towards zero,
		/// with the base unit. Returns a point.</summary>
		public static Point2I TruncateI(Vector2F a, int unit) {
			return new Point2I(	(int) Math.Truncate(a.X / unit) * unit,
								(int) Math.Truncate(a.Y / unit) * unit);
		}

		/// <summary>Rounds the specified vector to the nearest point towards zero,
		/// with the base unit. Returns a point.</summary>
		public static Point2I TruncateI(Vector2F a, Point2I unit) {
			return new Point2I(	(int) Math.Truncate(a.X / unit.X) * unit.X,
								(int) Math.Truncate(a.Y / unit.Y) * unit.Y);
		}

		// Atruncate ------------------------------------------------------------------

		/// <summary>Rounds the specified value to the farthest integer from zero.
		/// Returns an integer.</summary>
		public static int AtruncateI(float a) {
			return (int) (a < 0.0f ? Math.Floor(a) : Math.Ceiling(a));
		}

		/// <summary>Rounds the specified value to the farthest integer from zero, with
		/// the base unit. Returns an integer.</summary>
		public static int AtruncateI(float a, int unit) {
			return (int) (a < 0.0f ? Math.Floor(  a / unit) * unit :
									 Math.Ceiling(a / unit) * unit);
		}

		/// <summary>Rounds the specified value to the farthest integer from zero.
		/// Returns an integer.</summary>
		public static int AtruncateI(double a) {
			return (int) (a < 0.0d ? Math.Floor(a) : Math.Ceiling(a));
		}

		/// <summary>Rounds the specified value to the farthest integer from zero, with
		/// the base unit. Returns an integer.</summary>
		public static int AtruncateI(double a, int unit) {
			return (int) (a < 0.0d ? Math.Floor(  a / unit) * unit :
									 Math.Ceiling(a / unit) * unit);
		}

		/// <summary>Rounds the specified value to the farthest integer from zero.
		/// Returns an integer.</summary>
		public static int AtruncateI(decimal a) {
			return (int) (a < 0.0m ? Math.Floor(a) : Math.Ceiling(a));
		}

		/// <summary>Rounds the specified value to the farthest integer from zero, with
		/// the base unit. Returns an integer.</summary>
		public static int AtruncateI(decimal a, int unit) {
			return (int) (a < 0.0m ? Math.Floor(  a / unit) * unit :
									 Math.Ceiling(a / unit) * unit);
		}

		/// <summary>Rounds the specified vector to the farthest point from zero.
		/// Returns a point.</summary>
		public static Point2I AtruncateI(Vector2F a) {
			return new Point2I(	a.X < 0.0f ? (int) Math.Floor(  a.X) :
											 (int) Math.Ceiling(a.X),
								a.Y < 0.0f ? (int) Math.Floor(  a.Y) :
											 (int) Math.Ceiling(a.Y));
		}

		/// <summary>Rounds the specified vector to the farthest point from zero, with
		/// the base unit. Returns a point.</summary>
		public static Point2I AtruncateI(Vector2F a, int unit) {
			return new Point2I( a.X < 0.0f ? (int) Math.Floor(  a.X / unit)
												* unit :
											 (int) Math.Ceiling(a.X / unit)
												* unit,
								a.Y < 0.0f ? (int) Math.Floor(  a.Y / unit)
												* unit :
											 (int) Math.Ceiling(a.Y / unit)
												* unit);
		}

		/// <summary>Rounds the specified vector to the farthest point from zero, with
		/// the base unit. Returns a point.</summary>
		public static Point2I AtruncateI(Vector2F a, Point2I unit) {
			return new Point2I( a.X < 0.0f ? (int) Math.Floor(  a.X / unit.X)
												* unit.X :
											 (int) Math.Ceiling(a.X / unit.X)
												* unit.X,
								a.Y < 0.0f ? (int) Math.Floor(  a.Y / unit.Y)
												* unit.Y :
											 (int) Math.Ceiling(a.Y / unit.Y)
												* unit.Y);
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

		/// <summary>Returns true if either of the specified coordinates are integers.</summary>
		public static bool IsAnyInt(Vector2F a) {
			return ((a.X % 1 == 0) || (a.Y % 1 == 0));
		}

		/// <summary>Returns true if either of the specified coordinates are integers.</summary>
		public static bool IsAnyInt(float aX, float aY) {
			return ((aX % 1 == 0) || (aY % 1 == 0));
		}

		// Floor Divide ---------------------------------------------------------------

		/// <summary>Divides 'a' by all parameters while flooring the result.</summary>
		public static int FloorDiv(int a, int b) {
			return (int) Math.Floor((float) a / b);
		}

		/// <summary>Divides 'a' by all parameters while flooring the result.</summary>
		public static int FloorDiv(int a, int b, int c) {
			return (int) Math.Floor((float) a / b / c);
		}

		/// <summary>Divides 'a' by all parameters while flooring the result.</summary>
		public static int FloorDiv(int a, int b, int c, int d) {
			return (int) Math.Floor((float) a / b / c / d);
		}

		/// <summary>Divides 'a' by all parameters while flooring the result.</summary>
		public static Point2I FloorDiv(Point2I a, Point2I b) {
			return new Point2I(	(int) Math.Floor((float) a.X / b.X),
								(int) Math.Floor((float) a.Y / b.Y));
		}

		/// <summary>Divides 'a' by all parameters while flooring the result.</summary>
		public static Point2I FloorDiv(Point2I a, int b) {
			return new Point2I(	(int) Math.Floor((float) a.X / b),
								(int) Math.Floor((float) a.Y / b));
		}

		/// <summary>Divides 'a' by all parameters while flooring the result.</summary>
		public static Point2I FloorDiv(Point2I a, Point2I b, Point2I c) {
			return new Point2I(	(int) Math.Floor((float) a.X / b.X / c.X),
								(int) Math.Floor((float) a.Y / b.Y / c.Y));
		}

		/// <summary>Divides 'a' by all parameters while flooring the result.</summary>
		public static Point2I FloorDiv(Point2I a, int b, Point2I c) {
			return new Point2I(	(int) Math.Floor((float) a.X / b / c.X),
								(int) Math.Floor((float) a.Y / b / c.Y));
		}

		/// <summary>Divides 'a' by all parameters while flooring the result.</summary>
		public static Point2I FloorDiv(Point2I a, Point2I b, int c) {
			return new Point2I(	(int) Math.Floor((float) a.X / b.X / c),
								(int) Math.Floor((float) a.Y / b.Y / c));
		}

		/// <summary>Divides 'a' by all parameters while flooring the result.</summary>
		public static Point2I FloorDiv(Point2I a, int b, int c) {
			return new Point2I(	(int) Math.Floor((float) a.X / b / c),
								(int) Math.Floor((float) a.Y / b / c));
		}

		// Ceiling Divide -------------------------------------------------------------

		/// <summary>Divides 'a' by all parameters while ceiling the result.</summary>
		public static int CeilingDiv(int a, int b) {
			return (int) Math.Ceiling((float) a / b);
		}

		/// <summary>Divides 'a' by all parameters while ceiling the result.</summary>
		public static int CeilingDiv(int a, int b, int c) {
			return (int) Math.Ceiling((float) a / b / c);
		}

		/// <summary>Divides 'a' by all parameters while ceiling the result.</summary>
		public static int CeilingDiv(int a, int b, int c, int d) {
			return (int) Math.Ceiling((float) a / b / c / d);
		}

		/// <summary>Divides 'a' by all parameters while ceiling the result.</summary>
		public static Point2I CeilingDiv(Point2I a, Point2I b) {
			return new Point2I(	(int) Math.Ceiling((float) a.X / b.X),
								(int) Math.Ceiling((float) a.Y / b.Y));
		}

		/// <summary>Divides 'a' by all parameters while ceiling the result.</summary>
		public static Point2I CeilingDiv(Point2I a, int b) {
			return new Point2I(	(int) Math.Ceiling((float) a.X / b),
								(int) Math.Ceiling((float) a.Y / b));
		}

		/// <summary>Divides 'a' by all parameters while ceiling the result.</summary>
		public static Point2I CeilingDiv(Point2I a, Point2I b, Point2I c) {
			return new Point2I(	(int) Math.Ceiling((float) a.X / b.X / c.X),
								(int) Math.Ceiling((float) a.Y / b.Y / c.Y));
		}

		/// <summary>Divides 'a' by all parameters while ceiling the result.</summary>
		public static Point2I CeilingDiv(Point2I a, int b, Point2I c) {
			return new Point2I(	(int) Math.Ceiling((float) a.X / b / c.X),
								(int) Math.Ceiling((float) a.Y / b / c.Y));
		}

		/// <summary>Divides 'a' by all parameters while ceiling the result.</summary>
		public static Point2I CeilingDiv(Point2I a, Point2I b, int c) {
			return new Point2I(	(int) Math.Ceiling((float) a.X / b.X / c),
								(int) Math.Ceiling((float) a.Y / b.Y / c));
		}

		/// <summary>Divides 'a' by all parameters while ceiling the result.</summary>
		public static Point2I CeilingDiv(Point2I a, int b, int c) {
			return new Point2I(	(int) Math.Ceiling((float) a.X / b / c),
								(int) Math.Ceiling((float) a.Y / b / c));
		}
	}
}
