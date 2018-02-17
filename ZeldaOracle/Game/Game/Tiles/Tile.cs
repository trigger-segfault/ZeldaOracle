using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Drops;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Projectiles.PlayerProjectiles;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Monsters;

namespace ZeldaOracle.Game.Tiles {

	public class Tile : IEventObject, ZeldaAPI.Tile {
		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private Rectangle2I			tileGridArea;

		private TileGraphicsComponent graphics;

		// Internal
		private RoomControl			roomControl;
		private bool				isAlive;
		private bool				isInitialized;
		private Point2I				location;			// The tile location in the room.
		private int					layer;				// The layer this tile is in.
		private Point2I				moveDirection;
		private int					moveDistance;
		private int					currentMoveDistance;
		private bool				isMoving;
		private float				movementSpeed;
		private Vector2F			offset;				// Offset in pixels from its tile location (used for movement).

		private Vector2F			previousOffset;
		private Point2I				previousLocation;
		private Tile				surfaceTile;

		private bool				hasMoved;
		protected TilePath			path;				// The path the tile is currently following.
		private int					pathTimer;
		private int					pathMoveIndex;
		protected bool				fallsInHoles;
		private Vector2F			velocity;
		protected Sound				soundMove;
		private Vector2F			conveyorVelocity;
		private Rectangle2I         hurtArea;
		private int                 hurtDamage;

		// Settings
		private TileDataInstance	tileData;			// The tile data used to create this tile.
		private TileFlags			flags;
		private Point2I				size;				// How many tile spaces this tile occupies. NOTE: this isn't supported yet.
		private ISprite				spriteAsObject;		// The sprite for the tile if it were picked up, pushed, etc.
		private Animation			breakAnimation;		// The animation to play when the tile is broken.
		private Sound				breakSound;			// The sound to play when the tile is broken.
		private int					pushDelay;			// Number of ticks of pushing before the player can move this tile.
		private DropList			dropList;
		private Properties			properties;
		
		private bool				isSolid;
		private CollisionModel		collisionModel;
		private CollisionStyle		collisionStyle;

		private bool				cancelBreakSound;
		private bool				cancelBreakEffect;
		
		public bool IsUpdated { get; set; } // This is to make sure tiles are only updated once per frame.


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		// Use Tile.CreateTile() instead of this constructor.
		protected Tile() {
			tileGridArea	= Rectangle2I.Zero;
			isAlive				= false;
			isInitialized		= false;
			location			= Point2I.Zero;
			layer				= 0;
			offset				= Point2I.Zero;
			size				= Point2I.One;
			spriteAsObject      = null;
			isSolid				= false;
			isMoving			= false;
			pushDelay			= 20;
			properties			= new Properties(this);
			tileData            = new TileDataInstance(new TileData());
			moveDirection		= Point2I.Zero; 
			dropList			= null;
			hasMoved			= false;
			path				= null;
			pathTimer			= 0;
			pathMoveIndex		= 0;
			fallsInHoles		= true;
			soundMove			= GameData.SOUND_BLOCK_PUSH;
			conveyorVelocity	= Vector2F.Zero;
			surfaceTile			= null;
			collisionStyle		= CollisionStyle.Rectangular;
			graphics			= new TileGraphicsComponent(this);
			cancelBreakSound	= false;
			cancelBreakEffect	= false;
		}


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------
		
		public void Initialize(RoomControl control) {
			this.roomControl	= control;
			this.isAlive		= true;

			if (!isInitialized) {
				isInitialized	= true;
				hasMoved		= false;
				velocity		= Vector2F.Zero;

				// Leap ledges use constant collision models to prevent any positioning issues.
				if (IsLeapLedge)
					collisionModel = GameData.MODEL_LEAP_LEDGES[LedgeDirection];

				// Begin a path if there is one.
				string pathString = properties.GetString("path", "");
				TilePath p = TilePath.Parse(pathString);
				BeginPath(p);

				// Set the solid state.
				isSolid = (SolidType != TileSolidType.NotSolid);

				// Setup default drop list.
				if (IsDigable && !IsSolid)
					dropList = RoomControl.GameControl.DropManager.GetDropList("dig");
				else
					dropList = RoomControl.GameControl.DropManager.GetDropList("default");

				// Call the virtual initialization method.
				OnInitialize();
			}
		}


		//-----------------------------------------------------------------------------
		// Flags
		//-----------------------------------------------------------------------------

		// Returns true if the tile has the normal flags.
		public bool HasFlag(TileFlags flags) {
			return Flags.HasFlag(flags);
		}
		
		public void SetFlags(TileFlags flagsToSet, bool enabled) {
			if (enabled)
				flags |= flagsToSet;
			else
				flags &= ~flagsToSet;
		}
		

		//-----------------------------------------------------------------------------
		// Movement
		//-----------------------------------------------------------------------------

		// Begin following a path.
		public void BeginPath(TilePath path) {
			this.path = path;
			pathMoveIndex = 0;
			pathTimer = 0;
		}

		// Move over a distance.
		protected bool Move(int direction, int distance, float movementSpeed) {
			if (isMoving)
				return false;

			int newLayer;
			if (IsMoveObstructed(direction, out newLayer))
				return false;

			this.movementSpeed	= movementSpeed;
			this.moveDistance	= distance;
			this.moveDirection	= Directions.ToPoint(direction);
			this.isMoving		= true;
			this.hasMoved		= true;
			this.currentMoveDistance	= 0;
			
			// Move the tile one step forward.
			Point2I oldLocation = location;
			RoomControl.MoveTile(this, location + moveDirection, newLayer);
			offset = -Directions.ToVector(direction) * GameSettings.TILE_SIZE;

			return true;
		}

		protected bool IsMoveObstructed(int direction, out int newLayer) {
			Point2I newLocation = location + Directions.ToPoint(direction);
			return IsMoveObstructed(newLocation, out newLayer);
		}

		protected bool IsMoveObstructed(Point2I newLocation, out int newLayer) {
			newLayer = -1;

			Rectangle2I newGridArea = tileGridArea;
			newGridArea.Point += newLocation - location;

			// Check if the move will keep the tile in the room bounds.
			if (!roomControl.TileManager.GridArea.Contains(newGridArea))
				return true;

			// Check if there is a free grid area to move to.
			int highestLayer = 0;
			foreach (Tile tile in roomControl.TileManager.GetTilesInArea(newGridArea)) {
				if (tile != this) {
					if (!tile.IsCoverableByBlock)
						return true;
					else if (tile.Layer > highestLayer)
						highestLayer = tile.Layer;
				}
			}
			newLayer = highestLayer + 1;
			return (newLayer < 0 || newLayer >= roomControl.TileManager.LayerCount);
		}


		//-----------------------------------------------------------------------------
		// Interaction Methods
		//-----------------------------------------------------------------------------
		
		// Called when a seed of the given type hits this tile.
		public virtual void OnSeedHit(SeedType type, SeedEntity seed) { }
		
		// Called when a thrown object crashes onto this tile.
		public virtual void OnHitByThrownObject(CarriedTile thrownObject) {  }

		// Called when the tile is hit by one of the player's projectile.
		public virtual void OnHitByProjectile(Projectile projectile) {
			if (projectile is SeedEntity) {
				SeedEntity seed = (SeedEntity) projectile;
				OnSeedHit(seed.SeedType, seed);
			}
		}

		// Called when the player presses A on this tile, when facing the given direction.
		// Return true if player controls should be disabled for the rest of the frame.
		public virtual bool OnAction(int direction) { return false; }

		// Called when the player hits this tile with the sword.
		public virtual void OnSwordHit(ItemWeapon swordItem) {
			int minLevel = properties.GetInteger("cuttable_sword_level", Item.Level1);
			if (!isMoving && flags.HasFlag(TileFlags.Cuttable) &&
				(!(swordItem is ItemSword) || swordItem.Level >= minLevel))
			{
				Break(true);
			}
		}

		// Called when the player hits this tile with the sword.
		public virtual void OnBombExplode() {
			if (!isMoving && flags.HasFlag(TileFlags.Bombable))
				Break(true);
		}

		// Called when the tile is burned by a fire.
		public virtual void OnBurn() {
			if (!isMoving && flags.HasFlag(TileFlags.Burnable)) {
				SpawnDrop();
				roomControl.RemoveTile(this);
				if (properties.GetBoolean("disable_on_destroy", false))
					IsEnabled = false; // TODO: this won't exactly work anymore.
			}
		}

		// Called when the tile is hit by the player's boomerang.
		public virtual void OnBoomerang() {
			if (!isMoving && flags.HasFlag(TileFlags.Boomerangable))
				Break(true);
		}

		public virtual void OnGrab(int direction, ItemBracelet bracelet) {
			if (!isMoving && !flags.HasFlag(TileFlags.NotGrabbable)) {
				Player player = roomControl.Player;
				player.GrabState.Bracelet = bracelet;
				player.GrabState.Tile = this;
				player.BeginWeaponState(player.GrabState);
			}
		}
		
		// Called when the player wants to push the tile.
		public virtual bool OnPush(int direction, float movementSpeed) {
			if (!HasFlag(TileFlags.Movable))
				return false;
			if (roomControl.IsSideScrolling && Directions.IsVertical(direction))
				return false;
			if (properties.GetBoolean("move_once", false) && hasMoved)
				return false;
			int moveDir = properties.GetInteger("move_direction", -1);
			if (moveDir >= 0 && direction != moveDir)
				return false;
			if (Move(direction, 1, movementSpeed)) {
				if (soundMove != null)
					AudioSystem.PlaySound(soundMove);
				return true;
			}
			return false;
		}

		// Called when the player digs the tile with the shovel.
		public virtual bool OnDig(int direction) {
			if (isMoving || !IsDigable)
				return false;

			// Remove/dig the tile.
			if (layer == 0) {
				roomControl.RemoveTile(this);
					
				// Spawn the a "dug" tile in this tile's place.
				TileData data = Resources.GetResource<TileData>("dug");
				Tile dugTile = Tile.CreateTile(data);
				roomControl.PlaceTile(dugTile, location, layer);
			}
			else {
				roomControl.RemoveTile(this);
			}

			if (properties.GetBoolean("disable_on_destroy", false))
				IsEnabled = false; // TODO: this won't exactly work anymore.

			// Spawn drops.
			Entity dropEntity = SpawnDrop();
			if (dropEntity != null) {
				if (dropEntity is Collectible)
					(dropEntity as Collectible).PickupableDelay = GameSettings.COLLECTIBLE_DIG_PICKUPABLE_DELAY;
				else if (dropEntity is Monster)
					(dropEntity as Monster).BeginSpawnState();

				dropEntity.Physics.Velocity = Directions.ToVector(direction) * GameSettings.DROP_ENTITY_DIG_VELOCITY;
			}

			return true;
		}

		// Called while the player is trying to push the tile but before it's actually moved.
		public virtual void OnPushing(int direction) { }

		// Called when the player jumps and lands on the tile.
		public virtual void OnLand(Point2I startTile) { }
			
		// Called when the tile completes a movement (like after being pushed).
		public virtual void OnCompleteMovement() {
			// Check if we are over a hazard tile (water, lava, hole).
			
		}

		// Called when the tile is pushed into a hole.
		public virtual void OnFallInHole() {
			if (fallsInHoles) {
				RoomControl.SpawnEntity(new EffectFallingObject(), Center);
				AudioSystem.PlaySound(GameData.SOUND_OBJECT_FALL);
				RoomControl.RemoveTile(this);
			}
		}

		// Called when the tile is pushed into water.
		public virtual void OnFallInWater() {
			if (fallsInHoles) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_WATER_SPLASH, DepthLayer.EffectSplash, true), Center);
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
				RoomControl.RemoveTile(this);
			}
		}

		// Called when the tile is pushed into lava.
		public virtual void OnFallInLava() {
			if (fallsInHoles) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_LAVA_SPLASH, DepthLayer.EffectSplash, true), Center);
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
				RoomControl.RemoveTile(this);
			}
		}

		/// <summary>Called when a tile is in mid-air in a side-scrolling room.</summary>
		public virtual void OnFloating() { }

		// Called when another tile covers this tile.
		//public virtual void OnCover(Tile tile) { }

		// Called when this tile is uncovered.
		//public virtual void OnUncover(Tile tile) { }

		public virtual void OnCoverBegin(Tile tile) { }
		
		public virtual void OnCoverComplete(Tile tile) { }
		
		public virtual void OnUncoverBegin(Tile tile) { }

		public virtual void OnUncoverComplete(Tile tile) { }


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		// Break the tile, destroying it.
		public virtual void Break(bool spawnDrops) {
			// Spawn the break effect.
			if (breakAnimation != null && !CancelBreakEffect) {
				Effect breakEffect = new Effect(breakAnimation, DepthLayer.EffectTileBreak, true);
				RoomControl.SpawnEntity(breakEffect, Center);
			}

			if (breakSound != null && !CancelBreakSound)
				AudioSystem.PlaySound(breakSound);

			// Spawn drops.
			if (spawnDrops)
				SpawnDrop();

			// Destroy the tile.
			if (properties.GetBoolean("disable_on_destroy", false))
				IsEnabled = false; // TODO: this won't exactly work anymore.
			RoomControl.RemoveTile(this);
		}

		// Spawn a drop entity based on the random drop-list.
		public Entity SpawnDrop() {
			Entity dropEntity = null;

			// Choose a random drop (or none) from the list.
			if (dropList != null)
				dropEntity = dropList.CreateDropEntity(GameControl);

			// Spawn the drop if there is one.
			if (dropEntity != null) {
				dropEntity.SetPositionByCenter(Center);
				dropEntity.Physics.ZVelocity = GameSettings.DROP_ENTITY_SPAWN_ZVELOCITY;
				if (dropEntity is Monster) {
					RoomControl.ScheduleEvent(2, () => {
						RoomControl.SpawnEntity(dropEntity);
					});
				}
				else {
					RoomControl.SpawnEntity(dropEntity);
				}
			}

			return dropEntity;
		}


		//-----------------------------------------------------------------------------
		// Simulation
		//-----------------------------------------------------------------------------

		public virtual void OnInitialize() {}
		
		public virtual void OnPostInitialize() {}

		public virtual void OnRemoveFromRoom() {}

		public virtual void Update() {
			UpdateMovement();

			// Check if hurting the player.
			if (HasFlag(TileFlags.HurtPlayer) && roomControl.Player.IsOnGround &&
				(!roomControl.Player.IsPassable || IsSolid))
			{
				Rectangle2F playerBox = roomControl.Player.Physics.PositionedCollisionBox;
				Rectangle2F hurtBox = hurtArea;
				hurtBox.Point += Position;
				if (hurtBox.Intersects(playerBox)) {
					roomControl.Player.Hurt(new DamageInfo(hurtDamage) {
						ApplyKnockback		= true,
						KnockbackDuration	= 14,
						InvincibleDuration	= 35,
						FlickerDuration		= 35,
						HasSource			= true,
						SourcePosition		= Center
					});
				}
			}
		}

		private void UpdateMovement() {
			// Update movement.
			if (isMoving) {
				velocity = (Vector2F) moveDirection * movementSpeed;
				
				if (offset.Dot(moveDirection) >= 0.0f) {
					currentMoveDistance++;
					offset -= (Vector2F) (moveDirection * GameSettings.TILE_SIZE);

					if (currentMoveDistance >= moveDistance) {
						offset			= Vector2F.Zero;
						velocity		= Vector2F.Zero;
						moveDirection	= Point2I.Zero;
						isMoving		= false;
						CheckSurfaceTile();
						if (IsDestroyed)
							return;

						// Fire the Moved event.
						GameControl.FireEvent(this, "moved");

						OnCompleteMovement();
					}
					else {
						roomControl.MoveTile(this, location + moveDirection, layer);
					}
				}
				else if (currentMoveDistance + 1 >= moveDistance) {
					// Don't overshoot the destination.
					float overshoot = (offset + velocity).Dot(moveDirection);
					if (overshoot >= 0.0f) {
						velocity -= overshoot * (Vector2F) moveDirection;
					}
				}
			}
			// Update path following.
			else if (path != null) {
				TilePathMove move = path.Moves[pathMoveIndex];
					
				// Begin the next move in the path after the delay has been passed.
				if (pathTimer >= move.Delay) {
					Move(move.Direction, move.Distance, move.Speed);
					pathTimer = 0;
					pathMoveIndex++;
					if (pathMoveIndex >= path.Moves.Count) {
						if (path.Repeats)
							pathMoveIndex = 0;
						else
							path = null;
					}
				}

				pathTimer++;
			}
			
			// Integrate velocity.
			if (isMoving)
				offset += velocity;
			else
				velocity = Vector2F.Zero;
		}

		protected void CheckSurfaceTile() {
			// Find the surface tile (tile below this one).
			Tile newSurfaceTile = null;
			foreach (Tile tile in roomControl.TileManager
				.GetTilesAtLocation(location, TileLayerOrder.HighestToLowest))
			{
				if (tile != this && tile.IsSurface) {
					newSurfaceTile = tile;
					break;
				}
			}

			// Check if the surface tile has changed.
			if (surfaceTile != newSurfaceTile) {
				surfaceTile = newSurfaceTile;
				if (surfaceTile != null && path == null) {
					if (surfaceTile.IsWater)
						OnFallInWater();
					else if (surfaceTile.IsLava)
						OnFallInLava();
					else if (surfaceTile.IsHole)
						OnFallInHole();
				}
			}

			// Check for floating in side-scrolling
			if (IsFloating) {
				OnFloating();
			}
		}

		public virtual void UpdateGraphics() {
			graphics.Update();
		}

		public virtual void Draw(RoomGraphics g) {
			graphics.Draw(g);
		}

		public virtual void DrawAbove(RoomGraphics g) {
			graphics.DrawAbove(g);
		}


		//-----------------------------------------------------------------------------
		// Projectiles
		//-----------------------------------------------------------------------------

		public Projectile ShootFromDirection(Projectile projectile, int direction, float speed) {
			return ShootFromDirection(projectile, direction, speed, Vector2F.Zero, 0);
		}

		public Projectile ShootFromAngle(Projectile projectile, int angle, float speed) {
			return ShootFromAngle(projectile, angle, speed, Vector2F.Zero, 0);
		}

		public Projectile ShootProjectile(Projectile projectile, Vector2F velocity) {
			return ShootProjectile(projectile, velocity, Vector2F.Zero, 0);
		}

		public Projectile ShootFromDirection(Projectile projectile, int direction, float speed, Vector2F positionOffset, int zPositionOffset = 0) {
			projectile.TileOwner	= this;
			projectile.Direction	= direction;
			projectile.Physics.Velocity = Directions.ToVector(direction) * speed;
			RoomControl.SpawnEntity(projectile, Center + positionOffset, zPositionOffset);
			return projectile;
		}

		public Projectile ShootFromAngle(Projectile projectile, int angle, float speed, Vector2F positionOffset, int zPositionOffset = 0) {
			projectile.TileOwner	= this;
			projectile.Angle		= angle;
			projectile.Physics.Velocity = Angles.ToVector(angle, true) * speed;
			RoomControl.SpawnEntity(projectile, Center + positionOffset, zPositionOffset);
			return projectile;
		}

		public Projectile ShootProjectile(Projectile projectile, Vector2F velocity, Vector2F positionOffset, int zPositionOffset) {
			projectile.TileOwner	= this;
			projectile.Physics.Velocity = velocity;
			RoomControl.SpawnEntity(projectile, Center + positionOffset, zPositionOffset);
			return projectile;
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		// Construct a tile from the given tile-data.
		public static Tile CreateTile(TileData data) {
			return CreateTile(new TileDataInstance(data, 0, 0, 0));
		}

		// Construct a tile from the given tile-data.
		public static Tile CreateTile(TileDataInstance data) {
			Tile tile;
			
			// Construct the tile.
			if (data.Type == null)
				tile = new Tile();
			else
				tile = (Tile) data.Type.GetConstructor(Type.EmptyTypes).Invoke(null);
			
			tile.location			= data.Location;
			tile.layer				= data.Layer;

			tile.tileData			= data;
			tile.flags				= data.Flags;
			tile.spriteAsObject		= data.SpriteAsObject;
			tile.breakAnimation		= data.BreakAnimation;
			tile.breakSound			= data.BreakSound;
			tile.collisionModel		= data.CollisionModel;
			tile.size				= data.Size;

			tile.hurtArea			= data.HurtArea;
			tile.hurtDamage			= data.HurtDamage;

			if (tile.collisionModel == null)
				tile.collisionModel = GameData.MODEL_BLOCK;
			
			if (data.SpriteList.Length > 0)
				tile.graphics.PlayAnimation(data.SpriteList[0]);

			int conveyorAngle = data.ConveyorAngle;
			if (conveyorAngle >= 0)
				tile.conveyorVelocity = Angles.ToVector(conveyorAngle, true) * data.ConveyorSpeed;

			//tile.properties.SetAll(data.BaseProperties);
			//tile.properties.SetAll(data.Properties);
			//data.ResetState();
			//tile.properties.BaseProperties = data.ModifiedProperties;
			
			// NOTE: properties.PropertyObject will refer to the tile's TileDataInstance.
			tile.properties = data.ModifiedProperties;
			
			return tile;
		}

		/*public static Type GetType(string typeName, bool ignoreCase) {
			StringComparison comparision = StringComparison.Ordinal;
			if (ignoreCase)
				comparision = StringComparison.OrdinalIgnoreCase;

			return Assembly.GetExecutingAssembly().GetTypes()
				.FirstOrDefault(t => t.Name.Equals(typeName, comparision));
		}*/

		/// <summary>Draws the tile data to display in the editor.</summary>
		public static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			int spriteIndex = args.Properties.GetInteger("sprite_index", 0);
			ISprite sprite = args.Tile.GetSpriteIndex(spriteIndex);
			if (sprite is Animation) {
				int substripIndex = args.Properties.GetInteger("substrip_index", 0);
				sprite = ((Animation) sprite).GetSubstrip(substripIndex);
			}
			if (sprite != null) {
				g.DrawSprite(
					sprite,
					args.SpriteDrawSettings,
					args.Position,
					args.Color);
			}
		}

		/// <summary>Draws the tile data to display in the editor.</summary>
		public static void DrawTileDataWithOffset(Graphics2D g, TileDataDrawArgs args, Point2I offset) {
			ISprite sprite = args.Tile.GetSpriteIndex(args.Properties.GetInteger("sprite_index"));
			if (sprite is Animation) {
				int substripIndex = args.Properties.GetInteger("substrip_index", 0);
				sprite = ((Animation) sprite).GetSubstrip(substripIndex);
			}
			if (sprite != null) {
				g.DrawSprite(
					sprite,
					args.SpriteDrawSettings,
					args.Position + offset,
					args.Color);
			}
		}

		/// <summary>Draws the tile data to display in the editor with the specified sprite index.</summary>
		public static void DrawTileDataIndex(Graphics2D g, TileDataDrawArgs args, int spriteIndex = -1, int substripIndex = -1) {
			if (spriteIndex == -1)
				spriteIndex = args.Properties.GetInteger("sprite_index", 0);
			ISprite sprite = args.Tile.GetSpriteIndex(spriteIndex);
			if (sprite is Animation) {
				if (substripIndex == -1)
					substripIndex = args.Properties.GetInteger("substrip_index", 0);
				sprite = ((Animation) sprite).GetSubstrip(substripIndex);
			}
			if (sprite != null) {
				g.DrawSprite(
					sprite,
					args.SpriteDrawSettings,
					args.Position,
					args.Color);
			}
		}

		/// <summary>Draws the tile data to display in the editor.</summary>
		public static void DrawTileDataAbove(Graphics2D g, TileDataDrawArgs args) {
			if (args.Tile.SpriteAbove != null) {
				g.DrawSprite(
					args.Tile.SpriteAbove,
					args.SpriteDrawSettings,
					args.Position,
					args.Color);
			}
		}

		/// <summary>Draws the tile data to display in the editor.</summary>
		public static void DrawTileDataColors(Graphics2D g, TileDataDrawArgs args, ColorDefinitions colorDefinitions) {
			int spriteIndex = args.Properties.GetInteger("sprite_index", 0);
			ISprite sprite = args.Tile.GetSpriteIndex(spriteIndex);
			if (sprite is Animation) {
				int substripIndex = args.Properties.GetInteger("substrip_index", 0);
				sprite = ((Animation) sprite).GetSubstrip(substripIndex);
			}
			if (sprite != null) {
				SpriteDrawSettings settings = new SpriteDrawSettings(args.Zone.StyleDefinitions,
					colorDefinitions, args.Time);
				g.DrawSprite(
					sprite,
					settings,
					args.Position,
					args.Color);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Returns the room control this tlie belongs to.
		public RoomControl RoomControl {
			get { return roomControl; }
			set { roomControl = value; }
		}
		
		public GameControl GameControl {
			get { return roomControl.GameControl; }
		}

		public TileGraphicsComponent Graphics {
			get { return graphics; }
		}

		public Zone Zone {
			get { return roomControl.Room.Zone; }
		}

		public Vector2F Position {
			get { return (location * GameSettings.TILE_SIZE) + offset; }
		}

		public Vector2F PreviousPosition {
			get { return (previousLocation * GameSettings.TILE_SIZE) + previousOffset; }
		}

		public Vector2F Center {
			get { return Position + ((Vector2F) size * (0.5f * GameSettings.TILE_SIZE)); }
		}

		public Vector2F Offset {
			get { return offset; }
			set { offset = value; }
		}

		public Point2I Location {
			get { return location; }
			set { location = value; }
		}

		public Vector2F PreviousOffset {
			get { return previousOffset; }
			set { previousOffset = value; }
		}

		public Point2I PreviousLocation {
			get { return previousLocation; }
			set { previousLocation = value; }
		}
		
		public int Layer {
			get { return layer; }
			set { layer = value; }
		}

		public Point2I Size {
			get { return size; }
			set { size = value; }
		}
		
		public int Width {
			get { return size.X; }
			set { size.X = value; }
		}
		
		public int Height {
			get { return size.Y; }
			set { size.Y = value; }
		}
		
		public Rectangle2F Bounds {
			get { return new Rectangle2F(Position, size * GameSettings.TILE_SIZE); }
		}
		
		public Rectangle2F PreviousBounds {
			get { return new Rectangle2F(PreviousPosition, size * GameSettings.TILE_SIZE); }
		}

		public TileFlags Flags {
			get { return flags; }
		}
		
		public ISprite SpriteAsObject {
			get { return spriteAsObject; }
			set { spriteAsObject = value; }
		}

		public ISprite[] SpriteList {
			get { return tileData.SpriteList; }
		}

		public Animation BreakAnimation {
			get { return breakAnimation; }
			set { breakAnimation = value; }
		}

		public Sound BreakSound {
			get { return breakSound; }
			set { breakSound = value; }
		}

		public CollisionModel CollisionModel {
			get { return collisionModel; }
			set { collisionModel = value; }
		}

		public int PushDelay {
			get { return pushDelay; }
			set { pushDelay = value; }
		}

		public bool IsMoving {
			get { return isMoving; }
		}

		public bool IsInMotion {
			get { return (isMoving || path != null); }
		}

		public int MoveDirection {
			get { return Directions.FromPoint(moveDirection); }
		}

		public Properties Properties {
			get { return properties; }
		}

		public EventCollection Events {
			get { return tileData.Events; }
		}

		// Get the original tile data from which this was created.
		public TileDataInstance TileData {
			get { return tileData; }
		}

		public bool IsCoverableByBlock {
			get { return (!IsNotCoverable && !IsSolid && !IsStairs && (!IsLadder || RoomControl.IsSideScrolling)); }
		}

		public DropList DropList {
			get { return dropList; }
			set { dropList = value; }
		}

		public bool IsEnabled {
			get { return Properties.GetBoolean("enabled"); }
			set { Properties.Set("enabled", value); }
		}

		public bool CancelBreakSound {
			get { return cancelBreakSound; }
			set { cancelBreakSound = value; }
		}

		public bool CancelBreakEffect {
			get { return cancelBreakEffect; }
			set { cancelBreakEffect = value; }
		}

		public bool DrawAsEntity {
			get { return TileData.DrawAsEntity; }
		}

		/// <summary>Gets the tile data to appear when
		/// this one is removed while on layer 1.</summary>
		public TileData TileBelow {
			get { return TileData.TileBelow; }
		}


		//-----------------------------------------------------------------------------
		// Flag Properties
		//-----------------------------------------------------------------------------

		public bool IsNotCoverable {
			get { return flags.HasFlag(TileFlags.NotCoverable); }
		}

		public bool IsNotPushable {
			get { return flags.HasFlag(TileFlags.NotPushable); }
		}

		public bool IsDigable {
			get { return flags.HasFlag(TileFlags.Digable); }
		}

		public bool IsSwitchable {
			get { return flags.HasFlag(TileFlags.Switchable); }
		}

		public bool StaysOnSwitch {
			get { return flags.HasFlag(TileFlags.SwitchStays); }
		}

		public bool BreaksOnSwitch {
			get { return !flags.HasFlag(TileFlags.SwitchStays); }
		}

		public bool IsBoomerangable {
			get { return flags.HasFlag(TileFlags.Boomerangable); }
		}

		public bool IsBreakable {
			get { return (flags &  (TileFlags.Cuttable |
									TileFlags.Pickupable |
									TileFlags.Movable |
									TileFlags.Switchable |
									TileFlags.Boomerangable)) != 0; }
		}
		
		public bool IsHole {
			get { return EnvironmentType == TileEnvironmentType.Hole ||
						 EnvironmentType == TileEnvironmentType.Whirlpool; }
		}
		
		public bool IsWater {
			get { return EnvironmentType == TileEnvironmentType.Water ||
						 EnvironmentType == TileEnvironmentType.Ocean ||
						 EnvironmentType == TileEnvironmentType.Whirlpool ||
						 EnvironmentType == TileEnvironmentType.DeepWater; }
		}
		
		public bool IsLava {
			get { return EnvironmentType == TileEnvironmentType.Lava; }
		}
		
		public bool IsHoleWaterOrLava {
			get { return (IsHole || IsWater || IsLava); }
		}
		
		public bool IsSolid {
			get { return isSolid; }
			set { isSolid = value; }
		}
		
		public virtual bool IsSurface {
			get { return ((!isSolid || IsHalfSolid) && !IsPlatform); }
		}
		
		public bool IsPlatform {
			get { return (!isSolid && (isMoving || path != null)); }
		}

		public bool IsHalfSolid {
			get { return (SolidType == TileSolidType.HalfSolid); }
		}

		public bool IsLedge {
			get { return (SolidType == TileSolidType.Ledge); }
		}

		public bool IsLeapLedge {
			get { return (SolidType == TileSolidType.LeapLedge); }
		}

		public bool IsAnyLedge {
			get {
				return (SolidType == TileSolidType.Ledge ||
						SolidType == TileSolidType.LeapLedge);
			}
		}

		public bool IsCrushable {
			get {
				// TODO: Check if any other criteria could be acceptable.
				// Maybe make this its own flag.
				return (Flags.HasFlag(TileFlags.Cuttable) || Flags.HasFlag(TileFlags.Switchable));
			}
		}

		public bool IsIce {
			get { return EnvironmentType == TileEnvironmentType.Ice; }
		}

		public bool IsStairs {
			get { return EnvironmentType == TileEnvironmentType.Stairs; }
		}
		
		public bool IsLadder {
			get { return EnvironmentType == TileEnvironmentType.Ladder; }
		}

		// Returns true if the tile is not alive.
		public bool IsDestroyed {
			get { return !isAlive; }
		}

		// Returns true if the tile is still alive.
		public bool IsAlive {
			get { return isAlive; }
			set { isAlive = value; }
		}

		public TileEnvironmentType EnvironmentType {
			get { return properties.GetEnum<TileEnvironmentType>("environment_type", TileEnvironmentType.Normal); }
			set { properties.Set("environment_type", (int) value); }
		}

		public TileSolidType SolidType {
			get { return properties.GetEnum<TileSolidType>("solidity", TileSolidType.NotSolid); }
			set { properties.Set("solidity", (int) value); }
		}

		public int LedgeDirection {
			get { return properties.GetInteger("ledge_direction", Directions.Down); }
		}

		public bool ClingWhenStabbed {
			get { return properties.GetBoolean("cling_on_stab", true); }
			set { properties.Set("cling_on_stab", value); }
		}

		public Vector2F Velocity {
			get { return velocity; }
		}

		public Vector2F ConveyorVelocity {
			get { return conveyorVelocity; }
		}

		public virtual TileDataInstance TileDataOwner {
			get { return tileData; }
		}

		public Rectangle2I TileGridArea {
			get { return tileGridArea; }
			set { tileGridArea = value; }
		}

		public CollisionStyle CollisionStyle {
			get { return collisionStyle; }
			set { collisionStyle = value; }
		}

		/// <summary>Determines if the block is floating in air
		/// in a side-scrolling environment.</summary>
		public bool IsFloating {
			get {
				if (roomControl.IsSideScrolling) {
					Tile ssSurfaceTile = roomControl.TileManager.GetTopTile(Location + Directions.ToPoint(Directions.Down));
					return (ssSurfaceTile == null || !ssSurfaceTile.IsSolid || ssSurfaceTile.IsInMotion);
					// TODO: Check if collision box does not have a flat surface on top?
				}
				return false;
			}
		}

		public Rectangle2I HurtArea {
			get { return hurtArea; }
			set { hurtArea = value; }
		}

		public int HurtDamage {
			get { return hurtDamage; }
			set { hurtDamage = value; }
		}


		//-----------------------------------------------------------------------------
		// Scripting API
		//-----------------------------------------------------------------------------

		public void OverrideDefaultState() {
			tileData.OverrideDefaultState();
		}

		string ZeldaAPI.Tile.ID {
			get { return properties.GetString("id", ""); }
		}

		bool ZeldaAPI.Tile.IsMovable {
			get { return HasFlag(TileFlags.Movable); }
			set { SetFlags(TileFlags.Movable, value); }
		}
	}
}
