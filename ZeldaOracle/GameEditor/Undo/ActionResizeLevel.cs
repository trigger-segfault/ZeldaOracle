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
		private Dictionary<Point2I, Room> cutinRooms;

		public ActionResizeLevel(Level level, Point2I newSize) {
			ActionName = "Resize Level";
			ActionIcon = EditorImages.LevelResize;
			this.level = level;
			this.oldSize = level.Dimensions;
			this.newSize = newSize;
			if (oldSize != newSize) {
				this.cutoffRooms = new Dictionary<Point2I, Room>();
				this.cutinRooms = new Dictionary<Point2I, Room>();
				Point2I max = GMath.Max(oldSize, newSize);
				for (int x = newSize.X; x < oldSize.X; x++) {
					for (int y = 0; y < max.Y; y++) {
						AddCutoffRoom(new Point2I(x, y), level.GetRoomAt(x, y));
					}
				}
				for (int y = newSize.Y; y < oldSize.Y; y++) {
					for (int x = 0; x < max.X; x++) {
						AddCutoffRoom(new Point2I(x, y), level.GetRoomAt(x, y));
					}
				}
			}
		}

		private void AddCutoffRoom(Point2I point, Room room) {
			if (!cutoffRooms.ContainsKey(point))
				cutoffRooms.Add(point, room);
		}

		private void AddCutinRoom(Point2I point, Room room) {
			if (!cutinRooms.ContainsKey(point))
				cutinRooms.Add(point, room);
		}

		public override void Execute(EditorControl editorControl) {
			level.Resize(newSize);
			// Store the newly created rooms to keep links to property objects
			Point2I max = GMath.Max(oldSize, newSize);
			for (int x = oldSize.X; x < newSize.X; x++) {
				for (int y = 0; y < max.Y; y++) {
					AddCutinRoom(new Point2I(x, y), level.GetRoomAt(x, y));
				}
			}
			for (int y = oldSize.Y; y < newSize.Y; y++) {
				for (int x = 0; x < max.X; x++) {
					AddCutinRoom(new Point2I(x, y), level.GetRoomAt(x, y));
				}
			}
			editorControl.OpenLevel(level);
			editorControl.LevelDisplay.UpdateLevel();
			editorControl.NeedsNewEventCache = true;
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			level.Resize(oldSize, cutoffRooms);
			editorControl.LevelDisplay.UpdateLevel();
			editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			level.Resize(newSize, cutinRooms);
			editorControl.LevelDisplay.UpdateLevel();
			editorControl.NeedsNewEventCache = true;
		}

		public override bool IgnoreAction { get { return oldSize == newSize; } }
	}
}
