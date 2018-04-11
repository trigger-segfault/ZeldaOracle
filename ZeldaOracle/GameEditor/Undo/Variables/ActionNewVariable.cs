using ZeldaEditor.Control;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Undo {
	/// <summary>Create a new variable within the Variables Editor.</summary>
	public class ActionNewVariable : UndoAction<ObjectEditor> {
		protected Variables variables;
		protected Variable variable;

		public ActionNewVariable(Variable variable, Variables variables) {
			this.variables = variables;
			this.variable = variable;
			ActionName = "New Variable";
			ActionIcon = EditorImages.LevelAdd;
		}

		public override void Redo(ObjectEditor context) {
			variables.AddVariable(new Variable(variable));
		}

		public override void Undo(ObjectEditor context) {
			variables.RemoveVariable(variable.Name);
		}
	}
}
