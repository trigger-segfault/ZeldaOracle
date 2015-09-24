using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Items {
	[Flags]
	public enum ItemFlags {

		None				= 0x0,

		TwoHanded			= 0x1,
		
		UsableInMinecart	= 0x2,
		UsableWhileJumping	= 0x4,
		UsableWithSword		= 0x8,
		UsableUnderwater	= 0x10,
	}
}
