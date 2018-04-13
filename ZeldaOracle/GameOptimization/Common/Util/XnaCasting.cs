using XnaColor		= Microsoft.Xna.Framework.Color;
using XnaPoint		= Microsoft.Xna.Framework.Point;
using XnaVector2	= Microsoft.Xna.Framework.Vector2;
using XnaVector4	= Microsoft.Xna.Framework.Vector4;
using XnaRectangle	= Microsoft.Xna.Framework.Rectangle;

using GdiColor = System.Drawing.Color;
using GdiPoint = System.Drawing.Point;
using GdiPointF = System.Drawing.PointF;
using GdiSize = System.Drawing.Size;
using GdiSizeF = System.Drawing.SizeF;
using GdiRectangle = System.Drawing.Rectangle;
using GdiRectangleF = System.Drawing.RectangleF;

using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Util {
	/// <summary>Static Xna extensions for casting geometry and colors.</summary>
	public static class XnaCasting {

		//-----------------------------------------------------------------------------
		// Color
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Color to an Xna Color.</summary>
		public static XnaColor ToXnaColor(this Color color) {
			return new XnaColor(color.R, color.G, color.B, color.A);
		}

		/// <summary>Casts the Gdi Color to an Xna Color.</summary>
		public static XnaColor ToXnaColor(this GdiColor color) {
			return new XnaColor(color.R, color.G, color.B, color.A);
		}

		/// <summary>Casts the Xna Color to a Zelda Color.</summary>
		public static Color ToColor(this XnaColor color) {
			return new Color(color.R, color.G, color.B, color.A);
		}

		/// <summary>Casts the Xna Color to a Gdi Color.</summary>
		public static GdiColor ToGdiColor(this XnaColor color) {
			return GdiColor.FromArgb(color.A, color.R, color.G, color.B);
		}


		//-----------------------------------------------------------------------------
		// Vector4
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Color to an Xna Vector4.</summary>
		public static XnaVector4 ToXnaVector4(this Color color) {
			return new XnaVector4(color.RF, color.GF, color.BF, color.AF);
		}

		/// <summary>Casts the Gdi Color to an Xna Vector4.</summary>
		public static XnaVector4 ToXnaVector4(this GdiColor color) {
			return new XnaVector4(	color.R / 255f, color.G / 255f,
									color.B / 255f, color.A / 255f);
		}

		/// <summary>Casts the Xna Vector4 to a Zelda Color.</summary>
		public static Color ToColor(this XnaVector4 vector) {
			return new Color(vector.X, vector.Y, vector.Z, vector.W);
		}

		/// <summary>Casts the Xna Vector4 to a Zelda Color.</summary>
		public static GdiColor ToGdiColor(this XnaVector4 vector) {
			return GdiColor.FromArgb((int) (vector.W * 255), (int) (vector.X * 255),
									 (int) (vector.Y * 255), (int) (vector.Z * 255));
		}


		//-----------------------------------------------------------------------------
		// Point
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Point2I to an Xna Point.</summary>
		public static XnaPoint ToXnaPoint(this Point2I point) {
			return new XnaPoint(point.X, point.Y);
		}

		/// <summary>Casts the Zelda Vector2F to an Xna Point.</summary>
		public static XnaPoint ToXnaPoint(this Vector2F vector) {
			return new XnaPoint((int) vector.X, (int) vector.Y);
		}

		/// <summary>Casts the Gdi Point to an Xna Point.</summary>
		public static XnaPoint ToXnaPoint(this GdiPoint point) {
			return new XnaPoint(point.X, point.Y);
		}

		/// <summary>Casts the Gdi PointF to an Xna Point.</summary>
		public static XnaPoint ToXnaPoint(this GdiPointF point) {
			return new XnaPoint((int) point.X, (int) point.Y);
		}

		/// <summary>Casts the Gdi Size to an Xna Point.</summary>
		public static XnaPoint ToXnaPoint(this GdiSize size) {
			return new XnaPoint(size.Width, size.Height);
		}

		/// <summary>Casts the Gdi SizeF to an Xna Point.</summary>
		public static XnaPoint ToXnaPoint(this GdiSizeF size) {
			return new XnaPoint((int) size.Width, (int) size.Height);
		}

		/// <summary>Casts the Xna Point to a Zelda Point2I.</summary>
		public static Point2I ToPoint2I(this XnaPoint point) {
			return new Point2I(point.X, point.Y);
		}

		/// <summary>Casts the Xna Point to a Zelda Vector2F.</summary>
		public static Vector2F ToVector2F(this XnaPoint point) {
			return new Vector2F(point.X, point.Y);
		}

		/// <summary>Casts the Xna Point to a Gdi Point.</summary>
		public static GdiPoint ToGdiPoint(this XnaPoint point) {
			return new GdiPoint(point.X, point.Y);
		}

		/// <summary>Casts the Xna Point to a Gdi PointF.</summary>
		public static GdiPointF ToGdiPointF(this XnaPoint point) {
			return new GdiPointF(point.X, point.Y);
		}

		/// <summary>Casts the Xna Point to a Gdi Size.</summary>
		public static GdiSize ToGdiSize(this XnaPoint point) {
			return new GdiSize(point.X, point.Y);
		}

		/// <summary>Casts the Xna Point to a Gdi SizeF.</summary>
		public static GdiSizeF ToGdiSizeF(this XnaPoint point) {
			return new GdiSizeF(point.X, point.Y);
		}


		//-----------------------------------------------------------------------------
		// Vector2
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Point2I to an Xna Vector2.</summary>
		public static XnaVector2 ToXnaVector2(this Point2I point) {
			return new XnaVector2(point.X, point.Y);
		}

		/// <summary>Casts the Zelda Vector2F to an Xna Vector2.</summary>
		public static XnaVector2 ToXnaVector2(this Vector2F vector) {
			return new XnaVector2(vector.X, vector.Y);
		}

		/// <summary>Casts the Gdi Point to an Xna Vector2.</summary>
		public static XnaVector2 ToXnaVector2(this GdiPoint point) {
			return new XnaVector2(point.X, point.Y);
		}

		/// <summary>Casts the Gdi PointF to an Xna Vector2.</summary>
		public static XnaVector2 ToXnaVector2(this GdiPointF point) {
			return new XnaVector2(point.X, point.Y);
		}

		/// <summary>Casts the Gdi Size to an Xna Vector2.</summary>
		public static XnaVector2 ToXnaVector2(this GdiSize size) {
			return new XnaVector2(size.Width, size.Height);
		}

		/// <summary>Casts the Gdi SizeF to an Xna Vector2.</summary>
		public static XnaVector2 ToXnaVector2(this GdiSizeF size) {
			return new XnaVector2(size.Width, size.Height);
		}

		/// <summary>Casts the Xna Vector2 to a Zelda Point2I.</summary>
		public static Point2I ToPoint2I(this XnaVector2 vector) {
			return new Point2I((int) vector.X, (int) vector.Y);
		}

		/// <summary>Casts the Xna Vector2 to a Zelda Vector2F.</summary>
		public static Vector2F ToVector2F(this XnaVector2 vector) {
			return new Vector2F(vector.X, vector.Y);
		}

		/// <summary>Casts the Xna Vector2 to a Gdi Point.</summary>
		public static GdiPoint ToGdiPoint(this XnaVector2 vector) {
			return new GdiPoint((int) vector.X, (int) vector.Y);
		}

		/// <summary>Casts the Xna Vector2 to a Gdi PointF.</summary>
		public static GdiPointF ToGdiPointF(this XnaVector2 vector) {
			return new GdiPointF(vector.X, vector.Y);
		}

		/// <summary>Casts the Xna Vector2 to a Gdi Size.</summary>
		public static GdiSize ToGdiSize(this XnaVector2 vector) {
			return new GdiSize((int) vector.X, (int) vector.Y);
		}

		/// <summary>Casts the Xna Vector2 to a Gdi SizeF.</summary>
		public static GdiSizeF ToGdiSizeF(this XnaVector2 vector) {
			return new GdiSizeF(vector.X, vector.Y);
		}


		//-----------------------------------------------------------------------------
		// Rectangle
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Rectangle2I to an Xna Rectangle.</summary>
		public static XnaRectangle ToXnaRectangle(this Rectangle2I rect) {
			return new XnaRectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Zelda Rectangle2F to an Xna Rectangle.</summary>
		public static XnaRectangle ToXnaRectangle(this Rectangle2F rect) {
			return new XnaRectangle((int) rect.X,		(int) rect.Y,
									(int) rect.Width,	(int) rect.Height);
		}

		/// <summary>Casts the Gdi Rectangle to an Xna Rectangle.</summary>
		public static XnaRectangle ToXnaRectangle(this GdiRectangle rect) {
			return new XnaRectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Gdi RectangleF to an Xna Rectangle.</summary>
		public static XnaRectangle ToXnaRectangle(this GdiRectangleF rect) {
			return new XnaRectangle((int) rect.X,		(int) rect.Y,
									(int) rect.Width,	(int) rect.Height);
		}

		/// <summary>Casts the Xna Rectangle to a Zelda Rectangle2I.</summary>
		public static Rectangle2I ToRectangle2I(this XnaRectangle rect) {
			return new Rectangle2I(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Xna Rectangle to a Zelda Rectangle2F.</summary>
		public static Rectangle2F ToRectangle2F(this XnaRectangle rect) {
			return new Rectangle2F(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Xna Rectangle to a Gdi Rectangle.</summary>
		public static GdiRectangle ToGdiRectangle(this XnaRectangle rect) {
			return new GdiRectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Xna Rectangle to a Gdi RectangleF.</summary>
		public static GdiRectangleF ToGdiRectangleF(this XnaRectangle rect) {
			return new GdiRectangleF(rect.X, rect.Y, rect.Width, rect.Height);
		}
	}
}
