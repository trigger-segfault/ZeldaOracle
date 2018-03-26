using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Util {
	/// <summary>Extensions for strings.</summary>
	public static class StringExtensions {

		/// <summary>Surrounds both sides of the string with the same character.</summary>
		public static string Surround(this string str, char bothSides,
			bool trim = false)
		{
			if (trim) str = str.Trim();
			return bothSides + str + bothSides;
		}

		/// <summary>Surrounds each side of the string with a character.</summary>
		public static string Surround(this string str, char leftSide, char rightSide,
			bool trim = false)
		{
			if (trim) str = str.Trim();
			return leftSide + str + rightSide;
		}

		/// <summary>Surrounds both sides of the string with the same text.</summary>
		public static string Surround(this string str, string bothSides,
			bool trim = false)
		{
			if (trim) str = str.Trim();
			return bothSides + str + bothSides;
		}

		/// <summary>Surrounds each side of the string with text.</summary>
		public static string Surround(this string str, string leftSide,
			string rightSide, bool trim = false)
		{
			if (trim) str = str.Trim();
			return leftSide + str + rightSide;
		}
	}
}
