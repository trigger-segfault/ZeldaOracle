using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
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


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public SpritePart(Image image, Rectangle2I sourceRect, Point2I drawOffset, Flip flip, Rotation rotation) {
			this.image          = image;
			this.sourceRect     = sourceRect;
			this.drawOffset     = drawOffset;
			this.flipEffects    = flip;
			this.rotation		= rotation;
		}

		public SpritePart(SpritePart part, Point2I addDrawOffset, Flip addFlip, Rotation addRotation) {
			this.image			= part.image;
			this.sourceRect		= part.sourceRect;
			this.drawOffset		= part.drawOffset + addDrawOffset;
			this.flipEffects    = Flipping.Add(part.flipEffects, addFlip);
			this.rotation		= Rotating.Add(part.rotation, addRotation);
		}

		public SpritePart(SpritePart part, Point2I addDrawOffset, Flip addFlip, Rotation addRotation, Rectangle2I bounds) {
			this.image          = part.image;
			this.sourceRect     = part.sourceRect;
			// Completely flip and rotate composite sprites and animations
			this.drawOffset     = GMath.FlipAndRotate(part.drawOffset + addDrawOffset, addFlip, addRotation, this.sourceRect.Size, bounds);
			this.flipEffects    = Flipping.Add(part.flipEffects, addFlip);
			this.rotation       = Rotating.Add(part.rotation, addRotation);
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		/// <summary>Returns the hash code for this sprite part.</summary>
		public override int GetHashCode() {
			return image.GetHashCode() + sourceRect.GetHashCode() + drawOffset.GetHashCode() +
				flipEffects.GetHashCode() + rotation.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return (obj is SpritePart && 
				(this == (SpritePart) obj));
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static bool operator ==(SpritePart part1, SpritePart part2) {
			return (part1.image == part2.image) &&
					(part1.sourceRect == part2.sourceRect) &&
					(part1.drawOffset == part2.drawOffset) &&
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

		// Helpers --------------------------------------------------------------------

		/// <summary>Gets the rotation of the sprite in radians.</summary>
		public float RotationRadians {
			get { return rotation.ToRadians(); }
		}

		/// <summary>Gets the center of the sprite in size only.</summary>
		public Vector2F Center {
			get { return (Vector2F)sourceRect.Size / 2f; }
		}

		/// <summary>Gets the center of the sprite in size only combined with the draw offset.</summary>
		public Vector2F CenterDrawOffset {
			get { return (Vector2F) sourceRect.Size / 2f + drawOffset; }
		}
	}
}
