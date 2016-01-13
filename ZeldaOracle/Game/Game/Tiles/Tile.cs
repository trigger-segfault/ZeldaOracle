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
using ZeldaOracle.Game.Items.Drops;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {
	
	public class Tile : IPropertyObject, ZeldaAPI.Tile {

		// Internal
		private RoomControl			roomControl;
		private bool				isAlive;
		private bool				isInitialized;
		private Point2I				location;		// The tile location in the room.
		private int					layer;			// The layer this tile is in.
		private Point2I				moveDirection;
		private bool				isMoving;
		private float				movementSpeed;
		private Vector2F			offset;			// Offset in pixels from its tile location (used for movement).
		protected AnimationPlayer	animationPlayer;
		private bool				hasMoved;

		// Settings
		private TileDataInstance	tileData;		// The tile data used to create this tile.
		private TileFlags			flags;
		private Point2I				size;			// How many tile spaces this tile occupies. NOTE: this isn't supported yet.
		private CollisionModel		collisionModel;
		private SpriteAnimation		customSprite;
		private SpriteAnimation		spriteAsObject;	// The sprite for the tile if it were picked up, pushed, etc.
		private Animation			breakAnimation;	// The animation to play when the tile is broken.
		private int					pushDelay;		// Number of ticks of pushing before the player can move this tile.
		private DropList			dropList;
		private bool				isSolid;
		private Properties			properties;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		// Use Tile.CreateTile() instead of this constructor.
		protected Tile() {
			isAlive			= false;
			isInitialized	= false;
			location		= Point2I.Zero;
			layer			= 0;
			offset			= Point2I.Zero;
			size			= Point2I.One;
			customSprite	= new SpriteAnimation();
			spriteAsObject	= new SpriteAnimation();
			isSolid			= false;
			isMoving		= false;
			pushDelay		= 20;
			properties		= new Properties(this);
			tileData		= null;
			moveDirection	= Point2I.Zero; 
			dropList		= null;
			animationPlayer	= null;
			hasMoved		= false;
		}


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------
		
		public void Initialize(RoomControl control) {
			this.roomControl = control;
			this.isAlive = true;

			this.hasMoved = false;

			if (!isInitialized) {
				isInitialized = true;
				
				isSolid = (SolidType != TileSolidType.NotSolid);

				// Setup default drop list.
				if (IsDigable && !IsSolid)
					dropList = RoomControl.GameControl.DropManager.GetDropList("dig");
				else
					dropList = RoomControl.GameControl.DropManager.GetDropList("default");

				OnInitialize();
			}
		}


		//-----------------------------------------------------------------------------
		// Flags
		//-----------------------------------------------------------------------------
		
		public void SetFlags(TileFlags flagsToSet, bool enabled) {
			if (enabled)
				flags |= flagsToSet;
			else
				flags &= ~flagsToSet;
		}


		//-----------------------------------------------------------------------------
		// Interaction Methods
		//-----------------------------------------------------------------------------
		
		// Called when a seed of the given type hits this tile.
		public virtual void OnSeedHit(SeedType type, Entity seed) {}

		// Called when the player presses A on this tile, when facing the given direction.
		// Return true if player controls should be disabled for the rest of the frame.
		public virtual bool OnAction(int direction) { return false; }

		// Called when the player touches any part of the tile area.
		public virtual void OnTouch() { }

		// Called when the player touches the collision box of the tile.
		public virtual void OnCollide() { }

		// Called when the player hits this tile with the sword.
		public virtual void OnSwordHit() {
			if (!isMoving && flags.HasFlag(TileFlags.Cuttable))
				Break(true);
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
			}
		}

		// Called when the tile is hit by the player's boomerang.
		public virtual void OnBoomerang() {
			if (!isMoving && flags.HasFlag(TileFlags.Boomerangable))
				Break(true);
		}

		// Called when the player wants to push the tile.
		public virtual bool OnPush(int direction, float movementSpeed) {
			if (!HasFlag(TileFlags.Movable))
				return false;
			if (properties.GetBoolean("move_once", false) && hasMoved)
				return false;
			int moveDir = properties.GetInteger("move_direction", -1);
			if (moveDir >= 0 && direction != moveDir)
				return false;
			return Move(direction, 1, movementSpeed);
		}

		public virtual bool OnDig(int direction) {
			if (!isMoving && IsDigable) {
				
				// Remove/dig the tile.
				if (layer == 0) {
					roomControl.RemoveTile(this);

					TileData data = Resources.GetResource<TileData>("dug");
					Tile dugTile = Tile.CreateTile(data);

					roomControl.PlaceTile(dugTile, location, layer);
					customSprite = GameData.SPR_TILE_DUG;
				}
				else {
					roomControl.RemoveTile(this);
				}

				// Spawn drop.
				Entity dropEntity = SpawnDrop();
				if (dropEntity != null) {
					if (dropEntity is Collectible)
						(dropEntity as Collectible).PickupableDelay = GameSettings.COLLECTIBLE_DIG_PICKUPABLE_DELAY;
					dropEntity.Physics.Velocity = Directions.ToVector(direction) * GameSettings.DROP_ENTITY_DIG_VELOCITY;
				}

				return true;
			}
			return false;
		}

		// Called while the player is trying to push the tile but before it's actually moved.
		public virtual void OnPushing(int direction) { }

		// Called when the player jumps and lands on the tile.
		public virtual void OnLand(Point2I startTile) { }
			
		// Called when a tile is finished moving after being pushed.
		public virtual void OnCompleteMovement() {
			// Check if we are over a hazard tile (water, lava, hole).
			Tile tile = null;
			for (int i = layer - 1; i >= 0 && tile == null; i--)
				tile = roomControl.GetTile(location, i);

			if (tile != null) {
				if (tile.IsWater)
					OnFallInWater();
				else if (tile.IsLava)
					OnFallInLava();
				else if (tile.IsHole)
					OnFallInHole();
				else
					tile.OnCover(this);
			}
		}

		// Called when the tile is pushed into a hole.
		public virtual void OnFallInHole() {
			RoomControl.SpawnEntity(new EffectFallingObject(), Center);
			RoomControl.RemoveTile(this);
		}

		// Called when the tile is pushed into water.
		public virtual void OnFallInWater() {
			RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_WATER_SPLASH, DepthLayer.EffectSplash), Center);
			RoomControl.RemoveTile(this);
		}

		// Called when the tile is pushed into lava.
		public virtual void OnFallInLava() {
			RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_LAVA_SPLASH, DepthLayer.EffectSplash), Center);
			RoomControl.RemoveTile(this);
		}

		// Called when a tile covers this tile.
		public virtual void OnCover(Tile tile) { }

		// Called when this tile is uncovered.
		public virtual void OnUncover(Tile tile) { }


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		protected bool Move(int direction, int distance, float movementSpeed) {
			Point2I oldLocation = location;

			if (isMoving)
				return false;

			// Make sure were not pushing out of bounds.
			Point2I newLocation = location + Directions.ToPoint(direction) * distance;
			if (!RoomControl.IsTileInBounds(newLocation))
				return false;

			// Make sure there are no obstructions.
			int newLayer = -1;
			for (int i = 0; i < RoomControl.Room.LayerCount; i++) {
				Tile t = RoomControl.GetTile(newLocation.X, newLocation.Y, i);
				if (t != null && !t.IsCoverableByBlock)
					return false;
				if (t == null && newLayer != layer)
					newLayer = i;
			}

			// Not enough layers to place this tile.
			if (newLayer < 0)
				return false;

			// Move the tile to the new location.
			isMoving = true;
			hasMoved = true;
			this.movementSpeed = movementSpeed;
			moveDirection = Directions.ToPoint(direction);
			offset = -Directions.ToVector(direction) * GameSettings.TILE_SIZE;
			RoomControl.MoveTile(this, newLocation, newLayer);

			Tile unconveredTile = roomControl.GetTopTile(oldLocation);
			if (unconveredTile != null)
				unconveredTile.OnUncover(this);

			// Fire the OnMove event.
			GameControl.ExecuteScript(properties.GetString("on_move", ""));

			return true;
		}

		public void Break(bool spawnDrops) {
			if (breakAnimation != null) {
				Effect breakEffect = new Effect(breakAnimation, DepthLayer.EffectTileBreak);
				RoomControl.SpawnEntity(breakEffect, Center);
			}

			RoomControl.RemoveTile(this);

			if (spawnDrops) {
				SpawnDrop();
			}
		}

		public Entity SpawnDrop() {
			Entity dropEntity = null;

			// Choose a drop or null.
			if (dropList != null)
				dropEntity = dropList.CreateDropEntity(GameControl);

			// Spawn the drop.
			if (dropEntity != null) {
				dropEntity.SetPositionByCenter(Center);
				dropEntity.Physics.ZVelocity = GameSettings.DROP_ENTITY_SPAWN_ZVELOCITY;
				RoomControl.SpawnEntity(dropEntity);
			}

			return dropEntity;
		}


		//-----------------------------------------------------------------------------
		// Simulation
		//-----------------------------------------------------------------------------

		public virtual void OnInitialize() {}

		public virtual void Update() {
			// Update movement (after pushed).
			if (isMoving) {
				if (offset.LengthSquared > 0.0f) {
					offset += (Vector2F)moveDirection * movementSpeed;
					if (offset.LengthSquared == 0.0f || GMath.Sign(offset) == GMath.Sign(moveDirection)) {
						offset = Vector2F.Zero;
						isMoving = false;
						OnCompleteMovement();
					}
				}
			}
			else {
				moveDirection = Point2I.Zero;
			}
		}

		public virtual void UpdateGraphics() {

		}

		public virtual void Draw(Graphics2D g) {
			SpriteAnimation sprite = (!customSprite.IsNull ? customSprite : CurrentSprite);
			if (isMoving && !spriteAsObject.IsNull)
				sprite = spriteAsObject;

			if (animationPlayer != null) {
				g.DrawAnimation(animationPlayer.SubStrip, Zone.ImageVariantID,
					animationPlayer.PlaybackTime, Position);
			}
			else if (sprite.IsAnimation) {
				// Draw as an animation.
				g.DrawAnimation(sprite.Animation, Zone.ImageVariantID,
					RoomControl.GameControl.RoomTicks, Position);
			}
			else if (sprite.IsSprite) {
				// Draw as a sprite.
				g.DrawSprite(sprite.Sprite, Zone.ImageVariantID, Position);
			}
		}

		
		//-----------------------------------------------------------------------------
		// Flags Methods
		//-----------------------------------------------------------------------------

		// Returns true if the tile has the normal flags.
		public bool HasFlag(TileFlags flags) {
			return Flags.HasFlag(flags);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		// Instantiate a tile from the given tile-data.
		public static Tile CreateTile(TileData data) {
			return CreateTile(new TileDataInstance(data, 0, 0, 0));
		}

		// Instantiate a tile from the given tile-data.
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
			tile.collisionModel		= data.CollisionModel;
			tile.size				= data.Size;

			tile.properties.SetAll(data.BaseProperties);
			tile.properties.SetAll(data.Properties);
			tile.properties.BaseProperties	= data.Properties;
			
			return tile;
		}

		public static Type GetType(string typeName, bool ignoreCase) {
			StringComparison comparision = StringComparison.Ordinal;
			if (ignoreCase)
				comparision = StringComparison.OrdinalIgnoreCase;

			return Assembly.GetExecutingAssembly().GetTypes()
				.FirstOrDefault(t => t.Name.Equals(typeName, comparision));
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

		public Zone Zone {
			get { return roomControl.Room.Zone; }
		}

		public Vector2F Position {
			get { return (location * GameSettings.TILE_SIZE) + offset; }
		}

		public Vector2F Center {
			get { return Position + new Vector2F(8, 8); }
		}

		public Vector2F Offset {
			get { return offset; }
			set { offset = value; }
		}

		public Point2I Location {
			get { return location; }
			set { location = value; }
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

		public TileFlags Flags {
			get { return flags; }
		}

		public SpriteAnimation CustomSprite {
			get { return customSprite; }
			set {
				if (value == null)
					customSprite.SetNull();
				else
					customSprite.Set(value);
			}
		}

		public SpriteAnimation SpriteAsObject {
			get { return spriteAsObject; }
			set {
				if (value == null)
					spriteAsObject.SetNull();
				else
					spriteAsObject.Set(value);
			}
		}

		public SpriteAnimation CurrentSprite {
			get {
				if (tileData.SpriteList.Length > 0)
					return tileData.SpriteList[properties.GetInteger("sprite_index")];
				return new SpriteAnimation();
			}
		}

		public int SpriteIndex {
			get { return properties.GetInteger("sprite_index"); }
			set { properties.Set("sprite_index", value); }
		}

		public Animation BreakAnimation {
			get { return breakAnimation; }
			set { breakAnimation = value; }
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

		public int MoveDirection {
			get { return Directions.FromPoint(moveDirection); }
		}

		public Properties Properties {
			get { return properties; }
		}
		
		// Get the original tile data from which this was created.
		public TileDataInstance TileData {
			get { return tileData; }
		}
		
		// Get the modified properties of the tile data from which this was created.
		// Do not access these properties, only modify them.
		public Properties BaseProperties {
			get { return tileData.Properties; }
		}

		public bool IsCoverableByBlock {
			get { return (!IsNotCoverable && !IsSolid && !IsStairs && !IsLadder); }
		}

		public DropList DropList {
			get { return dropList; }
			set { dropList = value; }
		}


		//-----------------------------------------------------------------------------
		// Flag Properties
		//-----------------------------------------------------------------------------

		public bool IsNotCoverable {
			get { return Flags.HasFlag(TileFlags.NotCoverable); }
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
		
		public bool IsHole {
			get { return EnvironmentType == TileEnvironmentType.Hole ||
						 EnvironmentType == TileEnvironmentType.Whirlpool; }
		}
		
		public bool IsWater {
			get { return EnvironmentType == TileEnvironmentType.Water; }
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

		public bool IsHalfSolid {
			get { return (SolidType == TileSolidType.HalfSolid); }
		}

		public bool IsLedge {
			get { return (SolidType == TileSolidType.Ledge); }
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

		public AnimationPlayer AnimationPlayer {
			get { return animationPlayer; }
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


		//-----------------------------------------------------------------------------
		// Scripting API
		//-----------------------------------------------------------------------------

	}
}
