using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaEditor.Undo {
	public enum SelectionModes {
		Move,
		Delete,
		Duplicate,
		Paste,
		Cut
	}

	public class ActionSelection : EditorAction {

		private Level level;
		private Point2I start;
		private Point2I end;
		private TileGrid tileGrid;
		private TileGrid overwrittenTileGrid;
		private SelectionModes mode;
		private bool merge;

		private ActionSelection() { }
		
		public static ActionSelection CreateMoveAction(Level level, Point2I start,
			Point2I end, TileGrid tileGrid, TileGrid overwrittenTileGrid, bool merge)
		{
			ActionSelection action = new ActionSelection();
			action.ActionName = "Move Selection";
			action.ActionIcon = EditorImages.SelectAll;
			action.level = level;
			action.start = start;
			action.end = end;
			action.tileGrid = tileGrid;
			action.overwrittenTileGrid = overwrittenTileGrid;
			action.mode = SelectionModes.Move;
			action.merge = merge;
			return action;
		}

		public static ActionSelection CreateDeleteAction(Level level, Point2I start,
			TileGrid tileGrid, bool isCut)
		{
			ActionSelection action = new ActionSelection();
			action.ActionName = (isCut ? "Cut" : "Delete") + " Selection";
			action.ActionIcon = (isCut ? EditorImages.Cut : EditorImages.Deselect);
			action.level = level;
			action.start = start;
			action.tileGrid = tileGrid;
			action.mode = SelectionModes.Delete;
			return action;
		}

		public static ActionSelection CreateDuplicateAction(Level level, Point2I end,
			TileGrid tileGrid, TileGrid overwrittenTileGrid, bool isPaste, bool merge)
		{
			ActionSelection action = new ActionSelection();
			action.ActionName = (isPaste ? "Paste" : "Duplicate") + " Selection";
			action.ActionIcon = (isPaste ? EditorImages.Paste : EditorImages.Copy);
			action.level = level;
			action.end = end;
			action.tileGrid = tileGrid;
			action.overwrittenTileGrid = overwrittenTileGrid;
			action.mode = SelectionModes.Duplicate;
			action.merge = merge;
			return action;
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			switch (mode) {
			case SelectionModes.Move:
				level.PlaceTileGrid(overwrittenTileGrid, end, false);
				level.PlaceTileGrid(tileGrid, start, false);
				break;
			case SelectionModes.Delete:
				level.PlaceTileGrid(tileGrid, start, false);
				break;
			case SelectionModes.Duplicate:
				level.PlaceTileGrid(overwrittenTileGrid, end, false);
				break;
			}
			editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.OpenLevel(level);

			if (mode == SelectionModes.Move) {
				level.RemoveArea(new Rectangle2I(start, tileGrid.Size), tileGrid);
				level.PlaceTileGrid(tileGrid, end, merge);
			}
			if (mode == SelectionModes.Delete) {
				level.RemoveArea(new Rectangle2I(start, tileGrid.Size), tileGrid);
			}
			if (mode == SelectionModes.Duplicate) {
				level.PlaceTileGrid(tileGrid, end, merge);
			}

			editorControl.NeedsNewEventCache = true;
		}

		public override bool IgnoreAction {
			get {
				if (mode == SelectionModes.Move) {
					return (start == end);
				}
				else if (mode == SelectionModes.Duplicate) {
					Rectangle2I levelBounds = new Rectangle2I(level.Dimensions * level.RoomSize);
					Rectangle2I tileGridBounds = new Rectangle2I(end, tileGrid.Size);
					return !levelBounds.Intersects(tileGridBounds);
				}
				else
					return false;
			}
		}
	}
}
