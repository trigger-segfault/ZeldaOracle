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
		/// <summary>Keep looping back and playing from the beginning endlessly. Same as Loop.</summary>
		Repeat,
		/// <summary>Keep looping back and playing from the beginning endlessly. Same as Repeat.</summary>
		Loop,
	}


	/// <summary>A sprite with different combinations of smaller sprites for each frame.</summary>
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

		/// <summary>Constructs an empty animation.</summary>
		public Animation() {
			this.frames		= new List<AnimationFrame>();
			this.duration	= 0;
			this.nextStrip	= null;
			this.loopMode	= LoopMode.Repeat;
		}

		/// <summary>Constructs an animation with a single sprite as a frame.</summary>
		public Animation(ISprite sprite) {
			this.frames		= new List<AnimationFrame>();
			this.duration	= 0;
			this.nextStrip	= null;
			this.loopMode	= LoopMode.Repeat;

			this.frames.Add(new AnimationFrame(0, 1, sprite));
		}

		/// <summary>Constructs an animation with the specified loop mode.</summary>
		public Animation(LoopMode loopMode) {
			this.frames		= new List<AnimationFrame>();
			this.duration	= 0;
			this.nextStrip	= null;
			this.loopMode	= loopMode;
		}

		/// <summary>Constructs a copy of the specified animation.</summary>
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
		public SpritePart GetParts(SpriteSettings settings) {
			float time = settings.PlaybackTime;
			if (loopMode == LoopMode.Repeat) {
				if (duration == 0)
					time = 0;
				else
					time %= duration;
			}
			SpritePart firstPart = null;
			SpritePart nextParts = null;

			for (int i = 0; i < frames.Count; ++i) {
				AnimationFrame frame = frames[i];
				if (time >= frame.StartTime && (time < frame.EndTime || (time >= duration && frame.StartTime + frame.Duration == duration))) {
					nextParts = frame.OffsetSprite.GetParts(settings);
					if (nextParts != null) {
						if (firstPart == null) {
							firstPart = nextParts;
						}
						else {
							firstPart.TailPart.NextPart = nextParts;
							firstPart.TailPart = nextParts.TailPart;
						}
					}
				}
			}
			return firstPart;
		}

		/// <summary>Clones the sprite.</summary>
		public ISprite Clone() {
			return new Animation(this);
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteSettings settings) {
			Rectangle2I bounds = Rectangle2I.Zero;
			// Bounds is based on the entire duration of the animation
			foreach (AnimationFrame frame in frames) {
				if (frame.Sprite == null)
					continue;
				if (bounds.IsEmpty)
					bounds = frame.OffsetSprite.GetBounds(settings);
				else
					bounds = Rectangle2I.Union(bounds, frame.OffsetSprite.GetBounds(settings));
			}
			return bounds;
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I Bounds {
			get {
				Rectangle2I bounds = Rectangle2I.Zero;
				// Bounds is based on the entire duration of the animation
				foreach (AnimationFrame frame in frames) {
					if (frame.Sprite == null)
						continue;
					if (bounds.IsEmpty)
						bounds = frame.OffsetSprite.Bounds;
					else
						bounds = Rectangle2I.Union(bounds, frame.OffsetSprite.Bounds);
				}
				return bounds;
			}
		}

		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Gets the collection of frames in the animation.</summary>
		public IEnumerable<AnimationFrame> GetFrames() {
			return frames;
		}

		/// <summary>Gets the frame at the specified index in the list.</summary>
		public AnimationFrame GetFrameAt(int index) {
			return frames[index];
		}

		/// <summary>Gets the last frame in the list.</summary>
		public AnimationFrame LastFrame() {
			return frames[frames.Count - 1];
		}

		/// <summary>Gets the last frame in the list or returns null if the list is empty.</summary>
		public AnimationFrame LastFrameOrDefault() {
			if (frames.Count == 0)
				return new AnimationFrame();
			return frames[frames.Count - 1];
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Clears the frames from the list.</summary>
		public void ClearFrames() {
			frames.Clear();
			duration = 0;
		}

		/// <summary>Adds a range of frames.</summary>
		public void AddFrameRange(IEnumerable<AnimationFrame> frames) {
			foreach (AnimationFrame frame in frames) {
				AddFrame(new AnimationFrame(frame));
			}
		}

		/// <summary>Adds a frame.</summary>
		public void AddFrame(AnimationFrame frame) {
			int index = 0;
			while (index < frames.Count && frame.Depth >= frames[index].Depth) {
				if (frame.StartTime < frames[index].StartTime && frame.Depth == frames[index].Depth)
					break;
				index++;
			}
			frames.Insert(index, frame);
			duration = GMath.Max(duration, frame.EndTime);
		}

		/// <summary>Adds a frame with the specified settings.</summary>
		public void AddFrame(int startTime, int duration, ISprite sprite, Rectangle2I? clipping = null,
			Flip flip = Flip.None, Rotation rotation = Rotation.None, int depth = 0)
		{
			AddFrame(new AnimationFrame(startTime, duration, sprite, clipping, flip, rotation, depth));
		}

		/// <summary>Adds a frame with the specified settings.</summary>
		public void AddFrame(int startTime, int duration, ISprite sprite, Point2I drawOffset,
			Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None, int depth = 0)
		{
			AddFrame(new AnimationFrame(startTime, duration, sprite, drawOffset, clipping, flip, rotation, depth));
		}

		public void AddFrame(int startTime, int duration, ISpriteSource source, Point2I index,
			Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None, int depth = 0)
		{
			AddFrame(new AnimationFrame(startTime, duration, source, index, clipping, flip, rotation, depth));
		}

		/// <summary>Adds a frame with the specified settings.</summary>
		public void AddFrame(int startTime, int duration, ISpriteSource source, Point2I index,
			string definition, Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None,
			int depth = 0)
		{
			AddFrame(new AnimationFrame(startTime, duration, source, index, definition, clipping, flip, rotation, depth));
		}

		/// <summary>Adds a frame with the specified settings.</summary>
		public void AddFrame(int startTime, int duration, ISpriteSource source, Point2I index,
			Point2I drawOffset, Rectangle2I? clipping = null, Flip flip = Flip.None, Rotation rotation = Rotation.None,
			int depth = 0)
		{
			AddFrame(new AnimationFrame(startTime, duration, source, index, drawOffset, clipping, flip, rotation, depth));
		}

		/// <summary>Adds a frame with the specified settings.</summary>
		public void AddFrame(int startTime, int duration, ISpriteSource source, Point2I index,
			string definition, Point2I drawOffset, Rectangle2I? clipping = null, Flip flip = Flip.None,
			Rotation rotation = Rotation.None, int depth = 0)
		{
			AddFrame(new AnimationFrame(startTime, duration, source, index, definition, drawOffset, clipping, flip, rotation, depth));
		}

		/// <summary>Removes the frame at the specified index.</summary>
		public void RemoveFrameAt(int index) {
			frames.RemoveAt(index);
		}


		//-----------------------------------------------------------------------------
		// Sprites
		//-----------------------------------------------------------------------------

		/// <summary>Gets a snapshot of the animation in time as a composite sprite.</summary>
		public CompositeSprite GetFrameAsCompositeSprite(int time) {
			CompositeSprite sprite = new CompositeSprite();
			for (int i = 0; i < frames.Count; i++) {
				if (frames[i].StartTime <= time && frames[i].StartTime + frames[i].Duration - 1 >= time) {
					sprite.AddSprite(frames[i].Sprite, frames[i].DrawOffset);
				}
			}
			return sprite;
		}

		/// <summary>Gets the substrip of the animation.</summary>
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

		/// <summary>Gets or sets the total duration of the animation.</summary>
		public int Duration {
			get { return duration; }
			set { duration = value; }
		}

		/// <summary>Gets or sets the next substrip in the animation's linked list.</summary>
		public Animation NextStrip {
			get { return nextStrip; }
			set { nextStrip = value; }
		}

		/// <summary>Gets or sets the loop mode of the animation.</summary>
		public LoopMode LoopMode {
			get { return loopMode; }
			set { loopMode = value; }
		}

		/// <summary>Gets the frame count of the animation.</summary>
		public int FrameCount {
			get { return frames.Count; }
		}

		/// <summary>Returns true if the animation has substrips.</summary>
		public bool HasSubstrips {
			get { return nextStrip != null; }
		}

		/// <summary>Gets the number of substrips in teh animation.</summary>
		public int SubstripCount {
			get {
				Animation substrip = this;
				int count = 0;
				do {
					substrip = substrip.nextStrip;
					count++;
				} while (substrip != null);
				return count;
			}
		}
	}
}
