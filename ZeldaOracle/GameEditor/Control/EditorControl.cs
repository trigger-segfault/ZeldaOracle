using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Trigger = ZeldaOracle.Common.Scripting.Trigger;
using TriggerCollection = ZeldaOracle.Common.Scripting.TriggerCollection;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Translation;

using ZeldaOracle.Game;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Worlds;

using ZeldaEditor.PropertiesEditor;
using ZeldaEditor.Tools;
using ZeldaEditor.TreeViews;
using ZeldaEditor.Undo;
using ZeldaEditor.Windows;
using ZeldaEditor.WinForms;
using ZeldaOracle.Common.Graphics;
using ZeldaEditor.Util;
using ZeldaOracle.Common.Util;

namespace ZeldaEditor.Control {

	public class EditorControl {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private const int MaxUndos = 50;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		// Control
		private bool isInitialized;
		private bool isActive;
		private EditorWindow        editorWindow;
		private string              worldFilePath;
		/// <summary>Set to true during world file load if the world should be flagged
		/// as modified because a resource was located.</summary>
		private bool                worldFileLocatedResource;
		private RewardManager       rewardManager;
		private Inventory           inventory;

		private Stopwatch           timer;
		private int                 ticks;
		private StoppableTimer		updateTimer;

		// Settings
		private bool                playAnimations;
		private bool                actionMode;
		private int					previousToolIndex;
		private TileDrawModes		aboveTileDrawMode;
		private TileDrawModes		belowTileDrawMode;
		private bool				showRewards;
		private bool				showGrid;
		private bool				showModified;
		private bool				showStartLocation;
		private bool				showActions;
		private bool				showShared;
		private bool				singleLayer;
		private bool				roomOnly;
		private bool				merge;

		// Debug
		private bool                debugConsole;

		// Tools
		private List<EditorTool>    tools;
		private ToolPointer         toolPointer;
		private ToolPan             toolPan;
		private ToolPlace           toolPlace;
		private ToolSquare          toolSquare;
		private ToolFill            toolFill;
		private ToolSelection       toolSelection;
		private ToolEyedrop         toolEyedropper;
		private ObservableCollection<EditorAction> undoActions;
		private int                 undoPosition;

		// Editing
		private World			world;
		private Level			level;
		private Zone			zone;
		private bool            isModified;
		private int             roomSpacing;
		private int             currentLayer;
		private int             currentToolIndex;
		private bool            highlightMouseTile;
		private Tileset         selectedTileset;
		private Point2I         selectedTilesetLocation;
		private BaseTileData    selectedTileData;
		private bool            playerPlaceMode;
		private bool            startLocationMode;
		private Room editingRoom;
		private BaseTileDataInstance editingTileData;
		
		// Scripts
		private ScriptCompileService		scriptCompileService;
		private bool                        needsRecompiling;
		private ScriptCompileResult			scriptCompileResult;
		private Task<List<Event>>           eventCacheTask;
		private List<Event>                 eventCache;
		private bool                        needsNewEventCache;


		private HashSet<DispatcherTimer>	scheduledEvents;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EditorControl(EditorWindow editorWindow) {
			this.editorWindow       = editorWindow;

			isActive				= true;
			worldFilePath			= string.Empty;
			rewardManager			= null;
			inventory				= null;
			timer					= null;
			ticks					= 0;
			isInitialized			= false;

			// Settings
			roomSpacing				= 1;
			playAnimations			= false;
			aboveTileDrawMode		= TileDrawModes.Fade;
			belowTileDrawMode		= TileDrawModes.Fade;
			showRewards				= true;
			showGrid				= false;
			showModified			= false;
			showActions				= false;
			showShared				= true;
			highlightMouseTile		= true;
			playerPlaceMode			= false;
			startLocationMode		= false;
			showStartLocation		= true;
			singleLayer				= false;
			roomOnly				= false;
			merge					= false;

			// Editing
			isModified				= false;
			world					= null;
			level					= null;
			zone					= null;
			currentLayer			= 0;
			currentToolIndex		= 0;
			previousToolIndex		= 0;
			selectedTileset			= null;
			selectedTilesetLocation	= Point2I.Zero;
			selectedTileData		= null;

			// Scripts
			scriptCompileService	= new ScriptCompileService();
			needsRecompiling		= false;
			scriptCompileResult		= null;
			eventCache				= new List<Event>();
			eventCacheTask			= null;
			needsNewEventCache		= false;
		}

		public void Initialize() {
			// Create tools
			tools = new List<EditorTool>();
			AddTool(toolPointer     = new ToolPointer());
			AddTool(toolPan         = new ToolPan());
			AddTool(toolPlace       = new ToolPlace());
			AddTool(toolSquare      = new ToolSquare());
			AddTool(toolFill        = new ToolFill());
			AddTool(toolSelection   = new ToolSelection());
			AddTool(toolEyedropper  = new ToolEyedrop());
			currentToolIndex = 0;
			tools[currentToolIndex].Begin();

			undoActions = new ObservableCollection<EditorAction>();
			undoPosition = -1;

			try {
				GameData.Initialize();

				this.inventory      = new Inventory();
				this.rewardManager  = new RewardManager(inventory);
				this.timer          = Stopwatch.StartNew();
				this.ticks          = 0;
				this.roomSpacing    = 1;
				this.playAnimations = false;
				this.zone           = GameData.ZONE_DEFAULT;
				this.selectedTileData = null;
				this.actionMode      = false;

				inventory.Initialize();
				rewardManager.Initialize();
			}
			catch (Exception ex) {
				StoppableTimer.StopAll();
				ShowExceptionMessage(ex, "load", "resources");
				Environment.Exit(-1);
			}
			EditorResources.Initialize(this);

			this.updateTimer = StoppableTimer.StartNew(
				TimeSpan.FromMilliseconds(100),
				DispatcherPriority.ApplicationIdle,
				Update);
			/*this.updateTimer		= new DispatcherTimer(
				TimeSpan.FromMilliseconds(100),
				DispatcherPriority.ApplicationIdle,
				delegate { Update(); },
				Application.Current.Dispatcher);*/

			isInitialized = true;


			needsNewEventCache = true;
			needsRecompiling = true;

			string[] args = Environment.GetCommandLineArgs();
			if (args.Length > 1) {
				if (args[1] == "-dev") {
					DevSettings settings = new DevSettings();
					if (settings.Load() && !string.IsNullOrEmpty(settings.StartLocation.WorldFile)) {
						OpenWorld(settings.StartLocation.WorldFile);
					}
				}
				else {
					OpenWorld(args[1]);
				}
			}
		}

		public void Uninitialize() {
			foreach (DispatcherTimer timer in scheduledEvents) {
				timer.Stop();
			}
			scheduledEvents.Clear();
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Called with Application.Idle.</summary>
		private void Update() {
			UpdateScriptCompiling();
			UpdateEventCache();
		}

		private void UpdateLayers() {
			List<string> layers = new List<string>();
			if (IsLevelOpen) {
				if (level != null) {
					for (int i = 0; i < level.RoomLayerCount; i++) {
						layers.Add("Layer " + (i + 1));
					}
					layers.Add("Actions");
				}
				int index = 0;
				if (actionMode) {
					index = layers.Count - 1;
					currentLayer = layers.Count - 2;
				}
				else {
					index = Math.Min(currentLayer, layers.Count - 2);
					currentLayer = index;
				}

				editorWindow.SetLayersItemsSource(layers, index);
			}
			else {
				editorWindow.SetLayersItemsSource(layers, -1);
			}
		}


		//-----------------------------------------------------------------------------
		// World
		//-----------------------------------------------------------------------------

		// Save the world file to the current filename.
		public void SaveWorld() {
			if (IsWorldOpen) {
				CurrentTool.Finish();
				WorldFile saveFile = new WorldFile();
				saveFile.Save(worldFilePath, world, true);
				IsModified  = false;
			}
		}

		// Save the world file to the given filename.
		public void SaveWorldAs(string fileName) {
			if (IsWorldOpen) {
				CurrentTool.Finish();
				WorldFile saveFile = new WorldFile();
				saveFile.Save(fileName, world, true);
				worldFilePath   = fileName;
				IsModified      = false;
			}
		}

		/// <summary>Open a world file with the given filename.</summary>
		public void OpenWorld(string fileName) {
			// Load the world
			WorldFile worldFile = new WorldFile();
			worldFile.LocateResource += OnWorldLocateResource;
			worldFileLocatedResource = false;
			World loadedWorld;
			try {
				loadedWorld = worldFile.Load(fileName, true);
			}
			catch (Exception ex) {
				var result = TriggerMessageBox.Show(editorWindow, MessageIcon.Error, "An error occurred while " +
					"trying to open '" + Path.GetFileName(fileName) + "', would you like to see the error?",
					"Load Failed", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes) {
					ErrorMessageBox.Show(ex, true);
				}
				return;
			}

			// Verify the world was loaded successfully.
			if (loadedWorld != null) {
				CloseWorld();

				worldFilePath       = fileName;
				needsRecompiling    = true;
				needsNewEventCache  = true;

				world = loadedWorld;
				if (world.LevelCount > 0)
					OpenLevel(0);
				eventCache.Clear();
				RefreshWorldTreeView();

				undoActions.Clear();
				undoPosition = -1;
				PushAction(new ActionOpenWorld(), ActionExecution.None);
				IsModified = worldFileLocatedResource;
			}
			else {
				// Display the error
				TriggerMessageBox.Show(editorWindow, MessageIcon.Warning,
					"Failed to open world file:\n" + worldFile.ErrorMessage,
					"Error Opening World", MessageBoxButton.OK);
			}
		}

		private void OnWorldLocateResource(LocateResourceEventArgs e) {
			LocateResourceWindow.Show(editorWindow, e);
			worldFileLocatedResource = true;
		}

		// Close the world file.
		public void CloseWorld() {
			if (IsWorldOpen) {
				CurrentTool.Finish();
				PropertyGrid.CloseProperties();
				world           = null;
				level           = null;
				LevelDisplay.ChangeLevel();
				worldFilePath   = "";
				IsModified      = false;
				eventCache.Clear();
				RefreshWorldTreeView();
				UpdateLayers();
				editorWindow.CloseAllToolWindows();
			}
		}

		// Test/play the world.
		public void TestWorld() {
			if (IsWorldOpen) {
				CurrentTool.Finish();
				string worldPath = Path.Combine(Path.GetDirectoryName(
					Assembly.GetExecutingAssembly().Location), "testing.zwd");
				WorldFile worldFile = new WorldFile();
				worldFile.Save(worldPath, world, true);
				string exePath = Path.Combine(Path.GetDirectoryName(
					Assembly.GetExecutingAssembly().Location), "ZeldaOracle.exe");
				string arguments = "\"" + worldPath + "\"";
				if (debugConsole)
					arguments += " -console";
				Process.Start(exePath, arguments);
			}
		}

		// Test/play the world with the player placed at the given room and point.
		public void TestWorld(Point2I roomCoord, Point2I playerCoord) {
			if (IsWorldOpen) {
				CurrentTool.Finish();
				playerPlaceMode = false;
				int levelIndex = world.IndexOfLevel(level);
				string worldPath = Path.Combine(Path.GetDirectoryName(
					Assembly.GetExecutingAssembly().Location), "testing.zwd");
				WorldFile worldFile = new WorldFile();
				worldFile.Save(worldPath, world, true);
				string exePath = Path.Combine(Path.GetDirectoryName(
					Assembly.GetExecutingAssembly().Location), "ZeldaOracle.exe");
				string arguments = "\"" + worldPath + "\"";
				arguments += " -test " + levelIndex + " " + roomCoord.X + " " + roomCoord.Y + " " + playerCoord.X + " " + playerCoord.Y;
				if (debugConsole)
					arguments += " -console";
				Process.Start(exePath, arguments);
				editorWindow.FinishTestWorldFromLocation();
			}
		}

		public void SetStartLocation(Point2I roomLocation, Point2I playerCoord) {
			startLocationMode = false;
			if (World.StartRoomLocation != roomLocation ||
				World.StartTileLocation != playerCoord ||
				World.StartLevel != Level)
			{
				World.StartLevelIndex = World.IndexOfLevel(Level);
				World.StartRoomLocation = roomLocation;
				World.StartTileLocation = playerCoord;
				IsModified = true;
			}
			editorWindow.FinishStartLocation();
		}

		// Open the properties for the given tile in the property grid.
		public void OpenProperties(IPropertyObject propertyObject) {
			PropertyGrid.OpenProperties(propertyObject);
		}

		public void RefreshWorldTreeView() {
			editorWindow.WorldTreeView.RefreshTree();
		}


		//-----------------------------------------------------------------------------
		// Level Display
		//-----------------------------------------------------------------------------

		/// <summary>Open the given level in the level display.</summary>
		public void OpenLevel(Level level) {
			if (this.level != level) {
				this.level = level;
				CurrentTool.End();
				CurrentTool.Begin();
				LevelDisplay.ChangeLevel();
				editorWindow.UpdateWindowTitle();
				PropertyGrid.OpenProperties(level);
				if (currentLayer >= level.RoomLayerCount)
					currentLayer = level.RoomLayerCount - 1;
				UpdateLayers();
				WorldTreeView.RefreshLevels();
			}
		}

		/// <summary>Open the given level index in the level display.</summary>
		public void OpenLevel(int index) {
			OpenLevel(world.GetLevelAt(index));
		}

		public void CloseLevel() {
			level = null;
			LevelDisplay.ChangeLevel();
			CurrentTool.Finish();
			editorWindow.UpdateWindowTitle();
			UpdateLayers();
		}

		public void GotoTile(BaseTileDataInstance tile) {
			OpenLevel(tile.Room.Level);
			CurrentTool = toolPointer;
			toolPointer.GotoTile(tile);
			OpenProperties(tile);
		}

		public void GotoRoom(Room room) {
			OpenLevel(room.Level);
			CurrentTool = toolPointer;
			toolPointer.GotoRoom(room);
			OpenProperties(room);
		}


		//-----------------------------------------------------------------------------
		// Helpers
		//-----------------------------------------------------------------------------

		public void ShowExceptionMessage(Exception ex, string verb, string name) {
			var result = TriggerMessageBox.Show(editorWindow, MessageIcon.Error, "An error occurred while trying to " +
				verb + " '" + name + "'! Would you like to see the error?", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
				ErrorMessageBox.Show(ex, true);
		}

		//-----------------------------------------------------------------------------
		// Tools
		//-----------------------------------------------------------------------------

		// Change the current tool to the tool of the given index.
		public void ChangeTool(int toolIndex) {
			if (toolIndex != currentToolIndex) {
				previousToolIndex = currentToolIndex;

				tools[currentToolIndex].End();

				currentToolIndex = toolIndex;

				editorWindow.UpdateCurrentTool();
				tools[currentToolIndex].Begin();
				CommandManager.InvalidateRequerySuggested();
			}
		}

		// Add a new tool to the list of tools and initialize it.
		private EditorTool AddTool(EditorTool tool) {
			tool.Initialize(this);
			tools.Add(tool);
			return tool;
		}


		//-----------------------------------------------------------------------------
		// Undo Actions
		//-----------------------------------------------------------------------------

		/// <summary>Pushes the action to the list and performs the specified execution.</summary>
		public void PushAction(EditorAction action, ActionExecution execution) {
			// Ignore if nothing occurred during this action
			if (action.IgnoreAction)
				return;

			IsModified = true;

			// Execute the action if requested
			if (execution == ActionExecution.Execute)
				action.Execute(this);
			else if (execution == ActionExecution.PostExecute)
				action.PostExecute(this);

			if (undoPosition >= 0)
				undoActions[undoPosition].IsSelected = false;

			while (undoPosition + 1 < undoActions.Count) {
				undoActions.RemoveAt(undoPosition + 1);
			}
			undoActions.Add(action);
			while (undoActions.Count > MaxUndos) {
				undoActions.RemoveAt(0);
			}
			undoPosition = undoActions.Count - 1;

			editorWindow.SelectHistoryItem(action);
		}

		/// <summary>Pops the last action from the list.</summary>
		public void PopAction() {
			while (undoPosition < undoActions.Count) {
				undoActions.RemoveAt(undoPosition);
			}
			undoPosition = undoActions.Count - 1;

			editorWindow.SelectHistoryItem(LastAction);
		}

		/// <summary>Undos the last executed action.</summary>
		public void Undo() {
			if (CurrentTool.CancelCountsAsUndo) {
				CurrentTool.Cancel();
			}
			else if (undoPosition > 0) {
				undoActions[undoPosition].Undo(this);
				undoActions[undoPosition].IsUndone = true;
				undoActions[undoPosition].IsSelected = false;
				undoPosition--;
				editorWindow.SelectHistoryItem(undoActions[undoPosition]);
				IsModified = true;
			}
		}

		/// <summary>Redos the next undone action.</summary>
		public void Redo() {
			if (undoPosition + 1 < undoActions.Count) {
				CurrentTool.Cancel();
				undoActions[undoPosition].IsSelected = false;
				undoPosition++;
				undoActions[undoPosition].IsUndone = false;
				undoActions[undoPosition].Redo(this);
				editorWindow.SelectHistoryItem(undoActions[undoPosition]);
				IsModified = true;
			}
		}

		/// <summary>Navigates to the action at the specified position in the list.</summary>
		public void GotoAction(int position) {
			if (position == -1)
				return;
			// Undo
			while (position < undoPosition) {
				if (CurrentTool.CancelCountsAsUndo) {
					CurrentTool.Cancel();
				}
				else {
					undoActions[undoPosition].IsSelected = false;
					undoActions[undoPosition].Undo(this);
					undoActions[undoPosition].IsUndone = true;
					undoPosition--;
					IsModified = true;
				}
			}
			// Redo
			while (position > undoPosition) {
				CurrentTool.Cancel();
				undoActions[undoPosition].IsSelected = false;
				undoPosition++;
				undoActions[undoPosition].IsUndone = false;
				undoActions[undoPosition].Redo(this);
				IsModified = true;
			}
		}

		/// <summary>Clears all undo actions except the original action.
		/// AKA: Open World or New World.</summary>
		public void PopToOriginalAction() {
			while (undoActions.Count > 1) {
				undoActions.RemoveAt(1);
			}
			undoPosition = 0;
			editorWindow.SelectHistoryItem(undoActions[undoPosition]);
		}

		/// <summary>Pushes the property change action to the list and
		/// performs the specified execution.</summary>
		public void PushPropertyAction(IPropertyObject propertyObject,
			string propertyName, object oldValue, object newValue,
			ActionExecution execution = ActionExecution.None) {
			Property property = propertyObject.Properties.
				GetProperty(propertyName, true);
			ActionChangeProperty action = new ActionChangeProperty(
					propertyObject, property, oldValue, newValue);

			// Special behavior for updating changes to tile size
			bool isTileSize = (property.Name == "size" &&
									propertyObject is TileDataInstance);
			if (isTileSize) {
				action = new ActionChangeTileSizeProperty(
					(TileDataInstance) propertyObject, property,
					(Point2I) oldValue, (Point2I) newValue);
				PushAction(action, ActionExecution.Execute);
				return;
			}

			if (LastAction is ActionChangeProperty) {
				ActionChangeProperty lastAction =
						(ActionChangeProperty) LastAction;
				if (action.PropertyObject == lastAction.PropertyObject &&
					action.PropertyName == lastAction.PropertyName) {
					action.OldValue = lastAction.OldValue;
					PopAction();
				}
			}

			PushAction(action, execution);
		}


		//-----------------------------------------------------------------------------
		// Scripts
		//-----------------------------------------------------------------------------

		public void OnScriptRenamed(string oldName, string newName) {
			scriptCompileService.CancelAllTasks();
			needsRecompiling = true;
		}

		// Internal -------------------------------------------------------------------

		private void UpdateScriptCompiling() {
			if (scriptCompileService.IsCompiling) {
				scriptCompileService.UpdateScriptCompiling();
			}
			else if (IsWorldOpen && needsRecompiling) {
				CompileTask task = scriptCompileService.CompileAllScripts(world);
				task.Completed += OnCompleteCompiling;
			}
		}

		private void OnCompleteCompiling(ScriptCompileResult result) {
			needsRecompiling = false;
			world.ScriptManager.RawAssembly = result.RawAssembly;
			
			scriptCompileResult = result;

			LogLevel level = LogLevel.Notice;
			if (result.Errors.Count > 0)
				level = LogLevel.Error;
			else if (result.Warnings.Count > 0)
				level = LogLevel.Warning;
			Logs.Scripts.LogMessage(level,
				"Compiled scripts with {0} errors and {0} warnings",
				result.Errors.Count, result.Warnings.Count);
			foreach (ScriptCompileError error in result.Errors)
				Logs.Scripts.LogMessage(LogLevel.Error, error.ToString());
			foreach (ScriptCompileError warning in result.Warnings)
				Logs.Scripts.LogMessage(LogLevel.Warning, warning.ToString());
		}



		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		private void UpdateEventCache() {
			if (eventCacheTask != null) {
				if (eventCacheTask.IsCompleted) {
					OnEventsCacheLoaded(eventCacheTask.Result);
				}
			}
			else if (needsNewEventCache) {
				eventCacheTask = Task.Run(() => ReloadEventCache());
			}
		}

		private List<Event> ReloadEventCache() {
			List<Event> cache = new List<Event>();
			//int internalID = 0;
			//if (IsWorldOpen) {
			//	foreach (Trigger trigger in world.GetAllTriggers()) {

			//	}

			//	foreach (Event evnt in world.GetDefinedEvents()) {
			//		if (evnt.GetExistingScript(world.ScriptManager.Scripts) == null) {
			//			evnt.InternalScriptID = ScriptManager.CreateInternalScriptName(internalID);
			//			cache.Add(evnt);
			//			internalID++;
			//		}
			//	}
			//}
			return cache;
		}

		private void OnEventsCacheLoaded(List<Event> newEventCache) {
			eventCacheTask = null;

			bool cacheChanged = (this.eventCache.Count != newEventCache.Count);
			if (!cacheChanged) {
				for (int i = 0; i < newEventCache.Count; i++) {
					if (this.eventCache[i] != newEventCache[i]) {
						cacheChanged = true;
						break;
					}
				}
			}
			if (cacheChanged) {
				this.eventCache = newEventCache;
				editorWindow.WorldTreeView.RefreshScripts(false, true);
			}
		}


		//-----------------------------------------------------------------------------
		// Event Scheduling
		//-----------------------------------------------------------------------------

		/// <summary>Schedules a delayed event.</summary>
		public void ScheduleEvent(double seconds, Action action) {
			ScheduleEvent(TimeSpan.FromSeconds(seconds), false, action);
		}

		/// <summary>Schedules a delayed event.</summary>
		public void ScheduleEvent(double seconds, bool highPriority, Action action) {
			ScheduleEvent(TimeSpan.FromSeconds(seconds), highPriority, action);
		}

		/// <summary>Schedules a delayed event.</summary>
		public void ScheduleEvent(TimeSpan delay, Action action) {
			ScheduleEvent(delay, false, action);
		}

		/// <summary>Schedules a delayed event.</summary>
		public void ScheduleEvent(TimeSpan delay, bool highPriority,
			Action action)
		{
			DispatcherPriority priority = DispatcherPriority.ApplicationIdle;
			if (highPriority)
				priority = DispatcherPriority.Render;
			DispatcherTimer timer = new DispatcherTimer(
				priority, Application.Current.Dispatcher);
			timer.Interval = delay;
			/*DispatcherTimer timer = new DispatcherTimer(
				delay, priority, null, Application.Current.Dispatcher);*/
			timer.Tick += (s, e) => {
				action();
				timer.Stop();
				scheduledEvents.Remove((DispatcherTimer) s);
			};
			timer.Start();
		}
		

		//-----------------------------------------------------------------------------
		// Ticks
		//-----------------------------------------------------------------------------

		// Update the elapsed ticks based on the total elapsed seconds.
		public void UpdateTicks() {
			double time = timer.Elapsed.TotalSeconds;
			if (playAnimations)
				ticks = (int)(time * 60.0);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorWindow EditorWindow {
			get { return editorWindow; }
			set { editorWindow = value; }
		}

		public ZeldaPropertyGrid PropertyGrid {
			get { return editorWindow.PropertyGrid; }
		}

		public LevelDisplay LevelDisplay {
			get { return editorWindow.LevelDisplay; }
		}

		public WorldTreeView WorldTreeView {
			get { return editorWindow.WorldTreeView; }
		}

		public RewardManager RewardManager {
			get { return rewardManager; }
		}

		public Inventory Inventory {
			get { return inventory; }
		}

		// World ----------------------------------------------------------------------
		
		public World World {
			get { return world; }
		}

		public bool IsWorldOpen {
			get { return (world != null); }
		}

		public string WorldFilePath {
			get { return worldFilePath; }
		}

		public string WorldFileName {
			get {
				if (string.IsNullOrEmpty(worldFilePath))
					return "Untitled";
				return Path.GetFileName(worldFilePath);
			}
		}

		public bool IsUntitled {
			get { return string.IsNullOrEmpty(worldFilePath); }
		}

		public bool IsModified {
			get { return isModified; }
			set {
				if (value != isModified) {
					isModified = value;
					CommandManager.InvalidateRequerySuggested();
					editorWindow.UpdateWindowTitle();
				}
			}
		}

		public bool PlayerPlaceMode {
			get { return playerPlaceMode; }
			set { playerPlaceMode = value; }
		}

		public bool StartLocationMode {
			get { return startLocationMode; }
			set { startLocationMode = value; }
		}

		public bool ShowStartLocation {
			get { return showStartLocation; }
			set { showStartLocation = value; }
		}

		// Editing --------------------------------------------------------------------

		public Level Level {
			get { return level; }
		}

		public bool IsLevelOpen {
			get { return (world != null && level != null); }
		}

		public Zone Zone {
			get { return zone; }
		}

		public int CurrentLayer {
			get { return currentLayer; }
			set {
				if (IsLevelOpen) {
					value = GMath.Clamp(value, 0, level.RoomLayerCount - 1);
					if (value != currentLayer) {
						currentLayer = value;
						CurrentTool.LayerChanged();
					}
				}
				else {
					currentLayer = 0;
				}
			}
		}

		public bool ActionLayer {
			get { return actionMode; }
			set {
				if (value != actionMode) {
					CurrentTool.Finish();
					actionMode = value;
					CurrentTool.LayerChanged();
					if (actionMode != IsSelectedTileAnAction)
						SelectedTileData = null;
				}
			}
		}

		public bool ActionMode {
			get { return (actionMode /*|| (selectedTileData is ActionTileData)*/); }
			set {
				if (value != actionMode) {
					CurrentTool.Finish();
					actionMode = value;
					CurrentTool.LayerChanged();
					if (actionMode != IsSelectedTileAnAction)
						SelectedTileData = null;
				}
			}
		}

		public Tileset SelectedTileset {
			get { return selectedTileset; }
			set { selectedTileset = value; }
		}

		public Point2I SelectedTilesetLocation {
			get { return selectedTilesetLocation; }
			set { selectedTilesetLocation = value; }
		}

		public BaseTileData SelectedTileData {
			get { return selectedTileData; }
			set {
				selectedTileData = value;
				if (IsSelectedTileAnAction) {
					if (!actionMode) {
						CurrentTool.Finish();
						actionMode = true;
						editorWindow.UpdateCurrentLayer();
					}
				}
				else if (selectedTileData != null) {
					if (actionMode) {
						CurrentTool.Finish();
						actionMode = false;
						editorWindow.UpdateCurrentLayer();
					}
				}
				editorWindow.TilesetPalette.SelectedTileData = selectedTileData;
			}
		}

		public bool IsSelectedTileAnAction {
			get { return (selectedTileData is ActionTileData); }
		}

		public Room EditingRoom {
			get { return editingRoom; }
			set {
				editingRoom = value;
				editingTileData = null;
			}
		}

		public BaseTileDataInstance EditingTileData {
			get { return editingTileData; }
			set {
				editingTileData = value;
				editingRoom = null;
			}
		}

		public bool IsEditingTileAnAction {
			get { return (editingTileData is ActionTileDataInstance); }
		}

		// Tools ----------------------------------------------------------------------

		public IEnumerable<EditorTool> Tools {
			get { return tools; }
		}

		public EditorTool CurrentTool {
			get {
				if (tools == null)
					return null;
				return tools[currentToolIndex];
			}
			set {
				ChangeTool(tools.IndexOf(value));
			}
		}

		public EditorTool PreviousTool {
			get {
				if (tools == null)
					return null;
				return tools[previousToolIndex];
			}
		}

		public ToolPointer ToolPointer {
			get { return toolPointer; }
		}

		public ToolPan ToolPan {
			get { return toolPan; }
		}

		public ToolPlace ToolPlace {
			get { return toolPlace; }
		}

		public ToolSquare ToolSquare {
			get { return toolSquare; }
		}

		public ToolFill ToolFill {
			get { return toolFill; }
		}

		public ToolSelection ToolSelection {
			get { return toolSelection; }
		}

		public ToolEyedrop ToolEyedropper {
			get { return toolEyedropper; }
		}
		
		// Tool Options ---------------------------------------------------------------

		public bool ToolOptionSingleLayer {
			get { return singleLayer; }
			set { singleLayer = value; }
		}

		public bool ToolOptionRoomOnly {
			get { return roomOnly; }
			set { roomOnly = value; }
		}

		public bool ToolOptionMerge {
			get { return merge; }
			set { merge = value; }
		}

		// Drawing --------------------------------------------------------------------

		public int Ticks {
			get {
				if (PlayAnimations)
					return ticks;
				return 0;
			}
		}

		public TileDrawModes AboveTileDrawMode {
			get { return aboveTileDrawMode; }
			set { aboveTileDrawMode = value; }
		}

		public TileDrawModes BelowTileDrawMode {
			get { return belowTileDrawMode; }
			set { belowTileDrawMode = value; }
		}

		public int RoomSpacing {
			get { return roomSpacing; }
			set { roomSpacing = value; }
		}

		public bool ShowModified {
			get { return showModified; }
			set { showModified = value; }
		}

		public bool ShowRewards {
			get { return showRewards; }
			set { showRewards = value; }
		}

		public bool ShowActions {
			get { return showActions; }
			set { showActions = value; }
		}

		public bool ShowShared {
			get { return showShared; }
			set { showShared = value; }
		}

		public bool ShowGrid {
			get { return showGrid; }
			set { showGrid = value; }
		}

		public bool PlayAnimations {
			get { return playAnimations; }
			set { playAnimations = value; }
		}
		
		public bool ShouldDrawActions {
			get { return (showActions || actionMode || selectedTileData is ActionTileData); }
		}

		// Undo Actions ---------------------------------------------------------------

		public ObservableCollection<EditorAction> UndoActions {
			get { return undoActions; }
		}

		public EditorAction LastAction {
			get { return undoActions[undoPosition]; }
		}

		public int UndoPosition {
			get { return undoPosition; }
		}

		public bool CanUndo {
			get { return undoPosition > 0 || CurrentTool.CancelCountsAsUndo; }
		}

		public bool CanRedo {
			get { return undoPosition + 1 < undoActions.Count; }
		}

		// Scripting ------------------------------------------------------------------

		public bool NeedsRecompiling {
			get { return needsRecompiling; }
			set { needsRecompiling = value; }
		}

		public bool NeedsNewEventCache {
			get { return needsNewEventCache; }
			set { needsNewEventCache = value; }
		}
		
		public List<Event> EventCache {
			get { return eventCache; }
		}

		public bool NoScriptErrors {
			get {
				return (scriptCompileResult == null ||
					scriptCompileResult.Errors.Count == 0);
			}
		}

		public bool NoScriptWarnings {
			get {
				return (scriptCompileResult == null ||
					scriptCompileResult.Warnings.Count == 0);
			}
		}

		public bool DebugConsole {
			get { return debugConsole; }
			set { debugConsole = value; }
		}

		public bool IsActive {
			get { return isActive; }
			set { isActive = value; }
		}

		public bool IsInitialized {
			get { return isInitialized; }
		}

		public ScriptCompileService ScriptCompileService {
			get { return scriptCompileService; }
			set { scriptCompileService = value; }
		}
	}
}
