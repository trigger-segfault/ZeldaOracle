using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Worlds.Editing;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game;
using ZeldaEditor.Undo;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Graphics;
using System.Windows.Input;
using Cursor = System.Windows.Forms.Cursor;

namespace ZeldaEditor.Tools {
	public class ToolSquare : EditorTool {
		private static readonly Cursor SquareCursor = LoadCursor("Square");

		private Point2I dragBeginCoord;
		
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

		protected override void OnBegin(ToolEventArgs e) {
			ShowCursor = true;
			CursorPosition = e.SnappedPosition;
			CursorTileSize = Point2I.One;
		}

		protected override void OnEnd(ToolEventArgs e) {
			LevelDisplay.ClearSelectionBox();
		}

		protected override void OnCancel(ToolEventArgs e) {
			LevelDisplay.ClearSelectionBox();
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseMove(ToolEventArgs e) {
			CursorPosition = e.SnappedPosition;
		}

		protected override void OnMouseDown(ToolEventArgs e) {
			if (e.IsOpposite(DragButton)) {
				Cancel();
			}
		}

		protected override void OnMouseDragBegin(ToolEventArgs e) {
			// Draw a new selecion box.
			if (e.IsLeftOrRight && !ActionMode && IsTileSingle) {
				IsDrawing = true;

				if (DragButton == MouseButton.Left)
					drawTile = CreateDrawTile();
				else
					drawTile = null;

				dragBeginCoord = e.LevelCoord;
				square = new Rectangle2I(dragBeginCoord, Point2I.One);
				LevelDisplay.SetSelectionBox(Level.LevelCoordToPosition(square));
			}
		}

		protected override void OnMouseDragEnd(ToolEventArgs e) {
			if (IsDrawing) {
				IsDrawing = false;
				square = Rectangle2I.FromEndPointsOne(dragBeginCoord, e.LevelCoord);
				square = Rectangle2I.Intersect(square, Level.TileBounds);

				TileData tileData = EditorControl.SelectedTileData as TileData;
				if (DragButton == MouseButton.Right)
					tileData = null;
				ActionSquare action = new ActionSquare(Level, Layer, square, tileData);
				for (int x = square.Left; x < square.Right; x++) {
					for (int y = square.Top; y < square.Bottom; y++) {
						Point2I levelCoord = new Point2I(x, y);
						action.AddOverwrittenTile(Level.GetTileAt(levelCoord, Layer));
					}
				}
				EditorControl.PushAction(action, ActionExecution.Execute);
				LevelDisplay.ClearSelectionBox();
			}
		}

		protected override void OnMouseDragMove(ToolEventArgs e) {
			// Update selection box.
			if (IsDrawing) {
				square = Rectangle2I.FromEndPointsOne(dragBeginCoord, e.LevelCoord);
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
			if (!ActionMode && layer == Layer && IsTileSingle) {
				if (!IsDrawing && levelCoord == CursorLevelCoord) {
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
