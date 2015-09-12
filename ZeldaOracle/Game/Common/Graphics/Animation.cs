using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Graphics {
	
	public class Animation {

		private List<AnimationFrame> frames;
		private int duration;
		private int loops;
		private Animation nextStrip; // This creates a linked list of animations for its variations (like for different directions).


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Animation() {
			frames		= new List<AnimationFrame>();
			duration	= 0;
			loops		= 0;
			nextStrip	= null;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void AddFrame(AnimationFrame frame) {
			int index = 0;
			while (index < frames.Count && frame.StartTime > frames[index].StartTime)
				++index;
			frames.Insert(index, frame);
			duration = Math.Max(duration, frame.StartTime + frame.Duration);
		}

		public void AddFrame(int startTime, int duration, Sprite sprite) {
			AddFrame(new AnimationFrame(startTime, duration, sprite));
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Duration {
			get { return duration; }
			set { duration = value; }
		}

		public List<AnimationFrame> Frames {
			get { return frames; }
			set { frames = value; }
		}

		public Animation NextStrip {
			get { return nextStrip; }
			set { nextStrip = value; }
		}

		public int LoopCount {
			get { return loops; }
			set { loops = value; }
		}
	}
}
