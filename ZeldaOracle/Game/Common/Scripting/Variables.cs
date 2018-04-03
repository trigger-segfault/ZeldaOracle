using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>A collection of variables accessible in scripting.</summary>
	public class Variables : ZeldaAPI.Variables {
		/// <summary>The object that holds these variables.</summary>
		[NonSerialized]
		private IVariableObject variableObject;
		/// <summary>The variable map.</summary>
		private Dictionary<string, Variable> map;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Construct an empty variables list.</summary>
		public Variables() {
			map = new Dictionary<string, Variable>();
			variableObject = null;
		}

		/// <summary>Construct an empty variables list with the given variable object.</summary>
		public Variables(IVariableObject variableObject) {
			map = new Dictionary<string, Variable>();
			this.variableObject = variableObject;
		}

		/// <summary>Constructs a copy of the variables collection.</summary>
		public Variables(Variables copy) {
			map = new Dictionary<string, Variable>();
			Clone(copy);
		}

		/// <summary>Constructs a copy of the variables collection.</summary>
		public Variables(Variables copy, IVariableObject variableObject) {
			map = new Dictionary<string, Variable>();
			Clone(copy);
			this.variableObject = variableObject;
		}

		/// <summary>Clones the variables collection.</summary>
		public void Clone(Variables copy) {
			variableObject = copy.variableObject;

			// Copy the variable map
			map.Clear();
			foreach (Variable variable in copy.map.Values) {
				Variable v      = new Variable(variable);
				v.Variables     = this;
				map[v.Name]     = v;
			}
		}


		//-----------------------------------------------------------------------------
		// Basic accessors
		//-----------------------------------------------------------------------------

		/// <summary>Get an enumerable list of all the variables.</summary>
		public IEnumerable<Variable> GetVariables() {
			foreach (Variable variable in map.Values) {
				yield return variable;
			}
		}

		/// <summary>Get an enumerable list of all the built-in variables.</summary>
		public IEnumerable<Variable> GetBuiltInVariables() {
			foreach (Variable variable in map.Values) {
				if (variable.IsBuiltIn)
					yield return variable;
			}
		}

		/// <summary>Get an enumerable list of all the custom (non-built-in) variables.</summary>
		public IEnumerable<Variable> GetCustomVariables() {
			foreach (Variable variable in map.Values) {
				if (!variable.IsBuiltIn)
					yield return variable;
			}
		}

		/// <summary>Get the variable with the given name.</summary>
		public Variable GetVariable(string name) {
			Variable variable;
			map.TryGetValue(name, out variable);
			return variable;
		}


		//-----------------------------------------------------------------------------
		// Variable Existance
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if there exists a variable with the given name.</summary>
		public bool Contains(string name) {
			return (GetVariable(name) != null);
		}

		/// <summary>Returns true if there exists a variable with the given name
		/// and type.</summary>
		public bool Contains(string name, VarType type) {
			Variable v = GetVariable(name);
			return (v != null && v.Type == type);
		}


		//-----------------------------------------------------------------------------
		// Contains Equals
		//-----------------------------------------------------------------------------

		/// <summary>Return true if there exists a variable with the given name that
		/// equates to the given value.</summary>
		public bool ContainsEquals(string name, object value) {
			Variable v = GetVariable(name);
			return (v != null && v.EqualsValue(value));
		}

		/// <summary>Return true if there exists a variable with the given name that
		/// does not equate to the given value.</summary>
		public bool ContainsNotEquals(string name, object value) {
			Variable v = GetVariable(name);
			return (v != null && !v.EqualsValue(value));
		}


		//-----------------------------------------------------------------------------
		// Variable Value Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Get an enum variable value.</summary>
		public E GetEnum<E>(string name) where E : struct {
			Variable v = GetVariable(name);
			if (v.Type == VarType.Integer)
				return (E) Enum.ToObject(typeof(E), v.IntValue);
			else if (v.Type == VarType.String)
				return (E) Enum.Parse(typeof(E), v.StringValue);
			else
				throw new InvalidOperationException("Variable type does not support enums.");
		}

		/// <summary>Tries to get an enum variable value.</summary>
		public E TryGetEnum<E>(string name) where E : struct {
			Variable v = GetVariable(name);
			if (v != null) {
				try {
					if (v.Type == VarType.Integer)
						return (E) Enum.ToObject(typeof(E), v.IntValue);
					else if (v.Type == VarType.String)
						return (E) Enum.Parse(typeof(E), v.StringValue, true);
					else
						throw new InvalidOperationException("Variable type does not support enums.");
				}
				catch { }
			}
			return default(E);
		}

		/// <summary>Get a string variable value.</summary>
		public string GetString(string name) {
			return Get<string>(name);
		}

		/// <summary>Get an integer variable value.</summary>
		public int GetInteger(string name) {
			return Get<int>(name);
		}

		/// <summary>Get a float variable value.</summary>
		public float GetFloat(string name) {
			return Get<float>(name);
		}

		/// <summary>Get a boolean variable value.</summary>
		public bool GetBoolean(string name) {
			return Get<bool>(name);
		}

		/// <summary>Get a boolean variable value.</summary>
		public Point2I GetPoint(string name) {
			return Get<Point2I>(name);
		}

		/// <summary>Get a generic variable value.</summary>
		public T Get<T>(string name) {
			return (T) GetVariable(name).ObjectValue;
		}


		//-----------------------------------------------------------------------------
		// Property value access (with defaults)
		//-----------------------------------------------------------------------------

		/// <summary>Get an enum variable value with a default value fallback.</summary>
		public E GetEnum<E>(string name, E defaultValue) where E : struct {
			Variable v = GetVariable(name);
			if (v != null) {
				if (v.Type == VarType.Integer)
					return (E) Enum.ToObject(typeof(E), v.IntValue);
				else if (v.Type == VarType.String)
					return (E) Enum.Parse(typeof(E), v.StringValue, true);
				else
					throw new InvalidOperationException("Variable type does not support enums.");
			}
			return defaultValue;
		}

		/// <summary>Tries to get an enum variable value with a default value fallback.</summary>
		public E TryGetEnum<E>(string name, E defaultValue) where E : struct {
			Variable v = GetVariable(name);
			if (v != null) {
				try {
					if (v.Type == VarType.Integer)
						return (E) Enum.ToObject(typeof(E), v.IntValue);
					else if (v.Type == VarType.String)
						return (E) Enum.Parse(typeof(E), v.StringValue, true);
					else
						throw new InvalidOperationException("Variable type does not support enums.");
				}
				catch { }
			}
			return defaultValue;
		}

		/// <summary>Get a string variable value with a default value fallback.</summary>
		public string GetString(string name, string defaultValue) {
			return Get<string>(name, defaultValue);
		}

		/// <summary>Get an integer variable value with a default value fallback.</summary>
		public int GetInteger(string name, int defaultValue) {
			return Get<int>(name, defaultValue);
		}

		/// <summary>Get a float variable value with a default value fallback.</summary>
		public float GetFloat(string name, float defaultValue) {
			return Get<float>(name, defaultValue);
		}

		/// <summary>Get a boolean variable value with a default value fallback.</summary>
		public bool GetBoolean(string name, bool defaultValue) {
			return Get<bool>(name, defaultValue);
		}

		/// <summary>Get a point variable value with a default value fallback.</summary>
		public Point2I GetPoint(string name, Point2I defaultValue) {
			return Get<Point2I>(name, defaultValue);
		}

		/// <summary>Get a generic variable value with a default value fallback.</summary>
		public T Get<T>(string name, T defaultValue) {
			Variable v = GetVariable(name);
			if (v != null)
				return (T) v.ObjectValue;
			return defaultValue;
		}


		//-----------------------------------------------------------------------------
		// General Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Clears the list of variables.</summary>
		public void Clear() {
			// Don't remove built-in variables
			var values = map.Values.ToArray();
			int index = 0;
			foreach (Variable v in values) {
				if (v.IsBuiltIn)
					index++;
				else
					map.Remove(v.Name);
			}
		}

		/// <summary>Removes the variable from the map.</summary>
		public bool RemoveVariable(string name) {
			// Don't remove built-in variables
			Variable v;
			if (map.TryGetValue(name, out v) && !v.IsBuiltIn)
				return map.Remove(name);
			return false;
		}

		/// <summary>Renames the variable to the new name.</summary>
		public bool RenameVariable(string oldName, string newName) {
			// Don't rename built-in variables
			Variable v = GetVariable(oldName);
			if (v != null && !v.IsBuiltIn) {
				v.Name = newName;
				map[newName] = v;
				map.Remove(oldName);
				return true;
			}
			return false;
		}

		/// <summary>Merge these variables with another.</summary>
		public void SetAll(Variables other) {
			foreach (Variable otherVariable in other.map.Values)
				SetVariable(otherVariable.Name, otherVariable.ObjectValue);
		}


		//-----------------------------------------------------------------------------
		// Property Setters
		//-----------------------------------------------------------------------------

		/// <summary>Sets the variable's value as an enum.</summary>
		public Variable SetEnum<E>(string name, E value,
			VarType defaultType = VarType.String) where E : struct
		{
			Variable v = GetVariable(name);
			VarType type = defaultType;
			if (v != null)
				type = v.Type;
			if (type == VarType.Integer)
				return SetVariable(name, (int) (object) value);
			else if (type == VarType.String)
				return SetVariable(name, value.ToString());
			else
				throw new InvalidOperationException("Variable type does not support enums.");
		}
		
		/// <summary>Sets the variable's value as an integer enum.</summary>
		public Variable SetEnumInt<E>(string name, E value) where E : struct {
			Variable v = GetVariable(name);
			if (v == null || v.Type == VarType.Integer)
				return SetVariable(name, (int) (object) value);
			else
				throw new InvalidOperationException("Variable type is not an integer.");
		}
		
		/// <summary>Sets the variable's value as a string enum.</summary>
		public Variable SetEnumStr<E>(string name, E value) where E : struct {
			Variable v = GetVariable(name);
			if (v == null || v.Type == VarType.String)
				return SetVariable(name, value.ToString());
			else
				throw new InvalidOperationException("Variable type is not a string.");
		}

		/// <summary>Sets the variable's string value.</summary>
		public Variable Set(string name, string value) {
			return SetVariable(name, value);
		}

		/// <summary>Sets the variable's integer value.</summary>
		public Variable Set(string name, int value) {
			return SetVariable(name, value);
		}

		/// <summary>Sets the variable's float value.</summary>
		public Variable Set(string name, float value) {
			return SetVariable(name, value);
		}

		/// <summary>Sets the variable's boolean value.</summary>
		public Variable Set(string name, bool value) {
			return SetVariable(name, value);
		}

		/// <summary>Sets the variable's point value.</summary>
		public Variable Set(string name, Point2I value) {
			return SetVariable(name, value);
		}

		/// <summary>Sets the variable's object value.</summary>
		public Variable SetGeneric(string name, object value) {
			return SetVariable(name, value);
		}


		//-----------------------------------------------------------------------------
		// Property API Setters
		//-----------------------------------------------------------------------------

		/// <summary>Sets the variable's value as an enum.</summary>
		void ZeldaAPI.Variables.SetEnum<E>(string name, E value,
			VarType defaultType)
		{
			Variable v = GetVariable(name);
			VarType type = defaultType;
			if (v != null)
				type = v.Type;
			if (type == VarType.Integer)
				SetVariable(name, (int) (object) value);
			else if (type == VarType.String)
				SetVariable(name, value.ToString());
			else
				throw new InvalidOperationException("Variable type does not support enums.");
		}

		/// <summary>Sets the variable's value as an integer enum.</summary>
		void ZeldaAPI.Variables.SetEnumInt<E>(string name, E value) {
			Variable v = GetVariable(name);
			if (v == null || v.Type == VarType.Integer)
				SetVariable(name, (int) (object) value);
			else
				throw new InvalidOperationException("Variable type is not an integer.");
		}

		/// <summary>Sets the variable's value as a string enum.</summary>
		void ZeldaAPI.Variables.SetEnumStr<E>(string name, E value) {
			Variable v = GetVariable(name);
			if (v == null || v.Type == VarType.String)
				SetVariable(name, value.ToString());
			else
				throw new InvalidOperationException("Variable type is not a string.");
		}


		//-----------------------------------------------------------------------------
		// Property Adders (Built-In)
		//-----------------------------------------------------------------------------

		/// <summary>Adds a built-in variable as an enum.</summary>
		public Variable AddBuiltInEnum<E>(string name, E value,
			VarType type = VarType.String) where E : struct
		{
			if (type == VarType.Integer)
				return AddBuiltInVariable(name, (int) (object) value);
			else if (type == VarType.String)
				return AddBuiltInVariable(name, value.ToString());
			else
				throw new InvalidOperationException("Variable type does not support enums.");
		}

		/// <summary>Adds a built-in variable as a string.</summary>
		public Variable AddBuiltIn(string name, string value) {
			return AddBuiltInVariable(name, value);
		}

		/// <summary>Adds a built-in variable as an integer.</summary>
		public Variable AddBuiltIn(string name, int value) {
			return AddBuiltInVariable(name, value);
		}

		/// <summary>Adds a built-in variable as a float.</summary>
		public Variable AddBuiltIn(string name, float value) {
			return AddBuiltInVariable(name, value);
		}

		/// <summary>Adds a built-in variable as a boolean.</summary>
		public Variable AddBuiltIn(string name, bool value) {
			return AddBuiltInVariable(name, value);
		}

		/// <summary>Adds a built-in variable as a point.</summary>
		public Variable AddBuiltIn(string name, Point2I value) {
			return AddBuiltInVariable(name, value);
		}

		/// <summary>Adds a built-in variable as a generic value.</summary>
		public Variable AddBuiltInGeneric(string name, object value) {
			return AddBuiltInVariable(name, value);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Sets a variable's value, creating it if it doesn't already exist.
		/// This can modify an existing variable or create a new variable.</summary>
		private Variable SetVariable(string name, object value, bool builtIn = false) {
			// Set the variable value.
			if (map.ContainsKey(name) && !builtIn) {
				// Set an existing variable's value.
				Variable v = map[name];
				v.ObjectValue = value;
				return v;
			}
			else {
				// Create a new variable and set the value.
				Variable v = Variable.Create(name, value, builtIn);
				v.Variables = this;
				map[name] = v;
				return v;
			}
		}

		/// <summary>Adds a new built-in variable.</summary>
		private Variable AddBuiltInVariable(string name, object value) {
			// Create a new variable and set the value.
			Variable v = Variable.Create(name, value, true);
			v.Variables = this;
			map.Add(name, v);
			return v;
		}


		//-----------------------------------------------------------------------------
		// Debug Methods
		//-----------------------------------------------------------------------------

		/// <summary>Writes all variables and their values to the console.</summary>
		public void Print() {
			foreach (Variable variable in map.Values) {
				Console.WriteLine(variable);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the object that contains these variables.</summary>
		public IVariableObject VariableObject {
			get { return variableObject; }
			set { variableObject = value; }
		}

		/// <summary>Gets the number of variables.</summary>
		public int Count {
			get { return map.Count; }
		}

		/// <summary>Gets the number of built-in variables.</summary>
		public int BuiltInCount {
			get { return map.Count(v => v.Value.IsBuiltIn); }
		}

		/// <summary>Gets the number of custom (non-built-in) variables.</summary>
		public int CustomCount {
			get { return map.Count(v => !v.Value.IsBuiltIn); }
		}
	}
}
