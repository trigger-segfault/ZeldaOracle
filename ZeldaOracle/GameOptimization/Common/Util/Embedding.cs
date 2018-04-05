using System;
using System.IO;
using System.Reflection;

namespace ZeldaOracle.Common.Util {
	/// <summary>A static helper class for loading embedded resource streams.</summary>
	public static class Embedding {

		/// <summary>Loads the specified manifest resource from the entry assembly.</summary>
		public static Stream Get(params string[] paths) {
			return Get(Assembly.GetEntryAssembly(), paths);
		}

		/// <summary>Loads the specified manifest resource from the specified assembly.</summary>
		public static Stream Get(Assembly assembly, params string[] paths) {
			string path = string.Join(".", paths);
			return assembly.GetManifestResourceStream(path);
		}

		/// <summary>Loads the specified manifest resource, scoped by the namespace of
		/// the specified type, from this assembly.</summary>
		public static Stream Get(Type type, string name) {
			return type.Assembly.GetManifestResourceStream(type, name);
		}
	}
}
