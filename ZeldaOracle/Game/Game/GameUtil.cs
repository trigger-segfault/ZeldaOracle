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

		/// <summary>Returns the type with the specified name. Throws an exception
		/// if the type could not be found or did not inherit from the base type.</summary>
		public static Type GetTypeWithBase<BaseType>(string typeName, bool ignoreCase) {
			StringComparison comparision = StringComparison.Ordinal;
			if (ignoreCase)
				comparision = StringComparison.OrdinalIgnoreCase;

			Type type = Assembly.GetExecutingAssembly().GetTypes()
				.FirstOrDefault(t => t.Name.Equals(typeName, comparision));
			if (type != null) {
				Type baseType = type;
				do {
					if (baseType.Equals(typeof(BaseType)))
						return type;
					baseType = baseType.BaseType;
				} while (baseType != null);
				throw new Exception("The type '" + typeName + "' does not inherit from '" + typeof(BaseType).Name + "'!");
			}
			else {
				throw new Exception("No type exists with the name '" + typeName + "'!");
			}
		}
	}
}
