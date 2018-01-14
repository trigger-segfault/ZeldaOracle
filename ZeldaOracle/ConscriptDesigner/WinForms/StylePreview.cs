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
using ConscriptDesigner.Control;
using ConscriptDesigner.Util;

namespace ConscriptDesigner.WinForms {

	public class StylePreview : GraphicsDeviceControl {


		private DispatcherTimer dispatcherTimer;
		private float ticks;
		private bool animating;
		private SpriteBatch spriteBatch;

		private List<SpriteInfo> sprites;

		private int columns;

		private string styleGroup;
		private Point2I spriteSize;
		private Dictionary<string, List<SpriteInfo>> styleGroups;
		private List<string> orderedStyleGroups;

		private List<SpriteInfo> filteredSprites;

		private string filter;

		Stopwatch watch = new Stopwatch();
		double lastSeconds;
		SpriteInfo hoverSprite;

		private Point2I mouse;

		public StylePreview() {
			this.sprites = new List<SpriteInfo>();
			this.spriteBatch = null;
			this.styleGroup = "";
			this.styleGroups = new Dictionary<string, List<SpriteInfo>>();
			this.filter = "";
			this.filteredSprites = new List<SpriteInfo>();
			this.orderedStyleGroups = new List<string>();
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
		
		private void TimerUpdate() {
			if (animating)
				Invalidate();
			if (ClientSize.Width != AutoScrollMinSize.Width)
				UpdateHeight();
		}

		private void OnMouseLeave(object sender, EventArgs e) {
			mouse = -Point2I.One;
			UpdateHoverSprite();
		}

		public event EventHandler Refreshed;
		public event EventHandler HoverSpriteChanged;

		private void OnClientSizeChanged(object sender, EventArgs e) {
			HorizontalScroll.Value = 0;
			VerticalScroll.Value = 0;
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
			styleGroups.Clear();
			orderedStyleGroups.Clear();
			filteredSprites.Clear();
			UpdateHeight();
			if (Refreshed != null)
				Refreshed(this, EventArgs.Empty);
			UpdateHoverSprite();
		}

		public void RefreshList() {
			sprites.Clear();
			styleGroups.Clear();
			orderedStyleGroups.Clear();
			foreach (StyleGroupCollection collection in Resources.GetRegisteredStyles()) {
				AddStyleGroup(collection.Group);
				foreach (string style in collection.Styles) {
					AddStyle(collection.Group, style, collection.Preview);
				}
			}
			orderedStyleGroups.Sort((a, b) => AlphanumComparator.Compare(a, b, true));
			if (!styleGroups.ContainsKey(styleGroup) && orderedStyleGroups.Any()) {
				styleGroup = orderedStyleGroups[0];
			}
			UpdateStyleGroup(styleGroup);
			if (GameData.PaletteShader != null && !GameData.PaletteShader.Effect.IsDisposed) {
				GameData.PaletteShader.EntityPalette = GameData.PAL_ENTITIES_DEFAULT;
				GameData.PaletteShader.TilePalette = GameData.PAL_PRESENT;
				GameData.PaletteShader.ApplyPalettes();
			}
			if (Refreshed != null)
				Refreshed(this, EventArgs.Empty);
			UpdateHoverSprite();
		}

		private void AddStyleGroup(string styleGroup) {
			if (!styleGroups.ContainsKey(styleGroup)) {
				styleGroups.Add(styleGroup, new List<SpriteInfo>());
				orderedStyleGroups.Add(styleGroup);
			}
		}

		private void AddStyle(string styleGroup, string style, ISprite sprite) {
			SpriteInfo spr = new SpriteInfo(style, sprite);
			sprites.Add(spr);
			Point2I size = spr.Bounds.Size;
			styleGroups[styleGroup].Add(spr);
		}

		private List<SpriteInfo> GetStyleList() {
			if (styleGroups.ContainsKey(styleGroup))
				return styleGroups[styleGroup];
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

			AutoScrollMinSize = new Size(ClientSize.Width, height);
			HorizontalScroll.Value = 0;
			VerticalScroll.Value = 0;
			if (!animating)
				Invalidate();
		}

		public void UpdateStyleGroup(string styleGroup) {
			this.styleGroup = styleGroup;
			UpdateSpriteSize();
			UpdateFilter(filter);
		}

		private void UpdateSpriteSize() {
			spriteSize = Point2I.One;
			List<SpriteInfo> styles;
			if (styleGroups.TryGetValue(styleGroup, out styles)) {
				foreach (SpriteInfo style in styles) {
					spriteSize = GMath.Max(spriteSize, style.Bounds.Size);
				}
			}
		}

		public void UpdateFilter(string filter) {
			this.filter = filter;
			filteredSprites = new List<SpriteInfo>();
			if (HasFilter) {
				foreach (var spr in GetStyleList()) {
					if (spr.Name.Contains(filter)) {
						filteredSprites.Add(spr);
					}
				}
			}
			else {
				filteredSprites = GetStyleList();
			}
			UpdateHeight();
		}

		public string StyleGroup {
			get { return styleGroup; }
		}

		public bool HasFilter {
			get { return !string.IsNullOrWhiteSpace(filter); }
		}
		public string Filter {
			get { return filter; }
		}

		public IEnumerable<string> GetStyleGroups() {
			return orderedStyleGroups;
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
				string originalStyle = settings.Styles.Get(styleGroup);
				for (int i = startIndex; i < endIndex && i < sprites.Count; i++) {
					SpriteInfo sprite = sprites[i];
					int row = i / columns;
					int column = i % columns;
					int x = 1 + column * (spriteSize.X + 1);
					int y = 1 + row * (spriteSize.Y + 1);
					settings.Styles.Set(styleGroup, sprite.Name);
					g.DrawISprite(sprite.Sprite, settings, new Vector2F(x, y) - sprite.Bounds.Point);
					if (sprite == hoverSprite) {
						hover = new Point2I(x, y);
					}
				}
				if (originalStyle != null)
					settings.Styles.Set(styleGroup, originalStyle);
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
