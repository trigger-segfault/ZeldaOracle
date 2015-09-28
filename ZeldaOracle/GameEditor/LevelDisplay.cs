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
			this.ResizeRedraw = true;

			// Start the timer to refresh the panel.
			Application.Idle += delegate { Invalidate(); };

			UpdateLevel();
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
				return GMath.Clamp(roomCoord, Point2I.Zero, Level.Dimensions);
			return roomCoord;
		}

		public Room GetRoom(Point2I point) {
			if (editorControl.IsLevelOpen)
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
			return GMath.Clamp(tileCoord, Point2I.Zero, Level.RoomSize);
		}
		
		public TileData GetTopTile(Point2I point) {
			if (editorControl.IsLevelOpen)
				return null;
			Room room = GetRoom(point);
			if (room == null)
				return null;
			Point2I tileCoord = GetTileCoordinates(point);
			for (int i = room.LayerCount - 1; i >= 0; i--) {
				TileData t = room.TileData[tileCoord.X, tileCoord.Y, i];
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
				TileData tileData = GetTopTile(mousePos);

				if (tileData != null) {
					// Do something.
				}
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e) {
			if (editorControl.IsLevelOpen) {
				Point2I mousePos = ScrollPosition + e.Location;
				Point2I roomCoord = GetRoomCoordinates(mousePos, false);
				Point2I tileCoord = GetTileCoordinates(mousePos);

				if (Level.ContainsRoom(roomCoord)) {
					editorForm.StatusBarLabelRoomLoc.Text = "Room " + roomCoord.ToString();
					editorForm.StatusBarLabelTileLoc.Text = "Tile " + tileCoord.ToString();
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
			for (int i = 0; i < room.LayerCount; i++) {
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						TileData data = room.TileData[x, y, i];
						Vector2F position = new Vector2F(x, y) * GameSettings.TILE_SIZE;
						
						if (data != null) {
							if (data.Animation != null) {
								g.DrawAnimation(data.Animation, room.Zone.ImageVariantID, editorControl.Ticks, position);
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

