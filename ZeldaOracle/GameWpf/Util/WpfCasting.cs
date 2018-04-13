using WpfColor = System.Windows.Media.Color;
using WpfPoint = System.Windows.Point;
using WpfSize = System.Windows.Size;
using WpfRect = System.Windows.Rect;
using WpfInt32Rect = System.Windows.Int32Rect;

using GdiColor = System.Drawing.Color;
using GdiPoint = System.Drawing.Point;
using GdiPointF = System.Drawing.PointF;
using GdiSize = System.Drawing.Size;
using GdiSizeF = System.Drawing.SizeF;
using GdiRectangle = System.Drawing.Rectangle;
using GdiRectangleF = System.Drawing.RectangleF;

using XnaColor = Microsoft.Xna.Framework.Color;
using XnaPoint = Microsoft.Xna.Framework.Point;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;
using XnaVector4 = Microsoft.Xna.Framework.Vector4;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Util {
	/// <summary>Static Wpf extensions for casting geometry and colors.</summary>
	public static partial class WpfCasting {

		//-----------------------------------------------------------------------------
		// Color
		//-----------------------------------------------------------------------------
		
		/// <summary>Casts the Zelda Color to a Wpf Color.</summary>
		public static WpfColor ToWpfColor(this Color color) {
			return WpfColor.FromArgb(color.A, color.R, color.G, color.B);
		}

		/// <summary>Casts the Gdi Color to a Wpf Color.</summary>
		public static WpfColor ToWpfColor(this GdiColor color) {
			return WpfColor.FromArgb(color.A, color.R, color.G, color.B);
		}

		/// <summary>Casts the Xna Color to a Wpf Color.</summary>
		public static WpfColor ToWpfColor(this XnaColor color) {
			return WpfColor.FromArgb(color.A, color.R, color.G, color.B);
		}

		/// <summary>Casts the Xna Vector4 to a Wpf Color.</summary>
		public static WpfColor ToWpfColor(this XnaVector4 vector) {
			return WpfColor.FromScRgb(vector.W, vector.X, vector.Y, vector.Z);
		}

		//-----------------------------------------------------------------------------

		/// <summary>Casts the Wpf Color to a Zelda Color.</summary>
		public static Color ToColor(this WpfColor color) {
			return new Color(color.R, color.G, color.B, color.A);
		}

		/// <summary>Casts the Wpf Color to a Gdi Color.</summary>
		public static GdiColor ToGdiColor(this WpfColor color) {
			return GdiColor.FromArgb(color.A, color.R, color.G, color.B);
		}

		/// <summary>Casts the Wpf Color to an Xna Color.</summary>
		public static XnaColor ToXnaColor(this WpfColor color) {
			return new XnaColor(color.R, color.G, color.B, color.A);
		}

		/// <summary>Casts the Wpf Color to an Xna Vector4.</summary>
		public static XnaVector4 ToXnaVector4(this WpfColor color) {
			return new XnaVector4(	color.R / 255f, color.G / 255f,
									color.B / 255f, color.A / 255f);
		}


		//-----------------------------------------------------------------------------
		// Point
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Point2I to a Wpf Point.</summary>
		public static WpfPoint ToWpfPoint(this Point2I point) {
			return new WpfPoint(point.X, point.Y);
		}

		/// <summary>Casts the Zelda Vector2F to a Wpf Point.</summary>
		public static WpfPoint ToWpfPoint(this Vector2F vector) {
			return new WpfPoint(vector.X, vector.Y);
		}

		/// <summary>Casts the Gdi Point to a Wpf Point.</summary>
		public static WpfPoint ToWpfPoint(this GdiPoint point) {
			return new WpfPoint(point.X, point.Y);
		}

		/// <summary>Casts the Gdi PointF to a Wpf Point.</summary>
		public static WpfPoint ToWpfPoint(this GdiPointF point) {
			return new WpfPoint(point.X, point.Y);
		}

		/// <summary>Casts the Gdi Size to a Wpf Point.</summary>
		public static WpfPoint ToWpfPoint(this GdiSize size) {
			return new WpfPoint(size.Width, size.Height);
		}

		/// <summary>Casts the Gdi SizeF to a Wpf Point.</summary>
		public static WpfPoint ToWpfPoint(this GdiSizeF size) {
			return new WpfPoint(size.Width, size.Height);
		}

		/// <summary>Casts the Xna Point to a Wpf Point.</summary>
		public static WpfPoint ToWpfPoint(this XnaPoint point) {
			return new WpfPoint(point.X, point.Y);
		}

		/// <summary>Casts the Xna Vector2 to a Wpf Point.</summary>
		public static WpfPoint ToWpfPoint(this XnaVector2 vector) {
			return new WpfPoint(vector.X, vector.Y);
		}

		//-----------------------------------------------------------------------------

		/// <summary>Casts the Wpf Point to a Zelda Point2I.</summary>
		public static Point2I ToPoint2I(this WpfPoint point) {
			return new Point2I((int) point.X, (int) point.Y);
		}

		/// <summary>Casts the Wpf Point to a Zelda Vector2F.</summary>
		public static Vector2F ToVector2F(this WpfPoint point) {
			return new Vector2F((float) point.X, (float) point.Y);
		}

		/// <summary>Casts the Wpf Point to a Gdi Point.</summary>
		public static GdiPoint ToGdiPoint(this WpfPoint point) {
			return new GdiPoint((int) point.X, (int) point.Y);
		}

		/// <summary>Casts the Wpf Point to a Gdi PointF.</summary>
		public static GdiPointF ToGdiPointF(this WpfPoint point) {
			return new GdiPointF((float) point.X, (float) point.Y);
		}

		/// <summary>Casts the Wpf Point to a Gdi Size.</summary>
		public static GdiSize ToGdiSize(this WpfPoint point) {
			return new GdiSize((int) point.X, (int) point.Y);
		}

		/// <summary>Casts the Wpf Point to a Gdi SizeF.</summary>
		public static GdiSizeF ToGdiSizeF(this WpfPoint point) {
			return new GdiSizeF((float) point.X, (float) point.Y);
		}

		/// <summary>Casts the Wpf Point to an Xna Point.</summary>
		public static XnaPoint ToXnaPoint(this WpfPoint point) {
			return new XnaPoint((int) point.X, (int) point.Y);
		}

		/// <summary>Casts the Wpf Point to an Xna Point.</summary>
		public static XnaVector2 ToXnaVector2(this WpfPoint point) {
			return new XnaVector2((float) point.X, (float) point.Y);
		}


		//-----------------------------------------------------------------------------
		// Size
		//-----------------------------------------------------------------------------
		
		/// <summary>Casts the Zelda Point2I to a Wpf Size.</summary>
		public static WpfSize ToWpfSize(this Point2I point) {
			return new WpfSize(point.X, point.Y);
		}

		/// <summary>Casts the Zelda Vector2F to a Wpf Size.</summary>
		public static WpfSize ToWpfSize(this Vector2F vector) {
			return new WpfSize(vector.X, vector.Y);
		}

		/// <summary>Casts the Gdi Point to a Wpf Size.</summary>
		public static WpfSize ToWpfSize(this GdiPoint point) {
			return new WpfSize(point.X, point.Y);
		}

		/// <summary>Casts the Gdi PointF to a Wpf Size.</summary>
		public static WpfSize ToWpfSize(this GdiPointF point) {
			return new WpfSize(point.X, point.Y);
		}

		/// <summary>Casts the Gdi Size to a Wpf Size.</summary>
		public static WpfSize ToWpfSize(this GdiSize size) {
			return new WpfSize(size.Width, size.Height);
		}

		/// <summary>Casts the Gdi SizeF to a Wpf Size.</summary>
		public static WpfSize ToWpfSize(this GdiSizeF size) {
			return new WpfSize(size.Width, size.Height);
		}

		/// <summary>Casts the Xna Point to a Wpf Size.</summary>
		public static WpfSize ToWpfSize(this XnaPoint point) {
			return new WpfSize(point.X, point.Y);
		}

		/// <summary>Casts the Xna Vector2 to a Wpf Size.</summary>
		public static WpfSize ToWpfSize(this XnaVector2 vector) {
			return new WpfSize(vector.X, vector.Y);
		}

		//-----------------------------------------------------------------------------

		/// <summary>Casts the Wpf Size to a Zelda Point2I.</summary>
		public static Point2I ToPoint2I(this WpfSize size) {
			return new Point2I((int) size.Width, (int) size.Height);
		}

		/// <summary>Casts the Wpf Size to a Zelda Vector2F.</summary>
		public static Vector2F ToVector2F(this WpfSize size) {
			return new Vector2F((float) size.Width, (float) size.Height);
		}

		/// <summary>Casts the Wpf Size to a Gdi Point.</summary>
		public static GdiPoint ToGdiPoint(this WpfSize size) {
			return new GdiPoint((int) size.Width, (int) size.Height);
		}

		/// <summary>Casts the Wpf Size to a Gdi PointF.</summary>
		public static GdiPointF ToGdiPointF(this WpfSize size) {
			return new GdiPointF((float) size.Width, (float) size.Height);
		}

		/// <summary>Casts the Wpf Size to a Gdi Size.</summary>
		public static GdiSize ToGdiSize(this WpfSize size) {
			return new GdiSize((int) size.Width, (int) size.Height);
		}

		/// <summary>Casts the Wpf Size to a Gdi SizeF.</summary>
		public static GdiSizeF ToGdiSizeF(this WpfSize size) {
			return new GdiSizeF((float) size.Width, (float) size.Height);
		}

		/// <summary>Casts the Wpf Size to an Xna Point.</summary>
		public static XnaPoint ToXnaPoint(this WpfSize size) {
			return new XnaPoint((int) size.Width, (int) size.Height);
		}

		/// <summary>Casts the Wpf Size to an Xna Vector2.</summary>
		public static XnaVector2 ToXnaVector2(this WpfSize size) {
			return new XnaVector2((float) size.Width, (float) size.Height);
		}


		//-----------------------------------------------------------------------------
		// Rect
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Rectangle2I to a Wpf Rect.</summary>
		public static WpfRect ToWpfRect(this Rectangle2I rect) {
			return new WpfRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Zelda Rectangle2F to a Wpf Rect.</summary>
		public static WpfRect ToWpfRect(this Rectangle2F rect) {
			return new WpfRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Gdi Rectangle to a Wpf Rect.</summary>
		public static WpfRect ToWpfRect(this GdiRectangle rect) {
			return new WpfRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Gdi RectangleF to a Wpf Rect.</summary>
		public static WpfRect ToWpfRect(this GdiRectangleF rect) {
			return new WpfRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Xna Rectangle to a Wpf Rect.</summary>
		public static WpfRect ToWpfRect(this XnaRectangle rect) {
			return new WpfRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		//-----------------------------------------------------------------------------

		/// <summary>Casts the Wpf Rect to a Zelda Rectangle2I.</summary>
		public static Rectangle2I ToRectangle2I(this WpfRect rect) {
			return new Rectangle2I(	(int) rect.X,		(int) rect.Y,
									(int) rect.Width,	(int) rect.Height);
		}
		
		/// <summary>Casts the Wpf Rect to a Zelda Rectangle2F.</summary>
		public static Rectangle2F ToRectangle2F(this WpfRect rect) {
			return new Rectangle2F(	(float) rect.X,		(float) rect.Y,
									(float) rect.Width,	(float) rect.Height);
		}
		
		/// <summary>Casts the Wpf Rect to a Gdi Rectangle.</summary>
		public static GdiRectangle ToGdiRectangle(this WpfRect rect) {
			return new GdiRectangle((int) rect.X,		(int) rect.Y,
									(int) rect.Width,	(int) rect.Height);
		}

		/// <summary>Casts the Wpf Rect to a Gdi RectangleF.</summary>
		public static GdiRectangleF ToGdiRectangleF(this WpfRect rect) {
			return new GdiRectangleF((float) rect.X,		(float) rect.Y,
									 (float) rect.Width,	(float) rect.Height);
		}

		/// <summary>Casts the Wpf Rect to an Xna Rectangle.</summary>
		public static XnaRectangle ToXnaRectangle(this WpfRect rect) {
			return new XnaRectangle((int) rect.X, (int) rect.Y,
									(int) rect.Width, (int) rect.Height);
		}


		//-----------------------------------------------------------------------------
		// Int32Rect
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Rectangle2I to a Wpf Int32Rect.</summary>
		public static WpfInt32Rect ToWpfInt32Rect(this Rectangle2I rect) {
			return new WpfInt32Rect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Zelda Rectangle2F to a Wpf Int32Rect.</summary>
		public static WpfInt32Rect ToWpfInt32Rect(this Rectangle2F rect) {
			return new WpfInt32Rect((int) rect.X,		(int) rect.Y,
									(int) rect.Width,	(int) rect.Height);
		}

		/// <summary>Casts the Gdi Rectangle to a Wpf Int32Rect.</summary>
		public static WpfInt32Rect ToWpfInt32Rect(this GdiRectangle rect) {
			return new WpfInt32Rect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Gdi RectangleF to a Wpf Int32Rect.</summary>
		public static WpfInt32Rect ToWpfInt32Rect(this GdiRectangleF rect) {
			return new WpfInt32Rect((int) rect.X,		(int) rect.Y,
									(int) rect.Width,	(int) rect.Height);
		}

		/// <summary>Casts the Xna Rectangle to a Wpf Int32Rect.</summary>
		public static WpfInt32Rect ToWpfInt32Rect(this XnaRectangle rect) {
			return new WpfInt32Rect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		//-----------------------------------------------------------------------------

		/// <summary>Casts the Wpf Int32Rect to a Zelda Rectangle2I.</summary>
		public static Rectangle2I ToRectangle2I(this WpfInt32Rect rect) {
			return new Rectangle2I(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Wpf Int32Rect to a Zelda Rectangle2F.</summary>
		public static Rectangle2F ToRectangle2F(this WpfInt32Rect rect) {
			return new Rectangle2F(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Wpf Int32Rect to a Gdi Rectangle.</summary>
		public static GdiRectangle ToGdiRectangle(this WpfInt32Rect rect) {
			return new GdiRectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Wpf Int32Rect to a Gdi RectangleF.</summary>
		public static GdiRectangleF ToGdiRectangleF(this WpfInt32Rect rect) {
			return new GdiRectangleF(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Wpf Int32Rect to an Xna Rectangle.</summary>
		public static XnaRectangle ToXnaRectangle(this WpfInt32Rect rect) {
			return new XnaRectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}
	}
}
