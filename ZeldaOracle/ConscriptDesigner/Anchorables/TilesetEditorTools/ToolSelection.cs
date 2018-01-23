using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Key = System.Windows.Input.Key;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Worlds;

namespace ConscriptDesigner.Anchorables.TilesetEditorTools {
	public class ToolSelection : TilesetEditorTool {
		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private static readonly Cursor SelectionCursor = LoadCursor("Selection");
		private static readonly Cursor DraggingCursor = Cursors.SizeAll;

		private const Keys DuplicateModifier = Keys.Control;
		private const Keys RoomModeModifier = Keys.Shift;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------
		
		private Point2I dragBeginTileCoord;
		private bool isCreatingSelectionBox;
		private bool isMovingSelectionBox;
		private Point2I selectionBoxBeginPoint;

		private BaseTileData[,] clipboard;
		
		private Point2I start;
		private Rectangle2I selectionGridRect;
		private BaseTileData[,] selectionGrid;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolSelection() : base("Selection Tool", Key.S) {
			clipboard = null;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Clipboard Methods
		//-----------------------------------------------------------------------------

		public override void Cut() {
			if (CanCopyCut) {
				clipboard = selectionGrid;
				selectionGrid = null;
				IsDrawing = false;
				ClearSelection();
				UpdateCommands();
				TilesetEditor.Invalidate();
			}
		}

		public override void Copy() {
			if (CanCopyCut) {
				clipboard = DuplicateGrid(selectionGrid);
				UpdateCommands();
			}
		}

		public override void Paste() {
			if (clipboard != null) {
				Finish();
				IsDrawing = true;
				selectionGrid = DuplicateGrid(clipboard);
				selectionGridRect = new Rectangle2I(GridStart, GridSize(selectionGrid));
				UpdateCommands();
				TilesetEditor.Invalidate();
			}
		}

		public override void Delete() {
			if (CanDeleteDeselect) {
				ClearSelection();
				UpdateCommands();
				TilesetEditor.Invalidate();
			}
		}

		public override void SelectAll() {
			Finish();
			IsDrawing = true;

			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;

			selectionGridRect = new Rectangle2I(Point2I.Zero, Tileset.Dimensions);
			start = Point2I.Zero;
			CreateGrid(selectionGridRect);
			UpdateCommands();
			TilesetEditor.Invalidate();
		}

		public override void Deselect() {
			if (CanDeleteDeselect) {
				Finish();
				ClearSelection();
				UpdateCommands();
				TilesetEditor.Invalidate();
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden State Methods
		//-----------------------------------------------------------------------------

		protected override void OnInitialize() {
			MouseCursor = SelectionCursor;
		}

		protected override void OnBegin() {
			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;
		}

		protected override void OnEnd() {
			Finish();
			ClearSelection();
		}

		protected override void OnCancel() {
			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;
			Finish();
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseDown(MouseEventArgs e) {
			if (e.Button == MouseButtons.Right) {
				Finish();
				ClearSelection();
				TilesetEditor.Invalidate();
			}
		}

		protected override void OnMouseDragBegin(MouseEventArgs e) {
			
			Point2I mouseTile = MouseTile(e);

			if (isCreatingSelectionBox || isMovingSelectionBox)
				return;

			// Draw a new selecion box.
			if (e.Button == MouseButtons.Left) {

				if (selectionGridRect.Contains(mouseTile)) {
					// Begin moving the selection box.
					isMovingSelectionBox    = true;
					dragBeginTileCoord      = mouseTile;
					selectionBoxBeginPoint  = selectionGridRect.Point;

					// Duplicate selection if holding Ctrl.
					if (System.Windows.Forms.Control.ModifierKeys.HasFlag(DuplicateModifier)) {
						var newSelectionGrid = DuplicateGrid(selectionGrid);
						var newSelectionGridRect = selectionGridRect;
						Finish();
						selectionGrid = newSelectionGrid;
						start = selectionGridRect.Point;
						selectionGridRect = newSelectionGridRect;
					}
				}
				else {
					Finish();
					IsDrawing = true;

					// Create a new selection box.
					isCreatingSelectionBox  = true;
					dragBeginTileCoord      = mouseTile;
					
					selectionGridRect = new Rectangle2I(dragBeginTileCoord, Point2I.One);
					Rectangle2I tilesetBounds = new Rectangle2I(Point2I.Zero, Tileset.Dimensions);
					selectionGridRect = Rectangle2I.Intersect(selectionGridRect, tilesetBounds);
				}
			}
		}

		protected override void OnMouseDragEnd(MouseEventArgs e) {
			if (e.Button == MouseButtons.Left && isCreatingSelectionBox) {
				isCreatingSelectionBox = false;
				start = selectionGridRect.Point;

				selectionGrid = CreateGrid(selectionGridRect);
				OnMouseMove(e);
				TilesetEditor.Invalidate();
			}
			else if (e.Button == MouseButtons.Left && isMovingSelectionBox) {
				isMovingSelectionBox = false;
				TilesetEditor.Invalidate();
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			Point2I mouseTile = MouseTile(e);

			// Update selection box.
			if (e.Button == MouseButtons.Left && isCreatingSelectionBox) {
				
				Point2I minCoord  = GMath.Min(dragBeginTileCoord, mouseTile);
				Point2I maxCoord  = GMath.Max(dragBeginTileCoord, mouseTile);

				Rectangle2I tilesetBounds = new Rectangle2I(Point2I.Zero, Tileset.Dimensions);
				selectionGridRect = new Rectangle2I(minCoord, maxCoord - minCoord + Point2I.One);
				selectionGridRect = Rectangle2I.Intersect(selectionGridRect, tilesetBounds);
			}
			else if (e.Button == MouseButtons.Left && isMovingSelectionBox) {
				Point2I moveAmount = mouseTile - dragBeginTileCoord;

				selectionGridRect.Point = selectionBoxBeginPoint + moveAmount;
			}
			else if (e.Button == MouseButtons.Right) {
				Finish();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			// Check if mouse is over selection.
			Point2I mouseTile = MouseTile(e);

			if ((!isCreatingSelectionBox && selectionGridRect.Contains(mouseTile)) || isMovingSelectionBox) {
				MouseCursor = DraggingCursor;
			}
			else {
				MouseCursor = SelectionCursor;
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------
		
		public override bool CanCopyCut {
			get { return !selectionGridRect.IsEmpty && !isCreatingSelectionBox; }
		}

		public override bool CanDeleteDeselect {
			get { return !selectionGridRect.IsEmpty && !isCreatingSelectionBox; }
		}


		//-----------------------------------------------------------------------------
		// Overridden Drawing Methods
		//-----------------------------------------------------------------------------

		public override void DrawTiles(Graphics2D g, Zone zone) {
			if (selectionGrid != null) {
				if (TilesetEditor.Overwrite) {
					g.FillRectangle(SelectionGridArea, Color.White);
				}
				for (int x = 0; x < GridSize(selectionGrid).X; x++) {
					for (int y = 0; y < GridSize(selectionGrid).Y; y++) {
						Point2I point = new Point2I(x, y) + selectionGridRect.Point;
						BaseTileData tile = selectionGrid[x, y];
						if (tile != null) {
							DrawTile(g, tile, point, zone, Color.White);
						}
					}
				}
			}
			if (!selectionGridRect.IsEmpty)
				DrawSelectionBox(g, selectionGridRect);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		protected void Finish() {
			if (selectionGrid != null && !isCreatingSelectionBox) {
				for (int x = GridSize(selectionGrid).X - 1; x >= 0; x--) {
					for (int y = GridSize(selectionGrid).Y - 1; y >= 0; y--) {
						Point2I point = new Point2I(x, y) + selectionGridRect.Point;
						if (point >= Point2I.Zero && point < Tileset.Dimensions) {
							Tileset.SetTileData(point, selectionGrid[x, y]);
						}
					}
				}
				TilesetEditor.Modfied();
			}
			ClearSelection();
			IsDrawing = false;
		}

		private void UpdateSelectionBox() {
			//LevelDisplay.SetSelectionBox(selectionGridArea * GameSettings.TILE_SIZE);
		}

		private void ClearSelection() {
			IsDrawing = false;
			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;
			selectionGridRect = Rectangle2I.Zero;
			selectionGrid = null;
		}

		private BaseTileData[,] CreateGrid(Rectangle2I selection) {
			BaseTileData[,] result = new BaseTileData[selection.Width, selection.Height];
			for (int x = 0; x < selection.Width; x++) {
				for (int y = 0; y < selection.Height; y++) {
					result[x, y] = Tileset.GetTileDataAtOrigin(selection.Point + new Point2I(x, y));
					if (result[x, y] != null)
						Tileset.RemoveTileData(selection.Point + new Point2I(x, y));
				}
			}
			TilesetEditor.Modfied();
			return result;
		}

		private BaseTileData[,] DuplicateGrid(BaseTileData[,] grid) {
			BaseTileData[,] result = new BaseTileData[grid.GetLength(0), grid.GetLength(1)];
			for (int x = 0; x < grid.GetLength(0); x++) {
				for (int y = 0; y < grid.GetLength(1); y++) {
					result[x, y] = grid[x, y];
				}
			}
			return result;
		}

		private Point2I GridSize(BaseTileData[,] grid) {
			return new Point2I(grid.GetLength(0), grid.GetLength(1));
		}


		//-----------------------------------------------------------------------------
		// Tool-Specific Properties
		//-----------------------------------------------------------------------------

		private Rectangle2I SelectionGridArea {
			get {
				Rectangle2I area = selectionGridRect;
				area.Point = (GameSettings.TILE_SIZE + 1) * area.Point;
				area.Size = (GameSettings.TILE_SIZE + 1) * area.Size + 1;
				return area;
			}
		}

		private Point2I GridStart {
			get { return (TilesetEditor.UnscaledScrollPosition - 1 + GameSettings.TILE_SIZE - 1) / (GameSettings.TILE_SIZE + 1); }
		}

		public BaseTileData[,] Clipboard {
			get { return clipboard; }
			set { clipboard = value; }
		}

		public bool CanPaste {
			get { return clipboard != null; }
		}

		public bool HasSelection {
			get { return !selectionGridRect.IsEmpty; }
		}
	}
}
