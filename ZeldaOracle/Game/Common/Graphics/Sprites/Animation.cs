using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {

	/// <summary>Modes for how looping should be handled for an animation.</summary>
	public enum LoopMode {
		/// <summary>Remain on the last frame when completed.</summary>
		Clamp,
		/// <summary>Reset back to the beginning and stop.</summary>
		Reset,
		/// <summary>Keep looping back and playing from the beginning endlessly.</summary>
		Repeat
	}


	public class Animation : ISprite {

		/// <summary>The list of frames.</summary>
		private List<AnimationFrame> frames;
		/// <summary>Duratin in ticks.</summary>
		private int duration;
		/// <summary>This creates a linked list of animations for its variations (like for different directions).</summary>
		private Animation nextStrip;
		/// <summary>How looping should be handled.</summary>
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

		public Animation(ISprite sprite) {
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
			this.frames		= new List<AnimationFrame>();
			this.nextStrip	= null;
			this.duration	= copy.duration;
			this.loopMode	= copy.loopMode;

			for (int i = 0; i < copy.frames.Count; i++)
				this.frames.Add(new AnimationFrame(copy.frames[i]));
			if (copy.nextStrip != null)
				this.nextStrip = new Animation(copy.nextStrip);
		}

		
		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the drawable parts for the sprite.</summary>
		public IEnumerable<SpritePart> GetParts(SpriteDrawSettings settings) {
			float time = settings.PlaybackTime;
			if (loopMode == LoopMode.Repeat) {
				if (duration == 0)
					time = 0;
				else
					time %= duration;
			}
			for (int i = 0; i < frames.Count; ++i) {
				AnimationFrame frame = frames[i];
				if (time < frame.StartTime)
					yield break;
				if (time < frame.StartTime + frame.Duration || (time >= duration && frame.StartTime + frame.Duration == duration)) {
					foreach (SpritePart sprite in frame.Sprite.GetParts(settings)) {
						SpritePart offsetSprite = sprite;
						offsetSprite.DrawOffset += frame.DrawOffset;
						yield return offsetSprite;
					}
				}
			}
		}

		/// <summary>Clones the sprite.</summary>
		public ISprite Clone() {
			return new Animation(this);
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteDrawSettings settings) {
			Rectangle2I bounds = Rectangle2I.Zero;
			float time = settings.PlaybackTime;
			for (int i = 0; i < frames.Count; ++i) {
				AnimationFrame frame = frames[i];
				if (time < frame.StartTime)
					return bounds;
				if (time < frame.StartTime + frame.Duration || (time >= duration && frame.StartTime + frame.Duration == duration)) {
					if (bounds.IsEmpty)
						bounds = frame.Sprite.GetBounds(settings) + frame.DrawOffset;
					else
						bounds = Rectangle2I.Union(bounds, frame.Sprite.GetBounds(settings) + frame.DrawOffset);
				}
			}
			return bounds;
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I Bounds {
			get {
				Rectangle2I bounds = Rectangle2I.Zero;
				foreach (AnimationFrame frame in frames) {
					if (bounds.IsEmpty)
						bounds = frame.Sprite.Bounds + frame.DrawOffset;
					else
						bounds = Rectangle2I.Union(bounds, frame.Sprite.Bounds + frame.DrawOffset);
				}
				return bounds;
			}
		}

		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public IEnumerable<AnimationFrame> GetFrames() {
			return frames;
		}

		public AnimationFrame GetFrameAt(int index) {
			return frames[index];
		}

		public AnimationFrame LastFrame() {
			return frames[frames.Count - 1];
		}

		public AnimationFrame LastFrameOrDefault() {
			if (frames.Count == 0)
				return new AnimationFrame();
			return frames[frames.Count - 1];
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void ClearFrames() {
			frames.Clear();
			duration = 0;
		}

		public void AddFrameRange(IEnumerable<AnimationFrame> frames) {
			foreach (AnimationFrame frame in frames) {
				AddFrame(new AnimationFrame(frame));
			}
		}

		public void AddFrame(AnimationFrame frame) {
			int index = 0;
			while (index < frames.Count && frame.StartTime > frames[index].StartTime)
				++index;
			frames.Insert(index, frame);
			duration = Math.Max(duration, frame.EndTime);
		}

		public void AddFrame(int startTime, int duration, ISprite sprite) {
			AddFrame(new AnimationFrame(startTime, duration, sprite));
		}

		public void AddFrame(int startTime, int duration, ISprite sprite, Point2I drawOffset) {
			AddFrame(new AnimationFrame(startTime, duration, sprite, drawOffset));
		}

		public void AddFrame(int startTime, int duration, ISpriteSheet source, Point2I index) {
			AddFrame(new AnimationFrame(startTime, duration, source, index));
		}

		public void AddFrame(int startTime, int duration, ISpriteSheet source, Point2I index, Point2I drawOffset) {
			AddFrame(new AnimationFrame(startTime, duration, source, index, drawOffset));
		}

		public void RemoveFrameAt(int index) {
			frames.RemoveAt(index);
		}
		

		//-----------------------------------------------------------------------------
		// Sprites
		//-----------------------------------------------------------------------------

		public CompositeSprite GetFrameAsCompositeSprite(int time) {
			CompositeSprite sprite = new CompositeSprite();
			for (int i = 0; i < frames.Count; i++) {
				if (frames[i].StartTime <= time && frames[i].StartTime + frames[i].Duration - 1 >= time) {
					sprite.AddSprite(frames[i].Sprite, frames[i].DrawOffset);
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
		
		public int Duration {
			get { return duration; }
			set { duration = value; }
		}

		public Animation NextStrip {
			get { return nextStrip; }
			set { nextStrip = value; }
		}

		public LoopMode LoopMode {
			get { return loopMode; }
			set { loopMode = value; }
		}

		public int FrameCount {
			get { return frames.Count; }
		}
	}
}
