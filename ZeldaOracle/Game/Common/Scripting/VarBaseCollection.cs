using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Scripting {
	public abstract class VarBaseCollection<VarBaseType> where VarBaseType : VarBase {

		/// <summary>The map of vars in the collection.</summary>
		protected Dictionary<string, VarBaseType> map;

		//-----------------------------------------------------------------------------
		// Abstract Methods
		//-----------------------------------------------------------------------------
		
		protected abstract VarBaseType GetVar(string name);

		protected abstract VarBaseType SetVar(string name, object value);

		protected abstract VarBaseType SetVarAt(string name, int index, object value);


		//-----------------------------------------------------------------------------
		// Var Value Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the specified var as an object value.</summary>
		public object GetObject(string name) {
			return GetVar(name).ObjectValue;
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

		/// <summary>Gets the specified var as a bais array.</summary>
		public Array GetArray(string name) {
			return GetVar(name).ArrayValue;
		}

		/// <summary>Gets the specified var as a generic array.</summary>
		public T[] GetArray<T>(string name) {
			return GetVar(name).GetArray<T>();
		}

		/// <summary>Gets the specified var as a basic list.</summary>
		public IList GetList(string name) {
			return GetVar(name).ListValue;
		}

		/// <summary>Gets the specified var as a generic list.</summary>
		public List<T> GetList<T>(string name) {
			return GetVar(name).GetList<T>();
		}

		/// <summary>Gets the specified var as a basic enumerable.</summary>
		public IEnumerable GetEnumerable(string name) {
			return GetVar(name).EnumerableValue;
		}

		/// <summary>Gets the specified var as a generic enumerable.</summary>
		public IEnumerable<T> GetEnumerable<T>(string name) {
			return GetVar(name).GetEnumerable<T>();
		}

		/// <summary>Gets the count of the specified var.</summary>
		public int GetCount(string name) {
			return GetVar(name).Count;
		}


		//-----------------------------------------------------------------------------
		// Var Value Accessors (with defaults)
		//-----------------------------------------------------------------------------

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

		/// <summary>Gets the specified var as a generic array.</summary>
		public T[] GetArray<T>(string name, T[] defaultArray) {
			return GetVar(name)?.GetArray<T>() ?? defaultArray;
		}

		/// <summary>Gets the specified var as a generic list.</summary>
		public List<T> GetList<T>(string name, List<T> defaultList) {
			return GetVar(name)?.GetList<T>() ?? defaultList;
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

		/// <summary>Sets the specified var element at the specified index as a
		/// generic value.</summary>
		public VarBaseType SetAt<T>(string name, int index, T value) {
			return SetVarAt(name, index, value);
		}

		/// <summary>Sets the specified var element at the specified index as an enum.</summary>
		public VarBaseType SetEnumAt<E>(string name, int index, E value)
			where E : struct
		{
			var v = GetVar(name);
			return SetVarAt(name, index, VarBase.FromEnum<E>(v.VarType, value));
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
		public VarBaseType SetList<T>(string name, List<T> list) {
			return SetVar(name, list);
		}
	}
}
