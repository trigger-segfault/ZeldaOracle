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
			IsPersistentBetweenRooms = true;
		}

	}
}
