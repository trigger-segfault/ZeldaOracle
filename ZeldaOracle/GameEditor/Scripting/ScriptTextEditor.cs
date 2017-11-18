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
			// Create the parameters string.
			string parametersString = "";
			for (int i = 0; i < Script.Parameters.Count; i++) {
				if (i > 0)
					parametersString += ", ";
				parametersString += Script.Parameters[i].Type + " " + Script.Parameters[i].Name;
			}

			/*string prefix =
				"using System; " +
				"using System.Collections.Generic; " +
				"using System.Linq; " +
				"using System.Text; " +
				"using ZeldaAPI; " +
				"namespace ZeldaAPI.CustomScripts {" +
					"public class CustomScript2 : CustomScriptBase {" +
						"public void RunScript(" + parametersString + ") {";
			string postfix = 
						"}" +
					"}" +
				"}";*/
			int scriptOffset;
			string code = EditorControl.World.ScriptManager.CreateCode(Script, Document.Text, out scriptOffset);
			var doc = new ReadOnlyDocument(new StringTextSource(code), Document.FileName);
			offset = CaretOffset + scriptOffset;
			return doc;
		}

	}
}
