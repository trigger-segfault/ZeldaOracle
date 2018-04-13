using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;

namespace ZeldaWpf.Controls.TextEditing {
	/// <summary>An indentation strategy to disable auto line indentation.</summary>
	public class NoIndentationStrategy : IIndentationStrategy {

		//-----------------------------------------------------------------------------
		// IIndentationStrategy Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Sets the indentation for the specified line. Usualy this
		/// is constructed from the indentation of the previous line.</summary>
		public void IndentLine(TextDocument document, DocumentLine line) {
		}

		/// <summary>Reindents a set of lines.</summary>
		public void IndentLines(TextDocument document, int beginLine, int endLine) {
		}
	}
}
