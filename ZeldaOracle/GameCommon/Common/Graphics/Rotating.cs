using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>Rotations for an image in 90-degree increments.</summary>
	public enum Rotation {
		/// <summary>No rotation.</summary>
		None = 0,
		/// <summary>Rotate clockwise by 1 rotation.</summary>
		Clockwise90 = 1,
		/// <summary>Rotate clockwise by 2 rotations.</summary>
		Clockwise180 = 2,
		/// <summary>Rotate clockwise by 3 rotations.</summary>
		Clockwise270 = 3,
		/// <summary>Rotate counter-clockwise by 1 rotation.</summary
		Counter90 = 3,
		/// <summary>Rotate counter-clockwise by 2 rotations.</summary
		Counter180 = 2,
		/// <summary>Rotate counter-clockwise by 3 rotations.</summary
		Counter270 = 1,
	}

	/// <summary>Extensions for the Rotation enumeration.</summary>
	public static class Rotating {

		/// <summary>The array used to convert to radians.</summary>
		private static readonly float[] RotationRadians = new float[4] {
			0f,
			GMath.QuarterAngle,
			GMath.HalfAngle,
			GMath.ThreeFourthsAngle
		};

		/// <summary>Adds the rotations together by wrapping them.</summary>
		public static Rotation Add(Rotation rot1, Rotation rot2) {
			return (Rotation) (((int) rot1 + (int) rot2) % 4);
		}

		/// <summary>Converts the rotation to radians for use with Graphics2D.</summary>
		public static float ToRadians(this Rotation rot) {
			return RotationRadians[(int) rot];
		}
	}
}
