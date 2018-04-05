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

		private object selectedObject;


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
			// Delete the selected object
			if (SelectedTile != null && SelectedTile.Room.ContainsTile(SelectedTile)) {
				ActionPlace action = ActionPlace.CreatePlaceAction(SelectedTile.Room.Level, SelectedTile.Layer, null);
				Point2I location = SelectedTile.LevelCoord;
				action.AddOverwrittenTile(SelectedTile);
				action.AddPlacedTile(location);
				EditorControl.PushAction(action, ActionExecution.Execute);
			}
			else if (SelectedActionTile != null && SelectedActionTile.Room.ContainsActionTile(SelectedActionTile)) {
				ActionPlaceAction action = new ActionPlaceAction(SelectedActionTile.Room.Level);
				action.AddOverwrittenActionTile(SelectedActionTile);
				EditorControl.PushAction(action, ActionExecution.Execute);
			}

			if (SelectedRoom == null) {
				// Deselect
				LevelDisplay.ClearSelectionBox();
				EditorControl.EditingTileData = null;
				selectedObject = null;
				UpdateCommands();
			}
		}

		public override void SelectAll() {

		}

		public override void Deselect() {
			LevelDisplay.ClearSelectionBox();
			EditorControl.EditingTileData = null;
			selectedObject = null;
			UpdateCommands();
		}


		//-----------------------------------------------------------------------------
		// Overridden State Methods
		//-----------------------------------------------------------------------------

		protected override void OnBegin(ToolEventArgs e) {
			selectedObject = null;
			ShowCursor = false;
			CursorPosition = e.SnappedPosition;
		}

		protected override void OnEnd(ToolEventArgs e) {
			LevelDisplay.ClearSelectionBox();
			EditorControl.EditingTileData = null;
			UpdateCommands();
		}

		protected override void OnUpdate(ToolEventArgs e) {
			if (SelectedTile != null) {
				if (SelectedTile.Room.ContainsTile(SelectedTile))
					LevelDisplay.SetSelectionBox(SelectedTile);
				else
					Deselect();
			}
			else if (SelectedActionTile != null) {
				if (SelectedActionTile.Room.ContainsActionTile(SelectedActionTile))
					LevelDisplay.SetSelectionBox(SelectedActionTile);
				else
					Deselect();
			}
			else if (SelectedRoom != null) {
				if (SelectedRoom.Location <= Point2I.Zero ||
					SelectedRoom.Location >= Level.Dimensions)
					Deselect();
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

			// Select or deselect the tile
			if (e.Button == MouseButton.Left) {
				// Set to null by default
				selectedObject = null;

				Room room = null;
				bool roomSelect = false;
				if (Modifiers.HasFlag(RoomModeModifier)) {
					room = e.SampleRoom;
					//room = LevelDisplay.SampleRoom(e.Position, false);
					roomSelect = true;
				}
				
				if (roomSelect) {
					selectedObject = room;

					if (room != null) {
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
					selectedObject = baseTile;
					
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

		protected override void OnMouseDoubleClick(ToolEventArgs e) {
			EditorControl.EditorWindow.OpenObjectEditor(selectedObject);
		}


		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		public TileDataInstance SelectedTile {
			get { return selectedObject as TileDataInstance; }
		}

		public ActionTileDataInstance SelectedActionTile {
			get { return selectedObject as ActionTileDataInstance; }
		}

		public Room SelectedRoom {
			get { return selectedObject as Room; }
		}

		public override bool CanDeleteDeselect {
			get { return (selectedObject != null); }
		}

		public override int Snapping {
			// Always snap directly
			get { return 1; }
		}


		//-----------------------------------------------------------------------------
		// Tool-Specific Methods
		//-----------------------------------------------------------------------------

		public void GotoTile(BaseTileDataInstance baseTile) {
			selectedObject = baseTile;

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
			selectedObject = room;

			Point2I pixelPosition = LevelDisplay.GetRoomDrawPosition(room) + room.PixelSize / 2;

			LevelDisplay.SetSelectionBox(room);
			LevelDisplay.CenterViewOnPoint(pixelPosition);
		}
	}
}
