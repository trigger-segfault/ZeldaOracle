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

	public class EventTreeViewItem : ScriptTreeViewItem {
		private Event evnt;

		public EventTreeViewItem(Event evnt, EditorControl editorControl) : base(evnt.Script, editorControl) {
			this.evnt = evnt;
			Source = EditorImages.Event;
			if (script != null) {
				if (script.HasErrors && !editorControl.NoScriptErrors)
					Source = EditorImages.EventError;
				else if (script.HasWarnings && !editorControl.NoScriptWarnings)
					Source = EditorImages.EventWarning;
			}
			Header              = "'" + evnt.FinalReadableName + "' Event";// (" + evnt.InternalScriptID + ")";
			Tag                 = "event";
			ToolTip = evnt.InternalScriptID;
		}

		public override void Open(EditorControl editorControl) {
			if (!evnt.IsDefined)
				return;
			string scriptName = script.ID;
			string oldCode = script.Code;
			if (ScriptEditor.ShowCustomEditor(editorControl.EditorWindow, script, editorControl, false)) {
				bool deleted = string.IsNullOrEmpty(script.Code);
				EditorAction action;
				if (deleted) {
					action = ActionChangeEvent.CreateUndefineEventAction(evnt.Events.EventObject, evnt, oldCode);
					evnt.UndefineScript();
				}
				else {
					action = ActionChangeEvent.CreateChangeEventAction(evnt.Events.EventObject, evnt, oldCode, script.Code);
				}
				editorControl.PushAction(action, ActionExecution.PostExecute);
			}
		}

		public override void Delete(EditorControl editorControl) {
			if (!evnt.IsDefined)
				return;
			MessageBoxResult result = TriggerMessageBox.Show(editorControl.EditorWindow, MessageIcon.Info,
				"You are about to delete the script '" + script.ID + "'. This will be permanent. Continue?", "Confirm",
				MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) {
				EditorAction action = ActionChangeEvent.CreateUndefineEventAction(evnt.Events.EventObject, evnt, script.Code);
				editorControl.PushAction(action, ActionExecution.PostExecute);
				evnt.UndefineScript();
			}
		}

		public override void Rename(EditorControl editorControl, string name) {
			
		}

		public override void Duplicate(EditorControl editorControl) {
			
		}

		public Event Event {
			get { return evnt; }
		}

		public override IIDObject IDObject { get { return script; } }
	}
}
