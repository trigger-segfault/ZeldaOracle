using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common;
using ZeldaOracle.Common.Geometry;

namespace ZeldaAPI {
	/// <summary>An interface for static variables that can be modified from the API.</summary>
	public interface Variables {

		//-----------------------------------------------------------------------------
		// Variable Existance
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if there exists a variable with the given name.</summary>
		bool Contains(string name);

		/// <summary>Returns true if there exists a variable with the given name
		/// and type.</summary>
		bool Contains<T>(string name);


		//-----------------------------------------------------------------------------
		// Contains Equals
		//-----------------------------------------------------------------------------

		/// <summary>Return true if there exists a variable with the given name that
		/// equates to the given value.</summary>
		bool ContainsEquals(string name, object value);

		/// <summary>Return true if there exists a variable with the given name that
		/// does not equate to the given value.</summary>
		bool ContainsNotEquals(string name, object value);


		//-----------------------------------------------------------------------------
		// Variable Value Accessors
		//-----------------------------------------------------------------------------

		// Single ---------------------------------------------------------------------

		/// <summary>Gets the variable's object value.</summary>
		object GetObject(string name);

		/// <summary>Gets the variable's generic value.</summary>
		T Get<T>(string name);

		/// <summary>Gets the variable's value as an enum.</summary>
		E GetEnum<E>(string name) where E : struct;

		/// <summary>Tries to get the variable's value as an enum.</summary>
		E TryGetEnum<E>(string name) where E : struct;

		// Indexers -------------------------------------------------------------------

		/// <summary>Gets the variable's object value at the specified index.</summary>
		object GetObjectAt(string name, int index);

		/// <summary>Gets the variable's generic value at the specified index.</summary>
		T GetAt<T>(string name, int index);

		/// <summary>Gets the variable's value at the specified index as an enum.</summary>
		E GetEnumAt<E>(string name, int index) where E : struct;

		/// <summary>Tries to get the variable's value at the specified index as an
		/// enum.</summary>
		E TryGetEnumAt<E>(string name, int index) where E : struct;

		// Enumerables ----------------------------------------------------------------

		/// <summary>Sets the variable as a basic array.</summary>
		Array GetObjectArray(string name);

		/// <summary>Sets the variable as a generic array.</summary>
		T[] GetArray<T>(string name);

		/// <summary>Gets the variable as a basic list.</summary>
		IList GetObjectList(string name);

		/// <summary>Gets the variable as a generic list.</summary>
		List<T> GetList<T>(string name);

		/// <summary>Gets the variable as a basic enumerable.</summary>
		IEnumerable GetObjectEnumerable(string name);

		/// <summary>Gets the variable as a generic enumerable.</summary>
		IEnumerable<T> GetEnumerable<T>(string name);

		// Count ----------------------------------------------------------------------

		/// <summary>Gets the count of the specified var.</summary>
		int GetCount(string name);


		//-----------------------------------------------------------------------------
		// Variable Value Accessors (with defaults)
		//-----------------------------------------------------------------------------

		// Single ---------------------------------------------------------------------

		/// <summary>Gets the variable's generic value.</summary>
		T Get<T>(string name, T defaultValue);

		/// <summary>Gets the variable's value as an enum.</summary>
		E GetEnum<E>(string name, E defaultValue) where E : struct;

		/// <summary>Tries to get the variable's value as an enum.</summary>
		E TryGetEnum<E>(string name, E defaultValue) where E : struct;

		// Indexers -------------------------------------------------------------------

		/// <summary>Gets the variable's object value at the specified index.</summary>
		object GetObjectAt(string name, int index, object defaultValue);

		/// <summary>Gets the variable's generic value at the specified index.</summary>
		T GetAt<T>(string name, int index, T defaultValue);

		/// <summary>Gets the variable's value at the specified index as an enum.</summary>
		E GetEnumAt<E>(string name, int index, E defaultValue) where E : struct;

		/// <summary>Tries to get the variable's value at the specified index as an
		/// enum.</summary>
		E TryGetEnumAt<E>(string name, int index, E defaultValue) where E : struct;

		// Enumerables ----------------------------------------------------------------

		/// <summary>Gets the specified var as a basic array.</summary>
		Array GetObjectArray(string name, Array defaultArray);

		/// <summary>Gets the specified var as a generic array.</summary>
		T[] GetArray<T>(string name, T[] defaultArray);

		/// <summary>Gets the specified var as a basic list.</summary>
		IList GetObjectList(string name, IList defaultList);

		/// <summary>Gets the specified var as a generic list.</summary>
		List<T> GetList<T>(string name, List<T> defaultList);

		/// <summary>Gets the specified var as a basic enumerable.</summary>
		IEnumerable GetObjectEnumerable(string name, IEnumerable defaultEnumerable);

		/// <summary>Gets the specified var as a generic enumerable.</summary>
		IEnumerable<T> GetEnumerable<T>(string name, IEnumerable<T> defaultEnumerable);


		//-----------------------------------------------------------------------------
		// Variable Mutators
		//-----------------------------------------------------------------------------

		// Single ---------------------------------------------------------------------

		/// <summary>Sets the variable's object value.</summary>
		void SetObject(string name, object value);

		/// <summary>Sets the variable's generic value.</summary>
		void Set<T>(string name, T value);

		/// <summary>Sets the variable's value as an enum.</summary>
		void SetEnum<E>(string name, E value) where E : struct;

		/// <summary>Sets the variable's value as an integer enum.</summary>
		void SetEnumInt<E>(string name, E value) where E : struct;

		/// <summary>Sets the variable's value as a string enum.</summary>
		void SetEnumStr<E>(string name, E value) where E : struct;

		// Indexers -------------------------------------------------------------------

		/// <summary>Sets the variable's object value at the specified index.</summary>
		void SetObjectAt(string name, int index, object value);

		/// <summary>Sets the variable's generic value at the specified index.</summary>
		void SetAt<T>(string name, int index, T value);

		/// <summary>Sets the variable's value at the specified index as an enum.</summary>
		void SetEnumAt<E>(string name, int index, E value) where E : struct;

		// Adding ---------------------------------------------------------------------

		/// <summary>Adds an object value to the variable list</summary>
		void AddObjectAt(string name, object value);

		/// <summary>Adds a generic value to the variable list.</summary>
		void AddAt<T>(string name, T value);

		/// <summary>Adds a value to the variable list as an enum.</summary>
		void AddEnumAt<E>(string name, E value) where E : struct;

		// Insertion ------------------------------------------------------------------

		/// <summary>Inserts an object value into the variable list.</summary>
		void InsertObjectAt(string name, int index, object value);

		/// <summary>Inserts a generic value into the variable list.</summary>
		void InsertAt<T>(string name, int index, T value);

		/// <summary>Inserts a value into the variable list as an enum.</summary>
		void InsertEnumAt<E>(string name, int index, E value) where E : struct;

		// Removing -------------------------------------------------------------------

		/// <summary>Removes the element from the variable list.</summary>
		void RemoveAt(string name, int index);

		/// <summary>Clears all elements from the variable list.</summary>
		void ClearList(string name);

		// Enumerables ----------------------------------------------------------------

		/// <summary>Sets the variable as a basic array.</summary>
		void SetObjectArray(string name, Array array);

		/// <summary>Sets the variable as a generic array.</summary>
		void SetArray<T>(string name, T[] array);

		/// <summary>Sets the variable as a generic array length.</summary>
		void SetArray<T>(string name, int length);

		/// <summary>Sets the variable as a basic list.</summary>
		void SetObjectList(string name, IList list);

		/// <summary>Sets the variable as a generic list.</summary>
		void SetList<T>(string name, List<T> list);


		//-----------------------------------------------------------------------------
		// General Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Clears the list of variables.</summary>
		void Clear();

		/// <summary>Removes the variable from the map.</summary>
		bool RemoveVariable(string name);

		/// <summary>Renames the variable to the new name.</summary>
		bool RenameVariable(string oldName, string newName);
	}
}
