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

namespace ZeldaEditor.Windows {

	public class ObjectEditorModel {

		private object obj;
		private ObservableCollection<Trigger> triggers;
		private ObservableCollection<TriggerEvent> eventTypes;
		private ITriggerObject triggerObject;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ObjectEditorModel() {
			obj = null;
			triggerObject = null;
			triggers = new ObservableCollection<Trigger>();
			eventTypes = new ObservableCollection<TriggerEvent>();
		}


		//-----------------------------------------------------------------------------
		// Object Management
		//-----------------------------------------------------------------------------

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
			if (triggerObject != null)
				triggerObject.Triggers.AddTrigger(trigger);
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

	/// <summary>
	/// Interaction logic for ObjectEditor.xaml
	/// </summary>
	public partial class ObjectEditor : Window {

		private EditorControl editorControl;
		private ObjectEditorModel model;
		private TilePreview tilePreview;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ObjectEditor(EditorControl editorControl, object obj) {
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
			
			RoslynPad.Editor.RoslynCodeEditor editor = scriptEditor;
			
			//editor.Loaded += OnEditorLoaded;
			//editor.TextArea.TextEntering += OnTextEntering;
			editor.TextArea.Margin = new Thickness(4, 4, 0, 4);
			editor.TextArea.TextView.Options.AllowScrollBelowDocument = true;

			// Selection Style
			editor.TextArea.SelectionCornerRadius = 0;
			editor.TextArea.SelectionBorder = null;
			//editor.FontFamily = new FontFamily("Lucida Console");
			editor.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			editor.FontSize = 12.667;
			//editor.TextChanged += OnTextChanged;
			//editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;

			//timer = StoppableTimer.StartNew(
			//	TimeSpan.FromMilliseconds(500),
			//	DispatcherPriority.ApplicationIdle,
			//	TimerUpdate);
			TextOptions.SetTextFormattingMode(editor, TextFormattingMode.Display);
			editor.IsModified = false;
			editor.Focus();
			editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy();
			editor.Options.ConvertTabsToSpaces = false;
			editor.TextArea.TextView.LineSpacing = 17.0 / 15.0;


			//scriptEditor.Completion = completion;
			//scriptEditor.Document.FileName = "dummyFileName.cs";
			//scriptEditor.Script = null;
			//scriptEditor.EditorControl = editorControl;
			
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
			// Update the model
			model.SetObject(obj);

			if (model.Triggers.Count >= 0)
				listBoxTriggers.SelectedIndex = 0;

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
					//scriptEditor.Trigger = null;
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
					//scriptEditor.Trigger = model.SelectedTrigger;
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
