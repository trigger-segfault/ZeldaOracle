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
		
		private Level level;

		public ActionCreateLevel(Level level) {
			ActionName = "Create '" + level.ID + "' Level";
			ActionIcon = EditorImages.LevelAdd;
			this.level = level;
		}

		public override void Undo(EditorControl editorControl) {
			int levelIndex = editorControl.World.IndexOfLevel(level);
			editorControl.World.RemoveLevelAt(levelIndex);
			if (editorControl.Level == level) {
				if (editorControl.World.LevelCount == 0) {
					editorControl.CloseLevel();
				}
				else {
					editorControl.OpenLevel(Math.Max(0, levelIndex - 1));
				}
			}
			editorControl.EditorWindow.WorldTreeView.RefreshLevels();
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.World.AddLevel(level);
			editorControl.EditorWindow.WorldTreeView.RefreshLevels();
			editorControl.OpenLevel(level);
		}
	}
}
