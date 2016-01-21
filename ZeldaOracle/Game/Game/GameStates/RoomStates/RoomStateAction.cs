using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.GameStates.RoomStates {

	// A room state that performs a single action then ends.
	public class RoomStateAction : RoomState {

		private Action action;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomStateAction(Action action) {
			this.action = action;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			if (action != null)
				action();
			End();
		}
	}
}
