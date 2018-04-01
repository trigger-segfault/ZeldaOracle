using System;
using System.Windows;
using ZeldaEditor.Control;
using System.Collections.ObjectModel;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Scripting;
using Trigger = ZeldaOracle.Common.Scripting.Trigger;
using TriggerCollection = ZeldaOracle.Common.Scripting.TriggerCollection;
using ZeldaEditor.WinForms;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game;
using ZeldaEditor.Scripting;
using ZeldaEditor.Controls;
using ZeldaOracle.Game.Worlds;
using System.Drawing;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using System.Windows.Controls;

namespace ZeldaEditor.Windows {

	/*
	public class ObjectEditorModel {

		public void SetObject(object obj) {
			this.obj = obj;
			triggerObject = obj as ITriggerObject;

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
			}
		}


		//-----------------------------------------------------------------------------
		// Trigger Management
		//-----------------------------------------------------------------------------

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
		// Properties
		//-----------------------------------------------------------------------------

		public ObservableCollection<Trigger> Triggers {
			get { return triggers; }
		}

		public ObservableCollection<TriggerEvent> EventTypes {
			get { return eventTypes; }
		}

		public Trigger SelectedTrigger { get; set; } = null;

		public int SelectedTriggerIndex {
			get {
				if (SelectedTrigger != null)
					return triggers.IndexOf(SelectedTrigger);
				return -1;
			}
		}

		public object Object {
			get { return obj; }
		}

		public EventCollection EventCollection {
			get {
				if (triggerObject != null)
					return triggerObject.Events;
				return null;
			}
		}

		public TriggerCollection TriggerCollection {
			get {
				if (triggerObject != null)
					return triggerObject.Triggers;
				return null;
			}
		}
	}
	*/

	/// <summary>
	/// Interaction logic for ObjectEditor.xaml
	/// </summary>
	public partial class ObjectEditor : Window {

		private ScriptTextEditor scriptEditor;
		private EditorControl editorControl;
		//private ObjectEditorModel model;
		// private ScriptEditorControl scriptEditor;
		private TilePreview tilePreview;
		

		private object obj;
		private ObservableCollection<Trigger> triggers;
		private ObservableCollection<TriggerEvent> eventTypes;
		private ITriggerObject triggerObject;



		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ObjectEditor(EditorControl editorControl, object obj) {
			InitializeComponent();


			this.editorControl = editorControl;
			
			panelEditTrigger.IsEnabled = false;
			textBoxTriggerName.Text = "";
			//comboBoxEventType.Text = model.SelectedEvent.EventType;
			checkBoxInitiallyOn.IsChecked = false;
			checkBoxFireOnce.IsChecked = false;
			
			// Create the script text editor
			scriptEditor = new ScriptTextEditor();
			Grid.SetRow(scriptEditor, 1);
			panelEditTrigger.Children.Add(scriptEditor);
			scriptEditor.ScriptCodeChanged += OnScriptTextChanged;

			// Create the tile preview
			tilePreview						= new TilePreview();
			tilePreview.EditorControl		= editorControl;
			tilePreview.Name				= "tilePreview";
			tilePreview.Dock				= System.Windows.Forms.DockStyle.Fill;
			hostTilePreview.Child			= tilePreview;
			
			obj = null;
			triggerObject = null;
			triggers = new ObservableCollection<Trigger>();
			eventTypes = new ObservableCollection<TriggerEvent>();
			
			listBoxTriggers.ItemsSource = triggers;
			comboBoxEventType.ItemsSource = eventTypes;

			// Setup the script text editor
			//scriptEditor.ScriptCodeChanged += OnScriptTextChanged;
			//scriptEditor.CaretPositionChanged += OnCaretPositionChanged;
			
			//timer = StoppableTimer.StartNew(
			//	TimeSpan.FromMilliseconds(500),
			//	DispatcherPriority.ApplicationIdle,
			//	TimerUpdate);

			SetObject(obj);
		}

		public static ObjectEditor Show(Window owner, EditorControl editorControl,
			object obj = null)
		{
			ObjectEditor window = new ObjectEditor(editorControl, obj);
			window.Owner = owner;
			window.Show();
			return window;
		}		

		/// <summary>Set the object to show properties for.</summary>
		public void SetObject(object obj) {
			this.obj = obj;
			triggerObject = obj as ITriggerObject;

			// Re-populate the trigger and event lists
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

			// Set the object preview image and name
			objectPreviewName.Text = "(none)";
			if (obj is BaseTileDataInstance) {
				objectPreviewName.Text =
					((BaseTileDataInstance) obj).BaseData.ResourceName;
				if (obj is TileDataInstance)
					Title = "Tile Properties";
				else
					Title = "Action Tile Properties";
			}
			else if (obj is Area) {
				Title = "Area Properties";
				objectPreviewName.Text = ((Area) obj).ID;
			}
			else if (obj is Room) {
				Title = "Room Properties";
				objectPreviewName.Text = "Room";
				if (!string.IsNullOrWhiteSpace(((Room) obj).ID))
					objectPreviewName.Text += " - " + ((Room) obj).ID;
			}
			else if (obj is Level) {
				Title = "Level Properties";
				objectPreviewName.Text = ((Level) obj).ID;
			}
			else if (obj is World) {
				Title = "World Properties";
				objectPreviewName.Text = "World";
			}
			tilePreview.UpdateTile(obj as BaseTileDataInstance);
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
		// UI Callbacks
		//-----------------------------------------------------------------------------

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

		private void OnScriptTextChanged(object sender, EventArgs e) {
			Trigger selectedTrigger = (Trigger) listBoxTriggers.SelectedItem;
			if (selectedTrigger != null) {
				if (selectedTrigger.Script == null)
					selectedTrigger.CreateScript();
				selectedTrigger.Script.Code = scriptEditor.ScriptCode;
				//needsRecompiling = true;
				//UpdateStatusBar();
				//CommandManager.InvalidateRequerySuggested();
			}
		}
	}
}
