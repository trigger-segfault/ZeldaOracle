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
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles.ActionTiles;
using System.Windows.Input;
using Cursor = System.Windows.Forms.Cursor;
using Cursors = System.Windows.Forms.Cursors;
using ZeldaOracle.Common.Scripting;

using WFClipboard = System.Windows.Clipboard;
using ZeldaOracle.Common.Content;
using System.Media;
using ZeldaEditor.Windows;
using ZeldaWpf.Windows;
using ZeldaWpf.Resources;
using ZeldaWpf.Util;

namespace ZeldaEditor.Tools {

	public class ToolSelection : EditorTool, IEventObjectContainer {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private static readonly MultiCursor SelectionCursor = WpfCursors.Square;//LoadCursor("Selection");
		private static readonly MultiCursor DraggingCursor = MultiCursors.SizeAll;

		private const ModifierKeys DuplicateModifier = ModifierKeys.Control;
		private const ModifierKeys RoomModeModifier = ModifierKeys.Shift;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private Point2I dragBeginRoomLocation;
		private Point2I dragBeginCoord;
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

		public IEnumerable<ITriggerObject> GetEventObjects() {
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
				Point2I scroll = ScrollPosition + GameSettings.TILE_SIZE - 1;
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
			
			selectionGridArea = new Rectangle2I(Level.TileDimensions);
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

		protected override void OnBegin(ToolEventArgs e) {
			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;
			ShowCursor = true;
			CursorPosition = e.SnappedPosition;
			CursorTileSize = Point2I.One;
		}

		protected override void OnEnd(ToolEventArgs e) {
			Finish();
			ClearSelection();
		}

		protected override void OnFinish(ToolEventArgs e) {
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

		protected override void OnCancel(ToolEventArgs e) {
			isCreatingSelectionBox = false;
			isMovingSelectionBox = false;
			ClearSelection();
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseDown(ToolEventArgs e) {
			if (e.Button == MouseButton.Right) {
				OnFinish(e);
				ClearSelection();
			}
		}

		protected override void OnMouseDragBegin(ToolEventArgs e) {
			
			//Point2I levelCoord = LevelDisplay.SampleLevelCoord(e.Position);
			//Point2I roomLocation = Level.LevelCoordToRoomLocation(levelCoord);

			if (isCreatingSelectionBox || isMovingSelectionBox)
				return;

			// Draw a new selecion box.
			if (e.Button == MouseButton.Left) {

				if (selectionGridArea.Contains(e.LevelCoord)) {
					// Begin moving the selection box.
					isMovingSelectionBox	= true;
					dragBeginCoord			= e.LevelCoord;
					dragBeginRoomLocation	= e.RoomLocation;
					selectionBoxBeginPoint	= selectionGridArea.Point;

					// Duplicate selection if holding Ctrl.
					if (Modifiers.HasFlag(DuplicateModifier)) {
						if (selectionGrid != null) {
							TileGrid newSelectionGrid = selectionGrid.Duplicate();
							OnFinish(e);
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
					OnFinish(e);
					IsDrawing = true;

					// Create a new selection box.
					isCreatingSelectionBox	= true;
					dragBeginCoord			= e.LevelCoord;


					if (!Level.ContainsLevelCoord(dragBeginCoord)) {
						// Do nothing
					}
					else if (Modifiers.HasFlag(RoomModeModifier)) {
						selectionGridArea = new Rectangle2I(
							Level.LevelToRoomLocationCoord(dragBeginCoord),
							Level.RoomSize);
					}
					else {
						selectionGridArea = new Rectangle2I(
							dragBeginCoord, Point2I.One);
					}

					UpdateSelectionBox();
				}
			}
		}

		protected override void OnMouseDragEnd(ToolEventArgs e) {
			if (e.Button == MouseButton.Left && isCreatingSelectionBox) {
				isCreatingSelectionBox = false;
				mode = SelectionModes.Move;
				start = selectionGridArea.Point;
				OnMouseMove(e);
				UpdateCommands();
			}
			else if (e.Button == MouseButton.Left && isMovingSelectionBox) {
				isMovingSelectionBox = false;
			}
		}

		protected override void OnMouseDragMove(ToolEventArgs e) {
			// Update selection box.
			if (e.Button == MouseButton.Left && isCreatingSelectionBox) {
				//Point2I tileCoord = LevelDisplay.SampleLevelCoord(e.Position);
				//Level level = EditorControl.Level;

				if (Modifiers.HasFlag(RoomModeModifier)) {
					selectionGridArea = Rectangle2I.FromEndPointsOne(
						dragBeginCoord, e.LevelCoord);
					selectionGridArea = Level.LevelToRoomLocationCoord(
						selectionGridArea, true);
				}
				else {
					selectionGridArea = Rectangle2I.FromEndPointsOne(
						dragBeginCoord, e.LevelCoord);
					selectionGridArea = Rectangle2I.Intersect(
						selectionGridArea, Level.TileBounds);
				}

				UpdateSelectionBox();
			}
			else if (e.Button == MouseButton.Left && isMovingSelectionBox) {
				Point2I moveAmount;

				if (Modifiers.HasFlag(RoomModeModifier)) {
					moveAmount = (e.RoomLocation - dragBeginRoomLocation) * Level.RoomSize;
				}
				else {
					moveAmount = e.LevelCoord - dragBeginCoord;
				}

				selectionGridArea.Point = selectionBoxBeginPoint + moveAmount;

				UpdateSelectionBox();
			}
			else if (e.Button == MouseButton.Right) {
				OnFinish(e);
			}
		}

		protected override void OnMouseMove(ToolEventArgs e) {
			// Check if mouse is over selection
			if (isMovingSelectionBox || (!isCreatingSelectionBox &&
				selectionGridArea.Contains(e.LevelCoord)))
			{
				MouseCursor = DraggingCursor;
				ShowCursor = false;
			}
			else {
				MouseCursor = SelectionCursor;
				ShowCursor = true;
				CursorPosition = e.SnappedPosition;
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
			UpdateCommands();
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
