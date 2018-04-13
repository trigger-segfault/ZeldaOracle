using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Worlds;

namespace ConscriptDesigner.WinForms {

	public class StylePreview : ZeldaGraphicsDeviceControl {
		
		private Point2I spriteSize;
		private List<SpriteInfo> styles;

		private string styleGroup;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public StylePreview() {
			this.styles = new List<SpriteInfo>();
			this.spriteSize = Point2I.One;
			this.styleGroup = "";
		}

		protected override void Initialize() {
			base.Initialize();
		}


		//-----------------------------------------------------------------------------
		// Loading/Updating
		//-----------------------------------------------------------------------------

		public void UpdateList(List<SpriteInfo> styles, Point2I spriteSize, string styleGroup) {
			this.styles = styles;
			this.spriteSize = spriteSize;
			this.styleGroup = styleGroup;
			UpdateHeight();
		}

		public void UpdateSpriteSize(Point2I spriteSize) {
			this.spriteSize = spriteSize;
			UpdateHeight();
		}

		public void UpdateScale() {
			UpdateHeight();
		}

		public void Unload() {
			styles.Clear();
			UpdateSize(Point2I.One);
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		protected override void TimerUpdate() {
			base.TimerUpdate();
			if (ClientWidth != AutoScrollMinSize.Width)
				UpdateHeight();
		}

		protected override bool IsValidHoverPoint(ref Point2I point, out Point2I hoverSize) {
			int index = (point.Y * columns) + point.X;
			hoverSize = Point2I.One;
			return index < styles.Count;
		}

		protected override void UpdateHeight() {
			columns = Math.Max(1, (UnscaledClientSize.X - 1) / (spriteSize.X + 1));
			int height = 1 + ((styles.Count + columns - 1) / columns) * (spriteSize.Y + 1);

			UpdateSize(new Point2I(UnscaledClientSize.X, height));
		}

		protected override void Draw(Graphics2D g, SpriteSettings settings, Zone zone) {
			string originalStyle = settings.Styles.Get(styleGroup);

			int startRow = (UnscaledScrollPosition.Y + 1) / (spriteSize.Y + 1);
			int startIndex = startRow * columns;
			int endRow = (UnscaledScrollPosition.Y + UnscaledClientSize.Y + 1 + spriteSize.Y) / (spriteSize.Y + 1);
			int endIndex = (endRow + 1) * columns;
			for (int i = startIndex; i < endIndex && i < styles.Count; i++) {
				SpriteInfo sprite = styles[i];
				int row = i / columns;
				int column = i % columns;
				int x = 1 + column * (spriteSize.X + 1);
				int y = 1 + row * (spriteSize.Y + 1);
				settings.Styles.Set(styleGroup, sprite.Name);
				g.DrawSprite(sprite.Sprite, settings, new Vector2F(x, y) - sprite.Bounds.Point);
			}

			if (originalStyle != null)
				settings.Styles.Set(styleGroup, originalStyle);
			else
				settings.Styles.Remove(styleGroup);
		}


		//-----------------------------------------------------------------------------
		// Override Properties
		//-----------------------------------------------------------------------------

		protected override Point2I BaseSpriteSize {
			get { return spriteSize; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SpriteInfo HoverSprite {
			get {
				if (hoverPoint == -Point2I.One)
					return null;
				int index = (hoverPoint.Y * columns) + hoverPoint.X;
				return styles[index];
			}
		}
	}
}
