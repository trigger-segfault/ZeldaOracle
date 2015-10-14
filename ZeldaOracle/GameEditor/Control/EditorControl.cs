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
using System.Windows.Forms;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Tools;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Control {

	public class EditorControl {

		private bool isInitialized;

		// Control
		private EditorForm			editorForm;
		private PropertyGridControl	propertyGridControl;
		private string				worldFilePath;
		private string				worldFileName;
		private World				world;
		private Level				level;
		private Tileset				tileset;
		private Zone				zone;
		private RewardManager		rewardManager;
		private Inventory			inventory;

		private Stopwatch			timer;
		private int					ticks;
		private bool				hasMadeChanges;

		// Settings
		private bool				playAnimations;
		private bool				eventMode;
		
		// Tools
		private List<EditorTool>	tools;
		private ToolPointer			toolPointer;
		private ToolPlace			toolPlace;
		private ToolSelection		toolSelection;
		private ToolEyedrop			toolEyedrop;

		// Editing
		private int				roomSpacing;
		private int				currentLayer;
		private int				currentToolIndex;
		private TileDrawModes	aboveTileDrawMode;
		private TileDrawModes	belowTileDrawMode;
		private bool			showRewards;
		private bool			showGrid;
		private bool			highlightMouseTile;
		private Point2I			selectedRoom;
		private Point2I			selectedTile;
		private Point2I			selectedTilesetTile;
		private TileData		selectedTilesetTileData;
		private bool			playerPlaceMode;
		private bool			sampleFromAllLayers; // TODO: implement this.
		private bool			showEvents;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EditorControl() {
			this.propertyGridControl	= null;
			this.worldFilePath	= String.Empty;
			this.worldFileName	= "untitled";
			this.world			= null;
			this.level			= null;
			this.tileset		= null;
			this.zone			= null;
			this.rewardManager	= null;
			this.inventory		= null;
			this.timer			= null;
			this.ticks			= 0;
			this.roomSpacing	= 1;
			this.playAnimations	= false;
			this.isInitialized	= false;
			this.hasMadeChanges	= false;

			this.currentLayer				= 0;
			this.currentToolIndex			= 0;
			this.aboveTileDrawMode			= TileDrawModes.Fade;
			this.belowTileDrawMode			= TileDrawModes.Fade;
			this.showRewards				= true;
			this.showGrid					= false;
			this.showEvents					= false;
			this.highlightMouseTile			= true;
			this.selectedRoom				= -Point2I.One;
			this.selectedTile				= -Point2I.One;
			this.selectedTilesetTile		= Point2I.Zero;
			this.selectedTilesetTileData	= null;
			this.playerPlaceMode			= false;
			this.sampleFromAllLayers		= false;
		}

		public void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice) {
			if (!isInitialized) {
				Resources.Initialize(contentManager, graphicsDevice);
				GameData.Initialize();
				EditorResources.Initialize();

				this.inventory		= new Inventory(null);
				this.rewardManager	= new RewardManager(null);
				this.timer			= Stopwatch.StartNew();
				this.ticks			= 0;
				this.roomSpacing	= 1;
				this.playAnimations = false;
				this.tileset		= GameData.TILESET_CLIFFS;
				this.zone			= GameData.ZONE_PRESENT;
				this.selectedTilesetTileData = this.tileset.TileData[0, 0];
				this.eventMode		= false;

				GameData.LoadInventory(inventory);
				GameData.LoadRewards(rewardManager);

				// Create tileset combo box.
				editorForm.ComboBoxTilesets.Items.Clear();
				foreach (KeyValuePair<string, Tileset> entry in Resources.GetResourceDictionary<Tileset>()) {
					editorForm.ComboBoxTilesets.Items.Add(entry.Key);
				}
				editorForm.ComboBoxTilesets.SelectedIndex = 0;
				
				// Create zone combo box.
				editorForm.ComboBoxZones.Items.Clear();
				foreach (KeyValuePair<string, Zone> entry in Resources.GetResourceDictionary<Zone>()) {
					if (tileset.SpriteSheet.Image.HasVariant(entry.Key))
						editorForm.ComboBoxZones.Items.Add(entry.Key);
				}
				editorForm.ComboBoxZones.SelectedIndex = 0;

				// Create controllers.
				propertyGridControl = new PropertyGridControl(this, editorForm.PropertyGrid);

				// Create tools.
				tools = new List<EditorTool>();
				AddTool(toolPointer		= new ToolPointer());
				AddTool(toolPlace		= new ToolPlace());
				AddTool(toolSelection	= new ToolSelection());
				AddTool(toolEyedrop		= new ToolEyedrop());
				currentToolIndex = 0;
				tools[currentToolIndex].OnBegin();

				this.isInitialized = true;
			}
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		public void UpdateWindowTitle() {
			editorForm.Text = "Oracle Engine Editor - " + worldFileName;
			if (hasMadeChanges)
				editorForm.Text += "*";
			if (level != null)
				editorForm.Text += " [" + level.Properties.GetString("id") + "]";
		}


		//-----------------------------------------------------------------------------
		// World
		//-----------------------------------------------------------------------------

		// Save the world file to the given filename.
		public void SaveFileAs(string fileName) {
			if (IsWorldOpen) {
				WorldFile saveFile = new WorldFile();
				saveFile.Save(fileName, world);
				hasMadeChanges = false;
			}
		}

		// Open a world file with the given filename.
		public void OpenFile(string fileName) {
			CloseFile();

			hasMadeChanges = false;
			worldFilePath = fileName;
			worldFileName = Path.GetFileName(fileName);

			// Load the world.
			WorldFile worldFile = new WorldFile();
			world = worldFile.Load(fileName);
			if (world.Levels.Count > 0)
				OpenLevel(0);

			RefreshWorldTreeView();
			editorForm.LevelTreeView.ExpandAll();
		}

		// Close the world file.
		public void CloseFile() {
			if (IsWorldOpen) {
				propertyGridControl.CloseProperties();
				world			= null;
				level			= null;
				hasMadeChanges	= false;
				worldFilePath	= "";
				editorForm.LevelTreeView.Nodes.Clear();
			}
		}

		// Open the given level index in the level display.
		public void OpenLevel(int index) {
			level = world.Levels[index];
			editorForm.LevelDisplay.UpdateLevel();
			UpdateWindowTitle();
			propertyGridControl.OpenProperties(level.Properties, level);
		}

		public void CloseLevel() {
			level = null;
			editorForm.LevelDisplay.UpdateLevel();
			UpdateWindowTitle();
			propertyGridControl.CloseProperties();
		}

		// Add a new level the world, and open it if specified.
		public void AddLevel(Level level, bool openLevel) {
			world.Levels.Add(level);
			
			// Add node in level list.
			//TreeNode levelNode = new TreeNode(level.Name);
			//editorForm.LevelTreeView.Nodes[0].Nodes.Add(levelNode);
			//levelNode.ContextMenuStrip = editorForm.ContextMenuLevelSelect;
			RefreshWorldTreeView();

			if (openLevel)
				OpenLevel(world.Levels.Count - 1);

		}

		public void ChangeTileset(string name) {
			tileset = Resources.GetResource<Tileset>(name);

			int index = 0;
			if (!tileset.SpriteSheet.Image.HasVariant(zone.ID))
				zone = Resources.GetResource<Zone>(tileset.SpriteSheet.Image.VariantName);
			editorForm.ComboBoxZones.Items.Clear();
			foreach (KeyValuePair<string, Zone> entry in Resources.GetResourceDictionary<Zone>()) {
				if (tileset.SpriteSheet.Image.HasVariant(entry.Key)) {
					editorForm.ComboBoxZones.Items.Add(entry.Key);
					if (entry.Key == zone.ID)
						editorForm.ComboBoxZones.SelectedIndex = index;
					index++;
				}
			}
			editorForm.TileDisplay.UpdateTileset();
			editorForm.TileDisplay.UpdateZone();

		}

		public void ChangeZone(string name) {
			if (name != "(none)") {
				zone = Resources.GetResource<Zone>(name);
				editorForm.TileDisplay.UpdateZone();
			}
		}

		// Test/play the world.
		public void TestWorld() {
			if (IsWorldOpen) {
				string worldPath = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "testing.zwd");
				WorldFile worldFile = new WorldFile();
				worldFile.Save(worldPath, world);
				string exePath = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "ZeldaOracle.exe");
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
				string worldPath = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "testing.zwd");
				WorldFile worldFile = new WorldFile();
				worldFile.Save(worldPath, world);
				string exePath = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "ZeldaOracle.exe");
				Process.Start(exePath, "\"" + worldPath + "\" -test " + levelIndex + " " + roomCoord.X + " " + roomCoord.Y + " " + playerCoord.X + " " + playerCoord.Y);
				// TODO: editorForm.ButtonTestPlayerPlace.Checked = false;
			}
		}


		public void RefreshWorldTreeView() {
			TreeView worldTreeView = editorForm.LevelTreeView;
			TreeNode worldNode, levelsNode, areasNode, dungeonsNode, scriptsNode;
			if (world == null) {
				worldTreeView.Nodes.Clear();
			}
			else {
				if (worldTreeView.Nodes.Count == 0) {
					worldNode = new TreeNode(world.Properties.GetString("id"), 0, 0);
					levelsNode = new TreeNode("Levels", 1, 1);
					areasNode = new TreeNode("Areas", 3, 3);
					dungeonsNode = new TreeNode("Dungeons", 5, 5);
					scriptsNode = new TreeNode("Scripts", 7, 7);
					worldNode.Name = "world";
					levelsNode.Name = "levels";
					areasNode.Name = "areas";
					dungeonsNode.Name = "dungeons";
					scriptsNode.Name = "scripts";
					worldTreeView.Nodes.Add(worldNode);
					worldNode.Nodes.Add(levelsNode);
					worldNode.Nodes.Add(areasNode);
					worldNode.Nodes.Add(dungeonsNode);
					worldNode.Nodes.Add(scriptsNode);

					worldNode.ContextMenuStrip	= editorForm.ContenxtMenuGeneral;
				}
				else {
					worldNode = worldTreeView.Nodes[0];
					levelsNode = worldNode.Nodes[0];
					areasNode = worldNode.Nodes[1];
					dungeonsNode = worldNode.Nodes[2];
					scriptsNode = worldNode.Nodes[3];

					levelsNode.Nodes.Clear();
					areasNode.Nodes.Clear();
					dungeonsNode.Nodes.Clear();
					scriptsNode.Nodes.Clear();
				}
				worldNode.Text = world.Properties.GetString("id");
				for (int i = 0; i < world.Levels.Count; i++) {
					TreeNode levelNode = new TreeNode(world.Levels[i].Properties.GetString("id"), 2, 2);
					levelNode.Name = "level";
					levelsNode.Nodes.Add(levelNode);
					levelNode.ContextMenuStrip = editorForm.ContextMenuLevelSelect;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Tiles
		//-----------------------------------------------------------------------------

		// Open the properties for the given tile in the property grid.
		public void OpenObjectProperties(IPropertyObject propertyObject) {
			propertyGridControl.OpenProperties(propertyObject.Properties, propertyObject);
		}


		//-----------------------------------------------------------------------------
		// Tools
		//-----------------------------------------------------------------------------
		
		// Change the current tool to the tool of the given index.
		public void ChangeTool(int toolIndex) {
			if (toolIndex != currentToolIndex) {
				tools[currentToolIndex].OnEnd();

				currentToolIndex = toolIndex;
				if (currentToolIndex != 0) {
					selectedRoom = -Point2I.One;
					selectedTile = -Point2I.One;
				}

				editorForm.OnToolChange(toolIndex);
				tools[currentToolIndex].OnBegin();
			}
		}
		
		// Add a new tool to the list of tools and initialize it.
		private EditorTool AddTool(EditorTool tool) {
			tool.Initialize(this);
			tools.Add(tool);
			return tool;
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

		public EditorForm EditorForm {
			get { return editorForm; }
			set { editorForm = value; }
		}
		
		public PropertyGridControl PropertyGridControl {
			get { return propertyGridControl; }
		}
		
		public LevelDisplay LevelDisplay {
			get { return editorForm.LevelDisplay; }
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

		public Tileset Tileset {
			get { return tileset; }
		}

		public Zone Zone {
			get { return zone; }
		}

		public Point2I SelectedRoom {
			get { return selectedRoom; }
			set { selectedRoom = value; }
		}

		public Point2I SelectedTile {
			get { return selectedTile; }
			set { selectedTile = value; }
		}

		public Point2I SelectedTilesetTile {
			get { return selectedTilesetTile; }
			set { selectedTilesetTile = value; }
		}

		public TileData SelectedTilesetTileData {
			get { return selectedTilesetTileData; }
			set { selectedTilesetTileData = value; }
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
			set { currentToolIndex = value; }
		}

		public EditorTool CurrentTool {
			get {
				if (tools == null)
					return null;
				return tools[currentToolIndex];
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

		public bool ShowEvents {
			get { return showEvents; }
			set { showEvents = value; }
		}

		public bool EventMode {
			get { return eventMode; }
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

		public bool HasMadeChanges {
			get { return hasMadeChanges; }
		}
	}
}
