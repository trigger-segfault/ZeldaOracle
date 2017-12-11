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


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public SpritePart(Image image, Rectangle2I sourceRect) {
			this.image          = image;
			this.sourceRect     = sourceRect;
			this.drawOffset     = Point2I.Zero;
		}

		public SpritePart(Image image, Rectangle2I sourceRect, Point2I drawOffset) {
			this.image          = image;
			this.sourceRect     = sourceRect;
			this.drawOffset     = drawOffset;
		}


		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------
		
		/// <summary>Returns the hash code for this sprite part.</summary>
		public override int GetHashCode() {
			return image.GetHashCode() + sourceRect.GetHashCode() + drawOffset.GetHashCode();
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
			return	(part1.image == part2.image) &&
					(part1.sourceRect == part2.sourceRect) &&
					(part1.drawOffset == part2.drawOffset);
		}

		public static bool operator !=(SpritePart part1, SpritePart part2) {
			return (part1.image != part2.image) ||
					(part1.sourceRect != part2.sourceRect) ||
					(part1.drawOffset != part2.drawOffset);
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
	}
}
