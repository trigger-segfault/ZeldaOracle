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
using ZeldaOracle.Game;
using ZeldaEditor.Undo;

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
			if (selectedTile != null) {
				ActionPlace action = ActionPlace.CreatePlaceAction(selectedTile.Room.Level, selectedTile.Layer, null);
				action.AddOverwrittenTile(selectedTile.Location + selectedTile.Room.Location * selectedTile.Room.Size, selectedTile);
				editorControl.PushAction(action, ActionExecution.Execute);
			}
			else if (selectedEventTile != null) {
				ActionEventTile action = new ActionEventTile(selectedEventTile.Room.Level);
				action.AddOverwrittenEventTile(selectedEventTile);
				editorControl.PushAction(action, ActionExecution.Execute);
			}
			selectedEventTile   = null;
			selectedTile        = null;
			selectedRoom        = null;
			UpdateCommands();
		}

		public override void SelectAll() {

		}

		public override void Deselect() {
			selectedEventTile	= null;
			selectedTile		= null;
			selectedRoom        = null;
			//EditorControl.PropertyGrid.CloseProperties();
			UpdateCommands();
		}

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		protected override void OnBegin() {
			selectedTile		= null;
			selectedEventTile	= null;
			selectedRoom        = null;
			EditorControl.HighlightMouseTile = false;
		}

		protected override void OnEnd() {
			selectedTile		= null;
			selectedEventTile	= null;
			selectedRoom        = null;
			LevelDisplay.ClearSelectionBox();
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


			// Select or deselect the tile.
			if (e.Button == MouseButtons.Left) {
				Room room = null;
				bool roomSelect = false;
				if (System.Windows.Forms.Control.ModifierKeys.HasFlag(Keys.Shift)) {
					room = LevelDisplay.SampleRoom(mousePos, false);
					roomSelect = true;
				}

				selectedTile = null;
				selectedEventTile = null;
				selectedRoom = null;
				LevelDisplay.DeselectTiles();
				if (roomSelect) {
					if (room != null) {
						selectedRoom = room;
						LevelDisplay.SetSelectionBox(room);
						EditorControl.PropertyGrid.OpenProperties(room);
					}
				}
				else if (baseTile != null) {
					if (baseTile is TileDataInstance)
						selectedTile = baseTile as TileDataInstance;
					else if (baseTile is EventTileDataInstance)
						selectedEventTile = baseTile as EventTileDataInstance;

					LevelDisplay.SetSelectionBox(baseTile);
					EditorControl.PropertyGrid.OpenProperties(baseTile);
				}
				else {
					LevelDisplay.ClearSelectionBox();
					EditorControl.PropertyGrid.CloseProperties();
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
				LevelDisplay.CursorTileSize = (tile != null ? tile.Size : Point2I.One);
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
				else {
					LevelDisplay.CursorTileSize = Point2I.One;
				}
			}
		}
		
		public override bool CanDeleteDeselect {
			get { return LevelDisplay.SelectedTiles.Any(); }
		}

		public void GotoTile(BaseTileDataInstance baseTile) {
			selectedTile = null;
			selectedEventTile = null;
			selectedRoom = null;
			Point2I pixelPosition = Point2I.Zero;
			if (baseTile is TileDataInstance) {
				selectedTile = baseTile as TileDataInstance;
				pixelPosition = LevelDisplay.GetRoomDrawPosition(selectedTile.Room) +
					(selectedTile.Location * GameSettings.TILE_SIZE) + selectedTile.GetBounds().Size / 2;
			}
			else if (baseTile is EventTileDataInstance) {
				selectedEventTile = baseTile as EventTileDataInstance;
				pixelPosition = LevelDisplay.GetRoomDrawPosition(selectedEventTile.Room) +
					selectedEventTile.Position + selectedEventTile.GetBounds().Size / 2;
			}

			LevelDisplay.SetSelectionBox(baseTile);
			LevelDisplay.CenterViewOnPoint(pixelPosition);
		}
		public void GotoRoom(Room room) {
			selectedTile = null;
			selectedEventTile = null;
			selectedRoom = room;

			Point2I pixelPosition = LevelDisplay.GetRoomDrawPosition(room) + room.Size * GameSettings.TILE_SIZE / 2;

			LevelDisplay.SetSelectionBox(room);
			LevelDisplay.CenterViewOnPoint(pixelPosition);
		}
	}
}
