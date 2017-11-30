using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using GdiPoint		= System.Drawing.Point;
using GdiPointF		= System.Drawing.PointF;
using GdiSize		= System.Drawing.Size;
using GdiSizeF		= System.Drawing.SizeF;
using GdiRectangle	= System.Drawing.Rectangle;
using GdiRectangleF	= System.Drawing.RectangleF;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>The 2D floating precision vector with numerous operations and functions.</summary>
	public struct Vector2F {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>Returns a vector positioned at (0, 0).</summary>
		public static readonly Vector2F Zero = new Vector2F(0f, 0f);
		/// <summary>Returns a vector positioned at (1, 1).</summary>
		public static readonly Vector2F One = new Vector2F(1f, 1f);
		

		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The x coordinate of the vector.</summary>
		public float X;
		/// <summary>The y coordinate of the vector.</summary>
		public float Y;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a vector positioned at the specified coordinates.</summary>
		public Vector2F(float x, float y) {
			this.X	= x;
			this.Y	= y;
		}

		/// <summary>Constructs a vector positioned at the specified polar coordinates.</summary>
		public Vector2F(float length, float direction, bool asPolar) {
			if (!asPolar) {
				this.X	= length;
				this.Y	= direction;
			}
			else {
				this.X	= length * GMath.Cos(direction);
				this.Y	= length * GMath.Sin(direction);
			}
		}

		/// <summary>Constructs a vector positioned at the specified coordinates.</summary>
		public Vector2F(float uniform) {
			this.X	= uniform;
			this.Y	= uniform;
		}

		/// <summary>Constructs a copy of the specified vector.</summary>
		public Vector2F(Vector2F v) {
			this.X	= v.X;
			this.Y	= v.Y;
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Outputs a string representing this vector as (x, y).</summary>
		public override string ToString() {
			return "(" + X + ", " + Y + ")";
		}

		/// <summary>Outputs a string representing this vector as (x, y).</summary>
		public string ToString(IFormatProvider provider) {
			return "(" + X.ToString(provider) + ", " + Y.ToString(provider) + ")";
		}

		/// <summary>Outputs a string representing this vector as (x, y).</summary>
		public string ToString(string format, IFormatProvider provider) {
			return "(" + X.ToString(format, provider) + ", " + Y.ToString(format, provider) + ")";
		}

		/// <summary>Outputs a string representing this vector as (x, y).</summary>
		public string ToString(string format) {
			return "(" + X.ToString(format) + ", " + Y.ToString(format) + ")";
		}

		/// <summary>Returns true if the specified vector has the same x and y coordinates.</summary>
		public override bool Equals(object obj) {
			if (obj is Vector2F)
				return (X == ((Vector2F)obj).X && Y == ((Vector2F)obj).Y);
			else if (obj is Vector2)
				return (X == ((Vector2)obj).X && Y == ((Vector2)obj).Y);
			else if (obj is Point2I)
				return (X == ((Point2I)obj).X && Y == ((Point2I)obj).Y);
			else if (obj is Point)
				return (X == ((Point)obj).X && Y == ((Point)obj).Y);
			return false;
		}

		/// <summary>Returns the hash code of this vector.</summary>
		public override int GetHashCode() {
			return base.GetHashCode();
		}

		/// <summary>Parse a string containing a vector.</summary>
		public static Vector2F Parse(string s) {
			Vector2F value = Vector2F.Zero;

			if (s.Length > 0) {
				if (s[0] == '(' && s[s.Length - 1] == ')')
					s = s.Substring(1, s.Length - 2);

				int commaPos = s.IndexOf(',');
				if (commaPos == -1)
					commaPos = s.IndexOf(' ');
				if (commaPos != -1) {

					string strX = s.Substring(0, commaPos).Trim();
					string strY = s.Substring(commaPos + 1).Trim();

					try {
						value.X = float.Parse(strX);
						value.Y = float.Parse(strY);
					}
					catch (FormatException e) {
						throw e;
					}
					catch (ArgumentNullException e) {
						throw e;
					}
					catch (OverflowException e) {
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

		/// <summary>Tries to parse the vector and returns whether the parsing was
		/// successful or not.</summary>
		public static bool TryParse(string s, out Vector2F result) {
			try {
				result = Parse(s);
				return true;
			}
			catch {
				result = Vector2F.Zero;
				return false;
			}
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static Vector2F operator +(Vector2F v) {
			return v;
		}

		public static Vector2F operator -(Vector2F v) {
			return new Vector2F(-v.X, -v.Y);
		}

		public static Vector2F operator ++(Vector2F v) {
			return new Vector2F(++v.X, ++v.Y);
		}

		public static Vector2F operator --(Vector2F v) {
			return new Vector2F(--v.X, --v.Y);
		}

		//--------------------------------

		public static Vector2F operator +(Vector2F v1, Vector2F v2) {
			return new Vector2F(v1.X + v2.X, v1.Y + v2.Y);
		}

		public static Vector2F operator +(Vector2F v1, float f2) {
			return new Vector2F(v1.X + f2, v1.Y + f2);
		}

		public static Vector2F operator +(float f1, Vector2F v2) {
			return new Vector2F(f1 + v2.X, f1 + v2.Y);
		}

		public static Vector2F operator -(Vector2F v1, Vector2F v2) {
			return new Vector2F(v1.X - v2.X, v1.Y - v2.Y);
		}

		public static Vector2F operator -(Vector2F v1, float f2) {
			return new Vector2F(v1.X - f2, v1.Y - f2);
		}

		public static Vector2F operator -(float f1, Vector2F v2) {
			return new Vector2F(f1 - v2.X, f1 - v2.Y);
		}

		public static Vector2F operator *(Vector2F v1, Vector2F v2) {
			return new Vector2F(v1.X * v2.X, v1.Y * v2.Y);
		}

		public static Vector2F operator *(float f1, Vector2F v2) {
			return new Vector2F(f1 * v2.X, f1 * v2.Y);
		}

		public static Vector2F operator *(Vector2F v1, float f2) {
			return new Vector2F(v1.X * f2, v1.Y * f2);
		}

		public static Vector2F operator /(Vector2F v1, Vector2F v2) {
			return new Vector2F(v1.X / v2.X, v1.Y / v2.Y);
		}

		public static Vector2F operator /(float f1, Vector2F v2) {
			return new Vector2F(f1 / v2.X, f1 / v2.Y);
		}

		public static Vector2F operator /(Vector2F v1, float f2) {
			return new Vector2F(v1.X / f2, v1.Y / f2);
		}

		public static Vector2F operator %(Vector2F v1, Vector2F v2) {
			return new Vector2F(v1.X % v2.X, v1.Y % v2.Y);
		}

		public static Vector2F operator %(float f1, Vector2F v2) {
			return new Vector2F(f1 % v2.X, f1 % v2.Y);
		}

		public static Vector2F operator %(Vector2F v1, float f2) {
			return new Vector2F(v1.X % f2, v1.Y % f2);
		}

		//--------------------------------

		public static bool operator ==(Vector2F v1, Vector2F v2) {
			return (v1.X == v2.X && v1.Y == v2.Y);
		}

		public static bool operator ==(float f1, Vector2F v2) {
			return (f1 == v2.X && f1 == v2.Y);
		}

		public static bool operator ==(Vector2F v1, float f2) {
			return (v1.X == f2 && v1.Y == f2);
		}

		public static bool operator !=(Vector2F v1, Vector2F v2) {
			return (v1.X != v2.X || v1.Y != v2.Y);
		}

		public static bool operator !=(float f1, Vector2F v2) {
			return (f1 != v2.X || f1 != v2.Y);
		}

		public static bool operator !=(Vector2F v1, float f2) {
			return (v1.X != f2 || v1.Y != f2);
		}

		public static bool operator <(Vector2F v1, Vector2F v2) {
			return (v1.X < v2.X && v1.Y < v2.Y);
		}

		public static bool operator <(float f1, Vector2F v2) {
			return (f1 < v2.X && f1 < v2.Y);
		}

		public static bool operator <(Vector2F v1, float f2) {
			return (v1.X < f2 && v1.Y < f2);
		}

		public static bool operator >(Vector2F v1, Vector2F v2) {
			return (v1.X > v2.X && v1.Y > v2.Y);
		}

		public static bool operator >(float f1, Vector2F v2) {
			return (f1 > v2.X && f1 > v2.Y);
		}

		public static bool operator >(Vector2F v1, float f2) {
			return (v1.X > f2 && v1.Y > f2);
		}

		public static bool operator <=(Vector2F v1, Vector2F v2) {
			return (v1.X <= v2.X && v1.Y <= v2.Y);
		}

		public static bool operator <=(float f1, Vector2F v2) {
			return (f1 <= v2.X && f1 <= v2.Y);
		}

		public static bool operator <=(Vector2F v1, float f2) {
			return (v1.X <= f2 && v1.Y <= f2);
		}

		public static bool operator >=(Vector2F v1, Vector2F v2) {
			return (v1.X >= v2.X && v1.Y >= v2.Y);
		}

		public static bool operator >=(float f1, Vector2F v2) {
			return (f1 >= v2.X && f1 >= v2.Y);
		}

		public static bool operator >=(Vector2F v1, float f2) {
			return (v1.X >= f2 && v1.Y >= f2);
		}


		//-----------------------------------------------------------------------------
		// Implicit Conversions
		//-----------------------------------------------------------------------------
		
		/// <summary>Convert from a Point2I to a Vector2F.</summary>
		public static implicit operator Vector2F(Point2I p) {
			return new Vector2F(p.X, p.Y);
		}

		/// <summary>Convert from a Microsfot.Xna.Framework.Vector2 to a Vector2F.</summary>
		public static implicit operator Vector2F(Vector2 v) {
			return new Vector2F(v.X, v.Y);
		}

		/// <summary>Convert from a Microsfot.Xna.Framework.Point to a Vector2F.</summary>
		public static implicit operator Vector2F(Point p) {
			return new Vector2F(p.X, p.Y);
		}

		/// <summary>Convert from a System.Drawing.Point to a Vector2F.</summary>
		public static implicit operator Vector2F(GdiPoint p) {
			return new Vector2F(p.X, p.Y);
		}

		/// <summary>Convert from a System.Drawing.Size to a Vector2F.</summary>
		public static implicit operator Vector2F(GdiSize s) {
			return new Vector2F(s.Width, s.Height);
		}

		/// <summary>Convert from a System.Drawing.PointF to a Vector2F.</summary>
		public static implicit operator Vector2F(GdiPointF p) {
			return new Vector2F(p.X, p.Y);
		}

		/// <summary>Convert from a System.Drawing.SizeF to a Vector2F.</summary>
		public static implicit operator Vector2F(GdiSizeF s) {
			return new Vector2F(s.Width, s.Height);
		}

		/// <summary>Convert from a Vector2F to a System.Drawing.PointF.</summary>
		public static implicit operator GdiPointF(Vector2F v) {
			return new GdiPointF(v.X, v.Y);
		}

		/// <summary>Convert from a Vector2F to a System.Drawing.SizeF.</summary>
		public static implicit operator GdiSizeF(Vector2F v) {
			return new GdiSizeF(v.X, v.Y);
		}


		//-----------------------------------------------------------------------------
		// Explicit Conversions
		//-----------------------------------------------------------------------------

		/// <summary>Convert from a Vector2F to a Point2I.</summary>
		public static explicit operator Point2I(Vector2F v) {
			return new Point2I((int)v.X, (int)v.Y);
		}

		/// <summary>Convert from a Vector2F to a Microsfot.Xna.Framework.Vector2.</summary>
		public static explicit operator Vector2(Vector2F v) {
			return new Vector2(v.X, v.Y);
		}

		/// <summary>Convert from a Vector2F to a Microsfot.Xna.Framework.Point.</summary>
		public static explicit operator Point(Vector2F v) {
			return new Point((int)v.X, (int)v.Y);
		}

		/// <summary>Convert from a Vector2F to a System.Drawing.Point.</summary>
		public static explicit operator GdiPoint(Vector2F v) {
			return new GdiPoint((int)v.X, (int)v.Y);
		}

		/// <summary>Convert from a Vector2F to a System.Drawing.Size.</summary>
		public static explicit operator GdiSize(Vector2F v) {
			return new GdiSize((int)v.X, (int)v.Y);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the direction of the vector.</summary>
		[ContentSerializerIgnore]
		public float Direction {
			get {
				if (IsZero)
					return 0f;
				return GMath.Plusdir(GMath.Atan2(Y, X));
			}
			set {
				float length = Length;
				X = length * GMath.Cos(value);
				Y = length * GMath.Sin(value);
			}
		}

		/// <summary>Gets or sets the length of the vector.</summary>
		[ContentSerializerIgnore]
		public float Length {
			get { return GMath.Sqrt((X * X) + (Y * Y)); }
			set {
				if (!IsZero) {
					float oldLength = Length;
					X *= value / oldLength;
					Y *= value / oldLength;
				}
				else {
					X = value;
					Y = 0f;
				}
			}
		}

		/// <summary>Gets the squared length of the vector.</summary>
		[ContentSerializerIgnore]
		public float LengthSquared {
			get { return ((X * X) + (Y * Y)); }
		}

		/// <summary>Gets or sets the x or y coordinate from the index.</summary>
		[ContentSerializerIgnore]
		public float this[int coordinate] {
			get {
				if (coordinate == 0)
					return X;
				else if (coordinate == 1)
					return Y;
				else
					throw new IndexOutOfRangeException("Vector2F[coordinateIndex] must be either 0 or 1.");
			}
			set {
				if (coordinate == 0)
					X = value;
				else if (coordinate == 1)
					Y = value;
				else
					throw new IndexOutOfRangeException("Vector2F[coordinateIndex] must be either 0 or 1.");
			}
		}

		/// <summary>Returns true if the vector is positioned at (0, 0).</summary>
		[ContentSerializerIgnore]
		public bool IsZero {
			get { return (X == 0f && Y == 0f); }
		}

		/// <summary>Returns the inverse of the vector.</summary>
		[ContentSerializerIgnore]
		public Vector2F Inverse {
			get { return new Vector2F((X == 0f ? 0f : 1f / X), (Y == 0f ? 0f : 1f / Y)); }
		}

		/// <summary>Returns the normalized vector.</summary>
		[ContentSerializerIgnore]
		public Vector2F Normalized {
			get {
				float length = GMath.Sqrt((X * X) + (Y * Y));
				if (length > 0)
					return new Vector2F(X / length, Y / length);
				return Vector2F.Zero;
			}
		}

		/// <summary>Returns the perpendicular vector.</summary>
		[ContentSerializerIgnore]
		public Vector2F Perpendicular {
			get { return new Vector2F(-Y, X); }
		}


		//-----------------------------------------------------------------------------
		// Calculations
		//-----------------------------------------------------------------------------

		/// <summary>Returns the dot product of this vector with another.</summary>
		public float Dot(float x, float y) {
			return ((X * x) + (Y * y));
		}

		/// <summary>Returns the dot product of this vector with another.</summary>
		public float Dot(Vector2F v) {
			return ((X * v.X) + (Y * v.Y));
		}

		/// <summary>Returns the distance between this vector and another.</summary>
		public float DistanceTo(float x, float y) {
			return (new Vector2F(x, y) - this).Length;
		}

		/// <summary>Returns the distance between this vector to another.</summary>
		public float DistanceTo(Vector2F v) {
			return (v - this).Length;
		}

		/// <summary>Returns the direction from this vector to another.</summary>
		public float DirectionTo(float x, float y) {
			return GMath.Plusdir((new Vector2F(x, y) - this).Direction);
		}

		/// <summary>Returns the direction from this vector to another.</summary>
		public float DirectionTo(Vector2F v) {
			return GMath.Plusdir((v - this).Direction);
		}

		/// <summary>Returns the angle between this vector and another.</summary>
		public float AngleBetween(float x, float y) {
			return GMath.Plusdir3(new Vector2F(x, y).Direction - Direction);
		}

		/// <summary>Returns the angle between this vector and another.</summary>
		public float AngleBetween(Vector2F v) {
			return GMath.Plusdir3(v.Direction - Direction);
		}

		/// <summary>Returns the shortest distance from this point to the specified line.</summary>
		public float DistanceToLine(Line2F l) {
			return DistanceTo(ClosestPointOnLine(l));
		}

		/// <summary>Returns the closest point on the specified line.</summary>
		public Vector2F ClosestPointOnLine(Line2F l) {
			if (l.IsEmpty)
				return l.End1;

			float dot2 = l.Size.Dot(l.Size);
			float dotp = l.Size.Dot(this - l.End1);
			float t = GMath.Clamp(dotp / dot2, 0f, 1f);

			return l.End1 + l.Size * t;
		}

		/// <summary>Returns the scalar projection on this vector and angle.</summary>
		public float ScalarProjection(float angle) {
			return (Length * GMath.Cos(Direction - angle));
		}

		/// <summary>Returns the scalar projection on this vector and another.</summary>
		public float ScalarProjection(float x, float y) {
			return (Length * GMath.Cos(Direction - new Vector2F(x, y).Direction));
		}

		/// <summary>Returns the scalar projection on this vector and another.</summary>
		public float ScalarProjection(Vector2F v) {
			return (Length * GMath.Cos(Direction - v.Direction));
		}

		/// <summary>Returns the projection of this vector on an angle.</summary>
		public Vector2F ProjectionOn(float angle) {
			return new Vector2F(ScalarProjection(angle), angle);
		}

		/// <summary>Returns the projection of this vector on another.</summary>
		public Vector2F ProjectionOn(float x, float y) {
			return new Vector2F(ScalarProjection(x, y), new Vector2F(x, y).Direction);
		}

		/// <summary>Returns the projection of this vector on another.</summary>
		public Vector2F ProjectionOn(Vector2F v) {
			return new Vector2F(ScalarProjection(v), v.Direction);
		}

		/// <summary>Returns the rejection of this vector on an angle.</summary>
		public Vector2F RejectionOn(float angle) {
			return (this - ProjectionOn(angle));
		}

		/// <summary>Returns the rejection of this vector on another.</summary>
		public Vector2F RejectionOn(float x, float y) {
			return (this - ProjectionOn(x, y));
		}

		/// <summary>Returns the rejection of this vector on another.</summary>
		public Vector2F RejectionOn(Vector2F v) {
			return (this - ProjectionOn(v));
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------
		
		public static float Distance(Vector2F a, Vector2F b) {
			return (b - a).Length;
		}

		public static Vector2F Lerp(Vector2F value1, Vector2F value2, float amount) {
			return new Vector2F(
				value1.X + ((value2.X - value1.X) * amount),
				value1.Y + ((value2.Y - value1.Y) * amount));
		}

		public static Vector2F CreatePolar(float length, float direction) {
			return new Vector2F(length, direction, true);
		}

		public static Vector2F SnapDirection(Vector2F v, float angleSnapInterval) {
			float theta = v.Direction;
			if (theta < 0f)
				theta += GMath.FullAngle;
			int angleIndex = (int) ((theta / angleSnapInterval) + 0.5f); // Round to nearest interval.
			float length = v.Length;
			return Vector2F.CreatePolar(length, angleIndex * angleSnapInterval);
		}

		public static Vector2F SnapDirectionByCount(Vector2F v, int intervalCount) {
			return SnapDirection(v, GMath.FullAngle / intervalCount);
		}
		
	}
}
