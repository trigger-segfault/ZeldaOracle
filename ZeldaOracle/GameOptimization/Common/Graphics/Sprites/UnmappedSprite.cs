using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>A sprite created directly from an image and draw offset.
	/// Used for unmapping paletted sprites.</summary>
	public class UnmappedSprite : ISprite {

		/// <summary>The image data for the unmapped sprite.</summary>
		public Image Image { get; }
		/// <summary>The draw offset for the texture.</summary>
		public Point2I DrawOffset { get; }


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an unmapped sprite.</summary>
		public UnmappedSprite(Image image, Point2I drawOffset) {
			Image		= image;
			DrawOffset	= drawOffset;
		}

		/// <summary>Constructs a copy of the unmapped sprite.</summary>
		public UnmappedSprite(UnmappedSprite copy) {
			Image		= copy.Image;
			DrawOffset	= copy.DrawOffset;
		}


		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the drawable parts for the sprite.</summary>
		public SpritePart GetParts(SpriteSettings settings) {
			return new SpritePart(Image, Image.Bounds, DrawOffset/*,
				flipEffects, rotation*/);
		}

		/// <summary>Gets all sprites contained by this sprite including this one.</summary>
		public IEnumerable<ISprite> GetAllSprites() {
			yield return this;
		}

		/// <summary>Returns true if this sprite contains the specified sprite.</summary>
		public bool ContainsSubSprite(ISprite sprite) {
			return false;
		}

		/// <summary>Clones the sprite.</summary>
		public ISprite Clone() {
			return new UnmappedSprite(this);
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteSettings settings) {
			return new Rectangle2I(DrawOffset, Image.Size);
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I Bounds {
			get { return new Rectangle2I(DrawOffset, Image.Size); }
		}
	}
}
