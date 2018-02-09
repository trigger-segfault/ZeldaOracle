
namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerJumpEnvironmentState : PlayerEnvironmentState {

		public PlayerJumpEnvironmentState() {
			StateParameters.ProhibitLedgeJumping	= true;
			StateParameters.ProhibitRoomTransitions	= true;
			StateParameters.EnableStrafing			= true;
			StateParameters.ProhibitPushing			= true;

			MotionSettings.MovementSpeed		= 1.0f;
			MotionSettings.IsSlippery			= true;
			MotionSettings.Acceleration			= 0.33f;
			MotionSettings.Deceleration			= 0.00f;
			MotionSettings.MinSpeed				= 0.05f;
			MotionSettings.DirectionSnapCount	= 8;
		}
	}
}
