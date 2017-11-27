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
	public class ActionMoveLevel : EditorAction {

		private Level level;
		private int distance;

		public ActionMoveLevel(Level level, int distance) {
			ActionName = "Move '" + level.ID + "' Level";
			ActionIcon = (distance < 0 ? EditorImages.MoveUp : EditorImages.MoveDown);
			this.level = level;
			this.distance = distance;
		}
		
		public override void Undo(EditorControl editorControl) {
			int index = editorControl.World.IndexOfLevel(level);
			editorControl.World.MoveLevel(index, -distance, true);
			editorControl.EditorWindow.WorldTreeView.RefreshLevels();
		}

		public override void Redo(EditorControl editorControl) {
			int index = editorControl.World.IndexOfLevel(level);
			editorControl.World.MoveLevel(index, distance, true);
			editorControl.EditorWindow.WorldTreeView.RefreshLevels();
		}

		public override bool IgnoreAction { get { return distance == 0; } }
	}
}
