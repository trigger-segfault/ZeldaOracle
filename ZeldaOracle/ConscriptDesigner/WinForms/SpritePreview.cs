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
using ZeldaOracle.Game.Worlds;
using ConscriptDesigner.Control;

namespace ConscriptDesigner.WinForms {

	public class SpriteInfo {
		public string Name { get; set; }
		public ISprite Sprite { get; set; }
		public Rectangle2I Bounds { get; set; }
		
		public int SubstripIndex { get; set; }

		public bool HasSubstrips {
			get {
				if (Sprite is Animation)
					return (SubstripIndex != 0 || ((Animation) Sprite).HasSubstrips);
				return false;
			}
		}

		public SpriteInfo(string spriteName, ISprite sprite, int substripIndex = 0) {
			this.Name = spriteName;
			this.Sprite = sprite;
			this.Bounds = this.Sprite.Bounds;
			this.SubstripIndex = substripIndex;
		}
	}

	public class SpritePreview : GraphicsDeviceControl {


		private DispatcherTimer dispatcherTimer;
		private float ticks;
		private bool animating;
		private SpriteBatch spriteBatch;

		private List<SpriteInfo> sprites;

		private int columns;

		private Point2I spriteSize;
		private Dictionary<Point2I, List<SpriteInfo>> spriteSizes;
		private List<Point2I> orderedSpriteSizes;

		private List<SpriteInfo> filteredSprites;

		private string filter;
		
		Stopwatch watch = new Stopwatch();
		double lastSeconds;
		SpriteInfo hoverSprite;

		private Point2I mouse;

		public SpritePreview() {
			this.sprites = new List<SpriteInfo>();
			this.spriteBatch = null;
			this.spriteSizes = new Dictionary<Point2I, List<SpriteInfo>>();
			this.filter = "";
			this.filteredSprites = new List<SpriteInfo>();
			this.orderedSpriteSizes = new List<Point2I>();
			this.columns = 1;
			this.ResizeRedraw = true;
			this.AutoScroll = true;
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
			UpdateHoverSprite();
		}

		public event EventHandler Refreshed;
		public event EventHandler HoverSpriteChanged;

		private void OnClientSizeChanged(object sender, EventArgs e) {
			this.HorizontalScroll.Value = 0;
			this.VerticalScroll.Value = 0;
		}

		private void TimerUpdate() {
			if (animating)
				Invalidate();
			if (ClientSize.Width != AutoScrollMinSize.Width)
				UpdateHeight();
		}

		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			mouse = new Point2I(e.X + HorizontalScroll.Value, e.Y + VerticalScroll.Value);
			UpdateHoverSprite();
		}

		private void UpdateHoverSprite() {
			int column = mouse.X / (spriteSize.X + 1);
			int row = mouse.Y / (spriteSize.Y + 1);
			int index = row * columns + column;
			SpriteInfo newHoverSprite = null;
			if (mouse >= Point2I.Zero && index < filteredSprites.Count) {
				newHoverSprite = filteredSprites[index];
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
				delegate { TimerUpdate(); },
				System.Windows.Application.Current.Dispatcher);
		}

		public void ClearList() {
			sprites.Clear();
			spriteSizes.Clear();
			orderedSpriteSizes.Clear();
			filteredSprites.Clear();
			UpdateHeight();
			if (Refreshed != null)
				Refreshed(this, EventArgs.Empty);
			UpdateHoverSprite();
		}

		public void RefreshList() {
			sprites.Clear();
			spriteSizes.Clear();
			orderedSpriteSizes.Clear();
			foreach (var pair in Resources.GetResourceDictionary<ISprite>()) {
				ISprite sprite = pair.Value;
				if (sprite is Animation) {
					Animation anim = sprite as Animation;
					int substripIndex = 0;
					Animation substrip = anim;
					do {
						AddSprite(pair.Key, substrip, substripIndex);
						substrip = substrip.NextStrip;
						substripIndex++;
					} while (substrip != null);
				}
				else {
					AddSprite(pair.Key, sprite);
				}
			}
			orderedSpriteSizes.Sort((p1, p2) => (p1.X + p1.Y) - (p2.X + p2.Y));
			if (!spriteSizes.ContainsKey(spriteSize) && orderedSpriteSizes.Any()) {
				spriteSize = orderedSpriteSizes[0];
			}
			UpdateSpriteSize(spriteSize);
			if (GameData.PaletteShader != null && !GameData.PaletteShader.Effect.IsDisposed) {
				GameData.PaletteShader.EntityPalette = GameData.PAL_ENTITIES_DEFAULT;
				GameData.PaletteShader.TilePalette = GameData.PAL_PRESENT;
				GameData.PaletteShader.ApplyPalettes();
			}
			if (Refreshed != null)
				Refreshed(this, EventArgs.Empty);
			UpdateHoverSprite();
		}

		private Point2I RoundSize(Point2I size) {
			return ((size + 7) / 8) * 8;
		}

		private void AddSprite(string name, ISprite sprite, int substripIndex = 0) {
			SpriteInfo spr = new SpriteInfo(name, sprite, substripIndex);
			sprites.Add(spr);
			Point2I size = RoundSize(spr.Bounds.Size);
			if (!spriteSizes.ContainsKey(size)) {
				spriteSizes.Add(size, new List<SpriteInfo>());
				orderedSpriteSizes.Add(size);
			}
			spriteSizes[size].Add(spr);
		}

		private List<SpriteInfo> GetSizeList() {
			if (spriteSizes.ContainsKey(spriteSize))
				return spriteSizes[spriteSize];
			return new List<SpriteInfo>();
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
			columns = Math.Max(1, (ClientSize.Width - 1) / (spriteSize.X + 1));
			int height = ((filteredSprites.Count + columns - 1) / columns) * (spriteSize.Y + 1);
			
			this.AutoScrollMinSize = new Size(ClientSize.Width, height);
			HorizontalScroll.Value = 0;
			VerticalScroll.Value = 0;
			if (!animating)
				Invalidate();
		}

		public void UpdateSpriteSize(Point2I spriteSize) {
			this.spriteSize = spriteSize;
			UpdateFilter(filter);
		}

		public void UpdateFilter(string filter) {
			this.filter = filter;
			filteredSprites = new List<SpriteInfo>();
			if (HasFilter) {
				foreach (var spr in GetSizeList()) {
					if (spr.Name.Contains(filter)) {
						filteredSprites.Add(spr);
					}
				}
			}
			else {
				filteredSprites = GetSizeList();
			}
			UpdateHeight();
		}

		public Point2I SpriteSize {
			get { return spriteSize; }
		}

		public bool HasFilter {
			get { return !string.IsNullOrWhiteSpace(filter); }
		}
		public string Filter {
			get { return filter; }
		}

		public IEnumerable<Point2I> GetSpriteSizes() {
			return orderedSpriteSizes;
		}

		public SpriteInfo HoverSprite {
			get { return hoverSprite; }
		}

		protected override void Draw() {
			if (animating) {
				ticks += (float) ((watch.Elapsed.TotalSeconds - lastSeconds) * 60.0);
			}
			lastSeconds = watch.Elapsed.TotalSeconds;

			List<SpriteInfo> sprites = filteredSprites;
			SpriteDrawSettings settings = new SpriteDrawSettings((float)ticks);
			Graphics2D g = new Graphics2D(spriteBatch);
			g.Clear(Color.White);
			Point2I hover = -Point2I.One;
			if (sprites.Any()) {

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
				int startRow = (VerticalScroll.Value + 1) / (spriteSize.Y + 1);
				int startIndex = startRow * columns;
				int endRow = (VerticalScroll.Value + ClientSize.Height + 1 + spriteSize.Y) / (spriteSize.Y + 1);
				int endIndex = (endRow + 1) * columns;
				for (int i = startIndex; i < endIndex && i < sprites.Count; i++) {
					SpriteInfo sprite = sprites[i];
					int row = i / columns;
					int column = i % columns;
					int x = 1 + column * (spriteSize.X + 1);
					int y = 1 + row * (spriteSize.Y + 1);
					g.DrawISprite(sprite.Sprite, settings, new Vector2F(x, y) - sprite.Bounds.Point);
					if (sprite == hoverSprite) {
						hover = new Point2I(x, y);
					}
				}
				if (hover != -Point2I.One) {
					Rectangle2I selectRect = new Rectangle2I(hover - 1, hoverSprite.Bounds.Size + 2);
					g.DrawRectangle(selectRect, 1, Color.Black);
					g.DrawRectangle(selectRect.Inflated(1, 1), 1, Color.White);
					g.DrawRectangle(selectRect.Inflated(2, 2), 1, Color.Black);
				}
				g.End();
			}
		}
	}
}
