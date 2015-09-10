using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace ZeldaOracle.Common.Geometry {
/** <summary>
 * The 2D double precision polygon with numerous operations and functions.
 * </summary> */
public struct Polygon2F : IShape2F {

	//========== CONSTANTS ===========
	#region Constants

	/** <summary> Returns an empty polygon. </summary> */
	public static Polygon2F Zero {
		get { return new Polygon2F(); }
	}

	#endregion
	//=========== MEMBERS ============
	#region Members

	/** <summary> The list of points in the polygon. </summary> */
	public Vector2F[] PointList;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs a polygon with the specified list of points. </summary> */
	public Polygon2F(params Vector2F[] points) {
		this.PointList	= (Vector2F[])points.Clone();
	}
	/** <summary> Constructs a polygon with the specified list of points. </summary> */
	public Polygon2F(params Point2I[] points) {
		this.PointList	= new Vector2F[points.Length];
		for (int i = 0; i < points.Length; i++) {
			this.PointList[i]	= points[i];
		}
	}
	/** <summary> Constructs a polygon with the specified list of points. </summary> */
	public Polygon2F(params int[] points) {
		this.PointList	= new Vector2F[points.Length / 2];
		for (int i = 0; i < points.Length / 2; i++) {
			this.PointList[i]	= new Vector2F(points[i * 2], points[i * 2 + 1]);
		}
	}
	/** <summary> Constructs a polygon with the specified list of points. </summary> */
	public Polygon2F(params float[] points) {
		this.PointList	= new Vector2F[points.Length / 2];
		for (int i = 0; i < points.Length / 2; i++) {
			this.PointList[i]	= new Vector2F(points[i * 2], points[i * 2 + 1]);
		}
	}
	/** <summary> Constructs a polygon with the specified list of points. </summary> */
	public Polygon2F(params double[] points) {
		this.PointList	= new Vector2F[points.Length / 2];
		for (int i = 0; i < points.Length / 2; i++) {
			this.PointList[i]	= new Vector2F(points[i * 2], points[i * 2 + 1]);
		}
	}
	/** <summary> Constructs a copy of the specified polygon. </summary> */
	public Polygon2F(Polygon2F p) {
		this.PointList	= (Vector2F[])p.PointList.Clone();
	}

	#endregion
	//=========== GENERAL ============
	#region General

	/** <summary> Outputs a string representing this rectangle. </summary> */
	public override string ToString() {
		return PointList.ToString();
	}
	/** <summary> Outputs a string representing this rectangle. </summary> */
	public string ToString(IFormatProvider provider) {
		// TODO: Write formatting for Rectangle2D.ToString(format).

		return PointList.ToString();
	}
	/** <summary> Outputs a string representing this rectangle. </summary> */
	public string ToString(string format) {
		return PointList.ToString();
	}
	/** <summary> Outputs a string representing this rectangle. </summary> */
	public string ToString(string format, IFormatProvider provider) {
		return PointList.ToString();
	}
	/** <summary> Returns true if the specified rectangle has the same x and y coordinates. </summary> */
	public override bool Equals(object obj) {
		/*if (obj is Rectangle2D)
			return (Point == ((Rectangle2D)obj).Point && Size == ((Rectangle2D)obj).Size);*/
		return false;
	}
	/** <summary> Returns the hash code for this rectangle. </summary> */
	public override int GetHashCode() {
		return base.GetHashCode();
	}

	#endregion
	//========== OPERATORS ===========
	#region Operators
	//--------------------------------
	#region Unary Arithmetic

	public static Polygon2F operator ++(Polygon2F p) {
		for (int i = 0; i < p.PointList.Length; i++)
			++p.PointList[i];
		return p;
	}
	public static Polygon2F operator --(Polygon2F p) {
		for (int i = 0; i < p.PointList.Length; i++)
			--p.PointList[i];
		return p;
	}

	#endregion
	//--------------------------------
	#region Binary Arithmetic

	public static Polygon2F operator +(Polygon2F p, Vector2F v) {
		for (int i = 0; i < p.PointList.Length; i++)
			p.PointList[i] += v;
		return p;
	}
	public static Polygon2F operator +(Polygon2F p, double d) {
		for (int i = 0; i < p.PointList.Length; i++)
			p.PointList[i] += d;
		return p;
	}
	public static Polygon2F operator -(Polygon2F p, Vector2F v) {
		for (int i = 0; i < p.PointList.Length; i++)
			p.PointList[i] -= v;
		return p;
	}
	public static Polygon2F operator -(Polygon2F p, double d) {
		for (int i = 0; i < p.PointList.Length; i++)
			p.PointList[i] -= d;
		return p;
	}

	/** <summary> Translates the polygon by the specified distance. </summary> */
	public IShape2F Translate(Vector2F v) {
		return this + v;
	}
	/** <summary> Translates the polygon by the specified distance. </summary> */
	public IShape2F Translate(double d) {
		return this + d;
	}

	#endregion
	//--------------------------------
	#region Binary Logic

	public static bool operator ==(Polygon2F p1, Polygon2F p2) {
		return false;
	}
	public static bool operator !=(Polygon2F p1, Polygon2F p2) {
		return false;
	}

	#endregion
	//--------------------------------
	#region Conversion
	
	/*public static implicit operator Rectangle2D(Rectangle2I r) {
		return new Rectangle2D(r.X, r.Y, r.Width, r.Height);
	}*/

	/*public static explicit operator Rectangle2I(Rectangle2D r) {
		return new Rectangle2I((int)r.Point.X, (int)r.Point.Y, (int)r.Size.X, (int)r.Size.Y);
	}*/

	#endregion
	//--------------------------------
	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Dimensions
	
	/** <summary> Gets or sets the point at the specified index. </summary> */
	public Vector2F this[int index] {
		get { return PointList[index]; }
		set { PointList[index] = value; }
	}

	#endregion
	//--------------------------------
	#region Shape

	/** <summary> Returns true if the rectangle has an size of zero. </summary> */
	public bool IsEmpty {
		get { return (PointList == null ? true : PointList.Length < 2); }
	}
	/** <summary> Gets the center of the rectangle. </summary> */
	public Vector2F Center {
		get { return Bounds.Center; }
	}
	/** <summary> Gets the bounding box of the rectangle. </summary> */
	public Rectangle2F Bounds {
		get {
			Vector2F max = PointList[0];
			Vector2F min = PointList[0];
			foreach (Vector2F v in PointList) {
				max = GMath.Max(v, max);
				min = GMath.Min(v, min);
			}
			return new Rectangle2F(min, max - min);
		}
	}
	/** <summary> Gets the maximum point of the rectangle. </summary> */
	public Vector2F Max {
		get {
			Vector2F max = PointList[0];
			foreach (Vector2F v in PointList) {
				max = GMath.Max(v, max);
			}
			return max;
		}
	}
	/** <summary> Gets the minimum point of the rectangle. </summary> */
	public Vector2F Min {
		get {
			Vector2F min = PointList[0];
			foreach (Vector2F v in PointList) {
				min = GMath.Min(v, min);
			}
			return Min;
		}
	}

	/** <summary> Gets the area of the rectangle. </summary> */
	public double Area {
		get { return 0.0; }
	}
	/** <summary> Gets the perimeter of the rectangle. </summary> */
	public double Perimeter {
		get {
			double perimeter = 0.0;
			for (int i = 0; i < PointList.Length; i++) {
				perimeter += PointList[i].DistanceTo(PointList[i % PointList.Length]);
			}
			return perimeter;
		}
	}

	/** <summary> Gets the number of points in the rectangle. </summary> */
	public int NumPoints {
		get { return PointList.Length; }
	}
	/** <summary> Gets the number of lines in the rectangle. </summary> */
	public int NumLines {
		get { return PointList.Length; }
	}
	/** <summary> Gets the list of points in the rectangle. </summary> */
	public Vector2F[] Points {
		get {
			return (Vector2F[])PointList.Clone();
		}
	}
	/** <summary> Gets the list of lines in the rectangle. </summary> */
	public Line2F[] Lines {
		get {
			Line2F[] lines = new Line2F[PointList.Length];
			for (int i = 0; i < PointList.Length; i++) {
				lines[i] = new Line2F(PointList[i], PointList[i % PointList.Length]);
			}
			return lines;
		}
	}

	#endregion
	//--------------------------------
	#endregion
	//========= CALCULATIONS =========
	#region Calculations

	/** <summary> Returns the point on the polygon based on the its perimeter. </summary> */
	public Vector2F PositionAt(double length, bool asRatio = false) {
		double perimeter = Perimeter;
		if (asRatio)
			length *= perimeter;
		if (length < 0 || length > perimeter)
			return PointList[0];
		foreach (Line2F l in Lines) {
			if (length <= l.Length)
				return l.PositionAt(length);
			length -= l.Length;
		}
		return PointList[0];
	}

	#endregion
	//=========== CONTAINS ===========
	#region Contains

	/** <summary> Returns true if the specified vector is inside this polygon. </summary> */
	public bool Contains(Vector2F point) {
		if (IsEmpty)
			return false;
		if (!Bounds.Contains(point))
			return false;
		Line2F checker = new Line2F(Min.X - 1.0, point.Y, point);
		bool inside = false;
		foreach (Line2F l in Lines) {
			if (l.Colliding(checker))
				inside = !inside;
		}
		return inside;
	}
	/** <summary> Returns true if the specified shape is inside this polygon. </summary> */
	public bool Contains(IShape2F shape) {
		if (shape is Line2F)
			return Contains((Line2F)shape);
		if (shape is Rectangle2F)
			return Contains((Rectangle2F)shape);
		if (shape is Polygon2F)
			return Contains((Polygon2F)shape);
		return false;
	}
	/** <summary> Returns true if the specified line is inside this polygon. </summary> */
	public bool Contains(Line2F line) {
		if (IsEmpty || line.IsEmpty)
			return false;
		if (!Bounds.Contains(line.Bounds))
			return false;
		if (!Contains(line.End1))
			return false;
		foreach (Line2F l in Lines) {
			if (l.Colliding(line))
				return false;
		}
		return true;
	}
	/** <summary> Returns true if the specified rectangle is inside this polygon. </summary> */
	public bool Contains(Rectangle2F rect) {
		if (IsEmpty || rect.IsEmpty)
			return false;
		if (!Bounds.Contains(rect))
			return false;
		if (!Contains(rect.Point))
			return false;
		foreach (Line2F l1 in Lines) {
			foreach (Line2F l2 in rect.Lines) {
				if (l1.Colliding(l2))
					return false;
			}
		}
		return true;
	}
	/** <summary> Returns true if the specified polygon is inside this polygon. </summary> */
	public bool Contains(Polygon2F poly) {
		if (IsEmpty || poly.IsEmpty)
			return false;
		if (!Bounds.Contains(poly.Bounds))
			return false;
		if (!Contains(poly.PointList[0]))
			return false;
		foreach (Line2F l1 in Lines) {
			foreach (Line2F l2 in poly.Lines) {
				if (l1.Colliding(l2))
					return false;
			}
		}
		return true;
	}

	#endregion
	//========== COLLISION ===========
	#region Collision

	/** <summary> Returns true if the specified vector is colliding with this polygon. </summary> */
	public bool Colliding(Vector2F point) {
		return Contains(point);
	}
	/** <summary> Returns true if the specified shape is colliding with this polygon. </summary> */
	public bool Colliding(IShape2F shape) {
		if (shape is Line2F)
			return Colliding((Line2F)shape);
		if (shape is Rectangle2F)
			return Colliding((Rectangle2F)shape);
		if (shape is Polygon2F)
			return Colliding((Polygon2F)shape);
		return false;
	}
	/** <summary> Returns true if the specified line is colliding with this polygon. </summary> */
	public bool Colliding(Line2F line) {
		if (IsEmpty || line.IsEmpty)
			return false;
		if (!Bounds.Colliding(line.Bounds))
			return false;
		if (Contains(line.End1))
			return true;
		foreach (Line2F l in Lines) {
			if (l.Colliding(line))
				return true;
		}
		return false;
	}
	/** <summary> Returns true if the specified rectangle is colliding with this polygon. </summary> */
	public bool Colliding(Rectangle2F rect) {
		if (IsEmpty || rect.IsEmpty)
			return false;
		if (!Bounds.Colliding(rect))
			return false;
		if (Contains(rect.Point))
			return true;
		if (rect.Contains(PointList[0]))
			return true;
		foreach (Line2F l1 in Lines) {
			foreach (Line2F l2 in rect.Lines) {
				if (l1.Colliding(l2))
					return true;
			}
		}
		return false;
	}
	/** <summary> Returns true if the specified polygon is colliding with this polygon. </summary> */
	public bool Colliding(Polygon2F poly) {
		if (IsEmpty || poly.IsEmpty)
			return false;
		if (!Bounds.Colliding(poly.Bounds))
			return false;
		if (Contains(poly.PointList[0]))
			return true;
		if (poly.Contains(PointList[0]))
			return true;
		foreach (Line2F l1 in Lines) {
			foreach (Line2F l2 in poly.Lines) {
				if (l1.Colliding(l2))
					return true;
			}
		}
		return false;
	}

	#endregion
}
} // End namespace
