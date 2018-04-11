using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Xna.Framework.Content;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Worlds;
using ZeldaWpf.Util;
using ZeldaWpf.WinForms;
using FrameworkElement = System.Windows.FrameworkElement;

namespace ZeldaEditor.WinForms {
	public class TilePathDisplay : TimersGraphicsDeviceControl {

		//-----------------------------------------------------------------------------
		// Classes
		//-----------------------------------------------------------------------------

		private class TilePathInfo {
			public TileDataInstance Tile { get; set; }
			public TilePath Path { get; set; }
			public int Index { get; set; }
			public int Timer { get; set; }
			public Vector2F Position { get; set; }
			public Vector2F Destination { get; set; }
			public TilePathMove Move {
				get {
					if (Path != null && Index != Path.Moves.Count)
						return Path.Moves[Index];
					return null;
				}
			}

			public TilePathInfo(TileDataInstance tile) {
				Tile = tile;
				Path = TilePath.Parse(Tile.Properties.Get<string>("path", ""));
				Index = 0;
				Timer = 0;
				Position = Tile.Location * GameSettings.TILE_SIZE;
				Destination = Position;
				if (Move != null) {
					Index = -1;
					NextMove();
				}
			}

			public void NextMove() {
				Position = Destination;
				Timer = 0;
				Index++;
				if (Index >= Path.Moves.Count) {
					if (Path.Repeats)
						Index = 0;
					else {
						Index = Path.Moves.Count;
						return;
					}
				}
				if (Move.Direction.IsValid && Move.Distance >= 0) {
					Destination = GMath.Round(
							Position + Move.Direction.ToVector(
							Move.Distance * GameSettings.TILE_SIZE),
							GameSettings.TILE_SIZE);
				}
			}

			public void Restart() {
				Path = TilePath.Parse(Tile.Properties.Get<string>("path", ""));
				Index = 0;
				Position = Tile.Location * GameSettings.TILE_SIZE;
				Destination = Position;
				if (Move != null) {
					Index = -1;
					NextMove();
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		public readonly Color FadeAboveColor = new Color(200, 200, 200, 100);
		public readonly Color FadeBelowColor = new Color(200, 200, 200, 150);

		private EditorControl    editorControl;

		private int lastTicks;
		private Room room;

		private TilePathInfo currentPath;
		private TileDataInstance tile;

		private List<TilePathInfo> paths;
		private HashSet<TileDataInstance> pathTiles;
		private Stopwatch watch;

		private static bool runAllPaths = true;
		private static bool highlightPath = true;
		private static bool fadeTiles = true;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public TilePathDisplay(FrameworkElement element, Room room,
			TileDataInstance tile) : base(element)
		{
			this.room = room;
			this.tile = tile;
			if (tile != null)
				this.currentPath = new TilePathInfo(tile);
			this.paths = new List<TilePathInfo>();
			this.pathTiles = new HashSet<TileDataInstance>();

			Size = room.PixelSize.ToGdiSize();

			foreach (var tileData in room.GetTiles()) {
				if (tileData == tile || !string.IsNullOrWhiteSpace(
					tileData.Properties.Get<string>("path", "")))
				{
					if (tileData == tile)
						paths.Add(currentPath);
					else
						paths.Add(new TilePathInfo(tileData));
					pathTiles.Add(tileData);
				}
			}
		}

		protected override void Initialize() {
			
			this.ResizeRedraw = true;

			ContinuousEvents.StartRender(
				() => {
					if (editorControl.IsActive)
						Invalidate();
				});

			watch = Stopwatch.StartNew();
		}


		//-----------------------------------------------------------------------------
		// Playback
		//-----------------------------------------------------------------------------

		public void Restart() {
			foreach (TilePathInfo info in paths) {
				info.Restart();
			}
		}

		private void Update(int count) {
			for (int i = 0; i < count; i++) {
				foreach (TilePathInfo path in paths) {
					UpdatePath(path);
				}
			}
		}

		private void UpdatePath(TilePathInfo info) {
			TilePath path = info.Path;
			TilePathMove move = info.Move;
			if (path == null || move == null)
				return;
			
			// Skip any move operations with no distance.
			// Make sure no extra frames are paused for.
			if (info.Timer >= move.Delay) {
				for (int i = 0; i < path.Moves.Count && move != null && !move.HasMovement; i++) {
					info.NextMove();
					move = info.Move;
					if (move == null)
						return;
					// It's not a dead move command
					if (move.Delay > 0)
						break;
				}
			}

			// Begin the next move in the path after the delay has been passed.
			if (info.Timer >= move.Delay) {
				
				int axis = move.Direction.Axis;

				Vector2F velocity = move.Direction.ToVector(move.Speed);
				Vector2F nextPosition = info.Position + velocity;
				Vector2F distanceLeft = (info.Destination - nextPosition) * move.Direction.ToVector();

				// Have we surpassed our destination?
				if (distanceLeft[axis] <= 0) {
					info.NextMove();
				}
				else {
					info.Position = nextPosition;
				}
			}
			else {
				info.Timer++;
			}
		}

		protected override void Draw() {
			if (!Resources.IsInitialized) return;
			if (!editorControl.IsInitialized)
				return;

			int currentTicks = (int)(watch.Elapsed.TotalSeconds * 60);
			Update(Math.Min(60, currentTicks - lastTicks));
			lastTicks = currentTicks;
			
			TileDataDrawing.PlaybackTime = currentTicks;
			TileDataDrawing.Extras = false;
			TileDataDrawing.Level = room.Level;
			TileDataDrawing.Room = room;

			GameData.SHADER_PALETTE.TilePalette = room.Zone.Palette;
			GameData.SHADER_PALETTE.ApplyParameters();

			Graphics2D g = new Graphics2D();
			g.Begin(GameSettings.DRAW_MODE_PALLETE);
			g.Clear(Color.White);

			DrawRoomTiles(g);
			DrawTilePath(g, currentPath);
			DrawPathTiles(g);
			DrawRoomActions(g);
			
			g.End();
		}

		private void DrawRoomTiles(Graphics2D g) {
			Color color = FadeTiles ? FadeBelowColor : Color.White;

			// Draw tile layers.
			for (int layer = 0; layer < room.LayerCount; layer++) {
				// Draw the tile grid for this layer.
				for (int x = 0; x < room.Width; x++) {
					for (int y = 0; y < room.Height; y++) {
						TileDataInstance tile = room.GetTile(x, y, layer);
						Point2I position = new Point2I(x, y) * GameSettings.TILE_SIZE;
						if (tile != null && tile.IsAtLocation(x, y) && !IsPathTile(tile)) {
							TileDataDrawing.DrawTileObject(g, tile, position, room.Zone, color);
						}
					}
				}
			}
		}

		private void DrawPathTiles(Graphics2D g) {
			foreach (TilePathInfo info in paths) {
				if (!IsPathTile(info.Tile))
					continue;
				TileDataDrawing.DrawTile(g, info.Tile,
					(Point2I) GameUtil.Bias(info.Position),
					room.Zone, Color.White);
			}
		}

		private void DrawTilePath(Graphics2D g, TilePathInfo info) {
			if (!HighlightPath || info.Path == null)
				return;

			Point2I start = info.Tile.Location * GameSettings.TILE_SIZE + GameSettings.TILE_SIZE / 2;
			Point2I currentBlack = start;
			Point2I currentWhite = start;
			for (int i = 0; i < info.Path.Moves.Count + 3; i++) {
				if (i < info.Path.Moves.Count) {
					TilePathMove move = info.Path.Moves[i % info.Path.Moves.Count];
					Point2I next = currentBlack + move.Direction.ToPoint(
						move.Distance * GameSettings.TILE_SIZE);
					//if (i == info.Path.Moves.Count)
					//	next = currentBlack;
					Point2I min = GMath.Min(currentBlack, next) - 2;
					Point2I max = GMath.Max(currentBlack, next) + 1;
					g.FillRectangle(Rectangle2F.FromEndPoints(min, max), Color.Black);
					currentBlack = next;
				}

				if (i >= 2) {
					TilePathMove move = info.Path.Moves[(i - 2) % info.Path.Moves.Count];
					Point2I next = currentWhite + move.Direction.ToPoint(
						move.Distance * GameSettings.TILE_SIZE);
					// Connect the first and last movements
					if (i == info.Path.Moves.Count + 2) {
						// Path doesn't reconnect, end the loop
						if (currentWhite != start || !info.Path.Repeats)
							break;
						next = currentWhite + move.Direction.ToPoint(
							move.Distance * GameSettings.TILE_SIZE / 2);
					}
					Point2I min = GMath.Min(currentWhite, next) - 1;
					Point2I max = GMath.Max(currentWhite, next);
					g.FillRectangle(Rectangle2F.FromEndPoints(min, max), Color.White);
					currentWhite = next;
				}
			}
		}

		private void DrawRoomActions(Graphics2D g) {
			// Draw action tiles.
			Color color = FadeTiles ? FadeAboveColor : Color.White;
			foreach (ActionTileDataInstance action in
				room.GetActionTiles(editorControl.ShowShared))
			{
				TileDataDrawing.DrawTile(g, action, action.Position, room.Zone, color);
			}
		}

		private bool IsPathTile(TileDataInstance tile) {
			if (RunAllPaths)
				return pathTiles.Contains(tile);
			else
				return (tile == this.tile);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorControl EditorControl {
			get { return editorControl; }
			set { editorControl = value; }
		}

		public bool RunAllPaths {
			get { return runAllPaths || tile == null; }
			set {
				runAllPaths = value;
				if (paths.Count > 1)
					Restart();
			}
		}

		public bool HighlightPath {
			get { return highlightPath && tile != null; }
			set { highlightPath = value; }
		}

		public bool FadeTiles {
			get { return fadeTiles; }
			set { fadeTiles = value; }
		}
	}
}
