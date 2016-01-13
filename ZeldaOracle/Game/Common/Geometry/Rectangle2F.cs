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
/** <summary>
 * The 2D floating precision rectangle with numerous operations and functions.
 * </summary> */
public struct Rectangle2F {

	//========== CONSTANTS ===========

	/** <summary> Returns an empty rectangle. </summary> */
	public static Rectangle2F Zero {
		get { return new Rectangle2F(); }
	}

	//=========== MEMBERS ============

	/** <summary> The position of the rectangle. </summary> */
	[ContentSerializerIgnore]
	public Vector2F Point;
	/** <summary> The size of the rectangle. </summary> */
	[ContentSerializerIgnore]
	public Vector2F Size;

	//========= CONSTRUCTORS =========

	/** <summary> Constructs a rectangle with the specified position and size. </summary> */
	public Rectangle2F(float x, float y, float width, float height) {
		this.Point	= new Vector2F(x, y);
		this.Size	= new Vector2F(width, height);
	}
	/** <summary> Constructs a rectangle with the specified position and size. </summary> */
	public Rectangle2F(Vector2F point, float width, float height) {
		this.Point	= point;
		this.Size	= new Vector2F(width, height);
	}
	/** <summary> Constructs a rectangle with the specified position and size. </summary> */
	public Rectangle2F(float x, float y, Vector2F size) {
		this.Point	= new Vector2F(x, y);
		this.Size	= size;
	}
	/** <summary> Constructs a rectangle with the specified position and size. </summary> */
	public Rectangle2F(Vector2F point, Vector2F size) {
		this.Point	= point;
		this.Size	= size;
	}
	/** <summary> Constructs a rectangle with the specified size. </summary> */
	public Rectangle2F(float size) {
		this.Point	= Vector2F.Zero;
		this.Size	= new Vector2F(size);
	}
	/** <summary> Constructs a rectangle with the specified size. </summary> */
	public Rectangle2F(float width, float height) {
		this.Point	= Vector2F.Zero;
		this.Size	= new Vector2F(width, height);
	}
	/** <summary> Constructs a rectangle with the specified size. </summary> */
	public Rectangle2F(Vector2F size) {
		this.Point	= Vector2F.Zero;
		this.Size	= size;
	}
	/** <summary> Constructs a copy of the specified rectangle. </summary> */
	public Rectangle2F(Rectangle2F r) {
		this.Point	= r.Point;
		this.Size	= r.Size;
	}

	//=========== GENERAL ============
	#region General

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
		if (obj is Rectangle2F)
			return (Point == ((Rectangle2F)obj).Point && Size == ((Rectangle2F)obj).Size);
		return false;
	}
	/** <summary> Returns the hash code for this rectangle. </summary> */
	public override int GetHashCode() {
		return base.GetHashCode();
	}

	#endregion
	//========== OPERATORS ===========
	
	public static Rectangle2F operator +(Rectangle2F r) {
		return r;
	}
	public static Rectangle2F operator -(Rectangle2F r) {
		return new Rectangle2F(r.Point, -r.Size);
	}
	public static Rectangle2F operator ++(Rectangle2F r) {
		return new Rectangle2F(++r.Point, r.Size);
	}
	public static Rectangle2F operator --(Rectangle2F r) {
		return new Rectangle2F(--r.Point, r.Size);
	}

	//--------------------------------

	public static Rectangle2F operator +(Rectangle2F r, Vector2F v) {
		return new Rectangle2F(r.Point + v, r.Size);
	}
	/*public static Rectangle2F operator +(Rectangle2F r, float f) {
		return new Rectangle2F(r.Point + new Vector2F(f, f), r.Size);
	}*/
	public static Rectangle2F operator -(Rectangle2F r, Vector2F v) {
		return new Rectangle2F(r.Point - v, r.Size);
	}
	/*public static Rectangle2F operator -(Rectangle2F r, float f) {
		return new Rectangle2F(r.Point - new Vector2F(f, f), r.Size);
	}*/

	public static Rectangle2F operator *(Rectangle2F r, Vector2F v) {
		return new Rectangle2F(r.Point, r.Size * v);
	}
	public static Rectangle2F operator *(Rectangle2F r, float f) {
		return new Rectangle2F(r.Point, r.Size * f);
	}
	public static Rectangle2F operator /(Rectangle2F r, Vector2F v) {
		return new Rectangle2F(r.Point, r.Size / v);
	}
	public static Rectangle2F operator /(Rectangle2F r, float f) {
		return new Rectangle2F(r.Point, r.Size / f);
	}
	public static Rectangle2F operator %(Rectangle2F r, Vector2F v) {
		return new Rectangle2F(r.Point, r.Size % v);
	}
	public static Rectangle2F operator %(Rectangle2F r, float f) {
		return new Rectangle2F(r.Point, r.Size % f);
	}

	//--------------------------------

	public static bool operator ==(Rectangle2F r1, Rectangle2F r2) {
		return (r1.Point == r2.Point && r1.Size == r2.Size);
	}
	public static bool operator !=(Rectangle2F r1, Rectangle2F r2) {
		return (r1.Point != r2.Point || r1.Size != r2.Size);
	}

	//--------------------------------
	
	public static implicit operator Rectangle2F(Rectangle2I r) {
		return new Rectangle2F(r.X, r.Y, r.Width, r.Height);
	}
	public static implicit operator Rectangle2F(Rectangle r) {
		return new Rectangle2F(r.X, r.Y, r.Width, r.Height);
	}

	public static explicit operator Rectangle2I(Rectangle2F r) {
		return new Rectangle2I((int)r.Point.X, (int)r.Point.Y, (int)r.Size.X, (int)r.Size.Y);
	}
	public static explicit operator Rectangle(Rectangle2F r) {
		return new Rectangle((int)r.Point.X, (int)r.Point.Y, (int)r.Size.X, (int)r.Size.Y);
	}

	//--------------------------------

	public static implicit operator Rectangle2F(GdiRectangle r) {
		return new Rectangle2F(r.X, r.Y, r.Width, r.Height);
	}
	public static implicit operator Rectangle2F(GdiRectangleF r) {
		return new Rectangle2F(r.X, r.Y, r.Width, r.Height);
	}

	public static explicit operator GdiRectangle(Rectangle2F r) {
		return new GdiRectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
	}
	public static implicit operator GdiRectangleF(Rectangle2F r) {
		return new GdiRectangleF(r.X, r.Y, r.Width, r.Height);
	}

	//========== PROPERTIES ==========

	/** <summary> Gets or sets the x position of the rectangle. </summary> */
	public float X {
		get { return Point.X; }
		set { Point.X = value; }
	}
	/** <summary> Gets or sets the y position of the rectangle. </summary> */
	public float Y {
		get { return Point.Y; }
		set { Point.Y = value; }
	}
	/** <summary> Gets or sets the width of the rectangle. </summary> */
	[ContentSerializer(ElementName = "W")]
	public float Width {
		get { return Size.X; }
		set { Size.X = value; }
	}
	/** <summary> Gets or sets the height of the rectangle. </summary> */
	[ContentSerializer(ElementName = "H")]
	public float Height {
		get { return Size.Y; }
		set { Size.Y = value; }
	}

	//--------------------------------

	/** <summary> Returns true if the rectangle has an size of zero. </summary> */
	public bool IsEmpty {
		get { return Size.IsZero; }
	}
	/** <summary> Gets the center of the rectangle. </summary> */
	public Vector2F Center {
		get { return Point + Size / 2; }
	}
	/** <summary> Gets the bounding box of the rectangle. </summary> */
	public Rectangle2F Bounds {
		get { return this; }
	}
	/** <summary> Gets the maximum point of the rectangle. </summary> */
	public Vector2F Max {
		get { return GMath.Max(Point, Point + Size); }
	}
	/** <summary> Gets the minimum point of the rectangle. </summary> */
	public Vector2F Min {
		get { return GMath.Min(Point, Point + Size); }
	}

	/** <summary> Gets the area of the rectangle. </summary> */
	public float Area {
		get { return GMath.Abs(Size.X * Size.Y); }
	}
	/** <summary> Gets the perimeter of the rectangle. </summary> */
	public float Perimeter {
		get { return GMath.Abs(Size.X * 2) + GMath.Abs(Size.Y * 2); }
	}

	/** <summary> Gets the number of points in the rectangle. </summary> */
	public int NumPoints {
		get { return 2; }
	}
	/** <summary> Gets the number of lines in the rectangle. </summary> */
	public int NumLines {
		get { return 1; }
	}
	/** <summary> Gets the list of points in the rectangle. </summary> */
	public Vector2F[] Points {
		get {
			return new Vector2F[] {
				Point,
				Point + new Vector2F(Size.X, 0),
				Point + Size,
				Point + new Vector2F(0, Size.Y)
			};
		}
	}
	/** <summary> Gets the list of lines in the rectangle. </summary> */
	public Line2F[] Lines {
		get {
			return new Line2F[] {
				new Line2F(Point, Point + new Vector2F(Size.X, 0)),
				new Line2F(Point + new Vector2F(Size.X, 0), Point + Size),
				new Line2F(Point + Size, Point + new Vector2F(0, Size.Y)),
				new Line2F(Point + new Vector2F(0, Size.Y), Point)
			};
		}
	}
	
	public float	Left		{ get { return Point.X; } }
	public float	Top			{ get { return Point.Y; } }
	public float	Right		{ get { return Point.X + Size.X; } }
	public float	Bottom		{ get { return Point.Y + Size.Y; } }
	public Vector2F	TopRight	{ get { return new Vector2F(Right, Top); } }
	public Vector2F	TopLeft		{ get { return new Vector2F(Left,  Top); } }
	public Vector2F	BottomRight	{ get { return new Vector2F(Right, Bottom); } }
	public Vector2F	BottomLeft	{ get { return new Vector2F(Left,  Bottom); } }


	//========= CALCULATIONS =========

	public float GetEdge(int direction) {
		if (direction == Directions.Right)
			return Right;
		if (direction == Directions.Left)
			return Left;
		if (direction == Directions.Up)
			return Top;
		return Bottom;
	}

	/** <summary> Returns the point on the rectangle based on the its perimeter. </summary> */
	public Vector2F PositionAt(float length, bool asRatio = false) {
		if (asRatio)
			length *= Perimeter;
		if (length < 0 || length > Perimeter)
			return Point;
		foreach (Line2F l in Lines) {
			if (length <= l.Length)
				return l.PositionAt(length);
			length -= l.Length;
		}
		return Point;
	}

	/** <summary> Returns a rectangle with the corners stretched out by the specified amount. </summary> */
	public Rectangle2F Inflated(Vector2F amount) {
		return new Rectangle2F(Point - amount, Size + amount * 2.0f);
	}
	/** <summary> Returns a rectangle with the corners stretched out by the specified amount. </summary> */
	public Rectangle2F Inflated(float x, float y) {
		return new Rectangle2F(Point - new Vector2F(x, y), Size + new Vector2F(x * 2.0f, y * 2.0f));
	}
	/** <summary> Stretches the corners of the rectangle out by the specified amount. </summary> */
	public void Inflate(Vector2F amount) {
		Point -= amount;
		Size += amount * 2.0f;
	}
	/** <summary> Stretches the corners of the rectangle out by the specified amount. </summary> */
	public void Inflate(float x, float y) {
		Point.X -= x;
		Point.Y -= y;
		Size.X += x * 2.0f;
		Size.Y += y * 2.0f;
	}

	//=========== CONTAINS ===========

	/** <summary> Returns true if the specified vector is inside this rectangle. </summary> */
	public bool Contains(Vector2F point) {
		return ((point <  Max) &&
				(point >= Min));
	}
	/** <summary> Returns true if the specified line is inside this rectangle. </summary> */
	public bool Contains(Line2F line) {
		if (IsEmpty || line.IsEmpty)
			return false;
		return ((line.Min <  Max) &&
				(line.Max >= Min));
	}
	/** <summary> Returns true if the specified rectangle is inside this rectangle. </summary> */
	public bool Contains(Rectangle2F rect) {
		if (IsEmpty || rect.IsEmpty)
			return false;
		return ((rect.Min >= Min) &&
				(rect.Max <= Max));
	}

	//========== COLLISION ===========

	/** <summary> Returns true if the specified vector is colliding with this rectangle. </summary> */
	public bool Colliding(Vector2F point) {
		return ((point <  Max) &&
				(point >= Min));
	}
	/** <summary> Returns true if the specified line is colliding with this rectangle. </summary> */
	public bool Colliding(Line2F line) {
		if (IsEmpty || line.IsEmpty)
			return false;
		if (!Colliding(line.Bounds))
			return false;
		if (Contains(line.End1))
			return true;
		foreach (Line2F l in Lines) {
			if (l.Colliding(line))
				return true;
		}
		return false;
	}
	/** <summary> Returns true if the specified rectangle is colliding with this rectangle. </summary> */
	public bool Colliding(Rectangle2F rect) {
		if (IsEmpty || rect.IsEmpty)
			return false;
		return ((rect.Min < Max) &&
				(rect.Max > Min));
	}

    // Return whether this and another rectangle are intersecting.
    public bool Intersects(Rectangle2F other) {
		if (IsEmpty && other.IsEmpty)
			return false;
		return !(other.Left - Right >= 0 || other.Top - Bottom >= 0 ||
		         Left - other.Right >= 0 || Top - other.Bottom >= 0);
    }


	//-----------------------------------------------------------------------------
	// Static methods
	//-----------------------------------------------------------------------------
        
	// Return the intersection between two rectangles.
	// Returns the Empty rect if there is no intersection.
	public static Rectangle2F Intersect(Rectangle2F r1, Rectangle2F r2) {
		float x1 = Math.Max(r1.Left,   r2.Left);
		float y1 = Math.Max(r1.Top,    r2.Top);
		float x2 = Math.Min(r1.Right,  r2.Right);
		float y2 = Math.Min(r1.Bottom, r2.Bottom);
		if (x2 > x1 && y2 > y1)
			return new Rectangle2F(x1, y1, x2 - x1, y2 - y1);
		return Rectangle2F.Zero;
	}
        
	// Creates a new rectangle that exactly contains two other rectangles.
	public static Rectangle2F Union(Rectangle2F r1, Rectangle2F r2) {
		float x1 = Math.Min(r1.Left,   r2.Left);
		float y1 = Math.Min(r1.Top,    r2.Top);
		float x2 = Math.Max(r1.Right,  r2.Right);
		float y2 = Math.Max(r1.Bottom, r2.Bottom);
		return new Rectangle2F(x1, y1, x2 - x1, y2 - y1);
	}
        
    // Creates a copy of a rectangle translated by an amount.
    public static Rectangle2F Translate(Rectangle2F r, float x, float y) {
        r.Point.X += x;
		r.Point.Y += y;
        return r;
    }

    // Creates a copy of a rectangle translated by an amount.
    public static Rectangle2F Translate(Rectangle2F r, Vector2F amount) {
        r.Point += amount;
        return r;
    }

}
} // End namespace
