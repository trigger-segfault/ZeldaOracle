using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;

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
				text = "You can't read it from there!";

			RoomControl.GameControl.DisplayMessage(text);
			return true;
		}



		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			Tile.DrawTileData(g, args);
		}
	}
}
