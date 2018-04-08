using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZeldaOracle.Common.Scripting.Internal;

namespace ZeldaOracle.Common.Scripting {
	/// <summary>A collection of variables accessible in scripting.</summary>
	[Serializable]
	public class Variables : VarBaseCollection<Variable>, IEnumerable,
		IEnumerable<Variable>, ZeldaAPI.Variables
	{
		/// <summary>The map of variables in the collection.</summary>
		private Dictionary<string, Variable> map;
		/// <summary>The object that holds these variables.</summary>
		[NonSerialized]
		private IVariableObject variableObject;


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
				Variable v		= new Variable(variable);
				v.Variables		= this;
				map[v.Name]		= v;
			}
		}


		//-----------------------------------------------------------------------------
		// IEnumerable
		//-----------------------------------------------------------------------------

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		public IEnumerator<Variable> GetEnumerator() {
			foreach (Variable variable in map.Values)
				yield return variable;
		}

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		IEnumerator IEnumerable.GetEnumerator() {
			foreach (Variable variable in map.Values)
				yield return variable;
		}


		//-----------------------------------------------------------------------------
		// Basic Accessors
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
			return map.ContainsKey(name);
		}

		/// <summary>Returns true if there exists a variable with the given name
		/// and type.</summary>
		public bool Contains<T>(string name) {
			Variable v;
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
			Variable v = GetVariable(name);
			return (v?.EqualsValue(value) ?? false);
		}

		/// <summary>Return true if there exists a variable with the given name that
		/// does not equate to the given value.</summary>
		public bool ContainsNotEquals(string name, object value) {
			Variable v = GetVariable(name);
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
		public void SetAll(Variables others) {
			foreach (Variable other in others.map.Values)
				SetVariable(other.Name, other.ObjectValue);
		}

		/// <summary>Used to restore variables that were aquired from the clipboard.</summary>
		public void RestoreFromClipboard(IVariableObject variableObject) {
			this.variableObject = variableObject;
			foreach (Variable variable in map.Values)
				variable.Variables = this;
		}

		public void AddVariable(Variable variable) {
			map[variable.Name] = variable;
			variable.Variables = this;
		}


		//-----------------------------------------------------------------------------
		// Variable Mutators (API)
		//-----------------------------------------------------------------------------

		// Single ---------------------------------------------------------------------

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

		// Indexers -------------------------------------------------------------------

		/// <summary>Sets the variable's object value at the specified index.</summary>
		void ZeldaAPI.Variables.SetObjectAt(string name, int index, object value) {
			SetObjectAt(name, index, value);
		}

		/// <summary>Sets the variable's generic value at the specified index.</summary>
		void ZeldaAPI.Variables.SetAt<T>(string name, int index, T value) {
			SetAt<T>(name, index, value);
		}

		/// <summary>Sets the variable's value at the specified index as an enum.</summary>
		void ZeldaAPI.Variables.SetEnumAt<E>(string name, int index, E value) {
			SetEnumAt<E>(name, index, value);
		}

		// Adding ---------------------------------------------------------------------

		/// <summary>Adds an object value to the variable list</summary>
		void ZeldaAPI.Variables.AddObjectAt(string name, object value) {
			AddObjectAt(name, value);
		}

		/// <summary>Adds a generic value to the variable list.</summary>
		void ZeldaAPI.Variables.AddAt<T>(string name, T value) {
			AddAt<T>(name, value);
		}

		/// <summary>Adds a value to the variable list as an enum.</summary>
		void ZeldaAPI.Variables.AddEnumAt<E>(string name, E value) {
			AddEnumAt<E>(name, value);
		}

		// Insertion ------------------------------------------------------------------

		/// <summary>Inserts an object value into the variable list.</summary>
		void ZeldaAPI.Variables.InsertObjectAt(string name, int index, object value) {
			InsertObjectAt(name, index, value);
		}

		/// <summary>Inserts a generic value into the variable list.</summary>
		void ZeldaAPI.Variables.InsertAt<T>(string name, int index, T value) {
			InsertAt<T>(name, index, value);
		}

		/// <summary>Inserts a value into the variable list as an enum.</summary>
		void ZeldaAPI.Variables.InsertEnumAt<E>(string name, int index, E value) {
			InsertEnumAt<E>(name, index, value);
		}

		// Removing -------------------------------------------------------------------

		/// <summary>Removes the element from the variable list.</summary>
		void ZeldaAPI.Variables.RemoveAt(string name, int index) {
			RemoveAt(name, index);
		}

		/// <summary>Clears all elements from the variable list.</summary>
		void ZeldaAPI.Variables.ClearList(string name) {
			ClearList(name);
		}

		// Enumerables ----------------------------------------------------------------

		/// <summary>Sets the variable as a basic array.</summary>
		void ZeldaAPI.Variables.SetObjectArray(string name, Array array) {
			SetObjectArray(name, array);
		}

		/// <summary>Sets the variable as a generic array.</summary>
		void ZeldaAPI.Variables.SetArray<T>(string name, T[] array) {
			SetArray<T>(name, array);
		}

		/// <summary>Sets the variable as a generic array length.</summary>
		void ZeldaAPI.Variables.SetArray<T>(string name, int length) {
			SetArray<T>(name, length);
		}

		/// <summary>Sets the variable as a basic list.</summary>
		void ZeldaAPI.Variables.SetObjectList(string name, IList list) {
			SetObjectList(name, list);
		}

		/// <summary>Sets the variable as a generic list.</summary>
		void ZeldaAPI.Variables.SetList<T>(string name, List<T> list) {
			SetList<T>(name, list);
		}


		//-----------------------------------------------------------------------------
		// Variable Mutators (Built-In)
		//-----------------------------------------------------------------------------

		/// <summary>Sets the specified var as an string-based enum.</summary>
		public Variable AddBuiltInEnumStr<E>(string name, E value) where E : struct {
			return AddBuiltInVariable(name,
				Variable.FromEnum<E>(VarType.String, value));
		}

		/// <summary>Sets the specified var as an integer-based enum.</summary>
		public Variable AddBuiltInEnumInt<E>(string name, E value) where E : struct {
			return AddBuiltInVariable(name,
				Variable.FromEnum<E>(VarType.Integer, value));
		}

		/// <summary>Adds a built-in variable as a generic value.</summary>
		public Variable AddBuiltIn<T>(string name, T value) {
			return AddBuiltInVariable(name, value);
		}

		/// <summary>Adds a built-in variable as an object value.</summary>
		public Variable AddBuiltInObject(string name, object value) {
			return AddBuiltInVariable(name, value);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Sets a variable's value, creating it if it doesn't already exist.
		/// This can modify an existing variable or create a new variable.</summary>
		/// <param name="builtIn">This value is only true during the creation of
		/// a built-in variable. This value is false when setting the variable.</param>
		private Variable SetVariable(string name, object value, bool builtIn = false) {
			// Set the variable value
			if (map.ContainsKey(name) && !builtIn) {
				// Set an existing variable's value
				Variable v = map[name];
				v.ObjectValue = value;
				return v;
			}
			else {
				// Create a new variable and set the value
				Variable v = new Variable(name, value, builtIn);
				v.Variables = this;
				map[name] = v;
				return v;
			}
		}

		/// <summary>Adds a new built-in variable.</summary>
		private Variable AddBuiltInVariable(string name, object value) {
			// Create a new variable and set the value
			Variable v = new Variable(name, value, true);
			v.Variables = this;
			map.Add(name, v);
			return v;
		}


		//-----------------------------------------------------------------------------
		// Debug Methods
		//-----------------------------------------------------------------------------

		/// <summary>Prints only the variables contained within this collection.</summary>
		public void Print() {
			foreach (Variable variable in GetVariables()) {
				Console.WriteLine(variable.ToString());
			}
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets the var.</summary>
		protected override Variable GetVar(string name) {
			Variable v;
			map.TryGetValue(name, out v);
			return v;
		}

		/// <summary>Sets the var.</summary>
		protected override Variable SetVar(string name, object value) {
			Variable v;
			if (!map.TryGetValue(name, out v)) {
				v = new Variable(name, value);
				v.Variables = this;
				map.Add(name, v);
			}
			else {
				v.Set(value);
			}
			return v;
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
