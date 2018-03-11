using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;

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
		public static Type FindTypeWithBase<BaseType>(string typeName, bool ignoreCase) {
			return TypeHelper.FindTypeWithBase<BaseType>(
				typeName, ignoreCase, Assemblies.Game);
		}

		/// <summary>Returns the type with the specified name. Throws an exception
		/// if the type could not be found or did not inherit from the base type.</summary>
		public static Type FindTypeWithBase(Type baseType, string typeName, bool ignoreCase) {
			return TypeHelper.FindTypeWithBase(baseType,
				typeName, ignoreCase, Assemblies.Game);
		}

		/// <summary>Returns the type with the specified name. Throws an exception
		/// if the type could not be found.</summary>
		public static Type FindType(string typeName, bool ignoreCase) {
			return TypeHelper.FindType(typeName, ignoreCase, Assemblies.Game);
		}
	}
}
