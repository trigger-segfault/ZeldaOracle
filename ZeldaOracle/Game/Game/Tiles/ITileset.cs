using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;

namespace ZeldaOracle.Game.Tiles {

	/// <summary>A tileset interface used by tile-based tilesets and event tile-based tilesets.</summary>
	public interface ITileset : IIDObject {
		/// <summary>Return the tile data at the given location.</summary>
		BaseTileData GetTileData(int x, int y);

		/// <summary>Return the tile data at the given location.</summary>
		BaseTileData GetTileData(Point2I location);
		
		/// <summary>The size of an individual tile cell.</summary>
		//Point2I CellSize { get; }

		/// <summary>The amount of spacing between tiles.</summary>
		Point2I Spacing { get; }

		/// <summary>The dimensions of the tileset.</summary>
		Point2I Dimensions { get; }

		/// <summary>Get the width of the tileset.</summary>
		int Width { get; }

		/// <summary>Get the height of the tileset.</summary>
		int Height { get; }

		/// <summary>The sprite sheet that represents the tileset.</summary>
		//SpriteSheet SpriteSheet { get; }

		/// <summary>Gets or sets if the existing 16x16 preview sprites are used to display tiles.</summary>
		bool UsePreviewSprites { get; }
	}
}
