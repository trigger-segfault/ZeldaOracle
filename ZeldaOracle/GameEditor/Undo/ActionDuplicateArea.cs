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
	public class ActionDuplicateArea : EditorAction {

		private Area area;
		private Area newArea;

		public ActionDuplicateArea(Area area, Area newArea) {
			ActionName = "Duplicate '" + area.ID + "' Area";
			ActionIcon = EditorImages.AreaDuplicate;
			this.area = area;
			this.newArea = newArea;
		}
		
		public override void Undo(EditorControl editorControl) {
			editorControl.World.RemoveArea(newArea);
			editorControl.EditorWindow.WorldTreeView.RefreshAreas();
			if (area.Events.HasDefinedEvents)
				editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.World.AddArea(newArea);
			editorControl.EditorWindow.WorldTreeView.RefreshAreas();
			if (area.Events.HasDefinedEvents)
				editorControl.NeedsNewEventCache = true;
		}
	}
}
