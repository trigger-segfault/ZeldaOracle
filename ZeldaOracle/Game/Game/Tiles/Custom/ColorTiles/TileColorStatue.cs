using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaAPI;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorStatue : Tile, IColoredTile, ZeldaAPI.ColorStatue {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorStatue() {

		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return (PuzzleColor)Properties.Get("color", (int)PuzzleColor.Red); }
		}


		//-----------------------------------------------------------------------------
		// API Implementations
		//-----------------------------------------------------------------------------

		ZeldaAPI.Color ZeldaAPI.ColorStatue.Color {
			get { return (ZeldaAPI.Color)Properties.Get("color", (int)PuzzleColor.Red); }
		}
	}
}
