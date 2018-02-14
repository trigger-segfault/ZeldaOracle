using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game;
using ZeldaEditor.Undo;
using Key = System.Windows.Input.Key;

namespace ZeldaEditor.Tools {
	public class ToolPointer : EditorTool {
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
				action.AddOverwrittenTile(selectedTile.Location + selectedTile.Room.Location * selectedTile.Room.Size, selectedTile);
				EditorControl.PushAction(action, ActionExecution.Execute);
			}
			else if (selectedActionTile != null && selectedActionTile.Room.ContainsActionTile(selectedActionTile)) {
				ActionPlaceAction action = new ActionPlaceAction(selectedActionTile.Room.Level);
				action.AddOverwrittenActionTile(selectedActionTile);
				EditorControl.PushAction(action, ActionExecution.Execute);
			}
			selectedActionTile	= null;
			selectedTile		= null;
			selectedRoom		= null;
			UpdateCommands();
		}

		public override void SelectAll() {

		}

		public override void Deselect() {
			selectedActionTile	= null;
			selectedTile		= null;
			selectedRoom        = null;
			UpdateCommands();
		}


		//-----------------------------------------------------------------------------
		// Overridden State Methods
		//-----------------------------------------------------------------------------

		protected override void OnBegin() {
			selectedTile		= null;
			selectedActionTile	= null;
			selectedRoom        = null;
			EditorControl.HighlightMouseTile = false;
		}

		protected override void OnEnd() {
			LevelDisplay.ClearSelectionBox();
		}

		protected override void OnUpdate() {
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

		protected override void OnMouseDown(MouseEventArgs e) {
			Point2I mousePos = e.MousePos();
			
			// Sample the tile at the mouse position.
			BaseTileDataInstance baseTile = null;
			if (EditorControl.ActionMode)
				baseTile = LevelDisplay.SampleActionTile(mousePos);
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
					else if (baseTile is ActionTileDataInstance)
						selectedActionTile = baseTile as ActionTileDataInstance;

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

			if (!EditorControl.ActionMode) {
				// Highlight tiles.
				TileDataInstance tile = LevelDisplay.SampleTile(mousePos, EditorControl.CurrentLayer);
				EditorControl.HighlightMouseTile = (tile != null);
				if (tile != null) {
					LevelDisplay.CursorTileLocation = tile.Room.Location * Level.RoomSize +
						tile.Location;
				}
				LevelDisplay.CursorTileSize = (tile != null ? tile.Size : Point2I.One);
			}
			else {
				// Highlight action tiles.
				ActionTileDataInstance actionTile = LevelDisplay.SampleActionTile(mousePos);
				EditorControl.HighlightMouseTile = (actionTile != null);
				if (actionTile != null) {
					LevelDisplay.CursorHalfTileLocation =
						LevelDisplay.SampleLevelHalfTileCoordinates(
							LevelDisplay.GetRoomDrawPosition(actionTile.Room) + actionTile.Position);
					LevelDisplay.CursorTileSize = actionTile.Size;
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
			get { return selectedTile != null || selectedActionTile != null || selectedRoom != null; }
		}


		//-----------------------------------------------------------------------------
		// Tool-Specific Methods
		//-----------------------------------------------------------------------------

		public void GotoTile(BaseTileDataInstance baseTile) {
			selectedTile = null;
			selectedActionTile = null;
			selectedRoom = null;
			Point2I pixelPosition = Point2I.Zero;
			if (baseTile is TileDataInstance) {
				selectedTile = baseTile as TileDataInstance;
				pixelPosition = LevelDisplay.GetRoomDrawPosition(selectedTile.Room) +
					(selectedTile.Location * GameSettings.TILE_SIZE) + selectedTile.GetBounds().Size / 2;
			}
			else if (baseTile is ActionTileDataInstance) {
				selectedActionTile = baseTile as ActionTileDataInstance;
				pixelPosition = LevelDisplay.GetRoomDrawPosition(selectedActionTile.Room) +
					selectedActionTile.Position + selectedActionTile.GetBounds().Size / 2;
			}

			LevelDisplay.SetSelectionBox(baseTile);
			LevelDisplay.CenterViewOnPoint(pixelPosition);
		}

		public void GotoRoom(Room room) {
			selectedTile = null;
			selectedActionTile = null;
			selectedRoom = room;

			Point2I pixelPosition = LevelDisplay.GetRoomDrawPosition(room) + room.Size * GameSettings.TILE_SIZE / 2;

			LevelDisplay.SetSelectionBox(room);
			LevelDisplay.CenterViewOnPoint(pixelPosition);
		}
	}
}
