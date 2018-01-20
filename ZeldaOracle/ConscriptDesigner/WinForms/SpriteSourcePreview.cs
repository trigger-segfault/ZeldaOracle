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
using ZeldaOracle.Game.Worlds;

namespace ConscriptDesigner.WinForms {
	public class SpriteSourcePreview : ZeldaGraphicsDeviceControl {
		
		private SpriteInfo[,] spriteGrid;
		private ISpriteSource source;
		private Point2I spriteSize;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SpriteSourcePreview() {
			this.spriteGrid = new SpriteInfo[0, 0];
			this.spriteSize = Point2I.One;
		}

		protected override void Initialize() {
			base.Initialize();
		}


		//-----------------------------------------------------------------------------
		// Loading/Updating
		//-----------------------------------------------------------------------------

		public void UpdateList(SpriteInfo[,] spriteGrid, ISpriteSource source, Point2I spriteSize) {
			this.spriteGrid = spriteGrid;
			this.source = source;
			this.spriteSize = spriteSize;
			UpdateHeight();
		}

		public void UpdateScale() {
			UpdateHeight();
		}

		public void Unload() {
			source = null;
			spriteGrid = new SpriteInfo[0, 0];
			spriteSize = Point2I.One;
			UpdateHeight();
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		protected override void TimerUpdate() {
			base.TimerUpdate();
		}

		protected override void UpdateHeight() {
			if (source != null) {
				columns = source.Width;
				UpdateSize(source.Dimensions * (spriteSize + 1) + 1);
			}
			else {
				columns = 1;
				UpdateSize(Point2I.One);
			}
		}

		protected override bool IsValidHoverPoint(ref Point2I point, out Point2I hoverSize) {
			hoverSize = Point2I.One;
			if (source == null)
				return false;
			return point < source.Dimensions;
		}

		protected override void Draw(Graphics2D g, SpriteDrawSettings settings, Zone zone) {
			if (source == null)
				return;
			for (int indexX = 0; indexX < source.Width; indexX++) {
				for (int indexY = 0; indexY < source.Height; indexY++) {
					SpriteInfo sprite = spriteGrid[indexX, indexY];
					if (sprite != null) {
						int x = 1 + indexX * (spriteSize.X + 1);
						int y = 1 + indexY * (spriteSize.Y + 1);
						g.DrawSprite(sprite.Sprite, settings, new Vector2F(x, y) - sprite.Bounds.Point);
					}
				}
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
				return spriteGrid[hoverPoint.X, hoverPoint.Y];
			}
		}
	}
}
