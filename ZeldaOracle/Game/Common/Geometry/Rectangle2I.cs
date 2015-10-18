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
	
	// The 2D integer precision rectangle with basic operations and functions.
	public struct Rectangle2I {

		//========== CONSTANTS ===========

		/** <summary> Returns an empty rectangle. </summary> */
		public static Rectangle2I Zero {
			get { return new Rectangle2I(); }
		}

		//=========== MEMBERS ============

		/** <summary> The position of the rectangle. </summary> */
		[ContentSerializerIgnore]
		public Point2I Point;
		/** <summary> The size of the rectangle. </summary> */
		[ContentSerializerIgnore]
		public Point2I Size;

		//========= CONSTRUCTORS =========

		/** <summary> Constructs a rectangle with the specified position and size. </summary> */
		public Rectangle2I(int x, int y, int width, int height) {
			this.Point	= new Point2I(x, y);
			this.Size	= new Point2I(width, height);
		}
		/** <summary> Constructs a rectangle with the specified position and size. </summary> */
		public Rectangle2I(Point2I point, int width, int height) {
			this.Point	= point;
			this.Size	= new Point2I(width, height);
		}
		/** <summary> Constructs a rectangle with the specified position and size. </summary> */
		public Rectangle2I(int x, int y, Point2I size) {
			this.Point	= new Point2I(x, y);
			this.Size	= size;
		}
		/** <summary> Constructs a rectangle with the specified position and size. </summary> */
		public Rectangle2I(Point2I point, Point2I size) {
			this.Point	= point;
			this.Size	= size;
		}
		/** <summary> Constructs a rectangle with the specified size. </summary> */
		public Rectangle2I(int size) {
			this.Point	= Point2I.Zero;
			this.Size	= new Point2I(size);
		}
		/** <summary> Constructs a rectangle with the specified size. </summary> */
		public Rectangle2I(int width, int height) {
			this.Point	= Point2I.Zero;
			this.Size	= new Point2I(width, height);
		}
		/** <summary> Constructs a rectangle with the specified size. </summary> */
		public Rectangle2I(Point2I size) {
			this.Point	= Point2I.Zero;
			this.Size	= size;
		}
		/** <summary> Constructs a copy of the specified rectangle. </summary> */
		public Rectangle2I(Rectangle2I r) {
			this.Point	= r.Point;
			this.Size	= r.Size;
		}

		//=========== GENERAL ============

		/** <summary> Outputs a string representing this rectangle. </summary> */
		public override string ToString() {
			return "((" + X + "," + Y + "), (" + Width + "x" + Height + "))";
		}
		/** <summary> Outputs a string representing this rectangle. </summary> */
		public string ToString(IFormatProvider provider) {
			return "((" + X + "," + Y + "), (" + Width + "x" + Height + "))";
		}
		/** <summary> Outputs a string representing this rectangle. </summary> */
		public string ToString(string format) {
			return "((" + X + "," + Y + "), (" + Width + "x" + Height + "))";
		}
		/** <summary> Outputs a string representing this rectangle. </summary> */
		public string ToString(string format, IFormatProvider provider) {
			return "((" + X + "," + Y + "), (" + Width + "x" + Height + "))";
		}
		/** <summary> Returns true if the specified rectangle has the same x and y coordinates. </summary> */
		public override bool Equals(object obj) {
			if (obj is Rectangle2I)
				return (Point == ((Rectangle2I)obj).Point && Size == ((Rectangle2I)obj).Size);
			return false;
		}
		/** <summary> Returns the hash code for this rectangle. </summary> */
		public override int GetHashCode() {
			return base.GetHashCode();
		}

		//========== OPERATORS ===========

		public static Rectangle2I operator +(Rectangle2I r) {
			return r;
		}
		public static Rectangle2I operator -(Rectangle2I r) {
			return new Rectangle2I(r.Point, -r.Size);
		}
		public static Rectangle2I operator ++(Rectangle2I r) {
			return new Rectangle2I(++r.Point, r.Size);
		}
		public static Rectangle2I operator --(Rectangle2I r) {
			return new Rectangle2I(--r.Point, r.Size);
		}

		//--------------------------------

		public static Rectangle2I operator +(Rectangle2I r, Point2I p) {
			return new Rectangle2I(r.Point + p, r.Size);
		}
		public static Rectangle2I operator +(Rectangle2I r, int i) {
			return new Rectangle2I(r.Point + i, r.Size);
		}
		public static Rectangle2I operator -(Rectangle2I r, Point2I p) {
			return new Rectangle2I(r.Point - p, r.Size);
		}
		public static Rectangle2I operator -(Rectangle2I r, int i) {
			return new Rectangle2I(r.Point - i, r.Size);
		}

		public static Rectangle2I operator *(Rectangle2I r, Point2I p) {
			return new Rectangle2I(r.Point, r.Size * p);
		}
		public static Rectangle2I operator *(Rectangle2I r, int i) {
			return new Rectangle2I(r.Point, r.Size * i);
		}
		public static Rectangle2I operator /(Rectangle2I r, Point2I p) {
			return new Rectangle2I(r.Point, r.Size / p);
		}
		public static Rectangle2I operator /(Rectangle2I r, int i) {
			return new Rectangle2I(r.Point, r.Size / i);
		}
		public static Rectangle2I operator %(Rectangle2I r, Point2I p) {
			return new Rectangle2I(r.Point, r.Size % p);
		}
		public static Rectangle2I operator %(Rectangle2I r, int i) {
			return new Rectangle2I(r.Point, r.Size % i);
		}

		//--------------------------------

		public static bool operator ==(Rectangle2I r1, Rectangle2I r2) {
			return (r1.Point == r2.Point && r1.Size == r2.Size);
		}
		public static bool operator !=(Rectangle2I r1, Rectangle2I r2) {
			return (r1.Point != r2.Point || r1.Size != r2.Size);
		}

		//--------------------------------

		public static implicit operator Rectangle2I(Rectangle r) {
			return new Rectangle2I(r.X, r.Y, r.Width, r.Height);
		}

		public static explicit operator Rectangle(Rectangle2I r) {
			return new Rectangle(r.Point.X, r.Point.Y, r.Size.X, r.Size.Y);
		}

		//--------------------------------

		public static implicit operator Rectangle2I(GdiRectangle r) {
			return new Rectangle2I(r.X, r.Y, r.Width, r.Height);
		}
		public static explicit operator Rectangle2I(GdiRectangleF r) {
			return new Rectangle2I((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
		}

		public static implicit operator GdiRectangle(Rectangle2I r) {
			return new GdiRectangle(r.X, r.Y, r.Width, r.Height);
		}
		public static implicit operator GdiRectangleF(Rectangle2I r) {
			return new GdiRectangleF(r.X, r.Y, r.Width, r.Height);
		}

		//========== PROPERTIES ==========
	
		// Dimensions

		/** <summary> Gets or sets the x position of the rectangle. </summary> */
		public int X {
			get { return Point.X; }
			set { Point.X = value; }
		}
		/** <summary> Gets or sets the y position of the rectangle. </summary> */
		public int Y {
			get { return Point.Y; }
			set { Point.Y = value; }
		}
		/** <summary> Gets or sets the width of the rectangle. </summary> */
		public int Width {
			get { return Size.X; }
			set { Size.X = value; }
		}
		/** <summary> Gets or sets the height of the rectangle. </summary> */
		public int Height {
			get { return Size.Y; }
			set { Size.Y = value; }
		}

		// Shape

		/** <summary> Returns true if the rectangle has a size of zero. </summary> */
		public bool IsEmpty {
			get { return Size.IsZero; }
		}
		/** <summary> Gets the center of the rectangle. </summary> */
		public Point2I Center {
			get { return Point + Size / 2; }
		}
		/** <summary> Gets the bounding box of the rectangle. </summary> */
		public Rectangle2I Bounds {
			get { return this; }
		}
		/** <summary> Gets the maximum point of the rectangle. </summary> */
		public Point2I Max {
			get { return GMath.Max(Point, Point + Size); }
		}
		/** <summary> Gets the minimum point of the rectangle. </summary> */
		public Point2I Min {
			get { return GMath.Min(Point, Point + Size); }
		}

		/** <summary> Gets the area of the rectangle. </summary> */
		public int Area {
			get { return GMath.Abs(Size.X * Size.Y); }
		}
		/** <summary> Gets the perimeter of the rectangle. </summary> */
		public int Perimeter {
			get { return GMath.Abs(Size.X * 2) + GMath.Abs(Size.Y * 2); }
		}
	
		public int     Left        { get { return Point.X; } }
		public int     Top         { get { return Point.Y; } }
		public int     Right       { get { return Point.X + Size.X; } }
		public int     Bottom      { get { return Point.Y + Size.Y; } }
		public Point2I TopRight    { get { return new Point2I(Right, Top); } }
		public Point2I TopLeft     { get { return new Point2I(Left,  Top); } }
		public Point2I BottomRight { get { return new Point2I(Right, Bottom); } }
		public Point2I BottomLeft  { get { return new Point2I(Left,  Bottom); } }
		public int     Parimeter   { get { return (2 * (Math.Abs(Width) + Math.Abs(Height))); } }


		//========= CALCULATIONS =========

		public int GetEdge(int direction) {
			if (direction == Directions.Right)
				return Right;
			if (direction == Directions.Left)
				return Left;
			if (direction == Directions.Up)
				return Top;
			return Bottom;
		}

		/** <summary> Returns a rectangle with the corners stretched out by the specified amount. </summary> */
		public Rectangle2I Inflated(Point2I amount) {
			return new Rectangle2I(Point - amount, Size + amount * 2);
		}
		/** <summary> Returns a rectangle with the corners stretched out by the specified amount. </summary> */
		public Rectangle2I Inflated(int x, int y) {
			return new Rectangle2I(Point - new Point2I(x, y), Size + new Point2I(x * 2, y * 2));
		}
		/** <summary> Stretches the corners of the rectangle out by the specified amount. </summary> */
		public void Inflate(Point2I amount) {
			Point -= amount;
			Size += amount * 2;
		}
		/** <summary> Stretches the corners of the rectangle out by the specified amount. </summary> */
		public void Inflate(int x, int y) {
			Point.X -= x;
			Point.Y -= y;
			Size.X += x * 2;
			Size.Y += y * 2;
		}

		//=========== CONTAINS ===========

		// Returns true if the specified vector is inside this rectangle.
		public bool Contains(Point2I point) {
			return ((point <  Max) &&
					(point >= Min));
		}
		
		// Returns true if the specified vector is inside this rectangle.
		public bool Contains(Vector2F point) {
			return (point.X >= Left &&
					point.Y >= Top &&
					point.X < Right &&
					point.Y < Bottom);
		}

		// Returns true if the specified rectangle is inside this rectangle.
		public bool Contains(Rectangle2I rect) {
			if (IsEmpty || rect.IsEmpty)
				return false;
			return ((rect.Min >= Min) &&
					(rect.Max <= Max));
		}


		//========== COLLISION ===========

		/** <summary> Returns true if the specified vector is colliding with this rectangle. </summary> */
		public bool Colliding(Point2I point) {
			return ((point <  Max) &&
					(point >= Min));
		}
		/** <summary> Returns true if the specified rectangle is colliding with this rectangle. </summary> */
		public bool Colliding(Rectangle2I rect) {
			if (IsEmpty || rect.IsEmpty)
				return false;
			return ((rect.Min < Max) &&
					(rect.Max > Min));
		}


		//-----------------------------------------------------------------------------
		// Static methods
		//-----------------------------------------------------------------------------
        
        // Return the intersection between two rectangles.
        // Returns the Empty rect if there is no intersection.
        public static Rectangle2I Intersect(Rectangle2I r1, Rectangle2I r2)
        {
            int x1 = Math.Max(r1.Left,   r2.Left);
            int y1 = Math.Max(r1.Top,    r2.Top);
            int x2 = Math.Min(r1.Right,  r2.Right);
            int y2 = Math.Min(r1.Bottom, r2.Bottom);
            if (x2 > x1 && y2 > y1)
                return new Rectangle2I(x1, y1, x2 - x1, y2 - y1);
            return Rectangle2I.Zero;
        }
        
        // Creates a new rectangle that exactly contains two other rectangles.
        public static Rectangle2I Union(Rectangle2I r1, Rectangle2I r2)
        {
            int x1 = Math.Min(r1.Left,   r2.Left);
            int y1 = Math.Min(r1.Top,    r2.Top);
            int x2 = Math.Max(r1.Right,  r2.Right);
            int y2 = Math.Max(r1.Bottom, r2.Bottom);
            return new Rectangle2I(x1, y1, x2 - x1, y2 - y1);
        }

}
} // End namespace
