using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Worlds.Editing;

namespace ZeldaEditor.Undo {
	public class ActionPlaceAction : EditorAction {

		private Level level;
		private ActionTileData placedActionData;
		private ActionTileInstancePosition placedActionTile;
		//private Point2I placedPosition;
		//private Room placedRoom;
		private List<ActionTileInstancePosition> overwrittenActionTiles;

		public ActionPlaceAction(Level level, ActionTileData placedActionData, Point2I position) {
			ActionName = (placedActionData == null ?
				"Erase" : "Place") + " Action";
			ActionIcon = (placedActionData == null ?
				EditorImages.ToolPlaceErase : EditorImages.ToolPlace);

			this.level = level;
			this.placedActionData = placedActionData;
			placedActionTile = new ActionTileInstancePosition(
				placedActionData, position);
			overwrittenActionTiles = new List<ActionTileInstancePosition>();
		}

		public ActionPlaceAction(Level level) :
			this(level, null, Point2I.Zero)
		{
		}

		public void AddOverwrittenActionTile(ActionTileDataInstance actionTile) {
			overwrittenActionTiles.Add(
				ActionTileInstancePosition.FromLevel(actionTile));
		}

		public override void Execute(EditorControl editorControl) {
			foreach (var actionTile in overwrittenActionTiles) {
				level.RemoveActionTile(actionTile.Action);
			}
			if (placedActionData != null) {
				level.PlaceActionTile(placedActionTile);
			}
		}

		public override void PostExecute(EditorControl editorControl) {
			if (placedActionData != null) {
				level.PlaceActionTile(placedActionTile);
			}
		}

		public override void Undo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			foreach (var actionTile in overwrittenActionTiles) {
				level.PlaceActionTile(actionTile);
				//actionTileInfo.ActionTile.Position = actionTileInfo.Position;
				//actionTileInfo.Room.PlaceActionTile(actionTileInfo.ActionTile,
				//	actionTileInfo.Position);
			}
			if (placedActionData != null) {
				level.RemoveActionTile(placedActionTile.Action);
			}
			//if (placedActionTileInfo.ActionTile != null)
			//	placedActionTileInfo.Room.RemoveActionTile(placedActionTileInfo.ActionTile);
			editorControl.NeedsNewEventCache = true;
		}

		public override void Redo(EditorControl editorControl) {
			editorControl.OpenLevel(level);
			foreach (var actionTile in overwrittenActionTiles) {
				level.RemoveActionTile(actionTile.Action);
				//actionTileInfo.ActionTile.Position = actionTileInfo.Position;
				//actionTileInfo.Room.PlaceActionTile(actionTileInfo.ActionTile,
				//	actionTileInfo.Position);
			}
			/*foreach (var actionTileInfo in overwrittenActionTiles) {
				actionTileInfo.Room.RemoveActionTile(actionTileInfo.ActionTile);
			}*/
			if (placedActionData != null) {
				level.PlaceActionTile(placedActionTile);
			}
			editorControl.NeedsNewEventCache = true;
		}

		public override bool IgnoreAction {
			get { return !overwrittenActionTiles.Any() && placedActionData == null; }
		}
	}
}
