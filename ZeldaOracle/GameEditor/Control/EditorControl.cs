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
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Worlds;

using ZeldaEditor.PropertiesEditor;
using ZeldaEditor.Tools;
using ZeldaEditor.TreeViews;
using ZeldaEditor.Undo;
using ZeldaEditor.Windows;
using ZeldaEditor.WinForms;
using ZeldaOracle.Common.Graphics;
using ZeldaEditor.Util;

namespace ZeldaEditor.Control {

	public delegate void ScriptCompileCallback(ScriptCompileResult result);

	public class EditorControl {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private const int MaxUndos = 50;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private bool isInitialized;

		// Control
		private EditorWindow        editorWindow;
		private string              worldFilePath;
		private World               world;
		/// <summary>Set to true during world file load if the world should be flagged as
		/// modified because a resource was located.</summary>
		private bool                worldFileLocatedResource;
		private Level               level;
		private Tileset				tileset;
		private Zone                zone;
		private RewardManager       rewardManager;
		private Inventory           inventory;

		private Stopwatch           timer;
		private int                 ticks;
		private bool                isModified;

		// Settings
		private bool                playAnimations;
		private bool                eventMode;

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
		private ObservableCollection<EditorAction>  undoActions;
		private int                 undoPosition;

		// Editing
		private int             roomSpacing;
		private int             currentLayer;
		private int             currentToolIndex;
		private int             previousToolIndex;
		private TileDrawModes   aboveTileDrawMode;
		private TileDrawModes   belowTileDrawMode;
		private bool            showRewards;
		private bool            showGrid;
		private bool            showModified;
		private bool            highlightMouseTile;
		private Tileset			selectedTileset;
		private Point2I			selectedTilesetLocation;
		private BaseTileData	selectedTileData;
		private string			tileSearchFilter;
		private bool            playerPlaceMode;
		private bool            showEvents;

		private DispatcherTimer             updateTimer;

		private bool                        needsRecompiling;
		private Task<ScriptCompileResult>   compileTask;
		private CancellationTokenSource     compileCancellationToken;
		private ScriptCompileCallback       compileCallback;
		private List<string>                scriptsToRecompile;
		private List<Event>                 eventsToRecompile;
		private bool                        isCompilingForScriptEditor;
		private Script                      currentCompilingScript;

		private Task<List<Event>>           eventCacheTask;
		private List<Event>                 eventCache;
		private bool                        needsNewEventCache;

		private bool                        noScriptErrors;
		private bool                        noScriptWarnings;


		private PaletteShader               paletteShader;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EditorControl(EditorWindow editorWindow) {
			this.editorWindow       = editorWindow;

			this.worldFilePath  = string.Empty;
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
			this.noScriptErrors     = false;
			this.noScriptWarnings   = false;

			this.currentLayer               = 0;
			this.currentToolIndex           = 0;
			this.previousToolIndex          = 0;
			this.aboveTileDrawMode          = TileDrawModes.Fade;
			this.belowTileDrawMode          = TileDrawModes.Fade;
			this.showRewards                = true;
			this.showGrid                   = false;
			this.showModified               = false;
			this.showEvents                 = false;
			this.highlightMouseTile         = true;
			this.selectedTileset            = null;
			this.selectedTilesetLocation	= Point2I.Zero;
			this.selectedTileData			= null;
			this.tileSearchFilter			= "";
			this.playerPlaceMode            = false;

		}

		public void SetGraphics(GraphicsDevice graphicsDevice, ContentManager contentManager) {
			if (!isInitialized) {
				isInitialized = true;
				Resources.Initialize(contentManager, graphicsDevice);
			}
		}

		public void Initialize() {
			GameData.Initialize();
			EditorResources.Initialize();
			FormatCodes.Initialize();
			paletteShader = new PaletteShader(GameData.PALETTE_LERP_SHADER);

			this.inventory      = new Inventory(null);
			this.rewardManager  = new RewardManager(null);
			this.timer          = Stopwatch.StartNew();
			this.ticks          = 0;
			this.roomSpacing    = 1;
			this.playAnimations = false;
			this.tileset        = null;
			this.zone           = GameData.ZONE_DEFAULT;
			this.selectedTileData = null;// this.tileset.GetTileData(0, 0);
			this.eventMode      = false;

			GameData.LoadInventory(inventory);
			GameData.LoadRewards(rewardManager);

			// Create tileset combo box.
			UpdateTilesets();

			// Create zone combo box.
			UpdateZones();

			// Create tools.
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

			this.undoActions = new ObservableCollection<EditorAction>();
			this.undoPosition = -1;

			this.isInitialized = true;

			this.updateTimer		= new DispatcherTimer(
				TimeSpan.FromMilliseconds(100),
				DispatcherPriority.ApplicationIdle,
				delegate { Update(); },
				Application.Current.Dispatcher);
			needsNewEventCache = true;
			needsRecompiling = true;
			
			// TEMP: Open this world file upon starting the editor.
			if (File.Exists("./temp_world.zwd"))
				OpenWorld("temp_world.zwd");
			else if (File.Exists("../../../../WorldFiles/temp_world.zwd"))
				OpenWorld("../../../../WorldFiles/temp_world.zwd");
			else if (File.Exists("../../../WorldFiles/temp_world.zwd"))
				OpenWorld("../../../WorldFiles/temp_world.zwd");
		}

		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		public void UpdateWindowTitle() {
			editorWindow.Title = "Oracle Engine Editor - " + WorldFileName;
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

		private void UpdateLayers() {
			List<string> layers = new List<string>();
			if (IsLevelOpen) {
				if (level != null) {
					for (int i = 0; i < level.RoomLayerCount; i++) {
						layers.Add("Layer " + (i + 1));
					}
					layers.Add("Events");
				}
				int index = 0;
				if (eventMode) {
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

		private void UpdateTilesets() {
			int index = 0;
			List<string> tilesets = new List<string>();
			foreach (var pair in Resources.GetResourceDictionary<Tileset>()) {
				tilesets.Add(pair.Key);
				if (tileset != null && pair.Key == tileset.ID)
					index = tilesets.Count; // No -1 because "<Tiel List>" is added after sorting to the front
			}
			tilesets.Sort((a, b) => AlphanumComparator.Compare(a, b, true));

			tilesets.Insert(0, "<Tile List>");

			editorWindow.SetTilesetsItemsSource(tilesets, index);
			UpdateTileSearch(tileSearchFilter);
		}

		private void UpdateZones() {
			int index = -1;
			List<string> zones = new List<string>();
			foreach (var pair in Resources.GetResourceDictionary<Zone>()) {
				zones.Add(pair.Key);
				if (pair.Key == zone.ID)
					index = zones.Count - 1;
			}
			zones.Sort((a, b) => AlphanumComparator.Compare(a, b, true));

			editorWindow.SetZonesItemsSource(zones, index);
		}


		//-----------------------------------------------------------------------------
		// Script Compiling
		//-----------------------------------------------------------------------------



		//-----------------------------------------------------------------------------
		// World
		//-----------------------------------------------------------------------------

		// Save the world file to the current filename.
		public void SaveWorld() {
			if (IsWorldOpen) {
				WorldFile saveFile = new WorldFile();
				saveFile.Save(worldFilePath, world, true);
				IsModified  = false;
			}
		}

		// Save the world file to the given filename.
		public void SaveWorldAs(string fileName) {
			if (IsWorldOpen) {
				WorldFile saveFile = new WorldFile();
				saveFile.Save(fileName, world, true);
				worldFilePath   = fileName;
				IsModified      = false;
			}
		}

		// Open a world file with the given filename.
		public void OpenWorld(string fileName) {
			// Load the world.
			WorldFile worldFile = new WorldFile();
			worldFile.LocateResource += OnWorldLocateResource;
			worldFileLocatedResource = false;
			World loadedWorld = worldFile.Load(fileName, true);

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
				IsModified          = worldFileLocatedResource;
			}
			else {
				// Display the error.
				TriggerMessageBox.Show(editorWindow, MessageIcon.Warning, "Failed to open world file:\n" + worldFile.ErrorMessage, "Error Opening World", MessageBoxButton.OK);
			}
		}

		private void OnWorldLocateResource(LocateResourceEventArgs e) {
			LocateResourceWindow.Show(editorWindow, e);
			worldFileLocatedResource = true;
		}

		// Close the world file.
		public void CloseWorld() {
			if (IsWorldOpen) {
				CurrentTool.End();
				CurrentTool.Begin();
				PropertyGrid.CloseProperties();
				world           = null;
				level           = null;
				LevelDisplay.UpdateLevel();
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

		public void ChangeTileset(string name) {
			if (name == "<Tile List>") {
				tileset = null;
				UpdateTileSearch(tileSearchFilter);
			}
			else {
				if (Resources.ContainsResource<Tileset>(name))
					tileset = Resources.GetResource<Tileset>(name);

				editorWindow.TileDisplay.UpdateTileset();
				//editorWindow.TileDisplay.UpdateZone();
			}

			//if (tileset.SpriteSheet != null) {
				// Setup zone combo box for the new tileset.
			//	UpdateZones();
			//}
		}

		public void ChangeZone(string name) {
			if (name != "(none)") {
				zone = Resources.GetResource<Zone>(name);
				editorWindow.TileDisplay.UpdateZone();
			}
		}

		public void UpdateTileSearch(string filter) {
			tileset = null;
			tileSearchFilter = filter;
			List<BaseTileData> filteredTileData = new List<BaseTileData>();
			foreach (var pair in Resources.GetResourceDictionary<BaseTileData>()) {
				if (pair.Key.Contains(filter)) {
					filteredTileData.Add(pair.Value);
				}
			}
			editorWindow.TileDisplay.UpdateTileset(filteredTileData);
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

		// Open the given level.
		public void OpenLevel(Level level) {
			if (this.level != level) {
				this.level = level;
				CurrentTool.End();
				CurrentTool.Begin();
				LevelDisplay.UpdateLevel();
				UpdateWindowTitle();
				PropertyGrid.OpenProperties(level);
				if (currentLayer >= level.RoomLayerCount)
					currentLayer = level.RoomLayerCount - 1;
				UpdateLayers();
				WorldTreeView.RefreshLevels();
			}
		}

		// Open the given level index in the level display.
		public void OpenLevel(int index) {
			OpenLevel(world.GetLevelAt(index));
		}

		public void CloseLevel() {
			level = null;
			LevelDisplay.UpdateLevel();
			CurrentTool.End();
			CurrentTool.Begin();
			UpdateWindowTitle();
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
		// Tiles
		//-----------------------------------------------------------------------------



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
			if (!scriptEditor)
				editorWindow.SetStatusBarTask("Compiling scripts...");
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
			else if (IsWorldOpen) {
				if (needsRecompiling) {
					CompileAllScriptsAsync(OnCompileCompleted);
				}
				else if (HasScriptsToCheck) {
					CompileNextScript();
				}
			}
			else {
				needsRecompiling = false;
			}
		}

		private void CompileAllScriptsAsync(ScriptCompileCallback callback) {
			compileCancellationToken = new CancellationTokenSource();
			compileCallback = callback;
			compileTask = Task.Run(() => world.ScriptManager.CompileScripts(world, true), compileCancellationToken.Token);
			editorWindow.SetStatusBarTask("Compiling scripts...");
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
				editorWindow.ClearStatusBarTask();
				editorWindow.WorldTreeView.RefreshScripts(true, true);
				scriptsToRecompile.Clear();
				eventsToRecompile.Clear();
			}
		}

		private void OnCompileScriptCompleted(ScriptCompileResult result) {
			compileTask = null;
			compileCancellationToken = null;

			currentCompilingScript.Errors   = result.Errors;
			currentCompilingScript.Warnings = result.Warnings;
			currentCompilingScript = null;

			if (!HasScriptsToCheck) {
				editorWindow.ClearStatusBarTask();
				editorWindow.WorldTreeView.RefreshScripts(true, true);
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
			if (IsWorldOpen) {
				foreach (Event evnt in world.GetDefinedEvents()) {
					if (evnt.GetExistingScript(world.ScriptManager.Scripts) == null) {
						evnt.InternalScriptID = ScriptManager.CreateInternalScriptName(internalID);
						cache.Add(evnt);
						internalID++;
					}
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
				editorWindow.WorldTreeView.RefreshScripts(false, true);
			}
		}

		//-----------------------------------------------------------------------------
		// Ticks
		//-----------------------------------------------------------------------------

		// Update the elapsed ticks based on the total elapsed seconds.
		public void UpdateTicks() {
			double time = timer.Elapsed.TotalSeconds;
			if (playAnimations)
				ticks++;// = (int)(time * 60.0);
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

		public PaletteShader PaletteShader {
			get { return paletteShader; }
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
					UpdateWindowTitle();
				}
			}
		}

		public bool PlayerPlaceMode {
			get { return playerPlaceMode; }
			set { playerPlaceMode = value; }
		}

		// Editing --------------------------------------------------------------------

		public Level Level {
			get { return level; }
		}

		public bool IsLevelOpen {
			get { return (world != null && level != null); }
		}

		public Tileset Tileset {
			get { return tileset; }
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

		public bool EventMode {
			get { return (eventMode || (selectedTileData is EventTileData)); }
			set {
				if (value != eventMode) {
					eventMode = value;
					CurrentTool.LayerChanged();
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
				editorWindow.TileDisplay.Invalidate();
			}
		}

		public bool IsSelectedTileAnEvent {
			get { return (selectedTileData is EventTileData); }
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
		
		// Drawing --------------------------------------------------------------------
		
		public int Ticks {
			get { return ticks; }
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

		public bool ShowEvents {
			get { return showEvents; }
			set { showEvents = value; }
		}

		public bool ShowGrid {
			get { return showGrid; }
			set { showGrid = value; }
		}

		public bool PlayAnimations {
			get { return playAnimations; }
			set { playAnimations = value; }
		}
		
		public bool ShouldDrawEvents {
			get { return (showEvents || eventMode || selectedTileData is EventTileData); }
		}
		
		public bool HighlightMouseTile {
			get { return highlightMouseTile; }
			set { highlightMouseTile = value; }
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

		public bool IsBusyCompiling {
			get { return (compileTask != null); }
		}

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
			get { return noScriptErrors; }
		}

		public bool NoScriptWarnings {
			get { return noScriptWarnings; }
		}

		public bool DebugConsole {
			get { return debugConsole; }
			set { debugConsole = value; }
		}
	}
}
