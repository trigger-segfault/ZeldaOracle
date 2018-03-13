using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game;

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

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Properties.SetEnumInt("color", PuzzleColor.Red)
				.SetDocumentation("Color", "enum", typeof(PuzzleColor), "Color", "The color of the block.");
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

		/*ZeldaAPI.Color ZeldaAPI.ColorBlock.Color {
			get { return (ZeldaAPI.Color) Color; }
		}*/
	}
}
