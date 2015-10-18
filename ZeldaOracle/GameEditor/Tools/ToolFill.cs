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
	public class ToolFill : EditorTool {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolFill() {
			name = "Fill Tool";
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void ActivateTile(MouseButtons mouseButton, Point2I levelTileCoord) {
			Room room = editorControl.Level.GetRoomAt(levelTileCoord / editorControl.Level.RoomSize);
			Point2I tileCoord = levelTileCoord % editorControl.Level.RoomSize;
			if (mouseButton == MouseButtons.Left) {
				room.CreateTile(
					editorControl.SelectedTilesetTileData,
					tileCoord.X, tileCoord.Y, editorControl.CurrentLayer
				);

			}
			else if (mouseButton == MouseButtons.Right) {
				/*if (editorControl.CurrentLayer == 0) {
					room.CreateTile(
						editorControl.Tileset.DefaultTileData,
						tileCoord.X, tileCoord.Y, editorControl.CurrentLayer
					);
				}
				else {*/
					room.RemoveTile(tileCoord.X, tileCoord.Y, editorControl.CurrentLayer);
				//}
			}
		}

		private TileDataInstance GetTileAt(Point2I levelTileCoord) {
			return editorControl.Level.GetRoomAt(levelTileCoord / editorControl.Level.RoomSize)
				.GetTile(levelTileCoord % editorControl.Level.RoomSize, editorControl.CurrentLayer);
		}

		private bool Matches(TileDataInstance targetTile, Point2I current) {
			TileDataInstance currentTile = GetTileAt(current);

			if (targetTile == null)
				return (currentTile == null);
			else if (currentTile != null)
				return (targetTile.TileData == currentTile.TileData);
			return false;
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
			Point2I target		= LevelDisplayControl.SampleLevelTileCoordinates(mousePos);
			Point2I totalSize	= editorControl.Level.Dimensions * editorControl.Level.RoomSize;
			TileDataInstance targetTile = GetTileAt(target);
			Point2I point;
			if (!EditorControl.EventMode) {
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
				else if (editorControl.CurrentLayer == 0 || GetTileAt(target) != null) {
					Queue<Point2I> nodes = new Queue<Point2I>();
					nodes.Enqueue(target);
					while (nodes.Count > 0) {
						point = nodes.Dequeue();
						if (Matches(targetTile, point)) {
							int width = 1;
							// Travel as far left as possible.
							for (; point.X - 1 >= 0 && Matches(targetTile, point - new Point2I(1, 0)); point.X--, width++) ;
							// Travel as far right as possible.
							for (; point.X + width < totalSize.X && Matches(targetTile, point + new Point2I(width, 0)); width++) ;

							// Fill the row of tiles.
							for (int i = 0; i < width; i++)
								ActivateTile(e.Button, point + new Point2I(i, 0));

							// This continue makes sure that one node is added for every range of pixels in a row.
							bool northContinue = false, southContinue = false;
							for (int i = 0; i < width; i++) {
								if (point.Y - 1 >= 0) {
									if (Matches(targetTile, point + new Point2I(i, -1))) {
										if (!northContinue) {
											nodes.Enqueue(point + new Point2I(i, -1));
											northContinue = true;
										}
									}
									else {
										northContinue = false;
									}
								}
								if (point.Y + 1 < totalSize.Y) {
									if (Matches(targetTile, point + new Point2I(i, 1))) {
										if (!southContinue) {
											nodes.Enqueue(point + new Point2I(i, 1));
											southContinue = true;
										}
									}
									else {
										southContinue = false;
									}
								}
							}
						}
					}
				}

			}
			else {
				if (e.Button == MouseButtons.Middle) {
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
		}

	}
}
