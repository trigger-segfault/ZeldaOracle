using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Common.Scripting.Internal {
	/// <summary>An exception thrown when a var type is not supported or mismatched.</summary>
	public class UnsupportedVarTypeException : Exception {
		public UnsupportedVarTypeException(string message) : base(message) { }
	}

	/// <summary>The base var class that var collections contain.</summary>
	[Serializable]
	public abstract class VarBase : IEnumerable {

		/// <summary>The name of the var.</summary>
		private string name;
		/// <summary>The element type of the var.</summary>
		private VarType varType;
		/// <summary>The list type of the var.</summary>
		private ListType listType;
		/// <summary>The object value of the var.</summary>
		private object objectValue;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a var base of the specified type.</summary>
		protected VarBase(string name, VarType varType, ListType listType,
			int length = 0)
		{
			this.name		= name;
			this.varType	= varType;
			this.listType	= listType;
			if (varType == VarType.Custom)
				throw new UnsupportedVarTypeException("Var type 'Custom'" +
					"' is not supported!");
			objectValue		= CreateInstance(varType, listType, length);
		}
		
		/// <summary>Constructs a var base of the specified type.</summary>
		protected VarBase(string name, Type type, int length = 0) {
			this.name		= name;
			if (!IsSupportedType(type))
				throw new UnsupportedVarTypeException("Var type '" + type.Name +
					"' is not supported!");
			varType			= TypeToVarType(type, out listType);
			objectValue		= CreateInstance(varType, listType, length);
		}
		
		/// <summary>Constructs a var base with the specified value.</summary>
		protected VarBase(string name, object value) {
			this.name		= name;
			Type type		= value.GetType();
			varType			= TypeToVarType(type, out listType);
			if (NeedsIntCasting(type))
				objectValue = Convert.ToInt32(value);
			else if (NeedsFloatCasting(type))
				objectValue = Convert.ToSingle(value);
			else if (IsSupportedType(type))
				objectValue = value;
			else
				throw new UnsupportedVarTypeException("Var type '" + type.Name +
					"' is not supported!");
		}

		/// <summary>Constructs a copy of the var base.</summary>
		protected VarBase(VarBase copy) {
			name			= copy.name;
			varType			= copy.varType;
			listType		= copy.listType;
			objectValue		= copy.objectValue;
		}


		//-----------------------------------------------------------------------------
		// IEnumerable
		//-----------------------------------------------------------------------------

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		IEnumerator IEnumerable.GetEnumerator() {
			foreach (object value in EnumerableValue) {
				yield return value;
			}
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Creates a string representation of the var.</summary>
		public override string ToString() {
			string str = name + " = ";
			if (IsEnumerable) {
				str += "{";
				bool first = true;
				foreach (object obj in EnumerableValue) {
					if (!first)	str += ", ";
					else		first = false;

					if (varType == VarType.String)
						str += "\"" + ((string) obj) + "\"";
					else
						str += obj.ToString();
				}
				str += "}";
			}
			else {
				if (varType == VarType.String)
					str += "\"" + ((string) objectValue) + "\"";
				else
					str += objectValue.ToString();
			}
			return str;
		}


		//-----------------------------------------------------------------------------
		// Indexers
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the element at the specified index.</summary>
		public object this[int index] {
			get {
				switch (listType) {
				case ListType.Single:
					throw new InvalidOperationException("Cannot use indexer for a non-" +
						"enumerable var type!");
				case ListType.Array:
					return ((Array) objectValue).GetValue(index);
				case ListType.List:
					return ((IList) objectValue)[index];
				default: return null;
				}
			}
			set {
				Type type = value.GetType();
				if (NeedsIntCasting(type))
					value = Convert.ToInt32(value);
				else if (NeedsFloatCasting(type))
					value = Convert.ToSingle(value);

				switch (listType) {
				case ListType.Single:
					throw new InvalidOperationException("Cannot use indexer for a non-" +
						"enumerable var type!");
				case ListType.Array:
					((Array) objectValue).SetValue(value, index); break;
				case ListType.List:
					((IList) objectValue)[index] = value; break;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		// Single ---------------------------------------------------------------------

		/// <summary>Gets the value as the specified type.</summary>
		public object GetObject() {
			return objectValue;
		}

		/// <summary>Gets the value as the specified type.</summary>
		public T Get<T>() {
			if (NeedsAnyCasting(typeof(T)))
				return (T) TypeHelper.ConvertFrom(typeof(T), objectValue);
			return (T) objectValue;
			//return (T) CastTo(typeof(T), objectValue);
		}

		/// <summary>Gets the singular value as the specified enum.</summary>
		public E GetEnum<E>() where E : struct {
			return ToEnum<E>(varType, objectValue);
		}

		/// <summary>Tries to get the singular value as the specified enum.</summary>
		public E TryGetEnum<E>() where E : struct {
			return TryToEnum<E>(varType, objectValue);
		}

		// Indexers -------------------------------------------------------------------

		/// <summary>Gets the element at the specified index.</summary>
		public object GetObjectAt(int index) {
			return this[index];
		}

		/// <summary>Gets the element at the specified index.</summary>
		public T GetAt<T>(int index) {
			if (NeedsAnyCasting(typeof(T)))
				return (T) TypeHelper.ConvertFrom(typeof(T), this[index]);
			return (T) this[index];
		}

		/// <summary>Gets the element at the specified index as an enum.</summary>
		public E GetEnumAt<E>(int index) where E : struct {
			switch (listType) {
			case ListType.Single:
				throw new InvalidOperationException("Cannot use indexer for a non-" +
					"enumerable var type!");
			case ListType.Array:
				return ToEnum<E>(varType, ((Array) objectValue).GetValue(index));
			case ListType.List:
				return ToEnum<E>(varType, ((IList) objectValue)[index]);
			default:
				return default(E);
			}
		}

		/// <summary>Tries to get the element at the specified index as an enum.</summary>
		public E TryGetEnumAt<E>(int index) where E : struct {
			switch (listType) {
			case ListType.Single:
				throw new InvalidOperationException("Cannot use indexer for a non-" +
					"enumerable var type!");
			case ListType.Array:
				return TryToEnum<E>(varType, ((Array) objectValue).GetValue(index));
			case ListType.List:
				return TryToEnum<E>(varType, ((IList) objectValue)[index]);
			default:
				return default(E);
			}
		}

		// Enumerables ----------------------------------------------------------------

		/// <summary>Gets the value as a basic array.</summary>
		public Array GetObjectArray() {
			return (Array) objectValue;
		}

		/// <summary>Gets the value as an array.</summary>
		public T[] GetArray<T>() {
			return (T[]) objectValue;
		}

		/// <summary>Gets the value as a basic list.</summary>
		public IList GetObjectList() {
			return (IList) objectValue;
		}

		/// <summary>Gets the value as a list.</summary>
		public List<T> GetList<T>() {
			return (List<T>) objectValue;
		}

		/// <summary>Gets the value as a basic enumerable.</summary>
		public IEnumerable GetObjectEnumerable() {
			if (IsEnumerable)
				return (IEnumerable) objectValue;
			return new object[1] { objectValue };
		}

		/// <summary>Gets the value as an enumerable.</summary>
		public IEnumerable<T> GetEnumerable<T>() {
			if (IsEnumerable)
				return (IEnumerable<T>) objectValue;
			return new T[1] { (T) objectValue };
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		// Single ---------------------------------------------------------------------

		/// <summary>Sets the value as an object.</summary>
		public void SetObject(object value) {
			ObjectValue = value;
		}

		/// <summary>Sets the value as the specified type.</summary>
		public void Set<T>(T value) {
			ObjectValue = value;
		}

		/// <summary>Sets the singular value as the specified enum.</summary>
		public void SetEnum<E>(E value) where E : struct {
			objectValue = FromEnum<E>(varType, value);
		}

		// Indexers -------------------------------------------------------------------

		/// <summary>Sets the element at the specified index.</summary>
		public void SetObjectAt(int index, object value) {
			this[index] = value;
		}

		/// <summary>Sets the element at the specified index.</summary>
		public void SetAt<T>(int index, T value) {
			this[index] = value;
		}

		/// <summary>Sets the element at the specified index as an enum.</summary>
		public void SetEnumAt<E>(int index, E value) where E : struct {
			switch (listType) {
			case ListType.Single:
				throw new InvalidOperationException("Cannot use indexer for a non-" +
					"enumerable var type!");
			case ListType.Array:
				((Array) objectValue).SetValue(FromEnum<E>(varType, value), index);
				break;
			case ListType.List:
				((IList) objectValue)[index] = FromEnum<E>(varType, value);
				break;
			}
		}

		// Adding ---------------------------------------------------------------------

		/// <summary>Adds the element to the end of the list.</summary>
		public void AddObjectAt(object value) {
			Type type = value.GetType();
			if (NeedsIntCasting(type))
				value = Convert.ToInt32(value);
			else if (NeedsFloatCasting(type))
				value = Convert.ToSingle(value);
			((IList) objectValue).Add(value);
		}

		/// <summary>Adds the element to the end of the list.</summary>
		public void AddAt<T>(T value) {
			AddObjectAt(value);
		}

		/// <summary>Adds the element to the end of the list as an enum.</summary>
		public void AddEnumAt<E>(E value) where E : struct {
			((IList) objectValue).Add(FromEnum<E>(varType, value));
		}

		// Insertion ------------------------------------------------------------------

		/// <summary>Inserts the element into the list.</summary>
		public void InsertObjectAt(int index, object value) {
			Type type = value.GetType();
			if (NeedsIntCasting(type))
				value = Convert.ToInt32(value);
			else if (NeedsFloatCasting(type))
				value = Convert.ToSingle(value);
			((IList) objectValue).Insert(index, value);
		}

		/// <summary>Inserts the element into the list.</summary>
		public void InsertAt<T>(int index, T value) {
			InsertObjectAt(index, value);
		}

		/// <summary>Inserts the element into the list as an enum.</summary>
		public void InsertEnumAt<E>(int index, E value) where E : struct {
			((IList) objectValue).Insert(index, FromEnum<E>(varType, value));
		}

		// Removing -------------------------------------------------------------------

		/// <summary>Removes the element from the list.</summary>
		public void RemoveAt(int index) {
			((IList) objectValue).RemoveAt(index);
		}

		/// <summary>Clears the list of all elements.</summary>
		public void ClearList() {
			((IList) objectValue).Clear();
		}


		//-----------------------------------------------------------------------------
		// Comparison
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the vars are of the same type and equal the
		/// same value.</summary>
		public bool EqualsValue(VarBase other) {
			if (varType == other.varType && listType == other.listType) {
				if (!IsEnumerable)
					return objectValue.Equals(other.objectValue);
				else
					EnumerableValue.Cast<object>().SequenceEqual(
						other.EnumerableValue.Cast<object>());
			}
			return false;
		}

		/// <summary>Returns true if the vars are of the same type and equal the
		/// same value.</summary>
		public bool EqualsValue(object other) {
			if (FullType == other.GetType()) {
				if (!IsEnumerable)
					return objectValue.Equals(other);
				else
					EnumerableValue.Cast<object>().SequenceEqual(
						((IEnumerable) other).Cast<object>());
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Cloning
		//-----------------------------------------------------------------------------

		/// <summary>Clones the value of the var. Only needed for lists and arrays.</summary>
		public object CloneValue() {
			switch (listType) {
			case ListType.Single:
				return objectValue;
			case ListType.Array:
				Array array = Array.CreateInstance(ElementType, Count);
				((Array) objectValue).CopyTo(array, 0);
				return array;
			case ListType.List:
				IList list = (IList) Activator.CreateInstance(FullType);
				foreach (object value in ((IList) objectValue))
					list.Add(value);
				return list;
			default:
				return null;
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets an object value from an enum and var type.</summary>
		public static object FromEnum<T>(VarType varType, T value)
			where T : struct
		{
			switch (varType) {
			case VarType.String:	return value.ToString();
			case VarType.Integer:	return Convert.ToInt32(value);
			default:
				throw new UnsupportedVarTypeException("Var Type does not support enums!");
			}
		}

		/// <summary>Gets an enum from an object value and var type.</summary>
		public static T ToEnum<T>(VarType varType, object value)
			where T : struct
		{
			switch (varType) {
			case VarType.String:	return (T) Enum.Parse(typeof(T), (string) value, true);
			case VarType.Integer:	return (T) Enum.ToObject(typeof(T), (int) value);
			default:
				throw new UnsupportedVarTypeException("Var Type does not support enums!");
			}
		}

		/// <summary>Tries to get an enum from an object value and var type.</summary>
		public static T TryToEnum<T>(VarType varType, object value)
			where T : struct
		{
			try {
				if (varType == VarType.String)
					return (T) Enum.Parse(typeof(T), (string) value);
				else if (varType == VarType.Integer)
					return (T) Enum.ToObject(typeof(T), (int) value);
			}
			catch {
				return default(T);
			}
			throw new UnsupportedVarTypeException("Var Type does not support enums!");
		}

		/// <summary>Convert a VarType to a System.Type.</summary>
		public static Type VarTypeToType(VarType varType) {
			switch (varType) {
			case VarType.String:	return typeof(string);
			case VarType.Integer:	return typeof(int);
			case VarType.Float:		return typeof(float);
			case VarType.Boolean:	return typeof(bool);
			case VarType.Point:		return typeof(Point2I);
			case VarType.Vector:	return typeof(Vector2F);
			case VarType.RangeI:	return typeof(RangeI);
			case VarType.RangeF:	return typeof(RangeF);
			case VarType.RectangleI:return typeof(Rectangle2I);
			case VarType.RectangleF:return typeof(Rectangle2F);
			case VarType.Color:		return typeof(Color);
			default: return null;
			}
		}
		
		/// <summary>Convert a VarType to a System.Type.</summary>
		public static Type VarTypeToType(VarType varType, ListType listType) {
			switch (listType) {
			case ListType.Single:
				return VarTypeToType(varType);
			case ListType.Array:
				switch (varType) {
				case VarType.String:	return typeof(string[]);
				case VarType.Integer:	return typeof(int[]);
				case VarType.Float:		return typeof(float[]);
				case VarType.Boolean:	return typeof(bool[]);
				case VarType.Point:		return typeof(Point2I[]);
				case VarType.Vector:	return typeof(Vector2F[]);
				case VarType.RangeI:	return typeof(RangeI[]);
				case VarType.RangeF:	return typeof(RangeF[]);
				case VarType.RectangleI:return typeof(Rectangle2I[]);
				case VarType.RectangleF:return typeof(Rectangle2F[]);
				case VarType.Color:		return typeof(Color[]);
				default: return null;
				}
			case ListType.List:
				switch (varType) {
				case VarType.String:	return typeof(List<string>);
				case VarType.Integer:	return typeof(List<int>);
				case VarType.Float:		return typeof(List<float>);
				case VarType.Boolean:	return typeof(List<bool>);
				case VarType.Point:		return typeof(List<Point2I>);
				case VarType.Vector:	return typeof(List<Vector2F>);
				case VarType.RangeI:	return typeof(List<RangeI>);
				case VarType.RangeF:	return typeof(List<RangeF>);
				case VarType.RectangleI:return typeof(List<Rectangle2I>);
				case VarType.RectangleF:return typeof(List<Rectangle2F>);
				case VarType.Color:		return typeof(List<Color>);
				default: return null;
				}
			default:
				return null;
			}
		}

		/// <summary>Convert a System.Type to a VarType.</summary>
		public static VarType TypeToVarType(Type type) {
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
			if (type == typeof(Vector2F))
				return VarType.Vector;
			if (type == typeof(RangeI))
				return VarType.RangeI;
			if (type == typeof(RangeF))
				return VarType.RangeF;
			if (type == typeof(Rectangle2I))
				return VarType.RectangleI;
			if (type == typeof(Rectangle2F))
				return VarType.RectangleF;
			if (type == typeof(Color))
				return VarType.Color;

			// Special casting
			if (type == typeof(Direction))
				return VarType.Integer;
			if (type == typeof(Angle))
				return VarType.Integer;

			return VarType.Custom;
		}

		/// <summary>Convert a System.Type to a VarType.</summary>
		public static VarType TypeToVarType(Type type, out ListType listType) {
			if (type.IsArray && type.GetArrayRank() == 1) {
				listType = ListType.Array;
				return TypeToVarType(type.GetElementType());
			}
			else if (type.IsGenericTypeDefinition &&
				type.GetGenericTypeDefinition() == typeof(List<>))
			{
				listType = ListType.List;
				return TypeToVarType(type.GetGenericArguments()[0]);
			}
			else {
				listType = ListType.Single;
				return TypeToVarType(type);
			}
		}

		/// <summary>Returns true if the type needs to be casted from an integer.</summary>
		public static bool NeedsIntCasting(Type type) {
			return
				type == typeof(Direction) ||
				type == typeof(Angle) ||
				type == typeof(sbyte) ||
				type == typeof(byte) ||
				type == typeof(short) ||
				type == typeof(ushort) ||
				type == typeof(uint) ||
				type == typeof(long) ||
				type == typeof(ulong);
		}

		/// <summary>Returns true if the type needs to be casted from a float.</summary>
		public static bool NeedsFloatCasting(Type type) {
			return
				type == typeof(double) ||
				type == typeof(decimal);
		}

		/// <summary>Returns true if the type needs to be casted from any base type.</summary>
		public static bool NeedsAnyCasting(Type type) {
			return NeedsIntCasting(type) || NeedsFloatCasting(type);
		}

		/// <summary>Returns true if this type is supported with vars.</summary>
		public static bool IsSupportedType(Type type) {
			// Get the element type
			if (type.IsArray && type.GetArrayRank() == 1) {
				type = type.GetElementType();
			}
			else if (type.IsGenericTypeDefinition &&
				type.GetGenericTypeDefinition() == typeof(List<>))
			{
				type = type.GetGenericArguments()[0];
			}
			return (TypeToVarType(type) != VarType.Custom);
		}

		/// <summary>Constructs the initial object for the var and list type.</summary>
		public static object CreateInstance(VarType varType, ListType listType, int length = 0) {
			Type elementType = VarTypeToType(varType);
			switch (listType) {
			case ListType.Single:
				return Activator.CreateInstance(elementType);
			case ListType.Array:
				return Array.CreateInstance(elementType, length);
			case ListType.List:
				Type newType = typeof(List<>).MakeGenericType(elementType);
				return Activator.CreateInstance(newType);
			default:
				return null;
			}
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Information ----------------------------------------------------------------

		/// <summary>Gets or sets the name of the var.</summary>
		public string Name {
			get { return name; }
			set { name = value; }
		}

		/// <summary>Gets the element type of the var.</summary>
		public VarType VarType {
			get { return varType; }
		}

		/// <summary>Gets the list type of the var.</summary>
		public ListType ListType {
			get { return listType; }
		}

		/// <summary>Gets the full type of the var.</summary>
		public Type FullType {
			get { return VarTypeToType(varType, listType); }
		}

		/// <summary>Gets the element type of the var.</summary>
		public Type ElementType {
			get { return VarTypeToType(varType); }
		}

		// Value ----------------------------------------------------------------------

		/// <summary>Gets the var's object value.</summary>
		public object ObjectValue {
			get { return objectValue; }
			set {
				Type type = value.GetType();
				if (NeedsIntCasting(type))
					value = Convert.ToInt32(value);
				else if (NeedsFloatCasting(type))
					value = Convert.ToSingle(value);
				if (FullType != value.GetType())
					throw new UnsupportedVarTypeException("Object type '" +
						value.GetType().Name + "' does not match var " +
						"type '" + FullType.Name + "'!");
				objectValue = value;
			}
		}

		/// <summary>Gets the var as an array.</summary>
		public Array ArrayValue {
			get { return (Array) objectValue; }
			set { ObjectValue = value; }
		}

		/// <summary>Gets the var as a list.</summary>
		public IList ListValue {
			get { return (IList) objectValue; }
			set { ObjectValue = value; }
		}

		/// <summary>Gets the var list type as an enumerable.</summary>
		public IEnumerable EnumerableValue {
			get {
				if (IsEnumerable)
					return (IEnumerable) objectValue;
				return new object[1] { objectValue };
			}
		}

		/// <summary>Gets if the var is not a single value.</summary>
		public bool IsEnumerable {
			get { return listType != ListType.Single; }
		}

		/// <summary>Gets if the var is not a value type.</summary>
		public bool IsClass {
			get { return listType != ListType.Single; }
		}

		/// <summary>Gets the number of elements in the enumerable.
		/// Returns one if this is a single value.</summary>
		public int Count {
			get {
				switch (listType) {
				case ListType.Array: return ((Array) objectValue).Length;
				case ListType.List: return ((IList) objectValue).Count;
				default: return 1;
				}
			}
		}
	}
}
