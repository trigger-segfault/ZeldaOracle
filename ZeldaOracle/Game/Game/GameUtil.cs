using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game {
	/// <summary>The static class for game helper functions.</summary>
	public static class GameUtil {
		/// <summary>Rounds the specified vector to the nearest integral coordinates with bias.</summary>
		public static Vector2F Bias(Vector2F a) {
			return GMath.Round(a + GameSettings.BIAS);
		}

		/// <summary>Rounds the specified vector to the nearest integral coordinates with bias.</summary>
		public static Vector2F ReverseBias(Vector2F a) {
			return GMath.Round(a - GameSettings.BIAS);
		}

		/// <summary>Returns true if the specified type has the specified base.</summary>
		public static bool TypeHasBase<BaseType>(Type type) {
			return TypeHasBase(typeof(BaseType), type);
		}

		/// <summary>Returns true if the specified type has the specified base.</summary>
		public static bool TypeHasBase(Type baseType, Type type) {
			if (baseType == null || type == null)
				return false;
			return baseType.IsAssignableFrom(type);
		}

		/// <summary>Returns the type with the specified name. Throws an exception
		/// if the type could not be found or did not inherit from the base type.</summary>
		public static Type FindTypeWithBase<BaseType>(string typeName, bool ignoreCase) {
			return FindTypeWithBase(typeof(BaseType), typeName, ignoreCase);
		}

		/// <summary>Returns the type with the specified name. Throws an exception
		/// if the type could not be found or did not inherit from the base type.</summary>
		public static Type FindTypeWithBase(Type baseType, string typeName, bool ignoreCase) {
			StringComparison comparision = StringComparison.Ordinal;
			if (ignoreCase)
				comparision = StringComparison.OrdinalIgnoreCase;

			// Check ZeldaOracle assembly
			Type type = typeof(GameUtil).Assembly.GetTypes()
				.FirstOrDefault(t => t.Name.Equals(typeName, comparision));

			// Check ZeldaCommon assembly
			if (type == null) {
				type = typeof(Point2I).Assembly.GetTypes()
					.FirstOrDefault(t => t.Name.Equals(typeName, comparision));
			}

			if (type != null) {
				if (!baseType.IsAssignableFrom(type))
					throw new Exception("The type '" + typeName + "' does not inherit from '" + baseType.Name + "'!");
				return type;
			}
			else {
				throw new Exception("No type exists with the name '" + typeName + "'!");
			}
		}

		/// <summary>Returns the type with the specified name. Throws an exception
		/// if the type could not be found.</summary>
		public static Type FindType(string typeName, bool ignoreCase) {
			StringComparison comparision = StringComparison.Ordinal;
			if (ignoreCase)
				comparision = StringComparison.OrdinalIgnoreCase;

			// Check ZeldaOracle assembly
			Type type = typeof(GameUtil).Assembly.GetTypes()
				.FirstOrDefault(t => t.Name.Equals(typeName, comparision));

			// Check ZeldaCommon assembly
			if (type == null) {
				type = typeof(Point2I).Assembly.GetTypes()
					.FirstOrDefault(t => t.Name.Equals(typeName, comparision));
			}

			if (type != null) {
				return type;
			}
			else {
				throw new Exception("No type exists with the name '" + typeName + "'!");
			}
		}
	}
}
