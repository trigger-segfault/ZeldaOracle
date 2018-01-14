using System;
using System.Collections.Generic;
using System.Diagnostics;
using Size = System.Drawing.Size;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using System.Windows.Threading;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game;
using ConscriptDesigner.Control;

namespace ConscriptDesigner.WinForms {
	public class SpriteSourcePreview : GraphicsDeviceControl {


		private DispatcherTimer dispatcherTimer;
		private float ticks;
		private bool animating;
		private SpriteBatch spriteBatch;
		
		private Point2I spriteSize;

		Stopwatch watch = new Stopwatch();
		double lastSeconds;
		Point2I hoverSprite;

		private string sourceName;
		private ISpriteSource source;

		private SpriteInfo[,] spriteGrid;

		private Point2I mouse;

		public SpriteSourcePreview() {
			this.sourceName = "";
			this.source = null;
			this.spriteBatch = null;
			this.ResizeRedraw = true;
			this.ticks = 0;
			this.animating = false;
			this.mouse = -Point2I.One;
			this.hoverSprite = -Point2I.One;

			MouseMove += OnMouseMove;
			MouseLeave += OnMouseLeave;

			watch = Stopwatch.StartNew();
			lastSeconds = 0;
		}

		private void OnMouseLeave(object sender, EventArgs e) {
			mouse = -Point2I.One;
			UpdateHoverSprite();
		}
		
		public event EventHandler HoverSpriteChanged;

		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			mouse = new Point2I(e.X + HorizontalScroll.Value, e.Y + VerticalScroll.Value);
			if (source != null)
				UpdateHoverSprite();
		}

		private void UpdateHoverSprite() {
			int indexX = mouse.X / (spriteSize.X + 1);
			int indexY = mouse.Y / (spriteSize.Y + 1);
			Point2I newHoverSprite = -Point2I.One;
			if (mouse >= Point2I.Zero && new Point2I(indexX, indexY) < source.Dimensions) {
				if (spriteGrid[indexX, indexY] != null)
					newHoverSprite = new Point2I(indexX, indexY);
			}
			if (newHoverSprite != hoverSprite) {
				hoverSprite = newHoverSprite;
				if (HoverSpriteChanged != null)
					HoverSpriteChanged(this, EventArgs.Empty);
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

		public void ClearSpriteSource() {
			source = null;
		}

		public void UpdateSpriteSource(string name, ISpriteSource source) {
			this.sourceName = name;
			this.source = source;
			spriteSize = Point2I.Zero;
			spriteGrid = new SpriteInfo[source.Width, source.Height];
			for (int x = 0; x < source.Width; x++) {
				for (int y = 0; y < source.Height; y++) {
					ISprite sprite = source.GetSprite(x, y);
					if (sprite != null) {
						spriteGrid[x, y] = new SpriteInfo("", sprite);
						spriteSize = GMath.Max(spriteSize, spriteGrid[x, y].Bounds.Size);
					}
				}
			}
			this.AutoScrollMinSize = new Size(
				(spriteSize.X + 1) * source.Width + 1,
				(spriteSize.Y + 1) * source.Height + 1);
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

		public Point2I SpriteSize {
			get { return spriteSize; }
		}

		public SpriteInfo HoverSprite {
			get {
				if (hoverSprite == -Point2I.One)
					return null;
				return spriteGrid[hoverSprite.X, hoverSprite.Y];
			}
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
			if (hoverSprite != -Point2I.One)
				hover = 1 + hoverSprite * (spriteSize + 1);
			if (source != null && spriteSize != Point2I.Zero) {
				if (GameData.PaletteShader != null && !GameData.PaletteShader.Effect.IsDisposed) {
					GameData.PaletteShader.EntityPalette = GameData.PAL_ENTITIES_DEFAULT;
					GameData.PaletteShader.TilePalette = GameData.PAL_TILES_DEFAULT;
					if (DesignerControl.PreviewZone != null && DesignerControl.PreviewZone.Palette != null)
						GameData.PaletteShader.TilePalette = DesignerControl.PreviewZone.Palette;
					GameData.PaletteShader.ApplyPalettes();
				}
				if (DesignerControl.PreviewZone != null) {
					settings.VariantID = DesignerControl.PreviewZone.ImageVariantID;
					settings.Styles = DesignerControl.PreviewZone.StyleDefinitions;
				}

				g.Begin(GameSettings.DRAW_MODE_DEFAULT);
				g.Translate(-HorizontalScroll.Value, -VerticalScroll.Value);
				for (int indexX = 0; indexX < source.Width; indexX++) {
					for (int indexY = 0; indexY < source.Height; indexY++) {
						SpriteInfo sprite = spriteGrid[indexX, indexY];
						if (sprite != null) {
							int x = 1 + indexX * (spriteSize.X + 1);
							int y = 1 + indexY * (spriteSize.Y + 1);
							g.DrawISprite(sprite.Sprite, settings, new Vector2F(x, y) - sprite.Bounds.Point);
						}
					}
				}
				if (hover != -Point2I.One) {
					Rectangle2I selectRect = new Rectangle2I(hover - 1, spriteSize + 2);
					g.DrawRectangle(selectRect, 1, Color.Black);
					g.DrawRectangle(selectRect.Inflated(1, 1), 1, Color.White);
					g.DrawRectangle(selectRect.Inflated(2, 2), 1, Color.Black);
				}
				g.End();
			}
		}
	}
}
