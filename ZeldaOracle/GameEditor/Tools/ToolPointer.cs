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
using FormsControl = System.Windows.Forms.Control;

namespace ZeldaEditor.Tools {
	public class ToolPointer : EditorTool {
		private TileDataInstance selectedTile;
		private EventTileDataInstance selectedEventTile;
		private Room selectedRoom;


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
			//Deselect();
		}

		public override void Cut() {
			//Deselect();
		}
		
		public override void Copy() {

		}
		
		public override void Paste() {
			//Deselect();
		}
		
		public override void Delete() {
			LevelDisplayControl.DeleteTileSelection();
		}

		public override void SelectAll() {

		}

		public override void Deselect() {
			selectedEventTile	= null;
			selectedTile		= null;
			LevelDisplayControl.DeselectTiles();
			LevelDisplayControl.DeselectSelectionGrid();
			EditorControl.PropertyGrid.CloseProperties();

		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			selectedTile		= null;
			selectedEventTile	= null;
			EditorControl.HighlightMouseTile = false;
		}

		public override void OnEnd() {
			selectedTile		= null;
			selectedEventTile	= null;
		}

		public override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);

			Point2I mousePos = new Point2I(e.X, e.Y);
			
			// Sample the tile at the mouse position.
			BaseTileDataInstance baseTile = null;
			if (editorControl.EventMode)
				baseTile = LevelDisplayControl.SampleEventTile(mousePos);
			else
				baseTile = LevelDisplayControl.SampleTile(mousePos, editorControl.CurrentLayer);
			
			// Select or deselect the tile.
			if (e.Button == MouseButtons.Left && baseTile != null) {
				if (FormsControl.ModifierKeys == Keys.Control) {
					// Add or remove tiles from selection.
					if (!LevelDisplayControl.IsTileInSelection(baseTile)) {
						LevelDisplayControl.AddTileToSelection(baseTile);
						EditorControl.PropertyGrid.OpenProperties(baseTile);
					}
					else {
						LevelDisplayControl.RemoveTileFromSelection(baseTile);
					}
				}
				else {
					// Select a new tile, deselecting others.
					LevelDisplayControl.DeselectTiles();
					LevelDisplayControl.AddTileToSelection(baseTile);
					EditorControl.PropertyGrid.OpenProperties(baseTile);
				}
			}
			/*
			Room	room		= LevelDisplayControl.SampleRoom(mousePos);
			Point2I tileCoord	= LevelDisplayControl.SampleTileCoordinates(mousePos);

			if (e.Button == MouseButtons.Left && room != null) {
				if (!editorControl.EventMode) {
					if (FormsControl.ModifierKeys == Keys.Shift) {
						selectedRoom = room;
						Point2I levelTileCoord = LevelDisplayControl.ToLevelTileCoordinates(room, Point2I.Zero);
						LevelDisplayControl.SetSelectionBox(levelTileCoord, room.Size);
						EditorControl.PropertyGrid.OpenProperties(room.Properties, room);
					}
					else {
						// Select tiles.
						selectedTile = room.GetTile(tileCoord, editorControl.CurrentLayer);

						if (selectedTile != null) {
							Point2I levelTileCoord = LevelDisplayControl.ToLevelTileCoordinates(room, tileCoord);
							LevelDisplayControl.SetSelectionBox(levelTileCoord, Point2I.One);
							EditorControl.PropertyGrid.OpenProperties(selectedTile.Properties, selectedTile);
							LevelDisplayControl.SetSelectionBox(selectedTile);
							LevelDisplayControl.AddTileToSelection(selectedTile);
						}
						else {
							LevelDisplayControl.ClearSelectionBox();
							EditorControl.PropertyGrid.CloseProperties();
						}
					}
				}
				else {
					// Select events.
					selectedEventTile = LevelDisplayControl.SampleEventTile(mousePos);

					if (selectedEventTile != null) {
						Point2I levelTileCoord = LevelDisplayControl.ToLevelTileCoordinates(room, tileCoord);
						LevelDisplayControl.SetSelectionBox(levelTileCoord, Point2I.One);
						EditorControl.PropertyGrid.OpenProperties(selectedEventTile.Properties, selectedEventTile);
						LevelDisplayControl.SetSelectionBox(selectedEventTile);
					}
					else {
						LevelDisplayControl.ClearSelectionBox();
						EditorControl.PropertyGrid.CloseProperties();
					}
				}
			}
			*/
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
