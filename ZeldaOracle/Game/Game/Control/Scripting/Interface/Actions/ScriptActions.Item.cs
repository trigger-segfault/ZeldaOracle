using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Control.Scripting.Interface.Actions {

	public class ScriptActionsItem :
		ScriptInterfaceSection, ZeldaAPI.ScriptActionsItem
	{
		public void GiveReward(ZeldaAPI.Reward reward) {
			LogMessage("Giving player the reward {0}", reward);
			RoomControl.GameControl.PushRoomState(
				new RoomStateReward((Reward) reward, ScriptInstance.Resume));
			ScriptInstance.WaitForResume();
		}
	}
}
