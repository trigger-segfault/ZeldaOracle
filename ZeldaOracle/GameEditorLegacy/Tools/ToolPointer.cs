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
using ZeldaOracle.Game;

namespace ZeldaEditor.Tools {
	public class ToolPointer : EditorTool {


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
			LevelDisplayControl.DeselectTiles();
			LevelDisplayControl.DeselectSelectionGrid();
			EditorControl.PropertyGrid.CloseProperties();

		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			EditorControl.HighlightMouseTile = false;
		}

		public override void OnEnd() {

		}

		public override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			
			Point2I mousePos = new Point2I(e.X, e.Y);
			Point2I levelPoint = LevelDisplayControl.SampleLevelPixelPosition(mousePos);

			// Open the object properties form when double clicking on a tile.
			foreach (BaseTileDataInstance tile in LevelDisplayControl.SelectedTiles) {
				Rectangle2I bounds = tile.GetBounds();
				bounds.Point += tile.Room.Location * tile.Room.Size * GameSettings.TILE_SIZE;

				if (bounds.Contains(levelPoint)) {
					EditorControl.EditorForm.OpenObjectPropertiesEditor(tile);
					break;
				}
			}
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
			if (e.Button == MouseButtons.Left) {
				/*if (FormsControl.ModifierKeys == Keys.Control) {
					if (baseTile != null) {
						// Add or remove tiles from selection.
						if (!LevelDisplayControl.IsTileInSelection(baseTile)) {
							LevelDisplayControl.AddTileToSelection(baseTile);
							EditorControl.PropertyGrid.OpenProperties(baseTile);
						}
						else {
							LevelDisplayControl.RemoveTileFromSelection(baseTile);
						}
					}
				}
				else */{
					// Select a new tile, deselecting others.
					LevelDisplayControl.DeselectTiles();
					if (baseTile != null) {
						LevelDisplayControl.AddTileToSelection(baseTile);
						EditorControl.PropertyGrid.OpenProperties(baseTile);
					}
					else
						EditorControl.PropertyGrid.CloseProperties();
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
