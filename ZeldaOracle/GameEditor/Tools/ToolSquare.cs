using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Worlds.Editing;
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
			if (DragButton.IsLeftOrRight() && !ActionMode && IsTileSingle) {
				IsDrawing = true;

				if (DragButton == MouseButtons.Left)
					drawTile = CreateDrawTile();
				else
					drawTile = null;
				
				dragBeginTileCoord = LevelDisplay.SampleLevelCoord(e.MousePos());
				square = new Rectangle2I(dragBeginTileCoord, Point2I.One);
				LevelDisplay.SetSelectionBox(Level.LevelCoordToPosition(square));
			}
		}

		protected override void OnMouseDragEnd(MouseEventArgs e) {
			if (IsDrawing) {
				IsDrawing = false;
				Point2I levelCoord = LevelDisplay.SampleLevelCoord(e.MousePos());
				/*Point2I totalSize   = Level.Dimensions * Level.RoomSize;
				Point2I minCoord  = GMath.Max(GMath.Min(dragBeginTileCoord, levelTileCoord), Point2I.Zero);
				Point2I maxCoord  = GMath.Min(GMath.Max(dragBeginTileCoord, levelTileCoord), totalSize - 1);
				square = new Rectangle2I(minCoord, maxCoord - minCoord + 1);*/
				square = Rectangle2I.FromEndPointsOne(dragBeginTileCoord, levelCoord);
				square = Rectangle2I.Intersect(square, Level.TileBounds);

				TileData tileData = EditorControl.SelectedTileData as TileData;
				if (e.Button == MouseButtons.Right)
					tileData = null;
				ActionSquare action = new ActionSquare(Level, Layer, square, tileData);
				for (int x = square.Left; x < square.Right; x++) {
					for (int y = square.Top; y < square.Bottom; y++) {
						levelCoord = new Point2I(x, y);
						/*Room room = Level.GetRoomAt(levelCoord / Level.RoomSize);
						Point2I roomCoord = levelCoord % Level.RoomSize;
						TileDataInstance tile = room.GetTile(roomCoord, Layer);
						if (tile != null) {
							action.AddOverwrittenTile(tile);
						}*/
						action.AddOverwrittenTile(Level.GetTileAt(levelCoord, Layer));
					}
				}
				EditorControl.PushAction(action, ActionExecution.Execute);
				LevelDisplay.ClearSelectionBox();
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			// Update selection box.
			if (IsDrawing) {
				Point2I levelCoord = LevelDisplay.SampleLevelCoord(e.MousePos());
				/*Point2I totalSize	= Level.TileDimensions;
				Point2I minCoord	= GMath.Max(GMath.Min(dragBeginTileCoord, levelCoord), Point2I.Zero);
				Point2I maxCoord	= GMath.Min(GMath.Max(dragBeginTileCoord, levelCoord), totalSize - 1);*/
				square = Rectangle2I.FromEndPointsOne(dragBeginTileCoord, levelCoord);
				square = Rectangle2I.Intersect(square, Level.TileBounds);
				LevelDisplay.SetSelectionBox(Level.LevelCoordToPosition(square));
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		public override bool CancelOnLayerChange { get { return true; } }


		//-----------------------------------------------------------------------------
		// Overridden Drawing Methods
		//-----------------------------------------------------------------------------

		public override bool DrawHideTile(TileDataInstance tile, Room room,
			Point2I levelCoord, int layer)
		{
			return (IsDrawing && square.Intersects(tile.LevelTileBounds) &&
					layer == Layer);
		}

		public override void DrawTile(Graphics2D g, Room room, Point2I position,
			Point2I levelCoord, int layer)
		{
			if (!ActionMode && layer == Layer) {
				if (!IsDrawing && levelCoord == LevelDisplay.CursorTileLocation) {
					TileDataInstance tile = CreateDrawTile();
					if (tile != null) {
						LevelDisplay.DrawTile(g, room, tile, position,
							LevelDisplay.FadeAboveColor);
					}
				}
				else if (IsDrawing && square.Contains(levelCoord) && drawTile != null) {
					LevelDisplay.DrawTile(g, room, drawTile, position,
						LevelDisplay.NormalColor);
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
			return EditorControl.SelectedTileData as TileData;
		}
		
		private bool IsTileSingle {
			get {
				if (GetTileData() != null)
					return (GetTileData().TileSize == Point2I.One);
				return true;
			}
		}
	}
}
