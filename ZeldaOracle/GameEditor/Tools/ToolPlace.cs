using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Worlds.Editing;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaEditor.Undo;
using ZeldaOracle.Common.Graphics;
using ZeldaEditor.Control;
using Key = System.Windows.Input.Key;

namespace ZeldaEditor.Tools {
	public class ToolPlace : EditorTool {

		private static readonly Cursor PencilCursor = LoadCursor("Pencil");

		private ActionPlace tileAction;

		private TileData drawTileData;
		private TileDataInstance drawTile;
		private HashSet<Point2I> overwrittenTiles;
		private Dictionary<Point2I, TileDataInstance> placedTiles;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolPlace() : base("Place Tool", Key.P) {
			overwrittenTiles = new HashSet<Point2I>();
			placedTiles = new Dictionary<Point2I, TileDataInstance>();
		}

		
		//-----------------------------------------------------------------------------
		// Overridden State Methods
		//-----------------------------------------------------------------------------
		
		protected override void OnInitialize() {
			MouseCursor = PencilCursor;
		}

		protected override void OnBegin() {
			EditorControl.HighlightMouseTile = true;
		}

		protected override void OnCancel() {
			overwrittenTiles.Clear();
			placedTiles.Clear();
			tileAction = null;
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseDown(MouseEventArgs e) {
			Point2I mousePos    = e.MousePos();
			Room room			= LevelDisplay.SampleRoom(mousePos);


			if (IsDrawing && e.Button.IsOpposite(DragButton)) {
				Cancel();
			}
			else if (e.Button.IsLeftOrRight()) {
				if (ActionMode) {
					if (room == null)
						return;

					ActionTileData actionTileData = GetActionTileData();
					if (e.Button == MouseButtons.Right)
						actionTileData = null;

					Point2I levelHalfTileCoord = LevelDisplay.SampleLevelHalfTileCoordinates(mousePos);
					Point2I position = (levelHalfTileCoord - room.LevelCoord * 2) * (GameSettings.TILE_SIZE / 2);
					ActionPlaceAction actionAction = new ActionPlaceAction(Level, actionTileData, room, position);

					ActionTileDataInstance actionTile = LevelDisplay.SampleActionTile(mousePos);
					if (e.Button == MouseButtons.Left) {
						while (actionTile != null) {
							actionAction.AddOverwrittenActionTile(actionTile);
							actionTile.Room.RemoveActionTile(actionTile);
							actionTile = LevelDisplay.SampleActionTile(mousePos);
						}
					}
					else if (actionTile != null) {
						actionAction.AddOverwrittenActionTile(actionTile);
						actionTile.Room.RemoveActionTile(actionTile);
					}
					EditorControl.PushAction(actionAction, ActionExecution.PostExecute);
					actionAction = null;
				}
				else if (!IsTileSingle) {
					Point2I size = GetTileDataSize();

					tileAction = ActionPlace.CreatePlaceAction(Level, Layer, GetTileData());

					Point2I levelCoord = LevelDisplay.SampleLevelCoord(mousePos);
					
					tileAction.AddPlacedTile(levelCoord);
					for (int x = 0; x < size.X; x++) {
						for (int y = 0; y < size.Y; y++) {
							Point2I point = levelCoord + new Point2I(x, y);
							TileDataInstance tile = Level.GetTileAt(point, Layer);
							if (tile == null) continue;
							tileAction.AddOverwrittenTile(tile);
							/*Point2I roomLocation = point / Level.RoomSize;
							room = Level.GetRoomAt(roomLocation);
							if (room == null) continue;
							TileDataInstance tile = LevelDisplay.GetTile(point, Layer);
							if (tile == null) continue;*/
						}
					}

					EditorControl.PushAction(tileAction, ActionExecution.Execute);
					tileAction = null;
				}
			}
		}

		protected override void OnMouseDragBegin(MouseEventArgs e) {
			if (!ActionMode && IsTileSingle && DragButton.IsLeftOrRight()) {
				IsDrawing = true;
				drawTileData = GetTileData();
				if (DragButton == MouseButtons.Left) {
					drawTile = CreateDrawTile();
				}
				else {
					drawTileData = null;
					drawTile = null;
				}
				tileAction = ActionPlace.CreatePlaceAction(Level, Layer, drawTileData);
				OnMouseDragMove(e);
			}
		}

		protected override void OnMouseDragEnd(MouseEventArgs e) {
			if (IsDrawing) {
				IsDrawing = false;
				EditorControl.PushAction(tileAction, ActionExecution.Execute);
				tileAction = null;
				overwrittenTiles.Clear();
				placedTiles.Clear();
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			if (IsDrawing) {
				Point2I mousePos = e.MousePos();
				Point2I levelCoord = LevelDisplay.SampleLevelCoord(mousePos);
				Room room = LevelDisplay.SampleRoom(mousePos);
				if (Level.ContainsLevelCoord(levelCoord) &&
					!placedTiles.ContainsKey(levelCoord))
				{
					placedTiles.Add(levelCoord, drawTile);
					tileAction.AddPlacedTile(levelCoord);
					//TileDataInstance tile = LevelDisplay.GetTile(levelCoord, Layer);
					TileDataInstance tile = Level.GetTileAt(levelCoord, Layer);
					if (tile != null && !overwrittenTiles.Contains(tile.LevelCoord)) {
						overwrittenTiles.Add(tile.LevelCoord);
						tileAction.AddOverwrittenTile(tile);
					}
				}
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
			if (IsDrawing && layer == Layer) {
				return (placedTiles.ContainsKey(levelCoord) ||
						overwrittenTiles.Contains(levelCoord));
			}
			return false;
		}

		public override void DrawTile(Graphics2D g, Room room, Point2I position,
			Point2I levelCoord, int layer)
		{
			if (!ActionMode && layer == Layer) {
				if (!IsDrawing && levelCoord == LevelDisplay.CursorTileLocation) {
					TileDataInstance tile = CreateDrawTile();
					if (tile != null)
						LevelDisplay.DrawTile(g, room, CreateDrawTile(), position,
							LevelDisplay.FadeAboveColor);
				}
				else if (IsDrawing && placedTiles.ContainsKey(levelCoord) &&
					drawTile != null)
				{
					LevelDisplay.DrawTile(g, room, drawTile, position,
						LevelDisplay.NormalColor);
				}
			}
		}

		public override void DrawActionTiles(Graphics2D g) {
			if (ActionMode) {
				ActionTileDataInstance actionTile = CreateDrawActionTile();
				if (actionTile != null) {
					Point2I position = LevelDisplay
						.GetLevelPixelDrawPosition(actionTile.Position);
					Room room = LevelDisplay.SampleRoom(position);
					if (room != null)
						LevelDisplay.DrawActionTile(g, room, actionTile, position,
							LevelDisplay.FadeAboveColor);
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

		private ActionTileDataInstance CreateDrawActionTile() {
			ActionTileData actionTileData = GetActionTileData();
			if (actionTileData != null)
				return new ActionTileDataInstance(actionTileData,
					LevelDisplay.CursorHalfTileLocation * (GameSettings.TILE_SIZE / 2));
			return null;
		}

		private TileData GetTileData() {
			return EditorControl.SelectedTileData as TileData;
		}

		private ActionTileData GetActionTileData() {
			return EditorControl.SelectedTileData as ActionTileData;
		}

		private Point2I GetTileDataSize() {
			if (GetTileData() != null)
				return GMath.Max(Point2I.One, GetTileData().TileSize);
			return Point2I.One;
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
