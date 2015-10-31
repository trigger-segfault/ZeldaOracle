using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Tiles {

	public interface ITileset {
		// Return the tile data at the given location.
		BaseTileData GetTileData(int x, int y);

		// Return the tile data at the given location.
		BaseTileData GetTileData(Point2I location);

		// The string identifier for the tileset.
		string ID { get; }

		// The dimensions of the tileset.
		Point2I Size { get; }

		// The size of an individual tile cell.
		Point2I CellSize { get; }
		
		// The amount of spacing between tiles.
		Point2I Spacing { get; }
		
		// Get the width of the tileset.
		int Width { get; }
		
		// Get the height of the tileset.
		int Height { get; }

		// The sprite sheet that represents the tileset.
		SpriteSheet SpriteSheet { get; }
	}
}
