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
		Default			= 0,		// Default tile flags (Flags are designed so this value is 0)
				
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

		NotCoverable	= 0x200,	// Tile can't be covered by movable blocks.
		NotPushable		= 0x400,	// Player will not use the pushing animation when walking into the tile.

		// Item/player interactions.
		NotGrabbable	= 0x800,	// The player cannot grab the tile with the power bracelet.
		AbsorbSeeds		= 0x1000,	// Seeds will be destroyed when hitting this tile instead of bouncing off.
		InstantPickup	= 0x2000,	// The tile is picked up instantly, without having to pull on it.
		HurtPlayer		= 0x4000,	// Hurt the player when he touches the tile.
	}
}
