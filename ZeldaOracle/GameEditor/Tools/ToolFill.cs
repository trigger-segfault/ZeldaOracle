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

		private TileDataInstance GetTileAt(Point2I levelTileCoord) {
			return editorControl.Level.GetRoomAt(levelTileCoord / editorControl.Level.RoomSize)
				.GetTile(levelTileCoord % editorControl.Level.RoomSize, editorControl.CurrentLayer);
		}

		private bool Matches(TileDataInstance targetTile, Point2I current) {
			TileDataInstance currentTile = GetTileAt(current);
			return Matches(targetTile, GetTileAt(current));
		}

		private bool Matches(TileDataInstance tile1, TileDataInstance tile2) {
			if (tile1 == null)
				return (tile2 == null);
			else if (tile2 != null)
				return (tile1.TileData == tile2.TileData);
			return false;
		}

		private void Fill(Point2I target, TileData fillData) {
			Point2I totalSize = editorControl.Level.Dimensions * editorControl.Level.RoomSize;
			TileDataInstance targetTile = GetTileAt(target);
			Point2I point;

			// Don't fill in the same tiles.
			if (fillData == null && targetTile == null)
				return;
			if (targetTile != null && targetTile.TileData == fillData)
				return;

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
						ReplaceTile(point + new Point2I(i, 0), fillData);

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

		private void ReplaceTile(Point2I levelTileCoord, TileData tileData) {
			Room room = editorControl.Level.GetRoomAt(levelTileCoord / editorControl.Level.RoomSize);
			Point2I tileCoord = levelTileCoord % editorControl.Level.RoomSize;

			if (tileData != null)
				room.CreateTile(tileData, tileCoord, editorControl.CurrentLayer);
			else
				room.RemoveTile(tileCoord.X, tileCoord.Y, editorControl.CurrentLayer);
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
			Point2I target		= LevelDisplay.SampleLevelTileCoordinates(mousePos);
			
			if (!EditorControl.EventMode) {
				if (e.Button == MouseButtons.Middle) {
					// Select/sample tiles.
					TileDataInstance selectedTile = room.GetTile(tileCoord, editorControl.CurrentLayer);

					if (selectedTile != null) {
						Point2I levelTileCoord = LevelDisplay.ToLevelTileCoordinates(room, tileCoord);
						EditorControl.PropertyGrid.OpenProperties(selectedTile);
						editorControl.SelectedTilesetTile = -Point2I.One;
						editorControl.SelectedTilesetTileData = selectedTile.TileData;
					}
					else {
						EditorControl.PropertyGrid.CloseProperties();
					}
				}
				else if (editorControl.CurrentLayer == 0 || GetTileAt(target) != null) {
					// Fill tiles.
					TileData fillData = editorControl.SelectedTilesetTileData as TileData;
					if (fillData != null) {
						if (e.Button == MouseButtons.Right)
							fillData = null;
						Fill(target, fillData);
						editorControl.IsModified = true;
					}
				}

			}
			else {
				if (e.Button == MouseButtons.Middle) {
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
		}

	}
}
