using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using GdiPoint		= System.Drawing.Point;
using GdiPointF		= System.Drawing.PointF;
using GdiSize		= System.Drawing.Size;
using GdiSizeF		= System.Drawing.SizeF;
using GdiRectangle	= System.Drawing.Rectangle;
using GdiRectangleF	= System.Drawing.RectangleF;

namespace ZeldaOracle.Common.Geometry {
	/**<summary>The 2D integer precision point with basic operations and functions.</summary>*/
	public struct Point2I {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/**<summary>Returns a point positioned at (0, 0).</summary>*/
		public static readonly Point2I Zero = new Point2I(0, 0);
		/**<summary>Returns a point positioned at (1, 1).</summary>*/
		public static readonly Point2I One = new Point2I(1, 1);


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/**<summary>The x coordinate of the point.</summary>*/
		public int X;
		/**<summary>The y coordinate of the point.</summary>*/
		public int Y;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/**<summary>Constructs a point positioned at the specified coordinates.</summary>*/
		public Point2I(int x, int y) {
			this.X	= x;
			this.Y	= y;
		}

		/**<summary>Constructs a point positioned at the specified coordinates.</summary>*/
		public Point2I(int uniform) {
			this.X	= uniform;
			this.Y	= uniform;
		}

		/**<summary>Constructs a copy of the specified point.</summary>*/
		public Point2I(Point2I p) {
			this.X	= p.X;
			this.Y	= p.Y;
		}

		/**<summary>Constructs a copy of the specified point.</summary>*/
		public Point2I(Vector2F v) {
			this.X	= (int)v.X;
			this.Y	= (int)v.Y;
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/**<summary>Outputs a string representing this point as (x, y).</summary>*/
		public override string ToString() {
			return "(" + X + ", " + Y + ")";
		}

		/**<summary>Outputs a string representing this point as (x, y).</summary>*/
		public string ToString(IFormatProvider provider) {
			// TODO: Write formatting for Point2I.ToString(format).

			return "(" + X.ToString(provider) + ", " + Y.ToString(provider) + ")";
		}

		/**<summary>Outputs a string representing this point as (x, y).</summary>*/
		public string ToString(string format, IFormatProvider provider) {
			return "(" + X.ToString(format, provider) + ", " + Y.ToString(format, provider) + ")";
		}

		/**<summary>Outputs a string representing this point as (x, y).</summary>*/
		public string ToString(string format) {
			return "(" + X.ToString(format) + ", " + Y.ToString(format) + ")";
		}

		/**<summary>Returns true if the specified point has the same x and y coordinates.</summary>*/
		public override bool Equals(object obj) {
			if (obj is Point2I)
				return (X == ((Point2I)obj).X && Y == ((Point2I)obj).Y);
			return false;
		}

		/**<summary>Returns the hash code for this point.</summary>*/
		public override int GetHashCode() {
			return base.GetHashCode();
		}

		/**<summary>Parses the point.</summary>*/
		public static Point2I Parse(string text) {
			Point2I value = Point2I.Zero;

			if (text.Length > 0) {
				if (text[0] == '(' && text[text.Length - 1] == ')')
					text = text.Substring(1, text.Length - 2);

				int commaPos = text.IndexOf(',');
				if (commaPos == -1)
					commaPos = text.IndexOf(' ');
				if (commaPos != -1) {

					string strX = text.Substring(0, commaPos).Trim();
					string strY = text.Substring(commaPos + 1).Trim();

					try {
						value.X = int.Parse(strX);
						value.Y = int.Parse(strY);
					} catch (FormatException e) {
						throw e;
					} catch (ArgumentNullException e) {
						throw e;
					} catch (OverflowException e) {
						throw e;
					}
				}
				else {
					throw new FormatException();
				}
			}
			else {
				throw new ArgumentNullException();
			}

			return value;
		}
		

		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static Point2I operator +(Point2I p) {
			return p;
		}

		public static Point2I operator -(Point2I p) {
			return new Point2I(-p.X, -p.Y);
		}

		public static Point2I operator ++(Point2I p) {
			return new Point2I(++p.X, ++p.Y);
		}

		public static Point2I operator --(Point2I p) {
			return new Point2I(--p.X, --p.Y);
		}

		//--------------------------------

		public static Point2I operator +(Point2I p1, Point2I p2) {
			return new Point2I(p1.X + p2.X, p1.Y + p2.Y);
		}

		public static Point2I operator +(int i1, Point2I p2) {
			return new Point2I(i1 + p2.X, i1 + p2.Y);
		}

		public static Point2I operator +(Point2I p1, int i2) {
			return new Point2I(p1.X + i2, p1.Y + i2);
		}

		public static Point2I operator -(Point2I p1, Point2I p2) {
			return new Point2I(p1.X - p2.X, p1.Y - p2.Y);
		}

		public static Point2I operator -(int i1, Point2I p2) {
			return new Point2I(i1 - p2.X, i1 - p2.Y);
		}

		public static Point2I operator -(Point2I p1, int i2) {
			return new Point2I(p1.X - i2, p1.Y - i2);
		}

		public static Point2I operator *(Point2I p1, Point2I p2) {
			return new Point2I(p1.X * p2.X, p1.Y * p2.Y);
		}

		public static Point2I operator *(int i1, Point2I p2) {
			return new Point2I(i1 * p2.X, i1 * p2.Y);
		}

		public static Point2I operator *(Point2I p1, int i2) {
			return new Point2I(p1.X * i2, p1.Y * i2);
		}

		public static Point2I operator /(Point2I p1, Point2I p2) {
			return new Point2I(p1.X / p2.X, p1.Y / p2.Y);
		}

		public static Point2I operator /(int i1, Point2I p2) {
			return new Point2I(i1 / p2.X, i1 / p2.Y);
		}

		public static Point2I operator /(Point2I p1, int i2) {
			return new Point2I(p1.X / i2, p1.Y / i2);
		}

		public static Point2I operator %(Point2I p1, Point2I p2) {
			return new Point2I(p1.X % p2.X, p1.Y % p2.Y);
		}

		public static Point2I operator %(int i1, Point2I p2) {
			return new Point2I(i1 % p2.X, i1 % p2.Y);
		}

		public static Point2I operator %(Point2I p1, int i2) {
			return new Point2I(p1.X % i2, p1.Y % i2);
		}

		//--------------------------------

		public static bool operator ==(Point2I p1, Point2I p2) {
			return (p1.X == p2.X && p1.Y == p2.Y);
		}

		public static bool operator ==(int i1, Point2I p2) {
			return (i1 == p2.X && i1 == p2.Y);
		}

		public static bool operator ==(Point2I p1, int i2) {
			return (p1.X == i2 && p1.Y == i2);
		}

		public static bool operator !=(Point2I p1, Point2I p2) {
			return (p1.X != p2.X || p1.Y != p2.Y);
		}

		public static bool operator !=(int i1, Point2I p2) {
			return (i1 != p2.X || i1 != p2.Y);
		}

		public static bool operator !=(Point2I p1, int i2) {
			return (p1.X != i2 || p1.Y != i2);
		}

		public static bool operator <(Point2I p1, Point2I p2) {
			return (p1.X < p2.X && p1.Y < p2.Y);
		}

		public static bool operator <(int i1, Point2I p2) {
			return (i1 < p2.X && i1 < p2.Y);
		}

		public static bool operator <(Point2I p1, int i2) {
			return (p1.X < i2 && p1.Y < i2);
		}

		public static bool operator >(Point2I p1, Point2I p2) {
			return (p1.X > p2.X && p1.Y > p2.Y);
		}

		public static bool operator >(int i1, Point2I p2) {
			return (i1 > p2.X && i1 > p2.Y);
		}

		public static bool operator >(Point2I p1, int i2) {
			return (p1.X > i2 && p1.Y > i2);
		}

		public static bool operator <=(Point2I p1, Point2I p2) {
			return (p1.X <= p2.X && p1.Y <= p2.Y);
		}

		public static bool operator <=(int i1, Point2I p2) {
			return (i1 <= p2.X && i1 <= p2.Y);
		}

		public static bool operator <=(Point2I p1, int i2) {
			return (p1.X <= i2 && p1.Y <= i2);
		}

		public static bool operator >=(Point2I p1, Point2I p2) {
			return (p1.X >= p2.X && p1.Y >= p2.Y);
		}

		public static bool operator >=(int i1, Point2I p2) {
			return (i1 >= p2.X && i1 >= p2.Y);
		}

		public static bool operator >=(Point2I p1, int i2) {
			return (p1.X >= i2 && p1.Y >= i2);
		}


		//-----------------------------------------------------------------------------
		// Implicit Conversions
		//-----------------------------------------------------------------------------

		public static implicit operator Point2I(Point p) {
			return new Point2I(p.X, p.Y);
		}

		public static implicit operator Point2I(GdiPoint p) {
			return new Point2I(p.X, p.Y);
		}

		public static implicit operator Point2I(GdiSize s) {
			return new Point2I(s.Width, s.Height);
		}

		public static implicit operator GdiPoint(Point2I p) {
			return new GdiPoint(p.X, p.Y);
		}

		public static implicit operator GdiSize(Point2I p) {
			return new GdiSize(p.X, p.Y);
		}

		public static implicit operator GdiPointF(Point2I p) {
			return new GdiPointF(p.X, p.Y);
		}

		public static implicit operator GdiSizeF(Point2I p) {
			return new GdiSizeF(p.X, p.Y);
		}


		//-----------------------------------------------------------------------------
		// Explicit Conversions
		//-----------------------------------------------------------------------------

		public static explicit operator Point2I(int i) {
			return new Point2I(i);
		}

		public static explicit operator Point(Point2I p) {
			return new Point(p.X, p.Y);
		}

		public static explicit operator Vector2(Point2I p) {
			return new Vector2((float)p.X, (float)p.Y);
		}

		public static explicit operator Point2I(GdiPointF p) {
			return new Point2I((int)p.X, (int)p.Y);
		}

		public static explicit operator Point2I(GdiSizeF s) {
			return new Point2I((int)s.Width, (int)s.Height);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/**<summary>Clamps the point with given bounds of the rectangle. Clamp is exclusive.</summary>*/
		public static Point2I Clamp(Point2I value, Rectangle2I bounds) {
			if (bounds.IsEmpty)
				return Point2I.Zero;
			return new Point2I(
				GMath.Clamp(value.X, bounds.Min.X, bounds.Max.X - 1),
				GMath.Clamp(value.Y, bounds.Min.Y, bounds.Max.Y - 1));
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/**<summary>Gets or sets the direction of the point.</summary>*/
		[ContentSerializerIgnore]
		public float Direction {
			get {
				if (X == 0 && Y == 0)
					return 0.0f;
				return GMath.Plusdir(GMath.Atan2(Y, X));
			}
			set {
				float length = GMath.Sqrt((X * X) + (Y * Y));
				X = (int)(length * GMath.Cos(value));
				Y = (int)(length * GMath.Sin(value));
			}
		}

		/**<summary>Gets or sets the length of the point.</summary>*/
		[ContentSerializerIgnore]
		public float Length {
			get {
				return GMath.Sqrt((X * X) + (Y * Y));
			}
			set {
				float oldLength = GMath.Sqrt((X * X) + (Y * Y));
				if (oldLength > 0) {
					X = (int)(X * (value / oldLength));
					Y = (int)(Y * (value / oldLength));
				}
				else {
					X = (int)value;
					Y = 0;
				}
			}
		}

		/**<summary>Gets or sets the x or y coordinate from the index.</summary>*/
		[ContentSerializerIgnore]
		public int this[int coordinate] {
			get {
				if (coordinate == 0)
					return X;
				else if (coordinate == 1)
					return Y;
				else
					throw new IndexOutOfRangeException("Point2I[coordinateIndex] must be either 0 or 1.");
			}
			set {
				if (coordinate == 0)
					X = value;
				else if (coordinate == 1)
					Y = value;
				else
					throw new IndexOutOfRangeException("Point2I[coordinateIndex] must be either 0 or 1.");
			}
		}

		/**<summary>Returns true if the point is positioned at (0, 0).</summary>*/
		[ContentSerializerIgnore]
		public bool IsZero {
			get { return (X == 0 && Y == 0); }
		}

		/**<summary>Returns the perpendicular point.</summary>*/
		[ContentSerializerIgnore]
		public Point2I Perpendicular {
			get { return new Point2I(-Y, X); }
		}

	}
} // End namespace
