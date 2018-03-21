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
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles.ActionTiles;
using Key = System.Windows.Input.Key;
using ModifierKeys = System.Windows.Input.ModifierKeys;
using ZeldaOracle.Common.Scripting;

using WFClipboard = System.Windows.Clipboard;
using ZeldaOracle.Common.Content;
using System.Media;
using ZeldaEditor.Windows;

namespace ZeldaEditor.Tools {

	public class ToolSelection : EditorTool, IEventObjectContainer {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private static readonly Cursor SelectionCursor = LoadCursor("Selection");
		private static readonly Cursor DraggingCursor = Cursors.SizeAll;

		private const ModifierKeys DuplicateModifier = ModifierKeys.Control;
		private const ModifierKeys RoomModeModifier = ModifierKeys.Shift;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private Point2I dragBeginRoomLocation;
		private Point2I dragBeginTileCoord;
		private bool isCreatingSelectionBox;
		private bool isMovingSelectionBox;
		private Point2I selectionBoxBeginPoint;

		//private TileGrid clipboard;

		private SelectionModes mode;
		private Point2I start;
		private Rectangle2I selectionGridArea;
		private TileGrid selectionGrid;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolSelection() : base("Selection Tool", Key.S) {
			//clipboard = null;
			mode = SelectionModes.Move;
			AddOption("SingleLayer");
			AddOption("Merge");
		}


		//-----------------------------------------------------------------------------
		// Tool-Specific Methods
		//-----------------------------------------------------------------------------
		
		public IEnumerable<IPropertyObject> GetPropertyObjects() {
			if (HasSelection) {
				if (selectionGrid == null) {
					selectionGrid = CreateTileGrid(CreateTileGridMode.Twin);
				}
				foreach (BaseTileDataInstance baseTile in selectionGrid.GetAllTiles()) {
					yield return baseTile;
				}
			}
		}

		public IEnumerable<IEventObject> GetEventObjects() {
			if (HasSelection) {
				if (selectionGrid == null) {
					selectionGrid = CreateTileGrid(CreateTileGridMode.Twin);
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
					selectionGrid = CreateTileGrid(CreateTileGridMode.Twin);
				}
				selectionGrid.SaveClipboard();
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
					selectionGrid = CreateTileGrid(CreateTileGridMode.Twin);
				}
				//clipboard = selectionGrid.Duplicate();
				selectionGrid.SaveClipboard();
				UpdateCommands();
			}
		}

		public override void Paste() {
			if (CanPaste) {
				Finish();
				IsDrawing = true;
				try {
					selectionGrid = TileGrid.LoadClipboard();
				}
				catch (ResourceReferenceException ex) {
					TriggerMessageBox.Show(EditorControl.EditorWindow, MessageIcon.Error,
						"Failed to paste selection: " + ex.Message, "Resource Mismatch");
					return;
				}
				catch (Exception ex) {
					EditorControl.ShowExceptionMessage(ex, "paste", "selection");
					return;
				}
				//selectionGrid = clipboard.Duplicate();
				Point2I scroll = new Point2I(LevelDisplay.HorizontalScroll.Value, LevelDisplay.VerticalScroll.Value) + GameSettings.TILE_SIZE - 1;
				Point2I gridStart = LevelDisplay.SampleLevelCoord(scroll);
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
						selectionGrid = CreateTileGrid(CreateTileGridMode.Remove);
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

		protected override void OnFinish() {
			if (selectionGrid != null && !isCreatingSelectionBox) {
				Point2I end = selectionGridArea.Point;
				if (start != end || (mode != SelectionModes.Move && mode != SelectionModes.Duplicate)) {
					EditorAction undo = null;
					// The selection grid that captures everything until the bottom right room boundary
					// in order to preserve tiles overwritten by tiles with sizes larger than 1x1.
					Rectangle2I remainingRoomGrid = selectionGridArea;
					remainingRoomGrid.Size =
						GMath.CeilingI(remainingRoomGrid.BottomRight, Level.RoomSize) -
						remainingRoomGrid.Point;
					switch (mode) {
					case SelectionModes.Move:
						undo = ActionSelection.CreateMoveAction(Level, start, end, selectionGrid,
							CreateTileGrid(remainingRoomGrid, CreateTileGridMode.Twin),
							EditorControl.ToolOptionMerge);
						break;
					case SelectionModes.Delete:
					case SelectionModes.Cut:
						undo = ActionSelection.CreateDeleteAction(Level, start, selectionGrid,
							mode == SelectionModes.Cut);
						break;
					case SelectionModes.Duplicate:
					case SelectionModes.Paste:
						undo = ActionSelection.CreateDuplicateAction(Level, end, selectionGrid,
							CreateTileGrid(remainingRoomGrid, CreateTileGridMode.Twin),
							mode == SelectionModes.Paste, EditorControl.ToolOptionMerge);
						break;
					}
					EditorControl.PushAction(undo, ActionExecution.Execute);
				}
				else {
					Level.PlaceTileGrid(selectionGrid,
						selectionGridArea.Point,
						EditorControl.ToolOptionMerge);
				}
				selectionGrid = null;
			}
			IsDrawing = false;
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
				OnFinish();
				ClearSelection();
			}
		}

		protected override void OnMouseDragBegin(MouseEventArgs e) {

			Point2I mousePos = e.MousePos();
			Point2I levelCoord = LevelDisplay.SampleLevelCoord(mousePos);
			Point2I roomLocation = Level.LevelCoordToRoomLocation(levelCoord);

			if (isCreatingSelectionBox || isMovingSelectionBox)
				return;

			// Draw a new selecion box.
			if (e.Button == MouseButtons.Left) {

				if (selectionGridArea.Contains(levelCoord)) {
					// Begin moving the selection box.
					isMovingSelectionBox	= true;
					dragBeginTileCoord		= levelCoord;
					dragBeginRoomLocation	= roomLocation;
					selectionBoxBeginPoint	= selectionGridArea.Point;

					// Duplicate selection if holding Ctrl.
					if (Modifiers.HasFlag(DuplicateModifier)) {
						if (selectionGrid != null) {
							TileGrid newSelectionGrid = selectionGrid.Duplicate();
							OnFinish();
							selectionGrid = newSelectionGrid;
						}
						else {
							selectionGrid = CreateTileGrid(CreateTileGridMode.Duplicate);
						}
						mode = SelectionModes.Duplicate;
						start = selectionGridArea.Point;
					}
					else if (selectionGrid == null) {
						selectionGrid = CreateTileGrid(CreateTileGridMode.Twin);
						mode = SelectionModes.Move;
						start = selectionGridArea.Point;
					}
				}
				else {
					OnFinish();
					IsDrawing = true;

					// Create a new selection box.
					isCreatingSelectionBox  = true;
					dragBeginTileCoord      = levelCoord;


					if (!Level.ContainsLevelCoord(dragBeginTileCoord)) {
						// Do nothing
					}
					else if (Modifiers.HasFlag(RoomModeModifier)) {
						selectionGridArea = new Rectangle2I(
							Level.LevelCoordToRoomCoord(dragBeginTileCoord),
							Level.RoomSize);
					}
					else {
						selectionGridArea = new Rectangle2I(
							dragBeginTileCoord, Point2I.One);
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
				Point2I tileCoord = LevelDisplay.SampleLevelCoord(mousePos);
				Level level = EditorControl.Level;

				if (Modifiers.HasFlag(RoomModeModifier)) {
					selectionGridArea = Rectangle2I.FromEndPointsOne(
						dragBeginTileCoord, tileCoord);
					selectionGridArea = level.LevelCoordToRoomCoord(
						selectionGridArea, true);
					/*Point2I roomCoord1 = level.LevelCoordToRoomLocation(dragBeginTileCoord);
					Point2I roomCoord2 = level.LevelCoordToRoomLocation(tileCoord);
					Point2I roomCoordMin = GMath.Min(roomCoord1, roomCoord2);
					Point2I roomCoordMax = GMath.Max(roomCoord1, roomCoord2);

					Rectangle2I levelDimensions = new Rectangle2I(Point2I.Zero, level.Dimensions);
					selectionGridArea = new Rectangle2I(roomCoordMin, roomCoordMax - roomCoordMin + Point2I.One);
					selectionGridArea = Rectangle2I.Intersect(selectionGridArea, levelDimensions);
					selectionGridArea *= level.RoomSize;*/
				}
				else {
					selectionGridArea = Rectangle2I.FromEndPointsOne(
						dragBeginTileCoord, tileCoord);
					selectionGridArea = Rectangle2I.Intersect(
						selectionGridArea, level.TileBounds);
					/*Point2I minCoord  = GMath.Min(dragBeginTileCoord, tileCoord);
					Point2I maxCoord  = GMath.Max(dragBeginTileCoord, tileCoord);

					Rectangle2I levelBounds = new Rectangle2I(Point2I.Zero, level.RoomSize * level.Dimensions);
					selectionGridArea = new Rectangle2I(minCoord, maxCoord - minCoord + Point2I.One);
					selectionGridArea = Rectangle2I.Intersect(selectionGridArea, levelBounds);*/
				}

				UpdateSelectionBox();
			}
			else if (e.Button == MouseButtons.Left && isMovingSelectionBox) {
				Point2I moveAmount;

				if (Modifiers.HasFlag(RoomModeModifier)) {
					Point2I roomCoord = LevelDisplay.SampleRoomCoordinates(mousePos);
					moveAmount = (roomCoord - dragBeginRoomLocation) * Level.RoomSize;
				}
				else {
					Point2I levelCoord = LevelDisplay.SampleLevelCoord(mousePos);
					moveAmount = levelCoord - dragBeginTileCoord;
				}

				selectionGridArea.Point = selectionBoxBeginPoint + moveAmount;

				UpdateSelectionBox();
			}
			else if (e.Button == MouseButtons.Right) {
				OnFinish();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			Point2I tileCoord = LevelDisplay.SampleLevelCoord(e.MousePos());

			// Check if mouse is over selection
			if ((!isCreatingSelectionBox && selectionGridArea.Contains(tileCoord)) ||
				isMovingSelectionBox)
			{
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

		public override bool DrawHideTile(TileDataInstance tile, Room room,
			Point2I levelCoord, int layer)
		{
			if (selectionGrid != null && selectionGrid.IncludesTiles) {
				if (layer < selectionGrid.StartLayer || layer > selectionGrid.EndLayer)
					return false;
				if (mode != SelectionModes.Duplicate && mode != SelectionModes.Paste &&
					new Rectangle2I(start, selectionGrid.Size).Contains(levelCoord))
				{
					return (!EditorControl.ToolOptionMerge ||
						selectionGrid.GetTile(levelCoord - start, layer) != null);
				}
				return selectionGridArea.Contains(levelCoord) &&
					(!EditorControl.ToolOptionMerge ||
					selectionGrid.GetTile(levelCoord - selectionGridArea.Point, layer) != null);
			}
			return false;
		}

		public override void DrawTile(Graphics2D g, Room room, Point2I position,
			Point2I levelCoord, int layer)
		{
			if (selectionGrid != null && selectionGridArea.Contains(levelCoord) &&
				selectionGrid.IncludesTiles)
			{
				if (layer < selectionGrid.StartLayer || layer > selectionGrid.EndLayer)
					return;
				TileDataInstance tile = selectionGrid.GetTile(levelCoord -
					selectionGridArea.Point, layer);
				if (tile != null) {
					LevelDisplay.DrawTile(g, room, tile, position, LevelDisplay.NormalColor);
				}
			}
		}

		public override bool DrawHideActionTile(ActionTileDataInstance actionTile,
			Room room, Point2I levelPosition)
		{
			if (selectionGrid != null && selectionGrid.IncludesActions) {
				if (mode != SelectionModes.Duplicate && mode != SelectionModes.Paste &&
					(new Rectangle2I(start, selectionGrid.Size) *
					GameSettings.TILE_SIZE).Contains(levelPosition))
					return true;
				return Level.LevelCoordToPosition(selectionGridArea).
					Contains(levelPosition) && !EditorControl.ToolOptionMerge;
			}
			return false;
		}

		public override void DrawActionTiles(Graphics2D g) {
			if (selectionGrid != null && selectionGrid.IncludesActions) {
				Point2I startLevelPixel = selectionGridArea.Point *
					GameSettings.TILE_SIZE;
				foreach (var action in selectionGrid.GetActionTilesAndPositions()) {
					Point2I position = LevelDisplay.
						GetLevelPixelDrawPosition(startLevelPixel + action.Position);
					Room room = LevelDisplay.SampleRoom(position);
					if (room != null)
						LevelDisplay.DrawActionTile(g, room, action.Action, position,
							LevelDisplay.NormalColor);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private TileGrid CreateTileGrid(CreateTileGridMode mode) {
			return CreateTileGrid(selectionGridArea, mode);
		}

		private TileGrid CreateTileGrid(Rectangle2I area, CreateTileGridMode mode) {
			if (EditorControl.ToolOptionSingleLayer) {
				if (ActionMode)
					return Level.CreateActionGrid(area, mode);
				else
					return Level.CreateSingleLayerTileGrid(area, Layer, mode);
			}
			else {
				return Level.CreateFullTileGrid(area, mode);
			}
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
		// Internal Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the selection grid area at the starting point.</summary>
		private Rectangle2I StartGridArea {
			get { return new Rectangle2I(start, selectionGridArea.Size); }
		}


		//-----------------------------------------------------------------------------
		// Tool-Specific Properties
		//-----------------------------------------------------------------------------
		
		public bool CanPaste {
			get { return TileGrid.ContainsClipboard(); }
		}

		public bool HasSelection {
			get { return !selectionGridArea.IsEmpty; }
		}
	}
}
