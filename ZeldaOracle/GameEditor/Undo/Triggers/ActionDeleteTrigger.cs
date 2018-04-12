using ZeldaEditor.Control;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Undo {
	/// <summary>Delete a trigger within the Triggers Editor.</summary>
	public class ActionDeleteTrigger : UndoAction<ObjectEditor> {
		private TriggerCollection triggers;
		private Trigger trigger;

		public ActionDeleteTrigger(Trigger trigger) {
			this.trigger = new Trigger(trigger);
			triggers = trigger.Collection;
			ActionName = "Delete Trigger";
			ActionIcon = EditorImages.ScriptDelete;
		}

		public override void Redo(ObjectEditor context) {
			triggers.RemoveTrigger(trigger.Name);
		}

		public override void Undo(ObjectEditor context) {
			triggers.AddTrigger(new Trigger(trigger));
		}
	}
}
