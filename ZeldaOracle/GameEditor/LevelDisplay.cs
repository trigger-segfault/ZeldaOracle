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
using ZeldaOracle.Common.Audio;
using ZeldaEditor.Control;

namespace ZeldaEditor {
	public class LevelDisplay : GraphicsDeviceControl {

		private static ContentManager content;
		private static Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;

		private EditorForm	editorForm;
		private EditorControl editorControl;

		private Point2I highlightedRoom;
		private Point2I highlightedTile;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			content		= new ContentManager(Services, "Content");
			spriteBatch	= new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);

			editorControl.Initialize(content, GraphicsDevice);

			// Wire the events.
			MouseMove += OnMouseMove;
			MouseDown += OnMouseDown;
			MouseLeave += OnMouseLeave;
			MouseEnter += OnMouseEnter;
			this.ResizeRedraw = true;

			// Start the timer to refresh the panel.
			Application.Idle += delegate { Invalidate(); };

			editorControl.OpenFile("../../../../WorldFiles/temp_world.zwd");

			UpdateLevel();

			this.highlightedRoom = -Point2I.One;
			this.highlightedTile = -Point2I.One;
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				content.Unload();
			}

			base.Dispose(disposing);
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public Point2I GetRoomCoordinates(Point2I point, bool clamp) {
			Point2I span = (Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing;
			Point2I roomCoord = point / span;
			if (clamp)
				return GMath.Clamp(roomCoord, Point2I.Zero, Level.Dimensions - 1);
			return roomCoord;
		}

		public Room GetRoom(Point2I point) {
			if (!editorControl.IsLevelOpen)
				return null;
			Point2I roomCoord = GetRoomCoordinates(point, false);
			if (Level.ContainsRoom(roomCoord))
				return Level.GetRoom(roomCoord);
			return null;
		}

		public Point2I GetTileCoordinates(Point2I point) {
			Point2I span = (Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing;
			Point2I begin = GetRoomCoordinates(point, false) * span;
			Point2I tileCoord = (point - begin) / GameSettings.TILE_SIZE;
			return GMath.Clamp(tileCoord, Point2I.Zero, Level.RoomSize - 1);
		}

		public TileDataInstance GetTile(Point2I point, int layer) {
			if (!editorControl.IsLevelOpen)
				return null;
			Room room = GetRoom(point);
			if (room == null)
				return null;
			Point2I tileCoord = GetTileCoordinates(point);
			return room.GetTile(tileCoord.X, tileCoord.Y, layer);
		}
		public TileDataInstance GetTopTile(Point2I point) {
			if (!editorControl.IsLevelOpen)
				return null;
			Room room = GetRoom(point);
			if (room == null)
				return null;
			Point2I tileCoord = GetTileCoordinates(point);
			for (int i = room.LayerCount - 1; i >= 0; i--) {
				TileDataInstance t = room.GetTile(tileCoord.X, tileCoord.Y, i);
				if (t != null)
					return t;
			}
			return null;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		public void UpdateLevel() {
			if (editorControl.IsLevelOpen) {
				this.AutoScrollMinSize = Level.Dimensions * (Level.RoomSize * GameSettings.TILE_SIZE + editorControl.RoomSpacing);
				this.HorizontalScroll.Value = 0;
				this.VerticalScroll.Value = 0;
			}
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		private void OnTileLeftClick() {

		}

		private void OnMouseDown(object sender, MouseEventArgs e) {
			if (editorControl.IsLevelOpen) {
				Point2I mousePos = ScrollPosition + e.Location;
				Room room = GetRoom(mousePos);
				TileDataInstance tile = GetTile(mousePos, editorControl.CurrentLayer);

				// Do something.
				switch (editorControl.CurrentTool) {
				case 0:
					if (tile != null) {
						editorControl.SelectedRoom = GetRoomCoordinates(mousePos, false);
						editorControl.SelectedTile = GetTileCoordinates(mousePos);
						EditorControl.OpenTileProperties(tile);
					}
					break;
				case 1:
					if (e.Button == System.Windows.Forms.MouseButtons.Left) {
						room.CreateTile(
							editorControl.Tileset.TileData[
								editorControl.SelectedTilesetTile.X,
								editorControl.SelectedTilesetTile.Y
							],
							highlightedTile.X, highlightedTile.Y, editorControl.CurrentLayer
						);
					}
					else if (e.Button == System.Windows.Forms.MouseButtons.Right) {
						if (editorControl.CurrentLayer == 0) {
							room.CreateTile(
								editorControl.Tileset.TileData[
									editorControl.Tileset.DefaultTile.X,
									editorControl.Tileset.DefaultTile.Y
								],
								highlightedTile.X, highlightedTile.Y, editorControl.CurrentLayer
							);
						}
						else {
							room.RemoveTile(
								highlightedTile.X, highlightedTile.Y, editorControl.CurrentLayer
							);
						}
					}

					break;
				}
			}
			this.Focus();
		}
		private void OnMouseLeave(object sender, EventArgs e) {
			highlightedRoom = -Point2I.One;
			highlightedTile = -Point2I.One;
		}
		private void OnMouseEnter(object sender, EventArgs e) {
			this.Focus();
		}

		private void OnMouseMove(object sender, MouseEventArgs e) {
			if (editorControl.IsLevelOpen) {
				Point2I mousePos = ScrollPosition + e.Location;
				Room room = GetRoom(mousePos);
				highlightedRoom = GetRoomCoordinates(mousePos, false);
				highlightedTile = GetTileCoordinates(mousePos);
				if (!(highlightedRoom < Level.Dimensions)) {
					highlightedRoom = -Point2I.One;
					highlightedTile = -Point2I.One;
				}
				TileDataInstance tile = GetTile(mousePos, editorControl.CurrentLayer);

				switch (editorControl.CurrentTool) {
				case 0:
					break;
				case 1:
					if (e.Button == System.Windows.Forms.MouseButtons.Left) {
						room.CreateTile(
							editorControl.Tileset.TileData[
								editorControl.SelectedTilesetTile.X,
								editorControl.SelectedTilesetTile.Y
							],
							highlightedTile.X, highlightedTile.Y, editorControl.CurrentLayer
						);
					}
					else if (e.Button == System.Windows.Forms.MouseButtons.Right) {
						if (editorControl.CurrentLayer == 0) {
							room.CreateTile(
								editorControl.Tileset.TileData[
									editorControl.Tileset.DefaultTile.X,
									editorControl.Tileset.DefaultTile.Y
								],
								highlightedTile.X, highlightedTile.Y, editorControl.CurrentLayer
							);
						}
						else {
							room.RemoveTile(
								highlightedTile.X, highlightedTile.Y, editorControl.CurrentLayer
							);
						}
					}

					break;
				}

				if (Level.ContainsRoom(highlightedRoom)) {
					editorForm.StatusBarLabelRoomLoc.Text = "Room " + highlightedRoom.ToString();
					editorForm.StatusBarLabelTileLoc.Text = "Tile " + highlightedTile.ToString();
					return;
				}
			}
			editorForm.StatusBarLabelRoomLoc.Text = "Room (?, ?)";
			editorForm.StatusBarLabelTileLoc.Text = "Tile (?, ?)";
		}


		//-----------------------------------------------------------------------------
		// Overriden methods
		//-----------------------------------------------------------------------------

		private void DrawRoom(Graphics2D g, Room room) {
			Color belowFade = new Color(255, 255, 255, 150);
			Color aboveFade = new Color(255, 255, 255, 100);
			Color hide = Color.Transparent;
			Color normal = Color.White;
			for (int i = 0; i < room.LayerCount; i++) {
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						TileDataInstance data = room.GetTile(x, y, i);
						Vector2F position = new Vector2F(x, y) * GameSettings.TILE_SIZE;

						if (i == 0) {
							g.FillRectangle(new Rectangle2I((Point2I)position, new Point2I(GameSettings.TILE_SIZE)), Color.White);
						}
						Color color = normal;
						if (editorControl.CurrentLayer > i) {
							if (editorControl.BelowTileDrawMode == TileDrawModes.Hide)
								color = hide;
							else if (editorControl.BelowTileDrawMode == TileDrawModes.Fade)
								color = belowFade;
						}
						else if (editorControl.CurrentLayer < i) {
							if (editorControl.AboveTileDrawMode == TileDrawModes.Hide)
								color = hide;
							else if (editorControl.AboveTileDrawMode == TileDrawModes.Fade)
								color = aboveFade;
						}
						if (data != null) {
							if (data.Animation != null) {
								g.DrawAnimation(data.Animation, room.Zone.ImageVariantID, editorControl.Ticks, position, color);
							}
							else if (data.Sprite != null) {
								g.DrawSprite(data.Sprite, room.Zone.ImageVariantID, position, color);
							}
							if (editorControl.ShowRewards && data.ModifiedProperties.Exists("reward") &&
								editorControl.RewardManager.HasReward(data.ModifiedProperties.GetString("reward")))
							{
								Animation anim = editorControl.RewardManager.GetReward(data.ModifiedProperties.GetString("reward")).Animation;
								g.DrawAnimation(anim, editorControl.Ticks, position, color);
							}
						}

						if (i == room.LayerCount - 1) {
							if (editorControl.ShowGrid)
								g.DrawRectangle(new Rectangle2I((Point2I)position, new Point2I(GameSettings.TILE_SIZE + 1)), 1, new Color(0, 0, 0, 150));
						}
					}
				}
			}

			if (editorControl.RoomSpacing > 0) {
				g.FillRectangle(new Rectangle2I(0, room.Height * GameSettings.TILE_SIZE, room.Width * GameSettings.TILE_SIZE, editorControl.RoomSpacing), Color.Black);
				g.FillRectangle(new Rectangle2I(room.Width * GameSettings.TILE_SIZE, 0, editorControl.RoomSpacing, room.Height * GameSettings.TILE_SIZE + editorControl.RoomSpacing), Color.Black);
			}
		}

		protected override void Draw() {
			editorControl.UpdateTicks();

			Graphics2D g = new Graphics2D(spriteBatch);

			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			g.Clear(new Color(175, 175, 180));

			if (editorControl.IsLevelOpen) {
				// Draw the rooms.
				for (int x = 0; x < Level.Width; x++) {
					for (int y = 0; y < Level.Height; y++) {
						g.Translate(new Vector2F(-this.HorizontalScroll.Value, -this.VerticalScroll.Value));
						g.Translate((Vector2F)(new Point2I(x, y) * ((Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing)));
						DrawRoom(g, Level.GetRoom(x, y));
						g.ResetTranslation();
					}
				}


				g.Translate(new Vector2F(-this.HorizontalScroll.Value, -this.VerticalScroll.Value));
				Point2I roomSize = (Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing;
				Point2I tilePoint = highlightedRoom * roomSize + highlightedTile * GameSettings.TILE_SIZE;
				if (editorControl.HighlightMouseTile && highlightedTile >= Point2I.Zero) {
					g.FillRectangle(new Rectangle2I(tilePoint, new Point2I(GameSettings.TILE_SIZE + 1)), Color.White * 0.8f);
					g.DrawRectangle(new Rectangle2I(tilePoint, new Point2I(GameSettings.TILE_SIZE + 1)), 1, Color.Black);
				}
				tilePoint = editorControl.SelectedRoom * roomSize + editorControl.SelectedTile * GameSettings.TILE_SIZE;
				if (editorControl.SelectedTile >= Point2I.Zero) {
					g.DrawRectangle(new Rectangle2I(tilePoint, new Point2I(GameSettings.TILE_SIZE + 1)), 1, Color.White);
					g.DrawRectangle(new Rectangle2I(tilePoint + 1, new Point2I(GameSettings.TILE_SIZE - 1)), 1, Color.Black);
					g.DrawRectangle(new Rectangle2I(tilePoint - 1, new Point2I(GameSettings.TILE_SIZE + 3)), 1, Color.Black);
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

		public EditorControl EditorControl {
			get { return editorControl; }
			set { editorControl = value; }
		}

		public Level Level {
			get { return editorControl.Level; }
		}

		public World World {
			get { return editorControl.World; }
		}

		public Point2I ScrollPosition {
			get { return new Point2I(HorizontalScroll.Value, VerticalScroll.Value); }
		}
	}
}

