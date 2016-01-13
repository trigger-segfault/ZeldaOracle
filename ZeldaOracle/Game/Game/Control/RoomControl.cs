using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Control.Menus;
using ZeldaOracle.Game.Debug;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.Custom;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Control {

	// Handles the main Zelda gameplay within a room.
	public class RoomControl : GameState, ZeldaAPI.Room {

		private Level			level;
		private Room			room;
		private Point2I			roomLocation;
		private Dungeon			dungeon;
		private List<Entity>	entities;
		private Tile[,,]		tiles;
		private List<EventTile>	eventTiles;
		private ViewControl		viewControl;
		private int				requestedTransitionDirection;
		private int				entityCount;
		private RoomGraphics	roomGraphics;

		private event Action<Player>	eventPlayerRespawn;
		private event Action<int>		eventRoomTransitioning;
		private bool			allMonstersDead;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomControl() {
			level			= null;
			room			= null;
			dungeon			= null;
			tiles			= null;
			roomLocation	= Point2I.Zero;
			entities		= new List<Entity>();
			eventTiles		= new List<EventTile>();
			viewControl		= new ViewControl();
			roomGraphics	= new RoomGraphics(this);
			requestedTransitionDirection = 0;
			eventPlayerRespawn		= null;
			eventRoomTransitioning	= null;
			entityCount				= 0;

			dungeon = new Dungeon("dungeon1"); // TODO: remove this.
		}
		

		//-----------------------------------------------------------------------------
		// Tile Accessors
		//-----------------------------------------------------------------------------

		// Return an enumerable list of tiles.
		public IEnumerable<Tile> GetTiles() {
			for (int i = 0; i < room.LayerCount; i++) {
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						Tile tile = tiles[x, y, i];
						if (tile != null && x == tile.Location.X && y == tile.Location.Y)
							yield return tile;
					}
				}
			}
		}
		
		// Return an enumerable list of tiles in the given grid based area.
		public IEnumerable<Tile> GetTilesInArea(Rectangle2I area) {
			Rectangle2I clippedArea = Rectangle2I.Intersect(area, new Rectangle2I(Point2I.Zero, room.Size));
			int minX = clippedArea.Min.X;
			int minY = clippedArea.Min.Y;
			int maxX = clippedArea.Max.X;
			int maxY = clippedArea.Max.Y;

			for (int i = 0; i < room.LayerCount; i++) {
				for (int x = minX; x < maxX; x++) {
					for (int y = minY; y < maxY; y++) {
						Tile tile = tiles[x, y, i];
						if (tile != null) {
							Point2I loc = tile.Location;
							if (!clippedArea.Contains(loc))
								loc = Point2I.Clamp(loc, clippedArea);
							if (x == loc.X && y == loc.Y)
								yield return tile;
						}
					}
				}
			}
		}
		
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

		// inflateAmount inflates the output rectangle.
		public Rectangle2I GetTileAreaFromRect(Rectangle2F rect, int inflateAmount = 0) {
			Rectangle2I area;
			area.Point	= (Point2I) (rect.TopLeft / (float) GameSettings.TILE_SIZE);
			area.Size	= ((Point2I) (rect.BottomRight / (float) GameSettings.TILE_SIZE)) + Point2I.One - area.Point;
			area.Inflate(inflateAmount, inflateAmount);
			return Rectangle2I.Intersect(area, new Rectangle2I(Point2I.Zero, room.Size));
		}

		public EventTile FindEventTile(EventTileDataInstance data) {
			for (int i = 0; i < eventTiles.Count; i++) {
				if (eventTiles[i].EventData == data)
					return eventTiles[i];
			}
			return null;
		}

		public bool IsTileSpawned(TileDataInstance tileDataInstance) {
			return GetTiles().Any(t => t.TileData == tileDataInstance);
		}

		public bool IsEventTileSpawned(EventTileDataInstance eventTileDataInstance) {
			return eventTiles.Any(t => t.EventData == eventTileDataInstance);
		}
		

		//-----------------------------------------------------------------------------
		// Tile & Entity Management
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
		public void PlaceTileOnHighestLayer(Tile tile, Point2I location) {
			int layer = room.LayerCount - 1;
			for (int i = room.LayerCount - 1; i >= 0; i--) {
				if (tiles[location.X, location.Y, i] == null) {
					layer = i;
					break;
				}
			}
			PlaceTile(tile, location, layer);
		}

		// Use this for placing tiles at runtime.
		public void PlaceTile(Tile tile, int x, int y, int layer) {
			for (int xx = 0; xx < tile.Width; xx++) {
				for (int yy = 0; yy < tile.Height; yy++) {
					tiles[x + xx, y + yy, layer] = tile;
				}
			}

			tile.Location = new Point2I(x, y);
			tile.Layer = layer;
			tile.Initialize(this);
		}

		// Use this for placing tiles at runtime.
		public void RemoveTile(Tile tile) {
			// TODO: OnRemove?
			for (int xx = 0; xx < tile.Width; xx++) {
				for (int yy = 0; yy < tile.Height; yy++) {
					tiles[tile.Location.X + xx, tile.Location.Y + yy, tile.Layer] = null;
				}
			}
			tiles[tile.Location.X, tile.Location.Y, tile.Layer] = null;
			tile.IsAlive = false;
		}

		// Put an event tile into the room.
		public void AddEventTile(EventTile eventTile) {
			eventTile.Initialize(this);
			eventTiles.Add(eventTile);
		}

		// Move the given tile to a new location.
		public void MoveTile(Tile tile, Point2I newLocation, int newLayer) {
			for (int xx = 0; xx < tile.Width; xx++) {
				for (int yy = 0; yy < tile.Height; yy++) {
					tiles[tile.Location.X + xx, tile.Location.Y + yy, tile.Layer] = null;
				}
			}
			for (int xx = 0; xx < tile.Width; xx++) {
				for (int yy = 0; yy < tile.Height; yy++) {
					tiles[newLocation.X + xx, newLocation.Y + yy, newLayer] = tile;
				}
			}
			//tiles[tile.Location.X, tile.Location.Y, tile.Layer] = null;
			//tiles[newLocation.X, newLocation.Y, newLayer] = tile;
			tile.Location = newLocation;
			tile.Layer = newLayer;
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
						if (data != null && x == data.Location.X && y == data.Location.Y &&
							data.Properties.GetBoolean("enabled", true))
						{
							tiles[x, y, i] = Tile.CreateTile(data);
							tiles[x, y, i].RoomControl = this;
						}
					}
				}
			}

			// Create the event tiles.
			eventTiles.Capacity = room.EventData.Count;
			for (int i = 0; i < room.EventData.Count; i++) {
				EventTileDataInstance data  = room.EventData[i];
				if (data.Properties.GetBoolean("enabled", true)) {
					EventTile eventTile = EventTile.CreateEvent(data);
					eventTiles.Add(eventTile);
				}
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

			entityCount = entities.Count;
			
			viewControl.Bounds = RoomBounds;
			viewControl.ViewSize = GameSettings.VIEW_SIZE;

			// Fire the RoomStart event.
			GameControl.FireEvent(room, "event_room_start");

			allMonstersDead = false;
		}

		// Set all entities to destroyed (except the player).
		public void DestroyRoom() {
			for (int i = 0; i < entities.Count; i++) {
				if (entities[i] != Player)
					entities[i].IsDestroyed = true;
			}
		}

		// Request to transition to an adjacent room.
		public void RequestRoomTransition(int transitionDirection) {
			requestedTransitionDirection = transitionDirection;
		}

		// Cancel any requested room transitions.
		// This should be called in the event 'RoomTransitioning'
		public void CancelRoomTransition() {
			requestedTransitionDirection = -1;
		}
		
		// Transition to another rooom.
		public void TransitionToRoom(Room nextRoom, RoomTransition transition) {
			TransitionToRoom(nextRoom, transition, null, null, null);
		}
		
		// Transition to another room through warp points.
		public void Warp(WarpEvent startPoint, EventTileDataInstance endPoint) {
			TransitionToRoom(endPoint.Room, 
				startPoint.CreateTransition(endPoint),
				startPoint.CreateExitState(),
				null, endPoint);
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
		
		public void TransitionToRoom(Room nextRoom, RoomTransition transition, GameState exitState, GameState enterState, EventTileDataInstance warpTile) {
			// Create the new room control.
			RoomControl newControl = new RoomControl();
			newControl.gameManager	= gameManager;
			newControl.level		= level;
			newControl.room			= nextRoom;
			newControl.roomLocation	= nextRoom.Location;
			
			//               [Exit]                       [Enter]
			// [RoomOld] -> [RoomOld] -> [Transition] -> [RoomNew] -> [RoomNew]
			
			// Create the sequence of game states for the transition.
			GameState postTransitionState = new GameStateAction(delegate(GameStateAction actionState) {
				gameManager.PopGameState(); // Pop the queue state.
				gameManager.PushGameState(newControl); // Push the new room control state.

				// Find the warp event were warping to and grab its enter-state.
				if (warpTile != null) {
					WarpEvent eventTile = newControl.FindEventTile(warpTile) as WarpEvent;
					if (eventTile != null)
						enterState = eventTile.CreateEnterState();
				}
				if (enterState != null)
					gameManager.PushGameState(enterState); // Push the enter state.
			});
			GameState preTransitionState = new GameStateAction(delegate(GameStateAction actionState) {
				GameControl.RoomControl = newControl;
				gameManager.PopGameState(); // Pop the queue state.
				gameManager.PopGameState(); // Pop the room control state.
				gameManager.PushGameState(new GameStateQueue(transition, postTransitionState));
				newControl.FindEventTile(warpTile);
			});

			if (warpTile != null) {
				transition.NewRoomSetup += delegate(RoomControl roomControl) {
					// Find the warp event were warping to.
					WarpEvent eventTile = newControl.FindEventTile(warpTile) as WarpEvent;
					if (eventTile != null)
						eventTile.SetupPlayerInRoom();
				};
			}

			// Create the game state for the transition sequence.
			GameState state = preTransitionState;
			if (exitState != null)
				state = new GameStateQueue(exitState, preTransitionState);

			// Begin the transition.
			transition.OldRoomControl = this;
			transition.NewRoomControl = newControl;
			gameManager.PushGameState(state);
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		public void OnPlayerRespawn() {
			if (eventPlayerRespawn != null)
				eventPlayerRespawn.Invoke(Player);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			GameControl.RoomControl = this;
			requestedTransitionDirection = 0;
		}
		
		public override void OnEnd() {
			
		}

		private void UpdateObjects() {
			requestedTransitionDirection = -1;

			// Update entities.
			entityCount = entities.Count;
			for (int i = 0; i < entities.Count; i++) {
				if (entities[i].IsAlive && entities[i].IsInRoom && i < entityCount) {
					if (GameControl.UpdateRoom)
						entities[i].Update();
					if (GameControl.AnimateRoom)
						entities[i].UpdateGraphics();

					if (requestedTransitionDirection >= 0)
						break;
				}
			}

			// Remove destroyed entities.
			for (int i = 0; i < entities.Count; i++) {
				if (!entities[i].IsAlive || !entities[i].IsInRoom) {
					entities.RemoveAt(i--);
				}
			}

			if (requestedTransitionDirection >= 0)
				return;
			
			// Update tiles.
			foreach (Tile t in GetTiles())
				t.IsUpdated = false;
			for (int i = 0; i < room.LayerCount; i++) {
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						Tile t = tiles[x, y, i];
						if (t != null && x == t.Location.X && y == t.Location.Y && !t.IsUpdated) {
							t.IsUpdated = true;
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
			
			bool nextAllMonstersDead = AllMonstersDead();
			if (nextAllMonstersDead && !allMonstersDead)
				GameControl.FireEvent(room, "event_all_monsters_dead");
			allMonstersDead = nextAllMonstersDead;
		}

		public override void Update() {
			GameControl.RoomTicks++;

			RoomState roomState		= GameControl.CurrentRoomState;
			GameControl.UpdateRoom	= roomState.UpdateRoom;
			GameControl.AnimateRoom	= roomState.AnimateRoom;

			// Update entities, tiles, and event tiles.
			UpdateObjects();

			// Update view to follow player.
			viewControl.PanTo(Player.Center);
			
			if (requestedTransitionDirection >= 0) {
				// Call the event RoomTransitioning.
				// This event has a chance to cancel the room transition.
				if (eventRoomTransitioning != null) {
					Delegate[] invocationList = eventRoomTransitioning.GetInvocationList();
					for (int i = 0; i < invocationList.Length; i++) {
						invocationList[i].DynamicInvoke(requestedTransitionDirection);
						if (requestedTransitionDirection < 0)
							break;
					}
				}
				if (requestedTransitionDirection >= 0) {
					EnterAdjacentRoom(requestedTransitionDirection);
					requestedTransitionDirection = -1;
				}
			}
			
			GameControl.HUD.Update();
			GameControl.UpdateRoomState();

			if (GameControl.UpdateRoom) {
				// [Start] Open inventory.
				if (Controls.Start.IsPressed()) {
					GameControl.OpenMenu(GameControl.MenuWeapons);
					return;
				}

				// [Select] Open map screen.
				if (Controls.Select.IsPressed()) {
					GameControl.OpenMapScreen();
					return;
				}
				
				// DEBUG: Update debug keys.
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
						if (t != null && x == t.Location.X && y == t.Location.Y)
							t.Draw(g);
					}
				}
			}
			
			// Draw entities.
			g.End();
			g.Begin(GameSettings.DRAW_MODE_ROOM_GRAPHICS);

			// Draw entities in reverse order (because newer entities are drawn below older ones).
			roomGraphics.Clear();
			for (int i = entities.Count - 1; i >= 0; i--)
				entities[i].Draw(roomGraphics);
			roomGraphics.DrawAll(g);
			
			// Draw event tiles.
			g.End();
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			for (int i = 0; i < eventTiles.Count; ++i) {
				eventTiles[i].Draw(g);
			}

			GameDebug.DrawRoom(g, this);

			// Draw HUD.
			g.End();
			g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			g.ResetTranslation();
			GameControl.HUD.Draw(g, false);

			// Draw the current room state.
			GameControl.DrawRoomState(g);
		}
		
		
		//-----------------------------------------------------------------------------
		// Scripting API
		//-----------------------------------------------------------------------------

		public void OpenAllDoors(bool instantaneous = false, bool rememberState = false) {
			foreach (TileDoor door in GetTilesOfType<TileDoor>())
				door.Open(instantaneous, rememberState);
		}

		public void CloseAllDoors(bool instantaneous = false, bool rememberState = false) {
			foreach (TileDoor door in GetTilesOfType<TileDoor>())
				door.Close(instantaneous, rememberState);
		}
		
		public void SetDoorStates(ZeldaAPI.DoorState state, bool rememberState = false) {
			foreach (TileDoor door in GetTilesOfType<TileDoor>()) {
				if (state == ZeldaAPI.DoorState.Opened)
					door.Open(true, rememberState);
				else
					door.Close(true, rememberState);
			}
		}

		public IEnumerable<T> GetTilesOfType<T>() where T : class {
			foreach (Tile tile in GetTiles()) {
				if (tile is T)
					yield return (tile as T);
			}
		}
		
		public void SpawnTile(string id, bool staySpawned = false) {
			foreach (TileDataInstance tileData in Room.GetTiles(id)) {
				if (!IsTileSpawned(tileData)) {
					Tile tile = Tile.CreateTile(tileData);
					PlaceTile(tile, tileData.Location, tileData.Layer);
				}
				if (staySpawned)
					tileData.Properties.Set("enabled", true);
			}
			foreach (EventTileDataInstance eventTileData in Room.EventData) {
				if (!IsEventTileSpawned(eventTileData)) {
					EventTile tile = EventTile.CreateEvent(eventTileData);
					AddEventTile(tile);
				}
				if (staySpawned)
					eventTileData.Properties.Set("enabled", true);
			}
		}
		
		public IEnumerable<T> GetEntitiesOfType<T>() where T : class {
			foreach (Entity entity in entities) {
				T t = entity as T;
				if (t != null)
					yield return t;
			}
		}
		
		public bool AllMonstersDead() {
			return !GetEntitiesOfType<Monster>().Any(m => m.IsAlive);
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

		// Get the number of entities (use this to iterate the entity list).
		public int EntityCount {
			get { return entityCount; }
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

		public Dungeon Dungeon {
			get { return dungeon; }
		}

		// Called after the player respawns.
		public event Action<Player> PlayerRespawn {
			add { eventPlayerRespawn += value; }
			remove { eventPlayerRespawn -= value; }
		}

		// Called as the room is about to transition.
		public event Action<int> RoomTransitioning {
			add { eventRoomTransitioning += value; }
			remove { eventRoomTransitioning -= value; }
		}
	}
}
