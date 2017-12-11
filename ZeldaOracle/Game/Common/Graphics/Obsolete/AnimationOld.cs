using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Graphics {

	/// <summary>Modes for how looping should be handled for an animation.</summary>
	public enum LoopModeOld {
		/// <summary>Remain on the last frame when completed.</summary>
		Clamp,
		/// <summary>Reset back to the beginning and stop.</summary>
		Reset,
		/// <summary>Keep looping back and playing from the beginning endlessly.</summary>
		Repeat
	}

	public class AnimationOld {

		// The list of frames.
		private List<AnimationFrameOld> frames;
		// Duratin in ticks.
		private int duration;
		// This creates a linked list of animations for its variations (like for different directions).
		private AnimationOld nextStrip;
		// How looping should be handled.
		private LoopModeOld loopMode;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public AnimationOld() {
			this.frames		= new List<AnimationFrameOld>();
			this.duration	= 0;
			this.nextStrip	= null;
			this.loopMode	= LoopModeOld.Repeat;
		}

		public AnimationOld(SpriteOld sprite) {
			this.frames		= new List<AnimationFrameOld>();
			this.duration	= 0;
			this.nextStrip	= null;
			this.loopMode	= LoopModeOld.Repeat;

			this.frames.Add(new AnimationFrameOld(0, 0, sprite));
		}
		
		public AnimationOld(LoopModeOld loopMode) {
			this.frames		= new List<AnimationFrameOld>();
			this.duration	= 0;
			this.nextStrip	= null;
			this.loopMode	= loopMode;
		}

		public AnimationOld(AnimationOld copy) {
			frames		= new List<AnimationFrameOld>();
			nextStrip	= null;
			duration	= copy.duration;
			loopMode	= copy.loopMode;

			for (int i = 0; i < copy.frames.Count; i++)
				frames.Add(new AnimationFrameOld(copy.frames[i]));
			if (copy.nextStrip != null)
				nextStrip = new AnimationOld(copy.nextStrip);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void AddFrame(AnimationFrameOld frame) {
			int index = 0;
			while (index < frames.Count && frame.StartTime >= frames[index].StartTime)
				++index;
			frames.Insert(index, frame);
			duration = Math.Max(duration, frame.StartTime + frame.Duration);
		}

		public void AddFrame(int startTime, int duration, SpriteOld sprite) {
			AddFrame(new AnimationFrameOld(startTime, duration, sprite));
		}

		public void SwitchSpriteSheet(SpriteSheet sheet) {
			foreach (AnimationFrameOld frame in frames) {
				SpriteOld sprite = frame.Sprite;
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

		public SpriteOld GetFrameAsSprite(int time) {
			SpriteOld sprite = null;
			SpriteOld currentSprite = null;
			for (int i = 0; i < frames.Count; i++) {
				if (frames[i].StartTime <= time && frames[i].StartTime + frames[i].Duration - 1 >= time) {
					if (sprite == null) {
						sprite = new SpriteOld(frames[i].Sprite);
						currentSprite = sprite;
					}
					else {
						currentSprite.NextPart = new SpriteOld(frames[i].Sprite);
						currentSprite = currentSprite.NextPart;
					}
				}
			}
			return sprite;
		}

		public AnimationOld GetSubstrip(int index) {
			AnimationOld substrip = this;
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

		public AnimationFrameOld this[int index] {
			get { return frames[index]; }
		}

		public int Duration {
			get { return duration; }
			set { duration = value; }
		}

		public List<AnimationFrameOld> Frames {
			get { return frames; }
			set { frames = value; }
		}

		public AnimationOld NextStrip {
			get { return nextStrip; }
			set { nextStrip = value; }
		}

		public LoopModeOld LoopMode {
			get { return loopMode; }
			set { loopMode = value; }
		}

		public int FrameCount {
			get { return frames.Count; }
		}
	}
}
