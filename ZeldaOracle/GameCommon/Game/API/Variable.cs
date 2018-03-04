using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.API {
	/// <summary>The possible raw data types for a variable.</summary>
	[Serializable]
	public enum VariableType {
		String,
		Integer,
		Float,
		Boolean,
		Point,
	}

	/// <summary>A variable represents a piece of data that can be represented
	/// multiple types including lists of other variables.</summary>
	[Serializable]
	public class Variable {
		/// <summary>True if the variable is built-in and cannot be renamed or removed.</summary>
		private bool builtIn;
		/// <summary>The name of the variable.</summary>
		private string name;
		/// <summary>The data type of the variable.</summary>
		private VariableType type;
		/// <summary>The object value for the variable.</summary>
		private object objectValue;
		/// <summary>The variables containing this variable.</summary>
		[NonSerialized]
		private Variables variables;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Construct a variable with the given name and type.</summary>
		public Variable(string name, VariableType type = VariableType.String, bool builtIn = false) {
			this.builtIn		= builtIn;
			this.name			= name;
			this.type			= type;
			this.objectValue	= 0;
			this.variables		= null;
		}

		/// <summary>Construct a variable as a copy of another.</summary>
		public Variable(Variable copy) {
			builtIn			= copy.builtIn;
			name			= copy.name;
			type			= copy.type;
			objectValue		= copy.objectValue;
			variables		= null;
		}


		//-----------------------------------------------------------------------------
		// General Methods
		//-----------------------------------------------------------------------------

		/// <summary>Returns a string representing the variable name and value.</summary> 
		public override string ToString() {
			// Use quotes around strings
			if (type == VariableType.String)
				return name + " = \"" + StringValue + "\"";
			return name + " = " + objectValue.ToString();
		}

		/// <summary>Returns a string representing the variable value with formatting.</summary> 
		public string FormatValue(string format) {
			if (!string.IsNullOrWhiteSpace(format)) {
				switch (type) {
				case VariableType.Integer:
					return IntValue.ToString(format);
				case VariableType.Float:
					return FloatValue.ToString(format);
				case VariableType.Boolean:
					string[] trueFalse = format.Split(new char[] { ':' }, 2);
					if (trueFalse.Length == 2) {
						return trueFalse[BoolValue ? 0 : 1];
					}
					break;
				case VariableType.Point:
					if (format.ToLower() == "x")
						return PointValue.X.ToString();
					else if(format.ToLower() == "y")
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
			Variable v = new Variable(name, TypeToVariableType(value.GetType()), builtIn);
			v.objectValue = value;
			return v;
		}

		/// <summary>Convert a VariableType to a System.Type.</summary>
		public static Type VariableTypeToType(VariableType propertyType) {
			if (propertyType == VariableType.String)
				return typeof(string);
			if (propertyType == VariableType.Integer)
				return typeof(int);
			if (propertyType == VariableType.Float)
				return typeof(float);
			if (propertyType == VariableType.Boolean)
				return typeof(bool);
			if (propertyType == VariableType.Point)
				return typeof(Point2I);
			return null;
		}

		/// <summary>Convert a System.Type to a VariableType.</summary>
		public static VariableType TypeToVariableType(Type type) {
			if (type == typeof(string))
				return VariableType.String;
			if (type == typeof(int))
				return VariableType.Integer;
			if (type == typeof(float))
				return VariableType.Float;
			if (type == typeof(bool))
				return VariableType.Boolean;
			if (type == typeof(Point2I))
				return VariableType.Point;
			return VariableType.Integer;
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
		public VariableType Type {
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
