using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game;
using ZeldaEditor.Undo;
using System.Windows.Input;
using Cursor = System.Windows.Forms.Cursor;
using ZeldaOracle.Game.Worlds.Editing;

namespace ZeldaEditor.Tools {
	public class ToolPointer : EditorTool {
		private const ModifierKeys RoomModeModifier = ModifierKeys.Shift;


		private TileDataInstance selectedTile;
		private ActionTileDataInstance selectedActionTile;
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
			if (selectedTile != null && selectedTile.Room.ContainsTile(selectedTile)) {
				ActionPlace action = ActionPlace.CreatePlaceAction(selectedTile.Room.Level, selectedTile.Layer, null);
				Point2I location = selectedTile.LevelCoord;
				action.AddOverwrittenTile(selectedTile);
				action.AddPlacedTile(location);
				EditorControl.PushAction(action, ActionExecution.Execute);
			}
			else if (selectedActionTile != null && selectedActionTile.Room.ContainsActionTile(selectedActionTile)) {
				ActionPlaceAction action = new ActionPlaceAction(selectedActionTile.Room.Level);
				action.AddOverwrittenActionTile(selectedActionTile);
				EditorControl.PushAction(action, ActionExecution.Execute);
			}
			LevelDisplay.ClearSelectionBox();
			EditorControl.EditingTileData = null;
			selectedActionTile	= null;
			selectedTile		= null;
			selectedRoom		= null;
			UpdateCommands();
		}

		public override void SelectAll() {

		}

		public override void Deselect() {
			LevelDisplay.ClearSelectionBox();
			EditorControl.EditingTileData = null;
			selectedActionTile	= null;
			selectedTile		= null;
			selectedRoom        = null;
			UpdateCommands();
		}


		//-----------------------------------------------------------------------------
		// Overridden State Methods
		//-----------------------------------------------------------------------------

		protected override void OnBegin(ToolEventArgs e) {
			selectedTile		= null;
			selectedActionTile	= null;
			selectedRoom        = null;
			ShowCursor = false;
			CursorPosition = e.SnappedPosition;
		}

		protected override void OnEnd(ToolEventArgs e) {
			LevelDisplay.ClearSelectionBox();
			EditorControl.EditingTileData = null;
			UpdateCommands();
		}

		protected override void OnUpdate(ToolEventArgs e) {
			if (selectedTile != null) {
				if (selectedTile.Room.ContainsTile(selectedTile))
					LevelDisplay.SetSelectionBox(selectedTile);
				else {
					selectedTile = null;
					LevelDisplay.ClearSelectionBox();
				}
			}
			else if (selectedActionTile != null) {
				if (selectedActionTile.Room.ContainsActionTile(selectedActionTile))
					LevelDisplay.SetSelectionBox(selectedActionTile);
				else {
					selectedActionTile = null;
					LevelDisplay.ClearSelectionBox();
				}
			}
			else if (selectedRoom != null) {
				if (!(selectedRoom.Location < Level.Dimensions)) {
					selectedRoom = null;
					LevelDisplay.ClearSelectionBox();
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseDown(ToolEventArgs e) {
			// Sample the tile at the mouse position.
			BaseTileDataInstance baseTile = null;
			if (ActionMode)
				baseTile = e.SampleActionTile;
			else
				baseTile = e.SampleTile;

			// Select or deselect the tile.
			if (e.Button == MouseButton.Left) {
				// Set to null by default
				selectedTile = null;
				selectedActionTile = null;
				selectedRoom = null;

				Room room = null;
				bool roomSelect = false;
				if (Modifiers.HasFlag(RoomModeModifier)) {
					room = e.SampleRoom;
					//room = LevelDisplay.SampleRoom(e.Position, false);
					roomSelect = true;
				}
				
				if (roomSelect) {
					selectedRoom = room;
					if (room != null) {
						selectedRoom = room;
						LevelDisplay.SetSelectionBox(room);
						EditorControl.PropertyGrid.OpenProperties(room);
						EditorControl.EditingRoom = room;
						UpdateCommands();
					}

					CursorPosition = Level.RoomLocationToLevelPosition(e.RoomLocation);
					CursorSize = Level.RoomPixelSize;
					ShowCursor = true;
				}
				else if (baseTile != null) {
					if (baseTile is TileDataInstance)
						selectedTile = baseTile as TileDataInstance;
					else if (baseTile is ActionTileDataInstance)
						selectedActionTile = baseTile as ActionTileDataInstance;

					CursorPosition = baseTile.LevelPosition;
					CursorSize = baseTile.PixelSize;
					ShowCursor = true;

					LevelDisplay.SetSelectionBox(baseTile);
					EditorControl.PropertyGrid.OpenProperties(baseTile);
					EditorControl.EditingTileData = baseTile;
					UpdateCommands();
				}
				else {
					LevelDisplay.ClearSelectionBox();
					EditorControl.PropertyGrid.CloseProperties();
					EditorControl.EditingTileData = null;
					ShowCursor = false;
					UpdateCommands();
				}
			}
		}

		protected override void OnMouseMove(ToolEventArgs e) {
			if (!ActionMode) {
				// Highlight tiles.
				TileDataInstance tile = e.SampleTile;
				ShowCursor = (tile != null);
				if (tile != null) {
					CursorPosition = tile.LevelPosition;
					CursorSize = tile.PixelSize;
				}
			}
			else {
				// Highlight action tiles.
				ActionTileDataInstance actionTile = e.SampleActionTile;
				ShowCursor = (actionTile != null);
				if (actionTile != null) {
					CursorPosition = actionTile.LevelPosition;
					CursorSize = actionTile.PixelSize;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		public override bool CanDeleteDeselect {
			get { return selectedTile != null || selectedActionTile != null || selectedRoom != null; }
		}

		public override int Snapping {
			// Always snap directly
			get { return 1; }
		}


		//-----------------------------------------------------------------------------
		// Tool-Specific Methods
		//-----------------------------------------------------------------------------

		public void GotoTile(BaseTileDataInstance baseTile) {
			selectedTile = null;
			selectedActionTile = null;
			selectedRoom = null;
			Point2I pixelPosition = Point2I.Zero;
			pixelPosition = LevelDisplay.GetLevelPixelDrawPosition(
				baseTile.LevelPosition + baseTile.PixelSize / 2);
			/*if (baseTile is TileDataInstance) {
				selectedTile = baseTile as TileDataInstance;
				pixelPosition = LevelDisplay.GetRoomDrawPosition(selectedTile.Room) +
					(selectedTile.Location * GameSettings.TILE_SIZE) + selectedTile.PixelSize / 2;
			}
			else if (baseTile is ActionTileDataInstance) {
				selectedActionTile = baseTile as ActionTileDataInstance;
				pixelPosition = LevelDisplay.GetRoomDrawPosition(selectedActionTile.Room) +
					selectedActionTile.Position + selectedActionTile.PixelSize / 2;
			}*/

			LevelDisplay.SetSelectionBox(baseTile);
			LevelDisplay.CenterViewOnPoint(pixelPosition);
		}

		public void GotoRoom(Room room) {
			selectedTile = null;
			selectedActionTile = null;
			selectedRoom = room;

			Point2I pixelPosition = LevelDisplay.GetRoomDrawPosition(room) + room.PixelSize / 2;

			LevelDisplay.SetSelectionBox(room);
			LevelDisplay.CenterViewOnPoint(pixelPosition);
		}
	}
}
