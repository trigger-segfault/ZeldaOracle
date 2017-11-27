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
using Key = System.Windows.Input.Key;

namespace ZeldaEditor.Tools {
	public class ToolSquare : EditorTool {
		private static readonly Cursor SquareCursor = LoadCursor("Square");

		private Point2I dragBeginTileCoord;
		
		private TileDataInstance drawTile;

		private Rectangle2I square;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolSquare() : base("Square Tool", Key.O) {
			
		}
		

		//-----------------------------------------------------------------------------
		// Overridden State Methods
		//-----------------------------------------------------------------------------

		protected override void OnInitialize() {
			MouseCursor = SquareCursor;
		}

		protected override void OnBegin() {
			EditorControl.HighlightMouseTile = true;
		}

		protected override void OnEnd() {
			LevelDisplay.ClearSelectionBox();
		}

		protected override void OnCancel() {
			LevelDisplay.ClearSelectionBox();
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			
			if (DragButton.IsOpposite(e.Button)) {
				Cancel();
			}
		}

		protected override void OnMouseDragBegin(MouseEventArgs e) {
			// Draw a new selecion box.
			if (DragButton.IsLeftOrRight() && !EditorControl.EventMode) {
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
				Point2I totalSize   = Level.Dimensions * Level.RoomSize;
				Point2I minCoord  = GMath.Max(GMath.Min(dragBeginTileCoord, levelTileCoord), Point2I.Zero);
				Point2I maxCoord  = GMath.Min(GMath.Max(dragBeginTileCoord, levelTileCoord), totalSize - 1);
				square = new Rectangle2I(minCoord, maxCoord - minCoord + 1);

				TileData tileData = EditorControl.SelectedTilesetTileData as TileData;
				if (e.Button == MouseButtons.Right)
					tileData = null;
				ActionSquare action = new ActionSquare(Level, EditorControl.CurrentLayer, square, tileData);
				for (int x = minCoord.X; x <= maxCoord.X && x < totalSize.X; x++) {
					for (int y = minCoord.Y; y <= maxCoord.Y && y < totalSize.Y; y++) {
						levelTileCoord = new Point2I(x, y);
						Room room = Level.GetRoomAt(levelTileCoord / Level.RoomSize);
						Point2I roomCoord = levelTileCoord % Level.RoomSize;
						TileDataInstance tile = room.GetTile(roomCoord, EditorControl.CurrentLayer);
						if (tile != null) {
							action.AddOverwrittenTile(levelTileCoord, tile);
						}
					}
				}
				EditorControl.PushAction(action, ActionExecution.Execute);
				LevelDisplay.ClearSelectionBox();
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			// Update selection box.
			if (IsDrawing) {
				Point2I levelTileCoord = LevelDisplay.SampleLevelTileCoordinates(e.MousePos());
				Point2I totalSize   = Level.Dimensions * Level.RoomSize;
				Point2I minCoord  = GMath.Max(GMath.Min(dragBeginTileCoord, levelTileCoord), Point2I.Zero);
				Point2I maxCoord  = GMath.Min(GMath.Max(dragBeginTileCoord, levelTileCoord), totalSize - 1);
				square = new Rectangle2I(minCoord, maxCoord - minCoord + 1);
				LevelDisplay.SetSelectionBox(square.Point * GameSettings.TILE_SIZE, square.Size * GameSettings.TILE_SIZE);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		public override bool CancelOnLayerChange { get { return true; } }


		//-----------------------------------------------------------------------------
		// Overridden Drawing Methods
		//-----------------------------------------------------------------------------

		public override bool DrawHideTile(TileDataInstance tile, Room room, Point2I levelCoord, int layer) {
			return (IsDrawing && drawTile == null && square.Contains(levelCoord) && layer == EditorControl.CurrentLayer);
		}

		public override void DrawTile(Graphics2D g, Room room, Point2I position, Point2I levelCoord, int layer) {
			if (!EditorControl.EventMode && layer == EditorControl.CurrentLayer) {
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
		// Internal Methods
		//-----------------------------------------------------------------------------

		private TileDataInstance CreateDrawTile() {
			TileData tileData = GetTileData();
			if (tileData != null)
				return new TileDataInstance(tileData);
			return null;
		}

		private TileData GetTileData() {
			return EditorControl.SelectedTilesetTileData as TileData;
		}
	}
}
