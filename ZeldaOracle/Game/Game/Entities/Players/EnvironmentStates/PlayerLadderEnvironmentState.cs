
namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerLadderEnvironmentState : PlayerEnvironmentState {
		public PlayerLadderEnvironmentState() {
			MotionSettings.MovementSpeed		= 0.5f;
			StateParameters.ProhibitJumping		= true;
			StateParameters.AlwaysFaceUp		= true;
			StateParameters.ProhibitWeaponUse	= true;
		}

		public override void OnBegin(PlayerState previousState) {
			player.InterruptWeapons();
		}
	}
}
