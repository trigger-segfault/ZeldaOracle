using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorTile : Tile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorTile() {

		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			// Set the sprite.
			PuzzleColor color = Color;
			if (color == PuzzleColor.Red)
				CustomSprite = GameData.SPR_TILE_COLOR_TILE_RED;
			else if (color == PuzzleColor.Yellow)
				CustomSprite = GameData.SPR_TILE_COLOR_TILE_YELLOW;
			else if (color == PuzzleColor.Blue)
				CustomSprite = GameData.SPR_TILE_COLOR_TILE_BLUE;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return (PuzzleColor) Properties.Get("color", (int) PuzzleColor.Red); }
			set {
				if (Properties.Get("remember_state", false))
					Properties.SetBase("color", (int) value);
				else
					Properties.Set("color", (int) value);
			}
		}

	}
}
