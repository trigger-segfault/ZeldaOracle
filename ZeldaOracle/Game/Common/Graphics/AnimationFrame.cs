using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
	
	public struct AnimationFrame {
		int			startTime;		// Start time in ticks.
		int			duration;		// Duration in ticks.
		Image		image;
		Rectangle2I	sourceRect;
		Point2I		drawOffset;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public AnimationFrame(int startTime, int duration, Image image, Rectangle2I sourceRect, Point2I drawOffset) {
			this.startTime	= startTime;
			this.duration	= duration;
			this.image		= image;
			this.sourceRect	= sourceRect;
			this.drawOffset	= drawOffset;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int StartTime {
			get { return startTime; }
			set { startTime = value; }
		}
		
		public int Duration {
			get { return duration; }
			set { duration = value; }
		}
		
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
	}
}
