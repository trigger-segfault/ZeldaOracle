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
	public class ActionShiftLevel : EditorAction {

		private Level level;
		private Point2I distance;
		private Dictionary<Point2I, Room> cutoffRooms;
		private Dictionary<Point2I, Room> cutinRooms;

		public ActionShiftLevel(Level level, Point2I distance) {
			ActionName = "Shift Level";
			ActionIcon = EditorImages.LevelShift;
			this.level = level;
			this.distance = distance;
			this.cutoffRooms = new Dictionary<Point2I, Room>();
			this.cutinRooms = new Dictionary<Point2I, Room>();
			int startX = (distance.X > 0 ? level.Width - distance.X : 0);
			int startY = (distance.Y > 0 ? level.Height - distance.Y : 0);
			for (int x = startX; x < startX + Math.Abs(distance.X); x++) {
				for (int y = 0; y < level.Height; y++) {
					AddCutoffRoom(new Point2I(x, y), level.GetRoomAt(x, y));
				}
			}
			for (int y = startY; y < startY + Math.Abs(distance.Y); y++) {
				for (int x = 0; x < level.Width; x++) {
					AddCutoffRoom(new Point2I(x, y), level.GetRoomAt(x, y));
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
			level.ShiftRooms(distance);
			// Store the newly created rooms to keep links to property objects
			int startX = (distance.X <= 0 ? level.Width + distance.X : 0);
			int startY = (distance.Y <= 0 ? level.Height + distance.Y : 0);
			for (int x = startX; x < startX + Math.Abs(distance.X); x++) {
				for (int y = 0; y < level.Height; y++) {
					AddCutinRoom(new Point2I(x, y), level.GetRoomAt(x, y));
				}
			}
			for (int y = startY; y < startY + Math.Abs(distance.Y); y++) {
				for (int x = 0; x < level.Width; x++) {
					AddCutinRoom(new Point2I(x, y), level.GetRoomAt(x, y));
				}
			}
			editorControl.OpenLevel(level);
			editorControl.NeedsNewEventCache = true;
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			level.ShiftRooms(-distance, cutoffRooms);
			editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			level.ShiftRooms(distance, cutinRooms);
			editorControl.OpenLevel(level);
			editorControl.NeedsNewEventCache = true;
		}

		public override bool IgnoreAction { get { return distance == Point2I.Zero; } }
	}
}
