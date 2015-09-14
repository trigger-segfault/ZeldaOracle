using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {

	// TODO: Multi-sprites. Probably as a linked list here

	public class Sprite {
		private Image		image;
		private Rectangle2I	sourceRect;
		private Point2I		drawOffset;
		private Sprite		nextPart;	// For compound sprites (made up of multiple sub-sprites).
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public Sprite() {
			image		= null;
			sourceRect	= Rectangle2I.Zero;
 			drawOffset	= Point2I.Zero;
			nextPart	= null;
		}
		
		public Sprite(SpriteSheet sheet, int sx, int sy)  :
			this(sheet, sx, sy, 0, 0)
		{
		}
		
		public Sprite(SpriteSheet sheet, int sx, int sy, int dx, int dy) {
			this.image		= sheet.Image;
			this.sourceRect	= new Rectangle2I(
				sheet.Offset.X + (sx * (sheet.CellSize.X + sheet.Spacing.X)),
				sheet.Offset.Y + (sy * (sheet.CellSize.Y + sheet.Spacing.Y)),
				sheet.CellSize.X, sheet.CellSize.Y);
			this.drawOffset = new Point2I(dx, dy);
			this.nextPart = null;
		}

		public Sprite(Image image, int sx, int sy, int sw, int sh) :
			this(image, sx, sy, sw, sh, 0, 0)
		{
		}

		public Sprite(Image image, int sx, int sy, int sw, int sh, int dx, int dy) {
			this.image		= image;
			this.sourceRect	= new Rectangle2I(sx, sy, sw, sh);
			this.drawOffset	= new Point2I(dx, dy);
			this.nextPart	= null;
		}

		public Sprite(Image image, Rectangle2I sourceRect) :
			this(image, sourceRect, Point2I.Zero)
		{
		}

		public Sprite(Image image, Rectangle2I sourceRect, Point2I drawOffset) {
			this.image		= image;
			this.sourceRect	= sourceRect;
			this.drawOffset	= drawOffset;
			this.nextPart	= null;
		}

		public Sprite(Sprite copy) {
			this.image		= copy.image;
			this.sourceRect	= copy.sourceRect;
			this.drawOffset	= copy.drawOffset;
			this.nextPart	= null;
			if (copy.nextPart != null)
				this.nextPart = new Sprite(copy.nextPart); // This is recursive.
		}

		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		public void Set(Sprite copy) {
			this.image		= copy.image;
			this.sourceRect	= copy.sourceRect;
			this.drawOffset	= copy.drawOffset;
			this.nextPart	= null;
			if (copy.nextPart != null)
				this.nextPart.Set(copy.nextPart); // This is recursive.
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Image Image {
			get { return image; }
			set { image = value; }
		}
		
		public Rectangle2I SourceRect {
			get { return sourceRect; }
			set { sourceRect = value; }
		}
		
		public Point2I DrawOffset {
			get { return drawOffset; }
			set { drawOffset = value; }
		}
		
		public Sprite NextPart {
			get { return nextPart; }
			set { nextPart = value; }
		}
	}
}
