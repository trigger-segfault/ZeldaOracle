using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Scripting {
	[Serializable]
	public class Variable2 : VarBase {
		
		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The list of all names reserved due to being C# object methods.</summary>
		private static readonly HashSet<string> ReservedNames = new HashSet<string>() {
			"Equals",
			"Finalize",
			"GetHashCode",
			"GetType",
			"MemberwiseClone",
			"ReferenceEquals",
			"ToString"
		};


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>True if the variable is built-in and cannot be renamed or removed.</summary>
		private bool builtIn;
		/// <summary>The variables containing this variable.</summary>
		[NonSerialized]
		private Variables2 variables;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a variable of the specified type.</summary>
		public Variable2(string name, VarType varType, ListType listType,
			int length = 0, bool builtIn = false)
			: base(name, varType, listType, length)
		{
			this.builtIn	= builtIn;
		}

		/// <summary>Constructs a variable of the specified type.</summary>
		public Variable2(string name, Type type, int length = 0, bool builtIn = false)
			: base(name, type, length)
		{
			this.builtIn	= builtIn;
		}

		/// <summary>Constructs a variable with the specified value.</summary>
		public Variable2(string name, object value, bool builtIn = false)
			: base(name, value)
		{
			this.builtIn	= builtIn;
		}

		/// <summary>Constructs a copy of the variable.</summary>
		public Variable2(Variable2 copy) : base(copy) {
			builtIn		= copy.builtIn;
			variables	= copy.variables;
		}


		//-----------------------------------------------------------------------------
		// General Methods
		//-----------------------------------------------------------------------------

		/// <summary>Returns a string representing the variable name and value.</summary> 
		public override string ToString() {
			// Use quotes around strings
			if (VarType == VarType.String)
				return Name + " = \"" + Get<string>() + "\"";
			return Name + " = " +ObjectValue.ToString();
		}

		/// <summary>Returns a string representing the variable value with formatting.</summary> 
		public string FormatValue(string format) {
			if (!string.IsNullOrWhiteSpace(format)) {
				switch (VarType) {
				case VarType.Integer:
					return Get<int>().ToString(format);
				case VarType.Float:
					return Get<float>().ToString(format);
				case VarType.Boolean:
					string[] trueFalse = format.Split(new char[] { ':' }, 2);
					if (trueFalse.Length == 2) {
						return trueFalse[Get<bool>() ? 0 : 1];
					}
					break;
				case VarType.Point:
					if (format.ToLower() == "x")
						return Get<Point2I>().X.ToString();
					else if (format.ToLower() == "y")
						return Get<Point2I>().Y.ToString();
					break;
				}
			}
			return ObjectValue.ToString();
		}

		
		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the specified name is a valid variable name.</summary>
		public static bool IsValidName(string varName) {
			// Empty names are not allowed
			if (varName.Length == 0)
				return false;

			// Reserved names cannot be implemented into the object class
			if (ReservedNames.Contains(varName))
				return false;

			// First characters can't have digits
			if (!char.IsLetter(varName[0]) && varName[0] != '_')
				return false;

			// Remaining characters can have digits
			for (int i = 1; i < varName.Length; i++) {
				if (!char.IsLetterOrDigit(varName[i]) && varName[i] != '_')
					return false;
			}
			return true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>True if the property is built-in to the base game.</summary>
		public bool IsBuiltIn {
			get { return builtIn; }
		}

		/// <summary>The variables container this variable is contained in.</summary>
		public Variables2 Variables {
			get { return variables; }
			set { variables = value; }
		}
	}
}
