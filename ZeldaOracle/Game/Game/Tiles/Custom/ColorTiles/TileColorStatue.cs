using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game;

namespace ZeldaOracle.Game.Tiles {

	public class TileColorStatue : Tile, IColoredTile, ZeldaAPI.ColorStatue {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileColorStatue() {

		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Flags |= TileFlags.Movable;

			data.Properties.SetEnumInt("color", PuzzleColor.Red)
				.SetDocumentation("Color", "enum", typeof(PuzzleColor), "Color", "The color of the statue.");
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PuzzleColor Color {
			get { return Properties.GetEnum<PuzzleColor>("color", PuzzleColor.Red); }
			set { }
		}


		//-----------------------------------------------------------------------------
		// API Implementations
		//-----------------------------------------------------------------------------

		/*ZeldaAPI.Color ZeldaAPI.ColorStatue.Color {
			get { return (ZeldaAPI.Color)Properties.Get("color", (int)PuzzleColor.Red); }
		}*/
	}
}
