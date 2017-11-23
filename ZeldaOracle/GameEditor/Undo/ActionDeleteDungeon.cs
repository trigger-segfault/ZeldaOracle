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
	public class ActionDeleteDungeon : EditorAction {

		private Dungeon dungeon;
		private int index;

		public ActionDeleteDungeon(Dungeon dungeon) {
			ActionName = "Delete Dungeon";
			ActionIcon = EditorImages.DungeonDelete;
			this.dungeon = dungeon;
		}

		public override void Execute(EditorControl editorControl) {
			index = editorControl.World.IndexOfDungeon(dungeon);
			Redo(editorControl);
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.World.InsertDungeon(index, dungeon);
			editorControl.EditorWindow.TreeViewWorld.RefreshDungeons();
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.World.RemoveDungeonAt(index);
			editorControl.EditorWindow.TreeViewWorld.RefreshDungeons();
		}
	}
}
