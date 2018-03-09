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
	public class ActionMoveArea : EditorAction {

		private Area area;
		private int distance;

		public ActionMoveArea(Area area, int distance) {
			ActionName = "Move '" + area.ID + "' Area";
			ActionIcon = (distance < 0 ? EditorImages.MoveUp : EditorImages.MoveDown);
			this.area = area;
			this.distance = distance;
		}

		public override void Undo(EditorControl editorControl) {
			int index = editorControl.World.IndexOfArea(area);
			editorControl.World.MoveArea(index, -distance, true);
			editorControl.EditorWindow.WorldTreeView.RefreshAreas();
		}

		public override void Redo(EditorControl editorControl) {
			int index = editorControl.World.IndexOfArea(area);
			editorControl.World.MoveArea(index, distance, true);
			editorControl.EditorWindow.WorldTreeView.RefreshAreas();
		}

		public override bool IgnoreAction { get { return distance == 0; } }
	}
}
