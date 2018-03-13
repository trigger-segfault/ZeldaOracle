using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using Microsoft.Win32;
using ZeldaEditor;
using ZeldaEditor.Control;
using ZeldaEditor.Controls;
using ZeldaEditor.PropertiesEditor;
using ZeldaEditor.Scripting;
using ZeldaEditor.Tools;
using ZeldaEditor.TreeViews;
using ZeldaEditor.Undo;
using ZeldaEditor.Util;
using ZeldaEditor.Windows;
using ZeldaEditor.WinForms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class EditorWindow : Window {

		private LevelDisplay		levelDisplay;
		private TilesetDisplay		tilesetDisplay;
		private TilePreview         tilePreview;
		private EditorControl		editorControl;

		private ToggleButton[]      toolButtons;

		private HistoryWindow		historyWindow;
		private RefactorWindow      refactorWindow;
		private ObjectEditor		objectWindow;

		private bool suppressEvents = false;

		private StoppableTimer updateTimer;


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
			levelDisplay					= new LevelDisplay();
			levelDisplay.EditorControl		= editorControl;
			levelDisplay.Name				= "levelDisplay";
			levelDisplay.Dock				= System.Windows.Forms.DockStyle.Fill;
			levelDisplay.EditorWindow		= this;
			hostLevelDisplay.Child			= levelDisplay;

			// Create the tileset display.
			tilesetDisplay					= new TilesetDisplay();
			tilesetDisplay.EditorControl	= editorControl;
			tilesetDisplay.Name				= "tilesetDisplay";
			tilesetDisplay.Dock				= System.Windows.Forms.DockStyle.Fill;
			tilesetDisplay.EditorWindow		= this;
			tilesetDisplay.HoverChanged		+= OnTilesetDisplayHoverChanged;
			hostTilesetDisplay.Child		= tilesetDisplay;

			// Create the tile preview
			tilePreview						= new TilePreview();
			tilePreview.EditorControl		= editorControl;
			tilePreview.Name				= "tilePreview";
			tilePreview.Dock				= System.Windows.Forms.DockStyle.Fill;
			hostTilePreview.Child			= tilePreview;
			
			statusTask.Content = "";

			// Setup layer combo-box.
			comboBoxLayers.Items.Clear();

			// Create tools.
			toolButtons = new ToggleButton[] {
				buttonToolPointer,
				buttonToolPan,
				buttonToolPlace,
				buttonToolSquare,
				buttonToolFill,
				buttonToolSelection,
				buttonToolEyedropper
			};

			UpdatePropertyPreview(null);

			suppressEvents = true;

			updateTimer = StoppableTimer.StartNew(
				TimeSpan.FromSeconds(0.4),
				DispatcherPriority.ApplicationIdle,
				Update);
			/*updateTimer = new DispatcherTimer(
				TimeSpan.FromSeconds(0.4),
				DispatcherPriority.ApplicationIdle,
				delegate { Update(); },
				Dispatcher);*/

			Application.Current.Activated += OnApplicationActivated;
			Application.Current.Deactivated += OnApplicationDeactivated;
		}
		
		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		private void Update() {
			statusFPS.Content = "FPS " + levelDisplay.FPS.ToString("0.0");
		}

		private void OnWindowLoaded(object sender, RoutedEventArgs e) {
			editorControl.Initialize();
			buttonToolPointer.Tag = editorControl.ToolPointer;
			buttonToolPan.Tag = editorControl.ToolPan;
			buttonToolPlace.Tag = editorControl.ToolPlace;
			buttonToolSquare.Tag = editorControl.ToolSquare;
			buttonToolFill.Tag = editorControl.ToolFill;
			buttonToolSelection.Tag = editorControl.ToolSelection;
			buttonToolEyedropper.Tag = editorControl.ToolEyedropper;

			UpdateCurrentTool();
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
			if (editorControl.IsUntitled)
				ShowSaveWorldDialog(); // Open Save as dialogue
			else
				editorControl.SaveWorld(); // Save to file.
		}

		// Open a save file dialogue to save the world.
		private void ShowSaveWorldDialog() {
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "Zelda world files (*.zwd)|*.zwd";
			dialog.ValidateNames = true;

			var result = dialog.ShowDialog(this);
			if (result.HasValue && result.Value) {
				Console.WriteLine("Saving file as " + dialog.FileName + ".");
				editorControl.SaveWorldAs(dialog.FileName);
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
				editorControl.OpenWorld(dialog.FileName);
			}
		}

		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (PromptSaveChanges() == MessageBoxResult.Cancel)
				e.Cancel = true;
		}

		private void OnApplicationActivated(object sender, EventArgs e) {
			editorControl.IsActive = true;
		}

		private void OnApplicationDeactivated(object sender, EventArgs e) {
			editorControl.IsActive = false;
		}

		//-----------------------------------------------------------------------------
		// Window Event Handlers
		//-----------------------------------------------------------------------------
		
		private void OnSingleLayerChanged(object sender, RoutedEventArgs e) {
			editorControl.ToolOptionSingleLayer = buttonToolSingleLayer.IsChecked.Value;
		}

		private void OnRoomOnlyChanged(object sender, RoutedEventArgs e) {
			editorControl.ToolOptionRoomOnly = buttonToolRoomOnly.IsChecked.Value;
		}

		private void OnMergeChanged(object sender, RoutedEventArgs e) {
			editorControl.ToolOptionMerge = buttonToolMerge.IsChecked.Value;
		}

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

		private void OnShowExtrasChecked(object sender, RoutedEventArgs e) {
			editorControl.ShowRewards = dropDownItemShowExtras.IsChecked;
		}

		private void OnShowStartLocationChecked(object sender, RoutedEventArgs e) {
			editorControl.ShowStartLocation = dropDownItemShowStartLocation.IsChecked;
		}

		private void OnShowRoomBordersChecked(object sender, RoutedEventArgs e) {
			editorControl.RoomSpacing = dropDownItemShowRoomBorders.IsChecked ? 1 : 0;
		}

		private void OnShowActionsChecked(object sender, RoutedEventArgs e) {
			editorControl.ShowActions = dropDownItemShowActions.IsChecked;
		}

		private void OnLayerChanged(object sender, SelectionChangedEventArgs e) {
			if (!suppressEvents) return;
			if (comboBoxLayers.SelectedIndex == comboBoxLayers.Items.Count - 1) {
				editorControl.ActionMode = true;
			}
			else {
				editorControl.ActionMode = false;
				editorControl.CurrentLayer = comboBoxLayers.SelectedIndex;
			}
			levelDisplay.Focus();
		}

		private void OnToolChanged(object sender, RoutedEventArgs e) {
			editorControl.CurrentTool = (EditorTool) ((ToggleButton)sender).Tag;
		}
		
		private void OnTilesetChanged(object sender, SelectionChangedEventArgs e) {
			if (!suppressEvents) return;
			if (comboBoxTilesets.SelectedIndex != -1) {
				if (comboBoxTilesets.SelectedIndex == 0) {
					toolbarTileSearch.Visibility = Visibility.Visible;
					editorControl.UpdateTileSearch(textBoxTileSearch.Text);
				}
				else {
					toolbarTileSearch.Visibility = Visibility.Collapsed;
					editorControl.ChangeTileset(comboBoxTilesets.SelectedItem as string);
				}
			}
			levelDisplay.Focus();
		}

		private void OnZoneChanged(object sender, SelectionChangedEventArgs e) {
			if (!suppressEvents) return;
			if (comboBoxZones.SelectedIndex != -1) {
				editorControl.ChangeZone(comboBoxZones.SelectedItem as string);
			}
			levelDisplay.Focus();
		}

		private void OnTileSearchTextChanged(object sender, TextChangedEventArgs e) {
			editorControl.UpdateTileSearch(textBoxTileSearch.Text);
		}

		private void OnTilesetDisplayHoverChanged(object sender, EventArgs e) {
			BaseTileData tileData = tilesetDisplay.HoverTileData;
			if (tileData == null)
				textBlockTileName.Text = "";
			else
				textBlockTileName.Text = tileData.Name;
		}

		public void UpdatePropertyPreview(IPropertyObject obj) {
			if (objectWindow != null)
				objectWindow.SetObject(obj as BaseTileDataInstance);

			System.Windows.Controls.Image image = new System.Windows.Controls.Image();
			image.Stretch = Stretch.None;
			image.HorizontalAlignment = HorizontalAlignment.Left;
			image.VerticalAlignment = VerticalAlignment.Top;
			tilePreview.UpdateTile(null);
			if (obj is Room) {
				Room room = obj as Room;
				image.Source = EditorImages.Room;
				propertyPreviewImage.Content = image;
				propertyPreviewName.Text = "Room[" + room.Location.X + ", " + room.Location.Y + "]";
				hostTilePreview.Visibility = Visibility.Hidden;
			}
			else if (obj is Level) {
				image.Source = EditorImages.Level;
				propertyPreviewImage.Content = image;
				propertyPreviewName.Text = (obj as Level).ID;
				hostTilePreview.Visibility = Visibility.Hidden;
			}
			else if (obj is Area) {
				image.Source = EditorImages.Area;
				propertyPreviewImage.Content = image;
				propertyPreviewName.Text = (obj as Area).ID;
				hostTilePreview.Visibility = Visibility.Hidden;
			}
			else if (obj is World) {
				image.Source = EditorImages.World;
				propertyPreviewImage.Content = image;
				propertyPreviewName.Text = (obj as World).ID;
				hostTilePreview.Visibility = Visibility.Hidden;
			}
			else if (obj is BaseTileDataInstance) {
				hostTilePreview.Visibility = Visibility.Visible;
				BaseTileDataInstance tile = obj as BaseTileDataInstance;
				tilePreview.UpdateTile(tile);
				propertyPreviewImage.Content = null;
				propertyPreviewName.Text = tile.BaseData.Name;
			}
			else {
				propertyPreviewImage.Content = null;
				propertyPreviewName.Text = "";
			}
		}

		private void OnViewPathsCommands(object sender, ExecutedRoutedEventArgs e) {
			if (editorControl.EditingTileData is TileDataInstance) {
				var tile = (TileDataInstance) editorControl.EditingTileData;
				string oldPath = tile.Properties.GetString("path", "");
				string newPath = TilePathEditor.ShowEditor(this,
					editorControl, tile.Properties, "path");

				editorControl.PushPropertyAction(tile,
					"path", oldPath, newPath, ActionExecution.Execute);
			}
			else if (editorControl.EditingRoom != null) {
				TilePathEditor.ShowViewer(this, editorControl,
					editorControl.EditingRoom);
			}
		}

		private void OnObjectEditorCommand(object sender, ExecutedRoutedEventArgs e) {
			if (objectWindow == null) {
				objectWindow = new ObjectEditor(editorControl,
					editorControl.EditingTileData);
				objectWindow.Owner = this;
				objectWindow.Closed += delegate(object unused1, EventArgs unused2) {
					objectWindow = null;
				};
				objectWindow.Show();
			}
			else {
				objectWindow.Focus();
			}
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
			if (!suppressEvents) return;
			e.CanExecute = editorControl.IsModified;
		}

		private void OnSaveCommand(object sender, ExecutedRoutedEventArgs e) {
			SaveWorld();
		}

		private void OnSaveAsCommand(object sender, ExecutedRoutedEventArgs e) {
			ShowSaveWorldDialog();
		}

		private void CanExecuteIsWorldOpen(object sender, CanExecuteRoutedEventArgs e) {
			if (!suppressEvents) return;
			e.CanExecute = editorControl.IsWorldOpen;
		}
		private void CanExecuteIsLevelOpen(object sender, CanExecuteRoutedEventArgs e) {
			if (!suppressEvents) return;
			e.CanExecute = editorControl.IsLevelOpen;
		}

		private void OnCloseCommand(object sender, ExecutedRoutedEventArgs e) {
			if (PromptSaveChanges() != MessageBoxResult.Cancel) {
				editorControl.CloseWorld();
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

		private void OnStartLocationCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.StartLocationMode = !editorControl.StartLocationMode;
			menuItemStartLocation.IsChecked = editorControl.StartLocationMode;
			buttonStartLocation.IsChecked = editorControl.StartLocationMode;
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
			editorControl.CurrentTool.Deselect();
		}

		private void CanExecuteDeselect(object sender, CanExecuteRoutedEventArgs e) {
			if (!suppressEvents) return;
			e.CanExecute = editorControl.CurrentTool.CanDeleteDeselect;
		}

		public void FinishTestWorldFromLocation() {
			menuItemTestLevelPlace.IsChecked = false;
			buttonTestLevelPlace.IsChecked = false;
		}

		public void FinishStartLocation() {
			menuItemStartLocation.IsChecked = false;
			buttonStartLocation.IsChecked = false;
		}

		private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
			if (!(Keyboard.FocusedElement is TextBox)) {
				
				if (Keyboard.Modifiers == ModifierKeys.None) {
					foreach (EditorTool tool in editorControl.Tools) {
						if (e.Key == tool.HotKey) {
							editorControl.CurrentTool = tool;
							return;
						}
					}
				}

				// Force commands and inputs to execute even when another control may have a command using that key gesture
				foreach (InputBinding inputBinding in this.InputBindings) {
					KeyGesture keyGesture = inputBinding.Gesture as KeyGesture;
					if (keyGesture != null && keyGesture.Key == e.Key && keyGesture.Modifiers == Keyboard.Modifiers) {
						if (inputBinding.Command != null) {
							inputBinding.Command.Execute(0);
							e.Handled = true;
						}
					}
				}

				foreach (CommandBinding cb in this.CommandBindings) {
					RoutedCommand command = cb.Command as RoutedCommand;
					if (command != null) {
						foreach (InputGesture inputGesture in command.InputGestures) {
							KeyGesture keyGesture = inputGesture as KeyGesture;
							if (keyGesture != null && keyGesture.Key == e.Key && keyGesture.Modifiers == Keyboard.Modifiers) {
								command.Execute(0, this);
								e.Handled = true;
							}
						}
					}
				}
			}
		}

		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (!WpfHelper.IsDescendant(this, Keyboard.FocusedElement as DependencyObject) &&
				Keyboard.FocusedElement is TextBox)
			{
				Keyboard.Focus(this);
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
		private void OnAddNewAreaCommand(object sender, ExecutedRoutedEventArgs e) {
			EditorAction action = AddNewAreaWindow.Show(this, editorControl);
			if (action != null) {
				editorControl.PushAction(action, ActionExecution.Execute);
			}
		}
		private void OnAddNewScriptCommand(object sender, ExecutedRoutedEventArgs e) {
			Script script = new Script();
			bool result = ScriptEditor.ShowRegularEditor(this, script, editorControl, true);
			if (result) {
				EditorAction action = ActionChangeScript.CreateDefineScriptAction(script.ID, script.Code);
				editorControl.PushAction(action, ActionExecution.Execute);
			}
		}
		private void CanExecuteCycleLayerUp(object sender, CanExecuteRoutedEventArgs e) {
			if (!suppressEvents) return;
			e.CanExecute = editorControl.IsLevelOpen && !editorControl.ActionLayer;
		}
		private void CanExecuteCycleLayerDown(object sender, CanExecuteRoutedEventArgs e) {
			if (!suppressEvents) return;
			e.CanExecute = editorControl.IsLevelOpen && (editorControl.CurrentLayer > 0 || editorControl.ActionLayer);
		}

		private void OnCycleLayerUpCommand(object sender, ExecutedRoutedEventArgs e) {
			comboBoxLayers.SelectedIndex++;
		}

		private void OnCycleLayerDownCommand(object sender, ExecutedRoutedEventArgs e) {
			comboBoxLayers.SelectedIndex--;
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

		private void OnShowModifiedTilesChecked(object sender, RoutedEventArgs e) {
			editorControl.ShowModified = dropDownItemShowModified.IsChecked;
		}

		private void OnShowSharedTilesChecked(object sender, RoutedEventArgs e) {
			editorControl.ShowShared = dropDownItemShowShared.IsChecked;
		}

		private void CanExecuteViewPaths(object sender, CanExecuteRoutedEventArgs e) {
			if (!suppressEvents) return;
			e.CanExecute = (editorControl.EditingRoom != null ||
				editorControl.EditingTileData is TileDataInstance);
		}

		private void CanExecuteCopyCut(object sender, CanExecuteRoutedEventArgs e) {
			if (!suppressEvents) return;
			e.CanExecute = editorControl.IsLevelOpen && editorControl.CurrentTool.CanCopyCut;
		}
		private void CanExecuteDeleteDeselect(object sender, CanExecuteRoutedEventArgs e) {
			if (!suppressEvents) return;
			e.CanExecute = editorControl.IsLevelOpen && editorControl.CurrentTool.CanDeleteDeselect;
		}
		private void CanExecutePaste(object sender, CanExecuteRoutedEventArgs e) {
			if (!suppressEvents) return;
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
			if (!suppressEvents) return;
			e.CanExecute = editorControl.CanUndo;
		}

		private void OnUndoCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.Undo();
		}

		private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e) {
			if (!suppressEvents) return;
			e.CanExecute = editorControl.CanRedo;
		}

		private void OnRedoCommand(object sender, ExecutedRoutedEventArgs e) {
			editorControl.Redo();
		}
		

		private void OnDebugConsole(object sender, RoutedEventArgs e) {
			editorControl.DebugConsole = menuItemDebugConsole.IsChecked;
		}


		//-----------------------------------------------------------------------------
		// History Window
		//-----------------------------------------------------------------------------

		private void OnHistoryCommand(object sender, ExecutedRoutedEventArgs e) {
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
			// Hack to fix minimizing to Visual Studio after closing
			// a tool window that has called a message box.
			Activate();
		}


		//-----------------------------------------------------------------------------
		// Refactor Window
		//-----------------------------------------------------------------------------

		private void OnRefactorPropertiesCommand(object sender, ExecutedRoutedEventArgs e) {
			menuItemRefactorProperties.IsChecked = true;
			menuItemRefactorEvents.IsChecked = false;
			if (refactorWindow != null && refactorWindow.RefactorType == RefactorType.Events) {
				refactorWindow.Close();
			}
			if (refactorWindow == null) {
				refactorWindow = RefactorWindow.Show(this, editorControl, RefactorType.Properties, OnRefactorWindowClosed);
			}
		}
		private void OnRefactorEventsCommand(object sender, ExecutedRoutedEventArgs e) {
			menuItemRefactorProperties.IsChecked = false;
			menuItemRefactorEvents.IsChecked = true;
			if (refactorWindow != null && refactorWindow.RefactorType == RefactorType.Properties) {
				refactorWindow.Close();
			}
			if (refactorWindow == null) {
				refactorWindow = RefactorWindow.Show(this, editorControl, RefactorType.Events, OnRefactorWindowClosed);
			}
		}

		private void OnRefactorWindowClosed(object sender, EventArgs e) {
			if (refactorWindow.RefactorType == RefactorType.Properties)
				menuItemRefactorProperties.IsChecked = false;
			else
				menuItemRefactorEvents.IsChecked = false;
			refactorWindow = null;
			// HACK: Prevent minimizing to Visual Studio after closing
			// a tool window that has called a message box.
			Activate();
		}
		

		//-----------------------------------------------------------------------------
		// Interface Mutators
		//-----------------------------------------------------------------------------

		public void CloseAllToolWindows() {
			if (historyWindow != null)
				historyWindow.Close();
			if (refactorWindow != null)
				refactorWindow.Close();
		}

		public void SelectHistoryItem(HistoryListViewItem item) {
			if (historyWindow != null) {
				historyWindow.SelectItem(item);
			}
			else {
				item.IsSelected = true;
			}
		}

		public void UpdateCurrentTool() {
			for (int i = 0; i < toolButtons.Length; i++)
				toolButtons[i].IsChecked = (toolButtons[i].Tag == editorControl.CurrentTool);
			separatorToolOptions.Visibility = (editorControl.CurrentTool.Options.Any() ?
				Visibility.Visible : Visibility.Collapsed);
			for (int i = toolBar2.Items.IndexOf(separatorToolOptions) + 1; ; i++) {
				if (toolBar2.Items[i] is Separator)
					break;

				ToggleButton button = (ToggleButton) toolBar2.Items[i];
				string buttonName = button.Name.Replace("buttonTool", "");
				bool visible = editorControl.CurrentTool.Options.Contains(buttonName);
				button.Visibility = (visible ? Visibility.Visible : Visibility.Collapsed);
			}
		}

		public void UpdateCurrentLayer() {
			if (editorControl.ActionLayer)
				comboBoxLayers.SelectedIndex = comboBoxLayers.Items.Count - 1;
			else
				comboBoxLayers.SelectedIndex = editorControl.CurrentLayer;
		}

		// Status Bar -----------------------------------------------------------------

		public void SetStatusBarLevelLocations(Point2I roomLocation, Point2I tileLocation) {
			statusRoomLocation.Content = "Room " + roomLocation;
			statusTileLocation.Content = "Tile " + tileLocation;
		}

		public void SetStatusBarInvalidLevelLocations() {
			statusRoomLocation.Content = "Room (?, ?)";
			statusTileLocation.Content = "Tile (?, ?)";
		}

		public void SetStatusBarTask(string task) {
			statusTask.Content = task;
		}

		public void ClearStatusBarTask() {
			statusTask.Content = "";
		}
		
		// Combo Boxes ----------------------------------------------------------------

		public void SetTilesetsItemsSource(IEnumerable<string> tilesets, int selectedIndex) {
			suppressEvents = false;
			comboBoxTilesets.ItemsSource = tilesets;
			comboBoxTilesets.SelectedIndex = selectedIndex;
			suppressEvents = true;
		}

		public void SetZonesItemsSource(IEnumerable<string> zones, int selectedIndex) {
			suppressEvents = false;
			comboBoxZones.ItemsSource = zones;
			comboBoxZones.SelectedIndex = selectedIndex;
			suppressEvents = true;
		}

		public void SetLayersItemsSource(IEnumerable<string> layers, int selectedIndex) {
			suppressEvents = false;
			comboBoxLayers.ItemsSource = layers;
			comboBoxLayers.SelectedIndex = selectedIndex;
			suppressEvents = true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorControl EditorControl {
			get { return editorControl; }
		}

		public HistoryWindow HistoryWindow {
			get { return historyWindow; }
		}
		
		public LevelDisplay LevelDisplay {
			get { return levelDisplay; }
		}

		public TilesetDisplay TilesetDisplay {
			get { return tilesetDisplay; }
		}

		public WorldTreeView WorldTreeView {
			get { return treeViewWorld; }
		}

		public ZeldaPropertyGrid PropertyGrid {
			get { return propertyGrid; }
		}
	}
}
