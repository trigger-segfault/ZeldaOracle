using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Tiles {
	
	public enum TileEnvironmentType {
		Normal = 0,
		Stairs,
		Ladder,
		Ice,
		Puddle,
		Grass,
		Hole,
		Water,
		DeepWater,	// (+water)
		Ocean,		// (+water)
		Waterfall,
		Lava,
		Lavafall,
		Whirlpool,	// (+water +hole)
	}

	public enum TileSolidType {
		NotSolid = 0,
		Solid,
		HalfSolid,	// Not solid to units, and certain projectiles can pass over half solids. Half solids include railings.
		Ledge,
	}

	public struct TileSpawnOptions {
		public bool PoofEffect { get; set; }
		public int SpawnDelayAfterPoof { get; set; }

	}

	[Flags]
	public enum TileFlags {
		None			= 0,
		Default			= 0,			// Default tile flags.
				
		// General.
		Movable			= 0x1,		// Tile can be pushed around.
		Pickupable		= 0x2,		// Tile can be picked up and carried.
		Burnable		= 0x4,		// Destroyed by fire.
		Cuttable		= 0x8,		// Destroyed by sword.
		Bombable		= 0x10,		// Destroyed by bomb explosions.
		Boomerangable	= 0x20,		// Destroyed by boomerang.
		Switchable		= 0x40,		// Can be switched with using the Switch Hook.
		SwitchStays		= 0x80,		// Won't be destroyed when switched using the Switch Hook.
		Digable			= 0x100,	// Can be dug with a shovel.
		
		NotCoverable	= 0x200,			// Tile can't be covered by movable blocks.
		NotPushable		= 0x400,			// Player will not use the pushing animation when walking into the tile.

		// Item/player interactions.
		NotGrabbable	= 0x800,		// The player cannot grab the tile with the power bracelet.
		AbsorbSeeds		= 0x1000,		// Seeds will be destroyed when hitting this tile instead of bouncing off.

		// General. NOTE: moved to enum TileSolidType
		//Solid			= 0x1,			// Solid tiles obstruct movement.
		//HalfSolid		= 0x2,			// Obstructs movement, but some projectiles can pass over the tile. Solid flag should also be active.

		// Ledges. NOTE: moved to integer property "ledge_direction"
		/*
		LedgeRight		= 0x10,			// Ledge that the player can jump off to the east.
		LedgeEast		= 0x10,
		LedgeUp			= 0x20,			// Ledge that the player can jump off to the north.
		LedgeNorth		= 0x20,
		LedgeLeft		= 0x40,			// Ledge that the player can jump off to the west.
		LedgeWest		= 0x40,
		LedgeDown		= 0x80,			// Ledge that the player can jump off to the south.
		LedgeSouth		= 0x80,
		*/

		// These flags have been moved to the enum TileEnvironmentType.
		/*Ice				= 0x100,		// Slippery surface.
		Stairs			= 0x200,		// Stairs slow movement speed.
		Ladder			= 0x400,		// A climbable ladder disables items, and makes you face away from the screen.
		Hole			= 0x800,		// Endless pit that objects can fall in. The Water and Ocean flag should also be specified for whirlpools.
		Water			= 0x1000,		// Swimmable water.
		Waterfall		= 0x2000,		// A Ledge flag should also be active.
		Ocean			= 0x4000,		// The water flag should also be active.
		Lava			= 0x8000,		// Hot lava.
		Puddle			= 0x10000,		// Draw the ripples effect on entities
		Grass			= 0x20000,*/		// Draw the grass effect on entities.

		//Surface			= 0x40000,		// Does this tile count as a walkable surface?
	}
}
