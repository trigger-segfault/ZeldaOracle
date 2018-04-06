using ZeldaAPI;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Debugging {

	public class DebugScripts : ZeldaAPI.CustomScriptBase {

		public void DebugScript1() {

			int option = Actions.General.Message(
				"Do you like trains?<p>   <1>Yes <0>No<n>  <2>Maybe <3>...");
			if (option == 0)
				Actions.General.Message("Fuck you trains are dope");
			else if (option == 1)
				Actions.General.Message("Yeaaaaaahhhh");
			else if (option == 2)
				Actions.General.Message("What?");
			else if (option == 3)
				Actions.General.Message("...");


			/*
			NPC self = (NPC) Functions.Unit.UnitByID("bear");

			if (self.Vars.Get<bool>("talked_to_bird") &&
				self.Vars.Get<bool>("talked_to_bunny"))
			{
				// Cutscene: Bear crawls up 1 tile
				Actions.General.BeginCutscene();
	
				// Face up
				Actions.Unit.MakeUnitFaceDirection(self, Direction.Up);
	
				// Move up
				Actions.General.Wait(24);
				Actions.Unit.MoveInDirection(self,
					Direction.Up, 16, 0.5f).Wait();
		
				// Face right
				Actions.General.Wait(22);
				Actions.Unit.MakeUnitFaceDirection(self, Direction.Right);
	
				Actions.General.Wait(34);
				Actions.General.EndCutscene();
	
				self.Text = "Sit here and listen. How charming...";
				Actions.General.Message(self.Text);
				
				Actions.Trigger.TurnOff(Functions.Trigger.ThisTrigger);
			}
			else {
				Actions.General.Message("How charming...");
			}
			*/

			//foreach (var lever in room.GetTilesOfType<Lever>()) {
			//	Actions.Tile.FlipSwitch(lever);
			//}
			//foreach (ColorTile tile in room.GetTilesOfType<ColorTile>()) {
			//	Actions.Tile.SetColor(tile, PuzzleColor.Blue);
			//}
			//return;

			//NPC This;
			
			//Actions.Trigger.TurnOff(Functions.Trigger.TriggerByName(This, "talk"));

			//This.Text = "How charming..."

			// Get the shopkeeper
			/*Unit actor = Functions.Unit.UnitByID("shopkeeper");
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

			Actions.General.EndCutscene();*/

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
