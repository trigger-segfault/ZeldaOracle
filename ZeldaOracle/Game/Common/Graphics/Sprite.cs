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
/** <summary>
 * A basic sprite used in sprite sheets.
 * </summary> */
public class Sprite {

	//=========== MEMBERS ============
	#region Members

	/** <summary> The name identifier of the sprite. </summary> */
	private string name;
	/** <summary> The sprite sheet containing this sprite. </summary> */
	private SpriteSheet sheet;
	/** <summary> The compacted rectangle of where this sprite is on the sprite sheet. </summary> */
	private Rectangle2I frameRect;
	/** <summary> The sheet-independent, preffered size of the sprite. </summary> */
	private Point2I sourceSize;
	/** <summary> The offset of the packed frame rectangle from the original source rectangle. </summary> */
	private Vector2F offset;
	/** <summary> The center/pivot of the sprite where it is drawn and rotated from. </summary> */
	private Vector2F origin;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default sprite. </summary> */
	public Sprite() {
		this.name		= "";
		this.sheet		= null;
		this.frameRect	= Rectangle2I.Zero;
		this.sourceSize	= Point2I.Zero;
		this.offset		= Vector2F.Zero;
		this.origin		= Vector2F.Zero;
	}
	/** <summary> Constructs the default sprite. </summary> */
	public Sprite(string name, SpriteSheet sheet, Rectangle2I sourceRect, Vector2F origin) {
		this.name		= name;
		this.sheet		= sheet;
		this.frameRect	= sourceRect;
		this.sourceSize	= sourceRect.Size;
		this.offset		= Vector2F.Zero;
		this.origin		= origin;
	}

	/** <summary> Constructs the default sprite. </summary> */
	public Sprite(Sprite sprite) {
		this.name		= sprite.name;
		this.sheet		= sprite.sheet;
		this.frameRect	= sprite.frameRect;
		this.sourceSize	= sprite.sourceSize;
		this.offset		= sprite.offset;
		this.origin		= sprite.origin;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> The name identifier of the sprite. </summary> */
	public string Name {
		get { return name; }
		set { name = value; }
	}
	/** <summary> The sprite sheet containing this sprite. </summary> */
	public SpriteSheet Sheet {
		get { return sheet; }
		set { sheet = value; }
	}
	/** <summary> The compacted rectangle of where this sprite is on the sprite sheet. </summary> */
	public Rectangle2I FrameRect {
		get { return frameRect; }
		set { frameRect = value; }
	}
	/** <summary> The sheet-independent, preffered size of the sprite. </summary> */
	public Point2I SourceSize {
		get { return sourceSize; }
		set { sourceSize = value; }
	}
	/** <summary> The offset of the packed frame rectangle from the original source rectangle. </summary> */
	public Vector2F Offset {
		get { return offset; }
		set { offset = value; }
	}
	/** <summary> The center/pivot of the sprite where it is drawn and rotated from. </summary> */
	public Vector2F Origin {
		get { return origin; }
		set { origin = value; }
	}

	#endregion
	//=========== SETTINGS ===========
	#region Settings

	/** <summary> Centers the origin of the sprite. </summary> */
	public void CenterOrigin() {
		AlignOrigin(0.5f, 0.5f);
	}
	/** <summary> Aligns the origin of the sprite. </summary> */
	public void AlignOrigin(double x, double y) {
		origin.X = sourceSize.X * x;
		origin.Y = sourceSize.Y * y;
	}
	/** <summary> Sets the color data of the sprite on the sprite sheet. </summary> */
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
	/** <summary> Gets the color data of the sprite on the sprite sheet. </summary> */
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
	/** <summary> Gets the average color of the sprite on the sprite sheet. </summary> */
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

	#endregion
}
} // end namespace
