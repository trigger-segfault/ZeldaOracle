
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Debug {

	public class PlayerDebugNoClipState : PlayerState {

		public PlayerDebugNoClipState() {
			StateParameters.DisableSolidCollisions = true;
		}

		public override void Update() {
			StateParameters.DisableGravity = RoomControl.IsSideScrolling;
			StateParameters.EnableGroundOverride = RoomControl.IsSideScrolling;
		}
	}
}
