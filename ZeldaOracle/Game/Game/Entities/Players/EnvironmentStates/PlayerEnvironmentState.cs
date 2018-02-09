
namespace ZeldaOracle.Game.Entities.Players.States {
	
	public class PlayerEnvironmentState : PlayerState {

		private PlayerMotionType motionSettings;

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		public PlayerEnvironmentState() {
			motionSettings = new PlayerMotionType();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public PlayerMotionType MotionSettings {
			get { return motionSettings; }
			set { motionSettings = value; }
		}
	}
}
