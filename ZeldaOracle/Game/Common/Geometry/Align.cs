using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>An enumeration for different alignments.</summary>
	[Flags]
	public enum Align {

		Center		= 0x0,
		Left		= 0x1,
		Right		= 0x2,
		Top			= 0x4,
		Bottom		= 0x8,
		TopLeft		= Top | Left,
		TopRight	= Top | Right,
		BottomLeft	= Bottom | Left,
		BottomRight	= Bottom | Right,

		Int = 0x10,

	}

	/// <summary>A static class for use with the Align enumeration.</summary>
	public static class Alignment {

		/// <summary>Aligns a point.</summary>
		/// <param name="position">The initial position.</param>
		/// <param name="area">The size of the area to align to.</param>
		/// <param name="size">The size of the object being aligned.</param>
		/// <param name="alignment">The alignment settings.</param>
		public static Point2I AlignPoint(Point2I position, Point2I area, Point2I size, Align alignment) {
			if (alignment.HasFlag(Align.Left)) { }
			else if (alignment.HasFlag(Align.Right))
				position.X += area.X - size.X;
			else
				position.X += (area.X - size.X) / 2;

			if (alignment.HasFlag(Align.Top)) { }
			else if (alignment.HasFlag(Align.Bottom))
				position.Y += area.Y - size.Y;
			else
				position.Y += (area.Y - size.Y) / 2;

			return position;
		}

		/// <summary>Aligns a vector.</summary>
		/// <param name="position">The initial position.</param>
		/// <param name="area">The size of the area to align to.</param>
		/// <param name="size">The size of the object being aligned.</param>
		/// <param name="alignment">The alignment settings.</param>
		public static Vector2F AlignVector(Vector2F position, Vector2F area, Vector2F size, Align alignment) {
			if (alignment.HasFlag(Align.Left)) { }
			else if (alignment.HasFlag(Align.Right))
				position.X += area.X - size.X;
			else
				position.X += (area.X - size.X) / 2;

			if (alignment.HasFlag(Align.Top)) { }
			else if (alignment.HasFlag(Align.Bottom))
				position.Y += area.Y - size.Y;
			else
				position.Y += (area.Y - size.Y) / 2;

			if (alignment.HasFlag(Align.Int))
				position = GMath.Floor(position);

			return position;
		}
	}
} // end namespace
