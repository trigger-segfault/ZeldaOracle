using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;

namespace ZeldaOracle.Common.Geometry {
/** <summary>
 * The double precision range between a min and a max.
 * </summary> */
public struct RangeF {

	//========== CONSTANTS ===========
	#region Constants

	/** <summary> Returns a range positioned at (0 - 0). </summary> */
	public static RangeF Zero {
		get { return new RangeF(); }
	}
	/** <summary> Returns a range positioned at (Int.Min - Int.Max). </summary> */
	public static RangeF Full {
		get { return new RangeF(Double.NegativeInfinity, Double.PositiveInfinity); }
	}

	#endregion
	//=========== MEMBERS ============
	#region Members
	
	/** <summary> The minimum value in the range. </summary> */
	public double Min;
	/** <summary> The maximum value in the range. </summary> */
	public double Max;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs a range between the 2 values. </summary> */
	public RangeF(double min, double max) {
		this.Min	= min;
		this.Max	= max;
	}
	/** <summary> Constructs a range with a single value. </summary> */
	public RangeF(double single) {
		this.Min	= single;
		this.Max	= single;
	}
	/** <summary> Constructs a copy of the specified range. </summary> */
	public RangeF(RangeF r) {
		this.Min	= r.Min;
		this.Max	= r.Max;
	}

	#endregion
	//=========== GENERAL ============
	#region General

	/** <summary> Outputs a string representing this range as (min - max). </summary> */
	public override string ToString() {
		return "(" + Min + ", " + Max + ")";
	}
	/** <summary> Outputs a string representing this range as (min - max). </summary> */
	public string ToString(IFormatProvider provider) {
		// TODO: Write formatting for RangeD.ToString(format).

		return "(" + Min.ToString(provider) + ", " + Max.ToString(provider) + ")";
	}
	/** <summary> Outputs a string representing this range as (min - max). </summary> */
	public string ToString(string format, IFormatProvider provider) {
		return "(" + Min.ToString(format, provider) + ", " + Max.ToString(format, provider) + ")";
	}
	/** <summary> Outputs a string representing this range as (min - max). </summary> */
	public string ToString(string format) {
		return "(" + Min.ToString(format) + ", " + Max.ToString(format) + ")";
	}
	/** <summary> Returns true if the specified range has the same min and max values. </summary> */
	public override bool Equals(object obj) {
		if (obj is RangeF)
			return (Min == ((RangeF)obj).Min && Max == ((RangeF)obj).Max);
		return false;
	}
	/** <summary> Returns the hash code for this range. </summary> */
	public override int GetHashCode() {
		return base.GetHashCode();
	}

	#endregion
	//========== OPERATORS ===========
	#region Operators
	//--------------------------------
	#region Unary Arithmetic

	
	#endregion
	//--------------------------------
	#region Binary Arithmetic


	#endregion
	//--------------------------------
	#region Binary Logic

	public static bool operator ==(RangeF r1, RangeF r2) {
		return (r1.Min == r2.Min && r1.Max == r2.Max);
	}
	public static bool operator ==(double d1, RangeF r2) {
		return (d1 == r2.Min && d1 == r2.Max);
	}
	public static bool operator ==(RangeF r1, double d2) {
		return (r1.Min == d2 && r1.Max == d2);
	}

	public static bool operator !=(RangeF r1, RangeF r2) {
		return (r1.Min != r2.Min || r1.Max != r2.Max);
	}
	public static bool operator !=(double d1, RangeF r2) {
		return (d1 != r2.Min || d1 != r2.Max);
	}
	public static bool operator !=(RangeF r1, double d2) {
		return (r1.Min != d2 || r1.Max != d2);
	}

	#endregion
	//--------------------------------
	#region Conversion


	#endregion
	//--------------------------------
	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> Gets the range between the min and max values. </summary> */
	[ContentSerializerIgnore]
	public double Range {
		get { return Max - Min; }
	}
	/** <summary> Gets or sets the min or max coordinate from the index. </summary> */
	[ContentSerializerIgnore]
	public double this[int index] {
		get {
			if (index < 0 || index > 1)
				throw new System.IndexOutOfRangeException("RangeD[index] must be either 0 or 1.");
			else
				return (index == 0 ? Min : Max);
		}
		set {
			if (index < 0 || index > 1)
				throw new System.IndexOutOfRangeException("RangeD[index] must be either 0 or 1.");
			else if (index == 0)
				Min = value;
			else
				Max = value;
		}
	}
	/** <summary> Returns true if the range has the values of (0 - 0). </summary> */
	public bool IsZero {
		get { return (Min == 0.0 && Max == 0.0); }
	}
	/** <summary> Returns true if the min and max values are the same. </summary> */
	public bool IsSingle {
		get { return (Min == Max); }
	}

	#endregion
	//=========== CONTAINS ===========
	#region Contains

	/** <summary> Returns true if the specified value is inside this range. </summary> */
	public bool Contains(double value) {
		return ((value >= Min) &&
				(value <=  Max));
	}

	#endregion
}
} // End namespace
