using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Units;

namespace ZeldaOracle.Game.Control.Scripting.Actions {

	public class ScriptActionsGeneral :
		ScriptInterfaceSection, ZeldaAPI.ScriptActionsGeneral
	{

		public void WaitForCondition(ZeldaAPI.WaitCondition condition) {
			LogMessage("Waiting for condition");
			ScriptInstance.WaitForCondition(condition);
		}
		
		public void Wait(int ticks) {
			LogMessage("Waiting for {0} ticks", ticks);
			int startWaitTime = RoomControl.GameManager.ElapsedTicks;
			ScriptInstance.WaitForCondition(delegate() {
				return (RoomControl.GameManager.ElapsedTicks - startWaitTime >= ticks);
			});
		}
		
		public void WaitSeconds(float seconds) {
			LogMessage("Waiting for {0} seconds", seconds);
			int ticks = (int) ((seconds * 60.0f) + 0.5f);
			Wait(ticks);
		}

		public void Message(string text) {
			LogMessage("Displaying message: {0}", text);
			RoomControl.GameControl.DisplayMessage(text, null, ScriptInstance.Resume);
			ScriptInstance.WaitForResume();
		}

		public void BeginCutscene() {
			LogMessage("Beginning cutscene");
			Player.BeginControlState(new PlayerCutsceneState());
		}
		
		public void EndCutscene() {
			LogMessage("Ending cutscene");
			Player.EndControlState();
		}
	}
}
