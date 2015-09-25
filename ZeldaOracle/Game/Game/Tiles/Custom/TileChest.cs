using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Tiles.Custom {
	public class TileChest : Tile {

		Reward reward;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileChest() {

		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		// Called when the player presses A on this tile, when facing the given direction.
		public override bool OnAction(int direction) {
			if (direction == Directions.Up)
				RoomControl.GameControl.DisplayMessage("It's so hard to cook meals for morning, noon, and night!");
			else
				RoomControl.GameControl.DisplayMessage("You can't read it from here!");
			return true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		

	}
}
