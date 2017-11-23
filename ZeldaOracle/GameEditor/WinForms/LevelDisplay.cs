using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
using System.Windows.Threading;
using MinecartTrackOrientation = ZeldaAPI.MinecartTrackOrientation;
using ZeldaEditor.Tools;

namespace ZeldaEditor.WinForms {

	public class LevelDisplay : GraphicsDeviceControl {

		private static ContentManager content;
		private static Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;

		public readonly Color NormalColor = Color.White;
		public readonly Color FadeAboveColor = new Color(200, 200, 200, 100);
		public readonly Color FadeBelowColor = new Color(200, 200, 200, 150);
		public readonly Color HideColor = Color.Transparent;

		private EditorWindow	editorWindow;
		private EditorControl	editorControl;
		private Point2I			highlightedRoom;
		private Point2I			highlightedTile;
		private Point2I         cursorPixelLocation;
		private Point2I			cursorHalfTileLocation;
		private Point2I         cursorTileSize;
		private Rectangle2I		selectionBox;
		private Room			selectedRoom;
		private bool            isSelectionRoom;

		private HashSet<BaseTileDataInstance> selectedTiles;
		private TileGrid	selectionGrid;
		private Rectangle2I	selectionGridArea;
		private Level		selectionGridLevel;
		//private Point2I		selectionGridLocation;
		private DispatcherTimer dispatcherTimer;


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
				editorControl.IsModified = true;
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
				selectionGridArea = Rectangle2I.Zero;
				selectionBox = Rectangle2I.Zero;
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
			isSelectionRoom = false;
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
		
		protected override void Initialize() {
			try {
				content     = new ContentManager(Services, "Content");
				spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);

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
				MouseEnter          += OnMouseEnter;
				MouseMove           += OnMouseMove;
				MouseDown           += OnMouseDown;
				MouseUp             += OnMouseUp;
				MouseLeave          += OnMouseLeave;
				MouseDoubleClick    += OnMouseDoubleClick;

				this.ResizeRedraw = true;

				// TEMP: Open this world file upon starting the editor.
				if (File.Exists("./temp_world.zwd"))
					editorControl.OpenFile("temp_world.zwd");
				else if (File.Exists("../../../../WorldFiles/temp_world.zwd"))
					editorControl.OpenFile("../../../../WorldFiles/temp_world.zwd");
				else if (File.Exists("../../../WorldFiles/temp_world.zwd"))
					editorControl.OpenFile("../../../WorldFiles/temp_world.zwd");
				//editorControl.OpenFile("temp_world.zwd");

				UpdateLevel();

				this.highlightedRoom = -Point2I.One;
				this.highlightedTile = -Point2I.One;

				dispatcherTimer = new DispatcherTimer(
					TimeSpan.FromMilliseconds(15),
					DispatcherPriority.Render,
					delegate { Invalidate(); },
					System.Windows.Application.Current.Dispatcher);
				/*drawThread = new Thread(() => {
					try {
						int ticks, lastTicks = 0;
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
				});*/
				//drawThread.Start();
				FrameWatch.Start();
				/*System.Windows.Media.CompositionTarget.Rendering += delegate {
					Console.WriteLine("Frame Time: " + FrameWatch.ElapsedMilliseconds);
					FrameWatch.Restart();
				};*/
			}
			catch (Exception e) {
				throw e;
			}
		}

		Stopwatch FrameWatch = new Stopwatch();

		protected override void Dispose(bool disposing) {
			//drawThread.Abort();
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

		// Sample the half-tile coordinates at the given point (tile coordinates are relative to their room).
		public Point2I SampleHalfTileCoordinates(Point2I point) {
			Point2I span = (Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing;
			Point2I begin = SampleRoomCoordinates(point) * span;
			Point2I tileCoord = (point - begin) / (GameSettings.TILE_SIZE / 2);
			return GMath.Clamp(tileCoord, Point2I.Zero, Level.RoomSize * 2 - Point2I.One);
		}

		// Sample the tile coordinates at the given point (tile coordinates are absolute to the level).
		public Point2I SampleLevelTileCoordinates(Point2I point) {
			Point2I roomCoord = SampleRoomCoordinates(point);
			Point2I tileCoord = SampleTileCoordinates(point);
			return ((roomCoord * Level.RoomSize) + tileCoord);
		}

		// Sample the half-tile coordinates at the given point (tile coordinates are absolute to the level).
		public Point2I SampleLevelHalfTileCoordinates(Point2I point) {
			Point2I roomCoord = SampleRoomCoordinates(point);
			Point2I halfTileCoord = SampleHalfTileCoordinates(point);
			return ((roomCoord * Level.RoomSize * 2) + halfTileCoord);
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

		// Convert level tile coordinates room half-tile coordinates.
		public Point2I ToRoomHalfTileCoordinates(Point2I levelTileLocation) {
			return GMath.Wrap(levelTileLocation, Level.RoomSize * 2);
		}


		//-----------------------------------------------------------------------------
		// Selection Box
		//-----------------------------------------------------------------------------

		// Clear the selection box by setting it to zero.
		public void ClearSelectionBox() {
			selectionBox = Rectangle2I.Zero;
		}

		// Set the selection box
		public void SetSelectionBox(Rectangle2I box) {
			selectionBox = box;
			Point2I roomLoc = GMath.Clamp(box.Point / Level.RoomSize, Point2I.Zero, Level.Dimensions);
			selectedRoom = Level.GetRoomAt(roomLoc);
			isSelectionRoom = false;
		}

		// Set the selection box
		public void SetSelectionBox(Point2I start, Point2I size) {
			selectionBox = new Rectangle2I(start, size);
			Point2I roomLoc = GMath.Clamp(start / Level.RoomSize, Point2I.Zero, Level.Dimensions);
			selectedRoom = Level.GetRoomAt(roomLoc);
			isSelectionRoom = false;
		}

		public void SetSelectionBox(BaseTileDataInstance tile) {
			selectionBox = tile.GetBounds();
			selectionBox.Point += tile.Room.Location * tile.Room.Size * GameSettings.TILE_SIZE;
			isSelectionRoom = false;
		}

		public void SelectRoom(Room room) {
			isSelectionRoom = true;
			selectedRoom = room;
			DeselectTiles();
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
			isSelectionRoom = false;
		}


		//-----------------------------------------------------------------------------
		// Drawing Helper Functions
		//-----------------------------------------------------------------------------
		
		// Get the top-left position to draw the given room.
		public Point2I GetRoomDrawPosition(Room room) {
			return GetRoomDrawPosition(room.Location);
		}

		// Get the top-left position to draw the room of the given coordinates.
		public Point2I GetRoomDrawPosition(Point2I roomCoord) {
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
		private Point2I GetLevelHalfTileCoordDrawPosition(Point2I levelHalfTileCoord) {
			Point2I roomCoord = levelHalfTileCoord / (Level.RoomSize * 2);
			if (levelHalfTileCoord.X < 0)
				roomCoord.X--;
			if (levelHalfTileCoord.Y < 0)
				roomCoord.Y--;
			Point2I halfTileCoord = ToRoomHalfTileCoordinates(levelHalfTileCoord);
			return (GetRoomDrawPosition(roomCoord) + (halfTileCoord * GameSettings.TILE_SIZE / 2));
		}

		// Get the top-left position to draw a tile with the given absolute coordinates.
		public Point2I GetLevelPixelDrawPosition(Point2I pixelInLevel) {
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
		public void DrawTile(Graphics2D g, Room room, TileDataInstance tile, Point2I position, Color drawColor) {
			if (editorControl.ShowModified && !tile.HasModifiedProperties)
				return;

			Sprite sprite = null;
			Animation animation = null;
			float playbackTime = editorControl.Ticks;
			int substripIndex =  tile.Properties.GetInteger("substrip_index", 0);
			
			//-----------------------------------------------------------------------------
			// Platform.
			if (tile.Type == typeof(TilePlatform)) {
				SpriteAnimation currentSprite = tile.CurrentSprite;
				if (!currentSprite.IsNull) {
					// Draw the tile once per point within its size.
					for (int y = 0; y < tile.Size.Y; y++) {
						for (int x = 0; x < tile.Size.X; x++) {
							Point2I drawPos = position +
								(new Point2I(x, y) * GameSettings.TILE_SIZE);
							g.DrawAnimation(currentSprite,
								room.Zone.ImageVariantID,
								editorControl.Ticks, drawPos, drawColor);
						}
					}
				}
				return;
			}
			//-----------------------------------------------------------------------------
			// Color Jump Pad.
			else if (tile.Type == typeof(TileColorJumpPad)) {
				PuzzleColor tileColor = (PuzzleColor)tile.Properties.GetInteger("color", 0);
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
			// Pull Handle.
			else if (tile.Type == typeof(TilePullHandle)) {
				int direction = tile.Properties.GetInteger("direction", Directions.Down);
				if (direction == Directions.Right)
					sprite = GameData.SPR_TILE_PULL_HANDLE_RIGHT;
				else if (direction == Directions.Up)
					sprite = GameData.SPR_TILE_PULL_HANDLE_UP;
				else if (direction == Directions.Left)
					sprite = GameData.SPR_TILE_PULL_HANDLE_LEFT;
				else if (direction == Directions.Down)
					sprite = GameData.SPR_TILE_PULL_HANDLE_DOWN;
			}
			//-----------------------------------------------------------------------------
			// Minecart Track.
			else if (tile.Type == typeof(TileMinecartTrack)) {
				MinecartTrackOrientation orientation = (MinecartTrackOrientation)tile.Properties.GetInteger("track_orientation", 0);
				switch (orientation) {
				case MinecartTrackOrientation.Horizontal: sprite = GameData.SPR_TILE_MINECART_TRACK_HORIZONTAL; break;
				case MinecartTrackOrientation.Vertical: sprite = GameData.SPR_TILE_MINECART_TRACK_VERTICAL; break;
				case MinecartTrackOrientation.UpRight: sprite = GameData.SPR_TILE_MINECART_TRACK_UP_RIGHT; break;
				case MinecartTrackOrientation.UpLeft: sprite = GameData.SPR_TILE_MINECART_TRACK_UP_LEFT; break;
				case MinecartTrackOrientation.DownLeft: sprite = GameData.SPR_TILE_MINECART_TRACK_DOWN_LEFT; break;
				case MinecartTrackOrientation.DownRight: sprite = GameData.SPR_TILE_MINECART_TRACK_DOWN_RIGHT; break;
				}
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

			if (animation == null && sprite == null) {
				SpriteAnimation currentSprite = tile.CurrentSprite;
				if (currentSprite.IsAnimation)
					animation = currentSprite.Animation;
				else if (currentSprite.IsSprite)
					sprite = currentSprite.Sprite;
			}
			/*if (animation == null && sprite == null && tile.CurrentSprite.IsAnimation)
				animation = tile.CurrentSprite.Animation;
			if (animation == null && sprite == null && tile.CurrentSprite.IsSprite)
				sprite = tile.CurrentSprite.Sprite;*/

			// Draw the custom sprite/animation
			if (animation != null) {
				g.DrawAnimation(animation.GetSubstrip(substripIndex),
					room.Zone.ImageVariantID, playbackTime, position, drawColor);
			}
			else if (sprite != null) {
				g.DrawSprite(sprite, room.Zone.ImageVariantID, position, drawColor);
			}


			//-----------------------------------------------------------------------------
			// Turnstile arrows.
			if (tile.Type == typeof(TileTurnstile)) {
				bool clockwise = tile.Properties.GetBoolean("clockwise", false);
				Animation arrowAnimation, turnstileAnimation;
				if (clockwise) {
					arrowAnimation = GameData.ANIM_TURNSTILE_ARROWS_CLOCKWISE;
					turnstileAnimation = GameData.ANIM_TURNSTILE_ROTATE_CLOCKWISE;
				}
				else {
					arrowAnimation = GameData.ANIM_TURNSTILE_ARROWS_COUNTERCLOCKWISE;
					turnstileAnimation = GameData.ANIM_TURNSTILE_ROTATE_COUNTERCLOCKWISE;
				}
				g.DrawAnimation(arrowAnimation.GetSubstrip(substripIndex),
					room.Zone.ImageVariantID, playbackTime, position, drawColor);
				g.DrawAnimation(turnstileAnimation.GetSubstrip(clockwise ? 0 : 1),
					room.Zone.ImageVariantID, 16, position, drawColor);
			}
			//-----------------------------------------------------------------------------

			/*else if (!tile.CurrentSprite.IsNull) {
				g.DrawAnimation(tile.CurrentSprite,
					room.Zone.ImageVariantID, editorControl.Ticks, position, drawColor);
			}*/

			// Draw rewards.
			if (editorControl.ShowRewards && tile.Properties.Exists("reward") &&
				editorControl.RewardManager.HasReward(tile.Properties.GetString("reward")))
			{
				Animation anim = editorControl.RewardManager.GetReward(tile.Properties.GetString("reward")).Animation;
				g.DrawAnimation(anim, editorControl.Ticks, position, drawColor);
			}
		}

		// Draw an event tile.
		public void DrawEventTile(Graphics2D g, Room room, EventTileDataInstance eventTile, Point2I position, Color drawColor) {
			if (editorControl.ShowModified && !eventTile.HasModifiedProperties)
				return;

			SpriteAnimation spr = eventTile.CurrentSprite;
			int imageVariantID = eventTile.Properties.GetInteger("image_variant");
			if (imageVariantID < 0)
				imageVariantID = room.Zone.ImageVariantID;
			
			// Select different sprites for certain events.
			if (eventTile.Type == typeof(NPCEvent)) {
				eventTile.SubStripIndex = eventTile.Properties.GetInteger("direction", 0);
			}
			else if (eventTile.Type == typeof(WarpEvent)) {
				string warpTypeStr = eventTile.Properties.GetString("warp_type", "tunnel");
				WarpType warpType = (WarpType) Enum.Parse(typeof(WarpType), warpTypeStr, true);
				if (warpType == WarpType.Entrance)
					spr = GameData.SPR_EVENT_TILE_WARP_ENTRANCE;
				else if (warpType == WarpType.Tunnel)
					spr = GameData.SPR_EVENT_TILE_WARP_TUNNEL;
				else if (warpType == WarpType.Stairs)
					spr = GameData.SPR_EVENT_TILE_WARP_STAIRS;
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
			//Color belowFade = new Color(150, 150, 150, 150);
			//Color aboveFade = new Color(100, 100, 100, 100);
			//Color hide = Color.Transparent;
			//Color normal = Color.White;
			Point2I roomStartTile = room.Location * Level.RoomSize;
			Point2I roomStartPixel = roomStartTile * GameSettings.TILE_SIZE;

			// Draw white background.
			g.FillRectangle(new Rectangle2I(Point2I.Zero, room.Size * GameSettings.TILE_SIZE), Color.White);

			// Draw tile layers.
			for (int layer = 0; layer < room.LayerCount; layer++) {
				// Determine color/transparency for layer based on layer visibility.
				Color color = NormalColor;
				if (!editorControl.EventMode) {
					if (editorControl.CurrentLayer > layer) {
						if (editorControl.BelowTileDrawMode == TileDrawModes.Hide)
							continue; //color = HideColor;
						else if (editorControl.BelowTileDrawMode == TileDrawModes.Fade)
							color = FadeBelowColor;
					}
					else if (editorControl.CurrentLayer < layer) {
						if (editorControl.AboveTileDrawMode == TileDrawModes.Hide)
							continue; //color = HideColor;
						else if (editorControl.AboveTileDrawMode == TileDrawModes.Fade)
							color = FadeAboveColor;
					}
				}
				
				// Draw the tile grid for this layer.
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						TileDataInstance tile = room.GetTile(x, y, layer);
						Point2I position = new Point2I(x, y) * GameSettings.TILE_SIZE;
						Point2I levelCoord = roomStartTile + new Point2I(x, y);
						if (tile != null && tile.IsAtLocation(x, y) && !CurrentTool.DrawHideTile(tile, room, levelCoord, layer)) {
							DrawTile(g, room, tile, position, color);
						}

						CurrentTool.DrawTile(g, room, position, levelCoord, layer);
					}
				}
			}

			// Draw event tiles.
			if (editorControl.ShowEvents || editorControl.ShouldDrawEvents) {
				for (int i = 0; i < room.EventData.Count; i++) {
					if (!CurrentTool.DrawHideEventTile(room.EventData[i], room, roomStartPixel + room.EventData[i].Position)) {
						DrawEventTile(g, room, room.EventData[i], room.EventData[i].Position, Color.White);
					}
				}
			}
		}
		
		// Draw an entire level.
		public void DrawLevel(Graphics2D g) {
			g.Clear(new Color(175, 175, 180)); // Gray background.

			// Draw the level if it is open.
			if (editorControl.IsLevelOpen) {
				// Draw the rooms.
				for (int x = 0; x < Level.Width; x++) {
					for (int y = 0; y < Level.Height; y++) {
						Point2I roomPosition = GetRoomDrawPosition(new Point2I(x, y)) - new Point2I(HorizontalScroll.Value, VerticalScroll.Value);
						if (roomPosition + (Level.RoomSize * GameSettings.TILE_SIZE) >= 0 && roomPosition < new Point2I(ClientSize.Width, ClientSize.Height)) {
							g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
							g.Translate((Vector2F)(new Point2I(x, y) * ((Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing)));
							DrawRoom(g, Level.GetRoomAt(x, y));
							g.ResetTranslation();
						}
						/*g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
						g.Translate((Vector2F)(new Point2I(x, y) * ((Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing)));
						DrawRoom(g, Level.GetRoomAt(x, y));
						g.ResetTranslation();*/
					}
				}

				CurrentTool.DrawEventTiles(g);
				/*TileDrawModes overrideDrawMode;
				IEnumerable<KeyValuePair<Point2I, EventTileDataInstance>> toolEvents = CurrentTool.DrawEventTiles(out overrideDrawMode);

				if (toolEvents != null && overrideDrawMode != TileDrawModes.DontOverride && overrideDrawMode != TileDrawModes.Hide) {
					Color color = NormalColor;
					if (overrideDrawMode == TileDrawModes.Fade)
						color = FadeBelowColor;

					foreach (var eventPair in toolEvents) {
						Point2I drawPosition = GetLevelPixelDrawPosition(eventPair.Value.Position);
						Room room = SampleRoom(drawPosition, true);
						DrawEventTile(g, room, eventPair.Value, eventPair.Value.Position, color);
					}
				}*/

				// Draw selection grid.
				if (selectionGridLevel == Level && !selectionGridArea.IsEmpty) {
					g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));

					if (selectionGrid != null) {
						Rectangle2I clientBounds = Rectangle2I.Zero;
						bool inView = false;
						for (int x = 0; x < selectionGrid.Width; x++) {
							int drawX = GetLevelTileCoordDrawPosition(new Point2I(selectionGridArea.X + x, 0)).X - HorizontalScroll.Value;
							if (drawX + GameSettings.TILE_SIZE >= 0) {
								clientBounds.X = x;
								inView = true;
								break;
							}
							else if (drawX >= ClientSize.Width) {
								break;
							}
						}
						if (inView) {
							inView = false;
							for (int y = 0; y < selectionGrid.Height; y++) {
								int drawY = GetLevelTileCoordDrawPosition(new Point2I(0, selectionGridArea.Y + y)).Y - VerticalScroll.Value;
								if (drawY + GameSettings.TILE_SIZE >= 0) {
									clientBounds.Y = y;
									inView = true;
									break;
								}
								else if (drawY >= ClientSize.Height) {
									break;
								}
							}
						}
						if (inView) {
							for (int x = selectionGrid.Width - 1; x >= 0; x--) {
								int drawX = GetLevelTileCoordDrawPosition(new Point2I(selectionGridArea.X + x, 0)).X - HorizontalScroll.Value;
								if (drawX < ClientSize.Width) {
									clientBounds.Width = x - clientBounds.X + 1;
									break;
								}
							}
							for (int y = selectionGrid.Height - 1; y >= 0; y--) {
								int drawY = GetLevelTileCoordDrawPosition(new Point2I(0, selectionGridArea.Y + y)).Y - VerticalScroll.Value;
								if (drawY < ClientSize.Height) {
									clientBounds.Height = y - clientBounds.Y + 1;
									break;
								}
							}
							Point2I start = GetLevelTileCoordDrawPosition(selectionGridArea.Point + clientBounds.TopLeft);
							Point2I end = GetLevelTileCoordDrawPosition(selectionGridArea.Point + clientBounds.BottomRight);
							g.FillRectangle(new Rectangle2I(start, end - start), Color.White);
							for (int i = 0; i < selectionGrid.LayerCount; i++) {
								for (int y = clientBounds.Top; y < clientBounds.Bottom; y++) {
									for (int x = clientBounds.Left; x < clientBounds.Right; x++) {
										Point2I position = GetLevelTileCoordDrawPosition(selectionGridArea.Point + new Point2I(x, y));
										TileDataInstance tile = selectionGrid.GetTileIfAtLocation(x, y, i);

										// Draw tile.
										if (tile != null)
											DrawTile(g, tile.Room, tile, position, Color.White);
									}
								}
							}
						}
						// Draw event tiles.
						if (editorControl.ShowEvents || editorControl.ShouldDrawEvents) {
							foreach (EventTileDataInstance eventTile in selectionGrid.GetEventTiles()) {
								Point2I position = GetLevelPixelDrawPosition(selectionGridArea.Point * GameSettings.TILE_SIZE + eventTile.Position);
								DrawEventTile(g, eventTile.Room, eventTile, position, Color.White);
							}
							/*for (int i = 0; i < selectionGrid.EventTiles.Count; i++) {
								Point2I position = GetLevelPixelDrawPosition(selectionGridArea.Point * GameSettings.TILE_SIZE + selectionGrid.EventTiles[i].Position);
								DrawEventTile(g, selectionGrid.EventTiles[i], position, Color.White);
							}*/
						}
					}

					g.ResetTranslation();
				}
				
				g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
				Point2I span = Level.Span;
				Point2I drawSpan = GetRoomDrawPosition(Level.Dimensions);
				// Draw the tile grid.
				if (editorControl.ShowGrid) {
					for (int x = 0; x < span.X; x++) {
						int drawX = GetLevelTileCoordDrawPosition(new Point2I(x, 0)).X;
						if (drawX >= HorizontalScroll.Value || drawX < HorizontalScroll.Value + ClientSize.Width) {
							g.FillRectangle(new Rectangle2F(
								drawX, VerticalScroll.Value,
								1, Math.Min(drawSpan.Y, ClientSize.Height)),
								Color.Black);
						}
					}
					for (int y = 0; y < span.Y; y++) {
						int drawY = GetLevelTileCoordDrawPosition(new Point2I(0, y)).Y;
						if (drawY >= VerticalScroll.Value || drawY < VerticalScroll.Value + ClientSize.Height) {
							g.FillRectangle(new Rectangle2F(
								HorizontalScroll.Value, drawY,
								Math.Min(drawSpan.X, ClientSize.Width), 1),
								Color.Black);
						}
					}
				}

				// Draw the room spacing.
				if (editorControl.RoomSpacing > 0) {
					for (int x = 0; x < Level.Width; x++) {
						int drawX = GetRoomDrawPosition(new Point2I(x + 1, 0)).X - editorControl.RoomSpacing;
						if (drawX >= HorizontalScroll.Value || drawX < HorizontalScroll.Value + ClientSize.Width) {
							g.FillRectangle(new Rectangle2F(
								drawX, VerticalScroll.Value,
								editorControl.RoomSpacing, Math.Min(drawSpan.Y, ClientSize.Height)),
								Color.Black);
						}
					}
					for (int y = 0; y < Level.Height; y++) {
						int drawY = GetRoomDrawPosition(new Point2I(0, y + 1)).Y - editorControl.RoomSpacing;
						if (drawY >= VerticalScroll.Value || drawY < VerticalScroll.Value + ClientSize.Height) {
							g.FillRectangle(new Rectangle2F(
								HorizontalScroll.Value, drawY,
								Math.Min(drawSpan.X, ClientSize.Width), editorControl.RoomSpacing),
								Color.Black);
						}
					}
				}
				g.ResetTranslation();


				//if (selectionGridLevel == Level && !selectionGridArea.IsEmpty) {
					g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
					
					// Draw the selection box.
					if (!selectionBox.IsEmpty) {
						Point2I start = GetLevelPixelDrawPosition(selectionBox.TopLeft);
						Point2I end   = GetLevelPixelDrawPosition(selectionBox.BottomRight);
						Rectangle2I box = new Rectangle2I(start, end - start);

						g.DrawRectangle(box, 1, ZeldaOracle.Common.Graphics.Color.White);
						g.DrawRectangle(box.Inflated(1, 1), 1, ZeldaOracle.Common.Graphics.Color.Black);
						g.DrawRectangle(box.Inflated(-1, -1), 1, ZeldaOracle.Common.Graphics.Color.Black);
					}

					g.ResetTranslation();
				//}

				// Draw the highlight box.
				if (editorControl.HighlightMouseTile && cursorHalfTileLocation >= Point2I.Zero) {
					g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
					Rectangle2I box = new Rectangle2I(GetLevelHalfTileCoordDrawPosition(cursorHalfTileLocation), cursorTileSize * 16);
					g.DrawRectangle(box.Inflated(1, 1), 1, ZeldaOracle.Common.Graphics.Color.White);
					g.ResetTranslation();
				}

				// Draw selected tiles.
				foreach (BaseTileDataInstance tile in selectedTiles) {
					g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
					Rectangle2I bounds = tile.GetBounds();
					bounds.Point += GetRoomDrawPosition(tile.Room);
					g.DrawRectangle(bounds, 1, ZeldaOracle.Common.Graphics.Color.White);
					g.DrawRectangle(bounds.Inflated(1, 1), 1, ZeldaOracle.Common.Graphics.Color.Black);
					g.DrawRectangle(bounds.Inflated(-1, -1), 1, ZeldaOracle.Common.Graphics.Color.Black);
					g.ResetTranslation();
				}

				if (isSelectionRoom) {
					g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
					Rectangle2I bounds = new Rectangle2I(
						GetRoomDrawPosition(selectedRoom),
						Level.RoomSize * GameSettings.TILE_SIZE);
					g.DrawRectangle(bounds, 1, ZeldaOracle.Common.Graphics.Color.White);
					g.DrawRectangle(bounds.Inflated(1, 1), 1, ZeldaOracle.Common.Graphics.Color.Black);
					g.DrawRectangle(bounds.Inflated(-1, -1), 1, ZeldaOracle.Common.Graphics.Color.Black);
					g.ResetTranslation();
				}

				// Draw player sprite for 'Test At Position'
				Point2I roomSize = (Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing;
				Point2I tilePoint = highlightedRoom * roomSize + highlightedTile * GameSettings.TILE_SIZE;
				if (editorControl.PlayerPlaceMode && highlightedTile >= Point2I.Zero) {
					g.DrawSprite(GameData.SPR_PLAYER_FORWARD, tilePoint + new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
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


		private void OnMouseEnter(object sender, EventArgs e) {
			Focus();
		}

		private void OnMouseLeave(object sender, EventArgs e) {
			// Clear any highlighted tile/rooms.
			highlightedRoom = new Point2I(-1000);
			highlightedTile = new Point2I(-2000);
			cursorHalfTileLocation	= new Point2I(-4000);
			cursorPixelLocation = new Point2I(-32000);
			editorWindow.StatusBarLabelRoomLoc.Content = "Room (?, ?)";
			editorWindow.StatusBarLabelTileLoc.Content = "Tile (?, ?)";
		}

		private void OnMouseDown(object sender, MouseEventArgs e) {
			Cursor = editorControl.CurrentTool.MouseCursor;

			if (editorControl.IsLevelOpen) {
				Point2I mousePos = ScrollPosition + e.Location;

				if (editorControl.PlayerPlaceMode) {
					// Test the world after placing the player.
					if (highlightedTile != -Point2I.One) {
						editorControl.TestWorld(highlightedRoom, highlightedTile);
					}
				}
				else {
					// Notify the current tool.
					e = new MouseEventArgs(e.Button, e.Clicks, mousePos.X, mousePos.Y, e.Delta);
					CurrentTool.MouseDown(e);
				}
			}
			this.Focus();
		}
		
		private void OnMouseUp(object sender, MouseEventArgs e) {
			Cursor = CurrentTool.MouseCursor;

			if (editorControl.IsLevelOpen) {
				Point2I mousePos = ScrollPosition + e.Location;

				// Notify the current tool.
				if (!editorControl.PlayerPlaceMode) {
					e = new MouseEventArgs(e.Button, e.Clicks, mousePos.X, mousePos.Y, e.Delta);
					CurrentTool.MouseUp(e);
				}
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e) {
			Cursor = CurrentTool.MouseCursor;

			if (editorControl.IsLevelOpen) {
				Point2I mousePos = ScrollPosition + e.Location;
				cursorTileSize = Point2I.One;

				// Find the highlighted room/tile coordinates.
				if (editorControl.EventMode)
					cursorHalfTileLocation  = SampleLevelHalfTileCoordinates(mousePos);
				else
					cursorHalfTileLocation  = SampleLevelTileCoordinates(mousePos) * 2;
				cursorPixelLocation = SampleLevelPixelPosition(mousePos);
				highlightedRoom		= SampleRoomCoordinates(mousePos, false);
				highlightedTile		= SampleTileCoordinates(mousePos);
				if (!(highlightedRoom < Level.Dimensions)) {
					highlightedRoom = new Point2I(-1000);
					highlightedTile = new Point2I(-2000);
					cursorHalfTileLocation  = new Point2I(-4000);
					cursorPixelLocation = new Point2I(-32000);
				}

				// Notify the current tool.
				if (!editorControl.PlayerPlaceMode) {
					e = new MouseEventArgs(e.Button, e.Clicks, mousePos.X, mousePos.Y, e.Delta);
					CurrentTool.MouseMove(e);
				}

				// Update the status bar for tile and room coordinates.
				if (Level.ContainsRoom(highlightedRoom)) {
					editorWindow.StatusBarLabelRoomLoc.Content = "Room " + highlightedRoom.ToString();
					editorWindow.StatusBarLabelTileLoc.Content = "Tile " + highlightedTile.ToString();
					return;
				}
			}
			editorWindow.StatusBarLabelRoomLoc.Content = "Room (?, ?)";
			editorWindow.StatusBarLabelTileLoc.Content = "Tile (?, ?)";
		}

		private void OnMouseDoubleClick(object sender, MouseEventArgs e) {
			if (editorControl.IsLevelOpen) {
				Point2I mousePos = ScrollPosition + e.Location;

				// Notify the current tool.
				if (!editorControl.PlayerPlaceMode) {
					e = new MouseEventArgs(e.Button, e.Clicks, mousePos.X, mousePos.Y, e.Delta);
					CurrentTool.OnMouseDoubleClick(e);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Overriden methods
		//-----------------------------------------------------------------------------

		protected override void Draw() {
			Stopwatch watch = new Stopwatch();
			watch.Start();
			editorControl.UpdateTicks();
			
			Graphics2D g = new Graphics2D(spriteBatch);
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			DrawLevel(g);
			g.End();
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorWindow EditorWindow {
			get { return editorWindow; }
			set { editorWindow = value; }
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

		public Point2I CursorTileLocation {
			get { return cursorHalfTileLocation / 2; }
			set { cursorHalfTileLocation = value * 2; }
		}
		public Point2I CursorHalfTileLocation {
			get { return cursorHalfTileLocation; }
			set { cursorHalfTileLocation = value; }
		}
		public Point2I CursorPixelLocation {
			get { return cursorPixelLocation; }
			set { cursorPixelLocation = value; }
		}
		public Point2I CursorTileSize {
			get { return cursorTileSize; }
			set { cursorTileSize = value; }
		}
		public bool IsSelectionRoom {
			get { return isSelectionRoom; }
			set { isSelectionRoom = value; }
		}
		public EditorTool CurrentTool {
			get { return editorControl.CurrentTool; }
		}
	}
}

