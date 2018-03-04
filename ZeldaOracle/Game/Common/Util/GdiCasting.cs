using GdiColor		= System.Drawing.Color;
using GdiPoint		= System.Drawing.Point;
using GdiPointF		= System.Drawing.PointF;
using GdiSize		= System.Drawing.Size;
using GdiSizeF		= System.Drawing.SizeF;
using GdiRectangle	= System.Drawing.Rectangle;
using GdiRectangleF	= System.Drawing.RectangleF;

using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Util {
	/// <summary>Static Gdi extensions for casting geometry and colors.</summary>
	public static class GdiCasting {

		//-----------------------------------------------------------------------------
		// Color
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Color to an Gdi Color.</summary>
		public static GdiColor ToGdiColor(this Color color) {
			return GdiColor.FromArgb(color.R, color.G, color.B, color.A);
		}

		/// <summary>Casts the Gdi Color to a Zelda Color.</summary>
		public static Color ToColor(this GdiColor color) {
			return new Color(color.R, color.G, color.B, color.A);
		}


		//-----------------------------------------------------------------------------
		// Point2I
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Point2I to an Gdi Point.</summary>
		public static GdiPoint ToGdiPoint(this Point2I point) {
			return new GdiPoint(point.X, point.Y);
		}

		/// <summary>Casts the Gdi Point to a Zelda Point2I.</summary>
		public static Point2I ToPoint2I(this GdiPoint point) {
			return new Point2I(point.X, point.Y);
		}

		/// <summary>Casts the Zelda Point2I to an Gdi PointF.</summary>
		public static GdiPointF ToGdiPointF(this Point2I point) {
			return new GdiPointF(point.X, point.Y);
		}

		/// <summary>Casts the Gdi PointF to a Zelda Point2I.</summary>
		public static Point2I ToPoint2I(this GdiPointF vector) {
			return new Point2I((int) vector.X, (int) vector.Y);
		}

		/// <summary>Casts the Zelda Point2I to an Gdi Size.</summary>
		public static GdiSize ToGdiSize(this Point2I point) {
			return new GdiSize(point.X, point.Y);
		}

		/// <summary>Casts the Gdi Size to a Zelda Point2I.</summary>
		public static Point2I ToPoint2I(this GdiSize size) {
			return new Point2I(size.Width, size.Height);
		}

		/// <summary>Casts the Zelda Point2I to an Gdi SizeF.</summary>
		public static GdiSizeF ToGdiSizeF(this Point2I point) {
			return new GdiSizeF(point.X, point.Y);
		}

		/// <summary>Casts the Gdi SizeF to a Zelda Point2I.</summary>
		public static Point2I ToPoint2I(this GdiSizeF size) {
			return new Point2I((int) size.Width, (int) size.Height);
		}


		//-----------------------------------------------------------------------------
		// Vector2F
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Vector2F to an Gdi Point.</summary>
		public static GdiPoint ToGdiPoint(this Vector2F vector) {
			return new GdiPoint((int) vector.X, (int) vector.Y);
		}

		/// <summary>Casts the Gdi Point to a Zelda Vector2F.</summary>
		public static Vector2F ToVector2F(this GdiPoint point) {
			return new Vector2F(point.X, point.Y);
		}

		/// <summary>Casts the Zelda Vector2F to an Gdi PointF.</summary>
		public static GdiPointF ToGdiPointF(this Vector2F vector) {
			return new GdiPointF(vector.X, vector.Y);
		}

		/// <summary>Casts the Gdi PointF to a Zelda Vector2F.</summary>
		public static Vector2F ToVector2F(this GdiPointF vector) {
			return new Vector2F(vector.X, vector.Y);
		}

		/// <summary>Casts the Zelda Vector2F to an Gdi Size.</summary>
		public static GdiSize ToGdiSize(this Vector2F vector) {
			return new GdiSize((int) vector.X, (int) vector.Y);
		}

		/// <summary>Casts the Gdi Size to a Zelda Vector2F.</summary>
		public static Vector2F ToVector2F(this GdiSize size) {
			return new Vector2F(size.Width, size.Height);
		}

		/// <summary>Casts the Zelda Vector2F to an Gdi SizeF.</summary>
		public static GdiSizeF ToGdiSizeF(this Vector2F vector) {
			return new GdiSizeF(vector.X, vector.Y);
		}

		/// <summary>Casts the Gdi SizeF to a Zelda Vector2F.</summary>
		public static Vector2F ToVector2F(this GdiSizeF size) {
			return new Vector2F(size.Width, size.Height);
		}


		//-----------------------------------------------------------------------------
		// Rectangle2I
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Rectangle2I to an Gdi Rectangle.</summary>
		public static GdiRectangle ToGdiRectangle(this Rectangle2I rect) {
			return new GdiRectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Gdi Rectangle to a Zelda Rectangle2I.</summary>
		public static Rectangle2I ToRectangle2I(this GdiRectangle rect) {
			return new Rectangle2I(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Zelda Rectangle2I to an Gdi Rectangle.</summary>
		public static GdiRectangleF ToGdiRectangleF(this Rectangle2I rect) {
			return new GdiRectangleF(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Gdi RectangleF to a Zelda Rectangle2I.</summary>
		public static Rectangle2I ToRectangle2I(this GdiRectangleF rect) {
			return new Rectangle2I((int) rect.X, (int) rect.Y,
									(int) rect.Width, (int) rect.Height);
		}


		//-----------------------------------------------------------------------------
		// Rectangle2F
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Rectangle2F to an Gdi Rectangle.</summary>
		public static GdiRectangle ToGdiRectangle(this Rectangle2F rect) {
			return new GdiRectangle((int) rect.X, (int) rect.Y,
									(int) rect.Width, (int) rect.Height);
		}

		/// <summary>Casts the Gdi Rectangle to a Zelda Rectangle2F.</summary>
		public static Rectangle2F ToRectangle2F(this GdiRectangle rect) {
			return new Rectangle2F(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Zelda Rectangle2F to an Gdi Rectangle.</summary>
		public static GdiRectangleF ToGdiRectangleF(this Rectangle2F rect) {
			return new GdiRectangleF(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Gdi RectangleF to a Zelda Rectangle2F.</summary>
		public static Rectangle2F ToRectangle2F(this GdiRectangleF rect) {
			return new Rectangle2F(rect.X, rect.Y, rect.Width, rect.Height);
		}
	}
}
