using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Players {
	[Flags]
	public enum PlayerSwimmingSkills {

		CantSwim		= 0x0,
		CanSwimInWater	= 0x1,
		CanSwimInOcean	= 0x2,
		CanSwimInLava	= 0x4
	}
}
