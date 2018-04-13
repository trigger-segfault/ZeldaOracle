using ZeldaEditor.Control;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Undo {
	/// <summary>Edit a variable within the Variables Editor.</summary>
	public class ActionEditVariable : UndoAction<ObjectEditor> {
		private Variables variables;
		private Variable originalValue;
		private Variable updatedValue;

		public ActionEditVariable(Variable originalValue,
			Variable updatedValue, Variables variables)
		{
			this.variables = variables;
			this.originalValue = originalValue;
			this.updatedValue = updatedValue;
			ActionName = "Edit Variable";
			ActionIcon = EditorImages.Edit;
		}

		public override void Redo(ObjectEditor context) {
			// Remove the original variable and add the updated variable
			variables.RemoveVariable(originalValue.Name);
			variables.AddVariable(new Variable(updatedValue));
		}

		public override void Undo(ObjectEditor context) {
			// Remove the updated variable and re-add the original variable
			variables.RemoveVariable(updatedValue.Name);
			variables.AddVariable(new Variable(originalValue));
		}
	}
}
