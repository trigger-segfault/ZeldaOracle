using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>The end result of all sprites. These are what Graphics2D uses to draw.
	/// Sprite parts are returned from GetParts() in ISprite.</summary>
	public struct SpritePart {
		/// <summary>The image used by the sprite.</summary>
		private Image image;
		/// <summary>The source rectangle of the sprite.</summary>
		private Rectangle2I sourceRect;
		/// <summary>The draw offset of the sprite.</summary>
		private Point2I drawOffset;
		/// <summary>The flipping applied to the sprite.</summary>
		private Flip flipEffects;
		/// <summary>The number of 90-degree rotations for the sprite.</summary>
		private Rotation rotation;
		/// <summary>The origin of the sprite used for rotation and flipping only.</summary>
		private Vector2F origin;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the sprite part. For use with BasicSprite.</summary>
		public SpritePart(Image image, Rectangle2I sourceRect, Point2I drawOffset,
			Flip flip, Rotation rotation)
		{
			this.image			= image;
			this.sourceRect		= sourceRect;
			this.drawOffset		= drawOffset;
			this.flipEffects	= flip;
			this.rotation		= rotation;
			this.origin         = (Vector2F) sourceRect.Size / 2f + drawOffset;
		}

		/// <summary>Constructs a sprite part with added flipping, rotation, and clipping.</summary>
		/*public SpritePart(Image image, Rectangle2I sourceRect, Point2I drawOffset,
			Rectangle2I? clipping, Flip flip, Rotation rotation)
		{
			this.image          = image;
			this.sourceRect     = sourceRect;
			this.drawOffset     = drawOffset;
			this.flipEffects    = flip;
			this.rotation		= rotation;
			if (clipping.HasValue) {
				Clip(clipping.Value, Point2I.Zero);
				this.drawOffset += clipping.Value.Point;
			}
		}*/

		/// <summary>Constructs a sprite part with added flipping and rotation around (0, 0).</summary>
		/*public SpritePart(SpritePart part, Point2I addDrawOffset, Rectangle2I? clipping,
			Flip addFlip, Rotation addRotation)
		{
			this.image			= part.image;
			this.sourceRect		= part.sourceRect;
			this.drawOffset		= part.drawOffset + addDrawOffset;
			this.flipEffects    = Flipping.Add(part.flipEffects, addFlip);
			this.rotation		= Rotating.Add(part.rotation, addRotation);
			if (clipping.HasValue) {
				Clip(clipping.Value, part.DrawOffset);
				this.drawOffset += clipping.Value.Point;
			}
		}*/

		/// <summary>Constructs an offset sprite part. For use with OffsetSprite.</summary>
		public SpritePart(SpritePart part, Point2I addDrawOffset, Rectangle2I? clipping,
			Flip addFlip, Rotation addRotation, Rectangle2I bounds)
		{
			this.image          = part.image;
			this.sourceRect     = part.sourceRect;
			this.drawOffset		= part.drawOffset + addDrawOffset;
			this.flipEffects    = Flipping.Add(part.flipEffects, addFlip);
			this.rotation       = Rotating.Add(part.rotation, addRotation);
			this.origin			= Vector2F.Zero; // Reassign after clipping is applied
			if (clipping.HasValue) {
				Clip(clipping.Value, part.DrawOffset);
				this.drawOffset += clipping.Value.Point;
			}
			this.origin			= (Vector2F) sourceRect.Size / 2f + this.drawOffset;
			// Completely flip and rotate composite sprites and animations
			this.drawOffset     = GMath.FlipAndRotate(this.drawOffset, addFlip, addRotation, this.origin);// this.sourceRect.Size, bounds);
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Returns the hash code for this sprite part.</summary>
		public override int GetHashCode() {
			return image.GetHashCode() + sourceRect.GetHashCode() + drawOffset.GetHashCode() +
				flipEffects.GetHashCode() + rotation.GetHashCode();
		}

		/// <summary>Returns true if the object is a sprite part with the same settings.</summary>
		public override bool Equals(object obj) {
			return (obj is SpritePart && (this == (SpritePart) obj));
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static bool operator ==(SpritePart part1, SpritePart part2) {
			return (part1.image == part2.image) &&
					(part1.sourceRect == part2.sourceRect) &&
					(part1.drawOffset == part2.drawOffset) &&
					(part1.flipEffects == part2.flipEffects) &&
					(part1.flipEffects == part2.flipEffects) &&
					(part1.rotation == part2.rotation);
		}

		public static bool operator !=(SpritePart part1, SpritePart part2) {
			return	(part1.image != part2.image) ||
					(part1.sourceRect != part2.sourceRect) ||
					(part1.drawOffset != part2.drawOffset) ||
					(part1.flipEffects != part2.flipEffects) ||
					(part1.rotation != part2.rotation);
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Clips the sprite part's source rectangle.</summary>
		/// <param name="drawOffset">The original draw offset of the sprite
		/// before the final OffsetSprite's draw offset is applied.</param>
		private void Clip(Rectangle2I clipping, Point2I drawOffset) {
			sourceRect = Rectangle2I.Intersect(sourceRect + drawOffset, clipping + sourceRect.Point) - drawOffset;
		}


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
		public Flip FlipEffects {
			get { return flipEffects; }
			set { flipEffects = value; }
		}

		/// <summary>Gets or sets the number of 90-degree rotations for the sprite.</summary>
		public Rotation Rotation {
			get { return rotation; }
			set { rotation = value; }
		}

		/// <summary>Gets or sets the origin of the sprite used for rotation and flipping only.</summary>
		public Vector2F Origin {
			get { return origin; }
			set { origin = value; }
		}

		// Helpers --------------------------------------------------------------------

		/// <summary>Gets the rotation of the sprite in radians.</summary>
		public float RotationRadians {
			get { return rotation.ToRadians(); }
		}

		/// <summary>Gets the size of the sprite part.</summary>
		public Point2I Size {
			get { return sourceRect.Size; }
		}

		/// <summary>Gets the center of the sprite in size only.</summary>
		public Vector2F Center {
			get { return (Vector2F) sourceRect.Size / 2f; }
		}

		/// <summary>Gets the center of the sprite in size only combined with the draw offset.</summary>
		public Vector2F CenterDrawOffset {
			get { return (Vector2F) sourceRect.Size / 2f + drawOffset; }
		}

		/// <summary>Returns true if the sprite part is empty and should be skipped.</summary>
		public bool IsEmpty {
			get { return sourceRect.IsEmpty; }
		}
	}
}
