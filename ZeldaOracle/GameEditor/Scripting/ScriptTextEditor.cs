using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.CodeCompletion;
using ICSharpCode.NRefactory.Editor;
using ZeldaEditor.Control;
using ZeldaOracle.Game.Control.Scripting;

namespace ZeldaEditor.Scripting {
	public class ScriptTextEditor : CodeTextEditor {

		public Script Script { get; set; } = null;
		public EditorControl EditorControl { get; set; }

		protected override IDocument GetCompletionDocument(out int offset) {
			int scriptOffset;
			string code = EditorControl.World.ScriptManager.CreateTestScriptCode(Script, Document.Text, out scriptOffset);
			var doc = new ReadOnlyDocument(new StringTextSource(code), Document.FileName);
			offset = CaretOffset + scriptOffset;
			return doc;
		}

	}
}
