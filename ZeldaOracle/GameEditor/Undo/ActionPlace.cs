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
	public class ActionPlace : EditorAction {

		private Level level;
		private int layer;
		private TileData placedTile;
		private Dictionary<Point2I, TileDataInstance> overwrittenTiles;

		private ActionPlace(Level level, int layer, TileData placedTile) {
			this.level = level;
			this.layer = layer;
			this.placedTile = placedTile;
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

		public void AddOverwrittenTile(Point2I point, TileDataInstance tile) {
			if ((tile != null || placedTile != null) && !overwrittenTiles.ContainsKey(point))
				overwrittenTiles.Add(point, tile);
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			foreach (var pair in overwrittenTiles) {
				Point2I roomLocation = pair.Key / level.RoomSize;
				Point2I tileLocation = pair.Key % level.RoomSize;
				Room room = level.GetRoomAt(roomLocation);
				room.PlaceTile(pair.Value, tileLocation, layer);
			}
			editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			foreach (var pair in overwrittenTiles) {
				Point2I roomLocation = pair.Key / level.RoomSize;
				Point2I tileLocation = pair.Key % level.RoomSize;
				Room room = level.GetRoomAt(roomLocation);
				room.CreateTile(placedTile, tileLocation, layer);
			}
			editorControl.NeedsNewEventCache = true;
		}

		public override bool IgnoreAction { get { return !overwrittenTiles.Any(); } }
	}
}
