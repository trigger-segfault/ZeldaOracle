using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Tiles {
	
	[Flags]
	public enum TileSpecialFlags : ulong {
		None			= 0,
		Default			= 0,			// Default tile flags.

		// General.
		Movable			= 0x1,			// Tile can be pushed around.

		// TileInteractable.
		Pickupable		= 0x2,			// Tile can be picked up and carried.
		Burnable		= 0x4,			// Destroyed by fire.
		Cuttable		= 0x8,			// Destroyed by sword.
		Bombable		= 0x10,			// Destroyed by bomb explosions.
		Boomerangable	= 0x20,			// Destroyed by boomerang.
		Switchable		= 0x40,			// Can be switched with using the Switch Hook.
		SwitchStays		= 0x80,			// Won't be destroyed when switched using the Switch Hook.

		// TileRoller.
		VerticalRoller	= 0x100,		// The roller will roll in vertical directions only instead of horizontal directions only.
	}
}
