using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaEditor.Undo {
	public class ActionSquare : EditorAction {

		private Level level;
		private int layer;
		private Rectangle2I square;
		private TileData placedTile;
		private Dictionary<Point2I, TileDataInstance> overwrittenTiles; // Used on layers 2 and up
		private TileDataInstance[,] overwrittenTileGrid; // Used on layer 1

		public ActionSquare(Level level, int layer, Rectangle2I square, TileData placedTile) {
			ActionName = (placedTile == null ? "Erase" : "Place") + " Tile Square";
			ActionIcon = (placedTile == null ? EditorImages.ToolSquareErase : EditorImages.ToolSquare);
			this.level = level;
			this.layer = layer;
			this.square = square;
			this.placedTile = placedTile;
			if (layer >= 1)
				this.overwrittenTiles = new Dictionary<Point2I, TileDataInstance>();
			else
				this.overwrittenTileGrid = new TileDataInstance[square.Width, square.Height];
		}

		public void AddOverwrittenTile(Point2I point, TileDataInstance tile) {
			if (layer >= 1) {
				if (!overwrittenTiles.ContainsKey(point))
					overwrittenTiles.Add(point, tile);
			}
			else {
				overwrittenTileGrid[point.X - square.X, point.Y - square.Y] = tile;
			}
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			if (layer >= 1) {
				for (int x = 0; x < square.Width; x++) {
					for (int y = 0; y < square.Height; y++) {
						Point2I point = square.Point + new Point2I(x, y);
						Point2I roomLocation = point / level.RoomSize;
						Point2I tileLocation = point % level.RoomSize;
						Room room = level.GetRoomAt(roomLocation);
						room.RemoveTile(x, y, layer);
					}
				}
				foreach (var pair in overwrittenTiles) {
					Point2I roomLocation = pair.Key / level.RoomSize;
					Point2I tileLocation = pair.Key % level.RoomSize;
					Room room = level.GetRoomAt(roomLocation);
					room.PlaceTile(pair.Value, tileLocation, layer);
				}
			}
			else {
				for (int x = 0; x < square.Width; x++) {
					for (int y = 0; y < square.Height; y++) {
						Point2I point = square.Point + new Point2I(x, y);
						Point2I roomLocation = point / level.RoomSize;
						Point2I tileLocation = point % level.RoomSize;
						Room room = level.GetRoomAt(roomLocation);
						room.PlaceTile(overwrittenTileGrid[x, y], tileLocation, layer);
					}
				}
			}
			editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			for (int x = 0; x < square.Width; x++) {
				for (int y = 0; y < square.Height; y++) {
					Point2I point = square.Point + new Point2I(x, y);
					Point2I roomLocation = point / level.RoomSize;
					Point2I tileLocation = point % level.RoomSize;
					Room room = level.GetRoomAt(roomLocation);
					room.CreateTile(placedTile, tileLocation, layer);
				}
			}
			editorControl.NeedsNewEventCache = true;
		}
	}
}
