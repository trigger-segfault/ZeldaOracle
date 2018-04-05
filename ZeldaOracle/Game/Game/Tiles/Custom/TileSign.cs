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
		public override bool OnAction(Direction direction) {
			string text = "";

			if (direction == Direction.Up)
				text = Properties.Get<string>("text", GameSettings.TEXT_UNDEFINED);
			else
				text = "You can't read it from there!";

			RoomControl.GameControl.DisplayMessage(text);
			return true;
		}
		

		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Properties.Set("text", "<red>undefined<red>")
				.SetDocumentation("Text", "text_message", "", "Sign", "The text to display when the sign is read from the front.");
		}
	}
}
