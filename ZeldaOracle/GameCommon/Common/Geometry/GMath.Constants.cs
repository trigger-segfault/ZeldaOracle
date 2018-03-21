using System;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>A static class for advanced game-related mathematical functions and
	/// calculations.</summary>
	public static partial class GMath {
		
		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>Represents the ratio of the circumference of a circle to its
		/// diameter, specified by the constant, pi.</summary>
		public const float Pi = 3.14159265358979323846f;
		/// <summary>Represents the natural logarithmic base, specified by the
		/// constant, e.</summary>
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
		
	}
}
