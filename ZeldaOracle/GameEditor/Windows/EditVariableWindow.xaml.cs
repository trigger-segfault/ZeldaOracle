using System;
using System.Linq;
using System.Windows;
using ZeldaOracle.Common.Scripting;
using ZeldaEditor.Control;
using ZeldaEditor.Undo;

namespace ZeldaEditor.Windows {

		public class ActionDeleteVariable : EditorAction {
			private Variables variables;
			private Variable variable;

			public ActionDeleteVariable(Variable variable) {
				this.variable = variable;
				this.variables = variable.Variables;
				ActionName = "Delete Variable";
				ActionIcon = EditorImages.LevelAdd;
			}
			public override void Redo(EditorControl editorControl) {
				variables.RemoveVariable(variable.Name);
			}
			public override void Undo(EditorControl editorControl) {
				variables.AddVariable(variable);
			}
		}
	/// <summary>
	/// Dialog window used to edit a variable or add a new variable.
	/// Interaction logic for EditVariableWindow.xaml
	/// </summary>
	public partial class EditVariableWindow : Window {

		public class ActionNewVariable : EditorAction {
			private Variables variables;
			private Variable variable;

			public ActionNewVariable(Variable variable, Variables variables) {
				this.variables = variables;
				this.variable = variable;
				ActionName = "New Variable";
				ActionIcon = EditorImages.LevelAdd;
			}
			public override void Redo(EditorControl editorControl) {
				variables.AddVariable(variable);
			}
			public override void Undo(EditorControl editorControl) {
				variables.RemoveVariable(variable.Name);
			}
		}

		public class ActionEditVariable : EditorAction {
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
				ActionIcon = EditorImages.LevelAdd;
			}
			public override void Redo(EditorControl editorControl) {
				// Remove the original variable and add the updated variable
				variables.RemoveVariable(originalValue.Name);
				variables.AddVariable(updatedValue);
			}
			public override void Undo(EditorControl editorControl) {
				// Remove the updated variable and re-add the original variable
				variables.RemoveVariable(updatedValue.Name);
				variables.AddVariable(originalValue);
			}
		}

		private Variable variable;
		private Variables variables;
		private bool isNewVariable;
		private EditorAction action;

		public string VariableName { get; set; }
		public VarType VariableType { get; set; }
		public ListType VariableListType { get; set; }
		public int VariableListCount { get; set; }


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EditVariableWindow(Variable variable, Variables variables, bool isNewVariable) {
			this.variable = variable;
			this.variables = variables;
			this.isNewVariable = isNewVariable;

			InitializeComponent();

			if (isNewVariable)
				Title = "New Variable";
			else
				Title = "Edit Variable";

			// Set the source of the combo boxes to Enum values
			comboBoxVariableType.ItemsSource =
				Enum.GetValues(typeof(VarType)).Cast<VarType>().Where(
					x => x != VarType.Custom);
			comboBoxListType.ItemsSource =
				Enum.GetValues(typeof(ListType)).Cast<ListType>();
			
			if (variable == null) {
				textBoxVariableName.Text = "";
				comboBoxVariableType.SelectedItem = VarType.Integer;
				comboBoxListType.SelectedItem = ListType.Single;
				spinnerListSize.Value = 1;
			}
			else {
				textBoxVariableName.Text = variable.Name;
				comboBoxVariableType.SelectedItem = variable.VarType;
				comboBoxListType.SelectedItem = variable.ListType;
				spinnerListSize.Value = variable.Count;
			}

			textBoxInitialValue.Text = "";
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		public static EditorAction ShowAddVariable(Window owner, Variables variables) {
			EditVariableWindow window = new EditVariableWindow(
				null, variables, true);
			window.Owner = owner;
			bool? result = window.ShowDialog();
			if (result.HasValue && result.Value)
				return window.action;
			return null;
		}

		public static EditorAction ShowEditVariable(Window owner, Variable variable) {
			EditVariableWindow window = new EditVariableWindow(
				variable, variable.Variables, false);
			window.Owner = owner;
			bool? result = window.ShowDialog();
			if (result.HasValue && result.Value)
				return window.action;
			return null;
		}

		private void OnAdd(object sender, RoutedEventArgs e) {
			// Grab the variable info
			string name = textBoxVariableName.Text;
			VarType variableType = (VarType) comboBoxVariableType.SelectedItem;
			ListType listType = (ListType) comboBoxListType.SelectedItem;
			int listCount = 1;
			if (spinnerListSize.Value.HasValue)
				listCount = spinnerListSize.Value.Value;
			
			// Validate the variable name and verify it is unique
			if (!Variable.IsValidName(name)) {
				MessageBox.Show(
					messageBoxText: string.Format(
						"'{0}' is not a valid variable name.", name),
					caption: "Error");
			}
			else if ((isNewVariable && variables.Contains(name)) ||
				(!isNewVariable && variables.Contains(name) &&
					variables.GetVariable(name) != variable))
			{
				MessageBox.Show(
					messageBoxText: string.Format(
						"A variable named '{0}' already exists.", name),
					caption: "Error");
			}
			else {
				Variable newVariable = new Variable(
					name, variableType, listType, listCount);

				if (isNewVariable)
					action = new ActionNewVariable(newVariable, variables);
				else
					action = new ActionEditVariable(variable, newVariable, variables);

				DialogResult = true;
				Close();
			}
		}
	}
}
