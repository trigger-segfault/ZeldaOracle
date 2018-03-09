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
	public class ActionDeleteArea : EditorAction {

		private Area area;
		private int index;

		public ActionDeleteArea(Area area) {
			ActionName = "Delete '" + area.ID + "' Area";
			ActionIcon = EditorImages.AreaDelete;
			this.area = area;
		}

		public override void Execute(EditorControl editorControl) {
			index = editorControl.World.IndexOfArea(area);
			Redo(editorControl);
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.World.InsertArea(index, area);
			editorControl.EditorWindow.WorldTreeView.RefreshAreas();
			if (area.Events.HasDefinedEvents)
				editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.World.RemoveAreaAt(index);
			editorControl.EditorWindow.WorldTreeView.RefreshAreas();
			if (area.Events.HasDefinedEvents)
				editorControl.NeedsNewEventCache = true;
		}
	}
}
