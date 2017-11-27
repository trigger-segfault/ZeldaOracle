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
		private string newLevelName;

		public ActionDuplicateLevel(Level level, string newLevelName) {
			ActionName = "Duplicate '" + level.ID + "' Level";
			ActionIcon = EditorImages.LevelDuplicate;
			this.level = level;
			this.newLevelName = newLevelName;
		}

		public override void Undo(EditorControl editorControl) {
			int index = editorControl.World.IndexOfLevel(newLevelName);
			editorControl.World.RemoveLevelAt(index);
			if (editorControl.Level.ID == newLevelName) {
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
			Level duplicate = new Level(level);
			duplicate.ID = newLevelName;
			editorControl.World.AddLevel(duplicate);
			editorControl.OpenLevel(duplicate);
			editorControl.EditorWindow.WorldTreeView.RefreshLevels();
			if (level.Events.HasDefinedEvents)
				editorControl.NeedsNewEventCache = true;
		}
	}
}
