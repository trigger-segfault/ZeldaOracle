using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;

namespace ZeldaEditor.Undo {
	public class ActionChangeScript : EditorAction {
		private enum ChangeScriptModes {
			ChangeScript,
			DefineScript,
			UndefineScript,
			DuplicateScript
		}

		private string scriptName;
		private string oldCode;
		private string newCode;
		private ChangeScriptModes mode;

		private ActionChangeScript(string scriptName) {
			this.scriptName = scriptName;
			ActionName = " '" + scriptName + "' Script";
		}

		public static ActionChangeScript CreateChangeScriptAction(string scriptName, string oldCode, string newCode) {
			ActionChangeScript action = new ActionChangeScript(scriptName);
			action.ActionName = "Change" + action.ActionName;
			action.ActionIcon = EditorImages.Script;
			action.oldCode = oldCode;
			action.newCode = newCode;
			action.mode = ChangeScriptModes.ChangeScript;
			return action;
		}

		public static ActionChangeScript CreateDefineScriptAction(string scriptName, string newCode) {
			ActionChangeScript action = new ActionChangeScript(scriptName);
			action.ActionName = "Create " + action.ActionName;
			action.ActionIcon = EditorImages.ScriptAdd;
			action.newCode = newCode;
			action.mode = ChangeScriptModes.DefineScript;
			return action;
		}

		public static ActionChangeScript CreateUndefineScriptAction(string scriptName, string oldCode) {
			ActionChangeScript action = new ActionChangeScript(scriptName);
			action.ActionName = "Delete" + action.ActionName;
			action.ActionIcon = EditorImages.ScriptDelete;
			action.oldCode = oldCode;
			action.mode = ChangeScriptModes.UndefineScript;
			return action;
		}

		public static ActionChangeScript CreateDuplicateScriptAction(string scriptName, string newCode) {
			ActionChangeScript action = new ActionChangeScript(scriptName);
			action.ActionName = "Duplicate" + action.ActionName;
			action.ActionIcon = EditorImages.ScriptDuplicate;
			action.newCode = newCode;
			action.mode = ChangeScriptModes.DuplicateScript;
			return action;
		}

		public override void PostExecute(EditorControl editorControl) {
			switch (mode) {
			case ChangeScriptModes.DefineScript:
			case ChangeScriptModes.DuplicateScript:
				editorControl.ScriptRenamed(null, scriptName);
				break;
			case ChangeScriptModes.UndefineScript:
				editorControl.ScriptRenamed(scriptName, null);
				break;
			}
			if (mode != ChangeScriptModes.ChangeScript) {
				editorControl.EditorWindow.WorldTreeView.RefreshScripts(true, false);
			}
		}

		public override void Undo(EditorControl editorControl) {
			switch (mode) {
			case ChangeScriptModes.ChangeScript:
				editorControl.World.GetScript(scriptName).Code = oldCode;
				break;
			case ChangeScriptModes.DefineScript:
			case ChangeScriptModes.DuplicateScript:
				editorControl.World.RemoveScript(scriptName);
				editorControl.ScriptRenamed(scriptName, null);
				break;
			case ChangeScriptModes.UndefineScript:
				Script script = new Script();
				script.ID = scriptName;
				script.Code = oldCode;
				editorControl.World.AddScript(script);
				editorControl.ScriptRenamed(null, scriptName);
				break;
			}
			if (mode != ChangeScriptModes.ChangeScript) {
				editorControl.EditorWindow.WorldTreeView.RefreshScripts(true, false);
			}
		}

		public override void Redo(EditorControl editorControl) {
			switch (mode) {
			case ChangeScriptModes.ChangeScript:
				editorControl.World.GetScript(scriptName).Code = newCode;
				break;
			case ChangeScriptModes.DefineScript:
			case ChangeScriptModes.DuplicateScript:
				Script script = new Script();
				script.ID = scriptName;
				script.Code = newCode;
				editorControl.World.AddScript(script);
				editorControl.ScriptRenamed(null, scriptName);
				break;
			case ChangeScriptModes.UndefineScript:
				editorControl.World.RemoveScript(scriptName);
				editorControl.ScriptRenamed(scriptName, null);
				break;
			}
			if (mode != ChangeScriptModes.ChangeScript) {
				editorControl.EditorWindow.WorldTreeView.RefreshScripts(true, false);
			}
		}
		
		public string OldCode {
			get { return oldCode; }
			set { oldCode = value; }
		}
		public string NewCode {
			get { return newCode; }
			set { newCode = value; }
		}

		public override bool IgnoreAction { get { return newCode == oldCode; } }
	}
}
