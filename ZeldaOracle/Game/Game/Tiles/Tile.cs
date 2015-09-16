using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Tiles {
	
	[Flags]
	public enum TileFlags {
		Disabled		= 0x1,		// Tile is disabled.
		Solid			= 0x2,		// Solid tiles obstruct movement.
		Diggable		= 0x4,		// Can be dug with a shovel.
		Stairs			= 0x8,		// Stairs slow movement speed.
		Ladder			= 0x10,		// A climbable ladder disables items, and makes you face away from the screen.
		Ice				= 0x20,		// Slippery surface.
		Hole			= 0x40,		// Endless pit that objects can fall in.
		Water			= 0x80,		// Swimmable water.
		Lava			= 0x100,	// Hot lava.
		Ledge			= 0x200,	// Ledge that the player can jump off.
		NotCoverable	= 0x400,	// Tile can't be covered by movable blocks.
		Movable			= 0x800,	// Tile can be pushed around.
		Pickupable		= 0x1000,	// Tile can be picked up and carried.
		Burnable		= 0x2000,	// Destroyed by fire.
		Cuttable		= 0x4000,	// Destroyed by sword.
		Bombable		= 0x8000,	// Destroyed by bomb explosions.
		Boomerangable	= 0x10000,	// Destroyed by boomerang.
		Switchable		= 0x20000,	// Can be switched with using the Switch Hook
		SwitchStays		= 0x40000,	// Won't be destroyed when switched using the Switch Hook.
		HalfSolid		= 0x80000,	// Obstructs movement, but some projectiles can pass over the tile.

		NullFlag		= 0x10000,	// This special flags indicates that the tile is NULL.

		Default			= Diggable, // Default tile flags (assumes a typical ground tile).
	};


	public class Tile {
		private RoomControl		control;
		private Point2I			location;		// The tile location.
		private int				layer;			// The layer this tile is in.
		private Vector2F		offset;			// Offset in pixels from its tile location.
		private Point2I			size;			// How many tile spaces this tile occupies.

		private TileFlags		flags;
		private CollisionModel	collisionModel;
		private Sprite			sprite;

		private AnimationPlayer	animationPlayer;

		private Point2I			tileSheetLoc;	// TODO: this doesn't mean anything yet
		private Tileset			tileset;
		private Point2I			moveDirection;
		private bool			isMoving;
		private float			movementSpeed;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public Tile() {
			location		= Point2I.Zero;
			layer			= 0;
			offset			= Point2I.Zero;
			size			= Point2I.One;
			flags			= TileFlags.Default;
			sprite			= null;
			animationPlayer	= new AnimationPlayer();
			isMoving		= false;
		}
		
		public Tile(TileData data, int x, int y, int layer) :
			this(data, new Point2I(x, y), layer)
		{
		}
		
		public Tile(TileData data, Point2I location, int layer) :
			this()
		{
			this.location		= location;
			this.layer			= layer;
			this.flags			= data.Flags;
			this.sprite			= data.Sprite;
			this.collisionModel	= data.CollisionModel;
			this.animationPlayer.Animation = data.Animation;
		}
		
		public void Initialize(RoomControl control) {
			this.control = control;
			this.animationPlayer.Play();
		}
		

		//-----------------------------------------------------------------------------
		// Interaction
		//-----------------------------------------------------------------------------
		
		public bool Push(int direction, float movementSpeed) {
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
			offset  = -Directions.ToVector(direction) * GameSettings.TILE_SIZE;
			RoomControl.MoveTile(this, newLocation, newLayer);
			return true;
		}


		//-----------------------------------------------------------------------------
		// Simulation
		//-----------------------------------------------------------------------------

		public void Update() {
			if (isMoving) {
				if (offset.LengthSquared > 0.0f) {
					offset += (Vector2F) moveDirection * movementSpeed;
					if (offset.LengthSquared == 0.0f) {
						offset = Vector2F.Zero;
						isMoving = false;
					}
				}
			}
			animationPlayer.Update();
		}

		public void Draw(Graphics2D g) {

			if (animationPlayer.SubStrip != null) {
				// Draw as an animation.
				g.DrawAnimation(animationPlayer.SubStrip, animationPlayer.PlaybackTime, Position);
			}
			else {
				// Draw as a sprite.
				g.DrawSprite(sprite, Position);
			}

			/*
			// DEBUG: Draw the collision model in a transparent red.
			if (collisionModel != null) {
				for (int i = 0; i < collisionModel.Boxes.Count; i++) {
					Rectangle2F r = collisionModel.Boxes[i];
					r.Point += Position;
					g.FillRectangle(r, Color.Red * 0.5f);
				}
			}
			*/
		}
		

		//-----------------------------------------------------------------------------
		// Static methods
		//-----------------------------------------------------------------------------

		public static Tile CreateTile(TileData data) {
			Tile tile = new Tile();

			tile.Tileset			= data.Tileset;
			tile.TileSheetLocation	= data.SheetLocation;
			tile.Flags				= data.Flags;
			tile.Sprite				= data.Sprite;
			tile.CollisionModel		= data.CollisionModel;
			tile.Size				= data.Size;
			tile.AnimationPlayer.Animation = data.Animation;

			return tile;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// Returns the room control this entity belongs to.
		public RoomControl RoomControl {
			get { return control; }
			set { control = value; }
		}

		public Vector2F Position {
			get { return (location * GameSettings.TILE_SIZE) + offset; }
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

		public bool IsMoving {
			get { return isMoving; }
		}
	}
}
