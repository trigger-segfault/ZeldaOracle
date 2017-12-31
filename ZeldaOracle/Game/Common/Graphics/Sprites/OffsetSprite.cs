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
			this.flipEffects    = Flip.None;
		}

		/// <summary>Constructs an offset sprite with the specified sprite.</summary>
		public OffsetSprite(ISprite sprite, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			this.sprite			= sprite;
			this.drawOffset		= Point2I.Zero;
			this.flipEffects    = flip;
			this.rotation		= rotation;
		}

		/// <summary>Constructs an offset sprite with the specified sprite and offset.</summary>
		public OffsetSprite(ISprite sprite, Point2I drawOffset, Flip flip = Flip.None, Rotation rotation = Rotation.None) {
			this.sprite			= sprite;
			this.drawOffset		= drawOffset;
			this.flipEffects	= flip;
			this.rotation		= rotation;
		}

		/// <summary>Constructs a copy of the offset sprite.</summary>
		public OffsetSprite(OffsetSprite copy) {
			this.sprite			= copy.sprite;
			this.drawOffset		= copy.drawOffset;
			this.flipEffects    = copy.flipEffects;
			this.rotation		= copy.rotation;
		}


		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the drawable parts for the sprite.</summary>
		public IEnumerable<SpritePart> GetParts(SpriteDrawSettings settings) {
			Rectangle2I bounds = Bounds;
			foreach (SpritePart part in sprite.GetParts(settings)) {
				yield return new SpritePart(part, drawOffset, flipEffects, rotation, bounds);
			}
		}

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

		/// <summary>Gets or sets the extra draw offset of the offset sprite.</summary>
		public Point2I DrawOffset {
			get { return drawOffset; }
			set { drawOffset = value; }
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
