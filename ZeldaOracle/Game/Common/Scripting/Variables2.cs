using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Scripting {
	[Serializable]
	public class Variables2 : VarBaseCollection<Variable2>, ZeldaAPI.Variables {

		/// <summary>The object that holds these variables.</summary>
		[NonSerialized]
		private IVariableObject2 variableObject;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Construct an empty variables list.</summary>
		public Variables2() {
			variableObject = null;
		}

		/// <summary>Construct an empty variables list with the given variable object.</summary>
		public Variables2(IVariableObject2 variableObject) {
			this.variableObject = variableObject;
		}

		/// <summary>Constructs a copy of the variables collection.</summary>
		public Variables2(Variables2 copy) {
			Clone(copy);
		}

		/// <summary>Constructs a copy of the variables collection.</summary>
		public Variables2(Variables2 copy, IVariableObject2 variableObject) {
			Clone(copy);
			this.variableObject = variableObject;
		}

		/// <summary>Clones the variables collection.</summary>
		public void Clone(Variables2 copy) {
			variableObject = copy.variableObject;

			// Copy the variable map
			map.Clear();
			foreach (Variable2 variable in copy.map.Values) {
				Variable2 v		= new Variable2(variable);
				v.Variables		= this;
				map[v.Name]		= v;
			}
		}


		//-----------------------------------------------------------------------------
		// Basic Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Get an enumerable list of all the variables.</summary>
		public IEnumerable<Variable2> GetVariables() {
			foreach (Variable2 variable in map.Values) {
				yield return variable;
			}
		}

		/// <summary>Get an enumerable list of all the built-in variables.</summary>
		public IEnumerable<Variable2> GetBuiltInVariables() {
			foreach (Variable2 variable in map.Values) {
				if (variable.IsBuiltIn)
					yield return variable;
			}
		}

		/// <summary>Get an enumerable list of all the custom (non-built-in) variables.</summary>
		public IEnumerable<Variable2> GetCustomVariables() {
			foreach (Variable2 variable in map.Values) {
				if (!variable.IsBuiltIn)
					yield return variable;
			}
		}

		/// <summary>Get the variable with the given name.</summary>
		public Variable2 GetVariable(string name) {
			Variable2 variable;
			map.TryGetValue(name, out variable);
			return variable;
		}

		//-----------------------------------------------------------------------------
		// Variable Existance
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if there exists a variable with the given name.</summary>
		public bool Contains(string name) {
			return map.ContainsKey(name);
		}

		/// <summary>Returns true if there exists a variable with the given name
		/// and type.</summary>
		public bool Contains<T>(string name) {
			Variable2 v;
			if (map.TryGetValue(name, out v))
				return v.FullType == typeof(T);
			return false;
		}


		//-----------------------------------------------------------------------------
		// Contains Equals
		//-----------------------------------------------------------------------------

		/// <summary>Return true if there exists a variable with the given name that
		/// equates to the given value.</summary>
		public bool ContainsEquals(string name, object value) {
			Variable2 v = GetVariable(name);
			return (v?.EqualsValue(value) ?? false);
		}

		/// <summary>Return true if there exists a variable with the given name that
		/// does not equate to the given value.</summary>
		public bool ContainsNotEquals(string name, object value) {
			Variable2 v = GetVariable(name);
			return (!v?.EqualsValue(value) ?? false);
		}


		//-----------------------------------------------------------------------------
		// General Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Clears the list of variables.</summary>
		public void Clear() {
			// Don't remove built-in variables
			var values = map.Values.ToArray();
			int index = 0;
			foreach (Variable2 v in values) {
				if (v.IsBuiltIn)
					index++;
				else
					map.Remove(v.Name);
			}
		}

		/// <summary>Removes the variable from the map.</summary>
		public bool RemoveVariable(string name) {
			// Don't remove built-in variables
			Variable2 v;
			if (map.TryGetValue(name, out v) && !v.IsBuiltIn)
				return map.Remove(name);
			return false;
		}

		/// <summary>Renames the variable to the new name.</summary>
		public bool RenameVariable(string oldName, string newName) {
			// Don't rename built-in variables
			Variable2 v = GetVariable(oldName);
			if (v != null && !v.IsBuiltIn) {
				v.Name = newName;
				map[newName] = v;
				map.Remove(oldName);
				return true;
			}
			return false;
		}

		/// <summary>Merge these variables with another.</summary>
		public void SetAll(Variables2 others) {
			foreach (Variable2 other in others.map.Values)
				SetVariable(other.Name, other.ObjectValue);
		}


		//-----------------------------------------------------------------------------
		// Variable Mutators
		//-----------------------------------------------------------------------------
		
		/// <summary>Sets the variable's object value.</summary>
		public Variable2 SetGeneric(string name, object value) {
			return SetVariable(name, value);
		}

		//-----------------------------------------------------------------------------
		// Variable API Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Sets the variable's object value.</summary>
		void ZeldaAPI.Variables.SetObject(string name, object value) {
			SetObject(name, value);
		}

		/// <summary>Sets the variable's generic value.</summary>
		void ZeldaAPI.Variables.Set<T>(string name, T value) {
			Set<T>(name, value);
		}

		/// <summary>Sets the variable's value as an enum.</summary>
		void ZeldaAPI.Variables.SetEnum<E>(string name, E value) {
			SetEnum<E>(name, value);
		}

		/// <summary>Sets the variable's value as an integer enum.</summary>
		void ZeldaAPI.Variables.SetEnumInt<E>(string name, E value) {
			SetEnumInt<E>(name, value);
		}

		/// <summary>Sets the variable's value as a string enum.</summary>
		void ZeldaAPI.Variables.SetEnumStr<E>(string name, E value) {
			SetEnumStr<E>(name, value);
		}

		/// <summary>Sets the variable's generic value at the specified index.</summary>
		void ZeldaAPI.Variables.SetAt<T>(string name, int index, T value) {
			SetAt<T>(name, index, value);
		}

		/// <summary>Sets the variable's value at the specified index as an enum.</summary>
		void ZeldaAPI.Variables.SetEnumAt<E>(string name, int index, E value) {
			SetEnumAt<E>(name, index, value);
		}

		/// <summary>Sets the variable as a generic array.</summary>
		void ZeldaAPI.Variables.SetArray<T>(string name, T[] array) {
			SetArray<T>(name, array);
		}

		/// <summary>Sets the variable as a generic array length.</summary>
		void ZeldaAPI.Variables.SetArray<T>(string name, int length) {
			SetArray<T>(name, length);
		}

		/// <summary>Sets the variable as a generic list.</summary>
		void ZeldaAPI.Variables.SetList<T>(string name, List<T> list) {
			SetList<T>(name, list);
		}


		//-----------------------------------------------------------------------------
		// Variable Mutators (Built-In)
		//-----------------------------------------------------------------------------

		/// <summary>Sets the specified var as an string-based enum.</summary>
		public Variable2 AddBuiltInEnumStr<E>(string name, E value) where E : struct {
			return AddBuiltInVariable(name,
				VarBase.FromEnum<E>(VarType.String, value));
		}

		/// <summary>Sets the specified var as an integer-based enum.</summary>
		public Variable2 AddBuiltInEnumInt<E>(string name, E value) where E : struct {
			return AddBuiltInVariable(name,
				VarBase.FromEnum<E>(VarType.Integer, value));
		}

		/// <summary>Adds a built-in variable as a generic value.</summary>
		public Variable2 AddBuiltIn<T>(string name, T value) {
			return AddBuiltInVariable(name, value);
		}

		/// <summary>Adds a built-in variable as an object value.</summary>
		public Variable2 AddBuiltInGeneric(string name, object value) {
			return AddBuiltInVariable(name, value);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Sets a variable's value, creating it if it doesn't already exist.
		/// This can modify an existing variable or create a new variable.</summary>
		private Variable2 SetVariable(string name, object value, bool builtIn = false) {
			// Set the variable value.
			if (map.ContainsKey(name) && !builtIn) {
				// Set an existing variable's value.
				Variable2 v = map[name];
				v.ObjectValue = value;
				return v;
			}
			else {
				// Create a new variable and set the value.
				Variable2 v = new Variable2(name, value, builtIn);
				v.Variables = this;
				map[name] = v;
				return v;
			}
		}

		/// <summary>Adds a new built-in variable.</summary>
		private Variable2 AddBuiltInVariable(string name, object value) {
			// Create a new variable and set the value.
			Variable2 v = new Variable2(name, value, true);
			v.Variables = this;
			map.Add(name, v);
			return v;
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		protected override Variable2 GetVar(string name) {
			Variable2 v;
			map.TryGetValue(name, out v);
			return v;
		}

		protected override Variable2 SetVar(string name, object value) {
			Variable2 v;
			if (!map.TryGetValue(name, out v)) {
				v = new Variable2(name, value);
				map.Add(name, v);
			}
			return v;
		}

		protected override Variable2 SetVarAt(string name, int index, object value) {
			Variable2 v;
			if (map.TryGetValue(name, out v)) {
				v[index] = value;
				return v;
			}
			throw new Exception("Var '" + name + "' does not exist!");
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the object that contains these variables.</summary>
		public IVariableObject2 VariableObject {
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
