using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;

namespace ZeldaOracle.Common.Geometry {
	/** <summary>
	 * The floating precision range between a min and a max.
	 * </summary> */
	public struct RangeF {
	
		// The minimum value in the range.
		public float Min;
		// The maximum value in the range.
		public float Max;

		
		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------
		
		// A range positioned at (0 to 0).
		public static readonly RangeF Zero = new RangeF(0.0f, 0.0f);

		// Returns a range positioned at (Float.Min - Float.Max).
		public static readonly RangeF Full = new RangeF(Single.NegativeInfinity, Single.PositiveInfinity);


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		// Constructs a range between the 2 values.
		public RangeF(float min, float max) {
			this.Min	= min;
			this.Max	= max;
		}
		// Constructs a range with a single value.
		public RangeF(float single) {
			this.Min	= single;
			this.Max	= single;
		}
		// Constructs a copy of the specified range.
		public RangeF(RangeF r) {
			this.Min	= r.Min;
			this.Max	= r.Max;
		}
		

		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		// Outputs a string representing this range as (min - max).
		public override string ToString() {
			return "(" + Min + ", " + Max + ")";
		}

		// Outputs a string representing this range as (min - max).
		public string ToString(IFormatProvider provider) {
			return "(" + Min.ToString(provider) + ", " + Max.ToString(provider) + ")";
		}

		// Outputs a string representing this range as (min - max).
		public string ToString(string format, IFormatProvider provider) {
			return "(" + Min.ToString(format, provider) + ", " + Max.ToString(format, provider) + ")";
		}

		// Outputs a string representing this range as (min - max).
		public string ToString(string format) {
			return "(" + Min.ToString(format) + ", " + Max.ToString(format) + ")";
		}

		// Returns true if the specified range has the same min and max values.
		public override bool Equals(object obj) {
			if (obj is RangeF)
				return (Min == ((RangeF)obj).Min && Max == ((RangeF)obj).Max);
			return false;
		}

		// Returns the hash code for this range.
		public override int GetHashCode() {
			return base.GetHashCode();
		}

		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void Set(float min, float max) {
			this.Min = min;
			this.Max = max;
		}

		
		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static bool operator ==(RangeF r1, RangeF r2) {
			return (r1.Min == r2.Min && r1.Max == r2.Max);
		}

		public static bool operator ==(float f1, RangeF r2) {
			return (f1 == r2.Min && f1 == r2.Max);
		}

		public static bool operator ==(RangeF r1, float f2) {
			return (r1.Min == f2 && r1.Max == f2);
		}

		public static bool operator !=(RangeF r1, RangeF r2) {
			return (r1.Min != r2.Min || r1.Max != r2.Max);
		}

		public static bool operator !=(float f1, RangeF r2) {
			return (f1 != r2.Min || f1 != r2.Max);
		}

		public static bool operator !=(RangeF r1, float f2) {
			return (r1.Min != f2 || r1.Max != f2);
		}
				

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the range between the min and max values.
		[ContentSerializerIgnore]
		public float Range {
			get { return Max - Min; }
		}

		// Gets or sets the min or max coordinate from the index.
		[ContentSerializerIgnore]
		public float this[int index] {
			get {
				if (index < 0 || index > 1)
					throw new System.IndexOutOfRangeException("RangeF[index] must be either 0 or 1.");
				else
					return (index == 0 ? Min : Max);
			}
			set {
				if (index < 0 || index > 1)
					throw new System.IndexOutOfRangeException("RangeF[index] must be either 0 or 1.");
				else if (index == 0)
					Min = value;
				else
					Max = value;
			}
		}

		// Returns true if the range has the values of (0 - 0).
		public bool IsZero {
			get { return (Min == 0.0f && Max == 0.0f); }
		}

		// Returns true if the min and max values are the same.
		public bool IsSingle {
			get { return (Min == Max); }
		}

		
		//-----------------------------------------------------------------------------
		// Contains
		//-----------------------------------------------------------------------------

		// Returns true if the specified value is inside this range.
		public bool Contains(float value) {
			return ((value >= Min) &&
					(value <=  Max));
		}

	}

} // End namespace
