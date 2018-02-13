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
	public class SpritePreview : ZeldaGraphicsDeviceControl {
		
		private List<SpriteInfo> sprites;
		private Point2I spriteSize;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SpritePreview() {
			this.sprites = new List<SpriteInfo>();
			this.spriteSize = Point2I.One;
		}

		protected override void Initialize() {
			base.Initialize();
		}


		//-----------------------------------------------------------------------------
		// Loading/Updating
		//-----------------------------------------------------------------------------

		public void UpdateList(List<SpriteInfo> sprites, Point2I spriteSize) {
			this.sprites = sprites;
			this.spriteSize = spriteSize;
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
			sprites.Clear();
			UpdateSize(Point2I.One);
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		protected override void TimerUpdate() {
			base.TimerUpdate();
			if (UnscaledClientSize.X * DesignerControl.PreviewScale != AutoScrollMinSize.Width)
				UpdateHeight();
		}

		protected override bool IsValidHoverPoint(ref Point2I point, out Point2I hoverSize) {
			int index = (point.Y * columns) + point.X;
			hoverSize = Point2I.One;
			return index < sprites.Count;
		}

		protected override void UpdateHeight() {
			columns = Math.Max(1, (UnscaledClientSize.X - 1) / (spriteSize.X + 1));
			int height = 1 + ((sprites.Count + columns - 1) / columns) * (spriteSize.Y + 1);
			
			UpdateSize(new Point2I(UnscaledClientSize.X, height));
		}
		
		protected override void Draw(Graphics2D g, SpriteSettings settings, Zone zone) {
			int startRow = (UnscaledScrollPosition.Y + 1) / (spriteSize.Y + 1);
			int startIndex = startRow * columns;
			int endRow = (UnscaledScrollPosition.Y + UnscaledClientSize.Y + 1 + spriteSize.Y) / (spriteSize.Y + 1);
			int endIndex = (endRow + 1) * columns;
			for (int i = startIndex; i < endIndex && i < sprites.Count; i++) {
				SpriteInfo sprite = sprites[i];
				int row = i / columns;
				int column = i % columns;
				int x = 1 + column * (spriteSize.X + 1);
				int y = 1 + row * (spriteSize.Y + 1);
				g.DrawSprite(sprite.Sprite, settings, new Vector2F(x, y) - sprite.Bounds.Point);
			}
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
				return sprites[index];
			}
		}
	}
}
