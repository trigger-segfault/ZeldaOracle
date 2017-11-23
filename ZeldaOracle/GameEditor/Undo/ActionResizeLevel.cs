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
	public class ActionResizeLevel : EditorAction {

		private Level level;
		private Point2I oldSize;
		private Point2I newSize;
		private Dictionary<Point2I, Room> cutoffRooms;

		public ActionResizeLevel(Level level, Point2I newSize) {
			ActionName = "Resize Level";
			ActionIcon = EditorImages.LevelResize;
			this.level = level;
			this.oldSize = level.Dimensions;
			this.newSize = newSize;
			if (!(oldSize <= newSize)) {
				this.cutoffRooms = new Dictionary<Point2I, Room>();
				if (oldSize.X > newSize.X) {
					for (int x = newSize.X; x < oldSize.X; x++) {
						for (int y = 0; y < oldSize.Y; y++) {
							AddRoom(new Point2I(x, y), level.GetRoomAt(x, y));
						}
					}
				}
				if (oldSize.Y > newSize.Y) {
					for (int x = 0; x < oldSize.X; x++) {
						for (int y = newSize.Y; y < oldSize.Y; y++) {
							AddRoom(new Point2I(x, y), level.GetRoomAt(x, y));
						}
					}
				}
			}
		}

		private void AddRoom(Point2I point, Room room) {
			if (!cutoffRooms.ContainsKey(point))
				cutoffRooms.Add(point, room);
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			if (!(oldSize <= newSize))
				level.Resize(oldSize, cutoffRooms);
			else
				level.Resize(oldSize);
			editorControl.LevelDisplay.UpdateLevel();
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			level.Resize(newSize);
			editorControl.LevelDisplay.UpdateLevel();
		}

		public override bool IgnoreAction { get { return oldSize == newSize; } }
	}
}
