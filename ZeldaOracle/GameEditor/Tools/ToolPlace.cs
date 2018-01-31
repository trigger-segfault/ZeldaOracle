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
		private Dictionary<Point2I, TileDataInstance> placedTiles;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolPlace() : base("Place Tool", Key.P) {
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
			placedTiles.Clear();
			tileAction = null;
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseDown(MouseEventArgs e) {
			Point2I mousePos    = e.MousePos();
			Room room			= LevelDisplay.SampleRoom(mousePos);

			Point2I size = GetTileDataSize();

			if (IsDrawing && e.Button.IsOpposite(DragButton)) {
				Cancel();
			}
			else if (e.Button.IsLeftOrRight()) {
				if (EditorControl.ActionMode) {
					if (room == null)
						return;

					ActionTileData actionTileData = GetActionTileData();
					if (e.Button == MouseButtons.Right)
						actionTileData = null;

					Point2I levelHalfTileCoord = LevelDisplay.SampleLevelHalfTileCoordinates(mousePos);
					Point2I position = (levelHalfTileCoord - room.Location * Level.RoomSize * 2) * (GameSettings.TILE_SIZE / 2);
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
				else if (size >= Point2I.One && size != Point2I.One) {
					tileAction = ActionPlace.CreatePlaceAction(Level, EditorControl.CurrentLayer, GetTileData());

					Point2I levelTileCoord = LevelDisplay.SampleLevelTileCoordinates(mousePos);
					
					tileAction.AddPlacedTile(levelTileCoord);
					for (int x = 0; x < size.X; x++) {
						for (int y = 0; y < size.Y; y++) {
							Point2I point = levelTileCoord + new Point2I(x, y);
							Point2I roomLocation = point / Level.RoomSize;
							room = Level.GetRoomAt(roomLocation);
							if (room == null) continue;
							TileDataInstance tile = LevelDisplay.GetTile(point, EditorControl.CurrentLayer);
							if (tile == null) continue;
							tileAction.AddOverwrittenTile(roomLocation * Level.RoomSize + tile.Location, tile);
						}
					}

					EditorControl.PushAction(tileAction, ActionExecution.Execute);
					tileAction = null;
				}
			}
		}

		protected override void OnMouseDragBegin(MouseEventArgs e) {
			if (!EditorControl.ActionMode && GetTileDataSize() == Point2I.One && DragButton.IsLeftOrRight()) {
				IsDrawing = true;
				drawTileData = GetTileData();
				if (DragButton == MouseButtons.Left) {
					drawTile = CreateDrawTile();
				}
				else {
					drawTileData = null;
					drawTile = null;
				}
				tileAction = ActionPlace.CreatePlaceAction(Level, EditorControl.CurrentLayer, drawTileData);
				OnMouseDragMove(e);
			}
		}

		protected override void OnMouseDragEnd(MouseEventArgs e) {
			if (IsDrawing) {
				IsDrawing = false;
				EditorControl.PushAction(tileAction, ActionExecution.Execute);
				tileAction = null;
				placedTiles.Clear();
			}
		}

		protected override void OnMouseDragMove(MouseEventArgs e) {
			if (IsDrawing) {
				Point2I mousePos = e.MousePos();
				Point2I levelTileCoord = LevelDisplay.SampleLevelTileCoordinates(mousePos);
				Room room = LevelDisplay.SampleRoom(mousePos);
				if (room != null && !placedTiles.ContainsKey(levelTileCoord)) {
					placedTiles.Add(levelTileCoord, drawTile);
					tileAction.AddPlacedTile(levelTileCoord);
					
					TileDataInstance tile = LevelDisplay.GetTile(levelTileCoord, EditorControl.CurrentLayer);
					if (tile != null)
						tileAction.AddOverwrittenTile(room.Location * Level.RoomSize + tile.Location, tile);
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

		public override bool DrawHideTile(TileDataInstance tile, Room room, Point2I levelCoord, int layer) {
			return (IsDrawing && drawTile == null && placedTiles.ContainsKey(levelCoord) && layer == EditorControl.CurrentLayer);
		}

		public override void DrawTile(Graphics2D g, Room room, Point2I position, Point2I levelCoord, int layer) {
			if (!EditorControl.ActionMode && layer == EditorControl.CurrentLayer) {
				if (!IsDrawing && levelCoord == LevelDisplay.CursorTileLocation) {
					TileDataInstance tile = CreateDrawTile();
					if (tile != null)
						LevelDisplay.DrawTile(g, room, CreateDrawTile(), position, LevelDisplay.FadeAboveColor);
				}
				else if (IsDrawing && placedTiles.ContainsKey(levelCoord) && drawTile != null) {
					LevelDisplay.DrawTile(g, room, drawTile, position, LevelDisplay.NormalColor);
				}
			}
		}

		public override void DrawActionTiles(Graphics2D g) {
			if (EditorControl.ActionMode) {
				ActionTileDataInstance actionTile = CreateDrawActionTile();
				if (actionTile != null) {
					Point2I position = LevelDisplay.GetLevelPixelDrawPosition(actionTile.Position);
					Room room = LevelDisplay.SampleRoom(position);
					if (room != null)
						LevelDisplay.DrawActionTile(g, room, actionTile, position, LevelDisplay.FadeAboveColor);
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
				return new ActionTileDataInstance(actionTileData, LevelDisplay.CursorHalfTileLocation * (GameSettings.TILE_SIZE / 2));
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
				return GMath.Max(Point2I.One, GetTileData().Size);
			return Point2I.One;
		}
	}
}
