using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZeldaEditor.Undo;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Controls {

	/// <summary>
	/// Interaction logic for ObjectVariableEditor.xaml
	/// </summary>
	public partial class ObjectVariableEditor : UserControl {

		/// <summary>Used to provide the displayed text for a variable.</summary>
		private class VariableItem {

			public VariableItem(Variable variable) {
				Variable = variable;
			}

			public Variable Variable { get; }

			/// <summary>Gets the variable's name.</summary>
			public string Name {
				get { return Variable.Name; }
			}

			/// <summary>Gets the string describing the variable's type.</summary>
			public string Type {
				get {
					// Example: "Integer List [3]"
					string text = Variable.VarType.ToString();
					if (Variable.ListType == ListType.Array)
						text += string.Format(" Array [{0}]", Variable.Count);
					else if (Variable.ListType == ListType.List)
						text += string.Format(" List [{0}]", Variable.Count);
					return text;
				}
			}

			/// <summary>Gets the string describing the variable's value.</summary>
			public string Value {
				get { return Variable.GetValueString(); }
			}
		}
		
		/// <summary>The variable collection that is being edited.</summary>
		private Variables variables;
		/// <summary>Name of the VariableItem property used for sorting the variable
		/// list.</summary>
		private string sortedPropertyName;
		/// <summary>The sort direction of the variable list.</summary>
		private ListSortDirection sortDirection;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ObjectVariableEditor() {
			InitializeComponent();

			// Initially sort by name in ascending order
			sortedPropertyName = "Name";
			sortDirection = ListSortDirection.Ascending;
			listView.Items.SortDescriptions.Add(
				new SortDescription(sortedPropertyName, sortDirection));

			variables = null;
			RefreshVariableList();
		}


		//-----------------------------------------------------------------------------
		// Variable Manipulation
		//-----------------------------------------------------------------------------

		/// <summary>Delete a variable from its collection.</summary>
		private EditorAction DeleteVariable(Variable variable) {
			EditorAction action = new ActionDeleteVariable(variable);
			action.Execute(null);
			RefreshVariableList();
			return action;
		}

		/// <summary>Open the edit variable window for the given variable.</summary>
		private EditorAction EditVariable(Variable variable) {
			EditorAction action = EditVariableWindow.ShowEditVariable(
				Window.GetWindow(this), SelectedVariable);
			if (action != null)
				action.Execute(null);
			//editorControl.PushAction(action, ActionExecution.Execute);
			RefreshVariableList();
			return action;
		}

		/// <summary>Refresh the list view of variables.</summary>
		private void RefreshVariableList() {
			if (variables != null) {
				List<VariableItem> items = new List<VariableItem>();
				foreach (Variable variable in variables)
					items.Add(new VariableItem(variable));
				listView.ItemsSource = items;
			}
			else {
				listView.ItemsSource = null;
			}
		}


		//-----------------------------------------------------------------------------
		// Command Can Execute
		//-----------------------------------------------------------------------------
		
		/// <summary>Returns true if the editing variables is not null.</summary>
		private void CanExecuteAnyCommand(
			object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (listView != null && variables != null);
		}
		
		/// <summary>Returns true if a variable is selected.</summary>
		private void CanExecuteVariableCommand(
			object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (listView != null && variables != null &&
				listView.SelectedItem != null);
		}
		
		
		//-----------------------------------------------------------------------------
		// Command Callbacks
		//-----------------------------------------------------------------------------

		private void OnCut(object sender, ExecutedRoutedEventArgs e) {

		}

		private void OnCopy(object sender, ExecutedRoutedEventArgs e) {

		}

		private void OnPaste(object sender, ExecutedRoutedEventArgs e) {

		}

		private void OnUndo(object sender, ExecutedRoutedEventArgs e) {

		}

		private void OnRedo(object sender, ExecutedRoutedEventArgs e) {

		}

		private void OnNewVariable(object sender, ExecutedRoutedEventArgs e) {
			EditorAction action = EditVariableWindow.ShowAddVariable(
				Window.GetWindow(this), variables);
			if (action != null)
				action.Execute(null);
			//editorControl.PushAction(action, ActionExecution.Execute);
			RefreshVariableList();
			// TODO: Select the new variable
		}

		private void OnEditVariable(object sender, ExecutedRoutedEventArgs e) {
			EditVariable(SelectedVariable);
		}

		private void OnDeleteVariable(object sender, ExecutedRoutedEventArgs e) {
			DeleteVariable(SelectedVariable);
		}
		

		//-----------------------------------------------------------------------------
		// UI Event Callbacks
		//-----------------------------------------------------------------------------

		private void OnDoubleClickVariable(object sender, MouseButtonEventArgs e) {
			Variable variable = ((ListViewItem) sender).Content as Variable;
			EditVariable(variable);
		}

		private void OnKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter)
				EditVariable(SelectedVariable);
		}

		private void OnClickColumnHeader(object sender, RoutedEventArgs e) {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string newSortedPropertyName = column.Tag.ToString();

			// Update the sorted column and sort direction.
			// Clicking on the same column will toggle the sort direction.
			if (newSortedPropertyName != sortedPropertyName) {
				sortedPropertyName = newSortedPropertyName;
				sortDirection = ListSortDirection.Ascending;
			}
			else if (sortDirection == ListSortDirection.Ascending)
				sortDirection = ListSortDirection.Descending;
			else
				sortDirection = ListSortDirection.Ascending;

			// Sort the variable list by the selected column
			listView.Items.SortDescriptions.Clear();
			listView.Items.SortDescriptions.Add(
				new SortDescription(sortedPropertyName, sortDirection));
		}
		
		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the currently selected variable in the variable list.
		/// </summary>
		public Variable SelectedVariable {
			get {
				if (listView != null)
					return ((VariableItem) listView.SelectedItem).Variable;
				return null;
			}
		}

		/// <summary>Gets or sets the variable collection that is being edited.
		/// </summary>
		public Variables Variables {
			get { return variables; }
			set {
				if (variables != value) {
					variables = value;
					RefreshVariableList();
				}
			}
		}
	}
}
