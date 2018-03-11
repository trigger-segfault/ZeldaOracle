using System.Collections.Generic;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control.Scripting.Actions {

	public partial class ScriptActions : ZeldaAPI.ScriptActions {

		private ScriptInstance script;
		private List<ScriptActionsSection> sections;

		public ZeldaAPI.ScriptActionsGeneral	General { get; set; }
		public ZeldaAPI.ScriptActionsUnit		Unit { get; set; }
		public ZeldaAPI.ScriptActionsCamera		Camera { get; set; }


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptActions(ScriptInstance script) {
			this.script = script;
			sections = new List<ScriptActionsSection>();

			General		= CreateSection(new ScriptActionsGeneral());
			Unit		= CreateSection(new ScriptActionsUnit());
			Camera		= CreateSection(new ScriptActionsCamera());
		}

		private T CreateSection<T>(T section) where T : ScriptActionsSection {
			section.Actions = this;
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
