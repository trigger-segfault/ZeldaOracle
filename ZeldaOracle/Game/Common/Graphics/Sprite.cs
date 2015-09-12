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

		public Sprite(Image image, Rectangle2I sourceRect, Point2I drawOffset) {
			this.image		= image;
			this.sourceRect	= sourceRect;
			this.drawOffset	= drawOffset;
			this.nextPart	= null;
		}

		public Sprite(Image image, int sx, int sy, int sw, int sh, int dx, int dy) {
			this.image		= image;
			this.sourceRect	= new Rectangle2I(sx, sy, sw, sh);
			this.drawOffset	= new Point2I(dx, dy);
			this.nextPart	= null;
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
