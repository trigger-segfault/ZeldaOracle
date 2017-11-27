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
	public class ActionDuplicateDungeon : EditorAction {

		private Dungeon dungeon;
		private string newDungeonName;

		public ActionDuplicateDungeon(Dungeon dungeon, string newDungeonName) {
			ActionName = "Duplicate '" + dungeon.ID + "' Dungeon";
			ActionIcon = EditorImages.DungeonDuplicate;
			this.dungeon = dungeon;
			this.newDungeonName = newDungeonName;
		}
		
		public override void Undo(EditorControl editorControl) {
			editorControl.World.RemoveDungeon(newDungeonName);
			editorControl.EditorWindow.WorldTreeView.RefreshDungeons();
			if (dungeon.Events.HasDefinedEvents)
				editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			Dungeon duplicate = new Dungeon(dungeon);
			duplicate.ID = newDungeonName;
			editorControl.World.AddDungeon(duplicate);
			editorControl.EditorWindow.WorldTreeView.RefreshDungeons();
			if (dungeon.Events.HasDefinedEvents)
				editorControl.NeedsNewEventCache = true;
		}
	}
}
