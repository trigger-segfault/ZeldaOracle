using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ZeldaEditor.Control;
using ZeldaEditor.Undo;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Controls {
	
	/// <summary>A one-way IValueConverter meant to format a value for being displayed
	/// as text.</summary>
	public abstract class ValueFormatter<T> : IValueConverter {
		public object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			return FormatValue((T) value);
		}
		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
		public abstract string FormatValue(T value);
	}
		
	/// <summary>Formats a Variable's VarType/ListType into a readable string.
	/// </summary>
	public class VariableTypeFormatter : ValueFormatter<Variable> {
		public override string FormatValue(Variable variable) {
			// Example: "Integer List [3]"
			string text = variable.VarType.ToString();
			if (variable.ListType == ListType.Array)
				text += string.Format(" Array [{0}]", variable.Count);
			else if (variable.ListType == ListType.List)
				text += string.Format(" List [{0}]", variable.Count);
			return text;
		}
	}
		
	/// <summary>Formats a Variable's value into a readable string.</summary>
	public class VariableValueFormatter : ValueFormatter<Variable> {
		public override string FormatValue(Variable variable) {
			return variable.GetValueString();
		}
	}

	/// <summary>
	/// Interaction logic for ObjectVariableEditor.xaml
	/// </summary>
	public partial class ObjectVariableEditor : UserControl, INotifyPropertyChanged {
		
		/// <summary>The variable collection that is being edited.</summary>
		private Variables variables;
		/// <summary>Name of the VariableItem property used for sorting the variable
		/// list.</summary>
		private string sortedPropertyName;
		/// <summary>The sort direction of the variable list.</summary>
		private ListSortDirection sortDirection;
		/// <summary>A duplicate of the variable that was copied or cut.</summary>
		public Variable Clipboard { get; set; }

		private UndoHistory<ObjectEditor> history;

        public event PropertyChangedEventHandler PropertyChanged;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ObjectVariableEditor() {
			history = new UndoHistory<ObjectEditor>(null, 50);

			history.PositionChanged += OnHistoryChanged;

			DataContext = this;

			InitializeComponent();

			history.Context = Window.GetWindow(this) as ObjectEditor;

			// Initially sort by name in ascending order
			sortedPropertyName = "Name";
			sortDirection = ListSortDirection.Ascending;
			listView.Items.SortDescriptions.Add(
				new SortDescription(sortedPropertyName, sortDirection));

			Clipboard = null;
			variables = null;
			RefreshVariableList();
		}


		//-----------------------------------------------------------------------------
		// Variable Manipulation
		//-----------------------------------------------------------------------------

		private void PasteVariable(Variable variable) {
			history.PushAction(new ActionPasteVariable(variable, variables),
				ActionExecution.Execute);
			RefreshVariableList();
		}

		/// <summary>Delete a variable from its collection.</summary>
		private void DeleteVariable(Variable variable) {
			history.PushAction(new ActionDeleteVariable(variable),
				ActionExecution.Execute);
			RefreshVariableList();
		}

		/// <summary>Open the edit variable window for the given variable.</summary>
		private void EditVariable(Variable variable) {
			UndoAction<ObjectEditor> action = EditVariableWindow.ShowEditVariable(
				Window.GetWindow(this), SelectedVariable);
			if (action != null)
				history.PushAction(action, ActionExecution.Execute);
			RefreshVariableList();
		}

		/// <summary>Refresh the list view of variables.</summary>
		private void RefreshVariableList() {
			if (variables != null) {
				List<Variable> items = new List<Variable>();
				// TODO: Show built-in variables as read-only list items
				foreach (Variable variable in variables.GetCustomVariables())
					items.Add(variable);
				listView.ItemsSource = items;
			}
			else {
				listView.ItemsSource = null;
			}
		}

		private void NotifyPropertyChanged(string propertyName = "") {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
		
		/// <summary>Returns true if a variable was copied or cut.</summary>
		private void CanExecutePasteCommand(
			object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (listView != null &&
				variables != null && Clipboard != null);
		}
		
		/// <summary>Returns true if a variable is selected.</summary>
		private void CanExecuteVariableCommand(
			object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (listView != null && variables != null &&
				listView.SelectedItem != null);
		}
		
		private void CanExecuteUndoCommand(
			object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = history.CanUndo;
		}
		
		private void CanExecuteRedoCommand(
			object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = history.CanRedo;
		}

		
		//-----------------------------------------------------------------------------
		// Command Callbacks
		//-----------------------------------------------------------------------------

		private void OnCut(object sender, ExecutedRoutedEventArgs e) {
			Clipboard = new Variable(SelectedVariable);
			DeleteVariable(SelectedVariable);
		}

		private void OnCopy(object sender, ExecutedRoutedEventArgs e) {
			Clipboard = new Variable(SelectedVariable);
		}

		private void OnPaste(object sender, ExecutedRoutedEventArgs e) {
			PasteVariable(Clipboard);
		}

		private void OnUndo(object sender, ExecutedRoutedEventArgs e) {
			history.Undo();
		}

		private void OnRedo(object sender, ExecutedRoutedEventArgs e) {
			history.Redo();
		}

		private void OnNewVariable(object sender, ExecutedRoutedEventArgs e) {
			UndoAction<ObjectEditor> action = EditVariableWindow.ShowAddVariable(
				Window.GetWindow(this), variables);
			if (action != null)
				history.PushAction(action, ActionExecution.Execute);
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
		// Event Callbacks
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

		private void OnHistoryChanged(object sender, EventArgs e) {
			NotifyPropertyChanged();
			RefreshVariableList();
			ObjectEditor editor = Window.GetWindow(this) as ObjectEditor;
			if (editor != null)
				editor.EditorControl.IsModified = true;
		}
		
		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the currently selected variable in the variable list.
		/// </summary>
		public Variable SelectedVariable {
			get {
				if (listView != null)
					return (Variable) listView.SelectedItem;
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

		public string UndoTooltip {
			get {
				string text = "Undo";
				if (history.CanUndo)
					text += " " + history.UndoAction.ActionName;
				text += " (Ctrl+Z)";
				return text;
			}
		}

		public string RedoTooltip {
			get {
				string text = "Redo";
				if (history.CanRedo)
					text += " " + history.RedoAction.ActionName;
				text += " (Ctrl+Y)";
				return text;
			}
		}
	}
}
