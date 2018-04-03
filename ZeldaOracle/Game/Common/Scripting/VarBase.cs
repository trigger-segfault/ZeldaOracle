using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Common.Scripting {

	public class UnsupportedVarTypeException : Exception {

		public UnsupportedVarTypeException(string message) : base(message) { }
	}

	[Serializable]
	public abstract class VarBase {

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
			objectValue		= CreateInstance(varType, listType, length);
		}
		
		/// <summary>Constructs a var base of the specified type.</summary>
		protected VarBase(string name, Type type, int length = 0) {
			this.name		= name;
			varType			= TypeToVarType(type, out listType);
			objectValue		= CreateInstance(varType, listType, length);
		}
		
		/// <summary>Constructs a var base with the specified value.</summary>
		protected VarBase(string name, object value) {
			this.name		= name;
			varType			= TypeToVarType(value.GetType(), out listType);
			objectValue		= value;
		}

		/// <summary>Constructs a copy of the var base.</summary>
		protected VarBase(VarBase copy) {
			name			= copy.name;
			varType			= copy.varType;
			listType		= copy.listType;
			objectValue		= copy.objectValue;
		}
		

		//-----------------------------------------------------------------------------
		// Indexers
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the element at the specified index.</summary>
		public object this[int index] {
			get {
				switch (listType) {
				case ListType.Single:
					if (index != 0)
						throw new IndexOutOfRangeException("Index must be 0 for a " +
							"singular var type!");
					return objectValue;
				case ListType.Array:
					return ((Array) objectValue).GetValue(index);
				case ListType.List:
					return ((IList) objectValue)[index];
				default: return null;
				}
			}
			set {
				switch (listType) {
				case ListType.Single:
					if (index != 0)
						throw new IndexOutOfRangeException("Index must be 0 for a " +
							"singular var type!");
					objectValue = value; break;
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
		
		/// <summary>Gets the value as the specified type.</summary>
		public T Get<T>() {
			return (T) objectValue;
		}

		/// <summary>Gets the singular value as the specified enum.</summary>
		public E GetEnum<E>() where E : struct {
			return ToEnum<E>(varType, objectValue);
		}

		/// <summary>Tries to get the singular value as the specified enum.</summary>
		public E TryGetEnum<E>() where E : struct {
			return TryToEnum<E>(varType, objectValue);
		}

		/// <summary>Gets the element at the specified index.</summary>
		public T GetAt<T>(int index) {
			switch (listType) {
			case ListType.Single:
				if (index != 0)
					throw new IndexOutOfRangeException("Index must be 0 for a " +
						"singular var type!");
				return (T) objectValue;
			case ListType.Array:
				return ((T[]) objectValue)[index];
			case ListType.List:
				return ((List<T>) objectValue)[index];
			default:
				return default(T);
			}
		}

		/// <summary>Gets the element at the specified index as an enum.</summary>
		public E GetEnumAt<E>(int index) where E : struct {
			switch (listType) {
			case ListType.Single:
				if (index != 0)
					throw new IndexOutOfRangeException("Index must be 0 for a " +
						"singular var type!");
				return ToEnum<E>(varType, objectValue);
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
				if (index != 0)
					throw new IndexOutOfRangeException("Index must be 0 for a " +
						"singular var type!");
				return TryToEnum<E>(varType, objectValue);
			case ListType.Array:
				return TryToEnum<E>(varType, ((Array) objectValue).GetValue(index));
			case ListType.List:
				return TryToEnum<E>(varType, ((IList) objectValue)[index]);
			default:
				return default(E);
			}
		}

		/// <summary>Gets the value as an array.</summary>
		public T[] GetArray<T>() {
			return (T[]) objectValue;
		}

		/// <summary>Gets the value as a list.</summary>
		public List<T> GetList<T>() {
			return (List<T>) objectValue;
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

		/// <summary>Sets the value as the specified type.</summary>
		public void Set<T>(T value) {
			ObjectValue = value;
		}

		/// <summary>Sets the singular value as the specified enum.</summary>
		public void SetEnum<E>(E value) where E : struct {
			objectValue = FromEnum<E>(varType, value);
		}

		/// <summary>Sets the element at the specified index.</summary>
		public void SetAt<T>(int index, T value) {
			switch (listType) {
			case ListType.Single:
				if (index != 0)
					throw new IndexOutOfRangeException("Index must be 0 for a " +
						"singular var type!");
				ObjectValue = value; break;
			case ListType.Array:
				((T[]) objectValue)[index] = value; break;
			case ListType.List:
				((List<T>) objectValue)[index] = value; break;
			}
		}

		/// <summary>Sets the element at the specified index as an enum.</summary>
		public void SetEnumAt<E>(int index, E value) where E : struct {
			switch (listType) {
			case ListType.Single:
				if (index != 0)
					throw new IndexOutOfRangeException("Index must be 0 for a " +
						"singular var type!");
				objectValue = FromEnum<E>(varType, value); break;
			case ListType.Array:
				((Array) objectValue).SetValue(FromEnum<E>(varType, value), index);
				break;
			case ListType.List:
				((IList) objectValue)[index] = FromEnum<E>(varType, value);
				break;
			}
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
			case VarType.String:	return (T) Enum.Parse(typeof(T), (string) value);
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

		
		/// <summary>Convert a VariableType to a System.Type.</summary>
		public static Type VarTypeToType(VarType varType) {
			switch (varType) {
			case VarType.String:	return typeof(string);
			case VarType.Integer:	return typeof(int);
			case VarType.Float:		return typeof(float);
			case VarType.Boolean:	return typeof(bool);
			case VarType.Point:		return typeof(Point2I);
			case VarType.Vector:	return typeof(Vector2F);
			default: return null;
			}
		}


		/// <summary>Convert a VariableType to a System.Type.</summary>
		public static Type VarTypeToType(VarType varType, ListType listType) {
			switch (listType) {
			case ListType.Single:
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
				default: return null;
				}
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
				default: return null;
				}
			default:
				return null;
			}
		}

		/// <summary>Convert a System.Type to a VariableType.</summary>
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
			return VarType.Integer;
		}


		/// <summary>Convert a System.Type to a VariableType.</summary>
		public static VarType TypeToVarType(Type type, out ListType listType) {
			if (type.IsArray) {
				listType = ListType.Array;
				return TypeToVarType(type.GetElementType());
			}
			else if (type.GetGenericTypeDefinition() == typeof(List<>)) {
				listType = ListType.List;
				return TypeToVarType(type.GetGenericArguments()[0]);
			}
			else {
				listType = ListType.Single;
				return TypeToVarType(type);
			}
		}

		/*public static bool TypeMatchesVarType(Type type, VarType varType, ListType listType) {
			return (type == VarTypeToType(varType, listType));
		}*/

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
				if (FullType != value.GetType())
					throw new UnsupportedVarTypeException("Object type does not " +
						"match var type!");
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
