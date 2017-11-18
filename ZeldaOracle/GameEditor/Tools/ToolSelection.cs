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

namespace ZeldaEditor.Tools {
	public class ToolSelection : EditorTool {

		private const Keys KEYMOD_DUPLICATE = Keys.Control;
		private const Keys KEYMOD_ROOM_MODE = Keys.Alt;

		private Point2I dragBeginTileCoord;
		private bool isCreatingSelectionBox;
		private bool isMovingSelectionBox;
		private Point2I dragBeginPoint;
		private Point2I selectionBoxBeginPoint;

		private TileGrid clipboard;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolSelection() {
			name = "Selection Tool";
			clipboard = null;
		}

		
		//-----------------------------------------------------------------------------
		// Selection
		//-----------------------------------------------------------------------------

		public override void Cut() {
			LevelDisplay.PickupSelectionGrid();
			if (LevelDisplay.SelectionGrid != null) {
				clipboard = LevelDisplay.SelectionGrid;
				LevelDisplay.SelectionGrid = null;
				LevelDisplay.DeselectSelectionGrid();
			}
		}
		
		public override void Copy() {
			LevelDisplay.PickupSelectionGrid();
			if (LevelDisplay.SelectionGrid != null)
				clipboard = LevelDisplay.SelectionGrid.Duplicate();
		}
		
		public override void Paste() {
			if (clipboard != null) {
				LevelDisplay.DeselectSelectionGrid();
				LevelDisplay.SetSelectionGrid(
					clipboard.Duplicate(), Point2I.Zero, EditorControl.Level);
			}
		}
		
		public override void Delete() {
			LevelDisplay.DeleteSelectionGrid();
		}

		public override void SelectAll() {
			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;

			Level level = EditorControl.Level;
			Rectangle2I area = new Rectangle2I(Point2I.Zero, level.Dimensions * level.RoomSize);
			LevelDisplay.SetSelectionGridArea(area, level);
		}

		public override void Deselect() {
			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;
			LevelDisplay.DeselectSelectionGrid();
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {

		}

		public override void OnBegin() {
			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;
		}

		public override void OnEnd() {
			LevelDisplay.PlaceSelectionGrid();
		}

		public override void OnMouseDragBegin(MouseEventArgs e) {

			Point2I mousePos = new Point2I(e.X, e.Y);
			Point2I tileCoord = LevelDisplay.SampleLevelTileCoordinates(mousePos);
			Point2I point = LevelDisplay.SampleLevelPixelPosition(mousePos);

			if (isCreatingSelectionBox || isMovingSelectionBox)
				return;

			// Draw a new selecion box.
			if (e.Button == MouseButtons.Left) {

				if (EditorControl.Level == LevelDisplay.SelectionGridLevel &&
					LevelDisplay.SelectionBox.Contains(point))
				{
					// Begin moving the selection box.
					isMovingSelectionBox	= true;
					dragBeginPoint			= point;
					dragBeginTileCoord		= tileCoord;
					selectionBoxBeginPoint	= LevelDisplay.SelectionGridArea.Point;
					//LevelDisplayControl.PickupSelectionGrid();

					// Duplicate selection if holding Ctrl.
					if (System.Windows.Forms.Control.ModifierKeys.HasFlag(KEYMOD_DUPLICATE)) {
						LevelDisplay.DuplicateSelectionGrid();
					}
				}
				else {
					LevelDisplay.PlaceSelectionGrid();

					// Create a new selection box.
					isCreatingSelectionBox	= true;
					dragBeginPoint			= point;
					dragBeginTileCoord		= tileCoord;

					Rectangle2I selectionBox;
					Level level = EditorControl.Level;
					
					if (System.Windows.Forms.Control.ModifierKeys.HasFlag(KEYMOD_ROOM_MODE)) {
						selectionBox = new Rectangle2I(level.GetRoomLocation(
							(LevelTileCoord) dragBeginTileCoord), Point2I.One);
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

		public override void OnMouseDragEnd(MouseEventArgs e) {
			if (e.Button == MouseButtons.Left && isCreatingSelectionBox) {
				isCreatingSelectionBox = false;
				
			}
			else if (e.Button == MouseButtons.Left && isMovingSelectionBox) {
				isMovingSelectionBox = false;
			}
		}

		public override void OnMouseDragMove(MouseEventArgs e) {
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
					moveAmount = (Point2I) GMath.Round((Vector2F) moveAmount / (editorControl.Level.RoomSize * GameSettings.TILE_SIZE));
					moveAmount *= editorControl.Level.RoomSize;
				}
				else {
					moveAmount = pointInLevel - dragBeginPoint;
					moveAmount = (Point2I) GMath.Round((Vector2F) moveAmount / GameSettings.TILE_SIZE);

				}

				Point2I selectionBoxPoint = selectionBoxBeginPoint + moveAmount;
				LevelDisplay.MoveSelectionGridArea(selectionBoxPoint);
			}
		}

		public override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
		}

		public override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
		}

		public override void OnMouseMove(MouseEventArgs e) {
			// Check if mouse is over selection.
			Point2I point = new Point2I(e.X, e.Y);
			Point2I tileCoord = LevelDisplay.SampleLevelTileCoordinates(point) * GameSettings.TILE_SIZE;

			if (!isCreatingSelectionBox &&
				EditorControl.Level == LevelDisplay.SelectionGridLevel &&
				LevelDisplay.SelectionBox.Contains(tileCoord))
			{
				MouseCursor = Cursors.SizeAll;
				editorControl.HighlightMouseTile = false;
			}
			else {
				MouseCursor = Cursors.Default;
				editorControl.HighlightMouseTile = true;
			}

			base.OnMouseMove(e);
		}

	}
}
