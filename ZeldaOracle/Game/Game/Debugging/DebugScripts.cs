using ZeldaAPI;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Debugging {

	public class DebugScripts : ZeldaAPI.CustomScriptBase {

		public void DebugScript1() {

			foreach (var lever in room.GetTilesOfType<Lever>()) {
				Actions.Tile.FlipSwitch(lever);
			}
			foreach (ColorTile tile in room.GetTilesOfType<ColorTile>()) {
				Actions.Tile.SetColor(tile, PuzzleColor.Blue);
			}
			return;


			// Get the shopkeeper
			Unit actor = Functions.Unit.UnitByID("shopkeeper");
			if (actor == null)
				return;

			Actions.General.BeginCutscene();
			Actions.General.Wait(6);
			Actions.General.FadeScreenOut(Color.White, 8);
			Actions.General.FadeScreenIn(Color.White, 8);
			Actions.Sound.PlaySound(Functions.Sound.SoundByID("effect_cling"));
			Actions.Unit.MakeUnitFaceDirection(actor, Direction.Down);
			Actions.Unit.Jump(actor, 1.5f);
			Actions.General.Wait(50);
			Actions.Unit.MoveInDirection(actor, Direction.Down, 32, 2.0f).Wait();
			Actions.Unit.MakeUnitFaceDirection(actor, Direction.Right);
			Actions.General.Wait(2);
			Actions.General.Message("I'm sorry, sir!<p>Oh! That's a <red>Member's Card<red>! You're a member? Pardon me.<n>You may continue to the rear.");
			Actions.General.Wait(2);
			//Actions.Unit.Move(actor, Direction.Up, 32, 2.0f);
			Actions.Unit.MoveInDirection(actor, Direction.Up, 32, 0.5f).Wait();
			Actions.Unit.MakeUnitFaceDirection(actor, Direction.Right);

			Actions.General.EndCutscene();
			
			//Actions.General.BeginCutscene();
			//Actions.General.Wait(30);
			//Actions.Unit.MakeUnitFaceDirection(player, Direction.Down);
			//Actions.Item.GiveReward(Functions.Reward.RewardByID("flippers_2"));
			//Actions.General.Wait(30);
			//Actions.Unit.MakeUnitFaceDirection(player, Direction.Right);
			//Actions.General.Wait(30);
			//Actions.Unit.Move(player, Direction.Right, 32, 1.0f);
			//Actions.General.Wait(30);
			//Actions.General.EndCutscene();
		}


		public void DebugScript2() {


			//Actions.Trigger.Run();
		}

	}
}
