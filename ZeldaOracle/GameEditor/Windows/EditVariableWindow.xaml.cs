using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ZeldaEditor.Control;
using ZeldaEditor.Controls;
using ZeldaEditor.Undo;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaWpf.Controls;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Dialog window used to edit a variable or add a new variable.
	/// Interaction logic for EditVariableWindow.xaml
	/// </summary>
	public partial class EditVariableWindow : Window {
		
		/// <summary>Information about a FrameworkElement used to edit a specific Type.
		/// </summary>
		private interface IValueEditor {
			/// <summary>Creates the FrameworkElementFactory which is able to create the
			/// editor FrameworkElement as a Template.</summary>
			FrameworkElementFactory CreateFactory();
			/// <summary>Gets the underlying data type being edited.</summary>
			Type DataType { get; }
			/// <summary>Gets the FrameworkElement type of the value editor.</summary>
			Type EditorType { get; }
			/// <summary>Gets the DependencyProperty representing the editor's value.
			/// </summary>
			DependencyProperty ValueProperty { get; }
			/// <summary>Gets the ValueConverter to convert between the DataType value
			/// and the Editor's ValueProperty value.</summary>
			IValueConverter ValueConverter { get; }
		}

		/// <summary>Base class for a custom FrameworkElement used to edit a value of a
		/// specific Type.</summary>
		/// <typeparam name="TEditor">The FrameworkElement Type of the editor.</typeparam>
		/// <typeparam name="TType">The underlying data Type.</typeparam>
		private abstract class ValueEditor<TEditor, TType> : IValueEditor
			where TEditor : FrameworkElement, new()
		{
			/// <summary>Gets the FrameworkElementFactory which is able to create the
			/// editor FrameworkElement as a Template.</summary>
			public virtual FrameworkElementFactory CreateFactory() {
				return new FrameworkElementFactory(typeof(TEditor));
			}

			/// <summary>The underlying data type being edited.</summary>
			public Type DataType { get { return typeof(TType); } }

			/// <summary>The FrameworkElement type of the value editor.</summary>
			public Type EditorType { get { return typeof(TEditor); } }

			/// <summary>Gets the DependencyProperty representing the editor's value.
			/// </summary>
			public abstract DependencyProperty ValueProperty { get; }
			
			/// <summary>Gets the ValueConverter to convert between the DataType value
			/// and the Editor's ValueProperty value.</summary>
			public virtual IValueConverter ValueConverter { get { return null; } }
		}

		/// <summary>UpDown used to edit numeric types (integer, float).</summary>
		private class UpDownValueEditor<TEditor, TType> : ValueEditor<TEditor, TType>
			where TEditor : Xceed.Wpf.Toolkit.Primitives.UpDownBase<TType?>, new()
			where TType : struct, IComparable
		{
			public override DependencyProperty ValueProperty {
				get {
					return Xceed.Wpf.Toolkit.Primitives
						.UpDownBase<TType?>.ValueProperty;
				}
			}
		}
		
		/// <summary>ComboBox to edit booleans.</summary>
		private class BooleanValueEditor :
			ValueEditor<ComboBox, bool>, IValueConverter
		{
			public override FrameworkElementFactory CreateFactory() {
				FrameworkElementFactory factory = base.CreateFactory();
				factory.SetValue(ComboBox.ItemsSourceProperty,
					new string[] { "True", "False" });
				return factory;
			}

			object IValueConverter.Convert(object value, Type targetType,
				object parameter, CultureInfo culture)
			{
				return ((bool) value ? 0 : 1);
			}

			object IValueConverter.ConvertBack(object value, Type targetType,
				object parameter, CultureInfo culture)
			{
				return ((int) value == 0);
			}

			public override DependencyProperty ValueProperty {
				get { return ComboBox.SelectedIndexProperty; }
			}

			public override IValueConverter ValueConverter {
				get { return this; }
			}
		}

		/// <summary>TextBox to edit strings.</summary>
		private class StringValueEditor :
			ValueEditor<Xceed.Wpf.Toolkit.WatermarkTextBox, string>
		{
			public override DependencyProperty ValueProperty {
				get { return TextBox.TextProperty; }
			}
		}

		/// <summary>PointUpDown to edit points.</summary>
		private class PointValueEditor : ValueEditor<PointUpDown, Point2I> {
			public override DependencyProperty ValueProperty {
				get { return PointUpDown.ValueProperty; }
			}
		}

		private class InitialValueListItem {
			public int Index { get; set; }
			public object Value { get; set; }

			public InitialValueListItem(int index, object value) {
				Index = index;
				Value = value;
			}
		}

		private static Dictionary<VarType, IValueEditor> valueEditorTypes =
			new Dictionary<VarType, IValueEditor>();

		/// <summary>The variable being edited.</summary>
		private Variable variable;
		/// <summary>The collection of variables for which we are adding or editing a
		/// variable.</summary>
		private Variables variables;
		/// <summary>The initial value(s) for the variable.</summary>
		private ObservableCollection<InitialValueListItem> initialValues;
		/// <summary>The if adding a new variable, False if editing an existing one.
		/// </summary>
		private bool isNewVariable;
		/// <summary>The name of the variable.</summary>
		public string VariableName { get; set; }
		/// <summary>The C# Type of the variable.</summary>
		public VarType VariableType { get; set; }
		/// <summary>The list Type of the variable.</summary>
		public ListType ListType { get; set; }
		/// <summary>The size of the variable's list, or 1 if it is a single.</summary>
		public int ListCount { get; set; }
		/// <summary>The resulting UndoAction representing adding or editing a
		/// variable.</summary>
		private UndoAction<ObjectEditor> action;

		private bool suppressEvents;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EditVariableWindow(Variable variable, Variables variables, bool isNewVariable) {
			this.variable = variable;
			this.variables = variables;
			this.isNewVariable = isNewVariable;

			initialValues = new ObservableCollection<InitialValueListItem>();
			suppressEvents = true;

			// Setup the initial variable settings
			if (variable == null) {
				VariableName = "";
				VariableType = VarType.Integer;
				ListType = ListType.Single;
				ListCount = 1;
				initialValues.Add(new InitialValueListItem(0, 0));
			}
			else {
				VariableName = variable.Name;
				ListCount = variable.Count;
				ListType = variable.ListType;
				VariableType = variable.VarType;

				// Populate the list of initial values
				if (ListType == ListType.Single) {
					initialValues.Add(new InitialValueListItem(
						0, variable.ObjectValue));
				}
				else {
					IList list = variable.GetObjectList();
					for (int i = 0; i < list.Count; i++)
						initialValues.Add(new InitialValueListItem(i, list[i]));
				}
			}

			InitializeComponent();

			DataContext = this;

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
			
			// Create the value editor for the variable type
			if (variable == null)
				CreateValueEditor(0);
			else
				CreateValueEditor(variable.ObjectValue);

			Loaded += OnLoaded;

			textBoxVariableName.Focus();
		}



		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Setup the initial value editor for the variable type.</summary>
		private void CreateValueEditor(object value) {
			IValueEditor typeEditor = valueEditorTypes[VariableType];

			// Setup the initial value list to use the appropriate value editor for
			// this variable type
			if (valueEditorTypes.ContainsKey(VariableType)) {
				typeEditor = valueEditorTypes[VariableType];
				GridView gridView = (GridView) listViewInitialValues.View;

				listViewInitialValues.ItemsSource = null;
				gridView.Columns[1].CellTemplate = null;

				// Reset all initial values to the default for this variable type
				if (value is IList) {
					IList list = (IList) value;
					for (int i = 0; i < list.Count; i++)
						initialValues[i].Value = list[i];
				}
				else {
					for (int i = 0; i < initialValues.Count; i++)
						initialValues[i].Value = value;
				}

				// Update the list's DataTemplate
				FrameworkElementFactory factory = typeEditor.CreateFactory();
				factory.SetBinding(typeEditor.ValueProperty, new Binding("Value") {
					Converter = typeEditor.ValueConverter
				});
				gridView.Columns[1].CellTemplate = new DataTemplate() {
					DataType = typeEditor.DataType,
					VisualTree = factory,
				};
				listViewInitialValues.ItemsSource = initialValues;
			}
		}

		/// <summary>Resize the list of initial values. Any new items will be set to
		/// the default value for the variable type.</summary>
		/// <param name="listCount"></param>
		private void ResizeInitialValueList(int listCount) {
			if (initialValues.Count != listCount) {
				listViewInitialValues.ItemsSource = null;

				while (initialValues.Count < listCount)
					initialValues.Add(new InitialValueListItem(
						initialValues.Count, VariableType.GetDefaultValue()));
				while (initialValues.Count > listCount)
					initialValues.RemoveAt(initialValues.Count - 1);

				listViewInitialValues.ItemsSource = initialValues;
			}
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnLoaded(object sender, RoutedEventArgs e) {
			suppressEvents = false;
		}

		private void OnAdd(object sender, RoutedEventArgs e) {
			// Validate the variable name and verify it is unique
			if (!Variable.IsValidName(VariableName)) {
				MessageBox.Show(
					messageBoxText: string.Format(
						"'{0}' is not a valid variable name.", VariableName),
					caption: "Error");
			}
			else if ((isNewVariable && variables.Contains(VariableName)) ||
				(!isNewVariable && variables.Contains(VariableName) &&
					variables.GetVariable(VariableName) != variable))
			{
				MessageBox.Show(
					messageBoxText: string.Format(
						"A variable named '{0}' already exists.", VariableName),
					caption: "Error");
			}
			else {
				// Create the variable instance
				Variable newVariable = new Variable(
					VariableName, VariableType, ListType, ListCount);

				if (ListType == ListType.Single) {
					newVariable.ObjectValue = initialValues[0].Value;
				}
				else if (ListType == ListType.List) {
					// Create a List<> of values
					newVariable.ObjectValue = Activator.CreateInstance(
						typeof(List<>).MakeGenericType(
							new Type[] { VariableType.ToType() }));
					for (int i = 0; i < initialValues.Count; i++)
						newVariable.AddObjectAt(initialValues[i].Value);
				}
				else if (ListType == ListType.Array) {
					// Create an array of values
					Array array = Array.CreateInstance(VariableType.ToType(),
						initialValues.Count);
					for (int i = 0; i < initialValues.Count; i++)
						array.SetValue(initialValues[i].Value, i);
					newVariable.ObjectValue = array;
				}
				
				if (isNewVariable)
					action = new ActionNewVariable(newVariable, variables);
				else
					action = new ActionEditVariable(variable, newVariable, variables);

				DialogResult = true;
				Close();
			}
		}

		private void OnVariableTypeChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			CreateValueEditor(VariableType.GetDefaultValue());
		}

		private void OnListTypeChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			if (ListType == ListType.Single)
				ResizeInitialValueList(1);
			else
				ResizeInitialValueList(spinnerListSize.Value.Value);
		}

		private void OnListSizeChanged(object sender,
			RoutedPropertyChangedEventArgs<object> e)
		{
			if (suppressEvents) return;
			if (ListType == ListType.Array || ListType == ListType.List)
				ResizeInitialValueList(spinnerListSize.Value.Value);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		public static UndoAction<ObjectEditor> ShowAddVariable(Window owner, Variables variables) {
			EditVariableWindow window = new EditVariableWindow(
				null, variables, true);
			window.Owner = owner;
			bool? result = window.ShowDialog();
			if (result.HasValue && result.Value)
				return window.action;
			return null;
		}

		public static UndoAction<ObjectEditor> ShowEditVariable(Window owner, Variable variable) {
			EditVariableWindow window = new EditVariableWindow(
				variable, variable.Variables, false);
			window.Owner = owner;
			bool? result = window.ShowDialog();
			if (result.HasValue && result.Value)
				return window.action;
			return null;
		}

		static EditVariableWindow() {
			// Setup the value editors for the different variable types
			SetValueEditorType<StringValueEditor>(VarType.String);
			SetValueEditorType<UpDownValueEditor<Xceed.Wpf.Toolkit.IntegerUpDown, int>>(VarType.Integer);
			SetValueEditorType<UpDownValueEditor<Xceed.Wpf.Toolkit.SingleUpDown, float>>(VarType.Float);
			SetValueEditorType<BooleanValueEditor>(VarType.Boolean);
			SetValueEditorType<PointValueEditor>(VarType.Point);

			// TODO: Implement these for all variable types!
		}

		private static void SetValueEditorType<T>(VarType varType)
			where T : IValueEditor, new()
		{
			valueEditorTypes[varType] = new T();
		}
	}
}
