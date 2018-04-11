using ZeldaEditor.Control;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Undo {
	/// <summary>Paste a variable within the Variables Editor.</summary>
	public class ActionPasteVariable : ActionNewVariable {
		public ActionPasteVariable(Variable variable, Variables variables) :
			base(variable, variables)
		{
			ActionName = "Paste Variable";
			ActionIcon = EditorImages.Paste;

			string name = variable.Name;

			// Create a unique name by adding a '_copy' suffix
			if (variables.Contains(name))
				name = variable.Name + "_copy";

			// If there's still a name conflict, then add a number suffix
			for (int number = 2; variables.Contains(name); number++)
				name = string.Format("{0}_copy_{1}", variable.Name, number);

			this.variable.Name = name;
		}
	}
}
