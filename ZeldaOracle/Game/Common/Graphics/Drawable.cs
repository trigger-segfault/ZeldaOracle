using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
public class Drawable {

	protected Sprite	sprite;
	protected double	depth;
	protected Color		color;
	protected Vector2F	origin;
	protected Vector2F	drawOffset;
	protected double	rotation;
	protected double	scale;
	protected SpriteEffects spriteEffects;
        


	// ================== CONSTRUCTORS ================== //
        
	public Drawable():
		this(null, Color.White) {
	}

	public Drawable(Sprite sprite, double depth = 0):
		this(sprite, Color.White, depth) {
	}

	public Drawable(Sprite sprite, Color color, double depth = 0) :
		this(sprite, color, Vector2F.Zero, depth) {
	}

	public Drawable(Sprite sprite, Color color, Vector2F origin, double depth = 0) {
		this.sprite        = sprite;
		this.color         = color;
		this.depth         = depth;
		this.origin        = origin;
		this.drawOffset    = Vector2F.Zero;
		this.rotation      = 0.0f;
		this.scale         = 1.0f;
		this.spriteEffects = SpriteEffects.None;
	}

	public Drawable(Drawable copy) {
		this.sprite        = copy.sprite;
		this.color         = copy.color;
		this.depth         = copy.depth;
		this.origin        = copy.origin;
		this.drawOffset    = copy.drawOffset;
		this.rotation      = copy.rotation;
		this.scale         = copy.scale;
		this.spriteEffects = copy.spriteEffects;
	}


        
	// ================ IMPLEMENTATIONS ================ //
        
	public virtual void Draw(Graphics2D g, Vector2F position) {
		if (sprite != null) {
			Vector2F drawOrigin = (sprite.Origin + origin) - sprite.Offset;

			if (FlipHorizontally)
				position.X += (2.0 * drawOrigin.X) - sprite.FrameRect.Width;
			if (FlipVertically)
				position.Y += (2.0 * drawOrigin.Y) - sprite.FrameRect.Height;

			g.DrawImage(sprite.Sheet.Image, position, sprite.FrameRect, drawOrigin, (Vector2F)scale, rotation, color, spriteEffects, depth);
		}
	}



	// ================== PROPERTIES =================== //

	public Sprite Sprite {
		get { return sprite; }
		set { sprite = value; }
	}

	public double Depth {
		get { return depth; }
		set { depth = value; }
	}

	public double Rotation {
		get { return rotation; }
		set { rotation = value; }
	}

	public double Scale {
		get { return scale; }
		set { scale = value; }
	}

	public Color Color {
		get { return color; }
		set { color = value; }
	}

	public Vector2F Origin {
		get { return origin; }
		set { origin = value; }
	}

	public Vector2F DrawOffset {
		get { return drawOffset; }
		set { drawOffset = value; }
	}

	public SpriteEffects Effects {
		get { return spriteEffects; }
		set { spriteEffects = value; }
	}

	public bool FlipHorizontally {
		get { return spriteEffects.HasFlag(SpriteEffects.FlipHorizontally); }
		set {
			if (value)
				spriteEffects |= SpriteEffects.FlipHorizontally;
			else
				spriteEffects &= ~SpriteEffects.FlipHorizontally;
		}
	}

	public bool FlipVertically {
		get { return spriteEffects.HasFlag(SpriteEffects.FlipVertically); }
		set {
			if (value)
				spriteEffects |= SpriteEffects.FlipVertically;
			else
				spriteEffects &= ~SpriteEffects.FlipVertically;
		}
	}
}
}
