using System;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>A static class for advanced game-related mathematical functions and
	/// calculations.</summary>
	public static partial class GMath {
		
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
			return radians / Pi * 180.0d;
		}

		/// <summary>Converts the specified angle from degrees into radians.</summary>
		public static float ToRadians(float degrees) {
			return degrees / 180f * Pi;
		}

		/// <summary>Converts the specified angle from degrees into radians.</summary>
		public static double ToRadians(double degrees) {
			return degrees / 180.0 * Pi;
		}

		// Wrap Angle -----------------------------------------------------------------
		
		/// <summary>Returns the wrapped angle as (0, 360].</summary>
		public static float WrapAngle(float angle) {
			return Wrap(angle, FullAngle);
		}

		/// <summary>Returns the wrapped angle as (0, 360].</summary>
		public static double WrapAngle(double angle) {
			return Wrap(angle, FullAngle);
		}

		/// <summary>Returns the wrapped angle as [0, 360).</summary>
		public static float WrapAngle2(float angle) {
			float value = Wrap(angle, FullAngle);
			if (value == 0.0f)
				return FullAngle;
			return value;
		}

		/// <summary>Returns the wrapped angle as [0, 360).</summary>
		public static double WrapAngle2(double angle) {
			double value = Wrap(angle, FullAngle);
			if (value == 0.0d)
				return FullAngle;
			return value;
		}

		/// <summary>Returns the wrapped angle as [-180, 180).</summary>
		public static float WrapAngle3(float angle) {
			float value = Wrap(angle, FullAngle);
			if (value > HalfAngle)
				return value - FullAngle;
			return value;
		}

		/// <summary>Returns the wrapped angle as [-180, 180).</summary>
		public static double WrapAngle3(double angle) {
			double value = Wrap(angle, FullAngle);
			if (value > HalfAngle)
				return value - FullAngle;
			return value;
		}

		// Delta Angle ----------------------------------------------------------------

		/// <summary>Return the modular distance from one angle to another using the
		/// given winding order.</summary>
		public static float DeltaAngle(float source, float destination,
			WindingOrder windingOrder = WindingOrder.CounterClockwise)
		{
			source = WrapAngle(source);
			destination = WrapAngle(destination);
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

		/// <summary>Return the modular distance from one angle to another using the
		/// given winding order.</summary>
		public static double DeltaAngle(double source, double destination,
			WindingOrder windingOrder = WindingOrder.CounterClockwise)
		{
			source = WrapAngle(source);
			destination = WrapAngle(destination);
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

		// Delta Angle Sign -----------------------------------------------------------

		/// <summary>Return the sign that will get the source angle to the
		/// destination angle the quickest.</summary>
		public static float DeltaAngleSign(float source, float destination) {
			return Math.Sign(WrapAngle3(destination - source));
		}

		/// <summary>Return the sign that will get the source angle to the
		/// destination angle the quickest.</summary>
		public static double DeltaAngleSign(double source, double destination) {
			return Math.Sign(WrapAngle3(destination - source));
		}

		/// <summary>Return the sign that will get the source angle to the
		/// destination angle the quickest. Returns an integer.</summary>
		public static int DeltaAngleSignI(float source, float destination) {
			return Math.Sign(WrapAngle3(destination - source));
		}

		/// <summary>Return the sign that will get the source angle to the
		/// destination angle the quickest. Returns an integer.</summary>
		public static int DeltaAngleSignI(double source, double destination) {
			return Math.Sign(WrapAngle3(destination - source));
		}

		// Delta Angle Sign2 ----------------------------------------------------------

		/// <summary>Return the sign that will get the source angle to the
		/// destination angle the quickest. Returns 1 if the sign is 0.</summary>
		public static float DeltaAngleSign2(float source, float destination) {
			float sign = Math.Sign(WrapAngle3(destination - source));
			return (sign == 0.0f ? 1.0f : sign);
		}

		/// <summary>Return the sign that will get the source angle to the
		/// destination angle the quickest. Returns 1 if the sign is 0.</summary>
		public static double DeltaAngleSign2(double source, double destination) {
			double sign = Math.Sign(WrapAngle3(destination - source));
			return (sign == 0.0d ? 1.0d : sign);
		}

		/// <summary>Return the sign that will get the source angle to the
		/// destination angle the quickest. Returns 1 if the sign is 0 and
		/// returns an integer.</summary>
		public static int DeltaAngleSign2I(float source, float destination) {
			int sign = Math.Sign(WrapAngle3(destination - source));
			return (sign == 0 ? 1 : sign);
		}

		/// <summary>Return the sign that will get the source angle to the
		/// destination angle the quickest. Returns 1 if the sign is 0 and
		/// returns an integer.</summary>
		public static int DeltaAngleSign2I(double source, double destination) {
			int sign = Math.Sign(WrapAngle3(destination - source));
			return (sign == 0 ? 1 : sign);
		}

		// Delta Angle Window Order ---------------------------------------------------

		/// <summary>Return the winding order that will get the source angle to the
		/// destination angle the quickest.</summary>
		public static WindingOrder DeltaAngleOrder(float source, float destination) {
			int sign = Math.Sign(WrapAngle3(destination - source));
			return (sign >= 0 ?	WindingOrder.CounterClockwise :
								WindingOrder.Clockwise);
		}

		/// <summary>Return the winding order that will get the source angle to the
		/// destination angle the quickest.</summary>
		public static WindingOrder DeltaAngleOrder(double source, double destination) {
			int sign = Math.Sign(WrapAngle3(destination - source));
			return (sign >= 0 ?	WindingOrder.CounterClockwise :
								WindingOrder.Clockwise);
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
		
		/// <summary>Returns the corrected angle for use with XNA drawing functions.</summary>
		public static float CorrectAngle(float angle) {
			float value = -angle % FullAngle;
			return (value < 0.0f ? value + FullAngle : value);
		}
	}
}
