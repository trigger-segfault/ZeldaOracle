using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>A sprite with an additional offset.</summary>
	public class OffsetSprite : ISprite {
		/// <summary>The sprite for this offset sprite.</summary>
		private ISprite sprite;
		/// <summary>The draw offset for this offset sprite.</summary>
		private Point2I drawOffset;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an empty offset sprite.</summary>
		public OffsetSprite() {
			this.sprite     = null;
			this.drawOffset = Point2I.Zero;
		}

		/// <summary>Constructs an offset sprite with the specified sprite.</summary>
		public OffsetSprite(ISprite sprite) {
			this.sprite     = sprite;
			this.drawOffset = Point2I.Zero;
		}

		/// <summary>Constructs an offset sprite with the specified sprite and offset.</summary>
		public OffsetSprite(ISprite sprite, Point2I drawOffset) {
			this.sprite     = sprite;
			this.drawOffset = drawOffset;
		}

		/// <summary>Constructs a copy of the offset sprite.</summary>
		public OffsetSprite(OffsetSprite copy) {
			this.sprite     = copy.sprite;
			this.drawOffset = copy.drawOffset;
		}


		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the drawable parts for the sprite.</summary>
		public IEnumerable<SpritePart> GetParts(SpriteDrawSettings settings) {
			foreach (SpritePart part in sprite.GetParts(settings)) {
				SpritePart offsetPart = part;
				offsetPart.DrawOffset += drawOffset;
				yield return offsetPart;
			}
		}

		/// <summary>Translates the sprite.</summary>
		/*public ISprite Translate(Point2I distance) {
			foreach (ISprite sprite in styles.Values) {
				sprite.Translate(distance);
			}
			return this;
		}*/

		/// <summary>Clones the sprite.</summary>
		public ISprite Clone() {
			return new OffsetSprite(this);
		}
		
		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteDrawSettings settings) {
			return sprite.GetBounds(settings) + drawOffset;
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I Bounds {
			get { return sprite.Bounds + drawOffset; }
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the sprite of the offset sprite.</summary>
		public ISprite Sprite {
			get { return sprite; }
			set { sprite = value; }
		}

		/// <summary>Gets or sets the draw offset of the offset sprite.</summary>
		public Point2I DrawOffset {
			get { return drawOffset; }
			set { drawOffset = value; }
		}
	}
}
