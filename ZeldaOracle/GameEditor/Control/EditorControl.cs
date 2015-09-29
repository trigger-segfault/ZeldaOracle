using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace ZeldaEditor.Control {

	public class EditorControl {

		private bool isInitialized;

		private EditorForm		editorForm;
		private PropertyGridControl propertyGridControl;

		private World			world;
		private Level			level;
		private Tileset			tileset;
		private Zone			zone;
		private RewardManager	rewardManager;
		private Inventory		inventory;

		private Stopwatch		timer;
		private int				ticks;

		private bool			playAnimations;

		// Editing
		private int				roomSpacing;
		private int				currentLayer;
		private int				currentTool;
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


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EditorControl() {
			this.propertyGridControl	= null;
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

			this.currentLayer	= 0;
			this.currentTool	= 0;
			this.aboveTileDrawMode	= TileDrawModes.Fade;
			this.belowTileDrawMode	= TileDrawModes.Fade;
			this.showRewards	= true;
			this.showGrid		= false;
			this.highlightMouseTile	= true;
			this.selectedRoom	= -Point2I.One;
			this.selectedTile	= -Point2I.One;
			this.selectedTilesetTile = Point2I.Zero;
			this.selectedTilesetTileData	= null;
			this.playerPlaceMode	= false;
		}

		public void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice) {
			if (!isInitialized) {
				Resources.Initialize(contentManager, graphicsDevice);
				GameData.Initialize();

				this.inventory = new Inventory(null);
				this.rewardManager = new RewardManager(null);
				this.timer = Stopwatch.StartNew();
				this.ticks = 0;
				this.roomSpacing = 1;
				this.playAnimations = false;
				this.tileset		= GameData.TILESET_OVERWORLD;
				this.zone			= GameData.ZONE_SUMMER;

				this.selectedTilesetTileData = this.tileset.TileData[0, 0];

				GameData.LoadInventory(inventory);
				GameData.LoadRewards(rewardManager);

				editorForm.ComboBoxTilesets.Items.Clear();
				foreach (KeyValuePair<string, Tileset> entry in Resources.GetResourceDictionary<Tileset>()) {
					editorForm.ComboBoxTilesets.Items.Add(entry.Key);
				}
				editorForm.ComboBoxTilesets.SelectedIndex = 0;

				editorForm.ComboBoxZones.Items.Clear();
				foreach (KeyValuePair<string, Zone> entry in Resources.GetResourceDictionary<Zone>()) {
					if (tileset.SpriteSheet.Image.HasVariant(entry.Key))
						editorForm.ComboBoxZones.Items.Add(entry.Key);
				}
				editorForm.ComboBoxZones.SelectedIndex = 0;

				propertyGridControl = new PropertyGridControl(this, editorForm.PropertyGrid);

				this.isInitialized = true;
			}
		}

		//-----------------------------------------------------------------------------
		// World
		//-----------------------------------------------------------------------------

		public void SaveFile(string fileName) {
			if (IsWorldOpen) {
				WorldFile saveFile = new WorldFile();
				saveFile.Save(fileName, world);
			}
		}

		public void OpenFile(string fileName) {
			CloseFile();

			// Load the world.
			WorldFile worldFile = new WorldFile();
			world = worldFile.Load(fileName);
			if (world.Levels.Count > 0)
				OpenLevel(0);

			// Populate the level list. (tree view).
			TreeNode worldNode = new TreeNode("World Name");
			editorForm.LevelTreeView.Nodes.Clear();
			editorForm.LevelTreeView.Nodes.Add(worldNode);
			for (int i = 0; i < world.Levels.Count; i++) {
				TreeNode levelNode = new TreeNode("Level" + (i + 1));
				worldNode.Nodes.Add(levelNode);
				levelNode.ContextMenuStrip = editorForm.ContextMenuLevelSelect;
			}
			worldNode.Expand();
		}

		public void CloseFile() {
			if (IsWorldOpen) {
				world = null;
				level = null;
				editorForm.LevelTreeView.Nodes.Clear();
			}
		}

		public void OpenLevel(int index) {
			level = world.Levels[index];
			editorForm.LevelDisplay.UpdateLevel();
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
			zone = Resources.GetResource<Zone>(name);
			editorForm.TileDisplay.UpdateZone();
		}

		public void ChangeTool(int toolIndex) {
			currentTool = toolIndex;
			if (currentTool != 0) {
				selectedRoom = -Point2I.One;
				selectedTile = -Point2I.One;
			}
		}

		public void TestWorld() {
			string worldPath = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "testing.zwd");
			WorldFile worldFile = new WorldFile();
			worldFile.Save(worldPath, world);
			string exePath = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "ZeldaOracle.exe");
			Process.Start(exePath, "\"" + worldPath + "\"");
		}

		public void TestWorld(Point2I roomCoord, Point2I playerCoord) {
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
			editorForm.ButtonTestPlayerPlace.Checked = false;
		}

		//-----------------------------------------------------------------------------
		// Tiles
		//-----------------------------------------------------------------------------

		public void OpenTileProperties(TileDataInstance tile) {
			propertyGridControl.OpenProperties(tile.ModifiedProperties, tile.BaseProperties);
		}
		
		public void CloseProperties(TileDataInstance tile) {
			propertyGridControl.CloseProperties();
		}


		//-----------------------------------------------------------------------------
		// Ticks
		//-----------------------------------------------------------------------------

		public void UpdateTicks() {
			double time = timer.Elapsed.TotalSeconds;
			ticks = (int)(time * 60.0);
			if (!playAnimations)
				ticks = 0;
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

		public int CurrentTool {
			get { return currentTool; }
			set { currentTool = value; }
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

		public bool HighlightMouseTile {
			get { return highlightMouseTile; }
			set { highlightMouseTile = value; }
		}

		public bool PlayerPlaceMode {
			get { return playerPlaceMode; }
			set { playerPlaceMode = value; }
		}

		public TileData SelectedTilesetTileData {
			get { return selectedTilesetTileData; }
			set { selectedTilesetTileData = value; }
		}
	}
}
