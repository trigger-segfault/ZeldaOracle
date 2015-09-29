using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Properties;

namespace ZeldaOracle.Game.Tiles {

	public class TileSign : Tile {


		public TileSign() {

		}
		
		// Called when the player presses A on this tile, when facing the given direction.
		public override bool OnAction(int direction) {
			string text = "";

			if (direction == Directions.Up)
				text = Properties.GetString("text", GameSettings.TEXT_UNDEFINED);
			else
				text = Properties.GetString("text_side", GameSettings.TEXT_UNDEFINED);

			RoomControl.GameControl.DisplayMessage(text);
			return true;
		}


	}
}
