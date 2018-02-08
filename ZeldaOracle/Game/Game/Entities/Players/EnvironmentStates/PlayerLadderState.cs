
namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerLadderState : PlayerEnvironmentState {
		public PlayerLadderState() {
			MotionSettings.MovementSpeed		= 0.5f;
			StateParameters.ProhibitJumping		= true;
			StateParameters.AlwaysFaceUp		= true;
			StateParameters.ProhibitWeaponUse	= true;
		}

		public override void OnBegin(PlayerState previousState) {
			player.InterruptWeapons();
		}
	}

	public class PlayerEnvironmentStateSidescrollLadder : PlayerEnvironmentState {
		public PlayerEnvironmentStateSidescrollLadder() {
			StateParameters.DisableGravity			= true;
			StateParameters.EnableGroundOverride	= true;
			StateParameters.AlwaysFaceUp			= true;
		}
	}
}
