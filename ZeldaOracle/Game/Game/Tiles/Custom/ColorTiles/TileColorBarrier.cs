using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorBarrier : Tile {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorBarrier() {

		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			PuzzleColor color = Color;

			if (color == PuzzleColor.Red) {
				IsSolid = false;
				CustomSprite = GameData.SPR_TILE_COLOR_BARRIER_RED_LOWERED;
			}
			else if (color == PuzzleColor.Blue) {
				IsSolid = true;
				CustomSprite = GameData.SPR_TILE_COLOR_BARRIER_BLUE_RAISED;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return (PuzzleColor) Properties.Get("color", (int) PuzzleColor.Red); }
		}

	}
}
