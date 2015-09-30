using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;

namespace ZeldaEditor.Tools {
	public class ToolEyedrop : EditorTool {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolEyedrop() {
			name = "Eyedrop Tool";
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void ActivateTile(MouseButtons mouseButton, Room room, Point2I tileLocation) {
			if (mouseButton == MouseButtons.Left) {
				// Sample the tile.
				TileDataInstance tile = room.GetTile(tileLocation, editorControl.CurrentLayer);
				if (tile != null) {
					editorControl.SelectedTilesetTile = -Point2I.One;
					editorControl.SelectedTilesetTileData = tile.TileData;
				}
			}
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnMouseDragBegin(MouseButtons buttons, Room room, Point2I tileLocation) {
			if (room != null)
				ActivateTile(buttons, room, tileLocation);
		}

		public override void OnMouseDragEnd(MouseButtons buttons, Room room, Point2I tileLocation) {
			if (buttons == MouseButtons.Left) {
				// Switch back to place tool.
				editorControl.ChangeTool(1);
			}
		}

		public override void OnMouseDragMove(MouseButtons buttons, Room room, Point2I tileLocation) {
			if (room != null)
				ActivateTile(buttons, room, tileLocation);
		}
	}
}
