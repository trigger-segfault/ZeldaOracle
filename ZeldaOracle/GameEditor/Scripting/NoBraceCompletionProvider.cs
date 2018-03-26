using Microsoft.CodeAnalysis;
using RoslynPad.Roslyn.AutomaticCompletion;

namespace ZeldaEditor.Scripting {
	/// <summary>Disables brace completion. This exists because the existing
	/// brace completion does not follow our coding conventions. That and
	/// the brace completing has some annoying quirks.</summary>
	public class NoBraceCompletionProvider : IBraceCompletionProvider {
		/// <summary>Never completes a brace.</summary>
		public bool TryComplete(Document document, int position) {
			return false;
		}
	}
}
