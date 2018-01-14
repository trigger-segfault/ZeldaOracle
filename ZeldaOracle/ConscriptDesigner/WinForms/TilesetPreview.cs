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
	public class TilesetPreview : GraphicsDeviceControl {

		private DispatcherTimer dispatcherTimer;
		private float ticks;
		private bool animating;
		private SpriteBatch spriteBatch;

		private int columns;

		Stopwatch watch = new Stopwatch();
		double lastSeconds;

		private BaseTileData hoverTileData;
		private Point2I hoverIndex;
		private ITileset tileset;
		private string tilesetName;

		private Point2I mouse;

		public TilesetPreview() {
			this.spriteBatch = null;
			this.tileset = null;
			this.columns = 1;
			this.ResizeRedraw = true;
			this.ticks = 0;
			this.animating = false;
			this.mouse = -Point2I.One;
			this.hoverIndex = -Point2I.One;

			MouseMove += OnMouseMove;
			MouseLeave += OnMouseLeave;

			watch = Stopwatch.StartNew();
			lastSeconds = 0;
		}

		private void OnMouseLeave(object sender, EventArgs e) {
			mouse = -Point2I.One;
			UpdateHoverTileData();
		}

		public event EventHandler Refreshed;
		public event EventHandler HoverTileDataChanged;
		
		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			mouse = new Point2I(e.X + HorizontalScroll.Value, e.Y + VerticalScroll.Value);
			UpdateHoverTileData();
		}

		private void UpdateHoverTileData() {
			int indexX = mouse.X / (GameSettings.TILE_SIZE + 1);
			int indexY = mouse.Y / (GameSettings.TILE_SIZE + 1);
			hoverIndex = new Point2I(indexX, indexY);
			BaseTileData newHoverTileData = null;
			if (tileset != null && mouse >= Point2I.Zero && hoverIndex < tileset.Size)
				newHoverTileData = tileset.GetTileData(hoverIndex);
			if (newHoverTileData != hoverTileData) {
				hoverTileData = newHoverTileData;
				if (newHoverTileData == null)
					hoverIndex = -Point2I.One;
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

		public void ClearTileset() {
			tileset = null;
			this.AutoScrollMinSize = new Size(1, 1);
			this.HorizontalScroll.Value = 0;
			this.VerticalScroll.Value = 0;
			if (Refreshed != null)
				Refreshed(this, EventArgs.Empty);
			UpdateHoverTileData();
		}

		public void UpdateTileset(string name, ITileset tileset) {
			this.tilesetName = name;
			this.tileset = tileset;
			this.AutoScrollMinSize = new Size(
				(GameSettings.TILE_SIZE + 1) * tileset.Width + 1,
				(GameSettings.TILE_SIZE + 1) * tileset.Height + 1);
			this.HorizontalScroll.Value = 0;
			this.VerticalScroll.Value = 0;

			if (GameData.PaletteShader != null && !GameData.PaletteShader.Effect.IsDisposed) {
				GameData.PaletteShader.EntityPalette = GameData.PAL_ENTITIES_DEFAULT;
				GameData.PaletteShader.TilePalette = GameData.PAL_PRESENT;
				GameData.PaletteShader.ApplyPalettes();
			}
			if (!animating)
				Invalidate();
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

		public BaseTileData HoverTileData {
			get { return hoverTileData; }
		}
		public Point2I HoverIndex {
			get { return hoverIndex; }
		}

		protected override void Draw() {
			if (animating) {
				ticks += (float) ((watch.Elapsed.TotalSeconds - lastSeconds) * 60.0);
			}
			lastSeconds = watch.Elapsed.TotalSeconds;
			
			SpriteDrawSettings settings = new SpriteDrawSettings((float)ticks);
			Graphics2D g = new Graphics2D(spriteBatch);
			g.Clear(Color.White);
			Point2I hover = -Point2I.One;
			if (tileset != null) {
				TileDataDrawing.Extras = false;
				TileDataDrawing.Level = null;
				TileDataDrawing.PlaybackTime = ticks;
				TileDataDrawing.RewardManager = DesignerControl.RewardManager;

				if (GameData.PaletteShader != null && !GameData.PaletteShader.Effect.IsDisposed) {
					GameData.PaletteShader.EntityPalette = GameData.PAL_ENTITIES_DEFAULT;
					GameData.PaletteShader.TilePalette = GameData.PAL_TILES_DEFAULT;
					if (DesignerControl.PreviewZone != null && DesignerControl.PreviewZone.Palette != null)
						GameData.PaletteShader.TilePalette = DesignerControl.PreviewZone.Palette;
					GameData.PaletteShader.ApplyPalettes();
				}

				Zone zone = (DesignerControl.PreviewZone ?? new Zone());

				g.Begin(GameSettings.DRAW_MODE_DEFAULT);
				g.Translate(-HorizontalScroll.Value, -VerticalScroll.Value);
				int startRow = (VerticalScroll.Value + 1) / (GameSettings.TILE_SIZE + 1);
				int startIndex = startRow * columns;
				int endRow = (VerticalScroll.Value + ClientSize.Height + 1 + GameSettings.TILE_SIZE) / (GameSettings.TILE_SIZE + 1);
				int endIndex = (endRow + 1) * columns;
				for (int tx = 0; tx < tileset.Width; tx++) {
					for (int ty = startRow; ty < endRow && ty < tileset.Height; ty++) {
						BaseTileData tile = tileset.GetTileData(tx, ty);
						int x = 1 + tx * (GameSettings.TILE_SIZE + 1);
						int y = 1 + ty * (GameSettings.TILE_SIZE + 1);

						try {
							TileDataDrawing.DrawTile(g, tile, new Point2I(x, y), zone);
						}
						catch (Exception) {

						}
						if (tile == hoverTileData) {
							hover = new Point2I(x, y);
						}
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
