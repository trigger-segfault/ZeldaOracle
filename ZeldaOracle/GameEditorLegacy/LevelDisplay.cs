using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinForms;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;
using ZeldaEditor.Control;
using ZeldaOracle.Game.Tiles.Custom;
using ZeldaOracle.Game.Entities.Monsters;
using System.IO;
using System.Threading;
using ZeldaOracle.Common.Graphics.Sprites;

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
		private Room			selectedRoom;

		private HashSet<BaseTileDataInstance> selectedTiles;
		private TileGrid	selectionGrid;
		private Rectangle2I	selectionGridArea;
		private Level		selectionGridLevel;
		//private Point2I		selectionGridLocation;

		
		

		//-----------------------------------------------------------------------------
		// Selection Grid
		//-----------------------------------------------------------------------------

		public Rectangle2I SelectionGridArea {
			get { return selectionGridArea; }
		}

		public void SetSelectionGrid(TileGrid tileGrid, Point2I location, Level level) {
			PlaceSelectionGrid();
			selectionGridArea	= new Rectangle2I(location, tileGrid.Size);
			selectionGridLevel	= level;
			selectionGrid		= tileGrid;
			SetSelectionBox(selectionGridArea.Point * GameSettings.TILE_SIZE,
							selectionGridArea.Size  * GameSettings.TILE_SIZE);
		}

		public void SetSelectionGridArea(Rectangle2I area, Level level) {
			PlaceSelectionGrid();
			selectionGridArea  = area;
			selectionGridLevel = level;
			SetSelectionBox(selectionGridArea.Point * GameSettings.TILE_SIZE,
							selectionGridArea.Size  * GameSettings.TILE_SIZE);
		}

		public void MoveSelectionGridArea(Point2I newLocation) {
			if (newLocation != selectionGridArea.Point) {
				PickupSelectionGrid();
				selectionGridArea.Point = newLocation;
				SetSelectionBox(selectionGridArea.Point * GameSettings.TILE_SIZE,
								selectionGridArea.Size  * GameSettings.TILE_SIZE);
			}
		}

		public void PlaceSelectionGrid() {
			if (selectionGrid != null) {
				selectionGridLevel.PlaceTileGrid(
					selectionGrid, (LevelTileCoord) selectionGridArea.Point);
				selectionGrid = null;
				Console.WriteLine("Placed selection grid");
			}
		}

		public void PickupSelectionGrid() {
			if (selectionGrid == null && !selectionGridArea.IsEmpty) {
				selectionGrid = Level.CreateTileGrid(selectionGridArea);
				Console.WriteLine("Picked up selection grid");
			}
		}
		
		public void DuplicateSelectionGrid() {
			PickupSelectionGrid();
			if (selectionGrid != null) {
				TileGrid oldGrid = selectionGrid;
				selectionGrid = selectionGrid.Duplicate();
				selectionGridLevel.PlaceTileGrid(
					oldGrid, (LevelTileCoord) selectionGridArea.Point);
				Console.WriteLine("Duplicated Selection grid");
			}
		}
		
		public void DeleteSelectionGrid() {
			PickupSelectionGrid();
			if (selectionGrid != null) {
				selectionGrid = null;
			}
		}
		
		public void DeselectSelectionGrid() {
			PlaceSelectionGrid();
			selectionGridArea = Rectangle2I.Zero;
			selectionBox = Rectangle2I.Zero;
		}


		//-----------------------------------------------------------------------------
		// Individual Tile Selection
		//-----------------------------------------------------------------------------

		public bool IsTileInSelection(BaseTileDataInstance tile) {
			return selectedTiles.Contains(tile);
		}

		public void AddTileToSelection(BaseTileDataInstance tile) {
			selectedTiles.Add(tile);
			selectedRoom = tile.Room;
		}

		public void RemoveTileFromSelection(BaseTileDataInstance tile) {
			selectedTiles.Remove(tile);
		}

		public void DeselectTiles() {
			selectedTiles.Clear();
		}

		public void DeleteTileSelection() {
			foreach (BaseTileDataInstance tile in selectedTiles) {
				tile.Room.Remove(tile);
				editorControl.OnDeleteObject(tile);
			}
			selectedTiles.Clear();
		}

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		private int lastTicks = -1;

		Thread drawThread;

		protected override void Initialize() {
			content		= new ContentManager(Services, "Content");
			spriteBatch	= new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);
			
			selectedRoom = null;
			selectedTiles = new HashSet<BaseTileDataInstance>();
			selectionGrid = null;

			editorControl.Initialize(content, GraphicsDevice);

			//this.ContextMenuStrip = editorControl.EditorForm.ContextMenuStripTileInLevel;

			/*levelRenderTarget = new Microsoft.Xna.Framework.Graphics.RenderTarget2D(
				GraphicsDevice, 2048, 2048, false,
				Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color,
				Microsoft.Xna.Framework.Graphics.DepthFormat.None, 0,
				Microsoft.Xna.Framework.Graphics.RenderTargetUsage.PreserveContents);*/

			// Wire the events.
			MouseMove			+= OnMouseMove;
			MouseDown			+= OnMouseDown;
			MouseUp				+= OnMouseUp;
			MouseLeave			+= OnMouseLeave;
			MouseDoubleClick	+= OnMouseDoubleClick;
			
			this.ResizeRedraw = true;

			// Start the timer to refresh the panel.
			/*Application.Idle += delegate {
				int ticks = (int)((double)Stopwatch.GetTimestamp() / Stopwatch.Frequency * 60);
				//EditorControl.UpdateTicks();
				if (ticks != lastTicks) {
					lastTicks = ticks;
					Invalidate();
				}
			};*/
			EditorForm.Activated += delegate {
				try {
					drawThread.Abort();
				}
				catch { }
				//if (drawThread.ThreadState == System.Threading.ThreadState.Stopped || drawThread.ThreadState == System.Threading.ThreadState.Unstarted || drawThread.ThreadState == System.Threading.ThreadState.Aborted)
				drawThread = new Thread(() => {
					try {
						int ticks;
						while (true) {
							//Invalidate();
							//Thread.Sleep(15);
							ticks = (int)((double)Stopwatch.GetTimestamp() / Stopwatch.Frequency * 60);
							if (ticks != lastTicks) {
								lastTicks = ticks;
								Invalidate();
							}
							Thread.Sleep(2);
						}
					}
					catch { }
				});
				drawThread.Start();
			};
			EditorForm.Deactivate += delegate {
				//if (drawThread.ThreadState == System.Threading.ThreadState.Running)
					drawThread.Abort();
			};
			drawThread = new Thread(() => {
				try {
					int ticks;
					while (true) {
						//Invalidate();
						//Thread.Sleep(15);
						ticks = (int)((double)Stopwatch.GetTimestamp() / Stopwatch.Frequency * 60);
						if (ticks != lastTicks) {
							lastTicks = ticks;
							Invalidate();
						}
						Thread.Sleep(2);
					}
				}
				catch { }
			});

			// TEMP: Open this world file upon starting the editor.
			if (File.Exists("./temp_world.zwd"))
				editorControl.OpenFile("temp_world.zwd");
			else if (File.Exists("../../../../WorldFiles/temp_world.zwd"))
				editorControl.OpenFile("../../../../WorldFiles/temp_world.zwd");
			//editorControl.OpenFile("temp_world.zwd");

			this.highlightedRoom = -Point2I.One;
			this.highlightedTile = -Point2I.One;
			//drawThread.Start();

			UpdateLevel();
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

		// Sample the room coordinates at the given point, clamping them to the level's dimensions if specified.
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

		// Sample the room at the given point, clamping to the levels dimensions if specified.
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
		
		// Sample the tile coordinates at the given point (tile coordinates are absolute to the level).
		public Point2I SampleLevelPixelPosition(Point2I point) {
			Point2I roomCoord = SampleRoomCoordinates(point);
			Point2I pointInRoom = point - GetRoomDrawPosition(roomCoord);
			return ((roomCoord * Level.RoomSize * GameSettings.TILE_SIZE) + pointInRoom);
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
			Point2I roomLoc = GMath.Clamp(start / Level.RoomSize, Point2I.Zero, Level.Dimensions);
			selectedRoom = Level.GetRoomAt(roomLoc);
		}

		public void SetSelectionBox(BaseTileDataInstance tile) {
			selectionBox = tile.GetBounds();
			selectionBox.Point += tile.Room.Location * tile.Room.Size * GameSettings.TILE_SIZE;
		}
		

		public void SetSelectionBoxToSelectedTiles() {
			selectionBox = Rectangle2I.Zero;

			foreach (BaseTileDataInstance tile in selectedTiles) {
				Rectangle2I box = tile.GetBounds();
				box.Point += tile.Room.Location * tile.Room.Size * GameSettings.TILE_SIZE;

				if (selectionBox.IsEmpty)
					selectionBox = box;
				else
					selectionBox = Rectangle2I.Union(selectionBox, box);
			}
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

		// Get the top-left position to draw a tile with the given absolute coordinates.
		private Point2I GetLevelPixelDrawPosition(Point2I pixelInLevel) {
			Point2I roomCoord   = pixelInLevel / (Level.RoomSize * GameSettings.TILE_SIZE);
			Point2I pixelInRoom = pixelInLevel % (Level.RoomSize * GameSettings.TILE_SIZE);
			if (pixelInLevel.X < 0)
				roomCoord.X--;
			if (pixelInLevel.Y < 0)
				roomCoord.Y--;
			return (GetRoomDrawPosition(roomCoord) + pixelInRoom);
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------
		
		// Draw a tile.
		private void DrawTile(Graphics2D g, TileDataInstance tile, Point2I position, Color drawColor) {
			SpriteOld sprite = null;
			AnimationOld animation = null;
			float playbackTime = editorControl.Ticks;
			int substripIndex = tile.Properties.GetInteger("substrip_index", 0);
			
			//-----------------------------------------------------------------------------
			// Platform.
			if (tile.Type == typeof(TilePlatform)) {
				if (!tile.CurrentSprite.IsNull) {
					// Draw the tile once per point within its size.
					for (int y = 0; y < tile.Size.Y; y++) {
						for (int x = 0; x < tile.Size.X; x++) {
							Point2I drawPos = position +
								(new Point2I(x, y) * GameSettings.TILE_SIZE);
							g.DrawISprite(tile.CurrentSprite,
								new SpriteDrawSettings(tile.Room.Zone.ImageVariantID, editorControl.Ticks),
								position, drawColor);
						}
					}
				}
				return;
			}
			//-----------------------------------------------------------------------------
			// Color Jump Pad.
			else if (tile.Type == typeof(TileColorJumpPad)) {
				PuzzleColor tileColor = (PuzzleColor) tile.Properties.GetInteger("color", 0);
				if (tileColor == PuzzleColor.Red)
					sprite = GameData.SPR_TILE_COLOR_JUMP_PAD_RED;
				else if (tileColor == PuzzleColor.Yellow)
					sprite = GameData.SPR_TILE_COLOR_JUMP_PAD_YELLOW;
				else if (tileColor == PuzzleColor.Blue)
					sprite = GameData.SPR_TILE_COLOR_JUMP_PAD_BLUE;
			}
			//-----------------------------------------------------------------------------
			// Color Cube
			else if (tile.Type == typeof(TileColorCube)) {
				int orientationIndex = tile.Properties.GetInteger("orientation", 0);
				sprite = GameData.SPR_COLOR_CUBE_ORIENTATIONS[orientationIndex];
			}
			//-----------------------------------------------------------------------------
			// Crossing Gate.
			else if (tile.Type == typeof(TileCrossingGate)) {
				if (tile.Properties.GetBoolean("raised", false))
					animation = GameData.ANIM_TILE_CROSSING_GATE_LOWER;
				else
					animation = GameData.ANIM_TILE_CROSSING_GATE_RAISE;
				substripIndex = (tile.Properties.GetBoolean("face_left", false) ? 1 : 0);
				playbackTime = 0.0f;
			}
			//-----------------------------------------------------------------------------
			// Lantern.
			else if (tile.Type == typeof(TileLantern)) {
				if (tile.Properties.GetBoolean("lit", true))
					animation = GameData.ANIM_TILE_LANTERN;
				else
					sprite = GameData.SPR_TILE_LANTERN_UNLIT;
			}
			//-----------------------------------------------------------------------------
			// Chest.
			else if (tile.Type == typeof(TileChest)) {
				bool isLooted = tile.Properties.GetBoolean("looted", false);
				sprite = tile.SpriteList[isLooted ? 1 : 0].Sprite;
			}
			//-----------------------------------------------------------------------------
			// Color Lantern.
			/*else if (tile.Type == typeof(TileColorLantern)) {
				PuzzleColor color = (PuzzleColor) tile.Properties.GetInteger("color", -1);
				if (color == PuzzleColor.Red)
					animation = GameData.ANIM_EFFECT_COLOR_FLAME_RED;
				else if (color == PuzzleColor.Yellow)
					animation = GameData.ANIM_EFFECT_COLOR_FLAME_YELLOW;
				else if (color == PuzzleColor.Blue)
					animation = GameData.ANIM_EFFECT_COLOR_FLAME_BLUE;
			}*/
			//-----------------------------------------------------------------------------

			if (animation == null && sprite == null && tile.CurrentSprite.IsAnimation)
				animation = tile.CurrentSprite.Animation;
			if (animation == null && sprite == null && tile.CurrentSprite.IsSprite)
				sprite = tile.CurrentSprite.Sprite;

			// Draw the custom sprite/animation
			if (animation != null) {
				g.DrawAnimation(animation.GetSubstrip(substripIndex),
					tile.Room.Zone.ImageVariantID, playbackTime, position, drawColor);
			}
			else if (sprite != null) {
				g.DrawSprite(sprite, tile.Room.Zone.ImageVariantID, position, drawColor);
			}
			/*else if (!tile.CurrentSprite.IsNull) {
				g.DrawAnimation(tile.CurrentSprite,
					tile.Room.Zone.ImageVariantID, editorControl.Ticks, position, drawColor);
			}*/

			// Draw rewards.
			if (editorControl.ShowRewards && tile.Properties.Contains("reward") &&
				editorControl.RewardManager.HasReward(tile.Properties.GetString("reward")))
			{
				AnimationOld anim = editorControl.RewardManager.GetReward(tile.Properties.GetString("reward")).Sprite;
				g.DrawAnimation(anim, editorControl.Ticks, position, drawColor);
			}
		}

		// Draw an event tile.
		private void DrawEventTile(Graphics2D g, EventTileDataInstance eventTile, Point2I position, Color drawColor) {
			SpriteAnimation spr = eventTile.CurrentSprite;
			int imageVariantID = eventTile.Properties.GetInteger("image_variant");
			if (imageVariantID < 0)
				imageVariantID = eventTile.Room.Zone.ImageVariantID;
			
			// Select different sprites for certain events.
			if (eventTile.Type == typeof(NPCEvent)) {
				eventTile.SubStripIndex = eventTile.Properties.GetInteger("direction", 0);
			}
			else if (eventTile.Type == typeof(WarpEvent)) {
				string warpTypeStr = eventTile.Properties.GetString("warp_type", "tunnel");
				WarpType warpType = (WarpType) Enum.Parse(typeof(WarpType), warpTypeStr, true);
				if (warpType == WarpType.Entrance)
					spr = GameData.SPR_ACTION_TILE_WARP_ENTRANCE;
				else if (warpType == WarpType.Tunnel)
					spr = GameData.SPR_ACTION_TILE_WARP_TUNNEL;
				else if (warpType == WarpType.Stairs)
					spr = GameData.SPR_ACTION_TILE_WARP_STAIRS;
			}

			// Draw the sprite.
			if (!spr.IsNull) {
				g.DrawAnimation(spr, imageVariantID, editorControl.Ticks, position, drawColor);
			}
			else {
				Rectangle2I r = new Rectangle2I(position, eventTile.Size * GameSettings.TILE_SIZE);
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
				if (!editorControl.ShouldDrawEvents) {
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
						Point2I position = new Point2I(x, y) * GameSettings.TILE_SIZE;
						TileDataInstance tile = room.GetTile(x, y, i);
						
						// Draw tile.
						if (tile != null && tile.IsAtLocation(x, y))
							DrawTile(g, tile, position, color);

						// Draw grid square.
						if (i == room.LayerCount - 1) {
							if (editorControl.ShowGrid)
								g.DrawRectangle(new Rectangle2I(position, new Point2I(GameSettings.TILE_SIZE + 1)), 1, new Color(0, 0, 0, 150));
						}
					}
				}
			}

			// Draw event tiles.
			if (editorControl.ShowEvents || editorControl.ShouldDrawEvents) {
				for (int i = 0; i < room.EventData.Count; i++)
					DrawEventTile(g, room.EventData[i], room.EventData[i].Position, Color.White);
			}

			// Draw the spacing lines between rooms.
			if (editorControl.RoomSpacing > 0) {
				g.FillRectangle(new Rectangle2I(0, room.Height * GameSettings.TILE_SIZE, room.Width * GameSettings.TILE_SIZE, editorControl.RoomSpacing), Color.Black);
				g.FillRectangle(new Rectangle2I(room.Width * GameSettings.TILE_SIZE, 0, editorControl.RoomSpacing, room.Height * GameSettings.TILE_SIZE + editorControl.RoomSpacing), Color.Black);
			}
		}
		
		// Draw an entire level.
		public void DrawLevel(Graphics2D g) {
			g.Clear(new Color(175, 175, 180)); // Gray background.

			// Draw the level if it is open.
			if (editorControl.IsLevelOpen) {
				g.PushTranslation(-ScrollPosition);
				// Draw the rooms.
				for (int x = 0; x < Level.Width; x++) {
					for (int y = 0; y < Level.Height; y++) {
						Point2I roomPosition = GetRoomDrawPosition(new Point2I(x, y)) - new Point2I(HorizontalScroll.Value, VerticalScroll.Value);
						if (roomPosition + (Level.RoomSize * GameSettings.TILE_SIZE) >= Point2I.Zero && roomPosition < new Point2I(ClientSize.Width, ClientSize.Height)) {
							//g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
							g.PushTranslation((Vector2F)(new Point2I(x, y) * ((Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing)));
							DrawRoom(g, Level.GetRoomAt(x, y));
							g.PopTranslation();
						}
						/*g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
						g.Translate((Vector2F)(new Point2I(x, y) * ((Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing)));
						DrawRoom(g, Level.GetRoomAt(x, y));
						g.ResetTranslation();*/
					}
				}
				
				// Draw the highlight box.
				if (editorControl.HighlightMouseTile && cursorTileLocation >= Point2I.Zero) {
					Rectangle2I box = new Rectangle2I(GetLevelTileCoordDrawPosition(cursorTileLocation), new Point2I(16, 16));
					g.DrawRectangle(box.Inflated(1, 1), 1, Color.White);
				}

				// Draw selection grid.
				if (selectionGridLevel == Level && !selectionGridArea.IsEmpty) {
					if (selectionGrid != null) {
						for (int i = 0; i < selectionGrid.LayerCount; i++) {
							for (int y = 0; y < selectionGrid.Height; y++) {
								for (int x = 0; x < selectionGrid.Width; x++) {
									Point2I position = GetLevelTileCoordDrawPosition(selectionGridArea.Point + new Point2I(x, y));
									TileDataInstance tile = selectionGrid.GetTileIfAtLocation(x, y, i);
						
									// Draw tile.
									if (tile != null)
										DrawTile(g, tile, position, Color.White);
								}
							}
						}
						// Draw event tiles.
						if (editorControl.ShowEvents || editorControl.ShouldDrawEvents) {
							foreach (EventTileDataInstance eventTile in selectionGrid.GetEventTiles()) {
								Point2I position = GetLevelPixelDrawPosition(selectionGridArea.Point * GameSettings.TILE_SIZE + eventTile.Position);
								DrawEventTile(g, eventTile, position, Color.White);
							}
							/*for (int i = 0; i < selectionGrid.EventTiles.Count; i++) {
								Point2I position = GetLevelPixelDrawPosition(selectionGridArea.Point * GameSettings.TILE_SIZE + selectionGrid.EventTiles[i].Position);
								DrawEventTile(g, selectionGrid.EventTiles[i], position, Color.White);
							}*/
						}
					}
					
					// Draw the selection box.
					if (!selectionBox.IsEmpty) {
						Point2I start = GetLevelPixelDrawPosition(selectionBox.TopLeft);
						Point2I end   = GetLevelPixelDrawPosition(selectionBox.BottomRight);
						Rectangle2I box = new Rectangle2I(start, end - start);

						g.DrawRectangle(box, 1, Color.White);
						g.DrawRectangle(box.Inflated(1, 1), 1, Color.Black);
						g.DrawRectangle(box.Inflated(-1, -1), 1, Color.Black);
					}
				}

				// Draw selected tiles.
				foreach (BaseTileDataInstance tile in selectedTiles) {
					Rectangle2I bounds = tile.GetBounds();
					bounds.Point += GetRoomDrawPosition(tile.Room);
					g.DrawRectangle(bounds, 1, Color.White);
					g.DrawRectangle(bounds.Inflated(1, 1), 1, Color.Black);
					g.DrawRectangle(bounds.Inflated(-1, -1), 1, Color.Black);
				}

				// Draw player sprite for 'Test At Position'
				Point2I roomSize = (Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing;
				Point2I tilePoint = highlightedRoom * roomSize + highlightedTile * GameSettings.TILE_SIZE;
				if (editorControl.PlayerPlaceMode && highlightedTile >= Point2I.Zero) {
					g.DrawSprite(GameData.SPR_PLAYER_FORWARD, (Vector2F) tilePoint);
				}
				
				g.PopTranslation();
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

		private void OnMouseDoubleClick(object sender, MouseEventArgs e) {
			if (editorControl.IsLevelOpen) {
				Point2I mousePos = ScrollPosition + e.Location;

				// Notify the current tool.
				if (!editorControl.PlayerPlaceMode) {
					e = new MouseEventArgs(e.Button, e.Clicks, mousePos.X, mousePos.Y, e.Delta);
					editorControl.CurrentTool.OnMouseDoubleClick(e);
				}
			}
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

			/*
			Graphics2D g = new Graphics2D(spriteBatch);
			g.SetRenderTarget(levelRenderTarget);
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			DrawLevel(g);
			g.End();
			
			g.SetRenderTarget(null);
			
			spriteBatch.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Red);

			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			g.DrawImage(levelRenderTarget, Point2I.Zero);
			g.End();*/
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			/*
			Draw();
			EndDraw();
			
			Microsoft.Xna.Framework.Rectangle sourceRectangle =
				new Microsoft.Xna.Framework.Rectangle(0, 0, Math.Max(0, ClientSize.Width),
															Math.Max(0, ClientSize.Height));
			GraphicsDevice.Present(sourceRectangle, null, this.Handle);
			*/

			/*
			// Draw selection boxes.
			System.Drawing.Graphics g = e.Graphics;

			//g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
					
			Point2I start = GetLevelPixelDrawPosition(selectionBox.TopLeft);
			Point2I end   = GetLevelPixelDrawPosition(selectionBox.BottomRight);
			Rectangle2I box = new Rectangle2I(start, end - start);
			
			//g.Clear(System.Drawing.Color.White);

			g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red),
				box.X, box.Y, box.Width, box.Height);
			*/
			//Point2I start = GetLevelTileCoordDrawPosition(selectionBox.TopLeft);
			//Point2I end   = GetLevelTileCoordDrawPosition(selectionBox.BottomRight);
			//Rectangle2I box = new Rectangle2I(start, end - start);
			/*g.DrawRectangle(box, 1, Color.White);
			g.DrawRectangle(box.Inflated(1, 1), 1, Color.Black);
			g.DrawRectangle(box.Inflated(-1, -1), 1, Color.Black);
			g.ResetTranslation();*/
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
			set {
				selectionBox = value;
				Point2I roomLoc = GMath.Clamp(value.Point / Level.RoomSize, Point2I.Zero, Level.Dimensions);
				selectedRoom = Level.GetRoomAt(roomLoc);
			}
		}

		public Room SelectedRoom {
			get { return selectedRoom; }
		}

		public HashSet<BaseTileDataInstance> SelectedTiles {
			get { return selectedTiles; }
		}

		public TileGrid SelectionGrid {
			get { return selectionGrid; }
			set { selectionGrid = value; }
		}

		public Level SelectionGridLevel {
			get { return selectionGridLevel; }
		}
	}
}

