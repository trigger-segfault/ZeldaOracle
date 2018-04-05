using System;

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
}
