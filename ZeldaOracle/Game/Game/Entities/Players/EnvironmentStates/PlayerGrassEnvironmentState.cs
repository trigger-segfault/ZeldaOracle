
namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerGrassEnvironmentState : PlayerEnvironmentState {
		public PlayerGrassEnvironmentState() {
			MotionSettings.MovementSpeed = 0.75f;
			StateParameters.ProhibitJumping = true;
		}
	}
}
