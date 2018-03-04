using ZeldaOracle.Game.Entities.Units;

namespace ZeldaOracle.Game.Entities.Players.Tools {
	public class PlayerToolVisual : UnitTool {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PlayerToolVisual() {
			toolType = UnitToolType.Visual;
			syncAnimationWithDirection = false;
			IsPersistentBetweenRooms = true;
		}

	}
}
