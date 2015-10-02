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
	public class ToolPointer : EditorTool {
		private TileDataInstance selectedTile;
		private EventTileDataInstance selectedEventTile;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolPointer() {
			name = "Pointer Tool";
		}

		
		//-----------------------------------------------------------------------------
		// Selection
		//-----------------------------------------------------------------------------

		public override void OnChangeLayer() {
			Deselect();
		}

		public override void Cut() {
			Deselect();
		}
		
		public override void Copy() {

		}
		
		public override void Paste() {
			Deselect();
		}
		
		public override void Delete() {
			if (!EditorControl.EventMode) {
				if (selectedTile != null)
					selectedTile.Room.RemoveTile(selectedTile);
			}
			else {
				if (selectedEventTile != null)
					selectedEventTile.Room.RemoveEventTile(selectedEventTile);
			}
			Deselect();
		}

		public override void SelectAll() {
			if (EditorControl.EventMode) {
				// Select all events.

			}
		}

		public override void Deselect() {
			selectedEventTile = null;
			selectedTile = null;
			LevelDisplayControl.ClearSelectionBox();
			EditorControl.PropertyGridControl.CloseProperties();
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			selectedTile = null;
			selectedEventTile = null;
			EditorControl.HighlightMouseTile = false;
		}

		public override void OnEnd() {
			selectedTile = null;
			selectedEventTile = null;
			Deselect();
		}

		public override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			
			Point2I mousePos	= new Point2I(e.X, e.Y);
			Room	room		= LevelDisplayControl.SampleRoom(mousePos);
			Point2I tileCoord	= LevelDisplayControl.SampleTileCoordinates(mousePos);

			if (e.Button == MouseButtons.Left && room != null) {
				if (!editorControl.EventMode) {
					// Select tiles.
					selectedTile = room.GetTile(tileCoord, editorControl.CurrentLayer);

					if (selectedTile != null) {
						Point2I levelTileCoord = LevelDisplayControl.ToLevelTileCoordinates(room, tileCoord);
						LevelDisplayControl.SetSelectionBox(levelTileCoord, Point2I.One);
						EditorControl.PropertyGridControl.OpenProperties(selectedTile.Properties);
					}
					else {
						LevelDisplayControl.ClearSelectionBox();
						EditorControl.PropertyGridControl.CloseProperties();
					}
				}
				else {
					// Select events.
					selectedEventTile = LevelDisplayControl.SampleEventTile(mousePos);

					if (selectedEventTile != null) {
						Point2I levelTileCoord = LevelDisplayControl.ToLevelTileCoordinates(room, tileCoord);
						LevelDisplayControl.SetSelectionBox(levelTileCoord, Point2I.One);
						EditorControl.PropertyGridControl.OpenProperties(selectedEventTile.ModifiedProperties);
					}
					else {
						LevelDisplayControl.ClearSelectionBox();
						EditorControl.PropertyGridControl.CloseProperties();
					}
				}
			}
		}

		public override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
		}

		public override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			
			Point2I mousePos = new Point2I(e.X, e.Y);

			if (!editorControl.EventMode) {
				// Highlight tiles.
				TileDataInstance tile = LevelDisplayControl.SampleTile(mousePos, editorControl.CurrentLayer);
				EditorControl.HighlightMouseTile = (tile != null);
			}
			else {
				// Highlight event tiles.
				EventTileDataInstance eventTile = LevelDisplayControl.SampleEventTile(mousePos);
				EditorControl.HighlightMouseTile = (eventTile != null);
			}
		}

	}
}
