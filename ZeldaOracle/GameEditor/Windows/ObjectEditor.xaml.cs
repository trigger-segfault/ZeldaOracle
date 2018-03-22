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
using ICSharpCode.CodeCompletion;
using ZeldaOracle.Game;
using ZeldaEditor.Scripting;
using ZeldaEditor.Controls;

namespace ZeldaEditor.Windows {

	public class ObjectEditorModel {

		private ObservableCollection<Trigger> triggers;
		private ObservableCollection<TriggerEvent> eventTypes;
		private BaseTileDataInstance tileData;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ObjectEditorModel() {
			tileData = null;
			triggers = new ObservableCollection<Trigger>();
			eventTypes = new ObservableCollection<TriggerEvent>();
		}


		//-----------------------------------------------------------------------------
		// Object Management
		//-----------------------------------------------------------------------------

		public void OnChangeObject() {
			triggers.Clear();
			eventTypes.Clear();

			// First populate the list of event types
			eventTypes.Add(TriggerEvent.None);
			if (tileData != null) {
				foreach (Event e in tileData.Events.GetEvents())
					eventTypes.Add(new TriggerEvent(e));
			}

			// Then populate the trigger list
			if (tileData != null) {
				foreach (Trigger trigger in tileData.Triggers) {
					triggers.Add(trigger);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Trigger Management
		//-----------------------------------------------------------------------------

		public Trigger AddNewTrigger() {
			Trigger trigger = new Trigger();

			// Find an unused untitled trigger name
			bool nameIsTaken = true;
			for (int index = 1; nameIsTaken; index++) {
				trigger.Name = string.Format("trigger_{0}", index);
				nameIsTaken = false;
				foreach (Trigger other in triggers) {
					if (trigger.Name == other.Name) {
						nameIsTaken = true;
						break;
					}
				}
			}

			triggers.Add(trigger);
			if (tileData != null)
				tileData.Triggers.AddTrigger(trigger);
			return trigger;
		}

		public void DeleteTrigger(Trigger trigger) {
			triggers.Remove(trigger);
			tileData.Triggers.RemoveTrigger(trigger);
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

		public BaseTileDataInstance TileData {
			get { return tileData; }
			set {
				if (tileData != value) {
					tileData = value;
					OnChangeObject();
				}
			}
		}

		public EventCollection EventCollection {
			get {
				if (tileData != null)
					return tileData.Events;
				return null;
			}
		}

		public TriggerCollection TriggerCollection {
			get {
				if (tileData != null)
					return tileData.Triggers;
				return null;
			}
		}
	}

	/// <summary>
	/// Interaction logic for ObjectEditor.xaml
	/// </summary>
	public partial class ObjectEditor : Window {

		private EditorControl editorControl;
		private ObjectEditorModel model;
		private TilePreview tilePreview;
		
		private static CSharpCompletion completion = new CSharpCompletion(
			new ScriptProvider(), Assemblies.Scripting);


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ObjectEditor(EditorControl editorControl,
			BaseTileDataInstance tileData)
		{
			InitializeComponent();
			this.editorControl = editorControl;
			model = new ObjectEditorModel();
			listBoxTriggers.ItemsSource = model.Triggers;
			comboBoxEventType.ItemsSource = model.EventTypes;
			
			panelEditTrigger.IsEnabled = false;
			model.SelectedTrigger = null;
			textBoxTriggerName.Text = "";
			//comboBoxEventType.Text = model.SelectedEvent.EventType;
			checkBoxInitiallyOn.IsChecked = false;
			checkBoxFireOnce.IsChecked = false;

			// Create the tile preview
			tilePreview						= new TilePreview();
			tilePreview.EditorControl		= editorControl;
			tilePreview.Name				= "tilePreview";
			tilePreview.Dock				= System.Windows.Forms.DockStyle.Fill;
			hostTilePreview.Child			= tilePreview;
			
			// Setup the script text editor
			scriptEditor.TextChanged += OnScriptTextChanged;
			//scriptEditor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
			
			scriptEditor.Completion = completion;
			scriptEditor.Document.FileName = "dummyFileName.cs";
			scriptEditor.Script = null;
			scriptEditor.EditorControl = editorControl;
			
			SetObject(tileData);
		}

		public static ObjectEditor Show(Window owner, EditorControl editorControl,
			BaseTileDataInstance tileData = null)
		{
			ObjectEditor window = new ObjectEditor(editorControl, tileData);
			window.Owner = owner;
			window.Show();
			return window;
		}		

		public void SetObject(BaseTileDataInstance tileData) {
			// Update the model
			model.TileData = tileData;

			if (model.Triggers.Count >= 0)
				listBoxTriggers.SelectedIndex = 0;

			// Set the object preview image and name
			objectPreviewName.Text = "(none)";
			if (tileData != null)
				objectPreviewName.Text = tileData.BaseData.ResourceName;
			tilePreview.UpdateTile(tileData);
		}


		//-----------------------------------------------------------------------------
		// UI Callbacks
		//-----------------------------------------------------------------------------

		private void OnSelectTrigger(object sender, RoutedEventArgs e) {
			if (listBoxTriggers.SelectedIndex < 0) {
				if (model.SelectedTrigger != null) {
					panelEditTrigger.IsEnabled = false;
					model.SelectedTrigger = null;
					textBoxTriggerName.Text = "";
					comboBoxEventType.SelectedIndex = 0;
					checkBoxInitiallyOn.IsChecked = false;
					checkBoxFireOnce.IsChecked = false;
					scriptEditor.Text = "";
					scriptEditor.Trigger = null;
				}
			}
			else {
				if (listBoxTriggers.SelectedIndex != model.SelectedTriggerIndex) {
					panelEditTrigger.IsEnabled = true;
					model.SelectedTrigger = model.Triggers[listBoxTriggers.SelectedIndex];
					textBoxTriggerName.Text = model.SelectedTrigger.Name;
					comboBoxEventType.SelectedIndex =
						model.EventTypes.IndexOf(model.SelectedTrigger.EventType);
					checkBoxInitiallyOn.IsChecked = model.SelectedTrigger.InitiallyOn;
					checkBoxFireOnce.IsChecked = model.SelectedTrigger.FireOnce;
					if (model.SelectedTrigger.Script != null)
						scriptEditor.Text = model.SelectedTrigger.Script.Code;
					else
						scriptEditor.Text = "";
					scriptEditor.Trigger = model.SelectedTrigger;
				}
			}
		}
		
		private void OnAddTrigger(object sender, RoutedEventArgs e) {
			Trigger trigger = model.AddNewTrigger();
			listBoxTriggers.SelectedIndex = model.Triggers.IndexOf(trigger);
		}

		private void OnDeleteTrigger(object sender, RoutedEventArgs e) {
			if (model.SelectedTrigger != null) {
				int nextSelectedIndex = -1;
				if (listBoxTriggers.SelectedIndex < model.Triggers.Count - 1)
					nextSelectedIndex = listBoxTriggers.SelectedIndex;
				else if (model.Triggers.Count > 1)
					nextSelectedIndex = 0;
				model.DeleteTrigger(model.SelectedTrigger);
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
			if (model.SelectedTrigger != null) {
				if (comboBoxEventType.SelectedIndex >= 0)
					model.SelectedTrigger.EventType =
						model.EventTypes[comboBoxEventType.SelectedIndex];
				else
					model.SelectedTrigger.EventType = TriggerEvent.None;
			}
		}
		
		private void OnRenameEvent(object sender, RoutedEventArgs e) {
			if (model.SelectedTrigger != null &&
				model.SelectedTrigger.Name != textBoxTriggerName.Text)
			{
				model.SelectedTrigger.Name = textBoxTriggerName.Text;
				listBoxTriggers.Items.Refresh();
			}
		}

		private void OnClickInitiallyOn(object sender, RoutedEventArgs e) {
			if (model.SelectedTrigger != null)
				model.SelectedTrigger.InitiallyOn =
					checkBoxInitiallyOn.IsChecked.Value;
		}

		private void OnClickFireOnce(object sender, RoutedEventArgs e) {
			if (model.SelectedTrigger != null)
				model.SelectedTrigger.FireOnce = checkBoxFireOnce.IsChecked.Value;
		}

		private void OnScriptTextChanged(object sender, EventArgs e) {
			if (model.SelectedTrigger != null) {
				if (model.SelectedTrigger.Script == null)
					model.SelectedTrigger.CreateScript();
				model.SelectedTrigger.Script.Code = scriptEditor.Text;
				//needsRecompiling = true;
				//UpdateStatusBar();
				//CommandManager.InvalidateRequerySuggested();
			}
		}
	}
}
