using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>A variable represents a piece of data that can be represented
	/// multiple types including lists of other variables.</summary>
	[Serializable]
	public class Variable {

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
		/// <summary>The name of the variable.</summary>
		private string name;
		/// <summary>The data type of the variable.</summary>
		private VarType type;
		/// <summary>The object value for the variable.</summary>
		private object objectValue;
		/// <summary>The variables containing this variable.</summary>
		[NonSerialized]
		private Variables variables;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Construct a variable with the given name and type.</summary>
		public Variable(string name, VarType type = VarType.String, bool builtIn = false) {
			this.builtIn        = builtIn;
			this.name           = name;
			this.type           = type;
			this.objectValue    = 0;
			this.variables      = null;
		}

		/// <summary>Construct a variable as a copy of another.</summary>
		public Variable(Variable copy) {
			builtIn         = copy.builtIn;
			name            = copy.name;
			type            = copy.type;
			objectValue     = copy.objectValue;
			variables       = null;
		}


		//-----------------------------------------------------------------------------
		// General Methods
		//-----------------------------------------------------------------------------

		/// <summary>Returns a string representing the variable name and value.</summary> 
		public override string ToString() {
			// Use quotes around strings
			if (type == VarType.String)
				return name + " = \"" + StringValue + "\"";
			return name + " = " + objectValue.ToString();
		}

		/// <summary>Returns a string representing the variable value with formatting.</summary> 
		public string FormatValue(string format) {
			if (!string.IsNullOrWhiteSpace(format)) {
				switch (type) {
				case VarType.Integer:
					return IntValue.ToString(format);
				case VarType.Float:
					return FloatValue.ToString(format);
				case VarType.Boolean:
					string[] trueFalse = format.Split(new char[] { ':' }, 2);
					if (trueFalse.Length == 2) {
						return trueFalse[BoolValue ? 0 : 1];
					}
					break;
				case VarType.Point:
					if (format.ToLower() == "x")
						return PointValue.X.ToString();
					else if (format.ToLower() == "y")
						return PointValue.Y.ToString();
					break;
				}
			}
			return objectValue.ToString();
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Return true of this and another variable have the equivilent values.</summary>
		public bool EqualsValue(Variable other) {
			if (type != other.type)
				return false;
			return objectValue.Equals(other.objectValue);
		}

		/// <summary>Return true of this and another variable have the equivilent values.</summary>
		public bool EqualsValue(object other) {
			if (type != TypeToVariableType(other.GetType()))
				return false;
			return objectValue.Equals(other);
		}


		//-----------------------------------------------------------------------------
		// Static Factory Methods
		//-----------------------------------------------------------------------------

		/// <summary>Create a boolean property with the given value.</summary>
		public static Variable Create(string name, object value, bool builtIn = false) {
			if (!IsValidName(name))
				throw new ArgumentException("Invalid variable name!");
			Variable v = new Variable(name, TypeToVariableType(value.GetType()), builtIn);
			v.objectValue = value;
			return v;
		}

		/// <summary>Convert a VariableType to a System.Type.</summary>
		public static Type VariableTypeToType(VarType propertyType) {
			if (propertyType == VarType.String)
				return typeof(string);
			if (propertyType == VarType.Integer)
				return typeof(int);
			if (propertyType == VarType.Float)
				return typeof(float);
			if (propertyType == VarType.Boolean)
				return typeof(bool);
			if (propertyType == VarType.Point)
				return typeof(Point2I);
			return null;
		}

		/// <summary>Convert a System.Type to a VariableType.</summary>
		public static VarType TypeToVariableType(Type type) {
			if (type == typeof(string))
				return VarType.String;
			if (type == typeof(int))
				return VarType.Integer;
			if (type == typeof(float))
				return VarType.Float;
			if (type == typeof(bool))
				return VarType.Boolean;
			if (type == typeof(Point2I))
				return VarType.Point;
			return VarType.Integer;
		}

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

		/// <summary>The name/key of the variable.</summary>
		public string Name {
			get { return name; }
			internal set { name = value; }
		}

		/// <summary>The variables container this variable is contained in.</summary>
		public Variables Variables {
			get { return variables; }
			internal set { variables = value; }
		}

		// Property Value ------------------------------------------------------------

		/// <summary>The data type of the variable.</summary>
		public VarType Type {
			get { return type; }
		}

		/// <summary>The variable's raw object value.</summary>
		public object ObjectValue {
			get { return objectValue; }
			set { objectValue = value; }
		}

		/// <summary>The variable value as a string.</summary>
		public string StringValue {
			get { return (string) objectValue; }
			set { objectValue = value; }
		}

		/// <summary>The variable value as an integer.</summary>
		public int IntValue {
			get { return (int) objectValue; }
			set { objectValue = value; }
		}

		/// <summary>The variable value as a float.</summary>
		public float FloatValue {
			get { return (float) objectValue; }
			set { objectValue = value; }
		}

		/// <summary>The variable value as a boolean.</summary>
		public bool BoolValue {
			get { return (bool) objectValue; }
			set { objectValue = value; }
		}

		/// <summary>The variable value as a point.</summary>
		public Point2I PointValue {
			get { return (Point2I) objectValue; }
			set { objectValue = value; }
		}
	}
}
