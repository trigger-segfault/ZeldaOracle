using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {
	
	public class Tile : IPropertyObject {
		private static Type[] tileTypeList = null;

		// Internal
		private RoomControl		roomControl;
		private Point2I			location;		// The tile location in the room.
		private int				layer;			// The layer this tile is in.
		private Point2I			moveDirection;
		private bool			isMoving;
		private float			movementSpeed;
		private Vector2F		offset;			// Offset in pixels from its tile location (used for movement).

		// Settings
		private TileDataInstance	tileData;		// The tile data used to create this tile.
		private TileFlags			flags;
		private Point2I				size;			// How many tile spaces this tile occupies. NOTE: this isn't supported yet.
		private CollisionModel		collisionModel;
		private SpriteAnimation		customSprite;
		private SpriteAnimation		spriteAsObject;	// The sprite for the tile if it were picked up, pushed, etc.
		private Animation			breakAnimation;	// The animation to play when the tile is broken.
		private int					pushDelay;		// Number of ticks of pushing before the player can move this tile.
		private Properties			properties;

		protected bool				destroyedInHoles;
		protected bool				destroyedInWater;
		protected bool				destroyedInLava;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		// Use CreateTile() instead of this constructor.
		protected Tile() {
			location		= Point2I.Zero;
			layer			= 0;
			offset			= Point2I.Zero;
			size			= Point2I.One;
			flags			= TileFlags.Default;
			customSprite	= new SpriteAnimation();
			spriteAsObject	= new SpriteAnimation();
			isMoving		= false;
			pushDelay		= 20;
			properties		= new Properties();
			properties.PropertyObject = this;
			tileData		= null;
			moveDirection	= Point2I.Zero; 

			destroyedInHoles	= true;
			destroyedInWater	= true;
			destroyedInLava		= true;
		}


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------
		
		public void Initialize(RoomControl control) {
			this.roomControl = control;
			Initialize();
		}
		

		//-----------------------------------------------------------------------------
		// Interaction Methods
		//-----------------------------------------------------------------------------
		
		// Called when a seed of the given type hits this tile.
		public virtual void OnSeedHit(Seed seed) {}

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
			if (!isMoving && flags.HasFlag(TileFlags.Burnable))
				Break(true);
		}

		// Called when the player wants to push the tile.
		public virtual bool OnPush(int direction, float movementSpeed) {
			if (isMoving)
				return false;
			
			// Make sure were not pushing out of bounds.
			Point2I newLocation = location + Directions.ToPoint(direction);
			if (!RoomControl.IsTileInBounds(newLocation))
				return false;

			// Make sure there are no obstructions.
			int newLayer = -1;
			for (int i = 0; i < RoomControl.Room.LayerCount; i++) {
				Tile t = RoomControl.GetTile(newLocation.X, newLocation.Y, i);
				if (t != null && (t.Flags.HasFlag(TileFlags.Solid) || t.Flags.HasFlag(TileFlags.NotCoverable)))
					return false;
				if (t == null && newLayer != layer)
					newLayer = i;
			}

			// Not enough layers to place this tile.
			if (newLayer < 0)
				return false;

			// Move the tile to the new location.
			isMoving = true;
			this.movementSpeed = movementSpeed;
			moveDirection = Directions.ToPoint(direction);
			offset = -Directions.ToVector(direction) * GameSettings.TILE_SIZE;
			RoomControl.MoveTile(this, newLocation, newLayer);
			return true;
		}

		public virtual bool OnDig() {
			if (!isMoving && IsDiggable) {

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

				// TOOD: spawn drops when dug.

				return true;
			}
			return false;
		}

		// Called while the player is trying to push the tile but before it's actually moved.
		public virtual void OnPushing(int direction) {

		}

		// Called when the player jumps and lands on the tile.
		public virtual void OnLand(Point2I startTile) {
			
		}
		
		// Called when a tile is finished moving after being pushed.
		public virtual void OnCompleteMovement() {
			// Check if we are over a hazard tile (water, lava, hole).
			Tile tile = null;
			for (int i = layer - 1; i >= 0 && tile == null; i--)
				tile = roomControl.GetTile(location, i);

			if (tile != null) {
				if (tile.flags.HasFlag(TileFlags.Water))
					OnFallInWater();
				else if (tile.flags.HasFlag(TileFlags.Lava))
					OnFallInLava();
				else if (tile.flags.HasFlag(TileFlags.Hole))
					OnFallInHole();
			}
		}
		
		// Called when the tile is pushed into a hole.
		public virtual void OnFallInHole() {
			if (destroyedInHoles) {
				RoomControl.SpawnEntity(new EffectFallingObject(), Center);
				RoomControl.RemoveTile(this);
			}
		}
		
		// Called when the tile is pushed into water.
		public virtual void OnFallInWater() {
			if (destroyedInWater) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_WATER_SPLASH), Center);
				RoomControl.RemoveTile(this);
			}
		}
		
		// Called when the tile is pushed into lava.
		public virtual void OnFallInLava() {
			if (destroyedInLava) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_LAVA_SPLASH), Center);
				RoomControl.RemoveTile(this);
			}
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void Break(bool spawnDrops) {
			RoomControl.SpawnEntity(new Effect(breakAnimation), Center);
			RoomControl.RemoveTile(this);

			if (spawnDrops) {
				// TEMP: this is a temporary drop list.
				string[] drops = {
					"rupees_1", "rupees_5", "hearts_1",
					"ammo_ember_seeds_5", "ammo_scent_seeds_5", "ammo_pegasus_seeds_5", "ammo_gale_seeds_5", "ammo_mystery_seeds_5",
					"ammo_bombs_5", "ammo_arrows_5"
				 };

				string dropName = drops[GRandom.NextInt(drops.Length)];
				RoomControl.GameControl.RewardManager
					.SpawnCollectibleFromBreakableTile(dropName, (Point2I) Center);
			}
		}


		//-----------------------------------------------------------------------------
		// Simulation
		//-----------------------------------------------------------------------------

		public virtual void Initialize() {}

		public virtual void Update() {
			// Update movement (after pushed).
			if (isMoving) {
				if (offset.LengthSquared > 0.0f) {
					offset += (Vector2F) moveDirection * movementSpeed;
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

			if (sprite.IsAnimation) {
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
		// Static methods
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

			tile.properties.Merge(data.BaseProperties, true);
			tile.properties.Merge(data.Properties, true);
			tile.properties.BaseProperties	= data.Properties;

			return tile;
		}

		public static Type GetType(string typeName, bool ignoreCase) {
			if (tileTypeList == null)
				tileTypeList = Assembly.GetExecutingAssembly().GetTypes();

			StringComparison comparision = StringComparison.Ordinal;
			if (ignoreCase)
				comparision = StringComparison.OrdinalIgnoreCase;

			for (int i = 0; i < tileTypeList.Length; i++) {
				if (tileTypeList[i].Name.Equals(typeName, comparision))
					return tileTypeList[i];
			}
			return null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// Returns the room control this tlie belongs to.
		public RoomControl RoomControl {
			get { return roomControl; }
			set { roomControl = value; }
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
			set { flags = value; }
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

		public bool IsDiggable {
			get { return flags.HasFlag(TileFlags.Diggable); }
		}

		public bool IsLedge {
			get { return (
				flags.HasFlag(TileFlags.LedgeRight) ||
				flags.HasFlag(TileFlags.LedgeUp) ||
				flags.HasFlag(TileFlags.LedgeLeft) ||
				flags.HasFlag(TileFlags.LedgeDown)); }
		}

		public int LedgeDirection {
			get {
				if (flags.HasFlag(TileFlags.LedgeRight))
					return Directions.Right;
				if (flags.HasFlag(TileFlags.LedgeUp))
					return Directions.Up;
				if (flags.HasFlag(TileFlags.LedgeLeft))
					return Directions.Left;
				return Directions.Down;
			}
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
	}
}
