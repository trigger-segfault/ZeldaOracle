using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;

namespace ZeldaOracle.Game {
	/// <summary>A static class for storing links to all game content.</summary>
	public static partial class GameData {

		//-----------------------------------------------------------------------------
		// Tiles
		//-----------------------------------------------------------------------------

		[RequiresDefinition]
		public static TileData TILE_GROUND;
		[RequiresDefinition]
		public static TileData TILE_FLOOR;
		[RequiresDefinition]
		public static TileData TILE_DUG;
		[RequiresDefinition]
		public static TileData TILE_HOLE;


		//-----------------------------------------------------------------------------
		// Tile Loading
		//-----------------------------------------------------------------------------

		/// <summary>Loads "Tiles/tiles.conscript"</summary>
		private static void LoadTiles() {
			Resources.LoadTiles("Tiles/tiles.conscript");

			IntegrateResources<TileData>("TILE_");
			IntegrateResources<ActionTileData>("ACTION_");
		}

		/// <summary>Loads "Tilesets/tilesets.conscript"</summary>
		private static void LoadTilesets() {
			Resources.LoadTilesets("Tilesets/tilesets.conscript");
			// No use for integrating these resources
		}
	}
}
