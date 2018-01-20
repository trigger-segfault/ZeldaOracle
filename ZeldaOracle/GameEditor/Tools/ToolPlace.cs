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
using ZeldaOracle.Game.Tiles.EventTiles;
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

			if (IsDrawing && e.Button.IsOpposite(DragButton)) {
				Cancel();
			}
			else if (EditorControl.EventMode && e.Button.IsLeftOrRight()) {
				if (room == null)
					return;

				EventTileData eventTileData = GetEventTileData();
				if (e.Button == MouseButtons.Right)
					eventTileData = null;

				Point2I levelHalfTileCoord = LevelDisplay.SampleLevelHalfTileCoordinates(mousePos);
				Point2I position = (levelHalfTileCoord - room.Location * Level.RoomSize * 2) * (GameSettings.TILE_SIZE / 2);
				ActionEventTile eventAction = new ActionEventTile(Level, eventTileData, room, position);

				EventTileDataInstance eventTile = LevelDisplay.SampleEventTile(mousePos);
				if (e.Button == MouseButtons.Left) {
					while (eventTile != null) {
						eventAction.AddOverwrittenEventTile(eventTile);
						eventTile.Room.RemoveEventTile(eventTile);
						eventTile = LevelDisplay.SampleEventTile(mousePos);
					}
				}
				else if (eventTile != null) {
					eventAction.AddOverwrittenEventTile(eventTile);
					eventTile.Room.RemoveEventTile(eventTile);
				}
				EditorControl.PushAction(eventAction, ActionExecution.PostExecute);
			}
		}

		protected override void OnMouseDragBegin(MouseEventArgs e) {
			if (!EditorControl.EventMode && DragButton.IsLeftOrRight()) {
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
					TileDataInstance tile = LevelDisplay.SampleTile(mousePos, EditorControl.CurrentLayer);
					placedTiles.Add(levelTileCoord, drawTile);
					tileAction.AddOverwrittenTile(levelTileCoord, tile);
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
			if (!EditorControl.EventMode && layer == EditorControl.CurrentLayer) {
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

		public override void DrawEventTiles(Graphics2D g) {
			if (EditorControl.EventMode) {
				EventTileDataInstance eventTile = CreateDrawEventTile();
				if (eventTile != null) {
					Point2I position = LevelDisplay.GetLevelPixelDrawPosition(eventTile.Position);
					Room room = LevelDisplay.SampleRoom(position);
					if (room != null)
						LevelDisplay.DrawEventTile(g, room, eventTile, position, LevelDisplay.FadeAboveColor);
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

		private EventTileDataInstance CreateDrawEventTile() {
			EventTileData eventTileData = GetEventTileData();
			if (eventTileData != null)
				return new EventTileDataInstance(eventTileData, LevelDisplay.CursorHalfTileLocation * (GameSettings.TILE_SIZE / 2));
			return null;
		}

		private TileData GetTileData() {
			return EditorControl.SelectedTileData as TileData;
		}

		private EventTileData GetEventTileData() {
			return EditorControl.SelectedTileData as EventTileData;
		}
	}
}
