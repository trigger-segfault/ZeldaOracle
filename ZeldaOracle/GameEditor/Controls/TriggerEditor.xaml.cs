using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ZeldaOracle.Common.Scripting;
using Trigger = ZeldaOracle.Common.Scripting.Trigger;

namespace ZeldaEditor.Controls {
	
	/// <summary>Commands used by the TriggerEditor.</summary>
	public static class TriggerEditorCommands {
		public static readonly RoutedUICommand CreateTrigger = new RoutedUICommand(
			"CreateTrigger", "Create Trigger", typeof(TriggerEditorCommands));

		public static readonly RoutedUICommand DeleteTrigger = new RoutedUICommand(
			"DeleteTrigger", "Delete Trigger", typeof(TriggerEditorCommands));

		public static readonly RoutedUICommand DuplicateTrigger = new RoutedUICommand(
			"DuplicateTrigger", "Duplicate Trigger", typeof(TriggerEditorCommands));

		public static readonly RoutedUICommand MoveTriggerDown = new RoutedUICommand(
			"MoveTriggerDown", "Move Down", typeof(TriggerEditorCommands));

		public static readonly RoutedUICommand MoveTriggerUp = new RoutedUICommand(
			"MoveTriggerUp", "Move Up", typeof(TriggerEditorCommands));
	}

	/// <summary>
	/// Interaction logic for TriggerEditor.xaml
	/// </summary>
	public partial class TriggerEditor : UserControl {
		
		private ObservableCollection<Trigger> triggers;
		private ObservableCollection<TriggerEvent> eventTypes;
		private ITriggerObject triggerObject;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TriggerEditor() {
			InitializeComponent();

			triggers = new ObservableCollection<Trigger>();
			listBoxTriggers.ItemsSource = triggers;

			eventTypes = new ObservableCollection<TriggerEvent>();
			comboBoxEventType.ItemsSource = eventTypes;
			
			// Setup the script text editor
			scriptEditor.ScriptCodeChanged += OnScriptTextChanged;
			scriptEditor.CaretPositionChanged += OnCaretPositionChanged;

			SetObject(null);
		}


		//-----------------------------------------------------------------------------
		// Trigger Management
		//-----------------------------------------------------------------------------

		/// <summary>Set the object to show properties for.</summary>
		public void SetObject(ITriggerObject triggerObject) {
			this.triggerObject = triggerObject;

			// Re-populate the lists of triggers and events
			triggers.Clear();
			eventTypes.Clear();
			eventTypes.Add(TriggerEvent.None);
			if (triggerObject != null) {
				// First populate the list of event types
				foreach (Event e in triggerObject.Events.GetEvents())
					eventTypes.Add(new TriggerEvent(e));

				// Then populate the trigger list
				foreach (Trigger trigger in triggerObject.Triggers)
					triggers.Add(trigger);

				if (triggers.Count > 0)
					listBoxTriggers.SelectedIndex = 0;
			}
		}

		public Trigger AddNewTrigger() {
			// Find an unused untitled trigger name
			bool nameIsTaken = true;
			string name = "";
			for (int index = 1; nameIsTaken; index++) {
				name = string.Format("trigger_{0}", index);
				nameIsTaken = false;
				foreach (Trigger other in triggers) {
					if (name == other.Name) {
						nameIsTaken = true;
						break;
					}
				}
			}
			
			// Create the trigger
			Trigger trigger = triggerObject.Triggers.CreateNewTrigger(name);
			triggers.Add(trigger);
			return trigger;
		}

		public void DeleteTrigger(Trigger trigger) {
			triggers.Remove(trigger);
			if (triggerObject != null)
				triggerObject.Triggers.RemoveTrigger(trigger);
		}


		//-----------------------------------------------------------------------------
		// Command Can Execute
		//-----------------------------------------------------------------------------
		
		private void CanExecuteTriggerAction(
			object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsTriggerSelected;
		}


		//-----------------------------------------------------------------------------
		// UI Event Callbacks
		//-----------------------------------------------------------------------------

		/// <summary>Called when a new trigger is selected.</summary>
		private void OnSelectedTriggerChanged(object sender, RoutedEventArgs e) {
			if (IsTriggerSelected) {
				Trigger selectedTrigger = SelectedTrigger;

				// Update the trigger property fields
				textBoxTriggerName.Text = selectedTrigger.Name;
				comboBoxEventType.SelectedIndex =
					eventTypes.IndexOf(selectedTrigger.EventType);
				checkBoxInitiallyOn.IsChecked = selectedTrigger.InitiallyOn;
				checkBoxFireOnce.IsChecked = selectedTrigger.FireOnce;

				// Load the trigger's script into the script editor
				if (selectedTrigger.Script != null)
					scriptEditor.Trigger = selectedTrigger;
				else
					scriptEditor.Trigger = null;
			}
			else {
				textBoxTriggerName.Text = "";
				comboBoxEventType.SelectedIndex = 0;
				checkBoxInitiallyOn.IsChecked = false;
				checkBoxFireOnce.IsChecked = false;
				scriptEditor.Trigger = null;
			}
		}
		
		private void OnCreateTrigger(object sender, ExecutedRoutedEventArgs e) {
			Trigger trigger = AddNewTrigger();
			listBoxTriggers.SelectedIndex = triggers.IndexOf(trigger);
		}

		private void OnDeleteTrigger(object sender, ExecutedRoutedEventArgs e) {
			if (IsTriggerSelected) {
				int nextSelectedIndex = -1;
				if (listBoxTriggers.SelectedIndex < triggers.Count - 1)
					nextSelectedIndex = listBoxTriggers.SelectedIndex;
				else if (triggers.Count > 1)
					nextSelectedIndex = 0;
				DeleteTrigger(SelectedTrigger);
				listBoxTriggers.SelectedIndex = nextSelectedIndex;
			}
		}
		
		private void OnDuplicateTrigger(object sender, ExecutedRoutedEventArgs e) {
			if (IsTriggerSelected) {
				Trigger duplicate = new Trigger(SelectedTrigger);
				triggerObject.Triggers.AddTrigger(duplicate);
				triggers.Add(duplicate);
				listBoxTriggers.SelectedIndex = triggers.IndexOf(duplicate);
			}
		}
		
		private void OnMoveTriggerUp(object sender, ExecutedRoutedEventArgs e) {
			// TODO: Implement
		}

		private void OnMoveTriggerDown(object sender, ExecutedRoutedEventArgs e) {
			// TODO: Implement
		}
		
		private void OnSelectEvent(object sender, RoutedEventArgs e) {
			if (IsTriggerSelected) {
				if (comboBoxEventType.SelectedIndex >= 0)
					SelectedTrigger.EventType =
						eventTypes[comboBoxEventType.SelectedIndex];
				else
					SelectedTrigger.EventType = TriggerEvent.None;
			}
		}
		
		private void OnRenameTrigger(object sender, RoutedEventArgs e) {
			if (IsTriggerSelected &&
				SelectedTrigger.Name != textBoxTriggerName.Text)
			{
				SelectedTrigger.Name = textBoxTriggerName.Text;
				if (scriptEditor.Trigger == SelectedTrigger)
					scriptEditor.UpdateMethodName(SelectedTrigger.Name);
				listBoxTriggers.Items.Refresh();
			}
		}

		private void OnClickInitiallyOn(object sender, RoutedEventArgs e) {
			if (IsTriggerSelected)
				SelectedTrigger.InitiallyOn =
					checkBoxInitiallyOn.IsChecked.Value;
		}

		private void OnClickFireOnce(object sender, RoutedEventArgs e) {
			if (IsTriggerSelected)
				SelectedTrigger.FireOnce = checkBoxFireOnce.IsChecked.Value;
		}

		/// <summary>Called when the text in the script editor changed.</summary>
		private void OnScriptTextChanged(object sender, EventArgs e) {
			if (IsTriggerSelected) {
				if (SelectedTrigger.Script == null)
					SelectedTrigger.CreateScript();
				SelectedTrigger.Script.Code = scriptEditor.ScriptCode;
				//needsRecompiling = true;
				//CommandManager.InvalidateRequerySuggested();
			}
		}

		private void OnCaretPositionChanged(object sender, EventArgs e) {
			var caret = scriptEditor.CaretPosition;
			if (caret.Line == -1 || !IsTriggerSelected) {
				statusLine.Content = "Line -";
				statusColumn.Content = "Col -";
				statusChar.Content = "Char -";
			}
			else {
				statusLine.Content = "Line " + caret.Line;
				statusColumn.Content = "Col " + caret.VisualColumn;
				statusChar.Content = "Char " + caret.Column;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Trigger SelectedTrigger {
			get {
				if (!IsLoaded)
					return null;
				return (Trigger) listBoxTriggers.SelectedItem;
			}
		}

		public bool IsTriggerSelected {
			get { return SelectedTrigger != null; }
		}
	}
}
