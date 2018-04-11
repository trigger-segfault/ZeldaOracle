using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZeldaEditor.Control;
//using ZeldaEditor.PropertiesEditor.CustomEditors;
using ZeldaEditor.Scripting;
using ZeldaEditor.Undo;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaWpf.Windows;

namespace ZeldaEditor.TreeViews {
	
	public class ScriptTreeViewItem : IWorldTreeViewItem {
		protected Script script;
		
		public ScriptTreeViewItem(Script script, EditorControl editorControl) {
			this.script = script;
			Source = EditorImages.Script;
			if (script != null) {
				if (script.HasErrors && !editorControl.NoScriptErrors)
					Source = EditorImages.ScriptError;
				else if (script.HasWarnings && !editorControl.NoScriptWarnings)
					Source = EditorImages.ScriptWarning;
			}
			Header				= script.ID;
			Tag					= "script";
		}

		public override void Open(EditorControl editorControl) {
			string scriptName = script.ID;
			string oldCode = script.Code;
			if (ScriptEditor.ShowRegularEditor(editorControl.EditorWindow, script, editorControl, false)) {
				bool deleted = editorControl.World.GetScript(scriptName) == null;
				EditorAction action;
				if (deleted)
					action = ActionChangeScript.CreateUndefineScriptAction(scriptName, oldCode);
				else
					action = ActionChangeScript.CreateChangeScriptAction(scriptName, oldCode, script.Code);
				editorControl.PushAction(action, ActionExecution.PostExecute);
			}
		}

		public override void Delete(EditorControl editorControl) {
			MessageBoxResult result = TriggerMessageBox.Show(editorControl.EditorWindow, MessageIcon.Warning,
				"Are you sure you want to delete the script '" + script.ID + "'?", "Confirm",
				MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) {
				EditorAction action = ActionChangeScript.CreateUndefineScriptAction(script.ID, script.Code);
				editorControl.PushAction(action, ActionExecution.Execute);
			}
		}

		public override void Rename(EditorControl editorControl, string name) {
			editorControl.World.RenameScript(script, name);
			Header = script.ID;
			editorControl.IsModified = true;
		}

		public override void Duplicate(EditorControl editorControl) {
			// Dummy script for the rename window
			Script duplicate = new Script();
			string newName = RenameWindow.Show(Window.GetWindow(this), editorControl.World, duplicate);
			if (newName != null) {
				EditorAction action = ActionChangeScript.CreateDuplicateScriptAction(newName, script.Code);
				editorControl.PushAction(action, ActionExecution.Execute);
			}
		}

		public override IIDObject IDObject { get { return script; } }
	}
}
