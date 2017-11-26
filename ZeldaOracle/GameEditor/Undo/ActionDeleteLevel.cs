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
	public class ActionDeleteLevel : EditorAction {

		private Level level;
		private int index;

		public ActionDeleteLevel(Level level) {
			ActionName = "Delete '" + level.ID + "' Level";
			ActionIcon = EditorImages.LevelDelete ;
			this.level = level;
		}

		public override void Execute(EditorControl editorControl) {
			index = editorControl.World.IndexOfLevel(level);
			Redo(editorControl);
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.World.InsertLevel(index, level);
			editorControl.OpenLevel(level);
			editorControl.EditorWindow.TreeViewWorld.RefreshLevels();
			editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.World.RemoveLevelAt(index);
			if (editorControl.Level == level) {
				if (editorControl.World.LevelCount == 0) {
					editorControl.CloseLevel();
				}
				else {
					editorControl.OpenLevel(Math.Max(0, index - 1));
				}
			}
			editorControl.EditorWindow.TreeViewWorld.RefreshLevels();
			editorControl.NeedsNewEventCache = true;
		}
	}
}
