using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Tiles {

	public class TileSign : Tile {


		public TileSign() {

		}
		
		// Called when the player presses A on this tile, when facing the given direction.
		public override void OnAction(int direction) {
			if (direction == Directions.Up)
				RoomControl.GameControl.DisplayMessage("It's so hard to cook meals for morning, noon, and night!");
			else
				RoomControl.GameControl.DisplayMessage("You can't read it from here!");
		}


	}
}
