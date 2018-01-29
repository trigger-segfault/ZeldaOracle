using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;

namespace ZeldaEditor.Undo {
	public class ActionPlaceAction : EditorAction {

		private struct ActionTileInfo {
			public ActionTileDataInstance ActionTile { get; set; }
			public Point2I Position { get; set; }
			public Room Room { get; set; }

			public ActionTileInfo(ActionTileDataInstance actionTile) {
				this.ActionTile = actionTile;
				this.Position = actionTile.Position;
				this.Room = actionTile.Room;
			}
		}

		private Level level;
		private ActionTileData placedActionTileData;
		private ActionTileInfo placedActionTileInfo;
		private Point2I placedPosition;
		private Room placedRoom;
		private List<ActionTileInfo> overwrittenActionTiles;

		public ActionPlaceAction(Level level, ActionTileData placedActionTile, Room room, Point2I position) {
			this.ActionName = (placedActionTile == null ? "Erase" : "Place") + " Action";
			this.ActionIcon = (placedActionTile == null ? EditorImages.ToolPlaceErase : EditorImages.ToolPlace);
			this.level = level;
			this.placedActionTileData = placedActionTile;
			this.placedPosition = position;
			this.placedRoom = room;
			this.overwrittenActionTiles = new List<ActionTileInfo>();
		}
		public ActionPlaceAction(Level level) :
			this(level, null, null, Point2I.Zero) {
		}

		public void AddOverwrittenActionTile(ActionTileDataInstance actionTile) {
			overwrittenActionTiles.Add(new ActionTileInfo(actionTile));
		}

		public override void Execute(EditorControl editorControl) {
			foreach (var actionTileInfo in overwrittenActionTiles) {
				actionTileInfo.Room.RemoveActionTile(actionTileInfo.ActionTile);
			}
			if (placedActionTileData != null)
				placedActionTileInfo = new ActionTileInfo(placedRoom.CreateActionTile(placedActionTileData, placedPosition));
		}

		public override void PostExecute(EditorControl editorControl) {
			if (placedActionTileData != null)
				placedActionTileInfo = new ActionTileInfo(placedRoom.CreateActionTile(placedActionTileData, placedPosition));
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			foreach (var actionTileInfo in overwrittenActionTiles) {
				actionTileInfo.ActionTile.Position = actionTileInfo.Position;
				actionTileInfo.Room.AddActionTile(actionTileInfo.ActionTile);
			}
			if (placedActionTileInfo.ActionTile != null)
				placedActionTileInfo.Room.RemoveActionTile(placedActionTileInfo.ActionTile);
			editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			foreach (var actionTileInfo in overwrittenActionTiles) {
				actionTileInfo.Room.RemoveActionTile(actionTileInfo.ActionTile);
			}
			if (placedActionTileInfo.ActionTile != null)
				placedActionTileInfo.Room.AddActionTile(placedActionTileInfo.ActionTile);
			editorControl.NeedsNewEventCache = true;
		}

		public override bool IgnoreAction { get { return !overwrittenActionTiles.Any() && placedActionTileData == null; } }
	}
}
