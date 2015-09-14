using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Graphics {
	

	// Modes for how looping should be handled for an animation.
	public enum LoopMode {
		Clamp,	// Remain on the last frame when completed.
		Reset,	// Reset back to the beginning and stop.
		Repeat,	// Keep looping back and playing from the beginning endlessly.
	}


	public class Animation {

		private List<AnimationFrame> frames; // The list of frames.
		private int			duration; // Duratin in ticks.
		private Animation	nextStrip;	// This creates a linked list of animations for its variations (like for different directions).
		private LoopMode	loopMode;	// How looping should be handled.


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Animation() {
			frames		= new List<AnimationFrame>();
			duration	= 0;
			nextStrip	= null;
			loopMode	= LoopMode.Repeat;
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

		public LoopMode LoopMode {
			get { return loopMode; }
			set { loopMode = value; }
		}
	}
}
