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

		// The list of frames.
		private List<AnimationFrame> frames;
		// Duratin in ticks.
		private int duration;
		// This creates a linked list of animations for its variations (like for different directions).
		private Animation nextStrip;
		// How looping should be handled.
		private LoopMode loopMode;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Animation() {
			this.frames		= new List<AnimationFrame>();
			this.duration	= 0;
			this.nextStrip	= null;
			this.loopMode	= LoopMode.Repeat;
		}

		public Animation(Sprite sprite) {
			this.frames		= new List<AnimationFrame>();
			this.duration	= 0;
			this.nextStrip	= null;
			this.loopMode	= LoopMode.Repeat;

			this.frames.Add(new AnimationFrame(0, 0, sprite));
		}
		
		public Animation(LoopMode loopMode) {
			this.frames		= new List<AnimationFrame>();
			this.duration	= 0;
			this.nextStrip	= null;
			this.loopMode	= loopMode;
		}

		public Animation(Animation copy) {
			frames		= new List<AnimationFrame>();
			nextStrip	= null;
			duration	= copy.duration;
			loopMode	= copy.loopMode;

			for (int i = 0; i < copy.frames.Count; i++)
				frames.Add(new AnimationFrame(copy.frames[i]));
			if (copy.nextStrip != null)
				nextStrip = new Animation(copy.nextStrip);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void AddFrame(AnimationFrame frame) {
			int index = 0;
			while (index < frames.Count && frame.StartTime >= frames[index].StartTime)
				++index;
			frames.Insert(index, frame);
			duration = Math.Max(duration, frame.StartTime + frame.Duration);
		}

		public void AddFrame(int startTime, int duration, Sprite sprite) {
			AddFrame(new AnimationFrame(startTime, duration, sprite));
		}

		public void SwitchSpriteSheet(SpriteSheet sheet) {
			foreach (AnimationFrame frame in frames) {
				Sprite sprite = frame.Sprite;
				while (sprite != null) {
					sprite.Image = sheet.Image;
					sprite = sprite.NextPart;
				}
			}
			if (nextStrip != null)
				nextStrip.SwitchSpriteSheet(sheet);
		}


		//-----------------------------------------------------------------------------
		// Sprites
		//-----------------------------------------------------------------------------

		public Sprite GetFrameAsSprite(int time) {
			Sprite sprite = null;
			Sprite currentSprite = null;
			for (int i = 0; i < frames.Count; i++) {
				if (frames[i].StartTime <= time && frames[i].StartTime + frames[i].Duration - 1 >= time) {
					if (sprite == null) {
						sprite = new Sprite(frames[i].Sprite);
						currentSprite = sprite;
					}
					else {
						currentSprite.NextPart = new Sprite(frames[i].Sprite);
						currentSprite = currentSprite.NextPart;
					}
				}
			}
			return sprite;
		}

		public Animation GetSubstrip(int index) {
			Animation substrip = this;
			for (int i = 0; i < index; i++) {
				substrip = substrip.nextStrip;
				if (substrip == null)
					return this;
			}
			return substrip;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public AnimationFrame this[int index] {
			get { return frames[index]; }
		}

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
