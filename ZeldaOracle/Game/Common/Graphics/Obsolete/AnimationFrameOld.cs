using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
	public struct AnimationFrameOld {
		// Start time in ticks.
		private int startTime;
		// Duration in ticks.
		private int duration;
		// The sprite used in the frame.
		private SpriteOld sprite;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public AnimationFrameOld(int startTime, int duration, SpriteOld sprite) {
			this.startTime	= startTime;
			this.duration	= duration;
			this.sprite		= sprite;
		}

		public AnimationFrameOld(int startTime, int duration, Image image, Rectangle2I sourceRect, Point2I drawOffset) {
			this.startTime	= startTime;
			this.duration	= duration;
			this.sprite		= new SpriteOld(image, sourceRect, drawOffset);
		}

		public AnimationFrameOld(AnimationFrameOld copy) {
			this.startTime	= copy.startTime;
			this.duration	= copy.duration;
			this.sprite		= new SpriteOld(copy.sprite);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int StartTime {
			get { return startTime; }
			set { startTime = value; }
		}
		
		public int EndTime {
			get { return (startTime + duration); }
		}
		
		public int Duration {
			get { return duration; }
			set { duration = value; }
		}

		public SpriteOld Sprite {
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
