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
	public class ToolPlace : EditorTool {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolPlace() {
			name = "Place Tool";
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void ActivateTile(MouseButtons mouseButton, Room room, Point2I tileLocation) {
			if (mouseButton == MouseButtons.Left) {
				room.CreateTile(
					editorControl.SelectedTilesetTileData,
					tileLocation.X, tileLocation.Y, editorControl.CurrentLayer
				);
			}
			else if (mouseButton == MouseButtons.Right) {
				if (editorControl.CurrentLayer == 0) {
					room.CreateTile(
						editorControl.Tileset.DefaultTileData,
						tileLocation.X, tileLocation.Y, editorControl.CurrentLayer
					);
				}
				else {
					room.RemoveTile(tileLocation.X, tileLocation.Y, editorControl.CurrentLayer);
				}
			}
			else if (mouseButton == MouseButtons.Middle) {
				// Sample the tile.
				TileDataInstance tile = room.GetTile(tileLocation, EditorControl.CurrentLayer);
				if (tile != null) {
					editorControl.SelectedTilesetTile = -Point2I.One;
					editorControl.SelectedTilesetTileData = tile.TileData;
				}
			}
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {

		}

		public override void OnBegin() {

		}

		public override void OnMouseDragBegin(MouseButtons buttons, Room room, Point2I tileLocation) {
			if (room != null)
				ActivateTile(buttons, room, tileLocation);
		}

		public override void OnMouseDragEnd(MouseButtons buttons, Room room, Point2I tileLocation) {
			
		}

		public override void OnMouseDragMove(MouseButtons buttons, Room room, Point2I tileLocation) {
			if (room != null)
				ActivateTile(buttons, room, tileLocation);
		}
	}
}
