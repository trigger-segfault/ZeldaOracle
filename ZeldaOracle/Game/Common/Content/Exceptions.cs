using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Content {
	/// <summary>A fatal exception that's thrown during the games LoadContent function.
	/// This is used to end the game before updates start happening.</summary>
	public class LoadContentException : Exception {
		/// <summary>Constructs the load content exception.</summary>
		public LoadContentException(string message, Exception innerException = null)
			: base(message, innerException)
		{
		}

		/// <summary>Prints the exception message.</summary>
		public virtual void PrintMessage() {
			Console.WriteLine(Message);
		}
	}

	/// <summary>An exception thrown when a resource cannot be located.
	/// Possibly due to game content inconsistencies.</summary>
	public class ResourceReferenceException : Exception {
		/// <summary>Constructs the resource reference exception.</summary>
		public ResourceReferenceException(Type type, string name)
			: base("Could not find resource of type '" + type.Name +
				  "' with the name '" + name + "'!") { }
	}
}
