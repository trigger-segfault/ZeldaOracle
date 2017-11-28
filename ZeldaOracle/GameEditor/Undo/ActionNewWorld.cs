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
	public class ActionNewWorld : EditorAction {

		public ActionNewWorld() {
			ActionName = "Open World";
			ActionIcon = EditorImages.WorldNew;
		}

		public override void Undo(EditorControl editorControl) {
			// Dummy undo
		}

		public override void Redo(EditorControl editorControl) {
			// Dummy redo
		}
	}
}
