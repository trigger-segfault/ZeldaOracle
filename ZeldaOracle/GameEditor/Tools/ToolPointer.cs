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
	public class ToolPointer : EditorTool {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolPointer() {
			name = "Pointer Tool";
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnMouseDown(MouseEventArgs e, Room room, Point2I tileLocation) {
			if (e.Button == MouseButtons.Left) {
				if (room != null) {
					TileDataInstance tile = room.GetTile(tileLocation, editorControl.CurrentLayer);
					if (tile != null) {
						EditorControl.SelectedRoom = room.Location;
						EditorControl.SelectedTile = tileLocation;
						EditorControl.OpenTileProperties(tile);
					}
				}
			}
		}

		public override void OnMouseUp(MouseEventArgs e, Room room, Point2I tileLocation) {
			if (e.Button == MouseButtons.Left) {

			}
		}

		public override void OnMouseMove(MouseEventArgs e, Room room, Point2I tileLocation) {
			if (e.Button == MouseButtons.Left) {
				
			}
		}

	}
}
