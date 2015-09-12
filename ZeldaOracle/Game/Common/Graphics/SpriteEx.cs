using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaColor		= Microsoft.Xna.Framework.Color;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Graphics {

	// THE OLD SPRITE CLASS. TODO: DELETE THIS!
	// A basic sprite used in sprite sheets.
	public class SpriteEx {

		// The name identifier of the sprite.
		private string name;
		// The sprite sheet containing this sprite.
		private SpriteAtlas sheet;
		// The compacted rectangle of where this sprite is on the sprite sheet.
		private Rectangle2I frameRect;
		// The sheet-independent, preffered size of the sprite.
		private Point2I sourceSize;
		// The offset of the packed frame rectangle from the original source rectangle.
		private Vector2F offset;
		// The center/pivot of the sprite where it is drawn and rotated from.
		private Vector2F origin;
	
	
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		// Constructs the default sprite.
		public SpriteEx() {
			this.name		= "";
			this.sheet		= null;
			this.frameRect	= Rectangle2I.Zero;
			this.sourceSize	= Point2I.Zero;
			this.offset		= Vector2F.Zero;
			this.origin		= Vector2F.Zero;
		}

		// Constructs the default sprite.
		public SpriteEx(string name, SpriteAtlas sheet, Rectangle2I sourceRect, Vector2F origin) {
			this.name		= name;
			this.sheet		= sheet;
			this.frameRect	= sourceRect;
			this.sourceSize	= sourceRect.Size;
			this.offset		= Vector2F.Zero;
			this.origin		= origin;
		}

		// Constructs the default sprite.
		public SpriteEx(SpriteEx sprite) {
			this.name		= sprite.name;
			this.sheet		= sprite.sheet;
			this.frameRect	= sprite.frameRect;
			this.sourceSize	= sprite.sourceSize;
			this.offset		= sprite.offset;
			this.origin		= sprite.origin;
		}


		//-----------------------------------------------------------------------------
		// Settings
		//-----------------------------------------------------------------------------

		// Centers the origin of the sprite.
		public void CenterOrigin() {
			AlignOrigin(0.5f, 0.5f);
		}

		// Aligns the origin of the sprite.
		public void AlignOrigin(float x, float y) {
			origin.X = sourceSize.X * x;
			origin.Y = sourceSize.Y * y;
		}

		// Sets the color data of the sprite on the sprite sheet.
		public void SetColorData(Color[] data) {
			Color[] sheetData = new Color[sheet.Image.Width * sheet.Image.Height];
			sheet.Image.Texture.GetData(sheetData);

			for (int y = 0; y < frameRect.Height; ++y) {
				for (int x = 0; x < frameRect.Width; ++x) {
					int sheetX = x + frameRect.X;
					int sheetY = y + frameRect.Y;
					sheetData[(sheetY * sheet.Image.Width) + sheetX] = data[(y * frameRect.Width) + x];
				}
			}
		}

		// Gets the color data of the sprite on the sprite sheet.
		public Color[] GetColorData() {
			Color[] data = new Color[frameRect.Width * frameRect.Height];
			Color[] sheetData = new Color[sheet.Image.Width * sheet.Image.Height];
			sheet.Image.Texture.GetData(sheetData);

			for (int y = 0; y < frameRect.Height; ++y) {
				for (int x = 0; x < frameRect.Width; ++x) {
					int sheetX = x + frameRect.X;
					int sheetY = y + frameRect.Y;
					data[(y * frameRect.Width) + x] = sheetData[(sheetY * sheet.Image.Width) + sheetX];
				}
			}

			return data;
		}

		// Gets the average color of the sprite on the sprite sheet.
		public Color GetAverageColor(Color multiplier) {
			Vector4 colorSum = Vector4.Zero;
			Color[] data = GetColorData();
			Vector4 mult = ((XnaColor)multiplier).ToVector4();

			for (int i = 0; i < data.Length; ++i) {
				Vector4 c = ((XnaColor)data[i]).ToVector4() * mult;
				colorSum.X += c.X * c.W;
				colorSum.Y += c.Y * c.W;
				colorSum.Z += c.Z * c.W;
				colorSum.W += c.W;
			}

			colorSum.X /= colorSum.W;
			colorSum.Y /= colorSum.W;
			colorSum.Z /= colorSum.W;
			colorSum.W /= frameRect.Width * frameRect.Height;
			return new Color(colorSum.X, colorSum.Y, colorSum.Z, colorSum.W);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// The name identifier of the sprite.
		public string Name {
			get { return name; }
			set { name = value; }
		}
		// The sprite sheet containing this sprite.
		public SpriteAtlas Sheet {
			get { return sheet; }
			set { sheet = value; }
		}
		// The compacted rectangle of where this sprite is on the sprite sheet.
		public Rectangle2I FrameRect {
			get { return frameRect; }
			set { frameRect = value; }
		}
		// The sheet-independent, preffered size of the sprite.
		public Point2I SourceSize {
			get { return sourceSize; }
			set { sourceSize = value; }
		}
		// The offset of the packed frame rectangle from the original source rectangle.
		public Vector2F Offset {
			get { return offset; }
			set { offset = value; }
		}
		// The center/pivot of the sprite where it is drawn and rotated from.
		public Vector2F Origin {
			get { return origin; }
			set { origin = value; }
		}

	}
} // end namespace
