using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Tiles {
	
	[Flags]
	public enum TileFlags : ulong {
		None			= 0,
		Disabled		= 0x1,			// Tile is disabled.

		// General.
		Solid			= 0x2,			// Solid tiles obstruct movement.
		HalfSolid		= 0x4,			// Obstructs movement, but some projectiles can pass over the tile.
		NotCoverable	= 0x8,			// Tile can't be covered by movable blocks.
		
		// Environmental.
		//Ledge			= 0x10,			// Ledge that the player can jump off.
		Ice				= 0x20,			// Slippery surface.
		Stairs			= 0x40,			// Stairs slow movement speed.
		Ladder			= 0x80,			// A climbable ladder disables items, and makes you face away from the screen.
		Hole			= 0x100,		// Endless pit that objects can fall in.
		Water			= 0x200,		// Swimmable water.
		Waterfall		= 0x400,
		Ocean			= 0x800,
		Lava			= 0x1000,		// Hot lava.
		Puddle			= 0x2000,		// Draw the ripples effect on entities
		Grass			= 0x4000,		// Draw the grass effect on entities.
		
		// Item/player interactions.
		Movable			= 0x8000,		// Tile can be pushed around.
		Pickupable		= 0x10000,		// Tile can be picked up and carried.
		Burnable		= 0x20000,		// Destroyed by fire.
		Cuttable		= 0x40000,		// Destroyed by sword.
		Bombable		= 0x80000,		// Destroyed by bomb explosions.
		Boomerangable	= 0x100000,		// Destroyed by boomerang.
		Switchable		= 0x200000,		// Can be switched with using the Switch Hook
		SwitchStays		= 0x400000,		// Won't be destroyed when switched using the Switch Hook.
		Diggable		= 0x8000000,	// Can be dug with a shovel.
		
		LedgeRight		= 0x10000000,
		LedgeUp			= 0x20000000,
		LedgeLeft		= 0x40000000,
		LedgeDown		= 0x80000000,
		
		//Surface			= 0x100000000,	// Does this tile count as a walkable surface?
		NotGrabbable	= 0x200000000,
		AbsorbSeeds		= 0x400000000,

		Default			= 0,			// Default tile flags.
	}
}
