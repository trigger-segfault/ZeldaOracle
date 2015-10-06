using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Content.ResourceBuilders {

	public class SpriteBuilder {
		private Sprite sprite;
		private Sprite spriteBegin;
		private SpriteSheet sheet;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public SpriteBuilder() {
			sheet		= null;
			sprite		= null;
			spriteBegin	= null;
		}


		//-----------------------------------------------------------------------------
		// Begin/End
		//-----------------------------------------------------------------------------

		public SpriteBuilder Begin(Sprite sprite) {
			this.sprite = sprite;
			this.spriteBegin = sprite;
			return this;
		}
		
		public SpriteBuilder Begin() {
			sprite = new Sprite();
			spriteBegin = sprite;
			return this;
		}

		public Sprite End() {
			Sprite temp = spriteBegin;
			sprite = null;
			spriteBegin = null;
			return temp;
		}


		//-----------------------------------------------------------------------------
		// Building
		//-----------------------------------------------------------------------------

		public SpriteBuilder AddPart(int sheetX, int sheetY, int offsetX = 0, int offsetY = 0) {
			sprite.NextPart = new Sprite(sheet, sheetX, sheetY, offsetX, offsetY);
			sprite = sprite.NextPart;
			return this;
		}


		//-----------------------------------------------------------------------------
		// Modifications
		//-----------------------------------------------------------------------------

		public SpriteBuilder SetSheet(SpriteSheet sheet) {
			this.sheet = sheet;
			return this;
		}
		
		public SpriteBuilder SetSize(int width, int height) {
			sprite.SourceRect = new Rectangle2I(sprite.SourceRect.Point, new Point2I(width, height));
			return this;
		}

		public SpriteBuilder SetSize(Point2I size) {
			sprite.SourceRect = new Rectangle2I(sprite.SourceRect.Point, size);
			return this;
		}
		
		public SpriteBuilder Offset(int x, int y) {
			for (Sprite spr = spriteBegin; spr != null; spr = spr.NextPart) {
				spr.DrawOffset += new Point2I(x, y);
			}
			return this;
		}

		public SpriteBuilder Offset(Point2I offset) {
			for (Sprite spr = spriteBegin; spr != null; spr = spr.NextPart) {
				spr.DrawOffset += offset;
			}
			return this;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SpriteSheet SpriteSheet {
			get { return sheet; }
			set { sheet = value; }
		}

		public Sprite Sprite {
			get { return sprite; }
			set { sprite = value; }
		}
	}
}
