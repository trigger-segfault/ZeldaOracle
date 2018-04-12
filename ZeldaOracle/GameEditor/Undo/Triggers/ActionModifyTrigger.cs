using ZeldaEditor.Control;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Undo {
	/// <summary>Modifies a trigger's properties within the Triggers Editor.</summary>
	public class ActionModifyTrigger : UndoAction<ObjectEditor> {
		private TriggerCollection triggers;
		private Trigger oldState;
		private Trigger newState;

		public ActionModifyTrigger(Trigger trigger) {
			triggers = trigger.Collection;
			oldState = new Trigger(trigger);
			newState = new Trigger(trigger);
			ActionName = "Modify Trigger";
			ActionIcon = EditorImages.Edit;
		}

		public override void Redo(ObjectEditor context) {
			Trigger trigger = triggers.GetTrigger(oldState.Name);
			trigger.Name = newState.Name;
			trigger.Description = newState.Description;
			trigger.Script.Code = newState.Script.Code;
			trigger.InitiallyOn = newState.InitiallyOn;
			trigger.FireOnce = newState.FireOnce;
		}

		public override void Undo(ObjectEditor context) {
			Trigger trigger = triggers.GetTrigger(newState.Name);
			trigger.Name = oldState.Name;
			trigger.Description = oldState.Description;
			trigger.Script.Code = oldState.Script.Code;
			trigger.InitiallyOn = oldState.InitiallyOn;
			trigger.FireOnce = oldState.FireOnce;
		}

		public string TriggerName {
			get { return newState.Name; }
			set { newState.Name = value; }
		}

		public string ScriptCode {
			get { return newState.Script.Code; }
			set { newState.Script.Code = value; }
		}

		public string Description {
			get { return newState.Description; }
			set { newState.Description = value; }
		}

		public bool InitiallyOn {
			get { return newState.InitiallyOn; }
			set { newState.InitiallyOn = value; }
		}

		public bool FireOnce {
			get { return newState.FireOnce; }
			set { newState.FireOnce = value; }
		}
	}
}
