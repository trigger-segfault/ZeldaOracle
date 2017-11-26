using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Tools;
using ZeldaEditor.Scripting;
using ZeldaOracle.Common.Scripting;
using System.Windows;
using System.Windows.Threading;
using ZeldaEditor.Windows;
using System.Reflection;
using ZeldaEditor.PropertiesEditor;
using ZeldaEditor.WinForms;
using ZeldaOracle.Common.Translation;
using System.Windows.Input;
using ZeldaEditor.Undo;
using System.Collections.ObjectModel;
using System.Threading;

namespace ZeldaEditor.Control {

	public delegate void ScriptCompileCallback(ScriptCompileResult result);

	public class EditorControl {

		private const int MaxUndos = 50;
		
		private bool isInitialized;

		// Control
		private EditorWindow		editorWindow;
		private string				worldFilePath;
		private string				worldFileName;
		private World				world;
		private Level				level;
		private ITileset			tileset;
		private Zone				zone;
		private RewardManager		rewardManager;
		private Inventory			inventory;

		private Stopwatch			timer;
		private int					ticks;
		private bool				isModified;

		// Settings
		private bool				playAnimations;
		private bool				eventMode;
		
		// Tools
		private List<EditorTool>	tools;
		private ToolPointer			toolPointer;
		private ToolPlace			toolPlace;
		private	ToolSquare			toolSquare;
		private ToolFill			toolFill;
		private ToolSelection		toolSelection;
		private ToolEyedrop			toolEyedrop;
		private ObservableCollection<EditorAction>	undoActions;
		private int                 undoPosition;

		// Editing
		private int				roomSpacing;
		private int				currentLayer;
		private int				currentToolIndex;
		private TileDrawModes	aboveTileDrawMode;
		private TileDrawModes	belowTileDrawMode;
		private bool			showRewards;
		private bool			showGrid;
		private bool            showModified;
		private bool			highlightMouseTile;
		private Point2I			selectedRoom;
		private Point2I			selectedTilesetTile;
		private BaseTileData	selectedTilesetTileData;
		private bool			playerPlaceMode;
		private bool			showEvents;

		private DispatcherTimer             updateTimer;
		
		private bool						needsRecompiling;
		private Task<ScriptCompileResult>   compileTask;
		private CancellationTokenSource		compileCancellationToken;
		private ScriptCompileCallback       compileCallback;
		private List<string>                scriptsToRecompile;
		private List<Event>					eventsToRecompile;
		private bool                        isCompilingForScriptEditor;
		private Script                      currentCompilingScript;

		private Task<List<Event>>           eventCacheTask;
		private List<Event>                 eventCache;
		private bool                        needsNewEventCache;

		private bool                        noScriptErrors;
		private bool                        noScriptWarnings;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EditorControl(EditorWindow editorWindow) {
			this.editorWindow       = editorWindow;

			this.worldFilePath  = String.Empty;
			this.worldFileName  = "untitled";
			this.world          = null;
			this.level          = null;
			this.tileset        = null;
			this.zone           = null;
			this.rewardManager  = null;
			this.inventory      = null;
			this.timer          = null;
			this.ticks          = 0;
			this.roomSpacing    = 1;
			this.playAnimations = false;
			this.isInitialized  = false;
			this.isModified = false;
			this.needsRecompiling   = false;
			this.compileTask        = null;
			this.compileCallback    = null;
			this.scriptsToRecompile = new List<string>();
			this.eventsToRecompile = new List<Event>();
			this.isCompilingForScriptEditor = false;
			this.eventCache  = new List<Event>();
			this.eventCacheTask = null;
			this.needsNewEventCache = false;
			this.currentCompilingScript = null;
			this.noScriptErrors		= false;
			this.noScriptWarnings   = false;

			this.currentLayer               = 0;
			this.currentToolIndex           = 0;
			this.aboveTileDrawMode          = TileDrawModes.Fade;
			this.belowTileDrawMode          = TileDrawModes.Fade;
			this.showRewards                = true;
			this.showGrid                   = false;
			this.showModified               = false;
			this.showEvents                 = false;
			this.highlightMouseTile         = true;
			this.selectedRoom               = -Point2I.One;
			this.selectedTilesetTile        = Point2I.Zero;
			this.selectedTilesetTileData    = null;
			this.playerPlaceMode            = false;

		}

		public void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice) {
			if (!isInitialized) {
				Resources.Initialize(contentManager, graphicsDevice);
				GameData.Initialize();
				EditorResources.Initialize();
				FormatCodes.Initialize();

				this.inventory		= new Inventory(null);
				this.rewardManager	= new RewardManager(null);
				this.timer			= Stopwatch.StartNew();
				this.ticks			= 0;
				this.roomSpacing	= 1;
				this.playAnimations = false;
				this.tileset		= GameData.TILESET_CLIFFS;
				this.zone			= GameData.ZONE_PRESENT;
				this.selectedTilesetTileData = this.tileset.GetTileData(0, 0);
				this.eventMode		= false;

				GameData.LoadInventory(inventory);
				GameData.LoadRewards(rewardManager);

				// Create tileset combo box.
				editorWindow.ComboBoxTilesets.Items.Clear();
				foreach (KeyValuePair<string, Tileset> entry in Resources.GetResourceDictionary<Tileset>()) {
					editorWindow.ComboBoxTilesets.Items.Add(entry.Key);
				}
				foreach (KeyValuePair<string, EventTileset> entry in Resources.GetResourceDictionary<EventTileset>()) {
					editorWindow.ComboBoxTilesets.Items.Add(entry.Key);
				}
				editorWindow.ComboBoxTilesets.SelectedIndex = 0;
				
				// Create zone combo box.
				editorWindow.ComboBoxZones.Items.Clear();
				foreach (KeyValuePair<string, Zone> entry in Resources.GetResourceDictionary<Zone>()) {
					if (tileset.SpriteSheet.Image.HasVariant(entry.Key))
						editorWindow.ComboBoxZones.Items.Add(entry.Key);
				}
				editorWindow.ComboBoxZones.SelectedIndex = 0;

				// Create tools.
				tools = new List<EditorTool>();
				AddTool(toolPointer		= new ToolPointer());
				AddTool(toolPlace		= new ToolPlace());
				AddTool(toolSquare		= new ToolSquare());
				AddTool(toolFill		= new ToolFill());
				AddTool(toolSelection	= new ToolSelection());
				AddTool(toolEyedrop		= new ToolEyedrop());
				currentToolIndex = 0;
				tools[currentToolIndex].Begin();

				this.undoActions = new ObservableCollection<EditorAction>();
				this.undoPosition = -1;

				this.isInitialized = true;

				this.updateTimer            = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.ApplicationIdle, delegate { Update(); }, Application.Current.Dispatcher);
				//updateTimer.Start();
				needsNewEventCache = true;
				needsRecompiling = true;
			}
		}

		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		public void UpdateWindowTitle() {
			editorWindow.Title = "Oracle Engine Editor - " + worldFileName;
			if (isModified)
				editorWindow.Title += "*";
			if (level != null)
				editorWindow.Title += " [" + level.Properties.GetString("id") + "]";
		}

		// Called with Application.Idle.
		private void Update() {
			UpdateScriptCompiling();
			UpdateEventCache();
		}
		
		//-----------------------------------------------------------------------------
		// Script Compiling
		//-----------------------------------------------------------------------------



		//-----------------------------------------------------------------------------
		// World
		//-----------------------------------------------------------------------------

		// Save the world file to the given filename.
		public async void SaveFileAs(string fileName) {
			if (IsWorldOpen) {
				WorldFile saveFile = new WorldFile();
				saveFile.Save(fileName, world, true);
				isModified	= false;
				worldFilePath	= fileName;
				worldFileName	= Path.GetFileName(fileName);
			}
		}

		// Open a world file with the given filename.
		public void OpenFile(string fileName) {
			// Load the world.
			WorldFile worldFile = new WorldFile();
			World loadedWorld = worldFile.Load(fileName, true);

			// Verify the world was loaded successfully.
			if (loadedWorld != null) {
				CloseFile();

				worldFilePath		= fileName;
				worldFileName		= Path.GetFileName(fileName);
				needsRecompiling	= true;
				needsNewEventCache	= true;

				world = loadedWorld;
				if (world.Levels.Count > 0)
					OpenLevel(0);
				eventCache.Clear();
				RefreshWorldTreeView();

				undoActions.Clear();
				undoPosition = -1;
				PushAction(new ActionOpenWorld(), ActionExecution.None);
				isModified			= false;
			}
			else {
				// Display the error.
				TriggerMessageBox.Show(editorWindow, MessageIcon.Warning, "Failed to open world file:\n" + worldFile.ErrorMessage, "Error Opening World", MessageBoxButton.OK);
			}
		}

		// Close the world file.
		public void CloseFile() {
			if (IsWorldOpen) {
				PropertyGrid.CloseProperties();
				world           = null;
				level			= null;
				isModified	= false;
				worldFilePath   = "";
				eventCache.Clear();
				RefreshWorldTreeView();
			}
		}

		/*public void ResizeLevel(Level level, Point2I newSize) {
			if (newSize != level.Dimensions) {
				PushAction(new ActionResizeLevel(level, newSize));
				level.Resize(newSize);
				LevelDisplay.UpdateLevel();
				IsModified = true;
			}
		}
		public void ShiftLevel(Level level, Point2I distance) {
			if (distance != Point2I.Zero) {
				PushAction(new ActionShiftLevel(level, distance));
				level.ShiftRooms(distance);
				IsModified = true;
			}
		}*/

		// Add a new level to the world, and open it if specified.
		public void AddLevel(Level level, bool openLevel) {
			world.AddLevel(level);
			editorWindow.TreeViewWorld.RefreshLevels();
			if (openLevel)
				OpenLevel(level);
			isModified = true;
		}

		// Add a new dungeon to the world, and open it if specified.
		public void AddDungeon(Dungeon dungeon, bool openDungeonProperties) {
			world.AddDungeon(dungeon);
			editorWindow.TreeViewWorld.RefreshDungeons();
			if (openDungeonProperties)
				editorWindow.PropertyGrid.OpenProperties(dungeon);
			isModified = true;
		}

		public void AddScript(Script script) {
			world.AddScript(script);
			editorWindow.TreeViewWorld.RefreshScripts(true, false);
			isModified = true;
		}

		/*public void IntroduceScripts(Dictionary<string, Script> scripts) {
			bool scriptsAdded = false;
			foreach (var pair in scripts) {
				if (pair.Value.IsHidden) {
					GenerateInternalScript(pair.Value, false);
					scriptsAdded = true;
				}
				else if (world.ContainsScript(pair.Key)) {
					AddScript(pair.Value);
					scriptsAdded = true;
					IsModified = true;
				}
			}
			if (scriptsAdded) {
				editorWindow.TreeViewWorld.RefreshScripts();
				IsModified = true;
			}
		}*/

		public void ChangeTileset(string name) {
			if (Resources.ExistsResource<Tileset>(name))
				tileset = Resources.GetResource<Tileset>(name);
			else if (Resources.ExistsResource<EventTileset>(name))
				tileset = Resources.GetResource<EventTileset>(name);
			
			if (tileset.SpriteSheet != null) {
				// Determine which zone to begin using for this tileset.
				int index = 0;
				if (!tileset.SpriteSheet.Image.HasVariant(zone.ID)) {
					zone = Resources.GetResource<Zone>(tileset.SpriteSheet.Image.VariantName);
					if (zone == null)
						zone = GameData.ZONE_DEFAULT;
				}

				// Setup zone combo box for the new tileset.
				editorWindow.ComboBoxZones.Items.Clear();
				foreach (KeyValuePair<string, Zone> entry in Resources.GetResourceDictionary<Zone>()) {
					if (tileset.SpriteSheet.Image.HasVariant(entry.Key)) {
						editorWindow.ComboBoxZones.Items.Add(entry.Key);
						if (entry.Key == zone.ID)
							editorWindow.ComboBoxZones.SelectedIndex = index;
						index++;
					}
				}
			}

			editorWindow.TileDisplay.UpdateTileset();
			editorWindow.TileDisplay.UpdateZone();
		}

		public void ChangeZone(string name) {
			if (name != "(none)") {
				zone = Resources.GetResource<Zone>(name);
				editorWindow.TileDisplay.UpdateZone();
			}
		}

		// Test/play the world.
		public void TestWorld() {
			if (IsWorldOpen) {
				string worldPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "testing.zwd");
				WorldFile worldFile = new WorldFile();
				worldFile.Save(worldPath, world, true);
				string exePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ZeldaOracle.exe");
				Process.Start(exePath, "\"" + worldPath + "\"");
			}
		}
		
		// Test/play the world with the player placed at the given room and point.
		public void TestWorld(Point2I roomCoord, Point2I playerCoord) {
			if (IsWorldOpen) {
				playerPlaceMode = false;
				int levelIndex = 0;
				for (levelIndex = 0; levelIndex < world.Levels.Count; levelIndex++) {
					if (world.Levels[levelIndex] == level)
						break;
				}
				string worldPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "testing.zwd");
				WorldFile worldFile = new WorldFile();
				worldFile.Save(worldPath, world, true);
				string exePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ZeldaOracle.exe");
				Process.Start(exePath, "\"" + worldPath + "\" -test " + levelIndex + " " + roomCoord.X + " " + roomCoord.Y + " " + playerCoord.X + " " + playerCoord.Y);
				editorWindow.FinishTestWorldFromLocation();
			}
		}

		public void RefreshWorldTreeView() {
			editorWindow.TreeViewWorld.RefreshTree();
		}


		//-----------------------------------------------------------------------------
		// Tiles
		//-----------------------------------------------------------------------------

		// Open the properties for the given tile in the property grid.
		public void OpenObjectProperties(IPropertyObject propertyObject) {
			PropertyGrid.OpenProperties(propertyObject);
		}

		//-----------------------------------------------------------------------------
		// Tools
		//-----------------------------------------------------------------------------
		
		// Change the current tool to the tool of the given index.
		public void ChangeTool(int toolIndex) {
			if (toolIndex != currentToolIndex) {
				tools[currentToolIndex].End();

				currentToolIndex = toolIndex;
				if (currentToolIndex != 0) {
					selectedRoom = -Point2I.One;
				}

				editorWindow.OnToolChange(toolIndex);
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
		
		public void PushAction(EditorAction action, ActionExecution execution ) {
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

		public void PopAction() {
			while (undoPosition < undoActions.Count) {
				undoActions.RemoveAt(undoPosition);
			}
			undoPosition = undoActions.Count - 1;

			editorWindow.SelectHistoryItem(LastAction);
		}

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

		//-----------------------------------------------------------------------------
		// Level Display
		//-----------------------------------------------------------------------------
		
		// Open the given level.
		public void OpenLevel(Level level) {
			if (this.level != level) {
				this.level = level;
				CurrentTool.End();
				CurrentTool.Begin();
				editorWindow.LevelDisplay.UpdateLevel();
				UpdateWindowTitle();
				PropertyGrid.OpenProperties(level);
				if (currentLayer >= level.RoomLayerCount)
					currentLayer = level.RoomLayerCount - 1;
				editorWindow.UpdateLayers();
			}
		}

		// Open the given level index in the level display.
		public void OpenLevel(int index) {
			OpenLevel(world.Levels[index]);
		}

		public void CloseLevel() {
			level = null;
			editorWindow.LevelDisplay.UpdateLevel();
			CurrentTool.End();
			CurrentTool.Begin();
			UpdateWindowTitle();
			PropertyGrid.CloseProperties();
		}

		public void GotoTile(BaseTileDataInstance tile) {
			OpenLevel(tile.Room.Level);
			CurrentTool = toolPointer;
			toolPointer.GotoTile(tile);
			OpenObjectProperties(tile);
		}
		public void GotoRoom(Room room) {
			OpenLevel(room.Level);
			CurrentTool = toolPointer;
			toolPointer.GotoRoom(room);
			OpenObjectProperties(room);
		}


		//-----------------------------------------------------------------------------
		// Scripts
		//-----------------------------------------------------------------------------

		public void CompileScriptAsync(Script script, ScriptCompileCallback callback, bool scriptEditor) {
			if (!CancelCompilation(scriptEditor))
				return;

			compileCallback = callback;
			int scriptStart;
			string code = world.ScriptManager.CreateTestScriptCode(script, script.Code, out scriptStart);
			compileCancellationToken = new CancellationTokenSource();
			compileTask = Task.Run(() => world.ScriptManager.Compile(code, false, scriptStart), compileCancellationToken.Token);
			editorWindow.StatusBarLabelTask.Content = "Compiling scripts...";

		}

		public void ScriptRenamed(string oldName, string newName) {
			CancelCompilation();

			if (scriptsToRecompile != null && oldName != null && newName != null && scriptsToRecompile.Contains(oldName)) {
				scriptsToRecompile.Remove(oldName);
				scriptsToRecompile.Add(newName);
			}
			foreach (Script script in world.ScriptManager.Scripts.Values) {
				if (!scriptsToRecompile.Contains(script.ID)) {
					if (ScriptCallsScript(script, oldName) || ScriptCallsScript(script, newName)) {
						scriptsToRecompile.Add(script.ID);
					}
				}
			}
			foreach (Event evnt in world.GetDefinedEvents()) {
				if (evnt.GetExistingScript(world.ScriptManager.Scripts) == null) {
					Script script = evnt.Script;
					if (!eventsToRecompile.Contains(evnt)) {
						if (ScriptCallsScript(script, oldName) || ScriptCallsScript(script, newName)) {
							eventsToRecompile.Add(evnt);
						}
					}
				}
			}
			if (HasScriptsToCheck)
				needsRecompiling = true;
		}

		// Internal -------------------------------------------------------------------

		private void UpdateScriptCompiling() {
			if (compileTask != null) {
				if (compileTask.IsCompleted) {
					compileCallback(compileTask.Result);
					compileTask = null;
				}
			}
			else if (needsRecompiling) {
				CompileAllScriptsAsync(OnCompileCompleted);
			}
			else if (HasScriptsToCheck) {
				CompileNextScript();
			}
		}
		
		private void CompileAllScriptsAsync(ScriptCompileCallback callback) {
			compileCancellationToken = new CancellationTokenSource();
			compileCallback = callback;
			compileTask = Task.Run(() => world.ScriptManager.CompileScripts(world, true), compileCancellationToken.Token);
			editorWindow.StatusBarLabelTask.Content = "Compiling scripts...";
			currentCompilingScript = null;
		}

		private void OnCompileCompleted(ScriptCompileResult result) {
			needsRecompiling = false;
			compileTask = null;
			compileCancellationToken = null;

			world.ScriptManager.RawAssembly = result.RawAssembly;

			Console.WriteLine("Compiled scripts with " + result.Errors.Count + " errors and " + result.Warnings.Count + " warnings.");

			noScriptErrors = !result.Errors.Any();
			noScriptWarnings = !result.Warnings.Any();
			
			if (!HasScriptsToCheck || (noScriptErrors && noScriptWarnings)) {
				editorWindow.StatusBarLabelTask.Content = "";
				editorWindow.TreeViewWorld.RefreshScripts(true, true);
				scriptsToRecompile.Clear();
				eventsToRecompile.Clear();
			}
		}

		private void OnCompileScriptCompleted(ScriptCompileResult result) {
			compileTask = null;
			compileCancellationToken = null;

			currentCompilingScript.Errors	= result.Errors;
			currentCompilingScript.Warnings	= result.Warnings;
			currentCompilingScript = null;

			if (!HasScriptsToCheck) {
				editorWindow.StatusBarLabelTask.Content = "";
				editorWindow.TreeViewWorld.RefreshScripts(true, true);
			}
		}

		private void CompileNextScript() {
			Script script = null;
			if (scriptsToRecompile.Any()) {
				script = world.GetScript(scriptsToRecompile.First());
				scriptsToRecompile.RemoveAt(0);
			}
			else if (eventsToRecompile.Any()) {
				script = eventsToRecompile.First().Script;
				eventsToRecompile.RemoveAt(0);
			}
			if (script != null) {
				CompileScriptAsync(script, OnCompileScriptCompleted, false);
				currentCompilingScript = script;
			}
		}


		private bool CancelCompilation(bool scriptEditor = false) {
			if (compileTask != null && !compileTask.IsCompleted && compileCancellationToken != null) {
				if (scriptEditor || !isCompilingForScriptEditor) {
					compileCancellationToken.Cancel();
					compileCancellationToken = null;
					compileTask = null;
					currentCompilingScript = null;
					return true;
				}
				else {
					return false;
				}
			}
			return true;
		}

		private bool ScriptCallsScript(Script script, string scriptName) {
			if (scriptName == null)
				return false;
			int index = script.Code.IndexOf(scriptName + "(");
			if (index != -1) {
				if (index != 0) {
					char c = script.Code[index - 1];
					if (!char.IsLetterOrDigit(c) && c != '_')
						return true;
				}
				else {
					return true;
				}
			}
			return false;
		}

		private bool HasScriptsToCheck {
			get { return scriptsToRecompile.Any() || eventsToRecompile.Any(); }
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
			int internalID = 0;
			foreach (Event evnt in world.GetDefinedEvents()) {
				if (evnt.GetExistingScript(world.ScriptManager.Scripts) == null) {
					evnt.InternalScriptID = "__internal_script_" + internalID + "__";
					cache.Add(evnt);
					internalID++;
				}
			}
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
				editorWindow.TreeViewWorld.RefreshScripts(false, true);
			}
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

		public bool IsWorldOpen {
			get { return (world != null); }
		}

		public bool IsLevelOpen {
			get { return (world != null && level != null); }
		}

		public World World {
			get { return world; }
		}
		public Level Level {
			get { return level; }
		}

		public int RoomSpacing {
			get { return roomSpacing; }
			set { roomSpacing = value; }
		}

		public int Ticks {
			get { return ticks; }
		}

		public bool PlayAnimations {
			get { return playAnimations; }
			set { playAnimations = value; }
		}

		public ITileset Tileset {
			get { return tileset; }
		}

		public Zone Zone {
			get { return zone; }
		}

		public Point2I SelectedRoom {
			get { return selectedRoom; }
			set { selectedRoom = value; }
		}

		public Point2I SelectedTilesetTile {
			get { return selectedTilesetTile; }
			set { selectedTilesetTile = value; }
		}

		public BaseTileData SelectedTilesetTileData {
			get { return selectedTilesetTileData; }
			set {
				selectedTilesetTileData = value;
				editorWindow.TileDisplay.Invalidate();
			}
		}

		public RewardManager RewardManager {
			get { return rewardManager; }
		}

		public Inventory Inventory {
			get { return inventory; }
		}

		public int CurrentLayer {
			get { return currentLayer; }
			set { currentLayer = GMath.Clamp(value, 0, 3); }
		}

		public int CurrentToolIndex {
			get { return currentToolIndex; }
			set { ChangeTool(value); }
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

		public TileDrawModes AboveTileDrawMode {
			get { return aboveTileDrawMode; }
			set { aboveTileDrawMode = value; }
		}

		public TileDrawModes BelowTileDrawMode {
			get { return belowTileDrawMode; }
			set { belowTileDrawMode = value; }
		}

		public bool ShowRewards {
			get { return showRewards; }
			set { showRewards = value; }
		}

		public bool ShowGrid {
			get { return showGrid; }
			set { showGrid = value; }
		}

		public bool ShowModified {
			get { return showModified; }
			set { showModified = value; }
		}

		public bool ShowEvents {
			get { return showEvents; }
			set { showEvents = value; }
		}
		
		public bool ShouldDrawEvents {
			get { return (showEvents || eventMode || (selectedTilesetTileData is EventTileData) || (tileset is EventTileset)); }
		}

		public bool EventMode {
			get { return (eventMode || (selectedTilesetTileData is EventTileData)); }
			set { eventMode = value; }
		}

		public bool HighlightMouseTile {
			get { return highlightMouseTile; }
			set { highlightMouseTile = value; }
		}

		public bool PlayerPlaceMode {
			get { return playerPlaceMode; }
			set { playerPlaceMode = value; }
		}

		public ToolPointer ToolPointer {
			get { return toolPointer; }
		}

		public ToolPlace ToolPlace {
			get { return toolPlace; }
		}

		public ToolSelection ToolSelection {
			get { return toolSelection; }
		}

		public ToolEyedrop ToolEyedrop {
			get { return toolEyedrop; }
		}

		public bool IsWorldFromFile {
			get { return (worldFilePath != String.Empty); }
		}

		public string WorldFilePath {
			get { return worldFilePath; }
		}

		public string WorldFileName {
			get { return worldFileName; }
		}

		public bool IsModified {
			get { return isModified; }
			set {
				isModified = value;
				CommandManager.InvalidateRequerySuggested();
			}
		}

		public bool IsSelectedTileAnEvent {
			get { return (selectedTilesetTileData is EventTileData); }
		}

		public bool NeedsRecompiling {
			get { return needsRecompiling; }
			set { needsRecompiling = value; }
		}
		public bool NeedsNewEventCache {
			get { return needsNewEventCache; }
			set { needsNewEventCache = value; }
		}

		public bool IsBusyCompiling {
			get { return (compileTask != null); }
		}

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

		public List<Event> EventCache {
			get { return eventCache; }
		}

		public bool NoScriptErrors {
			get { return noScriptErrors; }
		}
		public bool NoScriptWarnings {
			get { return noScriptWarnings; }
		}
	}
}
