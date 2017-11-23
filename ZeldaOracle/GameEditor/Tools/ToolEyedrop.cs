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

		private static readonly Cursor EyedropperCursor = LoadCursor("Eyedropper");

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

		protected override void OnInitialize() {
			MouseCursor = EyedropperCursor;
		}

		public override void OnChangeLayer() {
			
		}

		protected override void OnBegin() {
			EditorControl.HighlightMouseTile = false;
		}

		protected override void OnMouseMove(MouseEventArgs e) {
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

		protected override void OnMouseDragBegin(MouseEventArgs e) {
			Point2I mousePos	= new Point2I(e.X, e.Y);
			Room	room		= LevelDisplay.SampleRoom(mousePos);
			Point2I tileCoord	= LevelDisplay.SampleTileCoordinates(mousePos);
			if (room != null)
				ActivateTile(e.Button, room, tileCoord);
		}

		protected override void OnMouseDragEnd(MouseEventArgs e) {
			// Switch back to place tool.
			if (e.Button == MouseButtons.Left) {
				editorControl.ChangeTool(1);
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			Point2I mousePos	= new Point2I(e.X, e.Y);
			Room	room		= LevelDisplay.SampleRoom(mousePos);
			Point2I tileCoord	= LevelDisplay.SampleTileCoordinates(mousePos);
			if (room != null)
				ActivateTile(e.Button, room, tileCoord);
		}
	}
}
