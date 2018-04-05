using System;
using System.Collections;
using System.Collections.Generic;

namespace ZeldaOracle.Common.Scripting.Internal {
	/// <summary>A base class for a collection of var bases.</summary>
	[Serializable]
	public abstract class VarBaseCollection<VarBaseType> where VarBaseType : VarBase {
		
		//-----------------------------------------------------------------------------
		// Abstract Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets the var.</summary>
		protected abstract VarBaseType GetVar(string name);

		/// <summary>Sets the var.</summary>
		protected abstract VarBaseType SetVar(string name, object value);


		//-----------------------------------------------------------------------------
		// Var Value Accessors
		//-----------------------------------------------------------------------------

		// Single ---------------------------------------------------------------------

		/// <summary>Gets the specified var as an object value.</summary>
		public object GetObject(string name) {
			return GetVar(name).GetObject();
		}

		/// <summary>Gets the specified var as a generic value.</summary>
		public T Get<T>(string name) {
			return GetVar(name).Get<T>();
		}

		/// <summary>Gets the specified var as an enum.</summary>
		public E GetEnum<E>(string name) where E : struct {
			return GetVar(name).GetEnum<E>();
		}

		/// <summary>Tries to get the specified var as an enum.</summary>
		public E TryGetEnum<E>(string name) where E : struct {
			return GetVar(name).TryGetEnum<E>();
		}

		// Indexers -------------------------------------------------------------------

		/// <summary>Gets the specified var element as an object value.</summary>
		public object GetObjectAt(string name, int index) {
			return GetVar(name)[index];
		}

		/// <summary>Gets the specified var element as a generic value.</summary>
		public T GetAt<T>(string name, int index) {
			return GetVar(name).GetAt<T>(index);
		}

		/// <summary>Gets the specified var element as an enum.</summary>
		public E GetEnumAt<E>(string name, int index) where E : struct {
			return GetVar(name).GetEnumAt<E>(index);
		}

		/// <summary>Tries to get the specified var element as an enum.</summary>
		public E TryGetEnumAt<E>(string name, int index) where E : struct {
			return GetVar(name).TryGetEnumAt<E>(index);
		}

		// Enumerables ----------------------------------------------------------------

		/// <summary>Gets the specified var as a bais array.</summary>
		public Array GetObjectArray(string name) {
			return GetVar(name).GetObjectArray();
		}

		/// <summary>Gets the specified var as a generic array.</summary>
		public T[] GetArray<T>(string name) {
			return GetVar(name).GetArray<T>();
		}

		/// <summary>Gets the specified var as a basic list.</summary>
		public IList GetObjectList(string name) {
			return GetVar(name).GetObjectList();
		}

		/// <summary>Gets the specified var as a generic list.</summary>
		public List<T> GetList<T>(string name) {
			return GetVar(name).GetList<T>();
		}

		/// <summary>Gets the specified var as a basic enumerable.</summary>
		public IEnumerable GetObjectEnumerable(string name) {
			return GetVar(name).GetObjectEnumerable();
		}

		/// <summary>Gets the specified var as a generic enumerable.</summary>
		public IEnumerable<T> GetEnumerable<T>(string name) {
			return GetVar(name).GetEnumerable<T>();
		}

		// Count ----------------------------------------------------------------------

		/// <summary>Gets the count of the specified var.</summary>
		public int GetCount(string name) {
			return GetVar(name).Count;
		}


		//-----------------------------------------------------------------------------
		// Var Value Accessors (with defaults)
		//-----------------------------------------------------------------------------

		// Single ---------------------------------------------------------------------

		/// <summary>Gets the specified var as an object value.</summary>
		public object GetObject(string name, object defaultValue) {
			var v = GetVar(name);
			if (v != null) return v.GetObject();
			return defaultValue;
		}

		/// <summary>Gets the specified var as a generic value.</summary>
		public T Get<T>(string name, T defaultValue) {
			var v = GetVar(name);
			if (v != null) return v.Get<T>();
			return defaultValue;
		}

		/// <summary>Gets the specified var as an enum.</summary>
		public E GetEnum<E>(string name, E defaultValue) where E : struct {
			var v = GetVar(name);
			if (v != null) return v.GetEnum<E>();
			return defaultValue;
		}

		/// <summary>Tries to get the specified var as an enum.</summary>
		public E TryGetEnum<E>(string name, E defaultValue) where E : struct {
			var v = GetVar(name);
			if (v != null) return v.TryGetEnum<E>();
			return defaultValue;
		}

		// Indexers -------------------------------------------------------------------

		/// <summary>Gets the specified var element as an object value.</summary>
		public object GetObjectAt(string name, int index, object defaultValue) {
			var v = GetVar(name);
			if (v != null) return v.GetObjectAt(index);
			return defaultValue;
		}

		/// <summary>Gets the specified var element as a generic value.</summary>
		public T GetAt<T>(string name, int index, T defaultValue) {
			var v = GetVar(name);
			if (v != null) return v.GetAt<T>(index);
			return defaultValue;
		}

		/// <summary>Gets the specified var element as an enum.</summary>
		public E GetEnumAt<E>(string name, int index, E defaultValue)
			where E : struct
		{
			var v = GetVar(name);
			if (v != null) return v.GetEnumAt<E>(index);
			return defaultValue;
		}

		/// <summary>Tries to get the specified var element as an enum.</summary>
		public E TryGetEnumAt<E>(string name, int index, E defaultValue)
			where E : struct
		{
			var v = GetVar(name);
			if (v != null) return v.TryGetEnumAt<E>(index);
			return defaultValue;
		}

		// Enumerables ----------------------------------------------------------------

		/// <summary>Gets the specified var as a basic array.</summary>
		public Array GetObjectArray(string name, Array defaultArray) {
			return GetVar(name)?.GetObjectArray() ?? defaultArray;
		}

		/// <summary>Gets the specified var as a generic array.</summary>
		public T[] GetArray<T>(string name, T[] defaultArray) {
			return GetVar(name)?.GetArray<T>() ?? defaultArray;
		}

		/// <summary>Gets the specified var as a basic list.</summary>
		public IList GetObjectList(string name, IList defaultList) {
			return GetVar(name)?.GetObjectList() ?? defaultList;
		}

		/// <summary>Gets the specified var as a generic list.</summary>
		public List<T> GetList<T>(string name, List<T> defaultList) {
			return GetVar(name)?.GetList<T>() ?? defaultList;
		}

		/// <summary>Gets the specified var as a generic enumerable.</summary>
		public IEnumerable GetObjectEnumerable(string name,
			IEnumerable defaultEnumerable)
		{
			return GetVar(name)?.GetObjectEnumerable() ?? defaultEnumerable;
		}

		/// <summary>Gets the specified var as a generic enumerable.</summary>
		public IEnumerable<T> GetEnumerable<T>(string name,
			IEnumerable<T> defaultEnumerable)
		{
			return GetVar(name)?.GetEnumerable<T>() ?? defaultEnumerable;
		}


		//-----------------------------------------------------------------------------
		// Var Value Mutators
		//-----------------------------------------------------------------------------

		// Single ---------------------------------------------------------------------

		/// <summary>Sets the specified var as an object value.</summary>
		public VarBaseType SetObject(string name, object value) {
			return SetVar(name, value);
		}

		/// <summary>Sets the specified var as a generic value.</summary>
		public VarBaseType Set<T>(string name, T value) {
			return SetVar(name, value);
		}

		/// <VarBaseType>Sets the specified var as an enum.</summary>
		public VarBaseType SetEnum<E>(string name, E value) where E : struct {
			var v = GetVar(name);
			return SetVar(name, VarBase.FromEnum<E>(v.VarType, value));
		}

		/// <summary>Sets the specified var as an string-based enum.</summary>
		public VarBaseType SetEnumStr<E>(string name, E value) where E : struct {
			return SetVar(name, VarBase.FromEnum<E>(VarType.String, value));
		}

		/// <summary>Sets the specified var as an integer-based enum.</summary>
		public VarBaseType SetEnumInt<E>(string name, E value) where E : struct {
			return SetVar(name, VarBase.FromEnum<E>(VarType.Integer, value));
		}

		// Indexers -------------------------------------------------------------------

		/// <summary>Sets the specified var element at the specified index as an
		/// object value.</summary>
		public VarBaseType SetObjectAt(string name, int index, object value) {
			var v = GetVar(name);
			v.SetObjectAt(index, value);
			return v;
		}

		/// <summary>Sets the specified var element at the specified index as a
		/// generic value.</summary>
		public VarBaseType SetAt<T>(string name, int index, T value) {

			var v = GetVar(name);
			v.SetAt<T>(index, value);
			return v;
		}

		/// <summary>Sets the specified var element at the specified index as an enum.</summary>
		public VarBaseType SetEnumAt<E>(string name, int index, E value)
			where E : struct
		{
			var v = GetVar(name);
			v.SetEnumAt<E>(index, value);
			return v;
		}

		// Adding ---------------------------------------------------------------------

		/// <summary>Adds the element to the end of the list var as an object value.</summary>
		public VarBaseType AddObjectAt(string name, object value) {
			var v = GetVar(name);
			v.AddObjectAt(value);
			return v;
		}

		/// <summary>Adds the element to the end of the list var as a generic value.</summary>
		public VarBaseType AddAt<T>(string name, T value) {
			var v = GetVar(name);
			v.AddAt<T>(value);
			return v;
		}

		/// <summary>Sets the specified var element at the specified index as an enum.</summary>
		public VarBaseType AddEnumAt<E>(string name, E value) where E : struct {
			var v = GetVar(name);
			v.AddEnumAt<E>(value);
			return v;
		}

		// Insertion ------------------------------------------------------------------

		/// <summary>Adds the element to the end of the list var as an object value.</summary>
		public VarBaseType InsertObjectAt(string name, int index, object value) {
			var v = GetVar(name);
			v.InsertObjectAt(index, value);
			return v;
		}

		/// <summary>Adds the element to the end of the list var as a generic value.</summary>
		public VarBaseType InsertAt<T>(string name, int index, T value) {
			var v = GetVar(name);
			v.InsertAt<T>(index, value);
			return v;
		}

		/// <summary>Sets the specified var element at the specified index as an enum.</summary>
		public VarBaseType InsertEnumAt<E>(string name, int index, E value)
			where E : struct
		{
			var v = GetVar(name);
			v.InsertEnumAt<E>(index, value);
			return v;
		}

		// Removing -------------------------------------------------------------------

		/// <summary>Removes the element from the list.</summary>
		public VarBaseType RemoveAt(string name, int index) {
			var v = GetVar(name);
			v.RemoveAt(index);
			return v;
		}

		/// <summary>Clears all elements from the list.</summary>
		public VarBaseType ClearList(string name) {
			var v = GetVar(name);
			v.ClearList();
			return v;
		}

		// Enumerables ----------------------------------------------------------------

		/// <summary>Sets the specified var as a basic array.</summary>
		public VarBaseType SetObjectArray(string name, Array array) {
			return SetVar(name, array);
		}

		/// <summary>Sets the specified var as a generic array.</summary>
		public VarBaseType SetArray<T>(string name, T[] array) {
			return SetVar(name, array);
		}

		/// <summary>Sets the specified var as a generic array length.</summary>
		public VarBaseType SetArray<T>(string name, int length) {
			return SetVar(name, new T[length]);
		}

		/// <summary>Sets the specified var as a generic list.</summary>
		public VarBaseType SetObjectList(string name, IList list) {
			return SetVar(name, list);
		}

		/// <summary>Sets the specified var as a generic list.</summary>
		public VarBaseType SetList<T>(string name, List<T> list) {
			return SetVar(name, list);
		}
	}
}
