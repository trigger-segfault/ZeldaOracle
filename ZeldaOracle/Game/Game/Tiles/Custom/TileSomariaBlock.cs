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
			// TODO: Break when getting crushed by Thwomp. (Only when moving)
		}

		public override void OnInitialize() {
			DropList = null;
			CancelBreakSound = true;
			CheckSurfaceTile();
			CancelBreakSound = false;
			// TODO: Prevent spawning on top of entities
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

		public override void OnFloating() {
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
