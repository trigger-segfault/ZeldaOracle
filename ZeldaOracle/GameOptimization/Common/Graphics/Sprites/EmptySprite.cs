using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>A sprite with no images attached.</summary>
	public class EmptySprite : ISprite {

		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the drawable parts for the sprite.</summary>
		public SpritePart GetParts(SpriteSettings settings) {
			return null;
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
			return new EmptySprite();
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteSettings settings) {
			return Rectangle2I.Zero;
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I Bounds {
			get { return Rectangle2I.Zero; }
		}
	}
}
