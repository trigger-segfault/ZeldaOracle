using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>A sprite with an additional offset and flip effects.</summary>
	public class OffsetSprite : ISprite {

		/// <summary>The sprite for this offset sprite.</summary>
		private ISprite sprite;
		/// <summary>The draw offset for this offset sprite.</summary>
		private Point2I drawOffset;
		/// <summary>The clipping of the sprite.</summary>
		public Rectangle2I? clipping;
		/// <summary>The flipping applied to the sprite.</summary>
		private Flip flipEffects;
		/// <summary>The number of 90-degree rotations for the sprite.</summary>
		private Rotation rotation;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an empty offset sprite.</summary>
		public OffsetSprite() {
			this.sprite			= null;
			this.drawOffset		= Point2I.Zero;
			this.clipping		= null;
			this.flipEffects	= Flip.None;
			this.rotation		= Rotation.None;
		}

		/// <summary>Constructs an offset sprite with the specified sprite and settings.</summary>
		public OffsetSprite(ISprite sprite, Rectangle2I? clipping = null, Flip flip = Flip.None,
			Rotation rotation = Rotation.None)
		{
			this.sprite			= sprite;
			this.drawOffset		= Point2I.Zero;
			this.clipping		= clipping;
			this.flipEffects	= flip;
			this.rotation		= rotation;
		}

		/// <summary>Constructs an offset sprite with the specified sprite and settings.</summary>
		public OffsetSprite(ISprite sprite, Point2I drawOffset, Rectangle2I? clipping = null,
			Flip flip = Flip.None, Rotation rotation = Rotation.None)
		{
			this.sprite			= sprite;
			this.drawOffset		= drawOffset;
			this.clipping		= clipping;
			this.flipEffects	= flip;
			this.rotation		= rotation;
		}

		/// <summary>Constructs a copy of the offset sprite.</summary>
		public OffsetSprite(OffsetSprite copy) {
			this.sprite			= copy.sprite;
			this.drawOffset		= copy.drawOffset;
			this.clipping		= copy.clipping;
			this.flipEffects	= copy.flipEffects;
			this.rotation		= copy.rotation;
		}


		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the drawable parts for the sprite.</summary>
		public SpritePart GetParts(SpriteSettings settings) {
			if (sprite == null)
				return null;
			SpritePart firstPart = sprite.GetParts(settings);
			SpritePart parts = firstPart;
			while (parts != null) {
				parts.AddOffset(drawOffset, clipping/*, flipEffects, rotation*/);
				parts = parts.NextPart;
			}
			return firstPart;
		}

		/// <summary>Clones the sprite.</summary>
		public ISprite Clone() {
			return new OffsetSprite(this);
		}
		
		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteSettings settings) {
			if (sprite == null)
				return Rectangle2I.Zero;
			Rectangle2I bounds = sprite.GetBounds(settings);
			if (clipping.HasValue)
				return Rectangle2I.Intersect(bounds, clipping.Value) + drawOffset;
			return bounds + drawOffset;
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I Bounds {
			get {
				if (sprite == null)
					return Rectangle2I.Zero;
				Rectangle2I bounds = sprite.Bounds;
				if (clipping.HasValue)
					return Rectangle2I.Intersect(bounds, clipping.Value) + drawOffset;
				return bounds + drawOffset;
			}
		}

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Adds the clipping to the sprite and insersects with the previous clipping.</summary>
		public void Clip(Rectangle2I clipping) {
			if (this.clipping.HasValue)
				this.clipping = Rectangle2I.Intersect(this.clipping.Value, clipping);
			else
				this.clipping = clipping;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the sprite of the offset sprite.</summary>
		public ISprite Sprite {
			get { return sprite; }
			set { sprite = value; }
		}

		/// <summary>Gets or sets the extra draw offset of the offset sprite.</summary>
		public Point2I DrawOffset {
			get { return drawOffset; }
			set { drawOffset = value; }
		}

		/// <summary>Gets or sets the clipping of the sprite.</summary>
		public Rectangle2I? Clipping {
			get { return clipping; }
			set { clipping = value; }
		}

		/// <summary>Gets or sets the extra flipping applied to the offset sprite.</summary>
		public Flip FlipEffects {
			get { return flipEffects; }
			set { flipEffects = value; }
		}

		/// <summary>Gets or sets the extra number of 90-degree rotations for the sprite.</summary>
		public Rotation Rotation {
			get { return rotation; }
			set { rotation = value; }
		}
	}
}
