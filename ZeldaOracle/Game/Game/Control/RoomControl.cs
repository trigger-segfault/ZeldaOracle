using System;
using System.Collections.Generic;
using System.Linq;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.GameStates.Transitions;
using ZeldaOracle.Game.Debugging;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles.Internal;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Tiles.Custom.Monsters;
using ZeldaOracle.Game.Control.RoomManagers;
using ZeldaOracle.Game.Control.VisualEffects;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game.Control {

	/// <summary>The modes for drawing room layers.</summary>
	[Flags]
	public enum RoomDrawing {
		/// <summary>No room layers will be drawn.</summary>
		None = 0,
		/// <summary>The normal tiles and entities will be drawn.</summary>
		DrawBelow = (1 << 0),
		/// <summary>The above sprites for tiles will be drawn.</summary>
		DrawAbove = (1 << 1),
		/// <summary>Everything will be drawn.</summary>
		DrawAll = DrawBelow | DrawAbove,
	}

	// Handles the main Zelda gameplay within a room.
	public class RoomControl : GameState, ZeldaAPI.Room, IVariableObject {

		private Room				room;
		private Point2I				roomLocation;
		
		private List<ActionTile>	actionTiles;
		private List<TimedEvent>	scheduledEvents;

		private AreaControl			areaControl;
		private ViewControl			viewControl;
		private int					requestedTransitionDirection;

		// Room Managers

		private EntityManager		entityManager;
		private TileManager			tileManager;
		private InteractionManager	interactionManager;
		private RoomPhysics			roomPhysics;
		private RoomGraphics		roomGraphics;

		private bool				allMonstersDead;
		private bool				roomCleared;
		private bool				hasSoftKills;
		private HashSet<Monster>	lingeringMonsters;
		private bool				isUnderwater;
		private bool				isSideScrolling;
		/// <summary>True if the player dies upon falling to the bottom of the room.
		/// </summary>
		private bool				deathOutOfBounds;
		
		private RoomVisualEffect		visualEffect;
		private UnderwaterVisualEffect	visualEffectUnderwater;
		private bool				disableVisualEffect;
		private int					currentRoomTicks;
		private Color				colorOverlay;
		/// <summary>Useful for keeping track of the current room through properties.
		/// </summary>
		private int					roomNumber;

		private event Action<Player>	eventPlayerRespawn;
		private event Action<int>		eventRoomTransitioning;

		private Palette				tilePaletteOverride;
		private Palette				entityPaletteOverride;
		
	
		private class TimedEvent {
			public Action Action { get; set; }
			public int Timer { get; set; }
		}


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomControl() {
			colorOverlay			= Color.Transparent;
			room					= null;
			roomLocation			= Point2I.Zero;
			actionTiles				= new List<ActionTile>();
			scheduledEvents			= new List<TimedEvent>();
			areaControl				= null;
			viewControl				= new ViewControl();
			entityManager			= new EntityManager(this);
			tileManager				= new TileManager(this);
			roomGraphics			= new RoomGraphics(this);
			roomPhysics				= new RoomPhysics(this);
			interactionManager		= new InteractionManager(this);
			requestedTransitionDirection = 0;
			eventPlayerRespawn		= null;
			eventRoomTransitioning	= null;
			isSideScrolling			= false;
			isUnderwater			= false;
			deathOutOfBounds		= false;
			visualEffect			= null;
			disableVisualEffect		= false;
			currentRoomTicks		= 0;
			tilePaletteOverride		= null;
			entityPaletteOverride	= null;

			lingeringMonsters		= new HashSet<Monster>();

			visualEffectUnderwater	= new UnderwaterVisualEffect();
			visualEffectUnderwater.RoomControl = this;
		}
		

		//-----------------------------------------------------------------------------
		// Tile Queries
		//-----------------------------------------------------------------------------

		/// <summary>Return an enumerable list of all grid tiles in the room.</summary>
		public IEnumerable<Tile> GetTiles() {
			return tileManager.GetTiles();
		}
		
		/// <summary>Return an enumerable list of tiles in the given grid based
		/// area.</summary>
		public IEnumerable<Tile> GetTilesInArea(Rectangle2I area) {
			return tileManager.GetTilesInArea(area);
		}
		
		/// <summary>Return an enumerable list of top-most-layer tiles in the given
		/// grid based area.</summary>
		public IEnumerable<Tile> GetTopTilesInArea(Rectangle2I area) {
			return tileManager.GetTopTilesInArea(area);
		}
		
		/// <summary>Return the tile at the given location (can return null).
		/// </summary>
		public Tile GetTile(Point2I location, int layer) {
			return tileManager.GetTile(location, layer);
		}

		/// <summary>Return the tile at the given location (can return null).
		/// </summary>
		public Tile GetTile(int x, int y, int layer) {
			return tileManager.GetTile(x, y, layer);
		}

		/// <summary>Return the tile at the given location that's on the highest layer.
		/// </summary>
		public Tile GetTopTile(int x, int y) {
			return tileManager.GetTopTile(x, y);
		}

		/// <summary>Return the tile at the given location that's on the highest layer.
		/// </summary>
		public Tile GetTopTile(Point2I location) {
			return tileManager.GetTopTile(location);
		}
		
		/// <summary>Return true if the given tile location is inside the room.
		/// </summary>
		public bool IsTileInBounds(Point2I location, int layer = 0) {
			return tileManager.IsTileInBounds(location, layer);
		}

		/// <summary>Return the tile location that the given position in pixels is
		/// situated in.</summary>
		public Point2I GetTileLocation(Vector2F position) {
			return tileManager.GetTileLocation(position);
		}

		/// <summary>inflateAmount inflates the output rectangle.</summary>
		public Rectangle2I GetTileAreaFromRect(Rectangle2F rect,
			int inflateAmount = 0)
		{
			return tileManager.GetTileAreaFromRect(rect, inflateAmount);
		}

		public ActionTile FindActionTile(ActionTileDataInstance data) {
			for (int i = 0; i < actionTiles.Count; i++) {
				if (actionTiles[i].ActionData == data)
					return actionTiles[i];
			}
			return null;
		}

		public bool IsTileSpawned(TileDataInstance tileDataInstance) {
			return GetTiles().Any(t => t.TileDataOwner == tileDataInstance);
		}

		public bool IsActionTileSpawned(
			ActionTileDataInstance actionTileDataInstance)
		{
			return actionTiles.Any(t => t.ActionData == actionTileDataInstance);
		}
		

		//-----------------------------------------------------------------------------
		// Tile & Entity Management
		//-----------------------------------------------------------------------------
		
		/// <summary>Initialize and spawn an entity, and have it be managed by the
		/// RoomControl.</summary>
		public void SpawnEntity(Entity entity) {
			entityManager.SpawnEntity(entity);
		}
		
		/// <summary>Initialize and spawn an entity at the given position, and have it
		/// be managed by the RoomControl.</summary>
		public void SpawnEntity(Entity entity, Vector2F position) {
			entityManager.SpawnEntity(entity, position);
		}
		
		/// <summary>Initialize and spawn an entity at the given position, and have it
		/// be managed by the RoomControl.</summary>
		public void SpawnEntity(Entity entity, Vector2F position, float zPosition) {
			entityManager.SpawnEntity(entity, position, zPosition);
		}
		
		/// <summary>Spawn a tile if it isn't already spawned.</summary>
		public void SpawnTile(TileDataInstance tileData, bool staySpawned) {
			if (!IsTileSpawned(tileData)) {
				TileSpawnOptions spawnOptions = tileData.SpawnOptions;
				if (spawnOptions.PoofEffect) {
					Tile tile = new AppearingTile(tileData, spawnOptions);
					PlaceTile(tile, tileData.Location, tileData.Layer);
				}
				else {
					Tile tile = Tile.CreateTile(tileData);
					PlaceTile(tile, tileData.Location, tileData.Layer);
				}
			}
			if (staySpawned)
				tileData.Properties.Set("enabled", true);
		}

		/// <summary>Despawn a tile if it's already spawned.</summary>
		public void DespawnTile(Tile tile, bool stayDespawned) {
			if (IsTileSpawned(tile.TileData)) {
				TileSpawnOptions spawnOptions = tile.TileData.SpawnOptions;
				if (spawnOptions.PoofEffect) {
					// Spawn the poof effect.
					if (spawnOptions.PoofEffect) {
						Point2I size = tile.Size;
						for (int x = 0; x < size.X; x++) {
							for (int y = 0; y < size.Y; y++) {
								Effect effect = new Effect(
									GameData.ANIM_EFFECT_BLOCK_POOF,
									DepthLayer.EffectBlockPoof);
								Vector2F pos = (tile.Location + new Point2I(x, y) +
									Vector2F.Half) * GameSettings.TILE_SIZE;
								SpawnEntity(effect, pos);
							}
						}
						AudioSystem.PlaySound(GameData.SOUND_APPEAR_VANISH);
					}
				}
				RemoveTile(tile);
			}
		}
		
		/// <summary>Spawn an action tile if it isn't already spawned.</summary>
		public void SpawnActionTile(
			ActionTileDataInstance actionTileData, bool staySpawned)
		{
			if (!IsActionTileSpawned(actionTileData)) {
				ActionTile tile = ActionTile.CreateAction(actionTileData);
				AddActionTile(tile);
			}
			if (staySpawned)
				actionTileData.Properties.Set("enabled", true);
		}

		/// <summary>Place a tile in the tile grid at the given location and layer.
		/// </summary>
		public void PlaceTile(Tile tile, Point2I location, int layer,
			bool initializeTile = true)
		{
			tileManager.PlaceTile(tile, location, layer, initializeTile);
		}
		
		/// <summary>Place a tile in highest empty layer at the given location.
		/// Returns true if there was an empty space to place the tile.</summary>
		public bool PlaceTileOnHighestLayer(Tile tile, Point2I location) {
			return tileManager.PlaceTileOnHighestLayer(tile, location);
		}

		/// <summary>Use this for placing tiles at runtime.</summary>
		public void PlaceTile(Tile tile, int x, int y, int layer,
			bool initializeTile = true)
		{
			tileManager.PlaceTile(tile, x, y, layer, initializeTile);
		}

		/// <summary>Remove a tile from the room.</summary>
		public void RemoveTile(Tile tile) {
			tileManager.RemoveTile(tile);
		}

		/// <summary>Put an action tile into the room.</summary>
		public void AddActionTile(ActionTile actionTile) {
			actionTile.Initialize(this);
			actionTiles.Add(actionTile);
		}

		/// <summary>Move the given tile to a new location.</summary>
		public void MoveTile(Tile tile, Point2I newLocation, int newLayer) {
			tileManager.MoveTile(tile, newLocation, newLayer);
		}

		public void ScheduleEvent(int delay, Action action) {
			scheduledEvents.Add(new TimedEvent() {
				Timer = delay,
				Action = action,
			});
		}

		/// <summary>Marks the room as containing monsters with soft kills,
		/// preventing it from being cleared.</summary>
		public void AddSoftKill(Monster monster) {
			// Only set if monster counts towards respawn clearing
			if (!monster.IgnoreMonster && !monster.IgnoreRespawn)
				hasSoftKills = true;
		}

		/// <summary>Adds a lingering monster that can keep its presense after death.</summary>
		public void AddLingeringMonster(Monster monster) {
			lingeringMonsters.Add(monster);
		}

		/// <summary>Remove a lingering monster so that it no longer keeps its presense.</summary>
		public void RemoveLingeringMonster(Monster monster) {
			lingeringMonsters.Remove(monster);
		}


		//-----------------------------------------------------------------------------
		// Room Setup & Destroy
		//-----------------------------------------------------------------------------

		public void BeginRoom() {
			BeginRoom(room);
		}

		public void BeginRoom(List<Entity> persistentEntities) {
			BeginRoom(room, persistentEntities);
		}

		public void BeginRoom(Room room) {
			List<Entity> persistentEntities = new List<Entity>();
			BeginRoom(room, persistentEntities);
		}

		/// <summary>Setup the room with the given list of initial entities to spawn.
		/// </summary>
		public void BeginRoom(Room room, List<Entity> entities) {
			this.room			= room;
			roomLocation		= room.Location;
			isSideScrolling		= room.Zone.IsSideScrolling;
			isUnderwater		= room.Zone.IsUnderwater;
			deathOutOfBounds	= room.DeathOutOfBounds;
			roomNumber			= GameControl.NextRoomNumber();
			if (isUnderwater)
				visualEffect = visualEffectUnderwater;
			else
				visualEffect = null;

			if (this.areaControl == null)
				areaControl = GameControl.GetAreaControl(Area);
			
			if (room.Area != null) {
				if (room.Area.Music != null)
					AudioSystem.PlayMusic(room.Area.Music);
				else
					AudioSystem.StopMusic();
			}

			viewControl.RoomBounds = RoomBounds;
			viewControl.SetTarget(Player);

			// Monster respawning
			RespawnManager.VisitRoom(room);
			/*if (RespawnManager.VisitRoom(room)) {
				//room.OnRespawn();
			}*/

			// Discover the room
			room.IsDiscovered = true;
			room.Level.IsDiscovered = true;

			// Clear the action tiles
			actionTiles.Clear();

			// Setup the entity manager with the initial entities
			entityManager.BeginRoom(entities);

			// Create the tile grid
			tileManager.Initialize(room);
			foreach (TileDataInstance data in room.GetTiles(true)) {
				Tile tile = null;
				if (data.IsModifiedEnabled)
					tile = Tile.CreateTile(data);
				else if (data.Layer == 0 && data.TileBelow != null)
					tile = Tile.CreateTile(data.TileBelow);
				else if (data.Layer == 0 && Zone.DefaultTileData != null)
					tile = Tile.CreateTile(Zone.DefaultTileData);
				if (tile != null)
					PlaceTile(tile, data.Location, data.Layer, false);
			}

			// Create the action tiles
			actionTiles.Capacity = room.ActionCount;
			foreach (ActionTileDataInstance data in room.GetActionTiles(true)) {
				if (data.IsModifiedEnabled) {
					ActionTile actionTile = ActionTile.CreateAction(data);
					actionTiles.Add(actionTile);
				}
			}
			
			// Initialize grid tiles and action tiles
			tileManager.InitializeTiles();
			
			// Initialize the action tiles
			for (int i = 0; i < actionTiles.Count; i++)
				actionTiles[i].Initialize(this);
			tileManager.PostInitializeTiles();
			
			// Setup view
			viewControl.CenterOnTarget();
			
			// Assign the area control which calls begin and end events if areas
			// have been changed.
			GameControl.AreaControl = areaControl;

			// Fire the RoomStart event
			GameControl.FireEvent(room, "room_start");

			allMonstersDead = false;
			roomCleared = IsRoomRespawnCleared();

			currentRoomTicks = 0;
		}

		/// <summary>Called when the room is left.</summary>
		public void DestroyRoom() {
			foreach (Tile tile in GetTiles())
				tile.OnRemoveFromRoom();
			foreach (ActionTile actionTile in actionTiles)
				actionTile.OnRemoveFromRoom();
			entityManager.LeaveRoom();
			GameControl.OnLeaveRoom(this);
		}
		

		//-----------------------------------------------------------------------------
		// Room Transitions
		//-----------------------------------------------------------------------------

		/// <summary> Request to transition to an adjacent room.</summary>
		public void RequestRoomTransition(int transitionDirection) {
			requestedTransitionDirection = transitionDirection;
		}

		/// <summary>Cancel any requested room transitions. This should be called in
		/// the event 'RoomTransitioning'</summary>
		public void CancelRoomTransition() {
			requestedTransitionDirection = -1;
		}
		
		/// <summary>Transition to another rooom.</summary>
		public void TransitionToRoom(Room nextRoom, RoomTransition transition) {
			TransitionToRoom(nextRoom, transition, null, null, null);
		}
		
		/// <summary>Transition to another room through warp points.</summary>
		public void Warp(WarpAction startPoint, ActionTileDataInstance endPoint) {
			TransitionToRoom(endPoint.Room, 
				startPoint.CreateTransition(endPoint),
				startPoint.CreateExitState(),
				null, endPoint, startPoint.ShowAreaName);
		}

		/// <summary>Transition to a room adjacent to the current one.</summary>
		public void EnterAdjacentRoom(Direction direction) {
			Point2I nextLocation = roomLocation + direction.ToPoint();

			// Transition to the room
			if (Level.ContainsRoom(nextLocation)) {
				Room nextRoom = Level.GetRoomAt(nextLocation);
				TransitionToRoom(nextRoom, new RoomTransitionPush(direction));
			}
		}
		
		public void TransitionToRoom(Room nextRoom, RoomTransition transition,
			GameState exitState, GameState enterState, ActionTileDataInstance warpTile,
			bool showAreaName = false)
		{
			// Create the new room control
			RoomControl newControl = new RoomControl();
			newControl.gameManager	= gameManager;
			newControl.room			= nextRoom;
			newControl.roomLocation	= nextRoom.Location;
			newControl.areaControl  = GameControl.GetAreaControl(nextRoom.Area);

			// Also set this here to prevent flickering of small keys in HUD
			//newControl.dungeon = nextRoom.Dungeon;

			//               [Exit]                       [Enter]
			// [RoomOld] -> [RoomOld] -> [Transition] -> [RoomNew] -> [RoomNew]
			
			// Create the sequence of game states for the transition
			GameState postTransitionState = new GameStateAction(
				delegate(GameStateAction actionState)
			{
				gameManager.PopGameState(); // Pop the queue state
				gameManager.PushGameState(newControl); // Push the new room control state

				// Find the warp action were warping to and grab its enter-state
				if (warpTile != null) {
					WarpAction actionTile =
						newControl.FindActionTile(warpTile) as WarpAction;
					// Create a dummy warp tile for deactivated warps
					if (actionTile == null) {
						actionTile = ActionTile.CreateAction(warpTile) as WarpAction;
						if (actionTile != null)
							actionTile.Initialize(newControl);
					}
					if (actionTile != null)
						enterState = actionTile.CreateEnterState();
				}

				GameStateQueue queue = new GameStateQueue();

				// Push the enter state
				if (enterState != null)
					queue.Add(enterState);

				// Show the area message
				if (showAreaName) {
					queue.Add(new GameStateAction(delegate {
						string message = newControl.areaControl.EnterMessage;
						TextReaderArgs args = new TextReaderArgs() {
							Spacing = 2
						};
						GameControl.DisplayMessage(message, args);
					}));
				}

				if (queue.Count != 0)
					gameManager.PushGameState(queue);
			});
			GameState preTransitionState = new GameStateAction(
				delegate(GameStateAction actionState)
			{
				GameControl.RoomControl = newControl;
				gameManager.PopGameState(); // Pop the queue state
				gameManager.PopGameState(); // Pop the room control state
				gameManager.PushGameState(
					new GameStateQueue(transition, postTransitionState));
				newControl.FindActionTile(warpTile);
			});

			if (warpTile != null) {
				transition.NewRoomSetup += delegate(RoomControl roomControl) {
					// Find the warp action were warping to
					WarpAction actionTile =
						newControl.FindActionTile(warpTile) as WarpAction;
					// Create a dummy warp tile for deactivated warps
					if (actionTile == null) {
						actionTile = ActionTile.CreateAction(warpTile) as WarpAction;
						if (actionTile != null)
							actionTile.Initialize(newControl);
					}
					if (actionTile != null)
						actionTile.SetupPlayerInRoom();
				};
			}

			// Create the game state for the transition sequence
			GameState state = preTransitionState;
			if (exitState != null)
				state = new GameStateQueue(exitState, preTransitionState);
		
			// Begin the transition
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
			requestedTransitionDirection = 0;
		}
		
		public override void OnEnd() {
		}

		private void UpdateObjects() {
			requestedTransitionDirection = -1;
			
			if (GameControl.UpdateRoom) {
				// Update the event schedule
				for (int i = 0; i < scheduledEvents.Count; i++) {
					scheduledEvents[i].Timer--;
					if (scheduledEvents[i].Timer <= 0) {
						scheduledEvents[i].Action?.Invoke();
						scheduledEvents.RemoveAt(i--);
					}
				}

				// Update entities and tiles
				entityManager.UpdateEntities();
				tileManager.UpdateTiles();
				for (int i = 0; i < actionTiles.Count; i++)
					actionTiles[i].Update();

				interactionManager.ProcessInteractions();
				roomPhysics.ProcessPhysics();

				// Check if the player wants to room transition
				Player.CheckRoomTransitions();
			}

			// Update tile and entity graphics
			if (GameControl.AnimateRoom) {
				tileManager.UpdateTileGraphics();
				entityManager.UpdateEntityGraphics();
			}

			// Room clearing is slightly different from all monsters dead as some
			// monsters count as dead for respawn purposes when children remain.
			// I.E. Only one Red Zole's Gel is left.
			if (!roomCleared && IsRoomRespawnCleared()) {
				roomCleared = true;
				RespawnManager.ClearRoom(room);
			}

			bool nextAllMonstersDead = AllMonstersDead();
			if (nextAllMonstersDead && !allMonstersDead) {
				GameControl.FireEvent(room, "all_monsters_dead");
			}
			allMonstersDead = nextAllMonstersDead;
		}

		public override void Update() {
			GameControl.RoomTicks++;
			currentRoomTicks++;

			RoomState roomState		= GameControl.CurrentRoomState;
			GameControl.UpdateRoom	= roomState.UpdateRoom;
			GameControl.AnimateRoom	= roomState.AnimateRoom;

			viewControl.ShakeOffset = Vector2F.Zero;
			
			// Update the current visual effect
			if (visualEffect != null && !disableVisualEffect)
				visualEffect.Update();
			
			// Update entities, tiles, and action tiles
			UpdateObjects();

			// Update view to follow player
			//viewControl.PanTo(Player.DrawCenter + Player.ViewFocusOffset);
			
			if (requestedTransitionDirection >= 0) {
				// Call the event RoomTransitioning
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
			
			// Update HUD and current room state
			GameControl.HUD.Update();
			GameControl.UpdateRoomState();

			// Update scripts
			GameControl.UpdateScripts();

			if (GameControl.UpdateRoom) {
				// [Start] Open inventory
				if (Controls.Start.IsPressed())
					GameControl.OpenMenu(GameControl.MenuWeapons);
				// [Select] Open map screen
				else if (Controls.Select.IsPressed())
					GameControl.OpenMapScreen();
			}

			ViewControl.UpdatMovement();
		}

		public void DrawRoom(Graphics2D g, Vector2F position,
			RoomDrawing roomDrawing)
		{
			g.PushTranslation(position);

			if (roomDrawing.HasFlag(RoomDrawing.DrawBelow)) {
				// Draw background (in the color of the HUD).
				Rectangle2I viewRect = new Rectangle2I(0, 0,
					GameSettings.VIEW_WIDTH, GameSettings.VIEW_HEIGHT);
				g.DrawSprite(GameData.SPR_HUD_BACKGROUND, viewRect);
			}

			Vector2F viewTranslation = -GameUtil.Bias(viewControl.Camera.TopLeft);
			g.PushTranslation(viewTranslation);

			if (roomDrawing.HasFlag(RoomDrawing.DrawBelow)) {
				StartVisualEffect(g, position);

				// Draw tiles.
				roomGraphics.Clear();
				tileManager.DrawTiles(roomGraphics);
				roomGraphics.DrawAll(g);

				EndVisualEffect(g, position);

				// DEBUG: Draw debug information over tiles.
				GameDebug.DrawRoomTiles(g, this);

				// Draw entities in reverse order (because newer entities are drawn below older ones).
				roomGraphics.Clear();
				tileManager.DrawEntityTiles(roomGraphics);
				for (int i = entityManager.Entities.Count - 1; i >= 0; i--)
					entityManager.Entities[i].Draw(roomGraphics);
				roomGraphics.SortDepthLayer(DepthLayer.PlayerAndNPCs); // Sort dynamic depth layers.
				roomGraphics.DrawAll(g);

				// Draw action tiles in reverse order.
				for (int i = actionTiles.Count - 1; i >= 0; i--)
					actionTiles[i].Draw(g);
			}
			if (roomDrawing.HasFlag(RoomDrawing.DrawAbove)) {
				// Draw the tile parts that display above the player and all entities
				StartVisualEffect(g, position);

				// Draw above tiles.
				roomGraphics.Clear();
				tileManager.DrawTilesAbove(roomGraphics);
				roomGraphics.DrawAll(g);

				EndVisualEffect(g, position);

				if (!colorOverlay.IsTransparent)
					g.FillRectangle(GameSettings.SCREEN_BOUNDS, colorOverlay);
			}

			// DEBUG: Draw debug information.
			GameDebug.DrawRoom(g, this);

			g.PopTranslation(2); // position + viewTranslation
		}

		public override void AssignPalettes() {
			GameData.SHADER_PALETTE.TilePalette = TilePalette;
			GameData.SHADER_PALETTE.EntityPalette = EntityPalette;
		}

		public void AssignLerpPalettes() {
			GameData.SHADER_PALETTE.LerpTilePalette = TilePalette;
			GameData.SHADER_PALETTE.LerpEntityPalette = EntityPalette;
		}

		public override void Draw(Graphics2D g) {
			// Draw the room (offset to make room for the HUD)
			DrawRoom(g, new Vector2F(0, GameSettings.HUD_HEIGHT), RoomDrawing.DrawAll);
			GameControl.HUD.Draw(g);
			GameControl.DrawRoomState(g);
		}

		private void StartVisualEffect(Graphics2D g, Vector2F position) {
			// If drawing a visual effect over the room, then
			// begin rendering to the temp render target
			if (visualEffect != null && !disableVisualEffect) {
				visualEffect.Begin(g, position);
				//g.End();
				//g.PopTranslation();
				//g.SetRenderTarget(GameData.RenderTargetGameTemp);
				//g.Clear(Color.Transparent);
				//g.Begin(GameSettings.DRAW_MODE_DEFAULT);
			}
		}

		private void EndVisualEffect(Graphics2D g, Vector2F position) {
			// Now render the visual effect.
			if (visualEffect != null && !disableVisualEffect) {
				visualEffect.End(g, position);
				//g.End();
				//g.PushTranslation(position);
				//g.SetRenderTarget(GameData.RenderTargetGame);
				//g.Begin(GameSettings.DRAW_MODE_DEFAULT);
				//visualEffect.Render(g, GameData.RenderTargetGameTemp);
			}
		}
		
		
		//-----------------------------------------------------------------------------
		// Scripting API
		//-----------------------------------------------------------------------------

		public void OpenAllDoors(bool instantaneous = false,
			bool rememberState = false)
		{
			foreach (TileDoor door in GetTilesOfType<TileDoor>())
				door.Open(instantaneous, rememberState);
		}

		public void CloseAllDoors(bool instantaneous = false,
			bool rememberState = false)
		{
			foreach (TileDoor door in GetTilesOfType<TileDoor>())
				door.Close(instantaneous, rememberState);
		}
		
		public void SetDoorStates(DoorState state, bool rememberState = false) {
			foreach (TileDoor door in GetTilesOfType<TileDoor>()) {
				if (state == DoorState.Opened)
					door.Open(true, rememberState);
				else
					door.Close(true, rememberState);
			}
		}

		public IEnumerable<T> GetTilesOfType<T>() where T : class {
			foreach (Tile tile in GetTiles()) {
				if (tile is T)
					yield return tile as T;
			}
		}

		public void SpawnTile(string id, bool staySpawned = false) {
			foreach (TileDataInstance tileData in Room.FindTilesByID(id, true))
				SpawnTile(tileData, staySpawned);
			foreach (ActionTileDataInstance actionTileData in Room.GetActionTiles(true))
				SpawnActionTile(actionTileData, staySpawned);
		}

		public void DespawnTile(string id, bool stayDespawned = false) {
			foreach (Tile tile in TileManager.GetTiles()) {
				if (tile.TileData.ID == id)
					DespawnTile(tile, stayDespawned);
			}
		}

		public ZeldaAPI.Tile GetTileById(string id) {
			foreach (Tile tile in GetTiles()) {
				if (tile.Properties.Get("id", "") == id)
					return (tile as ZeldaAPI.Tile);
			}
			return null;
		}
		
		public IEnumerable<T> GetEntitiesOfType<T>() where T : class {
			foreach (Entity entity in entityManager.AliveEntities) {
				T t = entity as T;
				if (t != null)
					yield return t;
			}
		}

		/// <summary>Returns true if all monsters that are not ignorable are gone
		/// from the room.</summary>
		public bool AllMonstersDead() {
			return	!lingeringMonsters.Any(m => m.NeedsClearing) &&
					!GetEntitiesOfType<Monster>().Any(m => m.NeedsClearing) &&
					!GetTilesOfType<TileMonster>().Any(t => t.NeedsClearing);
		}

		/// <summary>A deviation of AllMonstersDead used only for marking the room as
		/// cleared even when monsters remain.</summary>
		public bool IsRoomRespawnCleared() {
			return	!hasSoftKills &&
					!lingeringMonsters.Any(m => m.NeedsClearing ||
					m.IgnoreRespawn) &&
					!GetEntitiesOfType<Monster>().Any(m => m.NeedsClearing ||
					m.IgnoreRespawn) &&
					!GetTilesOfType<TileMonster>().Any(t => t.NeedsClearing);
		}

		/// <summary>Checks if a monster with the specified ID is cleared.
		/// Monster IDs are unique to each "root" room.</summary>
		public bool IsMonsterDead(int monsterID) {
			return RespawnManager.IsMonsterDead(room, monsterID);
		}

		/// <summary>Registers a monster with the specified ID as cleared.
		/// Monster IDs are unique to each "root" room.</summary>
		public void ClearMonster(int monsterID) {
			RespawnManager.ClearMonster(room, monsterID);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the current area control.</summary>
		public AreaControl AreaControl {
			get { return GameControl.AreaControl; }
		}

		/// <summary>The current area.</summary>
		public Area Area {
			get { return room.Area; }
		}

		/// <summary>The current level.</summary>
		public Level Level {
			get { return room.Level; }
		}

		/// <summary>Get the current room.</summary>
		public Room Room {
			get { return room; }
		}

		/// <summary>Get the player entity.</summary>
		public Player Player {
			get { return GameControl.Player; }
		}
		
		/// <summary>Get the list of entities.</summary>
		public List<Entity> Entities {
			get { return entityManager.Entities; }
		}
		
		/// <summary>Iterate a list of entities which are alive and in the current
		/// room.</summary>
		public IEnumerable<Entity> AliveEntities {
			get { return entityManager.AliveEntities; }
		}

		/// <summary>Get the size of the current room in pixels.</summary>
		public Rectangle2I RoomBounds {
			get { return room.Bounds; }
		}

		/// <summary>Get the current room's location within the level.</summary>
		public Point2I RoomLocation {
			get { return roomLocation; }
		}

		public ViewControl ViewControl {
			get { return viewControl; }
		}

		public Camera Camera {
			get { return viewControl.Camera; }
		}

		/// <summary>The room's entity manager.</summary>
		public EntityManager EntityManager {
			get { return entityManager; }
		}

		/// <summary>The room's grid-tile manager.</summary>
		public TileManager TileManager {
			get { return tileManager; }
		}

		/// <summary>The room's physics manager.</summary>
		public RoomPhysics RoomPhysics {
			get { return roomPhysics; }
		}

		/// <summary>The room's interaction manager.</summary>
		public InteractionManager InteractionManager {
			get { return interactionManager; }
		}

		/// <summary>Gets the manager for clearing and respawning rooms.</summary>
		public RespawnManager RespawnManager {
			get { return areaControl.RespawnManager; }
		}

		/// <summary>Called after the player respawns.</summary>
		public event Action<Player> PlayerRespawn {
			add { eventPlayerRespawn += value; }
			remove { eventPlayerRespawn -= value; }
		}

		/// <summary>Called as the room is about to transition.</summary>
		public event Action<int> RoomTransitioning {
			add { eventRoomTransitioning += value; }
			remove { eventRoomTransitioning -= value; }
		}

		/// <summary>True if the current room is a side-scrolling room.</summary>
		public bool IsSideScrolling {
			get { return isSideScrolling; }
			set { isSideScrolling = value; }
		}

		/// <summary>True if the current room is underwater.</summary>
		public bool IsUnderwater {
			get { return isUnderwater; }
			set { isUnderwater = value; }
		}

		public bool DisableVisualEffect {
			get { return disableVisualEffect; }
			set { disableVisualEffect = value; }
		}

		public Zone Zone {
			get { return room.Zone; }
		}

		public Palette TilePalette {
			get { return tilePaletteOverride ?? Zone.Palette; }
		}

		public Palette EntityPalette {
			get { return entityPaletteOverride ?? GameData.PAL_ENTITIES_DEFAULT; }
		}

		public Palette TilePaletteOverride {
			get { return tilePaletteOverride; }
			set {
				tilePaletteOverride = value;
				if (tilePaletteOverride != null &&
					tilePaletteOverride.PaletteType != PaletteTypes.Tile)
					throw new ArgumentException("Palette is not a tile palette!");
			}
		}

		public Palette EntityPaletteOverride {
			get { return entityPaletteOverride; }
			set {
				entityPaletteOverride = value;
				if (entityPaletteOverride != null &&
					entityPaletteOverride.PaletteType != PaletteTypes.Entity)
					throw new ArgumentException("Palette is not an entity palette!");
			}
		}

		/// <summary>Elapsed ticks since the room was entered.</summary>
		public int CurrentRoomTicks {
			get { return currentRoomTicks; }
		}
		
		/// <summary>True if the player dies upon falling to the bottom of the room.
		/// </summary>
		public bool DeathOutOfBounds {
			get { return deathOutOfBounds; }
			set { deathOutOfBounds = value; }
		}

		/// <summary>Useful for keeping track of the current room through properties.
		/// </summary>
		public int RoomNumber {
			get { return roomNumber; }
		}

		/// <summary>Gets if the room is cleared in the respawn manager.</summary>
		public bool IsCleared {
			get { return RespawnManager.IsRoomCleared(room); }
		}

		public Color OverlayColor {
			get { return colorOverlay; }
			set { colorOverlay = value; }
		}

		public RoomVisualEffect VisualEffect {
			get { return visualEffect; }
			set {
				if (value == null && isUnderwater)
					visualEffect = visualEffectUnderwater;
				else if (value != null) {
					visualEffect = value;
					visualEffect.RoomControl = this;
				}
				else
					visualEffect = null;
			}
		}

		/// <summary>Gets the variables for the room.</summary>
		public Variables Variables {
			get { return room.Variables; }
		}

		/// <summary>Gets the variables for the API Object.</summary>
		ZeldaAPI.Variables ZeldaAPI.ApiObject.Vars {
			get { return room.Variables; }
		}
	}
}
