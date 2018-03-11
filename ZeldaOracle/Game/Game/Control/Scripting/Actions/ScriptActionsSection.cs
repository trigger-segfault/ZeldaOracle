using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control.Scripting.Actions {

	public partial class ScriptActionsSection {

		private ScriptActions actions;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptActionsSection() {
		}


		public void LogMessage(string format, params object[] args) {
			ScriptInstance.LogMessage(format, args);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ScriptActions Actions {
			get { return actions; }
			set { actions = value; }
		}

		protected ScriptInstance ScriptInstance {
			get { return actions.ScriptInstance; }
		}

		protected RoomControl RoomControl {
			get { return actions.ScriptInstance.RoomControl; }
		}

		protected GameControl GameControl {
			get { return actions.ScriptInstance.RoomControl.GameControl; }
		}

		protected GameManager GameManager {
			get { return actions.ScriptInstance.RoomControl.GameManager; }
		}

		protected Player Player {
			get { return actions.ScriptInstance.RoomControl.Player; }
		}
	}

}
