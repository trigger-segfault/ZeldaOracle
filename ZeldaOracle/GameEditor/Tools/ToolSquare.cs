using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game;
using ZeldaEditor.Undo;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Graphics;

namespace ZeldaEditor.Tools {
	public class ToolSquare : EditorTool {
		private static readonly Cursor SquareCursor = LoadCursor("Square");

		private Point2I dragBeginTileCoord;

		private ActionSquare action;
		private TileDataInstance drawTile;

		private Rectangle2I square;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolSquare() {
			name = "Square Tool";
		}
		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		protected override void OnCancel() {
			LevelDisplay.ClearSelectionBox();
			action = null;
		}

		protected override void OnInitialize() {
			MouseCursor = SquareCursor;
		}

		protected override void OnBegin() {
			EditorControl.HighlightMouseTile = true;
		}

		protected override void OnEnd() {
			LevelDisplay.ClearSelectionBox();
			action = null;
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			
			if (DragButton.IsOpposite(e.Button)) {
				Cancel();
			}
		}

		protected override void OnMouseDragBegin(MouseEventArgs e) {
			// Draw a new selecion box.
			if (DragButton.IsLeftOrRight() && !editorControl.EventMode) {
				IsDrawing = true;

				if (DragButton == MouseButtons.Left)
					drawTile = CreateDrawTile();
				else
					drawTile = null;
				
				dragBeginTileCoord	= LevelDisplay.SampleLevelTileCoordinates(e.MousePos());
				square = new Rectangle2I(dragBeginTileCoord, Point2I.One);
				LevelDisplay.SetSelectionBox(dragBeginTileCoord * GameSettings.TILE_SIZE, (Point2I)GameSettings.TILE_SIZE);
			}
		}

		protected override void OnMouseDragEnd(MouseEventArgs e) {
			if (IsDrawing) {
				IsDrawing = false;
				Point2I levelTileCoord = LevelDisplay.SampleLevelTileCoordinates(e.MousePos());
				Point2I totalSize   = editorControl.Level.Dimensions * editorControl.Level.RoomSize;
				Point2I minCoord  = GMath.Max(GMath.Min(dragBeginTileCoord, levelTileCoord), Point2I.Zero);
				Point2I maxCoord  = GMath.Min(GMath.Max(dragBeginTileCoord, levelTileCoord), totalSize - 1);
				square = new Rectangle2I(minCoord, maxCoord - minCoord + 1);

				TileData tileData = editorControl.SelectedTilesetTileData as TileData;
				if (e.Button == MouseButtons.Right)
					tileData = null;
				action = new ActionSquare(editorControl.Level, editorControl.CurrentLayer, square, tileData);
				for (int x = minCoord.X; x <= maxCoord.X && x < totalSize.X; x++) {
					for (int y = minCoord.Y; y <= maxCoord.Y && y < totalSize.Y; y++) {
						levelTileCoord = new Point2I(x, y);
						Room room = editorControl.Level.GetRoomAt(levelTileCoord / editorControl.Level.RoomSize);
						Point2I roomCoord = levelTileCoord % editorControl.Level.RoomSize;
						TileDataInstance tile = room.GetTile(roomCoord, editorControl.CurrentLayer);
						if (tile != null) {
							action.AddOverwrittenTile(levelTileCoord, tile);
						}
					}
				}
				editorControl.PushAction(action, ActionExecution.Execute);
				action = null;
				LevelDisplay.ClearSelectionBox();
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			// Update selection box.
			if (IsDrawing) {
				Point2I levelTileCoord = LevelDisplay.SampleLevelTileCoordinates(e.MousePos());
				Point2I totalSize   = editorControl.Level.Dimensions * editorControl.Level.RoomSize;
				Point2I minCoord  = GMath.Max(GMath.Min(dragBeginTileCoord, levelTileCoord), Point2I.Zero);
				Point2I maxCoord  = GMath.Min(GMath.Max(dragBeginTileCoord, levelTileCoord), totalSize - 1);
				square = new Rectangle2I(minCoord, maxCoord - minCoord + 1);
				LevelDisplay.SetSelectionBox(square.Point * GameSettings.TILE_SIZE, square.Size * GameSettings.TILE_SIZE);
			}
		}

		//-----------------------------------------------------------------------------
		// Virtual drawing
		//-----------------------------------------------------------------------------

		public override bool DrawHideTile(TileDataInstance tile, Room room, Point2I levelCoord, int layer) {
			return (IsDrawing && drawTile == null && square.Contains(levelCoord));
		}

		public override void DrawTile(Graphics2D g, Room room, Point2I position, Point2I levelCoord, int layer) {
			if (!EditorControl.EventMode && layer == editorControl.CurrentLayer) {
				if (!IsDrawing && levelCoord == LevelDisplay.CursorTileLocation) {
					TileDataInstance tile = CreateDrawTile();
					if (tile != null) {
						LevelDisplay.DrawTile(g, room, tile, position, LevelDisplay.FadeAboveColor);
					}
				}
				else if (IsDrawing && square.Contains(levelCoord) && drawTile != null) {
					LevelDisplay.DrawTile(g, room, drawTile, position, LevelDisplay.NormalColor);
				}
			}
		}

		//-----------------------------------------------------------------------------
		// Helpers
		//-----------------------------------------------------------------------------

		private TileDataInstance CreateDrawTile() {
			TileData tileData = GetTileData();
			if (tileData != null)
				return new TileDataInstance(tileData);
			return null;
		}

		private TileData GetTileData() {
			return editorControl.SelectedTilesetTileData as TileData;
		}
	}
}
