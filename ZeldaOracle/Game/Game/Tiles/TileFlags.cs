using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Tiles {
	
	[Flags]
	public enum TileFlags : ulong {
		None			= 0,
		Default			= 0,			// Default tile flags.

		// General.
		Solid			= 0x1,			// Solid tiles obstruct movement.
		HalfSolid		= 0x2,			// Obstructs movement, but some projectiles can pass over the tile. Solid flag should also be active.
		NotCoverable	= 0x4,			// Tile can't be covered by movable blocks.
		NotPushable		= 0x8,			// Player will not use the pushing animation when walking into the tile.

		// Environmental.
		LedgeRight		= 0x10,			// Ledge that the player can jump off to the east.
		LedgeEast		= 0x10,
		LedgeUp			= 0x20,			// Ledge that the player can jump off to the north.
		LedgeNorth		= 0x20,
		LedgeLeft		= 0x40,			// Ledge that the player can jump off to the west.
		LedgeWest		= 0x40,
		LedgeDown		= 0x80,			// Ledge that the player can jump off to the south.
		LedgeSouth		= 0x80,
		Ice				= 0x100,		// Slippery surface.
		Stairs			= 0x200,		// Stairs slow movement speed.
		Ladder			= 0x400,		// A climbable ladder disables items, and makes you face away from the screen.
		Hole			= 0x800,		// Endless pit that objects can fall in. The Water and Ocean flag should also be specified for whirlpools.
		Water			= 0x1000,		// Swimmable water.
		Waterfall		= 0x2000,		// A Ledge flag should also be active.
		Ocean			= 0x4000,		// The water flag should also be active.
		Lava			= 0x8000,		// Hot lava.
		Puddle			= 0x10000,		// Draw the ripples effect on entities
		Grass			= 0x20000,		// Draw the grass effect on entities.
		Surface			= 0x40000,		// Does this tile count as a walkable surface?

		// Item/player interactions.
		Diggable		= 0x100000,		// Can be dug with a shovel.
		NotGrabbable	= 0x200000,		// The player cannot grab the tile with the power bracelet.
		AbsorbSeeds		= 0x400000,		// Seeds will be destroyed when hitting this tile instead of bouncing off.
	}
}
