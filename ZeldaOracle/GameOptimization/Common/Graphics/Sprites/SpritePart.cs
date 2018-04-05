using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>The end result of all sprites. These are what Graphics2D uses to draw.
	/// Sprite parts are returned from GetParts() in ISprite.</summary>
	public class SpritePart {
		/// <summary>The image used by the sprite.</summary>
		private Image image;
		/// <summary>The source rectangle of the sprite.</summary>
		private Rectangle2I sourceRect;
		/// <summary>The draw offset of the sprite.</summary>
		private Point2I drawOffset;
		/// <summary>The flipping applied to the sprite.</summary>
		//private Flip flipEffects;
		/// <summary>The number of 90-degree rotations for the sprite.</summary>
		//private Rotation rotation;
		/// <summary>The origin of the sprite used for rotation and flipping only.</summary>
		//private Vector2F origin;
		/// <summary>The next sprite in the collection of parts to draw.</summary>
		private SpritePart nextPart;
		/// <summary>The last sprite in the collection of parts to draw.
		/// Can be the same as this sprite.</summary>
		private SpritePart tailPart;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the sprite part. For use with BasicSprite.</summary>
		public SpritePart(Image image, Rectangle2I sourceRect, Point2I drawOffset/*,
			Flip flip, Rotation rotation*/)
		{
			this.image			= image;
			this.sourceRect		= sourceRect;
			this.drawOffset		= drawOffset;
			this.nextPart		= null;
			this.tailPart		= this;
			//this.flipEffects	= flip;
			//this.rotation		= rotation;
			//this.origin         = (Vector2F) sourceRect.Size / 2f + drawOffset;
		}
		
		/// <summary>Constructs an offset sprite part. For use with OffsetSprite.</summary>
		/*public SpritePart(SpritePart part, Point2I addDrawOffset, Rectangle2I? clipping,
			Flip addFlip, Rotation addRotation)
		{
			this.image          = part.image;
			this.sourceRect     = part.sourceRect;
			this.drawOffset		= part.drawOffset + addDrawOffset;
			//this.flipEffects    = Flipping.Add(part.flipEffects, addFlip);
			//this.rotation       = Rotating.Add(part.rotation, addRotation);
			if (clipping.HasValue) {
				Clip(clipping.Value, part.DrawOffset);
			}
			// Completely flip and rotate composite sprites and animations
			//this.drawOffset     = GMath.FlipAndRotate(this.drawOffset, addFlip, addRotation, this.origin);// this.sourceRect.Size, bounds);
		}*/

		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Adds offset and clipping to the sprite part.
		/// For use with OffsetSprite.</summary>
		public void AddOffset(Point2I addDrawOffset, Rectangle2I? clipping/*,
			Flip addFlip, Rotation addRotation*/)
		{
			Point2I originalDrawOffset = drawOffset;
			drawOffset     += addDrawOffset;
			//this.flipEffects    = Flipping.Add(this.flipEffects, addFlip);
			//this.rotation       = Rotating.Add(this.rotation, addRotation);
			//this.origin			= Vector2F.Zero; // Reassign after clipping is applied
			if (clipping.HasValue)
				Clip(clipping.Value, originalDrawOffset);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Clips the sprite part's source rectangle.</summary>
		/// <param name="originalDrawOffset">The original draw offset of the sprite
		/// before the final OffsetSprite's draw offset is applied.</param>
		private void Clip(Rectangle2I clipping, Point2I originalDrawOffset) {
			sourceRect = Rectangle2I.Intersect(sourceRect + originalDrawOffset,
				clipping + sourceRect.Point) - originalDrawOffset;
			drawOffset += clipping.Point;
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Gets the hashcode for the sprite part.</summary>
		public override int GetHashCode() {
			return image.GetHashCode() ^ sourceRect.GetHashCode() ^
				drawOffset.GetHashCode();
		}

		/// <summary>Returns true if the object is a sprite part and equal to this
		/// sprite part.</summary>
		public override bool Equals(object obj) {
			if (obj is SpritePart) {
				SpritePart b = (SpritePart) obj;
				return image == b.image &&
					sourceRect == b.sourceRect &&
					drawOffset == b.drawOffset;
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		/*public static bool operator ==(SpritePart a, SpritePart b) {
			if (a != null && b != null) {
				return a.image == b.image &&
					a.sourceRect == b.sourceRect &&
					a.drawOffset == b.drawOffset;
			}
			return (a == b);
		}

		public static bool operator !=(SpritePart a, SpritePart b) {
			if (a != null && b != null) {
				return a.image != b.image ||
					a.sourceRect != b.sourceRect ||
					a.drawOffset != b.drawOffset;
			}
			return (a != b);
		}*/


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the image used by the sprite.</summary>
		public Image Image {
			get { return image; }
			set { image = value; }
		}

		/// <summary>Gets or sets the source rectangle of the sprite.</summary>
		public Rectangle2I SourceRect {
			get { return sourceRect; }
			set { sourceRect = value; }
		}

		/// <summary>Gets or sets the draw offset of the sprite.</summary>
		public Point2I DrawOffset {
			get { return drawOffset; }
			set { drawOffset = value; }
		}

		/// <summary>Gets or sets the flipping applied to the sprite.</summary>
		/*public Flip FlipEffects {
			get { return flipEffects; }
			set { flipEffects = value; }
		}*/

		/// <summary>Gets or sets the number of 90-degree rotations for the sprite.</summary>
		/*public Rotation Rotation {
			get { return rotation; }
			set { rotation = value; }
		}*/

		/// <summary>Gets or sets the origin of the sprite used for rotation and
		/// flipping only.</summary>
		/*public Vector2F Origin {
			get { return origin; }
			set { origin = value; }
		}*/

		/// <summary>Gets or sets the next sprite in the collection of parts to draw.</summary>
		public SpritePart NextPart {
			get { return nextPart; }
			set { nextPart = value; }
		}

		/// <summary>Gets or sets the last sprite in the collection of parts to draw.
		/// Can be the same as this sprite.</summary>
		public SpritePart TailPart {
			get { return tailPart; }
			set { tailPart = value; }
		}

		// Helpers --------------------------------------------------------------------

		/// <summary>Gets the rotation of the sprite in radians.</summary>
		/*public float RotationRadians {
			get { return rotation.ToRadians(); }
		}*/

		/// <summary>Gets the size of the sprite part.</summary>
		public Point2I Size {
			get { return sourceRect.Size; }
		}

		/// <summary>Gets the center of the sprite in size only.</summary>
		public Vector2F Center {
			get { return (Vector2F) sourceRect.Size / 2f; }
		}

		/// <summary>Gets the center of the sprite in size only combined with the draw
		/// offset.</summary>
		public Vector2F CenterDrawOffset {
			get { return (Vector2F) sourceRect.Size / 2f + drawOffset; }
		}

		/// <summary>Returns true if the sprite part is empty and should be skipped.</summary>
		public bool IsEmpty {
			get { return sourceRect.IsEmpty; }
		}
	}
}
