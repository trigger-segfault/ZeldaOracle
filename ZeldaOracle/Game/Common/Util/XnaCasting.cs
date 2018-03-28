using XnaColor		= Microsoft.Xna.Framework.Color;
using XnaPoint		= Microsoft.Xna.Framework.Point;
using XnaVector2	= Microsoft.Xna.Framework.Vector2;
using XnaVector4	= Microsoft.Xna.Framework.Vector4;
using XnaRectangle	= Microsoft.Xna.Framework.Rectangle;

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

		/// <summary>Casts the Zelda Color to an Xna Color.</summary>
		public static XnaVector4 ToXnaVector4(this Color color) {
			return new XnaVector4(color.RF, color.GF, color.BF, color.AF);
		}

		/// <summary>Casts the Xna Color to a Zelda Color.</summary>
		public static Color ToColor(this XnaColor color) {
			return new Color(color.R, color.G, color.B, color.A);
		}


		//-----------------------------------------------------------------------------
		// Point2I
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Point2I to an Xna Point.</summary>
		public static XnaPoint ToXnaPoint(this Point2I point) {
			return new XnaPoint(point.X, point.Y);
		}

		/// <summary>Casts the Xna Point to a Zelda Point2I.</summary>
		public static Point2I ToPoint2I(this XnaPoint point) {
			return new Point2I(point.X, point.Y);
		}

		/// <summary>Casts the Zelda Point2I to an Xna Vector2.</summary>
		public static XnaVector2 ToXnaVector2(this Point2I point) {
			return new XnaVector2(point.X, point.Y);
		}

		/// <summary>Casts the Xna Vector2 to a Zelda Point2I.</summary>
		public static Point2I ToPoint2I(this XnaVector2 vector) {
			return new Point2I((int) vector.X, (int) vector.Y);
		}


		//-----------------------------------------------------------------------------
		// Vector2F
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Vector2F to an Xna Vector2.</summary>
		public static XnaVector2 ToXnaVector2(this Vector2F vector) {
			return new XnaVector2(vector.X, vector.Y);
		}

		/// <summary>Casts the Xna Vector2 to a Zelda Vector2F.</summary>
		public static Vector2F ToVector2F(this XnaVector2 vector) {
			return new Vector2F(vector.X, vector.Y);
		}

		/// <summary>Casts the Zelda Vector2F to an Xna Point.</summary>
		public static XnaPoint ToXnaPoint(this Vector2F vector) {
			return new XnaPoint((int) vector.X, (int) vector.Y);
		}

		/// <summary>Casts the Xna Point to a Zelda Vector2F.</summary>
		public static Vector2F ToVector2F(this XnaPoint point) {
			return new Vector2F(point.X, point.Y);
		}


		//-----------------------------------------------------------------------------
		// Rectangle2I
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Rectangle2I to an Xna Rectangle.</summary>
		public static XnaRectangle ToXnaRectangle(this Rectangle2I rect) {
			return new XnaRectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>Casts the Xna Rectangle to a Zelda Rectangle2I.</summary>
		public static Rectangle2I ToRectangle2I(this XnaRectangle rect) {
			return new Rectangle2I(rect.X, rect.Y, rect.Width, rect.Height);
		}


		//-----------------------------------------------------------------------------
		// Rectangle2F
		//-----------------------------------------------------------------------------

		/// <summary>Casts the Zelda Rectangle2F to an Xna Rectangle.</summary>
		public static XnaRectangle ToXnaRectangle(this Rectangle2F rect) {
			return new XnaRectangle((int) rect.X, (int) rect.Y,
									(int) rect.Width, (int) rect.Height);
		}

		/// <summary>Casts the Xna Rectangle to a Zelda Rectangle2F.</summary>
		public static Rectangle2F ToRectangle2F(this XnaRectangle rect) {
			return new Rectangle2F(rect.X, rect.Y, rect.Width, rect.Height);
		}
	}
}
