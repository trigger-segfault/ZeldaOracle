using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using ZeldaEditor;
using ZeldaEditor.Control;
using ZeldaEditor.Controls;
using ZeldaEditor.PropertiesEditor;
using ZeldaEditor.Scripting;
using ZeldaEditor.Tools;
using ZeldaEditor.TreeViews;
using ZeldaEditor.Undo;
using ZeldaEditor.Windows;
using ZeldaEditor.WinForms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class EditorWindow : Window {

		private LevelDisplay        levelDisplay;
		private TileDisplay         tileDisplay;
		private EditorControl       editorControl;

		private ToggleButton[]      toolButtons;

		private HistoryWindow		historyWindow;

		private bool loaded = false;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public EditorWindow() {
			InitializeComponent();
			// Create the editor control instance.
			editorControl = new EditorControl(this);

			// Initialize world tree view.
			treeViewWorld.EditorControl = editorControl;

			// Initialize property grid.
			propertyGrid.Initialize(editorControl);

			ScriptEditor.Initialize();

			// Create the level display.
			levelDisplay                = new LevelDisplay();
			levelDisplay.EditorControl  = editorControl;
			levelDisplay.Name           = "levelDisplay";
			levelDisplay.Dock           = System.Windows.Forms.DockStyle.Fill;
			levelDisplay.EditorWindow   = this;
			hostLevelDisplay.Child = levelDisplay;
			
			// Create the tileset display.
			tileDisplay                 = new TileDisplay();
			tileDisplay.EditorControl   = editorControl;
			tileDisplay.Name            = "tileDisplay";
			tileDisplay.Dock            = System.Windows.Forms.DockStyle.Fill;
			tileDisplay.EditorWindow    = this;
			hostTileDisplay.Child = tileDisplay;

			statusTask.Content = "";

			// Setup layer combo-box.
			comboBoxLayers.Items.Clear();
			comboBoxLayers.Items.Add("Layer 1");
			comboBoxLayers.Items.Add("Layer 2");
			comboBoxLayers.Items.Add("Layer 3");
			comboBoxLayers.Items.Add("Events");
			comboBoxLayers.SelectedIndex = 0;

			// Create tools.
			toolButtons = new ToggleButton[] {
				buttonToolPointer,
				buttonToolPlace,
				buttonToolSquare,
				buttonToolFill,
				buttonToolSelection,
				buttonToolEyedropper
			};

			UpdatePropertyPreview(null);

			loaded = true;

			UpdateLayers();
		}

		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		private void OnWindowLoaded(object sender, RoutedEventArgs e) {
			
		}

		// Prompt the user to save unsaved changes if there are any. Returns
		// the result of the prompt dialogue (yes/no/cancel), or 'yes' if
		// there were no unsaved changes.
		private MessageBoxResult PromptSaveChanges() {
			if (!editorControl.IsWorldOpen || !editorControl.IsModified)
				return MessageBoxResult.Yes;

			// Show the dialogue.
			string worldName = editorControl.WorldFileName;
			MessageBoxResult result = TriggerMessageBox.Show(this, MessageIcon.Warning,
				"Do you want to save changes to " + worldName + "?", "Unsaved Changes",
				MessageBoxButton.YesNoCancel);

			if (result == MessageBoxResult.Yes)
				SaveWorld();

			return result;
		}

		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------
		

		// Attempt to save the world automatically first, or open a dialogue
		// if the world isn't from a file.
		private void SaveWorld() {
			if (editorControl.IsWorldFromFile)
				editorControl.SaveFileAs(editorControl.WorldFilePath); // Save to file.
			else
				ShowSaveWorldDialog(); // Open Save as dialogue
		}

		// Open a save file dialogue to save the world.
		private void ShowSaveWorldDialog() {
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "Zelda world files (*.zwd)|*.zwd";
			dialog.ValidateNames = true;

			var result = dialog.ShowDialog(this);
			if (result.HasValue && result.Value) {
				Console.WriteLine("Saving file as " + dialog.FileName + ".");
				editorControl.SaveFileAs(dialog.FileName);
			}
		}

		// Open an open file dialogue to open a world file.
		private void ShowOpenWorldDialog() {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.DereferenceLinks = true;
			dialog.Filter = "Zelda world files (*.zwd)|*.zwd";
			dialog.CheckFileExists = true;

			var result = dialog.ShowDialog(this);
			if (result.HasValue && result.Value) {
				Console.WriteLine("Opened file " + dialog.FileName + ".");
				editorControl.OpenFile(dialog.FileName);
			}
		}

		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (PromptSaveChanges() == MessageBoxResult.Cancel)
				e.Cancel = true;
		}

		public void OnToolChange(int toolIndex) {
			for (int i = 0; i < toolButtons.Length; i++)
				toolButtons[i].IsChecked = (i == toolIndex);
		}

		//-----------------------------------------------------------------------------
		// Window Event Handlers
		//-----------------------------------------------------------------------------

		private void OnVisualsBelowChecked(object sender, RoutedEventArgs e) {
			dropDownItemHideBelow.IsChecked = false;
			dropDownItemFadeBelow.IsChecked = false;
			dropDownItemShowBelow.IsChecked = false;
			string tag = (sender as FrameworkElement).Tag as string;
			editorControl.BelowTileDrawMode = (TileDrawModes)Enum.Parse(typeof(TileDrawModes), tag);
			switch (editorControl.BelowTileDrawMode) {
			case TileDrawModes.Hide: dropDownItemHideBelow.IsChecked = true; break;
			case TileDrawModes.Fade: dropDownItemFadeBelow.IsChecked = true; break;
			case TileDrawModes.Show: dropDownItemShowBelow.IsChecked = true; break;
			}
		}

		private void OnVisualsAboveChecked(object sender, RoutedEventArgs e) {
			dropDownItemHideAbove.IsChecked = false;
			dropDownItemFadeAbove.IsChecked = false;
			dropDownItemShowAbove.IsChecked = false;
			string tag = (sender as FrameworkElement).Tag as string;
			editorControl.AboveTileDrawMode = (TileDrawModes)Enum.Parse(typeof(TileDrawModes), tag);
			switch (editorControl.AboveTileDrawMode) {
			case TileDrawModes.Hide: dropDownItemHideAbove.IsChecked = true; break;
			case TileDrawModes.Fade: dropDownItemFadeAbove.IsChecked = true; break;
			case TileDrawModes.Show: dropDownItemShowAbove.IsChecked = true; break;
			}
		}

		private void OnShowRewardsChecked(object sender, RoutedEventArgs e) {
			editorControl.ShowRewards = dropDownItemShowRewards.IsChecked;
		}

		private void OnShowRoomBordersChecked(object sender, RoutedEventArgs e) {
			editorControl.RoomSpacing = dropDownItemShowRoomBorders.IsChecked ? 1 : 0;
		}

		private void OnShowEventsChecked(object sender, RoutedEventArgs e) {
			editorControl.ShowEvents = dropDownItemShowEvents.IsChecked;
		}

		private void OnShowGridChecked(object sender, RoutedEventArgs e) {
			editorControl.ShowGrid = buttonShowGrid.IsChecked.Value;
		}

		private void OnPlayAnimationsChecked(object sender, RoutedEventArgs e) {
			editorControl.PlayAnimations = buttonPlayAnimations.IsChecked.Value;
		}

		private void OnLayerChanged(object sender, SelectionChangedEventArgs e) {
			if (!loaded) return;
			if (comboBoxLayers.SelectedIndex == comboBoxLayers.Items.Count - 1) {
				editorControl.EventMode = true;
			}
			else {
				editorControl.EventMode = false;
				editorControl.CurrentLayer = comboBoxLayers.SelectedIndex;
				if (editorControl.CurrentTool != null)
					editorControl.CurrentTool.OnChangeLayer();
			}
			// TODO: Reimplement
			//if (editorControl.PropertyGrid != null)
			//	editorControl.PropertyGrid.CloseProperties();
			levelDisplay.Focus();
		}

		private void OnToolChanged(object sender, RoutedEventArgs e) {
			for (int i = 0; i < toolButtons.Length; i++) {
				if (toolButtons[i] == sender)
					editorControl.ChangeTool(i);
			}
		}

		public StatusBarItem StatusBarLabelRoomLoc { get { return statusRoomLocation; } }
		public StatusBarItem StatusBarLabelTileLoc { get { return statusTileLocation; } }
		public StatusBarItem StatusBarLabelTask { get { return statusTask; } }
		public ToggleButton ButtonTestLevelPlace { get { return buttonTestLevelPlace; } }
		public ComboBox ComboBoxTilesets { get { return comboBoxTilesets; } }
		public ComboBox ComboBoxZones { get { return comboBoxZones; } }

		public LevelDisplay LevelDisplay { get { return levelDisplay; } }
		public TileDisplay TileDisplay { get { return tileDisplay; } }

		public WorldTreeView TreeViewWorld { get { return treeViewWorld; } }

		public ZeldaPropertyGrid PropertyGrid { get { return propertyGrid; } }
		
		private void OnTilesetChanged(object sender, SelectionChangedEventArgs e) {
			if (!loaded) return;
			if (comboBoxTilesets.SelectedIndex != -1) {
				editorControl.ChangeTileset(comboBoxTilesets.SelectedItem as string);
			}
			levelDisplay.Focus();
		}

		private void OnZoneChanged(object sender, SelectionChangedEventArgs e) {
			if (!loaded) return;
			if (comboBoxZones.SelectedIndex != -1) {
				editorControl.ChangeZone(comboBoxZones.SelectedItem as string);
			}
			levelDisplay.Focus();
		}

		private string SurroundID(string id) {
			if (string.IsNullOrWhiteSpace(id))
				return "";
			else
				return " \"" + id + "\"";
		}

		public void UpdatePropertyPreview(IPropertyObject obj) {
			System.Windows.Controls.Image image = new System.Windows.Controls.Image();
			image.Stretch = Stretch.None;
			image.HorizontalAlignment = HorizontalAlignment.Left;
			image.VerticalAlignment = VerticalAlignment.Top;
			Canvas canvas = null;
			if (obj is Room) {
				image.Source = EditorImages.Room;
				propertyPreviewImage.Content = image;
				propertyPreviewName.Text = "Room" + SurroundID((obj as Room).ID);
			}
			else if (obj is Level) {
				image.Source = EditorImages.Level;
				propertyPreviewImage.Content = image;
				propertyPreviewName.Text = "Level" + SurroundID((obj as Level).ID);
			}
			else if (obj is Dungeon) {
				image.Source = EditorImages.Dungeon;
				propertyPreviewImage.Content = image;
				propertyPreviewName.Text = "Dungeon" + SurroundID((obj as Dungeon).ID);
			}
			else if (obj is World) {
				image.Source = EditorImages.World;
				propertyPreviewImage.Content = image;
				propertyPreviewName.Text = "World" + SurroundID((obj as World).ID);
			}
			else if (obj is Zone) {
				image.Source = null;
				propertyPreviewImage.Content = image;
				propertyPreviewName.Text = "Zone" + SurroundID((obj as Zone).ID);
			}
			else if (obj is TileDataInstance) {
				TileDataInstance tile = obj as TileDataInstance;
				SpriteAnimation currentSprite = tile.CurrentSprite;
				if (currentSprite.IsSprite)
					canvas = EditorResources.GetSprite(currentSprite.Sprite, tile.Room.Zone.ImageVariantID);
				else if (currentSprite.IsAnimation)
					canvas = EditorResources.GetAnimation(currentSprite.Animation, tile.Room.Zone.ImageVariantID);
				if (canvas != null) {
					canvas.Height = 16;
					canvas.MinWidth = 16;
					canvas.MaxWidth = 32;
				}
				propertyPreviewImage.Content = canvas;
				string typeName = (tile.Type != null ? tile.Type.Name : "Tile");
				propertyPreviewName.Text = typeName + SurroundID(tile.ID);
			}
			else if (obj is EventTileDataInstance) {
				EventTileDataInstance tile = obj as EventTileDataInstance;
				SpriteAnimation currentSprite = tile.CurrentSprite;
				if (currentSprite.IsSprite)
					canvas = EditorResources.GetSprite(currentSprite.Sprite, tile.Room.Zone.ImageVariantID);
				else if (currentSprite.IsAnimation)
					canvas = EditorResources.GetAnimation(currentSprite.Animation, tile.Room.Zone.ImageVariantID);
				if (canvas != null) {
					canvas.Height = 16;
					canvas.MinWidth = 16;
					canvas.MaxWidth = 32;
				}
				propertyPreviewImage.Content = canvas;
				string typeName = (tile.Type != null ? tile.Type.Name : "EventTile");
				propertyPreviewName.Text = typeName + SurroundID(tile.ID);
			}
			else {
				propertyPreviewImage.Content = null;
				propertyPreviewName.Text = "";
			}
		}

		private void OnTestLevelPlace(object sender, RoutedEventArgs e) {
			editorControl.PlayerPlaceMode = buttonTestLevelPlace.IsChecked.Value;
		}

		private void OnTestWorld(object sender, RoutedEventArgs e) {
			editorControl.TestWorld();
		}

		private void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}

		private void OnNewCommand(object sender, ExecutedRoutedEventArgs e) {
			if (PromptSaveChanges() != MessageBoxResult.Cancel) {
				// TODO: New World
			}
		}

		private void OnOpenCommand(object sender, ExecutedRoutedEventArgs e) {
			if (PromptSaveChanges() != MessageBoxResult.Cancel) {
				ShowOpenWorldDialog();
			}
		}


		private void CanExecuteIsModified(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = editorControl.IsModified;
		}

		private void OnSaveCommand(object sender, ExecutedRoutedEventArgs e) {
			SaveWorld();
		}

		private void OnSaveAsCommand(object sender, ExecutedRoutedEventArgs e) {
			ShowSaveWorldDialog();
		}

		private void CanExecuteIsWorldOpen(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = editorControl.IsWorldOpen;
		}
		private void CanExecuteIsLevelOpen(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = editorControl.IsLevelOpen;
		}

		private void OnCloseCommand(object sender, ExecutedRoutedEventArgs e) {
			if (PromptSaveChanges() != MessageBoxResult.Cancel) {
				editorControl.CloseFile();
			}
		}

		private void OnExitCommand(object sender, ExecutedRoutedEventArgs e) {
			Close();
		}

		private void OnTestWorldCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.TestWorld();
		}
		private void OnTestWorldFromLocationCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.PlayerPlaceMode = !editorControl.PlayerPlaceMode;
			menuItemTestLevelPlace.IsChecked = editorControl.PlayerPlaceMode;
			buttonTestLevelPlace.IsChecked = editorControl.PlayerPlaceMode;
		}
		private void OnShowGridCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.ShowGrid = !editorControl.ShowGrid;
			menuItemShowGrid.IsChecked = editorControl.ShowGrid;
			buttonShowGrid.IsChecked = editorControl.ShowGrid;
		}
		private void OnPlayAnimationsCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.PlayAnimations = !editorControl.PlayAnimations;
			menuItemPlayAnimations.IsChecked = editorControl.PlayAnimations;
			buttonPlayAnimations.IsChecked = editorControl.PlayAnimations;
		}
		private void OnDeselectCommand(object sender, ExecutedRoutedEventArgs e) {
			levelDisplay.DeselectSelectionGrid();
			editorControl.CurrentTool.Deselect();
		}
		private void CanExecuteDeselect(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = levelDisplay.SelectedTiles.Any();
		}

		public void FinishTestWorldFromLocation() {
			menuItemTestLevelPlace.IsChecked = false;
			buttonTestLevelPlace.IsChecked = false;
		}

		private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
			if (!(FocusManager.GetFocusedElement(this) is TextBox) && Keyboard.Modifiers == ModifierKeys.None) {
				switch (e.Key) {
				case Key.M: OnToolChanged(buttonToolPointer, null); break;
				case Key.P: OnToolChanged(buttonToolPlace, null); break;
				case Key.O: OnToolChanged(buttonToolSquare, null); break;
				case Key.F: OnToolChanged(buttonToolFill, null); break;
				case Key.S: OnToolChanged(buttonToolSelection, null); break;
				case Key.K: OnToolChanged(buttonToolEyedropper, null); break;
				}
			}
		}

		private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e) {
			columnTreeView.MaxWidth = ActualWidth / 2 - 20;
			columnProperties.MaxWidth = ActualWidth / 2 - 20;
		}

		private void OnAddNewLevelCommand(object sender, ExecutedRoutedEventArgs e) {
			EditorAction action = AddNewLevelWindow.Show(this, editorControl);
			if (action != null) {
				editorControl.PushAction(action, ActionExecution.Execute);
			}
		}
		private void OnAddNewDungeonCommand(object sender, ExecutedRoutedEventArgs e) {
			EditorAction action = AddNewDungeonWindow.Show(this, editorControl);
			if (action != null) {
				editorControl.PushAction(action, ActionExecution.Execute);
				/*editorControl.AddDungeon(dungeon, true);
				treeViewWorld.RefreshDungeons();*/
			}
		}
		private void OnAddNewScriptCommand(object sender, ExecutedRoutedEventArgs e) {
			Script script = new Script();
			bool result = ScriptEditor.Show(this, script, EditorControl.Instance, true);
			if (result) {
				editorControl.AddScript(script);
				treeViewWorld.RefreshScripts();
			}
		}
		private void CanExecuteCycleLayerUp(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = editorControl.IsLevelOpen && (editorControl.CurrentLayer > 0 || editorControl.EventMode);
		}
		private void CanExecuteCycleLayerDown(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = editorControl.IsLevelOpen && (editorControl.CurrentLayer + 1 < editorControl.Level.RoomLayerCount || !editorControl.EventMode);
		}

		private void OnCycleLayerUpCommand(object sender, ExecutedRoutedEventArgs e) {
			comboBoxLayers.SelectedIndex--;
		}

		private void OnCycleLayerDownCommand(object sender, ExecutedRoutedEventArgs e) {
			comboBoxLayers.SelectedIndex++;
		}

		private void OnResizeLevelCommand(object sender, ExecutedRoutedEventArgs e) {
			Level level = (e.Parameter as Level ?? editorControl.Level);
			Point2I dimensions = level.Dimensions;
			bool result = ResizeLevelWindow.Show(this, ref dimensions);
			if (result) {
				EditorAction action = new ActionResizeLevel(level, dimensions);
				editorControl.PushAction(action, ActionExecution.Execute);
			}
		}
		private void OnShiftLevelCommand(object sender, ExecutedRoutedEventArgs e) {
			Level level = (e.Parameter as Level ?? editorControl.Level);
			Point2I distance;
			bool result = ShiftLevelWindow.Show(this, out distance);
			if (result) {
				EditorAction action = new ActionShiftLevel(level, distance);
				editorControl.PushAction(action, ActionExecution.Execute);
			}
		}

		public void UpdateLayers() {
			loaded = false;
			comboBoxLayers.Items.Clear();
			if (editorControl.IsLevelOpen) {
				for (int i = 0; i < editorControl.Level.RoomLayerCount; i++) {
					comboBoxLayers.Items.Add("Layer " + (i + 1));
				}
				comboBoxLayers.Items.Add("Events");

				if (editorControl.EventMode)
					comboBoxLayers.SelectedIndex = comboBoxLayers.Items.Count - 1;
				else
					comboBoxLayers.SelectedIndex = editorControl.CurrentLayer;
			}

			loaded = true;
		}

		private void OnShowModifiedTilesChecked(object sender, RoutedEventArgs e) {
			editorControl.ShowModified = dropDownItemShowModified.IsChecked;
		}

		private void CanExecuteCopyCut(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = editorControl.IsLevelOpen && editorControl.CurrentTool.CanCopyCut;
		}
		private void CanExecuteDeleteDeselect(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = editorControl.IsLevelOpen && editorControl.CurrentTool.CanDeleteDeselect;
		}
		private void CanExecutePaste(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = editorControl.ToolSelection.CanPaste;
		}
		private void OnCopyCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.CurrentTool.Copy();
		}

		private void OnCutCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.CurrentTool.Cut();
		}

		private void OnPasteCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.CurrentTool = editorControl.ToolSelection;
			editorControl.CurrentTool.Paste();
		}

		private void OnDeleteCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.CurrentTool.Delete();
		}

		private void OnSelectAllCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.CurrentTool = editorControl.ToolSelection;
			editorControl.CurrentTool.SelectAll();
		}

		private void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = editorControl.CanUndo;
		}

		private void OnUndoCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.Undo();
		}

		private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e) {
			if (!loaded) return;
			e.CanExecute = editorControl.CanRedo;
		}

		private void OnRedoCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.Redo();
		}
		
		private void OnShowHistory(object sender, RoutedEventArgs e) {
			if (historyWindow == null) {
				historyWindow = HistoryWindow.Show(this, editorControl, OnHistoryWindowClosed);
				buttonShowHistory.IsChecked = true;
			}
			else {
				historyWindow.Close();
				buttonShowHistory.IsChecked = false;
			}
		}
		private void OnHistoryWindowClosed(object sender, EventArgs e) {
			historyWindow = null;
			buttonShowHistory.IsChecked = false;
		}

		public HistoryWindow HistoryWindow {
			get { return historyWindow; }
		}

		public void SelectHistoryItem(HistoryListViewItem item) {
			if (historyWindow != null) {
				historyWindow.SelectItem(item);
			}
			else {
				item.IsSelected = true;
			}
		}
	}
}
