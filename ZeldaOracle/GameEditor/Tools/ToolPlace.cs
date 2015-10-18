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

		private void ActivateTile(MouseButtons mouseButton, Room room, Point2I tileLocation) {
			if (mouseButton == MouseButtons.Left) {
				room.CreateTile(
					editorControl.SelectedTilesetTileData,
					tileLocation.X, tileLocation.Y, editorControl.CurrentLayer
				);

			}
			else if (mouseButton == MouseButtons.Right) {
				/*if (editorControl.CurrentLayer == 0) {
					room.CreateTile(
						editorControl.Tileset.DefaultTileData,
						tileLocation.X, tileLocation.Y, editorControl.CurrentLayer
					);
				}
				else {*/
					room.RemoveTile(tileLocation.X, tileLocation.Y, editorControl.CurrentLayer);
				//}
			}
			else if (mouseButton == MouseButtons.Middle) {
				// Sample the tile.
				TileDataInstance tile = room.GetTile(tileLocation, EditorControl.CurrentLayer);
				if (tile != null) {
					editorControl.SelectedTilesetTile = -Point2I.One;
					editorControl.SelectedTilesetTileData = tile.TileData;
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

		}

		public override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);

			Point2I mousePos	= new Point2I(e.X, e.Y);
			Room room			= LevelDisplayControl.SampleRoom(mousePos);
			Point2I tileCoord	= LevelDisplayControl.SampleTileCoordinates(mousePos);

			if (EditorControl.EventMode) {
				if (EditorControl.EventMode && e.Button == MouseButtons.Left) {
					//EventTileData eventTile = Resources.GetResource<EventTileData>("warp");
					EventTileData eventTile = Resources.GetResource<EventTileData>("npc");

					//Point2I mousePos	= new Point2I(e.X, e.Y);
					//Room room		= LevelDisplayControl.SampleRoom(mousePos);

					if (room != null) {
						Point2I roomPos = LevelDisplayControl.GetRoomDrawPosition(room);
						Point2I pos = (mousePos - roomPos) / 8;
						pos *= 8;
						//Point2I tileCoord = LevelDisplayControl.SampleTileCoordinates(mousePos);
						room.CreateEventTile(eventTile, pos);
					}
				}
				else if (EditorControl.EventMode && e.Button == MouseButtons.Right) {
					EventTileDataInstance eventTile = LevelDisplayControl.SampleEventTile(new Point2I(e.X, e.Y));
					if (eventTile != null) {
						eventTile.Room.RemoveEventTile(eventTile);
					}
				}
				else if (e.Button == MouseButtons.Middle) {
					// Select events.
					EventTileDataInstance selectedEventTile = LevelDisplayControl.SampleEventTile(mousePos);

					if (selectedEventTile != null) {
						Point2I levelTileCoord = LevelDisplayControl.ToLevelTileCoordinates(room, tileCoord);
						EditorControl.PropertyGridControl.OpenProperties(selectedEventTile.Properties, selectedEventTile);
					}
					else {
						EditorControl.PropertyGridControl.CloseProperties();
					}
				}
			}
			else {
				if (e.Button == MouseButtons.Middle) {
					// Select tiles.
					TileDataInstance selectedTile = room.GetTile(tileCoord, editorControl.CurrentLayer);

					if (selectedTile != null) {
						Point2I levelTileCoord = LevelDisplayControl.ToLevelTileCoordinates(room, tileCoord);
						EditorControl.PropertyGridControl.OpenProperties(selectedTile.Properties, selectedTile);
					}
					else {
						EditorControl.PropertyGridControl.CloseProperties();
					}
				}
			}
		}

		public override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
		}

		public override void OnMouseDragBegin(MouseEventArgs e) {
			if (!EditorControl.EventMode) {
				Point2I mousePos	= new Point2I(e.X, e.Y);
				Room	room		= LevelDisplayControl.SampleRoom(mousePos);
				Point2I tileCoord	= LevelDisplayControl.SampleTileCoordinates(mousePos);
				if (room != null)
					ActivateTile(e.Button, room, tileCoord);
			}
		}

		public override void OnMouseDragEnd(MouseEventArgs e) {
			
		}

		public override void OnMouseDragMove(MouseEventArgs e) {
			if (!EditorControl.EventMode) {
				Point2I mousePos	= new Point2I(e.X, e.Y);
				Room	room		= LevelDisplayControl.SampleRoom(mousePos);
				Point2I tileCoord	= LevelDisplayControl.SampleTileCoordinates(mousePos);
				if (room != null)
					ActivateTile(e.Button, room, tileCoord);
			}
		}
	}
}
