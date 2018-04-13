using System;
using System.Diagnostics;
using System.Windows.Forms;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Worlds.Editing;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Tiles;
using ZeldaWpf.Util;
using ZeldaWpf.WinForms;
using ZeldaEditor.Control;
using ZeldaEditor.Tools;

namespace ZeldaEditor.WinForms {

	public class LevelDisplay : TimersGraphicsDeviceControl {

		public readonly Color NormalColor = Color.White;
		public readonly Color FadeAboveColor = new Color(200, 200, 200, 100);
		public readonly Color FadeBelowColor = new Color(200, 200, 200, 150);
		public readonly Color HideColor = Color.Transparent;

		private EditorWindow	editorWindow;
		private EditorControl	editorControl;
		private Rectangle2I		cursorBox;
		private Point2I			highlightedRoom;
		private Point2I			highlightedTile;
		private bool			showCursor;
		private Rectangle2I		selectionBox;

		// Frame Rate:
		/// <summary>The total number of frames passed since the last frame rate check.</summary>
		private int totalFrames;
		/// <summary>The amount of time passed since the last frame rate check.</summary>
		private double elapsedTime;
		/// <summary>The current frame rate of the game.</summary>
		private double fps;

		private Stopwatch fpsWatch;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public LevelDisplay() { }

		protected override void Initialize() {
				
			// Wire the events.
			MouseEnter          += OnMouseEnter;
			MouseMove           += OnMouseMove;
			MouseDown           += OnMouseDown;
			MouseUp             += OnMouseUp;
			MouseLeave          += OnMouseLeave;
			MouseDoubleClick    += OnMouseDoubleClick;
			MouseWheel          += OnMouseWheel;

			totalFrames = 0;
			elapsedTime = 0.0;
			fps = 0.0;
			fpsWatch = Stopwatch.StartNew();

			ResizeRedraw = true;

			UpdateLevel();

			cursorBox = Rectangle2I.Zero;

			highlightedRoom = -Point2I.One;
			highlightedTile = -Point2I.One;

			ContinuousEvents.StartRender(
				() => {
					if (editorControl.IsActive) {
						if (editorControl.IsLevelOpen)
							editorControl.CurrentTool.Update();
						Invalidate();
					}
				});
		}


		//-----------------------------------------------------------------------------
		// Level Sampling
		//-----------------------------------------------------------------------------

		// Sample the room coordinates at the given point, clamping them to the level's dimensions if specified.
		public Point2I SampleRoomLocation(Point2I point, bool clamp = false) {
			Point2I span = Level.RoomPixelSize + RoomSpacing;
			Point2I roomCoord = GMath.FloorDiv(point, span);
			if (clamp)
				return GMath.Clamp(roomCoord, Point2I.Zero,
					Level.Dimensions - Point2I.One);
			return roomCoord;
		}

		// Sample the room at the given point, clamping to the levels dimensions if specified.
		public Room SampleRoom(Point2I point, bool clamp = false) {
			if (!editorControl.IsLevelOpen)
				return null;
			Point2I roomCoord = SampleRoomLocation(point, clamp);
			return Level.GetRoomAt(roomCoord);
		}

		// Sample the tile coordinates at the given point (tile coordinates are relative to their room).
		public Point2I SampleTileLocation(Point2I point) {
			Point2I span = Level.RoomPixelSize + RoomSpacing;
			Point2I begin = SampleRoomLocation(point) * span;
			Point2I tileCoord = GMath.FloorDiv(point - begin, GameSettings.TILE_SIZE);
			return GMath.Clamp(tileCoord, Point2I.Zero, Level.RoomSize - Point2I.One);
		}

		// Sample the tile coordinates at the given point (tile coordinates are absolute to the level).
		public Point2I SampleLevelCoord(Point2I point) {
			Point2I roomCoord = SampleRoomLocation(point) * Level.RoomSize;
			Point2I tileCoord = SampleTileLocation(point);
			return (roomCoord + tileCoord);
		}

		// Sample the tile coordinates at the given point (tile coordinates are absolute to the level).
		public Point2I SampleLevelPosition(Point2I point) {
			Point2I roomLocation = SampleRoomLocation(point);
			point -= GetRoomDrawPosition(roomLocation);
			point = GMath.Min(point, Level.RoomPixelSize - 1);
			point += Level.RoomLocationToLevelPosition(roomLocation);
			return point;
		}
		
		// Convert a tile location relative to a room, to an absolute tile location in the level.
		public TileDataInstance SampleTile(Point2I point, int layer) {
			Room room = SampleRoom(point);
			if (room == null)
				return null;
			Point2I tileCoord = SampleTileLocation(point);
			return room.GetTile(tileCoord, layer);
		}
		
		// Sample an action tile at the given point.
		public ActionTileDataInstance SampleActionTile(Point2I point) {
			Room room = SampleRoom(point);
			if (room == null)
				return null;
			Point2I roomOffset = GetRoomDrawPosition(room);
			Point2I position = SampleLevelPosition(point);
			return Level.GetActionTileAt(position);
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
		}

		public void SetSelectionBox(Room room) {
			selectionBox = room.LevelBounds;
		}
		public void SetSelectionBox(BaseTileDataInstance tile) {
			selectionBox = tile.LevelBounds;
		}

		public void CenterViewOnPoint(Point2I point) {
			ScrollPosition = point - ClientSize / 2;
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
			return (roomCoord * (Level.RoomPixelSize + RoomSpacing));
		}

		// Get the top-left position to draw a tile with the given absolute coordinates.
		private Point2I GetLevelCoordDrawPosition(Point2I levelCoord) {
			Point2I roomLocation = Level.LevelCoordToRoomLocation(levelCoord);
			Point2I roomPosition = Level.LevelCoordToRoomPosition(levelCoord);
			return (GetRoomDrawPosition(roomLocation) + roomPosition);
		}
		
		// Get the top-left position to draw a tile with the given absolute coordinates.
		public Point2I GetLevelPixelDrawPosition(Point2I levelPosition) {
			Point2I roomLocation = Level.LevelPositionToRoomLocation(levelPosition);
			Point2I roomPosition = Level.LevelToRoomPosition(levelPosition);
			return (GetRoomDrawPosition(roomLocation) + roomPosition);
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		// Draw a tile.
		public void DrawTile(Graphics2D g, Room room, TileDataInstance tile,
			Point2I position, Color drawColor)
		{
			if (editorControl.ShowModified &&
				!tile.HasModifiedProperties &&
				!tile.HasDefinedEvents)
				return;

			TileDataDrawing.DrawTile(g, tile, position, room.Zone, drawColor);
		}

		// Draw an action tile.
		public void DrawActionTile(Graphics2D g, Room room,
			ActionTileDataInstance action, Point2I position, Color drawColor)
		{
			if (editorControl.ShowModified &&
				!action.HasModifiedProperties &&
				!action.HasDefinedEvents)
				return;

			TileDataDrawing.DrawTile(g, action, position, room.Zone, drawColor);
		}

		private Color GetLayerColor(int layer, bool shared) {
			if (shared && !editorControl.ShowShared)
				return Color.Transparent;
			if (!editorControl.ActionMode) {
				if (editorControl.CurrentLayer > layer) {
					if (editorControl.BelowTileDrawMode == TileDrawModes.Hide)
						return Color.Transparent;
					if (editorControl.BelowTileDrawMode == TileDrawModes.Fade)
						return FadeBelowColor;
				}
				else if (editorControl.CurrentLayer < layer) {
					if (editorControl.AboveTileDrawMode == TileDrawModes.Hide)
						return Color.Transparent;
					if (editorControl.AboveTileDrawMode == TileDrawModes.Fade)
						return FadeAboveColor;
				}
			}
			return NormalColor;
		}

		private Color GetActionColor(bool shared) {
			if (shared && !editorControl.ShowShared)
				return Color.Transparent;
			return NormalColor;
		}

		// Draw an entire room.
		private void DrawRoom(Graphics2D g, Room room) {
			TileDataDrawing.Room = room;
			Point2I roomStartTile = room.LevelCoord;
			Point2I roomStartPixel = roomStartTile * GameSettings.TILE_SIZE;

			// Draw white background
			g.FillRectangle(room.Bounds, Color.White);

			// Draw tile layers
			for (int layer = 0; layer < room.LayerCount; layer++) {
				foreach (var tile in room.GetTileLayer(
					layer, EditorControl.ShowShared))
				{
					Color color = GetLayerColor(layer, tile.Room != room);
					if (color.IsTransparent)
						continue;
					
					Point2I position = tile.Location * GameSettings.TILE_SIZE;
					Point2I levelCoord = roomStartTile + tile.Location;
					if (!CurrentTool.DrawHideTile(tile, room, levelCoord, layer)) {
						DrawTile(g, room, tile, position, color);
					}
				}

				// Draw the tool tiles for this layer.
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						Point2I position = new Point2I(x, y) * GameSettings.TILE_SIZE;
						Point2I levelCoord = roomStartTile + new Point2I(x, y);

						CurrentTool.DrawTile(g, room, position, levelCoord, layer);
					}
				}
			}
			
			// Draw action tiles
			if (editorControl.ShowActions || editorControl.ShouldDrawActions) {
				foreach (var action in room.GetActionTiles(editorControl.ShowShared)) {
					Color color = GetActionColor(action.Room != room);
					if (color.IsTransparent)
						continue;
					if (!CurrentTool.DrawHideActionTile(action, room,
						roomStartPixel + action.Position))
					{
						DrawActionTile(g, room, action, action.Position, color);
					}
				}
			}
		}
		
		// Draw an entire level.
		public void DrawLevel(Graphics2D g) {
			g.Clear(new Color(175, 175, 180)); // Gray background.
			
			// Draw the level if it is open.
			if (editorControl.IsLevelOpen) {
				Palette lastPalette = null;

				g.PushTranslation(-ScrollPosition);

				// Draw the rooms.
				for (int x = 0; x < Level.Width; x++) {
					for (int y = 0; y < Level.Height; y++) {
						Point2I roomPosition = GetRoomDrawPosition(new Point2I(x, y));
						Rectangle2I roomBounds = new Rectangle2I(roomPosition,
							Level.RoomPixelSize + Point2I.One);
						if (roomBounds.Intersects(ClientView)) {
							g.PushTranslation(roomPosition);
							Room room = Level.GetRoomAt(x, y);
							Palette newPalette = room.Zone.Palette;
							if (lastPalette != newPalette) {
								g.End();
								GameData.SHADER_PALETTE.TilePalette = newPalette;
								GameData.SHADER_PALETTE.ApplyParameters();
								g.Begin(GameSettings.DRAW_MODE_PALLETE);
							}
							lastPalette = newPalette;
							DrawRoom(g, room);
							g.PopTranslation();
						}
					}
				}
				
				CurrentTool.DrawActionTiles(g);
				
				Point2I span = Level.TileDimensions;
				Point2I drawSpan = GetRoomDrawPosition(Level.Dimensions);
				// Draw the tile grid
				Color tileGridColor = new Color(0, 0, 0, 150);
				if (editorControl.ShowGrid) {
					for (int x = 0; x < span.X; x++) {
						int drawX = GetLevelCoordDrawPosition(new Point2I(x, 0)).X;
						if (drawX >= ScrollX || drawX < ScrollX + ClientWidth) {
							g.FillRectangle(new Rectangle2F(
								drawX, ScrollY,
								1, Math.Min(drawSpan.Y, ClientHeight)),
								tileGridColor);
						}
					}
					for (int y = 0; y < span.Y; y++) {
						int drawY = GetLevelCoordDrawPosition(new Point2I(0, y)).Y;
						if (drawY >= ScrollY || drawY < ScrollY + ClientHeight) {
							g.FillRectangle(new Rectangle2F(
								ScrollX, drawY,
								Math.Min(drawSpan.X, ClientWidth), 1),
								tileGridColor);
						}
					}
				}

				// Draw the room spacing
				if (RoomSpacing > 0) {
					for (int x = 0; x < Level.Width; x++) {
						int drawX = GetRoomDrawPosition(new Point2I(x + 1, 0)).X -
							RoomSpacing;
						if (ClientView.LeftRight.Contains(drawX)) {
							g.FillRectangle(new Rectangle2F(drawX, ScrollY,
								RoomSpacing, ClientView.Height),
								Color.Black);
						}
					}
					for (int y = 0; y < Level.Height; y++) {
						int drawY = GetRoomDrawPosition(new Point2I(0, y + 1)).Y -
							RoomSpacing;
						if (ClientView.TopBottom.Contains(drawY)) {
							g.FillRectangle(new Rectangle2F(ScrollX, drawY,
								ClientView.Width, RoomSpacing),
								Color.Black);
						}
					}
				}

				
				if (!selectionBox.IsEmpty) {
					// Draw the selection box
					Point2I start = GetLevelPixelDrawPosition(selectionBox.TopLeft);
					Point2I end = GetLevelPixelDrawPosition(selectionBox.BottomRight);
					Rectangle2I box = Rectangle2I.FromEndPoints(start, end);

					g.DrawRectangle(box, 1, Color.White);
					g.DrawRectangle(box.Inflated(1, 1), 1, Color.Black);
					g.DrawRectangle(box.Inflated(-1, -1), 1, Color.Black);
				}

				// Draw the highlight cursor
				if (CanUpdateTool && showCursor && !cursorBox.IsEmpty &&
					cursorBox.Intersects(ClientView))
				{
					Rectangle2I box = new Rectangle2I(
						GetLevelPixelDrawPosition(cursorBox.Point), cursorBox.Size);
					g.DrawRectangle(box.Inflated(1, 1), 1, Color.White);
				}

				// Draw the player start location
				if (Level == World.StartLevel && (editorControl.ShowStartLocation ||
					editorControl.StartLocationMode))
				{
					Point2I position = GetLevelCoordDrawPosition(
						World.StartLevelCoord);
					g.DrawSprite(GameData.SPR_PLAYER_FORWARD, position);
				}

				// Draw player sprite for 'Test At Position'
				if (IsMouseOver && (editorControl.PlayerPlaceMode ||
					editorControl.StartLocationMode))
				{
					Point2I tilePosition = GetRoomDrawPosition(highlightedRoom) +
						Level.LevelCoordToPosition(highlightedTile);
					g.DrawSprite(GameData.SPR_PLAYER_FORWARD, tilePosition);
				}

				g.PopTranslation();
			}
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		public void UpdateLevel() {
			if (editorControl.IsLevelOpen) {
				this.AutoScrollMinSize = (Level.Dimensions * (Level.RoomPixelSize +
					RoomSpacing)).ToGdiSize();
			}
			else {
				this.AutoScrollMinSize = Point2I.One.ToGdiSize();
				ScrollPosition = Point2I.Zero;
			}
		}

		public void ChangeLevel() {
			UpdateLevel();
			ClearSelectionBox();
		}


		//-----------------------------------------------------------------------------
		// Form Events
		//-----------------------------------------------------------------------------
		
		private void OnMouseEnter(object sender, EventArgs e) {
			Cursor = CurrentTool.MouseCursor;
		}

		private void OnMouseLeave(object sender, EventArgs e) {
			// Clear any highlighted tile/rooms
			highlightedRoom = new Point2I(short.MinValue);
			highlightedTile = new Point2I(short.MinValue);
			cursorBox.Point = new Point2I(short.MinValue);
			editorWindow.SetStatusBarInvalidLevelLocations();
		}

		private void OnMouseDown(object sender, MouseEventArgs e) {
			Cursor = CurrentTool.MouseCursor;

			if (editorControl.IsLevelOpen) {
				bool containsRoom = Level.ContainsRoom(highlightedRoom);
				if (editorControl.PlayerPlaceMode) {
					// Test the world after placing the player
					if (containsRoom) {
						editorControl.TestWorld(highlightedRoom, highlightedTile);
					}
				}
				else if (editorControl.StartLocationMode) {
					// Set the new player start location
					if (containsRoom) {
						editorControl.SetStartLocation(
							highlightedRoom, highlightedTile);
					}
				}
				else if (CanUpdateTool) {
					// Notify the current tool
					CurrentTool.MouseDown(e);
				}
			}
			this.Focus();
		}
		
		private void OnMouseUp(object sender, MouseEventArgs e) {
			Cursor = CurrentTool.MouseCursor;

			if (editorControl.IsLevelOpen && CanUpdateTool) {
				// Notify the current tool
				CurrentTool.MouseUp(e);
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e) {
			Cursor = CurrentTool.MouseCursor;

			if (editorControl.IsLevelOpen) {
				Point2I mousePos = ScrollPosition + e.Location.ToPoint2I();

				highlightedRoom		= SampleRoomLocation(mousePos, false);
				highlightedTile		= SampleTileLocation(mousePos);
				if (!Level.ContainsRoom(highlightedRoom)) {
					highlightedRoom = new Point2I(short.MinValue);
					highlightedTile = new Point2I(short.MinValue);
					cursorBox.Point = new Point2I(short.MinValue);
				}

				// Notify the current tool
				if (CanUpdateTool) {
					CurrentTool.MouseMove(e);
				}

				// Update the status bar for tile and room coordinates
				if (Level.ContainsRoom(highlightedRoom)) {
					editorWindow.SetStatusBarLevelLocations(
						highlightedRoom, highlightedTile);
					return;
				}
			}
			editorWindow.SetStatusBarInvalidLevelLocations();
		}

		private void OnMouseDoubleClick(object sender, MouseEventArgs e) {
			if (editorControl.IsLevelOpen && CanUpdateTool) {
				// Notify the current tool
				CurrentTool.MouseDoubleClick(e);
			}
		}

		private void OnMouseWheel(object sender, MouseEventArgs e) {
			if (editorControl.IsLevelOpen && CanUpdateTool) {
				// Notify the current tool
				CurrentTool.MouseMove(e);
			}
		}

		/// <summary>Called every step to update the frame rate.</summary>
		private void UpdateFrameRate() {
			// FPS Counter from:
			// http://www.david-amador.com/2009/11/how-to-do-a-xna-fps-counter/
			elapsedTime     = fpsWatch.ElapsedMilliseconds;
			if (elapsedTime >= 1000.0) {
				fps         = (double) totalFrames * 1000.0 / elapsedTime;
				totalFrames = 0;
				elapsedTime = 0.0;
				fpsWatch.Restart();
			}
			totalFrames++;
		}

		//-----------------------------------------------------------------------------
		// Overriden methods
		//-----------------------------------------------------------------------------

		protected override void Draw() {
			if (!Resources.IsInitialized) return;
			if (!editorControl.IsInitialized)
				return;
			Stopwatch watch = new Stopwatch();
			watch.Start();
			editorControl.UpdateTicks();
			UpdateFrameRate();
			TileDataDrawing.PlaybackTime = editorControl.Ticks;
			TileDataDrawing.Extras = editorControl.ShowRewards;
			TileDataDrawing.Level = Level;
			Graphics2D g = new Graphics2D();
			g.Begin(GameSettings.DRAW_MODE_PALLETE);
			DrawLevel(g);
			g.End();
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		private bool CanUpdateTool {
			get {
				return	!editorControl.PlayerPlaceMode &&
						!editorControl.StartLocationMode;
			}
		}

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

		public Rectangle2I SelectionBox {
			get { return selectionBox; }
		}
		
		public Point2I CursorLevelCoord {
			get { return GMath.FloorDiv(cursorBox.Point, GameSettings.TILE_SIZE); }
			set { cursorBox.Point = value * GameSettings.TILE_SIZE; }
		}
		public Point2I CursorRoomLocation {
			get { return GMath.FloorDiv(cursorBox.Point, GameSettings.TILE_SIZE); }
			set { cursorBox.Point = value * GameSettings.TILE_SIZE; }
		}

		public bool ShowCursor {
			get { return showCursor; }
			set { showCursor = value; }
		}
		public Point2I CursorPosition {
			get { return cursorBox.Point; }
			set { cursorBox.Point = value; }
		}
		public Point2I CursorSize {
			get { return cursorBox.Size; }
			set { cursorBox.Size = value; }
		}
		public Point2I CursorTileSize {
			get { return GMath.CeilingDiv(cursorBox.Size, GameSettings.TILE_SIZE); }
			set { cursorBox.Size = value * GameSettings.TILE_SIZE; }
		}
		public Point2I CursorRoomSize {
			get {
				return GMath.CeilingDiv(cursorBox.Size,
					GameSettings.TILE_SIZE * Level.RoomSize);
			}
			set { cursorBox.Size = value * GameSettings.TILE_SIZE * Level.RoomSize; }
		}
		
		public EditorTool CurrentTool {
			get { return editorControl.CurrentTool; }
		}

		public int RoomSpacing {
			get { return editorControl.RoomSpacing; }
		}

		/*public Point2I ScrollPosition {
			get { return new Point2I(HorizontalScroll.Value, VerticalScroll.Value); }
			set {
				AutoScrollPosition = GMath.Clamp(value,
					new Point2I(HorizontalScroll.Minimum, VerticalScroll.Minimum),
					new Point2I(HorizontalScroll.Maximum, VerticalScroll.Maximum))
						.ToGdiPoint();
			}
		}

		public new Point2I ClientSize {
			get { return base.ClientSize.ToPoint2I(); }
		}

		public int ClientWidth {
			get { return base.ClientSize.Width; }
		}

		public int ClientHeight {
			get { return base.ClientSize.Height; }
		}

		public Point2I ScrollSize {
			get { return AutoScrollMinSize.ToPoint2I(); }
			set { AutoScrollMinSize = value.ToGdiSize(); }
		}

		public int ScrollX {
			get { return HorizontalScroll.Value; }
		}

		public int ScrollY {
			get { return VerticalScroll.Value; }
		}

		public Rectangle2I ClientView {
			get {
				return new Rectangle2I(ScrollPosition,
					GMath.Min(ScrollSize, ClientSize));
			}
		}*/

		public double FPS {
			get { return fps; }
		}
	}				
}

