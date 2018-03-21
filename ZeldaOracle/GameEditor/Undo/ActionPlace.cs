using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Worlds.Editing;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaEditor.Undo {
	public class ActionPlace : EditorAction {

		private Level level;
		private int layer;
		private TileData placedTile;
		private Dictionary<Point2I, TileDataInstance> placedTiles;
		private Dictionary<Point2I, TileDataInstance> overwrittenTiles;

		private ActionPlace(Level level, int layer, TileData placedTile) {
			this.level = level;
			this.layer = layer;
			this.placedTile = placedTile;
			this.placedTiles = new Dictionary<Point2I, TileDataInstance>();
			this.overwrittenTiles = new Dictionary<Point2I, TileDataInstance>();
		}

		public static ActionPlace CreatePlaceAction(Level level, int layer, TileData placedTile) {
			ActionPlace action = new ActionPlace(level, layer, placedTile);
			action.ActionName = (placedTile == null ? "Erase" : "Place") + " Tiles";
			action.ActionIcon = (placedTile == null ? EditorImages.ToolPlaceErase : EditorImages.ToolPlace);
			return action;
		}

		public static ActionPlace CreateFillAction(Level level, int layer, TileData placedTile) {
			ActionPlace action = new ActionPlace(level, layer, placedTile);
			action.ActionName = (placedTile == null ? "Erase" : "Place") + " Tile Fill";
			action.ActionIcon = (placedTile == null ? EditorImages.ToolFillErase : EditorImages.ToolFill);
			return action;
		}

		public void AddPlacedTile(Point2I point) {
			if (!placedTiles.ContainsKey(point)) {
				TileDataInstance tile = null;
				if (placedTile != null)
					tile = new TileDataInstance(placedTile);
				placedTiles.Add(point, tile);
			}
		}

		public void AddPlacedTile(TileDataInstance tile) {
			if (tile == null) return;
			Point2I point = tile.LevelCoord;
			if (!placedTiles.ContainsKey(point))
				placedTiles.Add(point, tile);
		}

		public void AddOverwrittenTile(TileDataInstance tile) {
			if (tile == null) return;
			Point2I point = tile.LevelCoord;
			if (!overwrittenTiles.ContainsKey(point))
				overwrittenTiles.Add(point, tile);
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			foreach (var pair in placedTiles) {
				/*Point2I roomLocation = pair.Key / level.RoomSize;
				Point2I tileLocation = pair.Key % level.RoomSize;
				Room room = level.GetRoomAt(roomLocation);
				room.RemoveTile(tileLocation, layer);*/
				level.RemoveTile(pair.Key, layer);
			}
			foreach (var pair in overwrittenTiles) {
				/*Point2I roomLocation = pair.Key / level.RoomSize;
				Point2I tileLocation = pair.Key % level.RoomSize;
				Room room = level.GetRoomAt(roomLocation);
				room.PlaceTile(pair.Value, tileLocation, layer);*/
				level.PlaceTile(pair.Value, pair.Key, layer);
			}
			editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			foreach (var pair in placedTiles) {
				/*Point2I roomLocation = pair.Key / level.RoomSize;
				Point2I tileLocation = pair.Key % level.RoomSize;
				Room room = level.GetRoomAt(roomLocation);
				room.PlaceTile(pair.Value, tileLocation, layer);*/
				level.PlaceTile(pair.Value, pair.Key, layer);
			}
			editorControl.NeedsNewEventCache = true;
		}

		public override bool IgnoreAction { get { return !placedTiles.Any() && !overwrittenTiles.Any(); } }
	}
}
