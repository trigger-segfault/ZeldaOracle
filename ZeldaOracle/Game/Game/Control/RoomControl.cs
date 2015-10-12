using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Debug;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Control.Menus;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Tiles.Custom;
using ZeldaOracle.Game.GameStates.RoomStates;

namespace ZeldaOracle.Game.Control {

	// Handles the main Zelda gameplay within a room.
	public class RoomControl : GameState {

		private Level			level;
		private Room			room;
		private Point2I			roomLocation;
		private List<Entity>	entities;
		private Tile[,,]		tiles;
		private List<EventTile>	eventTiles;
		private ViewControl		viewControl;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomControl() {
			level			= null;
			room			= null;
			tiles			= null;
			roomLocation	= Point2I.Zero;
			entities		= new List<Entity>();
			eventTiles		= new List<EventTile>();
			viewControl		= new ViewControl();
		}
		

		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------
		
		// Return the tile at the given location (can return null).
		public Tile GetTile(Point2I location, int layer) {
			return tiles[location.X, location.Y, layer];
		}

		// Return the tile at the given location (can return null).
		public Tile GetTile(int x, int y, int layer) {
			return tiles[x, y, layer];
		}

		// Return the tile at the given location that's on the highest layer.
		public Tile GetTopTile(int x, int y) {
			for (int i = room.LayerCount - 1; i >= 0; i--) {
				if (tiles[x, y, i] != null)
					return tiles[x, y, i];
			}
			return null;
		}

		// Return the tile at the given location that's on the highest layer.
		public Tile GetTopTile(Point2I location) {
			for (int i = room.LayerCount - 1; i >= 0; i--) {
				if (tiles[location.X, location.Y, i] != null)
					return tiles[location.X, location.Y, i];
			}
			return null;
		}
		
		// Return true if the given tile location is inside the room.
		public bool IsTileInBounds(Point2I location, int layer = 0) {
			return (location.X >= 0 && location.X < room.Width &&
					location.Y >= 0 && location.Y < room.Height &&
					layer >= 0 && layer < room.LayerCount);
		}

		// Return the tile location that the given position in pixels is situated in.
		public Point2I GetTileLocation(Vector2F position) {
			return (Point2I) (position / GameSettings.TILE_SIZE);
		}

		public Rectangle2I GetTileAreaFromRect(Rectangle2F rect, int inflateAmount = 0) {
			Rectangle2I area;
			area.Point	= (Point2I) (rect.TopLeft / (float) GameSettings.TILE_SIZE);
			area.Size	= ((Point2I) (rect.BottomRight / (float) GameSettings.TILE_SIZE)) + Point2I.One - area.Point;
			area.Inflate(inflateAmount, inflateAmount);
			return Rectangle2I.Intersect(area, new Rectangle2I(Point2I.Zero, room.Size));
		}
		

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		// Move the given tile to a new location.
		public void MoveTile(Tile tile, Point2I newLocation, int newLayer) {
			tiles[tile.Location.X, tile.Location.Y, tile.Layer] = null;
			tiles[newLocation.X, newLocation.Y, newLayer] = tile;
			tile.Location = newLocation;
			tile.Layer = newLayer;
		}


		//-----------------------------------------------------------------------------
		// Spawning / Removal
		//-----------------------------------------------------------------------------
		
		// Use this for spawning entites at runtime.
		public void SpawnEntity(Entity e) {
			e.Initialize(this);
			entities.Add(e);
		}
		
		// Use this for spawning entites at a position at runtime.
		public void SpawnEntity(Entity e, Vector2F position) {
			e.Position = position;
			e.Initialize(this);
			entities.Add(e);
		}
		
		// Use this for spawning entites at a position at runtime.
		public void SpawnEntity(Entity e, Vector2F position, float zPosition) {
			e.Position = position;
			e.ZPosition = zPosition;
			e.Initialize(this);
			entities.Add(e);
		}
		
		// Use this for placing tiles at runtime.
		public void PlaceTile(Tile tile, Point2I location, int layer) {
			PlaceTile(tile, location.X, location.Y, layer);
		}

		// Use this for placing tiles at runtime.
		public void PlaceTile(Tile tile, int x, int y, int layer) {
			tile.Initialize(this);
			tiles[x, y, layer] = tile;
		}

		// Use this for placing tiles at runtime.
		public void RemoveTile(Tile tile) {
			// TODO: OnRemove?
			tiles[tile.Location.X, tile.Location.Y, tile.Layer] = null;
		}

		// Put an event tile into the room.
		public void AddEventTile(EventTile eventTile) {
			eventTile.Initialize(this);
			eventTiles.Add(eventTile);
		}


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------
		
		public void BeginRoom() {
			BeginRoom(room);
		}

		public void BeginRoom(Room room) {
			this.room			= room;
			this.roomLocation	= room.Location;
			this.level			= room.Level;

			// Clear event tiles.
			eventTiles.Clear();

			// Clear all entities from the old room (except for the player).
			entities.Clear();
			if (Player != null) {
				Player.Initialize(this);
				entities.Add(Player);
			}

			// Create the tile grid.
			tiles = new Tile[room.Width, room.Height, room.LayerCount];
			for (int x = 0; x < room.Width; x++) {
				for (int y = 0; y < room.Height; y++) {
					for (int i = 0; i < room.LayerCount; i++) {
						TileDataInstance data = room.TileData[x, y, i];
						tiles[x, y, i] = null;
						if (data != null)
							tiles[x, y, i] = Tile.CreateTile(data);
					}
				}
			}

			// Create the event tiles.
			eventTiles.Capacity = room.EventData.Count;
			for (int i = 0; i < room.EventData.Count; i++) {
				EventTileDataInstance data  = room.EventData[i];
				EventTile eventTile = EventTile.CreateEvent(data);
				eventTiles.Add(eventTile);
			}
			
			// Initialize the tiles.
			for (int x = 0; x < room.Width; x++) {
				for (int y = 0; y < room.Height; y++) {
					for (int i = 0; i < room.LayerCount; i++) {
						Tile t = tiles[x, y, i];
						if (t != null)
							t.Initialize(this);
					}
				}
			}
			
			// Initialize the event tiles.
			for (int i = 0; i < eventTiles.Count; i++) {
				eventTiles[i].Initialize(this);
			}
			
			viewControl.Bounds = RoomBounds;
			viewControl.ViewSize = GameSettings.VIEW_SIZE;
		}

		// Set all entities to destroyed (except the player).
		public void DestroyRoom() {
			for (int i = 0; i < entities.Count; i++) {
				if (entities[i] != Player)
					entities[i].IsDestroyed = true;
			}
		}
		
		public void TransitionToRoom(Room nextRoom, RoomTransition transition) {
			// Create the new room control.
			RoomControl newControl = new RoomControl();
			newControl.gameManager	= gameManager;
			newControl.level		= level;
			newControl.room			= nextRoom;
			newControl.roomLocation	= nextRoom.Location;
			
			// Play the transition.
			transition.OldRoomControl = this;
			transition.NewRoomControl = newControl;
			gameManager.PopGameState();
			gameManager.PushGameState(transition);
			GameControl.RoomControl = newControl;
		}

		// Transition to a room adjacent to the current one.
		public void EnterAdjacentRoom(int direction) {
			Point2I nextLocation = roomLocation + Directions.ToPoint(direction);

			// Transition to the room.
			if (level.ContainsRoom(nextLocation)) {
				Room nextRoom = level.GetRoomAt(nextLocation);
				TransitionToRoom(nextRoom, new RoomTransitionPush(direction));
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			GameControl.RoomControl = this;
		}
		
		public override void OnEnd() {
			
		}

		public override void Update() {
			GameControl.RoomTicks++;

			RoomState roomState		= GameControl.CurrentRoomState;
			GameControl.UpdateRoom	= roomState.UpdateRoom;
			GameControl.AnimateRoom	= roomState.AnimateRoom;

			// Update entities.
			int entityCount = entities.Count;
			for (int i = 0; i < entities.Count; i++) {
				if (entities[i].IsAlive && entities[i].IsInRoom && i < entityCount) {
					if (GameControl.UpdateRoom)
						entities[i].Update();
					if (GameControl.AnimateRoom)
						entities[i].UpdateGraphics();
				}
				else if (entities[i].IsAlive && entities[i].IsInRoom && i >= entityCount) {
					// For entities spawned this frame, only update their graphics component.
					entities[i].Graphics.SyncAnimation();
				}
			}

			// Remove destroyed entities.
			for (int i = 0; i < entities.Count; i++) {
				if (!entities[i].IsAlive || !entities[i].IsInRoom) {
					entities.RemoveAt(i--);
				}
			}
			
			// Update tiles.
			for (int i = 0; i < room.LayerCount; i++) {
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						Tile t = tiles[x, y, i];
						if (t != null) {
							if (GameControl.UpdateRoom)
								t.Update();
							if (GameControl.AnimateRoom)
								t.UpdateGraphics();
						}
					}
				}
			}
			
			// Update the event tiles.
			for (int i = 0; i < eventTiles.Count; i++) {
				eventTiles[i].Update();
			}

			// Update view to follow player.
			viewControl.PanTo(Player.Center);

			// Detect room transitions.
			if (Player.AllowRoomTransition) {
				for (int direction = 0; direction < Directions.Count; direction++) {
					CollisionInfo info = Player.Physics.CollisionInfo[direction];

					if (info.Type == CollisionType.RoomEdge &&
						(Controls.GetArrowControl(direction).IsDown() ||
						Controls.GetAnalogDirection(direction) ||
						Player.AutoRoomTransition))
					{
						EnterAdjacentRoom(direction);
						break;
					}
				}
			}

			GameControl.HUD.Update();

			GameControl.UpdateRoomState();

			if (GameControl.UpdateRoom) {
				// [Start] Open inventory.
				if (Controls.Start.IsPressed()) {
					GameControl.OpenMenu(GameControl.MenuWeapons);
				}
				
				// Update debug keys.
				GameDebug.UpdateRoomDebugKeys(this);
			}
		}

		public override void Draw(Graphics2D g) {

			// Draw the room.
			g.Translate(0, 16);
			g.Translate(-viewControl.Position);

			// Draw tiles.
			for (int i = 0; i < room.LayerCount; i++) {
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						Tile t = tiles[x, y, i];
						if (t != null)
							t.Draw(g);
					}
				}
			}
			
			// Draw entities.
			g.End();
			g.Begin(GameSettings.DRAW_MODE_BACK_TO_FRONT);
			for (int i = 0; i < entities.Count; ++i) {
				entities[i].Draw(g);
			}
			
			// Draw event tiles.
			g.End();
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			for (int i = 0; i < eventTiles.Count; ++i) {
				eventTiles[i].Draw(g);
			}

			// Draw HUD.
			g.End();
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			g.ResetTranslation();
			GameControl.HUD.Draw(g, false);

			GameControl.DrawRoomState(g);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// The current level that contains the room the player is in.
		public Level Level {
			get { return level; }
		}

		// The current room the player is in.
		public Room Room {
			get { return room; }
		}

		// The player entity (NOTE: this can be null)
		public Player Player {
			get { return GameControl.Player; }
		}

		// Get the list of entities.
		public List<Entity> Entities {
			get { return entities; }
		}

		// Get the size of the room in pixels.
		public Rectangle2I RoomBounds {
			get { return new Rectangle2I(Point2I.Zero, room.Size * GameSettings.TILE_SIZE); }
		}

		// Get the room's location within the level.
		public Point2I RoomLocation {
			get { return roomLocation; }
		}

		public ViewControl ViewControl {
			get { return viewControl; }
		}
	}
}
