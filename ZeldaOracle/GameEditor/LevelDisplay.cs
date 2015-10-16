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
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;
using ZeldaEditor.Control;

namespace ZeldaEditor {
	public class LevelDisplay : GraphicsDeviceControl {

		private static ContentManager content;
		private static Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;

		private EditorForm		editorForm;
		private EditorControl	editorControl;
		private Point2I			highlightedRoom;
		private Point2I			highlightedTile;
		private Point2I			cursorTileLocation;
		private Rectangle2I		selectionBox;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			content		= new ContentManager(Services, "Content");
			spriteBatch	= new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);

			editorControl.Initialize(content, GraphicsDevice);
			
			// Wire the events.
			MouseMove	+= OnMouseMove;
			MouseDown	+= OnMouseDown;
			MouseUp		+= OnMouseUp;
			MouseLeave	+= OnMouseLeave;
			
			this.ResizeRedraw = true;

			// Start the timer to refresh the panel.
			Application.Idle += delegate { Invalidate(); };

			// TEMP: Open this world file upon starting the editor.
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
		// Level Sampling
		//-----------------------------------------------------------------------------

		// Convert a tile location relative to a room, to an absolute tile location in the level.
		public Point2I GetLocationInLevel(Room room, Point2I tileLocationInRoom) {
			return ((Level.RoomSize * room.Location) + tileLocationInRoom);
		}

		// Sample the room coordinates at the given point, clamping them to the level's imensions if specified.
		public Point2I SampleRoomCoordinates(Point2I point, bool clamp = false) {
			Point2I span = (Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing;
			Point2I roomCoord = point / span;
			if (point.X < 0)
				roomCoord.X--;
			if (point.Y < 0)
				roomCoord.Y--;
			if (clamp && editorControl.IsLevelOpen) {
				if (editorControl.IsLevelOpen)
					return GMath.Clamp(roomCoord, Point2I.Zero, Level.Dimensions - 1);
				else
					return Point2I.Zero;
			}
			return roomCoord;
		}

		// Sample the oom at the given point, clamping to the levels dimensions if specified.
		public Room SampleRoom(Point2I point, bool clamp = false) {
			if (!editorControl.IsLevelOpen)
				return null;
			Point2I roomCoord = SampleRoomCoordinates(point, clamp);
			if (Level.ContainsRoom(roomCoord))
				return Level.GetRoomAt(roomCoord);
			return null;
		}
		
		// Sample the tile coordinates at the given point (tile coordinates are relative to their room).
		public Point2I SampleTileCoordinates(Point2I point) {
			Point2I span = (Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing;
			Point2I begin = SampleRoomCoordinates(point) * span;
			Point2I tileCoord = (point - begin) / GameSettings.TILE_SIZE;
			return GMath.Clamp(tileCoord, Point2I.Zero, Level.RoomSize - Point2I.One);
		}
		
		// Sample the tile coordinates at the given point (tile coordinates are absolute to the level).
		public Point2I SampleLevelTileCoordinates(Point2I point) {
			Point2I roomCoord = SampleRoomCoordinates(point);
			Point2I tileCoord = SampleTileCoordinates(point);
			return ((roomCoord * Level.RoomSize) + tileCoord);
		}
		
		// Convert a tile location relative to a room, to an absolute tile location in the level.
		public TileDataInstance SampleTile(Point2I point, int layer) {
			Room room = SampleRoom(point);
			if (room == null)
				return null;
			Point2I tileCoord = SampleTileCoordinates(point);
			return room.GetTile(tileCoord.X, tileCoord.Y, layer);
		}
		
		// Sample an event tile at the given point.
		public EventTileDataInstance SampleEventTile(Point2I point) {
			Room room = SampleRoom(point);
			if (room == null)
				return null;
			Point2I roomOffset = GetRoomDrawPosition(room);
			for (int i = 0; i < room.EventData.Count; i++) {
				EventTileDataInstance eventTile = room.EventData[i];
				Rectangle2I tileRect = new Rectangle2I(eventTile.Position + roomOffset, eventTile.Size * GameSettings.TILE_SIZE);
				if (tileRect.Contains(point))
					return eventTile;
			}
			return null;
		}
		

		//-----------------------------------------------------------------------------
		// Coordinates Conversion
		//-----------------------------------------------------------------------------

		// Convert room tile coordinates level tile coordinates.
		public Point2I ToLevelTileCoordinates(Room room, Point2I roomTileLocation) {
			return ((Level.RoomSize * room.Location) + roomTileLocation);
		}
		
		// Convert level tile coordinates room tile coordinates.
		public Point2I ToRoomTileCoordinates(Point2I levelTileLocation) {
			return GMath.Wrap(levelTileLocation, Level.RoomSize);
		}


		//-----------------------------------------------------------------------------
		// Selection Box
		//-----------------------------------------------------------------------------

		// Clear the selection box by setting it to zero.
		public void ClearSelectionBox() {
			selectionBox = Rectangle2I.Zero;
		}

		// Set the selection box
		public void SetSelectionBox(Point2I start, Point2I size) {
			selectionBox = new Rectangle2I(start, size);
		}


		//-----------------------------------------------------------------------------
		// Drawing Helper Functions
		//-----------------------------------------------------------------------------
		
		// Get the top-left position to draw the given room.
		public Point2I GetRoomDrawPosition(Room room) {
			return GetRoomDrawPosition(room.Location);
		}

		// Get the top-left position to draw the room of the given coordinates.
		private Point2I GetRoomDrawPosition(Point2I roomCoord) {
			return (roomCoord * ((Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing));
		}

		// Get the top-left position to draw a tile with the given absolute coordinates.
		private Point2I GetLevelTileCoordDrawPosition(Point2I levelTileCoord) {
			Point2I roomCoord = levelTileCoord / Level.RoomSize;
			if (levelTileCoord.X < 0)
				roomCoord.X--;
			if (levelTileCoord.Y < 0)
				roomCoord.Y--;
			Point2I tileCoord = ToRoomTileCoordinates(levelTileCoord);
			return (GetRoomDrawPosition(roomCoord) + (tileCoord * GameSettings.TILE_SIZE));
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------
		
		// Draw a tile.
		private void DrawTile(Graphics2D g, TileDataInstance tile, Color color) {
			Point2I position = tile.Location * GameSettings.TILE_SIZE;

			// Draw tile sprite/animation.
			if (!tile.CurrentSprite.IsNull)
				g.DrawAnimation(tile.CurrentSprite, tile.Room.Zone.ImageVariantID, editorControl.Ticks, position, color);

			// Draw rewards.
			if (editorControl.ShowRewards && tile.Properties.Exists("reward") &&
				editorControl.RewardManager.HasReward(tile.Properties.GetString("reward")))
			{
				Animation anim = editorControl.RewardManager.GetReward(tile.Properties.GetString("reward")).Animation;
				g.DrawAnimation(anim, editorControl.Ticks, position, color);
			}
		}

		// Draw an event tile.
		private void DrawEventTile(Graphics2D g, EventTileDataInstance eventTile, Color color) {
			SpriteAnimation spr = eventTile.CurrentSprite;
			/*
			// Select different sprites for certain events.
			if (eventTile.Type == typeof(WarpEvent)) {
				string warpType = eventTile.Properties.GetString("warp_type");
				if (warpType == "tunnel")
					spr = GameData.SPR_EVENT_TILE_WARP_TUNNEL;
				else if (warpType == "stairs")
					spr = GameData.SPR_EVENT_TILE_WARP_STAIRS;
				else if (warpType == "entrance")
					spr = GameData.SPR_EVENT_TILE_WARP_ENTRANCE;
			}*/
			
			// Draw the sprite.
			if (!spr.IsNull) {
				g.DrawAnimation(spr, eventTile.Room.Zone.ImageVariantID, editorControl.Ticks, eventTile.Position, Color.White);
			}
			else {
				Rectangle2I r = new Rectangle2I(eventTile.Position, eventTile.Size * GameSettings.TILE_SIZE);
				g.FillRectangle(r, Color.Blue);
			}
		}
		
		// Draw an entire room.
		private void DrawRoom(Graphics2D g, Room room) {
			Color belowFade = new Color(255, 255, 255, 150);
			Color aboveFade = new Color(255, 255, 255, 100);
			Color hide = Color.Transparent;
			Color normal = Color.White;
						
			// Draw white background.
			g.FillRectangle(new Rectangle2I(Point2I.Zero, room.Size * GameSettings.TILE_SIZE), Color.White);

			// Draw tile layers.
			for (int i = 0; i < room.LayerCount; i++) {
				// Determine color/transparency for layer based on layer visibility.
				Color color = normal;
				if (!editorControl.EventMode) {
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
				}
				
				// Draw the tile grid for this layer.
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						Vector2F position = new Vector2F(x, y) * GameSettings.TILE_SIZE;
						TileDataInstance tile = room.GetTile(x, y, i);
						
						// Draw tile.
						if (tile != null)
							DrawTile(g, tile, color);

						// Draw grid square.
						if (i == room.LayerCount - 1) {
							if (editorControl.ShowGrid)
								g.DrawRectangle(new Rectangle2I((Point2I) position, new Point2I(GameSettings.TILE_SIZE + 1)), 1, new Color(0, 0, 0, 150));
						}
					}
				}
			}

			// Draw event tiles.
			if (editorControl.ShowEvents || editorControl.EventMode) {
				for (int i = 0; i < room.EventData.Count; i++)
					DrawEventTile(g, room.EventData[i], Color.White);
			}

			// Draw the spacing lines between rooms.
			if (editorControl.RoomSpacing > 0) {
				g.FillRectangle(new Rectangle2I(0, room.Height * GameSettings.TILE_SIZE, room.Width * GameSettings.TILE_SIZE, editorControl.RoomSpacing), Color.Black);
				g.FillRectangle(new Rectangle2I(room.Width * GameSettings.TILE_SIZE, 0, editorControl.RoomSpacing, room.Height * GameSettings.TILE_SIZE + editorControl.RoomSpacing), Color.Black);
			}
		}
		
		// Draw an entire level.
		public void DrawLevel(Graphics2D g) {
			g.Clear(new Color(175, 175, 180));
			
			// Draw the level if it is open.
			if (editorControl.IsLevelOpen) {
				// Draw the rooms.
				for (int x = 0; x < Level.Width; x++) {
					for (int y = 0; y < Level.Height; y++) {
						g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
						g.Translate((Vector2F)(new Point2I(x, y) * ((Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing)));
						DrawRoom(g, Level.GetRoomAt(x, y));
						g.ResetTranslation();
					}
				}
				
				// Draw the highlight box.
				if (editorControl.HighlightMouseTile && cursorTileLocation >= Point2I.Zero) {
					g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
					Rectangle2I box = new Rectangle2I(GetLevelTileCoordDrawPosition(cursorTileLocation), new Point2I(16, 16));
					g.DrawRectangle(box.Inflated(1, 1), 1, Color.White);
					g.ResetTranslation();
				}

				// Draw the selection box.
				if (!selectionBox.IsEmpty) {
					g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
					Point2I start = GetLevelTileCoordDrawPosition(selectionBox.TopLeft);
					Point2I end   = GetLevelTileCoordDrawPosition(selectionBox.BottomRight);
					Rectangle2I box = new Rectangle2I(start, end - start);
					g.DrawRectangle(box, 1, Color.White);
					g.DrawRectangle(box.Inflated(1, 1), 1, Color.Black);
					g.DrawRectangle(box.Inflated(-1, -1), 1, Color.Black);
					g.ResetTranslation();
				}

				// Draw player sprite for 'Test At Position'
				Point2I roomSize = (Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing;
				Point2I tilePoint = highlightedRoom * roomSize + highlightedTile * GameSettings.TILE_SIZE;
				if (editorControl.PlayerPlaceMode && highlightedTile >= Point2I.Zero) {
					g.DrawSprite(GameData.SPR_PLAYER_FORWARD, tilePoint);
				}
			}
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
			else {
				this.AutoScrollMinSize = Point2I.One;
				this.HorizontalScroll.Value = 0;
				this.VerticalScroll.Value = 0;
			}
		}


		//-----------------------------------------------------------------------------
		// Form Events
		//-----------------------------------------------------------------------------

		private void OnMouseLeave(object sender, EventArgs e) {
			// Clear any highlighted tile/rooms.
			highlightedRoom		= -Point2I.One;
			highlightedTile		= -Point2I.One;
			cursorTileLocation	= -Point2I.One;
		}

		private void OnMouseDown(object sender, MouseEventArgs e) {
			Cursor = editorControl.CurrentTool.MouseCursor;

			if (editorControl.IsLevelOpen) {
				Point2I mousePos = ScrollPosition + e.Location;

				if (editorControl.PlayerPlaceMode) {
					// Test the world after placing the player.
					if (highlightedTile != -Point2I.One) {
						editorControl.TestWorld(highlightedRoom, highlightedTile);
						editorForm.ButtonTestLevelPlace.Checked = false;
					}
				}
				else {
					// Notify the current tool.
					e = new MouseEventArgs(e.Button, e.Clicks, mousePos.X, mousePos.Y, e.Delta);
					editorControl.CurrentTool.OnMouseDown(e);
				}
			}
			this.Focus();
		}
		
		private void OnMouseUp(object sender, MouseEventArgs e) {
			Cursor = editorControl.CurrentTool.MouseCursor;

			if (editorControl.IsLevelOpen) {
				Point2I mousePos = ScrollPosition + e.Location;

				// Notify the current tool.
				if (!editorControl.PlayerPlaceMode) {
					e = new MouseEventArgs(e.Button, e.Clicks, mousePos.X, mousePos.Y, e.Delta);
					editorControl.CurrentTool.OnMouseUp(e);
				}
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e) {
			Cursor = editorControl.CurrentTool.MouseCursor;

			if (editorControl.IsLevelOpen) {
				Point2I mousePos = ScrollPosition + e.Location;

				// Find the highlighted room/tile coordinates.
				cursorTileLocation	= SampleLevelTileCoordinates(mousePos);
				highlightedRoom		= SampleRoomCoordinates(mousePos, false);
				highlightedTile		= SampleTileCoordinates(mousePos);
				if (!(highlightedRoom < Level.Dimensions)) {
					highlightedRoom = -Point2I.One;
					highlightedTile = -Point2I.One;
				}

				// Notify the current tool.
				if (!editorControl.PlayerPlaceMode) {
					e = new MouseEventArgs(e.Button, e.Clicks, mousePos.X, mousePos.Y, e.Delta);
					editorControl.CurrentTool.OnMouseMove(e);
				}

				// Update the status bar for tile and room coordinates.
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

		protected override void Draw() {
			editorControl.UpdateTicks();
			
			Graphics2D g = new Graphics2D(spriteBatch);
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			DrawLevel(g);
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

		public Rectangle2I SelectionBox {
			get { return selectionBox; }
			set { selectionBox = value; }
		}
	}
}

