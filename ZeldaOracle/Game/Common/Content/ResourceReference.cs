using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Content {
	/// <summary>An exception thrown when a resource cannot be located.
	/// Possibly due to game content inconsistencies.</summary>
	public class ResourceReferenceException : Exception {
		/// <summary>Constructs the resource reference exception.</summary>
		public ResourceReferenceException(Type type, string name)
			: base("Could not find resource of type '" + type.Name +
				  "' with the name '" + name + "'!") { }
	}

	/// <summary>A non-generic reference to a resource type and name.</summary>
	[Serializable]
	public class ResourceReference {
		/// <summary>Gets the type of the resource.</summary>
		public Type Type { get; }
		/// <summary>Gets the name of the resource.</summary>
		public string Name { get; }
		
		/// <summary>Creates a resource reference with the specified type.</summary>
		public ResourceReference(Type type, string name) {
			Type = type;
			Name = name;
		}

		/// <summary>Confirms that the resource exists in the database.</summary>
		public bool ConfirmResourceExists() {
			return Resources.Contains(Type, Name);
		}

		/// <summary>Looks up and returns the dereferenced resource.</summary>
		public object Dereference() {
			return Resources.Get(Type, Name);
		}
	}

	/// <summary>A generic reference to a resource type and name.</summary>
	[Serializable]
	public class ResourceReference<T> {
		/// <summary>Gets the type of the resource.</summary>
		public Type Type { get { return typeof(T); } }
		/// <summary>Gets the name of the resource.</summary>
		public string Name { get; }

		/// <summary>Creates a resource reference with the specified generic type.</summary>
		public ResourceReference(string name) {
			Name = name;
		}

		/// <summary>Confirms that the resource exists in the database.</summary>
		public bool ConfirmResourceExists() {
			return Resources.Contains<T>(Name);
		}

		/// <summary>Looks up and returns the dereferenced resource.</summary>
		public T Dereference() {
			return Resources.Get<T>(Name);
		}
	}
}
