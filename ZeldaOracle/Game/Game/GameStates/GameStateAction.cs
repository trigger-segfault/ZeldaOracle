using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.GameStates {

	// A game state that performs a single action and ends.
	public class GameStateAction : GameState {

		private Action<GameStateAction> action;

		public GameStateAction(Action<GameStateAction> action) {
			this.action = action;
		}
		
		public override void OnBegin() {
			if (action != null) {
				action(this);
				End();
			}
		}
	}
}
