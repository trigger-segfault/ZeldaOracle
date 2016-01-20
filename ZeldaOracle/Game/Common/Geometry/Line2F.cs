using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ZeldaOracle.Common.Geometry {
/** <summary>
 * The 2D floating precision line with numerous operations and functions.
 * </summary> */
public struct Line2F {

	//========== CONSTANTS ===========

	/** <summary> Returns a line positioned at (0, 0). </summary> */
	public static Line2F Zero {
		get { return new Line2F(); }
	}

	//=========== MEMBERS ============

	/** <summary> The first endpoint of the line. </summary> */
	[ContentSerializerIgnore]
	public Vector2F End1;
	/** <summary> The second endpoint of the line. </summary> */
	[ContentSerializerIgnore]
	public Vector2F End2;

	//========= CONSTRUCTORS =========

	/** <summary> Constructs a line with the specified endpoints. </summary> */
	public Line2F(float x1, float y1, float x2, float y2) {
		this.End1	= new Vector2F(x1, y1);
		this.End2	= new Vector2F(x2, y2);
	}
	/** <summary> Constructs a line with the specified endpoints. </summary> */
	public Line2F(Vector2F end1, float x2, float y2) {
		this.End1	= end1;
		this.End2	= new Vector2F(x2, y2);
	}
	/** <summary> Constructs a line with the specified endpoints. </summary> */
	public Line2F(float x1, float y1, Vector2F end2) {
		this.End1	= new Vector2F(x1, y1);
		this.End2	= end2;
	}
	/** <summary> Constructs a line with the specified endpoints. </summary> */
	public Line2F(Vector2F end1, Vector2F end2) {
		this.End1	= end1;
		this.End2	= end2;
	}
	/** <summary> Constructs a line with the specified position and size. </summary> */
	public Line2F(float x, float y, float width, float height, bool asSize) {
		if (!asSize) {
			this.End1	= new Vector2F(x, y);
			this.End2	= new Vector2F(x + width, y + height);
		}
		else {
			this.End1	= new Vector2F(x, y);
			this.End2	= new Vector2F(width, height);
		}
	}
	/** <summary> Constructs a line with the specified position and size. </summary> */
	public Line2F(Vector2F point, float width, float height, bool asSize) {
		if (!asSize) {
			this.End1	= point;
			this.End2	= point + new Vector2F(width, height);
		}
		else {
			this.End1	= point;
			this.End2	= new Vector2F(width, height);
		}
	}
	/** <summary> Constructs a line with the specified position and size. </summary> */
	public Line2F(float x, float y, Vector2F size, bool asSize) {
		if (!asSize) {
			this.End1	= new Vector2F(x, y);
			this.End2	= new Vector2F(x, y) + size;
		}
		else {
			this.End1	= new Vector2F(x, y);
			this.End2	= size;
		}
	}
	/** <summary> Constructs a line with the specified position and size. </summary> */
	public Line2F(Vector2F point, Vector2F size, bool asSize) {
		if (!asSize) {
			this.End1	= point;
			this.End2	= point + size;
		}
		else {
			this.End1	= point;
			this.End2	= size;
		}
	}
	/** <summary> Constructs a line with the specified size. </summary> */
	public Line2F(float width, float height) {
		this.End1	= Vector2F.Zero;
		this.End2	= new Vector2F(width, height);
	}
	/** <summary> Constructs a line with the specified size. </summary> */
	public Line2F(Vector2F size) {
		this.End1	= Vector2F.Zero;
		this.End2	= size;
	}
	/** <summary> Constructs a copy of the specified line. </summary> */
	public Line2F(Line2F l) {
		this.End1	= l.End1;
		this.End2	= l.End2;
	}

	//=========== GENERAL ============

	/** <summary> Outputs a string representing this line as ((x1, y1), (x2, y2)). </summary> */
	public override string ToString() {
		return "(" + End1 + ", " + End2 + ")";
	}
	/** <summary> Outputs a string representing this line as ((x1, y1), (x2, y2)). </summary> */
	public string ToString(IFormatProvider provider) {
		return "(" + End1.ToString(provider) + ", " + End2.ToString(provider) + ")";
	}
	/** <summary> Outputs a string representing this line as ((x1, y1), (x2, y2)). </summary> */
	public string ToString(string format, IFormatProvider provider) {
		return "(" + End1.ToString(format, provider) + ", " + End2.ToString(format, provider) + ")";
	}
	/** <summary> Outputs a string representing this line as ((x1, y1), (x2, y2)). </summary> */
	public string ToString(string format) {
		return "(" + End1.ToString(format) + ", " + End2.ToString(format) + ")";
	}
	/** <summary> Returns true if the specified line has the same endpoints. </summary> */
	public override bool Equals(object obj) {
		if (obj is Line2F)
			return (End1 == ((Line2F)obj).End1 && End2 == ((Line2F)obj).End2);
		return false;
	}
	/** <summary> Returns the hash code of this line. </summary> */
	public override int GetHashCode() {
		return base.GetHashCode();
	}

	//========== OPERATORS ===========

	public static Line2F operator +(Line2F l) {
		return l;
	}
	public static Line2F operator -(Line2F l) {
		return new Line2F(l.End1, l.End1 - l.Size);
	}
	/*
	public static Line2F operator ++(Line2F l) {
		return new Line2F(++l.End1, ++l.End2);
	}
	public static Line2F operator --(Line2F l) {
		return new Line2F(--l.End1, --l.End2);
	}*/

	//--------------------------------

	public static Line2F operator +(Line2F l, Vector2F v) {
		return new Line2F(l.End1 + v, l.End2 + v);
	}
	/*public static Line2F operator +(Line2F l, float f) {
		return new Line2F(l.End1 + f, l.End2 + f);
	}*/

	public static Line2F operator -(Line2F l, Vector2F v) {
		return new Line2F(l.End1 - v, l.End2 - v);
	}
	/*public static Line2F operator -(Line2F l, float f) {
		return new Line2F(l.End1 - f, l.End2 - f);
	}*/

	//--------------------------------

	public static bool operator ==(Line2F l1, Line2F l2) {
		return (l1.End1 == l2.End1 && l1.End2 == l2.End2);
	}
	public static bool operator !=(Line2F l1, Line2F l2) {
		return (l1.End1 != l2.End1 || l1.End2 != l2.End2);
	}

	//--------------------------------

	/*public static implicit operator Line2F(Line2I l) {
		return new Line2D(l.End1, l.End2);
	}*/

	/*public static explicit operator Line2I(Line2F l) {
		return new Line2I((Point2I)l.End1, (Point2I)l.End2);
	}*/

	//========== PROPERTIES ==========

	// Dimensions

	/** <summary> Gets or sets the x position of the first endpoint. </summary> */
	public float X1 {
		get { return End1.X; }
		set { End1.X = value; }
	}
	/** <summary> Gets or sets the y position of the first endpoint. </summary> */
	public float Y1 {
		get { return End1.Y; }
		set { End1.Y = value; }
	}
	/** <summary> Gets or sets the x position of the second endpoint. </summary> */
	public float X2 {
		get { return End2.X; }
		set { End2.X = value; }
	}
	/** <summary> Gets or sets the y position of the second endpoint. </summary> */
	public float Y2 {
		get { return End2.Y; }
		set { End2.Y = value; }
	}
	/** <summary> Gets or sets the size of the line. </summary> */
	[ContentSerializerIgnore]
	public Vector2F Size {
		get { return End2 - End1; }
		set { End2 = End1 + value; }
	}
	/** <summary> Gets or sets the width of the line. </summary> */
	[ContentSerializerIgnore]
	public float Width {
		get { return End2.X - End1.X; }
		set { End2.X = End1.X + value; }
	}
	/** <summary> Gets or sets the height of the line. </summary> */
	[ContentSerializerIgnore]
	public float Height {
		get { return End2.Y - End1.Y; }
		set { End2.Y = End1.Y + value; }
	}
	/** <summary> Gets or sets the first or second endpoint from the index. </summary> */
	[ContentSerializerIgnore]
	public Vector2F this[int endpoint] {
		get {
			if (endpoint < 0 || endpoint > 1)
				throw new System.IndexOutOfRangeException("Line2F[endpointIndex] must be either 0 or 1.");
			else
				return (endpoint == 0 ? End1 : End2);
		}
		set {
			if (endpoint < 0 || endpoint > 1)
				throw new System.IndexOutOfRangeException("Line2F[endpointIndex] must be either 0 or 1.");
			else if (endpoint == 0)
				End1 = value;
			else
				End2 = value;
		}
	}

	// Line

	/** <summary> Gets or sets the direction of the line. </summary> */
	[ContentSerializerIgnore]
	public float Direction {
		get {
			return End1.DirectionTo(End2);
		}
		set {
			End2 = End1 + new Vector2F(Length, value, true);
		}
	}
	/** <summary> Gets or sets the length of the line. </summary> */
	[ContentSerializerIgnore]
	public float Length {
		get {
			return End1.DistanceTo(End2);
		}
		set {
			float oldLength = End1.DistanceTo(End2);
			End2 = End1 + new Vector2F(value, Direction, true);
		}
	}
	/** <summary> Returns true if the line is horizontal. </summary> */
	public bool IsHorizontal {
		get { return End2.Y - End1.Y == 0; }
	}
	/** <summary> Returns true if the line is vertical. </summary> */
	public bool IsVertical {
		get { return End2.X - End1.X == 0; }
	}
	/** <summary> Returns true if the line is straight. </summary> */
	public bool IsStraight {
		get { return (End2.Y - End1.Y == 0) || (End2.X - End1.X == 0); }
	}
	/** <summary> Returns the perpendicular line. </summary> */
	public Line2F Perpendicular {
		get { return new Line2F(End1, End1 + Size.Perpendicular); }
	}
	
	// Shape

	/** <summary> Returns true if the line is empty. </summary> */
	public bool IsEmpty {
		get { return (End2 - End1).IsZero; }
	}
	/** <summary> Gets the midpoint of the line. </summary> */
	public Vector2F Center {
		get { return (End1 + End2) / 2; }
	}
	/** <summary> Gets the bounding box of the line. </summary> */
	public Rectangle2F Bounds {
		get { return new Rectangle2F(End1, End2 - End1); }
	}
	/** <summary> Gets the maximum point of the line. </summary> */
	public Vector2F Max {
		get { return GMath.Max(End1, End2); }
	}
	/** <summary> Gets the minimum point of the line. </summary> */
	public Vector2F Min {
		get { return GMath.Min(End1, End2); }
	}

	/** <summary> Gets the area of the line. </summary> */
	public float Area {
		get { return 0.0f; }
	}
	/** <summary> Gets the perimeter of the line. </summary> */
	public float Perimeter {
		get { return End1.DistanceTo(End2); }
	}

	/** <summary> Gets the number of points in the line. </summary> */
	public int NumPoints {
		get { return 2; }
	}
	/** <summary> Gets the number of lines in the line. </summary> */
	public int NumLines {
		get { return 1; }
	}
	/** <summary> Gets the list of points in the line. </summary> */
	public Vector2F[] Points {
		get { return new Vector2F[] { End1, End2 }; }
	}
	/** <summary> Gets the list of lines in the line. </summary> */
	public Line2F[] Lines {
		get { return new Line2F[] { this }; }
	}

	//========= CALCULATIONS =========

	/** <summary> Returns the point on the line based on its length. </summary> */
	public Vector2F PositionAt(float length, bool asRatio = false) {
		return End1 + Size * (asRatio ? length : (length / Length));
	}
	/** <summary> Returns the line with swapped endpoints. </summary> */
	public Line2F SwapEnds() {
		return new Line2F(End2, End1);
	}

	//========= INTERACTION ==========

	/** <summary> Returns true if the specified point is on the line. </summary> */
	public bool IsPointOnLine(Vector2F v) {
		double a = Height / Width;
		double b = Y1 - (a * X1);
		return GMath.Abs(v.Y - (a * v.X + b)) == 0;
	}
	/** <summary> Returns true if the specified line is parallel. </summary> */
	public bool IsParallel(Line2F l) {
		return ((Width * l.Height) - (Height * l.Width) == 0);
	}
	/** <summary> Returns true if the line is collinear with the specified point. </summary> */
	public bool IsPointCollinear(Vector2F v) {
		if (IsEmpty)
			return false;
		if (End1 != v)
			return IsParallel(new Line2F(End1, v));
		else
			return IsParallel(new Line2F(End2, v));
	}
	/** <summary> Returns true if the line is collinear with the specified line. </summary> */
	public bool IsLineCollinear(Line2F l) {
		if (IsParallel(l)) {
			if (IsEmpty || l.IsEmpty)
				return false;
			if (End1 != l.End1)
				return IsParallel(new Line2F(End1, l.End1));
			else
				return IsParallel(new Line2F(End2, l.End1));
		}
		return false;
	}
	/** <summary> Gets the point of endless intersection between the two lines. </summary> */
	public Vector2F? PointOfIntersectionEndless(Line2F l) {
		if (IsEmpty) {
			if ((l.IsEmpty && End1 == l.End1) ||
				(!l.IsEmpty && l.IsPointOnLine(End1)))
				return End1;
			else
				return null;
		}
		else if (l.IsEmpty) {
			return l.PointOfIntersectionEndless(this);
		}
		
		if (IsHorizontal) {
			if (l.IsHorizontal)
				return null;
			if (l.IsVertical)
				return new Vector2F(l.X1, this.Y1);
			
			float xi = l.X2 - ((l.Y2 - this.Y1) * (l.Width / l.Height));
			return new Vector2F(xi, this.Y1);
		}
		else if (IsVertical) {
			if (l.IsVertical)
				return null;
			if (l.IsHorizontal)
				return new Vector2F(this.X1, l.Y1);
			
			float yi = l.Y2 - ((l.X2 - this.X1) * (l.Height / l.Width));
			return new Vector2F(this.X1, yi);
		}
		else if (l.IsHorizontal || l.IsVertical) {
			return l.PointOfIntersectionEndless(this);
		}
		
		float det = (this.Width * l.Height) - (this.Height * l.Width);
		
		if (det == 0)
			return null;
		
		float d1 = ((this.X1 * this.Y2) - (this.Y1 * this.X2));
		float d2 = ((l.X1 * l.Y2) - (l.Y1 * l.X2));
		
		return new Vector2F(-((d1 *  l.Width) - (d2 *  this.Width)) / det,
							-((d1 * l.Height) - (d2 * this.Height)) / det);
	}
	/** <summary> Gets the point of intersection between the two lines. </summary> */
	public Vector2F? PointOfIntersection(Line2F l) {
		Vector2F? point = PointOfIntersectionEndless(l);
		
		float epsilon = 0.0f;

		if (point != null) {
			Vector2F epsilonVector = new Vector2F(epsilon, epsilon);
			if (point.Value + epsilonVector < Min || point.Value + epsilonVector < l.Min)
				return null;
			if (point.Value - epsilonVector > Max || point.Value - epsilonVector > l.Max)
				return null;
		}
		return point;
	}

	//=========== CONTAINS ===========

	/** <summary> Returns true if the specified vector is inside this line. </summary> */
	public bool Contains(Vector2F point) {
		return false;
	}
	/** <summary> Returns true if the specified line is inside this line. </summary> */
	public bool Contains(Line2F line) {
		return false;
	}
	/** <summary> Returns true if the specified rectangle is inside this line. </summary> */
	public bool Contains(Rectangle2F rect) {
		return false;
	}

	//========== COLLISION ===========

	/** <summary> Returns true if the specified vector is colliding with this line. </summary> */
	public bool Colliding(Vector2F point) {
		return IsPointOnLine(point);
	}
	/** <summary> Returns true if the specified line is colliding with this line. </summary> */
	public bool Colliding(Line2F line) {
		if (IsEmpty || line.IsEmpty)
			return false;
		if (!Bounds.Colliding(line.Bounds) && !IsStraight && !line.IsStraight)
			return false;
		if (PointOfIntersection(line) != null)
			return true;
		return IsLineCollinear(line);
	}
	/** <summary> Returns true if the specified rectangle is colliding with this line. </summary> */
	public bool Colliding(Rectangle2F rect) {
		if (IsEmpty || rect.IsEmpty)
			return false;
		if (!Bounds.Colliding(rect) && !IsStraight)
			return false;
		if (rect.Contains(End1))
			return true;
		foreach (Line2F l in rect.Lines) {
			if (Colliding(l))
				return true;
		}
		return false;
	}

}
} // End namespace
