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
	public class ActionDuplicateLevel : EditorAction {

		private Level level;
		private Level newLevel;

		public ActionDuplicateLevel(Level level, Level newLevel) {
			ActionName = "Duplicate '" + level.ID + "' Level";
			ActionIcon = EditorImages.LevelDuplicate;
			this.level = level;
			this.newLevel = newLevel;
		}

		public override void Undo(EditorControl editorControl) {
			int index = editorControl.World.IndexOfLevel(newLevel);
			editorControl.World.RemoveLevelAt(index);
			if (editorControl.Level == newLevel) {
				if (editorControl.World.LevelCount == 0) {
					editorControl.CloseLevel();
				}
				else {
					editorControl.OpenLevel(Math.Max(0, index - 1));
				}
			}
			editorControl.EditorWindow.WorldTreeView.RefreshLevels();
			if (level.Events.HasDefinedEvents)
				editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.World.AddLevel(newLevel);
			editorControl.OpenLevel(newLevel);
			editorControl.EditorWindow.WorldTreeView.RefreshLevels();
			if (level.Events.HasDefinedEvents)
				editorControl.NeedsNewEventCache = true;
		}
	}
}
