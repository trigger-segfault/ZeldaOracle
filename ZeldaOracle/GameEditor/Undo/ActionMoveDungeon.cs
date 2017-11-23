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
	public class ActionMoveDungeon : EditorAction {

		private Dungeon dungeon;
		private int distance;

		public ActionMoveDungeon(Dungeon dungeon, int distance) {
			ActionName = "Move Dungeon";
			ActionIcon = (distance < 0 ? EditorImages.MoveUp : EditorImages.MoveDown);
			this.dungeon = dungeon;
			this.distance = distance;
		}

		public override void Undo(EditorControl editorControl) {
			int index = editorControl.World.IndexOfDungeon(dungeon);
			editorControl.World.MoveDungeon(index, -distance, true);
			editorControl.EditorWindow.TreeViewWorld.RefreshDungeons();
		}

		public override void Redo(EditorControl editorControl) {
			int index = editorControl.World.IndexOfDungeon(dungeon);
			editorControl.World.MoveDungeon(index, distance, true);
			editorControl.EditorWindow.TreeViewWorld.RefreshDungeons();
		}

		public override bool IgnoreAction { get { return distance == 0; } }
	}
}
