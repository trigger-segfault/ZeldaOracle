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
using ZeldaEditor.Util;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.TreeViews {
	
	public class ScriptTreeViewItem : IWorldTreeViewItem {
		protected Script script;
		
		public ScriptTreeViewItem(Script script, EditorControl editorControl) {
			this.script = script;
			if (script != null)
				Source = ((script.HasErrors && !editorControl.NoScriptErrors) ?
					EditorImages.ScriptError : ((script.HasWarnings && !editorControl.NoScriptWarnings) ?
					EditorImages.ScriptWarning : EditorImages.Script));
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
			MessageBoxResult result = TriggerMessageBox.Show(editorControl.EditorWindow, MessageIcon.Info,
				"You are about to delete the script '" + script.ID + "'. This will be permanent. Continue?", "Confirm",
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
			Script duplicate = new Script(script);
			duplicate.ID = "";
			string newName = RenameWindow.Show(Window.GetWindow(this), editorControl.World, duplicate);
			if (newName != null) {
				duplicate.ID = newName;
				editorControl.AddScript(duplicate);
				editorControl.EditorWindow.TreeViewWorld.RefreshScripts(true, false);
			}
		}

		public override IIDObject IDObject { get { return script; } }
	}
}
