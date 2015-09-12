using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
	
	public struct AnimationFrame {
		private int		startTime;	// Start time in ticks.
		private int		duration;	// Duration in ticks.
		private Sprite	sprite;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public AnimationFrame(int startTime, int duration, Sprite sprite) {
			this.startTime	= startTime;
			this.duration	= duration;
			this.sprite		= sprite;
		}

		public AnimationFrame(int startTime, int duration, Image image, Rectangle2I sourceRect, Point2I drawOffset) {
			this.startTime	= startTime;
			this.duration	= duration;
			this.sprite		= new Sprite(image, sourceRect, drawOffset);
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
		
		public Sprite Sprite {
			get { return sprite; }
			set { sprite = value; }
		}
		
		public Image Image {
			get { return sprite.Image; }
			set { sprite.Image = value; }
		}
		
		public Rectangle2I SourceRect {
			get { return sprite.SourceRect; }
			set { sprite.SourceRect = value; }
		}
		
		public Point2I DrawOffset {
			get { return sprite.DrawOffset; }
			set { sprite.DrawOffset = value; }
		}
	}
}
