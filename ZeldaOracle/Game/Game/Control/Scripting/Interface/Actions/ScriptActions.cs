
namespace ZeldaOracle.Game.Control.Scripting.Interface.Actions {

	public class ScriptActions : ScriptInterface, ZeldaAPI.ScriptActions {

		public ZeldaAPI.ScriptActionsGeneral	General { get; set; }
		public ZeldaAPI.ScriptActionsUnit		Unit { get; set; }
		public ZeldaAPI.ScriptActionsCamera		Camera { get; set; }
		public ZeldaAPI.ScriptActionsSound		Sound { get; set; }
		public ZeldaAPI.ScriptActionsItem		Item { get; set; }
		public ZeldaAPI.ScriptActionsTiles		Tile { get; set; }

		public ScriptActions(ScriptInstance script) : base(script) {
			General		= CreateSection(new ScriptActionsGeneral());
			Unit		= CreateSection(new ScriptActionsUnit());
			Camera		= CreateSection(new ScriptActionsCamera());
			Sound		= CreateSection(new ScriptActionsSound());
			Item		= CreateSection(new ScriptActionsItem());
			Tile		= CreateSection(new ScriptActionsTiles());
		}
	}
}
