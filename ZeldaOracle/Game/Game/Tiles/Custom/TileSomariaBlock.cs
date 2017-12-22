using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game.Tiles {

	public class TileSomariaBlock : Tile {


		public TileSomariaBlock() {

		}

		public override void OnInitialize() {
			DropList = null;
		}

		public override void OnFallInHole() {
			Break(false);
		}
		
		public override void OnFallInWater() {
			Break(false);
		}
		
		public override void OnFallInLava() {
			Break(false);
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
