using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds.Editing;

namespace ZeldaEditor.Undo {
	public class ActionSquare : EditorAction {

		private Level level;
		private int layer;
		private Rectangle2I square;
		private TileData placedTile;
		private Dictionary<Point2I, TileDataInstance> overwrittenTiles;
		//private TileDataInstance[,] overwrittenTileGrid;
		private TileDataInstance[,] placedTiles;

		public ActionSquare(Level level, int layer, Rectangle2I square, TileData placedTile) {
			ActionName = (placedTile == null ? "Erase" : "Place") + " Tile Square";
			ActionIcon = (placedTile == null ? EditorImages.ToolSquareErase : EditorImages.ToolSquare);
			this.level = level;
			this.layer = layer;
			this.square = square;
			this.placedTile = placedTile;
			//if (layer >= 1)
				this.overwrittenTiles = new Dictionary<Point2I, TileDataInstance>();
			//else
			//	this.overwrittenTileGrid = new TileDataInstance[square.Width, square.Height];
			this.placedTiles = new TileDataInstance[square.Width, square.Height];

			for (int x = 0; x < square.Width; x++) {
				for (int y = 0; y < square.Height; y++) {
					if (placedTile != null)
						placedTiles[x, y] = new TileDataInstance(placedTile);
				}
			}
		}

		public void AddOverwrittenTile(TileDataInstance tile) {
			if (tile == null) return;
			Point2I point = tile.LevelCoord;
			if (!overwrittenTiles.ContainsKey(point))
				overwrittenTiles.Add(point, tile);
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			for (int x = 0; x < square.Width; x++) {
				for (int y = 0; y < square.Height; y++) {
					Point2I levelCoord = square.Point + new Point2I(x, y);
					level.RemoveTile(levelCoord, layer);
				}
			}
			foreach (var pair in overwrittenTiles) {
				level.PlaceTile(pair.Value, pair.Key, layer);
			}
			editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			for (int x = 0; x < square.Width; x++) {
				for (int y = 0; y < square.Height; y++) {
					Point2I levelCoord = square.Point + new Point2I(x, y);
					level.PlaceTile(placedTiles[x, y], levelCoord, layer);
				}
			}
			editorControl.NeedsNewEventCache = true;
		}
	}
}
