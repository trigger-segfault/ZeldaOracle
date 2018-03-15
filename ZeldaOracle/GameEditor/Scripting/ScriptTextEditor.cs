using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.CodeCompletion;
using ICSharpCode.NRefactory.Editor;
using System.Windows;
using System.Windows.Media;
using ZeldaEditor.Control;
using ZeldaOracle.Game.Control.Scripting;

namespace ZeldaEditor.Scripting {
	public class ScriptTextEditor : CodeTextEditor {

		public Script Script { get; set; } = null;
		public EditorControl EditorControl { get; set; }

		public ScriptTextEditor() {
			Document.FileName = "dummyFileName.cs";
			Text = "";
			TextArea.Margin = new Thickness(4, 4, 0, 4);
			TextArea.TextView.Options.AllowScrollBelowDocument = true;
			SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");

			// Selection Style
			TextArea.SelectionCornerRadius = 0;
			TextArea.SelectionBorder = null;
			FontFamily = new FontFamily("Lucida Console");
			FontSize = 12.667;
		}
		
		/// <summary>Gets the document used for code completion.</summary>
		protected override IDocument GetCompletionDocument(out int offset) {
			int scriptOffset;
			string code = EditorControl.World.ScriptManager.CreateTestScriptCode(
				Script, Document.Text, out scriptOffset);
			ReadOnlyDocument doc = new ReadOnlyDocument(
				new StringTextSource(code), Document.FileName);
			offset = CaretOffset + scriptOffset;
			return doc;
		}
	}
}
