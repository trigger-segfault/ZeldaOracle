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

namespace ZeldaEditor.Undo {
	public class ActionChangeEvent : EditorAction {
		private enum ChangeEventModes {
			ChangeScript,
			DefineScript,
			UndefineScript
		}

		private IEventObject eventObject;
		private string eventName;
		private string oldCode;
		private string newCode;
		private ChangeEventModes mode;

		private ActionChangeEvent(IEventObject eventObject, Event evnt) {
			this.eventObject = eventObject;
			this.eventName = evnt.Name;
			ActionName = " '" + evnt.FinalReadableName + "' Event";
		}

		public static ActionChangeEvent CreateChangeEventAction(IEventObject eventObject, Event evnt, string oldCode, string newCode) {
			ActionChangeEvent action = new ActionChangeEvent(eventObject, evnt);
			action.ActionName = "Change" + action.ActionName;
			action.ActionIcon = EditorImages.Event;
			action.oldCode = oldCode;
			action.newCode = newCode;
			action.mode = ChangeEventModes.ChangeScript;
			return action;
		}

		public static ActionChangeEvent CreateDefineEventAction(IEventObject eventObject, Event evnt, string newCode) {
			ActionChangeEvent action = new ActionChangeEvent(eventObject, evnt);
			action.ActionName = "Define" + action.ActionName;
			action.ActionIcon = EditorImages.EventAdd;
			action.newCode = newCode;
			action.mode = ChangeEventModes.DefineScript;
			return action;
		}

		public static ActionChangeEvent CreateUndefineEventAction(IEventObject eventObject, Event evnt, string oldCode) {
			ActionChangeEvent action = new ActionChangeEvent(eventObject, evnt);
			action.ActionName = "Undefine" + action.ActionName;
			action.ActionIcon = EditorImages.EventDelete;
			action.oldCode = oldCode;
			action.mode = ChangeEventModes.UndefineScript;
			return action;
		}

		public override void PostExecute(EditorControl editorControl) {
			if (editorControl.PropertyGrid.EventObject == eventObject) {
				editorControl.PropertyGrid.Update();
			}
			if (mode != ChangeEventModes.ChangeScript)
				editorControl.NeedsNewEventCache = true;
		}

		public override void Undo(EditorControl editorControl) {
			switch (mode) {
			case ChangeEventModes.ChangeScript:
				Event.Script.Code = oldCode;
				break;
			case ChangeEventModes.DefineScript:
				Event.UndefineScript();
				break;
			case ChangeEventModes.UndefineScript:
				Event.DefineScript(oldCode);
				break;
			}
			if (editorControl.PropertyGrid.EventObject == eventObject) {
				editorControl.PropertyGrid.Update();
			}
			if (mode != ChangeEventModes.ChangeScript)
				editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			switch (mode) {
			case ChangeEventModes.ChangeScript:
				Event.Script.Code = newCode;
				break;
			case ChangeEventModes.DefineScript:
				Event.DefineScript(newCode);
				break;
			case ChangeEventModes.UndefineScript:
				Event.UndefineScript();
				break;
			}
			if (editorControl.PropertyGrid.EventObject == eventObject) {
				editorControl.PropertyGrid.Update();
			}
			if (mode != ChangeEventModes.ChangeScript)
				editorControl.NeedsNewEventCache = true;
		}

		private Event Event {
			get { return eventObject.Events.GetEvent(eventName); }
		}
		public IEventObject EventObject {
			get { return eventObject; }
		}
		public string EventName {
			get { return eventName; }
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
