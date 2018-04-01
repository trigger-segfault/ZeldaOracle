using System;
using System.Windows;
using System.Collections.ObjectModel;
using ZeldaOracle.Common.Scripting;
using System.Windows.Controls;
using Trigger = ZeldaOracle.Common.Scripting.Trigger;

namespace ZeldaEditor.Controls {
	/// <summary>
	/// Interaction logic for TriggerEditor.xaml
	/// </summary>
	public partial class TriggerEditor : UserControl {

		private ScriptTextEditor scriptEditor;
		private ObservableCollection<Trigger> triggers;
		private ObservableCollection<TriggerEvent> eventTypes;
		private ITriggerObject triggerObject;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TriggerEditor() {
			InitializeComponent();

			triggers = new ObservableCollection<Trigger>();
			eventTypes = new ObservableCollection<TriggerEvent>();
			
			panelEditTrigger.IsEnabled = false;
			textBoxTriggerName.Text = "";
			checkBoxInitiallyOn.IsChecked = false;
			checkBoxFireOnce.IsChecked = false;
			listBoxTriggers.ItemsSource = triggers;
			comboBoxEventType.ItemsSource = eventTypes;
			
			// Create the script text editor
			scriptEditor = new ScriptTextEditor();
			Grid.SetRow(scriptEditor, 1);
			panelEditTrigger.Children.Add(scriptEditor);
			scriptEditor.ScriptCodeChanged += OnScriptTextChanged;

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
		// UI Event Callbacks
		//-----------------------------------------------------------------------------
		
		/// <summary>Called when a new trigger is selected.</summary>
		private void OnSelectTrigger(object sender, RoutedEventArgs e) {
			if (listBoxTriggers.SelectedIndex < 0) {
				panelEditTrigger.IsEnabled = false;
				textBoxTriggerName.Text = "";
				comboBoxEventType.SelectedIndex = 0;
				checkBoxInitiallyOn.IsChecked = false;
				checkBoxFireOnce.IsChecked = false;
				scriptEditor.Script = null;
			}
			else {
				panelEditTrigger.IsEnabled = true;
				Trigger selectedTrigger = triggers[listBoxTriggers.SelectedIndex];

				textBoxTriggerName.Text = selectedTrigger.Name;
				comboBoxEventType.SelectedIndex =
					eventTypes.IndexOf(selectedTrigger.EventType);
				checkBoxInitiallyOn.IsChecked = selectedTrigger.InitiallyOn;
				checkBoxFireOnce.IsChecked = selectedTrigger.FireOnce;

				if (selectedTrigger.Script != null)
					scriptEditor.Script = selectedTrigger.Script;
				else
					scriptEditor.Script = null;
			}
		}
		
		private void OnAddTrigger(object sender, RoutedEventArgs e) {
			Trigger trigger = AddNewTrigger();
			listBoxTriggers.SelectedIndex = triggers.IndexOf(trigger);
		}

		private void OnDeleteTrigger(object sender, RoutedEventArgs e) {
			Trigger selectedTrigger = (Trigger) listBoxTriggers.SelectedItem;

			if (selectedTrigger != null) {
				int nextSelectedIndex = -1;
				if (listBoxTriggers.SelectedIndex < triggers.Count - 1)
					nextSelectedIndex = listBoxTriggers.SelectedIndex;
				else if (triggers.Count > 1)
					nextSelectedIndex = 0;
				DeleteTrigger(selectedTrigger);
				listBoxTriggers.SelectedIndex = nextSelectedIndex;
			}
		}

		private void OnCutTrigger(object sender, RoutedEventArgs e) {
			Console.WriteLine("OnCutEvent");
		}

		private void OnCopyTrigger(object sender, RoutedEventArgs e) {
			Console.WriteLine("OnCopyEvent");
		}

		private void OnPasteTrigger(object sender, RoutedEventArgs e) {
			Console.WriteLine("OnPasteEvent");
		}
		
		private void OnSelectEvent(object sender, RoutedEventArgs e) {
			Trigger selectedTrigger = (Trigger) listBoxTriggers.SelectedItem;

			if (selectedTrigger != null) {
				if (comboBoxEventType.SelectedIndex >= 0)
					selectedTrigger.EventType =
						eventTypes[comboBoxEventType.SelectedIndex];
				else
					selectedTrigger.EventType = TriggerEvent.None;
			}
		}
		
		private void OnRenameEvent(object sender, RoutedEventArgs e) {
			Trigger selectedTrigger = (Trigger) listBoxTriggers.SelectedItem;

			if (selectedTrigger != null &&
				selectedTrigger.Name != textBoxTriggerName.Text)
			{
				selectedTrigger.Name = textBoxTriggerName.Text;
				listBoxTriggers.Items.Refresh();
			}
		}

		private void OnClickInitiallyOn(object sender, RoutedEventArgs e) {
			Trigger selectedTrigger = (Trigger) listBoxTriggers.SelectedItem;
			if (selectedTrigger != null)
				selectedTrigger.InitiallyOn =
					checkBoxInitiallyOn.IsChecked.Value;
		}

		private void OnClickFireOnce(object sender, RoutedEventArgs e) {
			Trigger selectedTrigger = (Trigger) listBoxTriggers.SelectedItem;
			if (selectedTrigger != null)
				selectedTrigger.FireOnce = checkBoxFireOnce.IsChecked.Value;
		}

		/// <summary>Called when the text in the script editor changed.</summary>
		private void OnScriptTextChanged(object sender, EventArgs e) {
			Trigger selectedTrigger = (Trigger) listBoxTriggers.SelectedItem;
			if (selectedTrigger != null) {
				if (selectedTrigger.Script == null)
					selectedTrigger.CreateScript();
				selectedTrigger.Script.Code = scriptEditor.ScriptCode;
				//needsRecompiling = true;
				//CommandManager.InvalidateRequerySuggested();
			}
		}
	}
}
