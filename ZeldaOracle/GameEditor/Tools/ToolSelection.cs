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
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles.EventTiles;

namespace ZeldaEditor.Tools {

	public class ToolSelection : EditorTool {

		private static readonly Cursor SelectionCursor = LoadCursor("Selection");
		private static readonly Cursor DraggingCursor = Cursors.SizeAll;

		private const Keys KEYMOD_DUPLICATE = Keys.Control;
		private const Keys KEYMOD_ROOM_MODE = Keys.Shift;

		private Point2I dragBeginTileCoord;
		private bool isCreatingSelectionBox;
		private bool isMovingSelectionBox;
		private Point2I dragBeginPoint;
		private Point2I selectionBoxBeginPoint;

		private TileGrid clipboard;

		private SelectionModes mode;
		private Point2I start;
		private Rectangle2I selectionGridArea;
		private TileGrid selectionGrid;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolSelection() {
			name = "Selection Tool";
			clipboard = null;
			mode = SelectionModes.Move;
		}


		//-----------------------------------------------------------------------------
		// Selection
		//-----------------------------------------------------------------------------

		public override void Cut() {
			if (CanCopyCut) {
				if (selectionGrid == null) {
					selectionGrid = Level.CreateTileGrid(selectionGridArea, CreateTileGridMode.Twin);
					clipboard = selectionGrid.Duplicate();
				}
				if (mode != SelectionModes.Duplicate && mode != SelectionModes.Paste) {
					mode = SelectionModes.Cut;
					Finish();
				}
				ClearSelection();
				UpdateCommands();
			}
		}

		public override void Copy() {
			if (CanCopyCut) {
				if (selectionGrid == null) {
					selectionGrid = Level.CreateTileGrid(selectionGridArea, CreateTileGridMode.Twin);
				}
				clipboard = selectionGrid.Duplicate();
				UpdateCommands();
			}
		}

		public override void Paste() {
			if (clipboard != null) {
				Finish();
				IsDrawing = true;
				selectionGrid = clipboard.Duplicate();
				selectionGridArea = new Rectangle2I(selectionGrid.Size);
				mode = SelectionModes.Paste;
				UpdateSelectionBox();
				UpdateCommands();
			}
		}

		public override void Delete() {
			if (CanDeleteDeselect) {
				if (mode != SelectionModes.Duplicate && mode != SelectionModes.Paste) {
					mode = SelectionModes.Delete;
					if (selectionGrid == null) {
						selectionGrid = Level.CreateTileGrid(selectionGridArea);
					}
					Finish();
				}
				ClearSelection();
				UpdateCommands();
			}
		}

		public override void SelectAll() {
			Finish();
			IsDrawing = true;

			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;
			
			selectionGridArea = new Rectangle2I(Point2I.Zero, Level.Dimensions * Level.RoomSize);
			start = Point2I.Zero;
			UpdateSelectionBox();
			UpdateCommands();
		}

		public override void Deselect() {
			if (CanDeleteDeselect) {
				Finish();
				ClearSelection();
				UpdateCommands();
			}
		}

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		protected override void OnCancel() {
			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;
			ClearSelection();
		}

		public override bool CancelCountsAsUndo {
			get { return !selectionGridArea.IsEmpty && !isCreatingSelectionBox; }
		}

		protected override void OnInitialize() {
			MouseCursor = SelectionCursor;
		}

		protected void Finish() {
			if (selectionGrid != null && !isCreatingSelectionBox) {
				Point2I end = selectionGridArea.Point;
				if (start != end || (mode != SelectionModes.Move && mode != SelectionModes.Duplicate)) {
					EditorAction undo = null;
					switch (mode) {
					case SelectionModes.Move:
						undo = ActionSelection.CreateMoveAction(editorControl.Level, start, end, selectionGrid,
							editorControl.Level.CreateTileGrid(selectionGridArea, CreateTileGridMode.Twin));
						break;
					case SelectionModes.Delete:
					case SelectionModes.Cut:
						undo = ActionSelection.CreateDeleteAction(editorControl.Level, start, selectionGrid,
							mode == SelectionModes.Cut);
						break;
					case SelectionModes.Duplicate:
					case SelectionModes.Paste:
						undo = ActionSelection.CreateDuplicateAction(editorControl.Level, end, selectionGrid,
							editorControl.Level.CreateTileGrid(selectionGridArea, CreateTileGridMode.Twin),
							mode == SelectionModes.Paste);
						break;
					}
					editorControl.PushAction(undo, ActionExecution.Execute);
				}
				else {
					Level.PlaceTileGrid(selectionGrid, (LevelTileCoord)selectionGridArea.Point);
				}
				selectionGrid = null;
			}
			IsDrawing = false;
		}

		protected override void OnBegin() {
			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;
		}

		protected override void OnEnd() {
			Finish();
			ClearSelection();
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			if (e.Button == MouseButtons.Right) {
				Finish();
				ClearSelection();
			}
		}
		protected override void OnMouseDragBegin(MouseEventArgs e) {

			Point2I mousePos = e.MousePos();
			Point2I levelTileCoord = LevelDisplay.SampleLevelTileCoordinates(mousePos);
			Point2I point = LevelDisplay.SampleLevelPixelPosition(mousePos);

			if (isCreatingSelectionBox || isMovingSelectionBox)
				return;

			// Draw a new selecion box.
			if (e.Button == MouseButtons.Left) {

				if (selectionGridArea.Contains(levelTileCoord)) {
					// Begin moving the selection box.
					isMovingSelectionBox    = true;
					dragBeginPoint          = point;
					dragBeginTileCoord      = levelTileCoord;
					selectionBoxBeginPoint  = selectionGridArea.Point;

					// Duplicate selection if holding Ctrl.
					if (System.Windows.Forms.Control.ModifierKeys.HasFlag(KEYMOD_DUPLICATE)) {
						if (selectionGrid != null) {
							TileGrid newSelectionGrid = selectionGrid.Duplicate();
							Finish();
							selectionGrid = newSelectionGrid;
						}
						else {
							selectionGrid = Level.CreateTileGrid(selectionGridArea, CreateTileGridMode.Duplicate);
						}
						mode = SelectionModes.Duplicate;
						start = selectionGridArea.Point;
					}
					else if (selectionGrid == null) {
						selectionGrid = Level.CreateTileGrid(selectionGridArea, CreateTileGridMode.Twin);
						mode = SelectionModes.Move;
						start = selectionGridArea.Point;
					}
				}
				else {
					Finish();
					IsDrawing = true;

					// Create a new selection box.
					isCreatingSelectionBox  = true;
					dragBeginPoint          = point;
					dragBeginTileCoord      = levelTileCoord;


					if (System.Windows.Forms.Control.ModifierKeys.HasFlag(KEYMOD_ROOM_MODE)) {
						selectionGridArea = new Rectangle2I(Level.GetRoomLocation(
							(LevelTileCoord)dragBeginTileCoord), Point2I.One);
						Rectangle2I levelBounds = new Rectangle2I(Point2I.Zero, Level.Dimensions);
						selectionGridArea = Rectangle2I.Intersect(selectionGridArea, levelBounds);
						selectionGridArea.Point *= Level.RoomSize;
						selectionGridArea.Size *= Level.RoomSize;
					}
					else {
						selectionGridArea = new Rectangle2I(dragBeginTileCoord, Point2I.One);
						Rectangle2I levelBounds = new Rectangle2I(Point2I.Zero, Level.RoomSize * Level.Dimensions);
						selectionGridArea = Rectangle2I.Intersect(selectionGridArea, levelBounds);
					}

					UpdateSelectionBox();
				}
			}
		}

		protected override void OnMouseDragEnd(MouseEventArgs e) {
			if (e.Button == MouseButtons.Left && isCreatingSelectionBox) {
				isCreatingSelectionBox = false;
				mode = SelectionModes.Move;
				start = selectionGridArea.Point;
			}
			else if (e.Button == MouseButtons.Left && isMovingSelectionBox) {
				isMovingSelectionBox = false;
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			Point2I mousePos = e.MousePos();
			Point2I pointInLevel = LevelDisplay.SampleLevelPixelPosition(mousePos);

			// Update selection box.
			if (e.Button == MouseButtons.Left && isCreatingSelectionBox) {
				Point2I tileCoord = LevelDisplay.SampleLevelTileCoordinates(mousePos);
				Level level = EditorControl.Level;

				if (System.Windows.Forms.Control.ModifierKeys.HasFlag(KEYMOD_ROOM_MODE)) {
					Point2I roomCoord1 = level.GetRoomLocation((LevelTileCoord) dragBeginTileCoord);
					Point2I roomCoord2 = level.GetRoomLocation((LevelTileCoord) tileCoord);
					Point2I roomCoordMin = GMath.Min(roomCoord1, roomCoord2);
					Point2I roomCoordMax = GMath.Max(roomCoord1, roomCoord2);

					Rectangle2I levelDimensions = new Rectangle2I(Point2I.Zero, level.Dimensions);
					selectionGridArea = new Rectangle2I(roomCoordMin, roomCoordMax - roomCoordMin + Point2I.One);
					selectionGridArea = Rectangle2I.Intersect(selectionGridArea, levelDimensions);
					selectionGridArea.Point *= level.RoomSize;
					selectionGridArea.Size *= level.RoomSize;
				}
				else {
					Point2I minCoord  = GMath.Min(dragBeginTileCoord, tileCoord);
					Point2I maxCoord  = GMath.Max(dragBeginTileCoord, tileCoord);

					Rectangle2I levelBounds = new Rectangle2I(Point2I.Zero, level.RoomSize * level.Dimensions);
					selectionGridArea = new Rectangle2I(minCoord, maxCoord - minCoord + Point2I.One);
					selectionGridArea = Rectangle2I.Intersect(selectionGridArea, levelBounds);
				}

				UpdateSelectionBox();
			}
			else if (e.Button == MouseButtons.Left && isMovingSelectionBox) {
				Point2I moveAmount;

				if (System.Windows.Forms.Control.ModifierKeys.HasFlag(KEYMOD_ROOM_MODE)) {
					moveAmount = pointInLevel - dragBeginPoint;
					moveAmount = (Point2I)GMath.Round((Vector2F)moveAmount / (editorControl.Level.RoomSize * GameSettings.TILE_SIZE));
					moveAmount *= editorControl.Level.RoomSize;
				}
				else {
					moveAmount = pointInLevel - dragBeginPoint;
					moveAmount = (Point2I)GMath.Round((Vector2F)moveAmount / GameSettings.TILE_SIZE);

				}

				selectionGridArea.Point = selectionBoxBeginPoint + moveAmount;

				UpdateSelectionBox();
			}
			else if (e.Button == MouseButtons.Right) {
				Finish();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			// Check if mouse is over selection.
			Point2I point = new Point2I(e.X, e.Y);
			Point2I tileCoord = LevelDisplay.SampleLevelTileCoordinates(point);

			if (!isCreatingSelectionBox && selectionGridArea.Contains(tileCoord)) {
				MouseCursor = DraggingCursor;
				editorControl.HighlightMouseTile = false;
			}
			else {
				MouseCursor = SelectionCursor;
				editorControl.HighlightMouseTile = true;
			}

			base.OnMouseMove(e);
		}


		public override bool CanCopyCut {
			get { return !selectionGridArea.IsEmpty && !isCreatingSelectionBox; }
		}
		public override bool CanDeleteDeselect {
			get { return !selectionGridArea.IsEmpty && !isCreatingSelectionBox; }
		}

		public bool CanPaste {
			get { return clipboard != null; }
		}

		public TileGrid Clipboard {
			get { return clipboard; }
			set { clipboard = value; }
		}

		private void UpdateSelectionBox() {
			LevelDisplay.SetSelectionBox(selectionGridArea * GameSettings.TILE_SIZE);
		}
		private void ClearSelection() {
			IsDrawing = false;
			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;
			selectionGridArea = Rectangle2I.Zero;
			selectionGrid = null;
			LevelDisplay.ClearSelectionBox();
		}

		//-----------------------------------------------------------------------------
		// Virtual drawing
		//-----------------------------------------------------------------------------

		public override bool DrawHideTile(TileDataInstance tile, Room room, Point2I levelCoord, int layer) {
			if (selectionGrid != null) {
				if (mode != SelectionModes.Duplicate && mode != SelectionModes.Paste &&
					new Rectangle2I(start, selectionGrid.Size).Contains(levelCoord))
					return true;
				return selectionGridArea.Contains(levelCoord);
			}
			return false;
		}

		public override void DrawTile(Graphics2D g, Room room, Point2I position, Point2I levelCoord, int layer) {
			if (selectionGrid != null && selectionGridArea.Contains(levelCoord)) {
				TileDataInstance tile = selectionGrid.GetTileIfAtLocation(levelCoord - selectionGridArea.Point, layer);
				if (tile != null) {
					LevelDisplay.DrawTile(g, room, tile, position, LevelDisplay.NormalColor);
				}
			}
		}
		public override bool DrawHideEventTile(EventTileDataInstance eventTile, Room room, Point2I levelPosition) {
			if (selectionGrid != null) {
				if (mode != SelectionModes.Duplicate && mode != SelectionModes.Paste &&
					(new Rectangle2I(start, selectionGrid.Size) * GameSettings.TILE_SIZE).Contains(levelPosition))
					return true;
				return (selectionGridArea * GameSettings.TILE_SIZE).Contains(levelPosition);
			}
			return false;
		}

		public override void DrawEventTiles(Graphics2D g) {
			if (selectionGrid != null) {
				Point2I startLevelPixel = selectionGridArea.Point * GameSettings.TILE_SIZE;
				foreach (var eventPair in selectionGrid.GetEventTilePositions()) {
					Point2I position = LevelDisplay.GetLevelPixelDrawPosition(startLevelPixel + eventPair.Key);
					Room room = LevelDisplay.SampleRoom(position);
					if (room != null)
						LevelDisplay.DrawEventTile(g, room, eventPair.Value, position, LevelDisplay.NormalColor);
				}
			}
		}
	}
}
