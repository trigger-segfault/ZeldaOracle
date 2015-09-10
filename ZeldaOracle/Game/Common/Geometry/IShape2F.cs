using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace ZeldaOracle.Common.Geometry {
/** <summary>
 * The interface for all 2D double precision shapes.
 * </summary> */
public interface IShape2F {

	//========== OPERATORS ===========
	#region Operators

	/** <summary> Translates the shape by the specified distance. </summary> */
	IShape2F Translate(Vector2F v);
	/** <summary> Translates the shape by the specified distance. </summary> */
	IShape2F Translate(double d);
	/** <summary> Returns the shape translated by the specified distance. </summary> */
	//public abstract IShape2D TranslatedBy(Vector2D v);
	/** <summary> Returns the shape translated by the specified distance. </summary> */
	//public abstract IShape2D TranslatedBy(double d);

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Returns true if the shape is empty. </summary> */
	bool IsEmpty { get; }
	/** <summary> Gets the center of the shape. </summary> */
	Vector2F Center { get; }
	/** <summary> Gets the bounding box of the shape. </summary> */
	Rectangle2F Bounds { get; }
	/** <summary> Gets the maximum point of the shape. </summary> */
	Vector2F Max { get; }
	/** <summary> Gets the minimum point of the shape. </summary> */
	Vector2F Min { get; }

	/** <summary> Gets the area of the shape. </summary> */
	double Area { get; }
	/** <summary> Gets the perimeter of the shape. </summary> */
	double Perimeter { get; }

	/** <summary> Gets the number of points in the shape. </summary> */
	int NumPoints { get; }
	/** <summary> Gets the number of lines in the shape. </summary> */
	int NumLines { get; }
	/** <summary> Gets the list of points in the shape. </summary> */
	Vector2F[] Points { get; }
	/** <summary> Gets the list of lines in the shape. </summary> */
	Line2F[] Lines { get; }

	#endregion
	//========= CALCULATIONS =========
	#region Calculations

	/** <summary> Returns the point on the shape based on the its perimeter. </summary> */
	Vector2F PositionAt(double length, bool asRatio = false);

	#endregion
	//=========== CONTAINS ===========
	#region Contains

	/** <summary> Returns true if the specified vector is inside this shape. </summary> */
	bool Contains(Vector2F point);
	/** <summary> Returns true if the specified shape is inside this shape. </summary> */
	bool Contains(IShape2F shape);
	/** <summary> Returns true if the specified line is inside this shape. </summary> */
	//public abstract bool Contains(Line2D line);
	/** <summary> Returns true if the specified rectangle is inside this shape. </summary> */
	//public abstract bool Contains(Rectangle2D rect);
	/** <summary> Returns true if the specified polygon is inside this shape. </summary> */
	//public abstract bool Contains(Polygon2D poly);

	#endregion
	//========== COLLISION ===========
	#region Collision

	/** <summary> Returns true if the specified vector is colliding with this shape. </summary> */
	bool Colliding(Vector2F point);
	/** <summary> Returns true if the specified shape is colliding with this shape. </summary> */
	bool Colliding(IShape2F shape);
	/** <summary> Returns true if the specified line is colliding with this shape. </summary> */
	//public abstract bool Colliding(Line2D line);
	/** <summary> Returns true if the specified rectangle is colliding with this shape. </summary> */
	//public abstract bool Colliding(Rectangle2D rect);
	/** <summary> Returns true if the specified polygon is colliding with this shape. </summary> */
	//public abstract bool Colliding(Polygon2D poly);

	#endregion
}
} // End namespace
