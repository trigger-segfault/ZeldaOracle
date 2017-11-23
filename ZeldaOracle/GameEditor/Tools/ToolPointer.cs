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
using Keyboard = System.Windows.Input.Keyboard;
using ModifierKeys = System.Windows.Input.ModifierKeys;

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
			/*editorControl.ToolSelection.Clipboard = LevelDisplay.GetSelectedTilesAsTileGrid(true);
			UpdateCommands();*/
		}
		
		public override void Copy() {
			/*editorControl.ToolSelection.Clipboard = LevelDisplay.GetSelectedTilesAsTileGrid(false);
			UpdateCommands();*/
		}
		
		public override void Paste() {
			
		}
		
		public override void Delete() {
			LevelDisplay.DeleteTileSelection();
			UpdateCommands();
		}

		public override void SelectAll() {

		}

		public override void Deselect() {
			selectedEventTile	= null;
			selectedTile		= null;
			LevelDisplay.DeselectTiles();
			LevelDisplay.DeselectSelectionGrid();
			EditorControl.PropertyGrid.CloseProperties();
			UpdateCommands();
		}

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		protected override void OnBegin() {
			selectedTile		= null;
			selectedEventTile	= null;
			EditorControl.HighlightMouseTile = false;
		}

		protected override void OnEnd() {
			selectedTile		= null;
			selectedEventTile	= null;
			LevelDisplay.DeselectTiles();
			LevelDisplay.DeleteSelectionGrid();
		}

		public override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			
			Point2I mousePos = new Point2I(e.X, e.Y);
			Point2I levelPoint = LevelDisplay.SampleLevelPixelPosition(mousePos);

			// Open the object properties form when double clicking on a tile.
			foreach (BaseTileDataInstance tile in LevelDisplay.SelectedTiles) {
				Rectangle2I bounds = tile.GetBounds();
				bounds.Point += tile.Room.Location * tile.Room.Size * GameSettings.TILE_SIZE;

				if (bounds.Contains(levelPoint)) {
					// TODO: Reimplement
					//EditorControl.EditorWindow.OpenObjectPropertiesEditor(tile);
					break;
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			
			Point2I mousePos = new Point2I(e.X, e.Y);
			
			// Sample the tile at the mouse position.
			BaseTileDataInstance baseTile = null;
			if (editorControl.EventMode)
				baseTile = LevelDisplay.SampleEventTile(mousePos);
			else
				baseTile = LevelDisplay.SampleTile(mousePos, editorControl.CurrentLayer);

			Room room = null;
			bool roomSelect = false;
			if (System.Windows.Forms.Control.ModifierKeys.HasFlag(Keys.Shift)) {
				room = LevelDisplay.SampleRoom(mousePos, false);
				roomSelect = true;
			}

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
					LevelDisplay.DeselectTiles();
					if (roomSelect) {
						if (room != null) {
							LevelDisplay.SelectRoom(room);
							EditorControl.PropertyGrid.OpenProperties(room);
						}
					}
					else if (baseTile != null) {
						LevelDisplay.AddTileToSelection(baseTile);
						// TODO: Reimplement
						EditorControl.PropertyGrid.OpenProperties(baseTile);
					}
					else {
						EditorControl.PropertyGrid.CloseProperties();
					}
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
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
		
		public override bool CanDeleteDeselect {
			get { return LevelDisplay.SelectedTiles.Any(); }
		}
	}
}
