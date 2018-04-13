using ZeldaEditor.Control;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Undo {
	/// <summary>Delete a variable within the Variables Editor.</summary>
	public class ActionDeleteVariable : UndoAction<ObjectEditor> {
		private Variables variables;
		private Variable variable;

		public ActionDeleteVariable(Variable variable) {
			this.variable = variable;
			this.variables = variable.Variables;
			ActionName = "Delete Variable";
			ActionIcon = EditorImages.EventDelete;
		}

		public override void Redo(ObjectEditor context) {
			variables.RemoveVariable(variable.Name);
		}

		public override void Undo(ObjectEditor context) {
			variables.AddVariable(new Variable(variable));
		}
	}
}
