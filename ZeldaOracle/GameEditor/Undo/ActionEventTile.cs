using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;

namespace ZeldaEditor.Undo {
	public class ActionEventTile : EditorAction {

		private struct EventTileInfo {
			public EventTileDataInstance EventTile { get; set; }
			public Point2I Position { get; set; }
			public Room Room { get; set; }

			public EventTileInfo(EventTileDataInstance eventTile) {
				this.EventTile = eventTile;
				this.Position = eventTile.Position;
				this.Room = eventTile.Room;
			}
		}

		private Level level;
		private EventTileData placedEventTileData;
		private EventTileInfo placedEventTileInfo;
		private Point2I placedPosition;
		private Room placedRoom;
		private List<EventTileInfo> overwrittenEventTiles;

		public ActionEventTile(Level level, EventTileData placedEventTile, Room room, Point2I position) {
			this.ActionName = (placedEventTile == null ? "Erase" : "Place") + " Event";
			this.ActionIcon = (placedEventTile == null ? EditorImages.ToolPlaceErase : EditorImages.ToolPlace);
			this.level = level;
			this.placedEventTileData = placedEventTile;
			this.placedPosition = position;
			this.placedRoom = room;
			this.overwrittenEventTiles = new List<EventTileInfo>();
		}

		public void AddOverwrittenEventTile(EventTileDataInstance eventTile) {
			overwrittenEventTiles.Add(new EventTileInfo(eventTile));
		}

		public override void Execute(EditorControl editorControl) {
			foreach (var eventTileInfo in overwrittenEventTiles) {
				eventTileInfo.Room.RemoveEventTile(eventTileInfo.EventTile);
			}
			placedEventTileInfo = new EventTileInfo(placedRoom.CreateEventTile(placedEventTileData, placedPosition));
		}

		public override void PostExecute(EditorControl editorControl) {
			if (placedEventTileData != null)
				placedEventTileInfo = new EventTileInfo(placedRoom.CreateEventTile(placedEventTileData, placedPosition));
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			foreach (var eventTileInfo in overwrittenEventTiles) {
				eventTileInfo.EventTile.Position = eventTileInfo.Position;
				eventTileInfo.Room.AddEventTile(eventTileInfo.EventTile);
			}
			if (placedEventTileInfo.EventTile != null)
				placedEventTileInfo.Room.RemoveEventTile(placedEventTileInfo.EventTile);
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			foreach (var eventTileInfo in overwrittenEventTiles) {
				eventTileInfo.Room.RemoveEventTile(eventTileInfo.EventTile);
			}
			if (placedEventTileInfo.EventTile != null)
				placedEventTileInfo.Room.AddEventTile(placedEventTileInfo.EventTile);
		}

		public override bool IgnoreAction { get { return !overwrittenEventTiles.Any() && placedEventTileData == null; } }
	}
}
