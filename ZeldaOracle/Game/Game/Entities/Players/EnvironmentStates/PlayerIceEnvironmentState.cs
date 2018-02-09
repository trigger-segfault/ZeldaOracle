
namespace ZeldaOracle.Game.Entities.Players.States {
	
	public class PlayerIceEnvironmentState : PlayerEnvironmentState {

		public PlayerIceEnvironmentState() {
			MotionSettings.MovementSpeed		= 1.0f;
			MotionSettings.IsSlippery			= true;
			MotionSettings.Acceleration			= 0.02f;
			MotionSettings.Deceleration			= 0.05f;
			MotionSettings.MinSpeed				= 0.05f;
			MotionSettings.DirectionSnapCount	= 32;
		}
	}
}
