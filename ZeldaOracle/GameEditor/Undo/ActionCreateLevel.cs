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
	public class ActionCreateLevel : EditorAction {
		
		private string id;
		private Point2I dimensions;
		private int layerCount;
		private Point2I roomSize;
		private Zone zone;

		public ActionCreateLevel(string id, Point2I dimensions, int layerCount, Point2I roomSize, Zone zone) {
			ActionName = "Create '" + id + "' Level";
			ActionIcon = EditorImages.LevelAdd;
			this.id = id;
			this.dimensions = dimensions;
			this.layerCount = layerCount;
			this.roomSize = roomSize;
			this.zone = zone;
		}

		public override void Undo(EditorControl editorControl) {
			int levelIndex = editorControl.World.IndexOfLevel(id);
			editorControl.World.RemoveLevel(id);
			if (editorControl.Level.ID == id) {
				if (editorControl.World.LevelCount == 0) {
					editorControl.CloseLevel();
				}
				else {
					editorControl.OpenLevel(Math.Max(0, levelIndex - 1));
				}
			}
			editorControl.EditorWindow.TreeViewWorld.RefreshLevels();
		}

		public override void Redo(EditorControl editorControl) {
			Level level = new Level(id, dimensions, layerCount, roomSize, zone);
			editorControl.World.AddLevel(level);
			editorControl.EditorWindow.TreeViewWorld.RefreshLevels();
			editorControl.OpenLevel(level);
		}
	}
}
