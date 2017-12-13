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
using ZeldaEditor.Tools;
using DrawMode = ZeldaOracle.Common.Graphics.DrawMode;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

using Effect = Microsoft.Xna.Framework.Graphics.Effect;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using XnaColor = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;
using SurfaceFormat = Microsoft.Xna.Framework.Graphics.SurfaceFormat;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaEditor.WinForms {

	public class LevelDisplay : GraphicsDeviceControl {

		public readonly Color NormalColor = Color.White;
		public readonly Color FadeAboveColor = new Color(200, 200, 200, 100);
		public readonly Color FadeBelowColor = new Color(200, 200, 200, 150);
		public readonly Color HideColor = Color.Transparent;

		private static ContentManager content;
		private static SpriteBatch spriteBatch;

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
		
		private DispatcherTimer dispatcherTimer;

		private Texture2D palette;
		private Effect paletteShader;
		private DrawMode paletteDrawMode;

		// Frame Rate:
		/// <summary>The total number of frames passed since the last frame rate check.</summary>
		private int totalFrames;
		/// <summary>The amount of time passed since the last frame rate check.</summary>
		private double elapsedTime;
		/// <summary>The current frame rate of the game.</summary>
		private double fps;

		private Stopwatch fpsWatch;


		//-----------------------------------------------------------------------------
		// Individual Tile Selection
		//-----------------------------------------------------------------------------

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			try {
				content     = new ContentManager(Services, "Content");
				spriteBatch = new SpriteBatch(GraphicsDevice);

				palette = new Texture2D(GraphicsDevice, 256, 256, false, SurfaceFormat.Color);

				XnaColor[] colorData = new XnaColor[256*256];

				for (int y = 0; y < 256; y++) {
					for (int x = 0; x < 256; x++) {
						colorData[x + y * 256] = new XnaColor(x, y, 0);
					}
				}
				palette.SetData<XnaColor>(colorData);

				paletteShader = content.Load<Effect>("Shaders/palette_shader");
				paletteDrawMode = new DrawMode(GameSettings.DRAW_MODE_DEFAULT);
				paletteDrawMode.Effect = paletteShader;

				editorControl.Initialize(content, GraphicsDevice);
				
				// Wire the events.
				MouseEnter          += OnMouseEnter;
				MouseMove           += OnMouseMove;
				MouseDown           += OnMouseDown;
				MouseUp             += OnMouseUp;
				MouseLeave          += OnMouseLeave;
				MouseDoubleClick    += OnMouseDoubleClick;

				totalFrames = 0;
				elapsedTime = 0.0;
				fps = 0.0;
				fpsWatch = Stopwatch.StartNew();

				this.ResizeRedraw = true;

				// TEMP: Open this world file upon starting the editor.
				if (File.Exists("./temp_world.zwd"))
					editorControl.OpenWorld("temp_world.zwd");
				else if (File.Exists("../../../../WorldFiles/temp_world.zwd"))
					editorControl.OpenWorld("../../../../WorldFiles/temp_world.zwd");
				else if (File.Exists("../../../WorldFiles/temp_world.zwd"))
					editorControl.OpenWorld("../../../WorldFiles/temp_world.zwd");
				//editorControl.OpenFile("temp_world.zwd");

				UpdateLevel();

				this.highlightedRoom = -Point2I.One;
				this.highlightedTile = -Point2I.One;

				dispatcherTimer = new DispatcherTimer(
					TimeSpan.FromMilliseconds(15),
					DispatcherPriority.Render,
					delegate { Invalidate(); },
					System.Windows.Application.Current.Dispatcher);
			}
			catch (Exception e) {
				throw e;
			}
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
		}

		// Set the selection box
		public void SetSelectionBox(Point2I start, Point2I size) {
			selectionBox = new Rectangle2I(start, size);
		}

		public void SetSelectionBox(Room room) {
			selectionBox = new Rectangle2I(room.Size * GameSettings.TILE_SIZE);
			selectionBox.Point += room.Location * room.Size * GameSettings.TILE_SIZE;
		}
		public void SetSelectionBox(BaseTileDataInstance tile) {
			selectionBox = tile.GetBounds();
			selectionBox.Point += tile.Room.Location * tile.Room.Size * GameSettings.TILE_SIZE;
		}

		public void CenterViewOnPoint(Point2I point) {
			ScrollPosition = point - new Point2I(ClientSize.Width, ClientSize.Height) / 2;

			/*this.AutoScrollPosition = new System.Drawing.Point(
				GMath.Clamp(point.X - ClientSize.Width / 2, HorizontalScroll.Minimum, HorizontalScroll.Maximum),
				GMath.Clamp(point.Y - ClientSize.Height / 2, VerticalScroll.Minimum, VerticalScroll.Maximum)
			);*/
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
			if (editorControl.ShowModified && !tile.HasModifiedProperties && !tile.HasDefinedEvents)
				return;

			ISprite sprite = null;
			float playbackTime = editorControl.Ticks;
			int substripIndex =  tile.Properties.GetInteger("substrip_index", 0);
			
			//-----------------------------------------------------------------------------
			// Platform.
			if (tile.Type == typeof(TilePlatform)) {
				ISprite currentSprite = tile.CurrentSprite;
				if (currentSprite != null) {
					// Draw the tile once per point within its size.
					for (int y = 0; y < tile.Size.Y; y++) {
						for (int x = 0; x < tile.Size.X; x++) {
							Point2I drawPos = position +
								(new Point2I(x, y) * GameSettings.TILE_SIZE);
							g.DrawISprite(currentSprite,
								new SpriteDrawSettings(room.Zone.ImageVariantID,
								editorControl.Ticks), drawPos, drawColor);
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
					sprite = GameData.ANIM_TILE_CROSSING_GATE_LOWER;
				else
					sprite = GameData.ANIM_TILE_CROSSING_GATE_RAISE;
				substripIndex = (tile.Properties.GetBoolean("face_left", false) ? 1 : 0);
				playbackTime = 0.0f;
			}
			//-----------------------------------------------------------------------------
			// Lantern.
			else if (tile.Type == typeof(TileLantern)) {
				if (tile.Properties.GetBoolean("lit", true))
					sprite = GameData.ANIM_TILE_LANTERN;
				else
					sprite = GameData.SPR_TILE_LANTERN_UNLIT;
			}
			//-----------------------------------------------------------------------------
			// Chest.
			else if (tile.Type == typeof(TileChest)) {
				bool isLooted = tile.Properties.GetBoolean("looted", false);
				sprite = tile.SpriteList[isLooted ? 1 : 0];
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

			if (sprite == null) {
				sprite = tile.CurrentSprite;
			}
			/*if (animation == null && sprite == null && tile.CurrentSprite.IsAnimation)
				animation = tile.CurrentSprite.Animation;
			if (animation == null && sprite == null && tile.CurrentSprite.IsSprite)
				sprite = tile.CurrentSprite.Sprite;*/

			// Draw the custom sprite/animation
			if (sprite is Animation) {
				g.DrawISprite(((Animation) sprite).GetSubstrip(substripIndex),
					new SpriteDrawSettings(room.Zone.StyleDefinitions, room.Zone.ImageVariantID, playbackTime),
					position, drawColor);
			}
			else if (sprite != null) {
				g.DrawISprite(sprite, new SpriteDrawSettings(room.Zone.StyleDefinitions,
					room.Zone.ImageVariantID), position, drawColor);
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
				g.DrawISprite(arrowAnimation.GetSubstrip(substripIndex),
					new SpriteDrawSettings(room.Zone.StyleDefinitions, room.Zone.ImageVariantID, playbackTime), position, drawColor);
				g.DrawISprite(turnstileAnimation.GetSubstrip(clockwise ? 0 : 1),
					new SpriteDrawSettings(room.Zone.StyleDefinitions,  room.Zone.ImageVariantID, 16f), position, drawColor);
			}
			//-----------------------------------------------------------------------------

			/*else if (!tile.CurrentSprite.IsNull) {
				g.DrawAnimation(tile.CurrentSprite,
					room.Zone.ImageVariantID, editorControl.Ticks, position, drawColor);
			}*/

			// Draw rewards.
			if (editorControl.ShowRewards && tile.Properties.Contains("reward") &&
				editorControl.RewardManager.HasReward(tile.Properties.GetString("reward")))
			{
				sprite = editorControl.RewardManager.GetReward(tile.Properties.GetString("reward")).Sprite;
				g.DrawISprite(sprite, new SpriteDrawSettings(room.Zone.StyleDefinitions,
					(float)editorControl.Ticks), position, drawColor);
			}
		}

		// Draw an event tile.
		public void DrawEventTile(Graphics2D g, Room room, EventTileDataInstance eventTile, Point2I position, Color drawColor) {
			if (editorControl.ShowModified && !eventTile.HasModifiedProperties && !eventTile.HasDefinedEvents)
				return;

			ISprite sprite = eventTile.CurrentSprite;
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
					sprite = GameData.SPR_EVENT_TILE_WARP_ENTRANCE;
				else if (warpType == WarpType.Tunnel)
					sprite = GameData.SPR_EVENT_TILE_WARP_TUNNEL;
				else if (warpType == WarpType.Stairs)
					sprite = GameData.SPR_EVENT_TILE_WARP_STAIRS;
			}

			// Draw the sprite.
			if (sprite is Animation) {
				g.DrawISprite(((Animation) sprite).GetSubstrip(eventTile.SubStripIndex),
					new SpriteDrawSettings(room.Zone.StyleDefinitions, room.Zone.ImageVariantID,
					(float) editorControl.Ticks), position, drawColor);
			}
			else if (sprite != null) {
				g.DrawISprite(sprite, new SpriteDrawSettings(
					room.Zone.StyleDefinitions, imageVariantID, (float) editorControl.Ticks),
					position, drawColor);
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

			Palette lastPalette = null;

			// Draw the level if it is open.
			if (editorControl.IsLevelOpen) {
				// Draw the rooms.
				for (int x = 0; x < Level.Width; x++) {
					for (int y = 0; y < Level.Height; y++) {
						Point2I roomPosition = GetRoomDrawPosition(new Point2I(x, y)) - new Point2I(HorizontalScroll.Value, VerticalScroll.Value);
						if (roomPosition + (Level.RoomSize * GameSettings.TILE_SIZE) >= Point2I.Zero && roomPosition < new Point2I(ClientSize.Width, ClientSize.Height)) {
							g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
							g.Translate((Vector2F)(new Point2I(x, y) * ((Level.RoomSize * GameSettings.TILE_SIZE) + editorControl.RoomSpacing)));
							Room room = Level.GetRoomAt(x, y);
							Palette newPalette = room.Zone.Palette;
							if (lastPalette != newPalette) {
								g.End();
								GameData.PaletteShader.TilePalette = newPalette;
								GameData.PaletteShader.ApplyPalettes();
								g.Begin(GameSettings.DRAW_MODE_DEFAULT);
							}
							lastPalette = newPalette;
							DrawRoom(g, room);
							g.ResetTranslation();
						}
					}
				}

				g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
				CurrentTool.DrawEventTiles(g);
				g.ResetTranslation();

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

				
				if (!selectionBox.IsEmpty) {
					g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));

					// Draw the selection box.
					Point2I start = GetLevelPixelDrawPosition(selectionBox.TopLeft);
					Point2I end   = GetLevelPixelDrawPosition(selectionBox.BottomRight);
					Rectangle2I box = new Rectangle2I(start, end - start);

					g.DrawRectangle(box, 1, Color.White);
					g.DrawRectangle(box.Inflated(1, 1), 1, Color.Black);
					g.DrawRectangle(box.Inflated(-1, -1), 1, Color.Black);

					g.ResetTranslation();
				}

				// Draw the highlight box.
				if (editorControl.HighlightMouseTile && cursorHalfTileLocation >= Point2I.Zero) {
					g.Translate(new Vector2F(-HorizontalScroll.Value, -VerticalScroll.Value));
					Rectangle2I box = new Rectangle2I(GetLevelHalfTileCoordDrawPosition(cursorHalfTileLocation), cursorTileSize * 16);
					g.DrawRectangle(box.Inflated(1, 1), 1, Color.White);
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

		}
		private void OnMouseLeave(object sender, EventArgs e) {
			// Clear any highlighted tile/rooms.
			highlightedRoom = new Point2I(-1000);
			highlightedTile = new Point2I(-2000);
			cursorHalfTileLocation	= new Point2I(-4000);
			cursorPixelLocation = new Point2I(-32000);
			editorWindow.SetStatusBarInvalidLevelLocations();
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
					editorWindow.SetStatusBarLevelLocations(highlightedRoom, highlightedTile);
					return;
				}
			}
			editorWindow.SetStatusBarInvalidLevelLocations();
		}

		private void OnMouseDoubleClick(object sender, MouseEventArgs e) {
			if (editorControl.IsLevelOpen) {
				Point2I mousePos = ScrollPosition + e.Location;

				// Notify the current tool.
				if (!editorControl.PlayerPlaceMode) {
					e = new MouseEventArgs(e.Button, e.Clicks, mousePos.X, mousePos.Y, e.Delta);
					CurrentTool.MouseDoubleClick(e);
				}
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
			Stopwatch watch = new Stopwatch();
			watch.Start();
			editorControl.UpdateTicks();
			UpdateFrameRate();
			//GraphicsDevice.Textures[1] = Resources.GetPalette("dungeon_ages_1").PaletteTexture;
			Graphics2D g = new Graphics2D(spriteBatch);
			//g.Begin(paletteDrawMode);
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			//GameSettings.DRAW_MODE_DEFAULT.Effect.CurrentTechnique.Passes[0].Apply();
			//paletteShader.CurrentTechnique.Passes[0].Apply();
			DrawLevel(g);
			//g.DrawImage(palette, Point2I.Zero);
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

		public Rectangle2I SelectionBox {
			get { return selectionBox; }
			set {
				selectionBox = value;
				Point2I roomLoc = GMath.Clamp(value.Point / Level.RoomSize, Point2I.Zero, Level.Dimensions);
				selectedRoom = Level.GetRoomAt(roomLoc);
			}
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

		public Point2I ScrollPosition {
			get { return new Point2I(HorizontalScroll.Value, VerticalScroll.Value); }
			set {
				AutoScrollPosition = new System.Drawing.Point(
					GMath.Clamp(value.X, HorizontalScroll.Minimum, HorizontalScroll.Maximum),
					GMath.Clamp(value.Y, VerticalScroll.Minimum, VerticalScroll.Maximum)
				);
			}
		}

		public double FPS {
			get { return fps; }
		}
	}				
}

