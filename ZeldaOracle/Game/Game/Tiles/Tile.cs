using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Tiles {
	
	[Flags]
	public enum TileFlags
	{
		TILE_DISABLED		= 0x1,		// Tile is disabled.
		TILE_SOLID			= 0x2,		// Solid tiles obstruct movement.
		TILE_DIGGABLE		= 0x4,		// Can be dug with a shovel.
		TILE_STAIRS			= 0x8,		// Stairs slow movement speed.
		TILE_LADDER			= 0x10,		// A climbable ladder disables items, and makes you face away from the screen.
		TILE_ICE			= 0x20,		// Slippery surface.
		TILE_HOLE			= 0x40,		// Endless pit that objects can fall in.
		TILE_WATER			= 0x80,		// Swimmable water.
		TILE_LAVA			= 0x100,	// Hot lava.
		TILE_LEDGE			= 0x200,	// Ledge that the player can jump off.
		TILE_NOT_COVERABLE	= 0x400,	// Tile can't be covered by movable blocks.
		TILE_MOVABLE		= 0x800,	// Tile can be pushed around.
		TILE_PICKUPABLE		= 0x1000,	// Tile can be picked up and carried.
		TILE_BURNABLE		= 0x2000,	// Destroyed by fire.
		TILE_CUTTABLE		= 0x4000,	// Destroyed by sword.
		TILE_BOMBABLE		= 0x8000,	// Destroyed by bomb explosions.
		TILE_BOOMERANGABLE	= 0x10000,	// Destroyed by boomerang.
		TILE_SWITCHABLE		= 0x20000,	// Can be switched with using the Switch Hook
		TILE_SWITCH_STAYS	= 0x40000,	// Won't be destroyed when switched using the Switch Hook.
		TILE_HALF_SOLID		= 0x80000,	// Obstructs movement, but some projectiles can pass over the tile.

		TILE_NULL_FLAG		= 0x10000,	// This special flags indicates that the tile is NULL.

		TILE_DEFAULT_FLAGS = TILE_DIGGABLE, // Default tile flags (assumes a typical ground tile).
	};


	public class Tile {
		private Point2I			location;		// The tile location.
		private Vector2F		offset;			// Offset in pixels from its tile location.
		private Point2I			tileSheetLoc;	// TODO: this doesn't mean anything yet
		private Point2I			size;
	
		private TileFlags		flags;
		private Sprite			sprite;
		private AnimationPlayer	animationPlayer;
		//private CollisionModel*	collisionModel;
		//private Tileset*		tileset;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Tile() {

		}
		

		//-----------------------------------------------------------------------------
		// Simulation
		//-----------------------------------------------------------------------------

		public void Update(float timeDelta) {
			animationPlayer.Update(timeDelta);
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
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Vector2F Position {
			get { return (location * GameSettings.TILE_SIZE) + offset; }
			set { offset = value - (location * GameSettings.TILE_SIZE); }
		}

		public Vector2F Offset {
			get { return offset; }
			set { offset = value; }
		}

		public Point2I Location {
			get { return location; }
			set { location = value; }
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
			set { Sprite = value; }
		}

		public AnimationPlayer AnimationPlayer {
			get { return animationPlayer; }
			set { animationPlayer = value; }
		}
	}
}
