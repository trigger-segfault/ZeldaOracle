using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Util {
	/// <summary>An exception thrown when a type could not be found.</summary>
	public class MissingTypeException : Exception {
		/// <summary>Constructs the missing type exception for the specified type.</summary>
		public MissingTypeException(string typeName)
			: base("Type '" + typeName + "' could not be found!") { }
	}

	/// <summary>An exception thrown when a type does not inherit from the
	/// required type.</summary>
	public class NoInheritanceException : Exception {
		/// <summary>Constructs the does not inherit exception for the specified types.</summary>
		public NoInheritanceException(Type baseType, Type type)
			: base("Type '" + type.Name + "' does not inherit '" +
				  baseType.Name + "'!") { }
	}

	/// <summary>A static class for helpers with object types</summary>
	public static class TypeHelper {
		
		//-----------------------------------------------------------------------------
		// Get Inheritance
		//-----------------------------------------------------------------------------

		/// <summary>Returns a list of types that Type inherits from.</summary>
		public static Type[] GetInheritance(Type type, bool baseTypesFirst) {
			return GetInheritance(typeof(object), type, baseTypesFirst);
		}

		/// <summary>Returns a list of types that Type inherits from until BaseType.</summary>
		public static Type[] GetInheritance<BaseType>(Type type, bool baseTypesFirst) {
			return GetInheritance(typeof(BaseType), type, baseTypesFirst);
		}

		/// <summary>Returns a list of types that Type inherits from until BaseType.</summary>
		/// <exception cref="ArgumentException">Type does not inherit base type.</exception>
		public static Type[] GetInheritance(Type baseType, Type type,
			bool baseTypesFirst)
		{
			if (baseType == null || type == null)
				return new Type[0];
			if (!baseType.IsAssignableFrom(type))
				throw new ArgumentException("Type does not inherit from BaseType!");
			List<Type> types = new List<Type>();
			while (type != null) {
				if (baseTypesFirst)
					types.Insert(0, type);
				else
					types.Add(type);
				if (type.Equals(baseType))
					break;
				type = type.BaseType;
			}
			return types.ToArray();
		}


		//-----------------------------------------------------------------------------
		// Type Has Base
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the specified type implements the specified base.</summary>
		public static bool TypeHasBase<BaseType>(Type type) {
			return TypeHasBase(typeof(BaseType), type);
		}

		/// <summary>Returns true if the specified type implements the specified base.</summary>
		public static bool TypeHasBase(Type baseType, Type type) {
			if (baseType == null || type == null)
				return false;
			return baseType.IsAssignableFrom(type);
		}


		//-----------------------------------------------------------------------------
		// Find Type With Base
		//-----------------------------------------------------------------------------

		/// <summary>Returns the type with the specified name.</summary>
		/// <exception cref="MissingTypeException">The type could not be found.</exception>
		/// <exception cref="MissingTypeException">The type does not inherit base type.</exception>
		public static Type FindTypeWithBase<BaseType>(string typeName,
			bool ignoreCase, params Assembly[] assemblies)
		{
			return FindTypeWithBase(typeof(BaseType), typeName, ignoreCase,
				assemblies);
		}

		/// <summary>Returns the type with the specified name.
		/// Returns null if the type was not found or does not inheritf base type.</summary>
		public static Type FindTypeWithBaseSafe<BaseType>(string typeName,
			bool ignoreCase, params Assembly[] assemblies)
		{
			return FindTypeWithBaseSafe(typeof(BaseType), typeName, ignoreCase,
				assemblies);
		}

		/// <summary>Returns the type with the specified name.</summary>
		/// <exception cref="MissingTypeException">The type could not be found.</exception>
		/// <exception cref="MissingTypeException">The type does not inherit base type.</exception>
		public static Type FindTypeWithBase(Type baseType, string typeName,
			bool ignoreCase, params Assembly[] assemblies)
		{
			Type type = FindType(typeName, ignoreCase, assemblies);
			if (!baseType.IsAssignableFrom(type))
				throw new NoInheritanceException(baseType, type);
			return type;
		}

		/// <summary>Returns the type with the specified name.
		/// Returns null if the type was not found or does not inheritf base type.</summary>
		public static Type FindTypeWithBaseSafe(Type baseType, string typeName,
			bool ignoreCase, params Assembly[] assemblies)
		{
			Type type = FindTypeSafe(typeName, ignoreCase, assemblies);
			if (type != null && !baseType.IsAssignableFrom(type))
				throw new NoInheritanceException(baseType, type);
			return type;
		}


		//-----------------------------------------------------------------------------
		// Find Type
		//-----------------------------------------------------------------------------

		/// <summary>Returns the type with the specified name. Throws an exception
		/// if the type could not be found.</summary>
		/// <exception cref="MissingTypeException">The type could not be found.</exception>
		public static Type FindType(string typeName, bool ignoreCase,
			params Assembly[] assemblies)
		{
			Type type = FindTypeSafe(typeName, ignoreCase, assemblies);
			if (type == null)
				throw new MissingTypeException(typeName);
			return type;
		}

		/// <summary>Returns the type with the specified name. Returns null if the
		/// type could not be found.</summary>
		public static Type FindTypeSafe(string typeName, bool ignoreCase,
			params Assembly[] assemblies) {
			StringComparison comparision = StringComparison.Ordinal;
			if (ignoreCase)
				comparision = StringComparison.OrdinalIgnoreCase;

			// Check the listed assemblies
			Type type = null;
			foreach (Assembly assembly in assemblies) {
				type = assembly.GetTypes()
					.FirstOrDefault(t => t.Name.Equals(typeName, comparision));
				if (type != null)
					return type;
			}

			return null;
		}


		//-----------------------------------------------------------------------------
		// Get Default Value
		//-----------------------------------------------------------------------------

		/// <summary>Gets the default value of the type.
		/// Runtime equivalent of default(type).</summary>
		public static T GetDefaultValue<T>() {
			return (T) GetDefaultValue(typeof(T));
		}

		/// <summary>Gets the default value of the type.
		/// Runtime equivalent of default(type).</summary>
		public static object GetDefaultValue(Type type) {
			if (type.IsValueType)
				return Activator.CreateInstance(type);
			return null;
		}


		//-----------------------------------------------------------------------------
		// Converting
		//-----------------------------------------------------------------------------

		/// <summary>Converts the value to the specified type using the type's
		/// type converter.</summary>
		public static object ConvertFrom(Type type, object value) {
			var converter = TypeDescriptor.GetConverter(type);
			return converter.ConvertFrom(value);
		}

		/// <summary>Converts the value to the specified type using the value's
		/// type converter.</summary>
		public static object ConvertTo(Type type, object value) {
			var converter = TypeDescriptor.GetConverter(value.GetType());
			return converter.ConvertTo(value, type);
		}
	}
}
