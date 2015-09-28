using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsGraphicsDevice;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;

namespace ZeldaEditor {
	public class LevelDisplay : GraphicsDeviceControl {

		private static ContentManager content;
		private static Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;
		private static Microsoft.Xna.Framework.Graphics.Texture2D texture;
		private static bool isInitialized = false;

		private EditorForm	editorForm;		private Stopwatch	timer;
		private int			ticks;			// The number of ticks elapsed.
		private World		world;
		private Level		level;			// The current level being displayed.
		private int			roomSpacing;	// Spacing in pixels between rooms
		
		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public Point2I GetRoomLoc(Point2I point, bool clamp) {
			Point2I span = (level.RoomSize * GameSettings.TILE_SIZE) + new Point2I(roomSpacing, roomSpacing);
			Point2I loc = point / span;
			if (clamp)
				return GMath.Clamp(loc, Point2I.Zero, level.Dimensions);
			return loc;
		}

		public Room GetRoom(Point2I point) {
			if (level == null)
				return null;
			Point2I loc = GetRoomLoc(point, false);
			if (level.ContainsRoom(loc))
				return level.GetRoom(loc);
			return null;
		}
		
		public Point2I GetTileLoc(Point2I point) {
			Point2I span = (level.RoomSize * GameSettings.TILE_SIZE) + new Point2I(roomSpacing, roomSpacing);
			Point2I begin = GetRoomLoc(point, false) * span;
			Point2I tileLoc = (point - begin) / GameSettings.TILE_SIZE;
			return GMath.Clamp(tileLoc, Point2I.Zero, level.RoomSize);
		}
		
		public TileData GetTopTile(Point2I point) {
			if (level == null)
				return null;
			Room room = GetRoom(point);
			if (room == null)
				return null;
			Point2I tileLoc = GetTileLoc(point);
			for (int i = room.LayerCount - 1; i >= 0; i--) {
				TileData t = room.TileData[tileLoc.X, tileLoc.Y, i];
				if (t != null)
					return t;
			}
			return null;
		}


		//-----------------------------------------------------------------------------
		// Mutators
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
			world = worldFile.Load("Content/Worlds/custom_world.zwd");
			if (world.Levels.Count > 0)
				level = world.Levels[0];

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
		}
		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		private void OnLeftClickTile() {

		}

		private void OnMouseDown(object sender, MouseEventArgs e) {
			if (IsLevelOpen) {
				Point2I mouseLoc = new Point2I(e.X, e.Y);
				TileData tileData = GetTopTile(mouseLoc);

				if (tileData != null) {
					// Do something.
				}
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e) {
			if (IsLevelOpen) {
				Point2I mouseLoc = new Point2I(e.X, e.Y);
				Point2I roomLoc = GetRoomLoc(mouseLoc, false);
				Point2I tileLoc = GetTileLoc(mouseLoc);

				if (level.ContainsRoom(roomLoc)) {
					editorForm.StatusBarLabelRoomLoc.Text = "Room (" + roomLoc.X + ", " + roomLoc.Y + ")";
					editorForm.StatusBarLabelTileLoc.Text = "Tile (" + tileLoc.X + ", " + tileLoc.Y + ")";
					return;
				}
			}
			editorForm.StatusBarLabelRoomLoc.Text = "Room (?, ?)";
			editorForm.StatusBarLabelTileLoc.Text = "Tile (?, ?)";
		}


		//-----------------------------------------------------------------------------
		// Overriden methods
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			if (!DesignMode && !isInitialized) {
				content		= new ContentManager(Services, "Content");
				spriteBatch	= new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);
				
				Resources.Initialize(content, GraphicsDevice);
				GameData.Initialize();
				isInitialized = true;
				
				// Wire the events.
				MouseMove += OnMouseMove;
				MouseDown += OnMouseDown;

				// Start the timer to refresh the panel.
				timer = Stopwatch.StartNew();
				Application.Idle += delegate { Invalidate(); };
				
				roomSpacing = 1;
			}
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				content.Unload();
			}

			base.Dispose(disposing);
		}

		private void DrawRoom(Graphics2D g, Room room) {
			for (int i = 0; i < room.LayerCount; i++) {
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						TileData data = room.TileData[x, y, i];
						Vector2F position = new Vector2F(x, y) * GameSettings.TILE_SIZE;
						
						if (data != null) {
							if (data.Animation != null) {
								g.DrawAnimation(data.Animation, room.Zone.ImageVariantID, ticks, position);
							}
							else if (data.Sprite != null) {
								g.DrawSprite(data.Sprite, room.Zone.ImageVariantID, position);
							}
						}
					}
				}
			}
		}

		protected override void Draw() {
			double time = timer.Elapsed.TotalSeconds;
			ticks = (int) (time * 60.0f);
			if (!editorForm.PlayAnimations)
				ticks = 0;

			GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
			Graphics2D g = new Graphics2D(spriteBatch);

			//g.SetRenderTarget(GameData.RenderTargetGame);

			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			g.Clear(Color.Black);
			
			if (IsLevelOpen) {
				// Draw the rooms.
				for (int x = 0; x < level.Width; x++) {
					for (int y = 0; y < level.Height; y++) {
						g.Translate((Vector2F) (new Point2I(x, y) * ((level.RoomSize * GameSettings.TILE_SIZE) + new Point2I(roomSpacing, roomSpacing))));
						DrawRoom(g, level.GetRoom(x, y));
						g.ResetTranslation();
					}
				}
			}

			g.End();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorForm EditorForm {
			get { return editorForm; }
			set { editorForm = value; }
		}
		
		public bool IsWorldOpen {
			get { return (world != null); }
		}
		
		public bool IsLevelOpen {
			get { return (world != null && level != null); }
		}
	}
}
