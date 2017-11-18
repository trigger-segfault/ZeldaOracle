using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;

namespace ZeldaEditor.Tools {
	public class ToolPlace : EditorTool {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolPlace() {
			name = "Place Tool";
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void ActivateTile(MouseEventArgs e) {
			Point2I mousePos = new Point2I(e.X, e.Y);
			Point2I levelTileCoord = LevelDisplay.SampleLevelTileCoordinates(mousePos);

			// Limit tiles to the selection box if there is one.
			if (LevelDisplay.SelectionGridArea.IsEmpty ||
				LevelDisplay.SelectionGridArea.Contains(levelTileCoord))
			{
				Room room = LevelDisplay.Level.GetRoom((LevelTileCoord) levelTileCoord);
				if (room != null) {
					Point2I tileLocation = LevelDisplay.Level.GetTileLocation((LevelTileCoord) levelTileCoord);
					ActivateTile(e.Button, room, tileLocation);
				}
			}
		}

		private void ActivateTile(MouseButtons mouseButton, Room room, Point2I tileLocation) {
			TileDataInstance tile = room.GetTile(tileLocation, EditorControl.CurrentLayer);

			if (mouseButton == MouseButtons.Left) {
				TileData selectedTilesetTileData = editorControl.SelectedTilesetTileData as TileData;

				if (selectedTilesetTileData != null) {
					// Remove the existing tile.
					if (tile != null) {
						room.RemoveTile(tile);
						editorControl.OnDeleteObject(tile);
					}
					// Place the new tile.
					room.PlaceTile(
						new TileDataInstance(selectedTilesetTileData),
						tileLocation.X, tileLocation.Y, editorControl.CurrentLayer);
					editorControl.IsModified = true;
				}
			}
			else if (mouseButton == MouseButtons.Right) {
				// Erase the tile.
				if (tile != null) {
					room.RemoveTile(tile);
					editorControl.OnDeleteObject(tile);
					editorControl.IsModified = true;
				}
			}
			else if (mouseButton == MouseButtons.Middle) {
				// Sample the tile.
				if (tile != null) {
					editorControl.SelectedTilesetTile		= -Point2I.One;
					editorControl.SelectedTilesetTileData	= tile.TileData;
				}
			}
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnChangeLayer() {
			StopDragging();
		}

		public override void Initialize() {

		}

		public override void OnBegin() {
			EditorControl.HighlightMouseTile = true;
		}

		public override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);

			Point2I mousePos	= new Point2I(e.X, e.Y);
			Room room			= LevelDisplay.SampleRoom(mousePos);
			Point2I tileCoord	= LevelDisplay.SampleTileCoordinates(mousePos);

			if (editorControl.EventMode) {
				if (EditorControl.EventMode && e.Button == MouseButtons.Left) {
					EventTileData eventTile = editorControl.SelectedTilesetTileData as EventTileData;

					if (room != null && eventTile != null) {
						Point2I roomPos = LevelDisplay.GetRoomDrawPosition(room);
						Point2I pos = (mousePos - roomPos) / 8;
						pos *= 8;
						//Point2I tileCoord = LevelDisplayControl.SampleTileCoordinates(mousePos);
						room.CreateEventTile(eventTile, pos);
					}
				}
				else if (EditorControl.EventMode && e.Button == MouseButtons.Right) {
					EventTileDataInstance eventTile = LevelDisplay.SampleEventTile(new Point2I(e.X, e.Y));
					if (eventTile != null) {
						eventTile.Room.RemoveEventTile(eventTile);
						editorControl.OnDeleteObject(eventTile);
					}
				}
				else if (e.Button == MouseButtons.Middle) {
					// Select events.
					EventTileDataInstance selectedEventTile = LevelDisplay.SampleEventTile(mousePos);

					if (selectedEventTile != null) {
						Point2I levelTileCoord = LevelDisplay.ToLevelTileCoordinates(room, tileCoord);
						EditorControl.PropertyGrid.OpenProperties(selectedEventTile);
					}
					else {
						EditorControl.PropertyGrid.CloseProperties();
					}
				}
			}
			else {
				if (e.Button == MouseButtons.Middle) {
					// Select tiles.
					TileDataInstance selectedTile = room.GetTile(tileCoord, editorControl.CurrentLayer);

					if (selectedTile != null) {
						Point2I levelTileCoord = LevelDisplay.ToLevelTileCoordinates(room, tileCoord);
						EditorControl.PropertyGrid.OpenProperties(selectedTile);
					}
					else {
						EditorControl.PropertyGrid.CloseProperties();
					}
				}
			}
		}

		public override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
		}

		public override void OnMouseDragBegin(MouseEventArgs e) {
			if (!EditorControl.EventMode)
				ActivateTile(e);
		}

		public override void OnMouseDragEnd(MouseEventArgs e) {
			
		}

		public override void OnMouseDragMove(MouseEventArgs e) {
			if (!EditorControl.EventMode) {
				ActivateTile(e);
			}
		}
	}
}
