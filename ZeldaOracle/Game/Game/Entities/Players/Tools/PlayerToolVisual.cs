using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Units;

namespace ZeldaOracle.Game.Entities.Players.Tools {
	public class PlayerToolVisual : UnitTool {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerToolVisual() {
			toolType = UnitToolType.Visual;
			IsPhysicsEnabled = false;
			syncAnimationWithDirection = false;
		}

	}
}
