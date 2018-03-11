using ZeldaAPI;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Debugging {

	public class DebugScripts : ZeldaAPI.CustomScriptBase {

		public void DebugScript1() {
			// Get the shopkeeper
			Unit actor = Functions.Unit.UnitByID("shopkeeper");
			if (actor == null)
				return;

			Actions.General.BeginCutscene();
			Actions.General.Wait(6);
			Actions.Sound.PlaySound(Functions.Sound.SoundByID("effect_cling"));
			Actions.Unit.MakeUnitFaceDirection(actor, Direction.Down);
			Actions.Unit.Jump(actor, 1.5f);
			Actions.General.Wait(50);
			Actions.Unit.Move(actor, Direction.Down, 32, 2.0f);
			Actions.Unit.MakeUnitFaceDirection(actor, Direction.Right);
			Actions.General.Wait(2);
			Actions.General.Message("I'm sorry, sir!<p>Oh! That's a <red>Member's Card<red>! You're a member? Pardon me.<n>You may continue to the rear.");
			Actions.General.Wait(2);
			Actions.Unit.Move(actor, Direction.Up, 32, 2.0f);
			Actions.Unit.MakeUnitFaceDirection(actor, Direction.Right);
			Actions.General.EndCutscene();
		}


	}
}
