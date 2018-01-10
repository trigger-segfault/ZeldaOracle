using System;
using System.Collections.Generic;
using Size = System.Drawing.Size;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game;
using System.Windows.Threading;
using System.Diagnostics;
using ZeldaOracle.Game.Tiles;
using ConscriptDesigner.Control;
using ZeldaOracle.Game.Worlds;

namespace ConscriptDesigner.WinForms {
	public class TileDataPreview : GraphicsDeviceControl {

		private DispatcherTimer dispatcherTimer;
		private float ticks;
		private bool animating;
		private SpriteBatch spriteBatch;

		private int columns;
		

		private List<BaseTileData> filteredTileData;

		private string filter;

		Stopwatch watch = new Stopwatch();
		double lastSeconds;

		private BaseTileData hoverTileData;

		private List<BaseTileData> tileData;

		private Point2I mouse;

		public TileDataPreview() {
			this.spriteBatch = null;
			this.filter = "";
			this.filteredTileData = new List<BaseTileData>();
			this.tileData = new List<BaseTileData>();
			this.columns = 1;
			this.ResizeRedraw = true;
			this.ticks = 0;
			this.animating = false;
			this.mouse = -Point2I.One;
			
			MouseMove += OnMouseMove;
			MouseLeave += OnMouseLeave;
			ClientSizeChanged += OnClientSizeChanged;

			watch = Stopwatch.StartNew();
			lastSeconds = 0;
		}

		private void OnMouseLeave(object sender, EventArgs e) {
			mouse = -Point2I.One;
			UpdateHoverTileData();
		}

		public event EventHandler Refreshed;
		public event EventHandler HoverTileDataChanged;

		private void OnClientSizeChanged(object sender, EventArgs e) {
			UpdateHeight();
		}

		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			mouse = new Point2I(e.X + HorizontalScroll.Value, e.Y + VerticalScroll.Value);
			UpdateHoverTileData();
		}

		private void UpdateHoverTileData() {
			int column = mouse.X / (GameSettings.TILE_SIZE + 1);
			int row = mouse.Y / (GameSettings.TILE_SIZE + 1);
			int index = row * columns + column;
			BaseTileData newHoverTileData = null;
			if (mouse >= Point2I.Zero && index < filteredTileData.Count) {
				newHoverTileData = filteredTileData[index];
			}
			if (newHoverTileData != hoverTileData) {
				hoverTileData = newHoverTileData;
				if (HoverTileDataChanged != null)
					HoverTileDataChanged(this, EventArgs.Empty);
				if (!animating)
					Invalidate();
			}
		}

		protected override void Initialize() {
			this.spriteBatch = new SpriteBatch(GraphicsDevice);


			this.dispatcherTimer = new DispatcherTimer(
				TimeSpan.FromMilliseconds(15),
				DispatcherPriority.Render,
				delegate { if (animating) Invalidate(); },
				System.Windows.Application.Current.Dispatcher);
		}

		public void ClearList() {
			tileData.Clear();
			filteredTileData.Clear();
			UpdateHeight();
			if (Refreshed != null)
				Refreshed(this, EventArgs.Empty);
			UpdateHoverTileData();
		}

		public void RefreshList() {
			tileData.Clear();
			filteredTileData.Clear();
			foreach (var pair in Resources.GetResourceDictionary<BaseTileData>()) {
				tileData.Add(pair.Value);
			}
			UpdateFilter(filter);
			if (GameData.PaletteShader != null && !GameData.PaletteShader.Effect.IsDisposed) {
				GameData.PaletteShader.EntityPalette = GameData.PAL_ENTITIES_DEFAULT;
				GameData.PaletteShader.TilePalette = GameData.PAL_PRESENT;
				GameData.PaletteShader.ApplyPalettes();
			}
			if (Refreshed != null)
				Refreshed(this, EventArgs.Empty);
			UpdateHoverTileData();
		}

		private Point2I RoundSize(Point2I size) {
			return ((size + 7) / 8) * 8;
		}
		

		public bool Animating {
			get { return animating; }
			set {
				if (animating != value) {
					animating = value;
					ticks = 0;
					if (!animating)
						Invalidate();
				}
			}
		}

		public void RestartAnimations() {
			ticks = 0;
		}

		private void UpdateHeight() {
			columns = Math.Max(1, (ClientSize.Width - 1) / (GameSettings.TILE_SIZE + 1));
			int height = ((filteredTileData.Count + columns - 1) / columns) * (GameSettings.TILE_SIZE + 1);
			if (!animating)
				Invalidate();

			this.AutoScrollMinSize = new Size(ClientSize.Width, height);
			this.HorizontalScroll.Value = 0;
			this.VerticalScroll.Value = 0;
		}

		public void UpdateFilter(string filter) {
			this.filter = filter;
			filteredTileData = new List<BaseTileData>();
			if (HasFilter) {
				foreach (var tile in tileData) {
					if (tile.Name.Contains(filter)) {
						filteredTileData.Add(tile);
					}
				}
			}
			else {
				filteredTileData = tileData;
			}
			UpdateHeight();
		}

		public bool HasFilter {
			get { return !string.IsNullOrWhiteSpace(filter); }
		}
		public string Filter {
			get { return filter; }
		}
		
		public BaseTileData HoverTileData {
			get { return hoverTileData; }
		}

		protected override void Draw() {
			if (animating) {
				ticks += (float) ((watch.Elapsed.TotalSeconds - lastSeconds) * 60.0);
			}
			lastSeconds = watch.Elapsed.TotalSeconds;

			List<BaseTileData> tileData = filteredTileData;
			SpriteDrawSettings settings = new SpriteDrawSettings((float)ticks);
			Graphics2D g = new Graphics2D(spriteBatch);
			g.Clear(Color.White);
			Point2I hover = -Point2I.One;
			if (tileData.Any()) {
				TileDataDrawing.Extras = false;
				TileDataDrawing.Level = null;
				TileDataDrawing.PlaybackTime = ticks;
				TileDataDrawing.RewardManager = DesignerControl.RewardManager;
				Zone zone = GameData.ZONE_PRESENT;
				g.Begin(GameSettings.DRAW_MODE_DEFAULT);
				g.Translate(-HorizontalScroll.Value, -VerticalScroll.Value);
				int startRow = (VerticalScroll.Value + 1) / (GameSettings.TILE_SIZE + 1);
				int startIndex = startRow * columns;
				int endRow = (VerticalScroll.Value + ClientSize.Height + 1 + GameSettings.TILE_SIZE) / (GameSettings.TILE_SIZE + 1);
				int endIndex = (endRow + 1) * columns;
				for (int i = startIndex; i < endIndex && i < tileData.Count; i++) {
					BaseTileData tile = tileData[i];
					int row = i / columns;
					int column = i % columns;
					int x = 1 + column * (GameSettings.TILE_SIZE + 1);
					int y = 1 + row * (GameSettings.TILE_SIZE + 1);

					try {
						TileDataDrawing.DrawTile(g, tile, new Point2I(x, y), zone);
					}
					catch (Exception ex) {

					}
					//g.DrawISprite(tile.Sprite, settings);
					if (tile == hoverTileData) {
						hover = new Point2I(x, y);
					}
				}
				if (hover != -Point2I.One) {
					Rectangle2I selectRect = new Rectangle2I(hover - 1, (Point2I)GameSettings.TILE_SIZE + 2);
					g.DrawRectangle(selectRect, 1, Color.Black);
					g.DrawRectangle(selectRect.Inflated(1, 1), 1, Color.White);
					g.DrawRectangle(selectRect.Inflated(2, 2), 1, Color.Black);
				}
				g.End();
			}
		}
	}
}
