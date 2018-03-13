using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control.Scripting.Interface {

	public partial class ScriptInterfaceSection {

		private ScriptInterface scriptInterface;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptInterfaceSection() {

		}

		public void LogMessage(string format, params object[] args) {
			ScriptInstance.LogMessage(format, args);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ScriptInterface ScriptInterface {
			get { return scriptInterface; }
			set { scriptInterface = value; }
		}

		protected ScriptInstance ScriptInstance {
			get { return scriptInterface.ScriptInstance; }
		}

		protected RoomControl RoomControl {
			get { return scriptInterface.ScriptInstance.RoomControl; }
		}

		protected GameControl GameControl {
			get { return scriptInterface.ScriptInstance.RoomControl.GameControl; }
		}

		protected GameManager GameManager {
			get { return scriptInterface.ScriptInstance.RoomControl.GameManager; }
		}

		protected Player Player {
			get { return scriptInterface.ScriptInstance.RoomControl.Player; }
		}
	}

}
