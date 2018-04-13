using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Util {
	/// <summary>A static class for exceptions revolving around exceptions.</summary>
	public static class ExceptionExtensions {

		/// <summary>Gets all inner exceptions within this exception.</summary>
		public static IEnumerable<Exception> GetInnerExceptions(this Exception ex) {
			ex = ex.InnerException;
			while (ex != null) {
				yield return ex;
				ex = ex.InnerException;
			}
		}

		/// <summary>Returns a string with every exception message and inner message.</summary>
		public static string MessageWithInner(this Exception ex) {
			StringBuilder builder = new StringBuilder();
			builder.Append(ex.GetType().Name);
			builder.Append(": ");
			builder.Append(ex.Message);
			
			IEnumerable<Exception> innerExceptions = ex.GetInnerExceptions();

			// Label the beginning of inner exceptions
			if (innerExceptions.Any()) {
				builder.AppendLine();
				builder.AppendLine();
				builder.Append("Inner Exceptions: ");
				builder.AppendLine();
			}

			// Append each inner exception
			foreach (Exception inner in innerExceptions) {
				builder.AppendLine();
				builder.Append(ex.GetType().Name);
				builder.Append(": ");
				builder.Append(inner.Message);
			}
			return builder.ToString();
		}

		/// <summary>Converts the exception to a string along with every inner exception.</summary>
		public static string ToStringWithInner(this Exception ex) {
			StringBuilder builder = new StringBuilder();
			builder.Append(ex.ToString());

			// Append each inner exception
			foreach (Exception inner in ex.GetInnerExceptions()) {
				builder.AppendLine();
				builder.AppendLine("----------------------------------------------------------------");
				builder.Append("Inner: ");
				builder.Append(inner.ToString());
			}
			return builder.ToString();
		}
	}
}
