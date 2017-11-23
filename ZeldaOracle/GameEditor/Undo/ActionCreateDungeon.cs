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
	public class ActionCreateDungeon : EditorAction {

		private string id;
		private string name;

		public ActionCreateDungeon(string id, string name) {
			ActionName = "Create Dungeon";
			ActionIcon = EditorImages.DungeonAdd;
			this.id = id;
			this.name = name;
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.World.RemoveDungeon(id);
			editorControl.EditorWindow.TreeViewWorld.RefreshDungeons();
		}

		public override void Redo(EditorControl editorControl) {
			Dungeon dungeon = new Dungeon(id, name);
			editorControl.World.AddDungeon(dungeon);
			editorControl.EditorWindow.TreeViewWorld.RefreshDungeons();
		}
	}
}
