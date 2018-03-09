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
	public class ActionCreateArea : EditorAction {

		private Area area;

		public ActionCreateArea(Area area) {
			ActionName = "Create '" + area.ID + "' Area";
			ActionIcon = EditorImages.AreaAdd;
			this.area = area;
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.World.RemoveArea(area);
			editorControl.EditorWindow.WorldTreeView.RefreshAreas();
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.World.AddArea(area);
			editorControl.EditorWindow.WorldTreeView.RefreshAreas();
		}
	}
}
