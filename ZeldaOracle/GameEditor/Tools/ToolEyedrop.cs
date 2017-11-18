using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;

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

		public override void OnChangeLayer() {
			
		}

		public override void OnBegin() {
			EditorControl.HighlightMouseTile = false;
		}

		public override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			
			Point2I mousePos = new Point2I(e.X, e.Y);

			if (!editorControl.EventMode) {
				// Highlight tiles.
				TileDataInstance tile = LevelDisplay.SampleTile(mousePos, editorControl.CurrentLayer);
				EditorControl.HighlightMouseTile = (tile != null);
			}
			else {
				// Highlight event tiles.
				EventTileDataInstance eventTile = LevelDisplay.SampleEventTile(mousePos);
				EditorControl.HighlightMouseTile = (eventTile != null);
				if (eventTile != null) {
					LevelDisplay.CursorHalfTileLocation =
						LevelDisplay.SampleLevelHalfTileCoordinates(
							LevelDisplay.GetRoomDrawPosition(eventTile.Room) + eventTile.Position);
					LevelDisplay.CursorTileSize = eventTile.Size;
				}
			}
		}

		public override void OnMouseDragBegin(MouseEventArgs e) {
			Point2I mousePos	= new Point2I(e.X, e.Y);
			Room	room		= LevelDisplay.SampleRoom(mousePos);
			Point2I tileCoord	= LevelDisplay.SampleTileCoordinates(mousePos);
			if (room != null)
				ActivateTile(e.Button, room, tileCoord);
		}

		public override void OnMouseDragEnd(MouseEventArgs e) {
			// Switch back to place tool.
			if (e.Button == MouseButtons.Left) {
				editorControl.ChangeTool(1);
			}
		}

		public override void OnMouseDragMove(MouseEventArgs e) {
			Point2I mousePos	= new Point2I(e.X, e.Y);
			Room	room		= LevelDisplay.SampleRoom(mousePos);
			Point2I tileCoord	= LevelDisplay.SampleTileCoordinates(mousePos);
			if (room != null)
				ActivateTile(e.Button, room, tileCoord);
		}
	}
}
