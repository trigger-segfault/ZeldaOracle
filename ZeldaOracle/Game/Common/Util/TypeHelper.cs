using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Util {
	/// <summary>A static class for helpers with object types</summary>
	public static class TypeHelper {

		/// <summary>Returns a list of types that Type inherits from.</summary>
		public static Type[] GetInheritance(Type type, bool baseTypesFirst) {
			return GetInheritance(typeof(object), type, baseTypesFirst);
		}

		/// <summary>Returns a list of types that Type inherits from until BaseType.</summary>
		public static Type[] GetInheritance<BaseType>(Type type, bool baseTypesFirst) {
			return GetInheritance(typeof(BaseType), type, baseTypesFirst);
		}

		/// <summary>Returns a list of types that Type inherits from until BaseType.</summary>
		public static Type[] GetInheritance(Type baseType, Type type, bool baseTypesFirst) {
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

		/// <summary>Returns the type with the specified name. Throws an exception
		/// if the type could not be found or did not inherit from the base type.</summary>
		public static Type FindTypeWithBase<BaseType>(string typeName,
			bool ignoreCase, params Assembly[] assemblies)
		{
			return FindTypeWithBase(typeof(BaseType), typeName, ignoreCase, assemblies);
		}

		/// <summary>Returns the type with the specified name. Throws an exception
		/// if the type could not be found or did not inherit from the base type.</summary>
		public static Type FindTypeWithBase(Type baseType, string typeName,
			bool ignoreCase, params Assembly[] assemblies)
		{
			StringComparison comparision = StringComparison.Ordinal;
			if (ignoreCase)
				comparision = StringComparison.OrdinalIgnoreCase;

			// Check the listed assemblies
			Type type = null;
			foreach (Assembly assembly in assemblies) {
				type = assembly.GetTypes()
					.FirstOrDefault(t => t.Name.Equals(typeName, comparision));
				if (type != null) {
					if (baseType.IsAssignableFrom(type))
						return type;
					else
						throw new Exception("The type '" + typeName + "' " +
							"does not inherit from '" + baseType.Name + "'!");
				}
			}
			
			throw new Exception("No type exists with the name '" + typeName + "'!");
		}

		/// <summary>Returns the type with the specified name. Throws an exception
		/// if the type could not be found.</summary>
		public static Type FindType(string typeName, bool ignoreCase,
			params Assembly[] assemblies)
		{
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
			
			throw new Exception("No type exists with the name '" + typeName + "'!");
		}
	}
}
