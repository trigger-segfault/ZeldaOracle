using System.Collections.Generic;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control.Scripting {

	public partial class ScriptInterface {

		private ScriptInstance script;
		private List<ScriptInterfaceSection> sections;
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptInterface(ScriptInstance script) {
			this.script = script;
			sections = new List<ScriptInterfaceSection>();
		}

		protected T CreateSection<T>(T section) where T : ScriptInterfaceSection {
			section.ScriptInterface = this;
			sections.Add(section);
			return section;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ScriptInstance ScriptInstance {
			get { return script; }
			set { script = value; }
		}

		private RoomControl RoomControl {
			get { return script.RoomControl; }
		}

		private GameControl GameControl {
			get { return script.RoomControl.GameControl; }
		}

		private GameManager GameManager {
			get { return script.RoomControl.GameManager; }
		}

		private Player Player {
			get { return script.RoomControl.Player; }
		}
	}

}
