using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>The floating precision range between a min and a max.</summary>
	public struct RangeF {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>A range positioned at (0 to 0).</summary>
		public static readonly RangeF Zero = new RangeF(0f, 0f);
		/// <summary>Returns a range positioned at (float.Min to float.Max).</summary>
		public static readonly RangeF Full = new RangeF(float.NegativeInfinity, float.PositiveInfinity);
		/// <summary>Returns a range positioned at (0 to float.Max).</summary>
		public static readonly RangeF Positive = new RangeF(0f, float.PositiveInfinity);
		/// <summary>Returns a range positioned at (float.Min to 0).</summary>
		public static readonly RangeF Negative = new RangeF(float.NegativeInfinity, 0f);


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The minimum value in the range.</summary>
		public float Min;
		/// <summary>The maximum value in the range.</summary>
		public float Max;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a range between the 2 values.</summary>
		public RangeF(float min, float max) {
			this.Min	= min;
			this.Max	= max;
		}

		/// <summary>Constructs a range with a single value.</summary>
		public RangeF(float single) {
			this.Min	= single;
			this.Max	= single;
		}

		/// <summary>Constructs a copy of the specified range.</summary>
		public RangeF(RangeF r) {
			this.Min	= r.Min;
			this.Max	= r.Max;
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Outputs a string representing this range as (min - max).</summary>
		public override string ToString() {
			return "(" + Min + ", " + Max + ")";
		}

		/// <summary>Outputs a string representing this range as (min - max).</summary>
		public string ToString(IFormatProvider provider) {
			return "(" + Min.ToString(provider) + ", " + Max.ToString(provider) + ")";
		}

		/// <summary>Outputs a string representing this range as (min - max).</summary>
		public string ToString(string format, IFormatProvider provider) {
			return "(" + Min.ToString(format, provider) + ", " + Max.ToString(format, provider) + ")";
		}

		/// <summary>Outputs a string representing this range as (min - max).</summary>
		public string ToString(string format) {
			return "(" + Min.ToString(format) + ", " + Max.ToString(format) + ")";
		}

		/// <summary>Returns true if the specified range has the same min and max values.</summary>
		public override bool Equals(object obj) {
			if (obj is RangeF)
				return (Min == ((RangeF)obj).Min && Max == ((RangeF)obj).Max);
			return false;
		}

		/// <summary>Returns the hash code for this range.</summary>
		public override int GetHashCode() {
			return base.GetHashCode();
		}

		
		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public bool Intersects(RangeF other) {
			return (Max > other.Min && Min < other.Max);
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
		// Implicit Conversions
		//-----------------------------------------------------------------------------

		/// <summary>Convert from a RangeI to a RangeF.</summary>
		public static implicit operator RangeF(RangeI r) {
			return new RangeF(r.Min, r.Max);
		}
		

		//-----------------------------------------------------------------------------
		// Explicit Conversions
		//-----------------------------------------------------------------------------

		/// <summary>Convert from a RangeF to a RangeI.</summary>
		public static explicit operator RangeI(RangeF r) {
			return new RangeI((int)r.Min, (int)r.Max);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the range between the min and max values.</summary>
		[ContentSerializerIgnore]
		public float Range {
			get { return Max - Min; }
		}

		/// <summary>Gets or sets the min or max coordinate from the index.</summary>
		[ContentSerializerIgnore]
		public float this[int index] {
			get {
				if (index == 0)
					return Min;
				else if (index == 1)
					return Max;
				else
					throw new IndexOutOfRangeException("RangeF[index] must be either 0 or 1.");
			}
			set {
				if (index == 0)
					Min = value;
				else if (index == 1)
					Max = value;
				else
					throw new IndexOutOfRangeException("RangeF[index] must be either 0 or 1.");
			}
		}

		/// <summary>Returns true if the range has the values of (0 - 0).</summary>
		public bool IsZero {
			get { return (Min == 0f && Max == 0f); }
		}

		/// <summary>Returns true if the min and max values are the same.</summary>
		public bool IsSingle {
			get { return (Min == Max); }
		}

		/// <summary>Gets the middle of the range.</summary>
		public float Mean {
			get { return (Max + Min) * 0.5f; }
		}


		//-----------------------------------------------------------------------------
		// Contains
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the specified value is inside this range.</summary>
		public bool Contains(float value) {
			return ((value >= Min) && (value <=  Max));
		}

	}

}
