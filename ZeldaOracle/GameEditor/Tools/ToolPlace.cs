using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Windows.Input;
using Cursor = System.Windows.Forms.Cursor;

namespace ZeldaEditor.Tools {
	public class ToolPlace : EditorTool {

		private static readonly Cursor PencilCursor = LoadCursor("Pencil");

		private ActionPlace tileAction;

		private TileData drawTileData;
		private TileDataInstance drawTile;
		//private HashSet<Point2I> overwrittenTiles;
		//private Dictionary<Point2I, TileDataInstance> placedTiles;
		private TileLocationHashSet overwrittenTiles;
		private TileLocationHashSet placedTiles;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ToolPlace() : base("Place Tool", Key.P) {
			//overwrittenTiles = new HashSet<Point2I>();
			//placedTiles = new Dictionary<Point2I, TileDataInstance>();
		}

		
		//-----------------------------------------------------------------------------
		// Overridden State Methods
		//-----------------------------------------------------------------------------
		
		protected override void OnInitialize() {
			MouseCursor = PencilCursor;
		}

		protected override void OnBegin(ToolEventArgs e) {
			ShowCursor = true;
			CursorPosition = e.SnappedPosition;
			BaseTileData data = EditorControl.SelectedTileData;
			if (data != null)
				CursorSize = data.PixelSize;
			else
				CursorTileSize = Point2I.One;
		}

		protected override void OnCancel(ToolEventArgs e) {
			tileAction			= null;
			overwrittenTiles	= null;
			placedTiles			= null;
		}

		protected override void OnUpdate(ToolEventArgs e) {
			BaseTileData data = EditorControl.SelectedTileData;
			if (data != null)
				CursorSize = data.PixelSize;
			else
				CursorTileSize = Point2I.One;
		}


		//-----------------------------------------------------------------------------
		// Overridden Mouse Methods
		//-----------------------------------------------------------------------------

		protected override void OnMouseMove(ToolEventArgs e) {
			CursorPosition = e.SnappedPosition;
		}

		protected override void OnMouseDown(ToolEventArgs e) {
			if (IsDrawing && e.IsOpposite(DragButton)) {
				Cancel();
			}
			else if (e.IsLeftOrRight) {
				if (ActionMode) {
					ActionTileData actionTileData = GetActionTileData();
					if (e.Button == MouseButton.Right)
						actionTileData = null;
					
					ActionPlaceAction actionAction = new ActionPlaceAction(Level, actionTileData, e.SnappedPosition);

					var actionTiles = e.SampleActionTiles.ToArray();
					if (e.Button == MouseButton.Left) {
						foreach (var actionTile in actionTiles) {
							actionAction.AddOverwrittenActionTile(actionTile);
							Level.RemoveActionTile(actionTile);
						}
					}
					else if (actionTiles.Any()) {
						// Only remove one action tile with right click
						actionAction.AddOverwrittenActionTile(actionTiles.First());
						Level.RemoveActionTile(actionTiles.First());
					}
					EditorControl.PushAction(actionAction, ActionExecution.PostExecute);
					actionAction = null;
				}
				else if (!IsTileSingle) {
					Point2I size = GetTileDataSize();

					tileAction = ActionPlace.CreatePlaceAction(Level, Layer, GetTileData());
					
					tileAction.AddPlacedTile(e.LevelCoord);
					for (int x = 0; x < size.X; x++) {
						for (int y = 0; y < size.Y; y++) {
							Point2I levelCoord = e.LevelCoord + new Point2I(x, y);
							TileDataInstance tile = e.SampleTile;
							if (tile != null)
								tileAction.AddOverwrittenTile(tile);
						}
					}

					EditorControl.PushAction(tileAction, ActionExecution.Execute);
					tileAction = null;
				}
			}
		}

		protected override void OnMouseDragBegin(ToolEventArgs e) {
			if (!ActionMode && IsTileSingle && e.IsLeftOrRight) {
				IsDrawing = true;
				drawTileData = GetTileData();
				if (DragButton == MouseButton.Left) {
					drawTile = CreateDrawTile();
				}
				else {
					drawTileData	= null;
					drawTile		= null;
				}
				tileAction			= ActionPlace.CreatePlaceAction(Level, Layer, drawTileData);
				overwrittenTiles	= new TileLocationHashSet(Layer);
				placedTiles			= new TileLocationHashSet(Layer);
				OnMouseDragMove(e);
			}
		}

		protected override void OnMouseDragEnd(ToolEventArgs e) {
			if (IsDrawing) {
				EditorControl.PushAction(tileAction, ActionExecution.Execute);
				tileAction			= null;
				overwrittenTiles	= null;
				placedTiles			= null;
				IsDrawing			= false;
			}
		}

		protected override void OnMouseDragMove(ToolEventArgs e) {
			if (IsDrawing) {
				//Point2I levelCoord = LevelDisplay.SampleLevelCoord(e.Position);
				//Room room = LevelDisplay.SampleRoom(e.Position);
				if (Level.ContainsLevelCoord(e.LevelCoord) &&
					!placedTiles.Contains(e.LevelCoord))
				{
					placedTiles.Add(e.LevelCoord);
					tileAction.AddPlacedTile(e.LevelCoord);
					//TileDataInstance tile = LevelDisplay.GetTile(levelCoord, Layer);
					TileDataInstance tile = e.SampleTile;
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

		public override int Snapping {
			get {
				if (ActionMode)
					return 8;
				return base.Snapping;
			}
		}

		//-----------------------------------------------------------------------------
		// Overridden Drawing Methods
		//-----------------------------------------------------------------------------

		public override bool DrawHideTile(TileDataInstance tile, Room room,
			Point2I levelCoord, int layer)
		{
			if (IsDrawing && layer == Layer) {
				return (placedTiles.Contains(levelCoord) ||
						overwrittenTiles.Contains(levelCoord));
			}
			return false;
		}

		public override void DrawTile(Graphics2D g, Room room, Point2I position,
			Point2I levelCoord, int layer)
		{
			if (!ActionMode && layer == Layer) {
				if (!IsDrawing && levelCoord == CursorLevelCoord) {
					TileDataInstance tile = CreateDrawTile();
					if (tile != null)
						LevelDisplay.DrawTile(g, room, CreateDrawTile(), position,
							LevelDisplay.FadeAboveColor);
				}
				else if (IsDrawing && placedTiles.Contains(levelCoord) &&
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
					Room room = Level.LevelPositionToRoom(actionTile.Position);
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
				return new ActionTileDataInstance(actionTileData, CursorPosition);
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
