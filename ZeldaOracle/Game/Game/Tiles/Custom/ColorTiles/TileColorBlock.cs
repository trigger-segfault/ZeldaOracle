using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorBlock : Tile, IColoredTile, ZeldaAPI.ColorBlock {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorBlock() {

		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			Tile.DrawTileData(g, args);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return Properties.GetEnum<PuzzleColor>("color", PuzzleColor.Red); }
		}
		
		
		//-----------------------------------------------------------------------------
		// API Implementations
		//-----------------------------------------------------------------------------

		ZeldaAPI.Color ZeldaAPI.ColorBlock.Color {
			get { return (ZeldaAPI.Color) Color; }
		}
	}
}
