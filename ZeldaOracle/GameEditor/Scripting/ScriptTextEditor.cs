using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.CodeCompletion;
using ICSharpCode.NRefactory.Editor;
using System.Windows.Media;
using ZeldaEditor.Control;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Scripting {

	public class ScriptTextEditor : CodeTextEditor {

		private EditorControl editorControl;
		private Trigger trigger;
		private Script script;
		private ScriptCodeGenerator codeGenerator;
		private string wrappedCodeOpening;
		private string wrappedCodeClosing;
		private bool needsRegeneration;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptTextEditor() {
			Document.FileName = "dummyFileName.cs";
			Text = "";
			TextArea.Margin = new System.Windows.Thickness(4, 4, 0, 4);
			TextArea.TextView.Options.AllowScrollBelowDocument = true;
			SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");

			// Selection Style
			TextArea.SelectionCornerRadius = 0;
			TextArea.SelectionBorder = null;
			FontFamily = new FontFamily("Lucida Console");
			FontSize = 12.667;

			editorControl = null;
			codeGenerator = new ScriptCodeGenerator();
			wrappedCodeOpening = "";
			wrappedCodeClosing = "";
			needsRegeneration = true;
		}

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the document used for code completion.</summary>
		protected override IDocument GetCompletionDocument(out int offset) {
			string wrappedCode = "";

			// Generate the code
			if (needsRegeneration) {
				GeneratedScriptCode result = null;

				if (trigger != null)
					result = codeGenerator.GenerateTestCode(trigger, Document.Text);
				else if (script != null)
					result = codeGenerator.GenerateTestCode(script, Document.Text);

				if (result != null) {
					int beginOffset = result.ScriptInfo[script].Offset;
					int endOffset = beginOffset + Document.Text.Length;
					wrappedCodeOpening = result.Code.Substring(0, beginOffset);
					wrappedCodeClosing = result.Code.Substring(endOffset);
					wrappedCode = result.Code;
					needsRegeneration = false;
				}
				else {
					wrappedCode = "";
					wrappedCodeOpening = "";
					wrappedCodeClosing = "";
				}
			}
			else {
				// Use the cached opening and closing text to create the full
				// wrapped code
				wrappedCode = wrappedCodeOpening + Document.Text + wrappedCodeClosing;
			}

			// Create the document
			ReadOnlyDocument doc = new ReadOnlyDocument(
				new StringTextSource(wrappedCode), Document.FileName);
			offset = CaretOffset + wrappedCodeOpening.Length;
			return doc;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorControl EditorControl {
			get { return editorControl; }
			set {
				editorControl = value;
				codeGenerator.World = editorControl.World;
				needsRegeneration = true;
			}
		}

		public Script Script {
			get { return script; }
			set {
				script = value;
				trigger = null;
				needsRegeneration = true;
			}
		}

		public Trigger Trigger {
			get { return trigger; }
			set {
				trigger = value;
				script = trigger.Script;
				needsRegeneration = true;
			}
		}
	}
}
