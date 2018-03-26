using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ZeldaOracle.Common.Util;

namespace ZeldaEditor.Scripting {
	/// <summary>Determines folds for a script in the editor.</summary>
	public class ScriptFoldingStrategy {

		//-----------------------------------------------------------------------------
		// Classes
		//-----------------------------------------------------------------------------

		/// <summary>Holds information about the start of a fold in a script line.</summary>
		private class ScriptFoldStart : NewFolding {
			public int StartLine { get; set; }
		}

		
		//-----------------------------------------------------------------------------
		// Folding
		//-----------------------------------------------------------------------------

		/// <summary>Create <see cref="NewFolding"/>s for the specified document and
		/// updates the folding manager with them.</summary>
		public void UpdateFoldings(FoldingManager manager, TextDocument document,
			int scriptStart)
		{
			IEnumerable<NewFolding> folding = CreateNewFoldings(document, scriptStart);
			manager.UpdateFoldings(folding, scriptStart);
		}

		/// <summary>Create <see cref="NewFolding"/>s for the specified document.</summary>
		public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document,
			int scriptStart)
		{
			List<NewFolding> foldMarkers = new List<NewFolding>();

			// The initial members/parameters folding
			NewFolding scriptFold = new NewFolding() {
				StartOffset = 0,
				EndOffset = scriptStart - Environment.NewLine.Length,
				Name = " Members/Parameters ",
				DefaultClosed = true
			};
			foldMarkers.Add(scriptFold);

			string text = document.Text;

			Stack<ScriptFoldStart> stack = new Stack<ScriptFoldStart>();
			int lineNumber = 0;
			int lineStart = 0;
			int lineWhiteSpaceEnd = -1;
			for (int i = 0; i < text.Length; i++) {
				char c = text[i];
				if (lineWhiteSpaceEnd == -1 && !char.IsWhiteSpace(c)) {
					lineWhiteSpaceEnd = i;
				}
				if (c == '\n') {
					lineNumber++;
					lineStart = i;
					lineWhiteSpaceEnd = -1;
				}
				else if (c == '{') {
					if (!stack.Any() || stack.Peek().StartLine != lineNumber) {
						ScriptFoldStart fold = new ScriptFoldStart() {
							StartOffset = lineWhiteSpaceEnd,
							StartLine = lineNumber,
							Name = text.Substring(lineWhiteSpaceEnd,
								i - lineWhiteSpaceEnd).Surround(' ', true)
						};
						stack.Push(fold);
					}
				}
				else if (c == '}') {
					if (stack.Any()) {
						ScriptFoldStart fold = stack.Pop();
						fold.EndOffset = i + 1; // + 1 to include the ending brace
						foldMarkers.Add(fold);
					}
				}
			}

			foldMarkers.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
			return foldMarkers;
		}
	}
}
