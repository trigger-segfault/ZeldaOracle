using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Properties;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {
	
	public class Tile {

		// Internal
		private RoomControl		roomControl;
		private Point2I			location;		// The tile location in the room.
		private int				layer;			// The layer this tile is in.
		private AnimationPlayer	animationPlayer;
		private Point2I			moveDirection;
		private bool			isMoving;
		private float			movementSpeed;
		private Vector2F		offset;			// Offset in pixels from its tile location (used for movement).

		// Settings
		private TileData		tileData;		// The tile data used to create this tile.
		private TileFlags		flags;
		private Tileset			tileset;		// The tileset that this tile was created from (can be null).
		private Point2I			tileSheetLoc;	// The location on the tileset this tile was created from.
		private Point2I			size;			// How many tile spaces this tile occupies. NOTE: this isn't supported yet.
		private CollisionModel	collisionModel;
		private Sprite			sprite;
		private Sprite			spriteAsObject;	// The sprite for the tile if it were picked up, pushed, etc.
		private Animation		breakAnimation;	// The animation to play when the tile is broken.
		private int				pushDelay;		// Number of ticks of pushing before the player can move this tile.
		protected Properties	properties;

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
			sprite			= null;
			animationPlayer	= new AnimationPlayer();
			isMoving		= false;
			pushDelay		= 20;
			properties		= new Properties();
			tileData		= null;
		}


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------
		
		public void Initialize(RoomControl control) {
			this.roomControl = control;
			this.animationPlayer.Play();
			Initialize();
		}
		

		//-----------------------------------------------------------------------------
		// Virtual methods
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
			if (!isMoving && flags.HasFlag(TileFlags.Cuttable)) {
				RoomControl.SpawnEntity(new Effect(breakAnimation), Center);
				RoomControl.RemoveTile(this);
				string[] drops = {
					"rupees_1", "rupees_5", "hearts_1",
					"ammo_ember_seeds_5", "ammo_scent_seeds_5", "ammo_pegasus_seeds_5", "ammo_gale_seeds_5", "ammo_mystery_seeds_5",
					"ammo_bombs_5", "ammo_arrows_5"
				 };
				RoomControl.GameControl.RewardManager.SpawnCollectibleFromBreakableTile(drops[GRandom.NextInt(drops.Length)], (Point2I)Center);
			}
		}

		// Called when the player hits this tile with the sword.
		public virtual void OnBombExplode() {
			if (!isMoving && flags.HasFlag(TileFlags.Bombable)) {
				RoomControl.SpawnEntity(new Effect(breakAnimation), Center);
				RoomControl.RemoveTile(this);
			}
		}

		// Called when the tile is burned by a fire.
		public virtual void OnBurn() {
			if (!isMoving && flags.HasFlag(TileFlags.Burnable)) {
				RoomControl.RemoveTile(this);
			}
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


		//-----------------------------------------------------------------------------
		// Simulation
		//-----------------------------------------------------------------------------

		public virtual void Initialize() {}

		public virtual void Update() {
			// Update movement (after pushed).
			if (isMoving) {
				if (offset.LengthSquared > 0.0f) {
					offset += (Vector2F) moveDirection * movementSpeed;
					if (offset.LengthSquared == 0.0f) {
						offset = Vector2F.Zero;
						isMoving = false;
					}
				}
			}
		}

		public virtual void UpdateGraphics() {

			// Update the animation.
			animationPlayer.Update();
		}

		public virtual void Draw(Graphics2D g) {
			if (animationPlayer.SubStrip != null) {
				// Draw as an animation.
				g.DrawAnimation(animationPlayer.SubStrip, Zone.ImageVariantID,
					RoomControl.GameControl.RoomTicks, Position);
			}
			else {
				// Draw as a sprite.
				Sprite spr = sprite;
				if (isMoving && spriteAsObject != null)
					spr = spriteAsObject;
				g.DrawSprite(spr, Zone.ImageVariantID, Position);
			}
		}
		

		//-----------------------------------------------------------------------------
		// Static methods
		//-----------------------------------------------------------------------------

		// Instantiate a tile from the given tile-data.
		public static Tile CreateTile(TileData data) {
			Tile tile;
			
			// Construct the tile.
			if (data.Type == null)
				tile = new Tile();
			else
				tile = (Tile) data.Type.GetConstructor(Type.EmptyTypes).Invoke(null);
			
			tile.tileData			= data;
			tile.tileset			= data.Tileset;
			tile.tileSheetLoc		= data.SheetLocation;
			tile.flags				= data.Flags;
			tile.sprite				= data.Sprite;
			tile.spriteAsObject		= data.SpriteAsObject;
			tile.breakAnimation		= data.BreakAnimation;
			tile.collisionModel		= data.CollisionModel;
			tile.size				= data.Size;
			tile.animationPlayer.Animation = data.Animation;

			tile.properties.Merge(data.Properties, true);
			tile.properties.Merge(data.ModifiedProperties, true);

			return tile;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// Returns the room control this entity belongs to.
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

		public Sprite Sprite {
			get { return sprite; }
			set { sprite = value; }
		}

		public Animation BreakAnimation {
			get { return breakAnimation; }
			set { breakAnimation = value; }
		}

		public Sprite SpriteAsObject {
			get { return spriteAsObject; }
			set { spriteAsObject = value; }
		}

		public AnimationPlayer AnimationPlayer {
			get { return animationPlayer; }
			set { animationPlayer = value; }
		}

		public CollisionModel CollisionModel {
			get { return collisionModel; }
			set { collisionModel = value; }
		}

		public Tileset Tileset {
			get { return tileset; }
			set { tileset = value; }
		}

		public Point2I TileSheetLocation {
			get { return tileSheetLoc; }
			set { tileSheetLoc = value; }
		}

		public int PushDelay {
			get { return pushDelay; }
			set { pushDelay = value; }
		}

		public bool IsMoving {
			get { return isMoving; }
		}

		public int LedgeDirection {
			get {
				if (collisionModel == GameData.MODEL_EDGE_W)
					return Directions.West;
				if (collisionModel == GameData.MODEL_EDGE_E)
					return Directions.East;
				if (collisionModel == GameData.MODEL_EDGE_N)
					return Directions.North;
				if (collisionModel == GameData.MODEL_EDGE_S)
					return Directions.South;
				return Directions.South;
			}
		}

		public Properties Properties {
			get { return properties; }
		}
		
		// Get the original tile data from which this was created.
		public TileData TileData {
			get { return tileData; }
		}
		
		// Get the modified properties of the tile data from which this was created.
		// Do not access these properties, only modify them.
		public Properties BaseProperties {
			get { return tileData.ModifiedProperties; }
		}
	}
}
