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
		private Rectangle2I gridArea;
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
			LevelDisplay.PickupSelectionGrid();
			if (LevelDisplay.SelectionGrid != null) {
				if (mode != SelectionModes.Duplicate && mode != SelectionModes.Paste) {
					mode = SelectionModes.Delete;
					CreateUndoAction(true);
				}
				clipboard = LevelDisplay.SelectionGrid;
				LevelDisplay.SelectionGrid = null;
				LevelDisplay.DeselectSelectionGrid();
				UpdateCommands();
			}
		}

		public override void Copy() {
			LevelDisplay.PickupSelectionGrid();
			if (LevelDisplay.SelectionGrid != null) {
				clipboard = LevelDisplay.SelectionGrid.Duplicate();
				UpdateCommands();
			}
		}

		public override void Paste() {
			if (clipboard != null) {
				CreateUndoAction(true);
				LevelDisplay.DeselectSelectionGrid();
				LevelDisplay.SetSelectionGrid(
					clipboard.Duplicate(), Point2I.Zero, EditorControl.Level);
				UpdateCommands();
				mode = SelectionModes.Paste;
			}
		}

		public override void Delete() {
			LevelDisplay.PickupSelectionGrid();
			if (mode != SelectionModes.Duplicate && mode != SelectionModes.Paste) {
				mode = SelectionModes.Delete;
				CreateUndoAction();
			}
			LevelDisplay.DeleteSelectionGrid();
			UpdateCommands();
		}

		public override void SelectAll() {
			CreateUndoAction();

			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;

			Level level = EditorControl.Level;
			Rectangle2I area = new Rectangle2I(Point2I.Zero, level.Dimensions * level.RoomSize);
			LevelDisplay.SetSelectionGridArea(area, level);
			start = Point2I.Zero;
			UpdateCommands();
		}

		public override void Deselect() {
			CreateUndoAction();

			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;
			LevelDisplay.DeselectSelectionGrid();
			UpdateCommands();
		}

		private void CreateUndoAction(bool isClipboard = false) {
			if (LevelDisplay.SelectionGrid != null) {
				Point2I end = LevelDisplay.SelectionGridArea.Point;
				if (start != end || (mode != SelectionModes.Move && mode != SelectionModes.Duplicate)) {
					EditorAction undo = null;
					TileGrid tileGrid = LevelDisplay.SelectionGrid;
					switch (mode) {
					case SelectionModes.Move:
						undo = ActionSelection.CreateMoveAction(editorControl.Level, start, end, tileGrid,
							editorControl.Level.CreateTileGrid(new Rectangle2I(end, tileGrid.Size), true));
						break;
					case SelectionModes.Delete:
						undo = ActionSelection.CreateDeleteAction(editorControl.Level, start, tileGrid, isClipboard);
						break;
					case SelectionModes.Duplicate:
					case SelectionModes.Paste:
						undo = ActionSelection.CreateDuplicateAction(editorControl.Level, end, tileGrid,
							editorControl.Level.CreateTileGrid(new Rectangle2I(end, tileGrid.Size), true), isClipboard);
						break;
					}
					editorControl.PushAction(undo, ActionExecution.None);
				}
			}
		}

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		protected override void OnCancel() {
			if (isCreatingSelectionBox) {
				isCreatingSelectionBox = false;
			}
			else {
				isMovingSelectionBox = false;
				if (!LevelDisplay.SelectionGridArea.IsEmpty) {
					if (LevelDisplay.SelectionGrid != null) {
						if (mode == SelectionModes.Move) {
							LevelDisplay.MoveSelectionGridArea(start);
							LevelDisplay.DeselectSelectionGrid();
						}
						else {
							LevelDisplay.DeleteSelectionGrid();
						}
					}
					else {
						LevelDisplay.DeselectSelectionGrid();
					}
				}
			}
		}

		public override bool CancelCountsAsUndo {
			get { return !LevelDisplay.SelectionGridArea.IsEmpty; }
		}

		protected override void OnInitialize() {
			MouseCursor = SelectionCursor;
		}

		protected override void OnBegin() {
			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;
		}

		protected override void OnEnd() {
			LevelDisplay.DeselectSelectionGrid();
		}

		protected override void OnMouseDragBegin(MouseEventArgs e) {

			Point2I mousePos = new Point2I(e.X, e.Y);
			Point2I tileCoord = LevelDisplay.SampleLevelTileCoordinates(mousePos);
			Point2I point = LevelDisplay.SampleLevelPixelPosition(mousePos);

			if (isCreatingSelectionBox || isMovingSelectionBox)
				return;

			// Draw a new selecion box.
			if (e.Button == MouseButtons.Left) {

				if (EditorControl.Level == LevelDisplay.SelectionGridLevel &&
					LevelDisplay.SelectionBox.Contains(point)) {
					// Begin moving the selection box.
					isMovingSelectionBox    = true;
					dragBeginPoint          = point;
					dragBeginTileCoord      = tileCoord;
					selectionBoxBeginPoint  = LevelDisplay.SelectionGridArea.Point;
					//LevelDisplayControl.PickupSelectionGrid();

					// Duplicate selection if holding Ctrl.
					if (System.Windows.Forms.Control.ModifierKeys.HasFlag(KEYMOD_DUPLICATE)) {
						CreateUndoAction();
						LevelDisplay.DuplicateSelectionGrid();
						mode = SelectionModes.Duplicate;
						start = LevelDisplay.SelectionGridArea.Point;
					}
					else {
						mode = SelectionModes.Move;
					}
				}
				else {
					CreateUndoAction();
					LevelDisplay.PlaceSelectionGrid();

					// Create a new selection box.
					isCreatingSelectionBox  = true;
					dragBeginPoint          = point;
					dragBeginTileCoord      = tileCoord;

					Rectangle2I selectionBox;
					Level level = EditorControl.Level;

					if (System.Windows.Forms.Control.ModifierKeys.HasFlag(KEYMOD_ROOM_MODE)) {
						selectionBox = new Rectangle2I(level.GetRoomLocation(
							(LevelTileCoord)dragBeginTileCoord), Point2I.One);
						Rectangle2I levelBounds = new Rectangle2I(Point2I.Zero, level.Dimensions);
						selectionBox = Rectangle2I.Intersect(selectionBox, levelBounds);
						selectionBox.Point *= level.RoomSize;
						selectionBox.Size *= level.RoomSize;
					}
					else {
						selectionBox = new Rectangle2I(dragBeginTileCoord, Point2I.One);
						Rectangle2I levelBounds = new Rectangle2I(Point2I.Zero, level.RoomSize * level.Dimensions);
						selectionBox = Rectangle2I.Intersect(selectionBox, levelBounds);
					}

					LevelDisplay.SetSelectionGridArea(selectionBox, level);
				}
			}
		}

		protected override void OnMouseDragEnd(MouseEventArgs e) {
			if (e.Button == MouseButtons.Left && isCreatingSelectionBox) {
				isCreatingSelectionBox = false;
				mode = SelectionModes.Move;
				start = LevelDisplay.SelectionGridArea.Point;
			}
			else if (e.Button == MouseButtons.Left && isMovingSelectionBox) {
				isMovingSelectionBox = false;
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			Point2I mousePos = new Point2I(e.X, e.Y);
			Point2I pointInLevel = LevelDisplay.SampleLevelPixelPosition(mousePos);

			// Update selection box.
			if (e.Button == MouseButtons.Left && isCreatingSelectionBox) {
				Point2I tileCoord = LevelDisplay.SampleLevelTileCoordinates(mousePos);
				Level level = EditorControl.Level;
				Rectangle2I selectionBox;

				if (System.Windows.Forms.Control.ModifierKeys.HasFlag(KEYMOD_ROOM_MODE)) {
					Point2I roomCoord1 = level.GetRoomLocation((LevelTileCoord) dragBeginTileCoord);
					Point2I roomCoord2 = level.GetRoomLocation((LevelTileCoord) tileCoord);
					Point2I roomCoordMin = GMath.Min(roomCoord1, roomCoord2);
					Point2I roomCoordMax = GMath.Max(roomCoord1, roomCoord2);

					Rectangle2I levelDimensions = new Rectangle2I(Point2I.Zero, level.Dimensions);
					selectionBox = new Rectangle2I(roomCoordMin, roomCoordMax - roomCoordMin + Point2I.One);
					selectionBox = Rectangle2I.Intersect(selectionBox, levelDimensions);
					selectionBox.Point *= level.RoomSize;
					selectionBox.Size *= level.RoomSize;
				}
				else {
					Point2I minCoord  = GMath.Min(dragBeginTileCoord, tileCoord);
					Point2I maxCoord  = GMath.Max(dragBeginTileCoord, tileCoord);

					Rectangle2I levelBounds = new Rectangle2I(Point2I.Zero, level.RoomSize * level.Dimensions);
					selectionBox = new Rectangle2I(minCoord, maxCoord - minCoord + Point2I.One);
					//selectionBox = Rectangle2I.Intersect(selectionBox, levelBounds);
				}

				LevelDisplay.SetSelectionGridArea(selectionBox, level);
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

				Point2I selectionBoxPoint = selectionBoxBeginPoint + moveAmount;
				LevelDisplay.MoveSelectionGridArea(selectionBoxPoint);
			}
			else if (e.Button == MouseButtons.Right && !isCreatingSelectionBox && !isMovingSelectionBox) {
				LevelDisplay.DeselectSelectionGrid();
			}
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			// Check if mouse is over selection.
			Point2I point = new Point2I(e.X, e.Y);
			Point2I tileCoord = LevelDisplay.SampleLevelTileCoordinates(point) * GameSettings.TILE_SIZE;

			if (!isCreatingSelectionBox &&
				EditorControl.Level == LevelDisplay.SelectionGridLevel &&
				LevelDisplay.SelectionBox.Contains(tileCoord)) {
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
			get { return !LevelDisplay.SelectionGridArea.IsEmpty; }
		}
		public override bool CanDeleteDeselect {
			get { return !LevelDisplay.SelectionGridArea.IsEmpty; }
		}

		public bool CanPaste {
			get { return clipboard != null; }
		}

		public TileGrid Clipboard {
			get { return clipboard; }
			set { clipboard = value; }
		}
	}
}
