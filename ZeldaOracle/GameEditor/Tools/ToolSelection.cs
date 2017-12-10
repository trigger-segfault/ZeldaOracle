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
using Key = System.Windows.Input.Key;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Tools {

	public class ToolSelection : EditorTool, IEventObjectContainer {

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

		private Point2I dragBeginRoomCoord;
		private Point2I dragBeginTileCoord;
		private bool isCreatingSelectionBox;
		private bool isMovingSelectionBox;
		private Point2I selectionBoxBeginPoint;

		private TileGrid clipboard;

		private SelectionModes mode;
		private Point2I start;
		private Rectangle2I selectionGridArea;
		private TileGrid selectionGrid;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolSelection() : base("Selection Tool", Key.S) {
			clipboard = null;
			mode = SelectionModes.Move;
		}


		//-----------------------------------------------------------------------------
		// Tool-Specific Methods
		//-----------------------------------------------------------------------------
		
		public IEnumerable<IPropertyObject> GetPropertyObjects() {
			if (HasSelection) {
				if (selectionGrid == null) {
					selectionGrid = Level.CreateTileGrid(selectionGridArea, CreateTileGridMode.Twin);
				}
				foreach (BaseTileDataInstance baseTile in selectionGrid.GetAllTiles()) {
					yield return baseTile;
				}
			}
		}

		public IEnumerable<IEventObject> GetEventObjects() {
			if (HasSelection) {
				if (selectionGrid == null) {
					selectionGrid = Level.CreateTileGrid(selectionGridArea, CreateTileGridMode.Twin);
				}
				foreach (BaseTileDataInstance baseTile in selectionGrid.GetAllTiles()) {
					yield return baseTile;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Clipboard Methods
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
				Point2I scroll = new Point2I(LevelDisplay.HorizontalScroll.Value, LevelDisplay.VerticalScroll.Value) + GameSettings.TILE_SIZE - 1;
				Point2I gridStart = LevelDisplay.SampleLevelTileCoordinates(scroll);
				selectionGridArea = new Rectangle2I(gridStart, selectionGrid.Size);
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
			ClearSelection();
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

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
					dragBeginTileCoord      = levelTileCoord;
					dragBeginRoomCoord      = LevelDisplay.SampleRoomCoordinates(mousePos, false);
					selectionBoxBeginPoint  = selectionGridArea.Point;

					// Duplicate selection if holding Ctrl.
					if (System.Windows.Forms.Control.ModifierKeys.HasFlag(DuplicateModifier)) {
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
					dragBeginTileCoord      = levelTileCoord;


					if (System.Windows.Forms.Control.ModifierKeys.HasFlag(RoomModeModifier)) {
						selectionGridArea = new Rectangle2I(Level.GetRoomLocation(
							(LevelTileCoord)dragBeginTileCoord), Point2I.One);
						Rectangle2I levelBounds = new Rectangle2I(Point2I.Zero, Level.Dimensions);
						selectionGridArea = Rectangle2I.Intersect(selectionGridArea, levelBounds);
						selectionGridArea *= Level.RoomSize;
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
				OnMouseMove(e);
			}
			else if (e.Button == MouseButtons.Left && isMovingSelectionBox) {
				isMovingSelectionBox = false;
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			Point2I mousePos = e.MousePos();

			// Update selection box.
			if (e.Button == MouseButtons.Left && isCreatingSelectionBox) {
				Point2I tileCoord = LevelDisplay.SampleLevelTileCoordinates(mousePos);
				Level level = EditorControl.Level;

				if (System.Windows.Forms.Control.ModifierKeys.HasFlag(RoomModeModifier)) {
					Point2I roomCoord1 = level.GetRoomLocation((LevelTileCoord) dragBeginTileCoord);
					Point2I roomCoord2 = level.GetRoomLocation((LevelTileCoord) tileCoord);
					Point2I roomCoordMin = GMath.Min(roomCoord1, roomCoord2);
					Point2I roomCoordMax = GMath.Max(roomCoord1, roomCoord2);

					Rectangle2I levelDimensions = new Rectangle2I(Point2I.Zero, level.Dimensions);
					selectionGridArea = new Rectangle2I(roomCoordMin, roomCoordMax - roomCoordMin + Point2I.One);
					selectionGridArea = Rectangle2I.Intersect(selectionGridArea, levelDimensions);
					selectionGridArea *= level.RoomSize;
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

				if (System.Windows.Forms.Control.ModifierKeys.HasFlag(RoomModeModifier)) {
					Point2I roomCoord = LevelDisplay.SampleRoomCoordinates(mousePos);
					moveAmount = (roomCoord - dragBeginRoomCoord) * Level.RoomSize;
				}
				else {
					Point2I levelCoord = LevelDisplay.SampleLevelTileCoordinates(mousePos);
					moveAmount = levelCoord - dragBeginTileCoord;
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

			if ((!isCreatingSelectionBox && selectionGridArea.Contains(tileCoord)) || isMovingSelectionBox) {
				MouseCursor = DraggingCursor;
				EditorControl.HighlightMouseTile = false;
			}
			else {
				MouseCursor = SelectionCursor;
				EditorControl.HighlightMouseTile = true;
			}

			base.OnMouseMove(e);
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------
		
		public override bool CancelCountsAsUndo {
			get { return !selectionGridArea.IsEmpty && !isCreatingSelectionBox; }
		}

		public override bool CanCopyCut {
			get { return !selectionGridArea.IsEmpty && !isCreatingSelectionBox; }
		}

		public override bool CanDeleteDeselect {
			get { return !selectionGridArea.IsEmpty && !isCreatingSelectionBox; }
		}


		//-----------------------------------------------------------------------------
		// Overridden Drawing Methods
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
				foreach (var eventPair in selectionGrid.GetEventTilesAndPositions()) {
					Point2I position = LevelDisplay.GetLevelPixelDrawPosition(startLevelPixel + eventPair.Key);
					Room room = LevelDisplay.SampleRoom(position);
					if (room != null)
						LevelDisplay.DrawEventTile(g, room, eventPair.Value, position, LevelDisplay.NormalColor);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		protected void Finish() {
			if (selectionGrid != null && !isCreatingSelectionBox) {
				Point2I end = selectionGridArea.Point;
				if (start != end || (mode != SelectionModes.Move && mode != SelectionModes.Duplicate)) {
					EditorAction undo = null;
					switch (mode) {
					case SelectionModes.Move:
						undo = ActionSelection.CreateMoveAction(Level, start, end, selectionGrid,
							Level.CreateTileGrid(selectionGridArea, CreateTileGridMode.Twin));
						break;
					case SelectionModes.Delete:
					case SelectionModes.Cut:
						undo = ActionSelection.CreateDeleteAction(Level, start, selectionGrid,
							mode == SelectionModes.Cut);
						break;
					case SelectionModes.Duplicate:
					case SelectionModes.Paste:
						undo = ActionSelection.CreateDuplicateAction(Level, end, selectionGrid,
							Level.CreateTileGrid(selectionGridArea, CreateTileGridMode.Twin),
							mode == SelectionModes.Paste);
						break;
					}
					EditorControl.PushAction(undo, ActionExecution.Execute);
				}
				else {
					Level.PlaceTileGrid(selectionGrid, (LevelTileCoord)selectionGridArea.Point);
				}
				selectionGrid = null;
			}
			IsDrawing = false;
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
		// Tool-Specific Properties
		//-----------------------------------------------------------------------------

		public TileGrid Clipboard {
			get { return clipboard; }
			set { clipboard = value; }
		}

		public bool CanPaste {
			get { return clipboard != null; }
		}

		public bool HasSelection {
			get { return !selectionGridArea.IsEmpty; }
		}
	}
}
