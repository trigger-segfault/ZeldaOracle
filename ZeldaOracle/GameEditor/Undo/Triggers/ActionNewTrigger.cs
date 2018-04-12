using ZeldaEditor.Control;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Undo {
	/// <summary>Create a new trigger within the Triggers Editor.</summary>
	public class ActionNewTrigger : UndoAction<ObjectEditor> {
		private TriggerCollection triggers;
		private string triggerName;

		public ActionNewTrigger(TriggerCollection triggers) {
			this.triggers = triggers;
			ActionName = "New Trigger";
			ActionIcon = EditorImages.ScriptAdd;
			
			// Create a unique trigger name
			triggerName = "";
			bool nameIsTaken = true;
			for (int index = 1; nameIsTaken; index++) {
				triggerName = string.Format("trigger_{0}", index);
				nameIsTaken = false;
				foreach (Trigger other in triggers) {
					if (triggerName == other.Name) {
						nameIsTaken = true;
						break;
					}
				}
			}
		}

		public override void Redo(ObjectEditor context) {
			triggers.AddTrigger(new Trigger() {
				Name = triggerName
			});
		}

		public override void Undo(ObjectEditor context) {
			triggers.RemoveTrigger(triggerName);
		}
	}
}
