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
using Key = System.Windows.Input.Key;

namespace ZeldaEditor.Tools {
	public class ToolPointer : EditorTool {
		private TileDataInstance selectedTile;
		private EventTileDataInstance selectedEventTile;
		private Room selectedRoom;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolPointer() : base("Pointer Tool", Key.M) {
			
		}

		
		//-----------------------------------------------------------------------------
		// Selection
		//-----------------------------------------------------------------------------

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
				EditorControl.PushAction(action, ActionExecution.Execute);
			}
			else if (selectedEventTile != null) {
				ActionEventTile action = new ActionEventTile(selectedEventTile.Room.Level);
				action.AddOverwrittenEventTile(selectedEventTile);
				EditorControl.PushAction(action, ActionExecution.Execute);
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
			UpdateCommands();
		}


		//-----------------------------------------------------------------------------
		// Overridden State Methods
		//-----------------------------------------------------------------------------

		protected override void OnBegin() {
			selectedTile		= null;
			selectedEventTile	= null;
			selectedRoom        = null;
			EditorControl.HighlightMouseTile = false;
		}

		protected override void OnEnd() {
			LevelDisplay.ClearSelectionBox();
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseDown(MouseEventArgs e) {
			Point2I mousePos = e.MousePos();
			
			// Sample the tile at the mouse position.
			BaseTileDataInstance baseTile = null;
			if (EditorControl.EventMode)
				baseTile = LevelDisplay.SampleEventTile(mousePos);
			else
				baseTile = LevelDisplay.SampleTile(mousePos, EditorControl.CurrentLayer);


			// Select or deselect the tile.
			if (e.Button == MouseButtons.Left) {
				Room room = null;
				bool roomSelect = false;
				if (System.Windows.Forms.Control.ModifierKeys.HasFlag(Keys.Shift)) {
					room = LevelDisplay.SampleRoom(mousePos, false);
					roomSelect = true;
				}
				
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

		protected override void OnMouseMove(MouseEventArgs e) {
			Point2I mousePos = e.MousePos();

			if (!EditorControl.EventMode) {
				// Highlight tiles.
				TileDataInstance tile = LevelDisplay.SampleTile(mousePos, EditorControl.CurrentLayer);
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


		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		public override bool CanDeleteDeselect {
			get { return selectedTile != null || selectedEventTile != null || selectedRoom != null; }
		}


		//-----------------------------------------------------------------------------
		// Tool-Specific Methods
		//-----------------------------------------------------------------------------

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
