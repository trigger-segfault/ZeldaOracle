using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Control.Scripting;

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
			ScriptRoslynInfo info)
		{
			IEnumerable<NewFolding> folding = CreateNewFoldings(document, info);
			manager.UpdateFoldings(folding, -1);
		}

		/// <summary>Create <see cref="NewFolding"/>s for the specified document.</summary>
		public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document,
			ScriptRoslynInfo info)
		{
			List<NewFolding> foldMarkers = new List<NewFolding>();

			if (info.MethodStart != 0) {
				NewFolding preMethodFold = new NewFolding() {
					StartOffset = 0,
					EndOffset = info.MethodStart,
					Name = "",
					DefaultClosed = true,
					IsHidden = true,
				};
				foldMarkers.Add(preMethodFold);
			}
			if (info.MethodStart != info.MethodEnd) {
				NewFolding methodFold = new NewFolding() {
					StartOffset = info.MethodStart,
					EndOffset = info.MethodEnd,
					Name = " Method Declaration ",
					DefaultClosed = true,
					IsHidden = false,
				};
				foldMarkers.Add(methodFold);
			}
			if (info.MethodEnd != info.ScriptStart) {
				NewFolding postMethodFold = new NewFolding() {
					StartOffset = info.MethodEnd,
					EndOffset = info.ScriptStart - 1, // -1 to exclude new line
					Name = "",
					DefaultClosed = true,
					IsHidden = true,
				};
				foldMarkers.Add(postMethodFold);
			}
			
			string text = document.Text;

			Stack<ScriptFoldStart> stack = new Stack<ScriptFoldStart>();
			int lineNumber = 0;
			int lineStart = 0;
			int lineWhiteSpaceEnd = -1;
			for (int i = info.ScriptStart; i < text.Length - info.EndLength; i++) {
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
					if (!stack.Any()) {
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
						if (!stack.Any() || stack.Peek().StartLine != fold.StartLine)
							foldMarkers.Add(fold);
					}
				}
			}

			if (info.EndLength > 0) {
				NewFolding endFold = new NewFolding() {
					StartOffset = text.Length - info.EndLength,
					EndOffset = text.Length,
					Name = "",
					DefaultClosed = true,
					IsHidden = true,
				};
				foldMarkers.Add(endFold);
			}

			foldMarkers.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
			return foldMarkers;
		}
	}
}
