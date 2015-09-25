using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.GameStates.RoomStates {
	public class RoomStateNormal : RoomState {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomStateNormal() {
			updateRoom		= true;
			animateRoom		= true;
		}
	}
}
