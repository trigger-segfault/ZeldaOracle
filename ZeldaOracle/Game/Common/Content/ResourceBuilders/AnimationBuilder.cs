using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Common.Content.ResourceBuilders {

	public class AnimationBuilder {
		
		private Animation			animation;
		private ISpriteSheet		source;
		private SpritePaletteArgs	paletteArgs;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public AnimationBuilder() {
			animation = null;
			source = null;
		}
		


		//-----------------------------------------------------------------------------
		// Begin/End
		//-----------------------------------------------------------------------------

		public AnimationBuilder Begin(Animation animation) {
			this.animation = animation;
			return this;
		}

		public AnimationBuilder Begin() {
			animation = new Animation();
			return this;
		}

		public AnimationBuilder BeginNull() {
			animation = null;
			return this;
		}

		public Animation End()
		{
			Animation temp = animation;
			animation = null;
			return temp;
		}

		//-----------------------------------------------------------------------------
		// Building
		//-----------------------------------------------------------------------------

		public AnimationBuilder InsertFrameStrip(int time, int duration, Point2I index, int length, Point2I drawOffset, Point2I relative) {
			if (relative.IsZero)
				relative = new Point2I(1, 0);
			for (int i = 0; i < length; ++i)
				InsertFrame(time + (duration * i), duration, index + (i * relative), drawOffset);
			return this;
		}

		public AnimationBuilder AddFrameStrip(int duration, Point2I index, int length, Point2I drawOffset, Point2I relative) {
			return InsertFrameStrip(animation.Duration, duration, index, length, drawOffset, relative);
		}

		public AnimationBuilder AddFrame(int duration, Point2I index, Point2I drawOffset) {
			return InsertFrame(animation.Duration, duration, index, drawOffset);
		}

		public AnimationBuilder AddFrame(int duration, ISprite sprite) {
			return InsertFrame(animation.Duration, duration, sprite, Point2I.Zero);
		}

		public AnimationBuilder AddFrame(int duration, ISprite sprite, Point2I drawOffset) {
			return InsertFrame(animation.Duration, duration, sprite, drawOffset);
		}

		public AnimationBuilder AddEmptyFrame(int duration) {
			return AddFrame(duration, new EmptySprite());
		}

		public AnimationBuilder AddPart(Point2I index, Point2I drawOffset) {
			AnimationFrame prevFrame = animation.LastFrame();
			return AddPart(prevFrame.Duration, index, drawOffset);
		}

		public AnimationBuilder AddPart(ISprite sprite) {
			//assert(m_strip->getNumFrames() > 0);
			AnimationFrame prevFrame = animation.LastFrame();
			return InsertFrame(prevFrame.StartTime, prevFrame.Duration, sprite);
		}

		public AnimationBuilder AddPart(ISprite sprite, Point2I drawOffset) {
			//assert(m_strip->getNumFrames() > 0);
			AnimationFrame prevFrame = animation.LastFrame();
			return InsertFrame(prevFrame.StartTime, prevFrame.Duration, sprite, drawOffset);
		}

		public AnimationBuilder AddPart(int duration, Point2I index, Point2I drawOffset) {
			AnimationFrame prevFrame = animation.LastFrame();
			return InsertFrame(prevFrame.StartTime, prevFrame.Duration, index, drawOffset);
		}

		public AnimationBuilder AddPart(int duration, ISprite sprite) {
			return AddPart(duration, sprite, Point2I.Zero);
		}

		public AnimationBuilder AddPart(int duration, ISprite sprite, Point2I drawOffset) {
			//assert(m_strip->getNumFrames() > 0);
			AnimationFrame prevFrame = animation.LastFrame();
			return InsertFrame(prevFrame.StartTime, duration, sprite, drawOffset);
		}
		
		public AnimationBuilder InsertFrame(int time, int duration, Point2I index, Point2I drawOffset) {
			if (paletteArgs.Dictionary != null && source is SpriteSheet) {
				paletteArgs.Image = SpriteSheet.Image;
				paletteArgs.SourceRect = SpriteSheet.GetSourceRect(index);
				ISprite sprite = Resources.PalettedSpriteDatabase.AddSprite(paletteArgs);
				return InsertFrame(time, duration, sprite, drawOffset);
			}
			else {
				animation.AddFrame(time, duration, source, index, drawOffset);
				return this;
			}
		}

		public AnimationBuilder InsertFrame(int time, int duration, ISprite sprite) {
			return InsertFrame(time, duration, sprite, Point2I.Zero);
		}

		public AnimationBuilder InsertFrame(int time, int duration, ISprite sprite, Point2I drawOffset) {
			animation.AddFrame(time, duration, sprite, drawOffset);
			return this;
		}

		public AnimationBuilder AddDelay(int duration) {
			animation.Duration += duration;
			return this;
		}

		public AnimationBuilder CreateSubStrip() {
			if (animation == null) {
				animation = new Animation();
			}
			else if (animation.FrameCount > 0 || animation.Duration > 0) {
				animation.NextStrip = new Animation();
				animation = animation.NextStrip;
			}
			return this;
		}

		
		//-----------------------------------------------------------------------------
		// Midifications
		//-----------------------------------------------------------------------------

		public AnimationBuilder RepeatPreviousFrames(int numFrames, int numRepeats) {
			int start = animation.FrameCount - numFrames;
			for (int i = 0; i < numRepeats; i++) {
				for (int j = 0; j < numFrames; j++) {
					AnimationFrame frame = new AnimationFrame(animation.GetFrameAt(start + j));
					frame.StartTime = animation.Duration;
					animation.AddFrame(frame);
				}
			}
			return this;
		}

		public AnimationBuilder SetDuration(int duration) {
			animation.Duration = duration;
			return this;
		}

		public AnimationBuilder SetLoopMode(LoopMode loopMode) {
			animation.LoopMode = loopMode;
			return this;
		}

		public AnimationBuilder Offset(Point2I offset) {
			for (Animation anim = animation; anim != null; anim = anim.NextStrip) {
				for (int i = 0; i < anim.FrameCount; i++)
					anim.GetFrameAt(i).DrawOffset += offset;
			}
			return this;
		}

		public AnimationBuilder ShiftSourcePositions(Point2I offset) {
			for (Animation anim = animation; anim != null; anim = anim.NextStrip) {
				for (int i = 0; i < anim.FrameCount; i++)
					anim.GetFrameAt(i).DrawOffset += offset * (SpriteSheet.CellSize + SpriteSheet.Spacing);
			}
			return this;
		}

		public AnimationBuilder MakeQuad() {
			int numFrames = animation.FrameCount;
			AnimationFrame[] frames = new AnimationFrame[numFrames];

			for (int i = 0; i < numFrames; ++i)
				frames[i] = new AnimationFrame(animation.GetFrameAt(i));

			for (int i = 0; i < numFrames; ++i) {
				for (int x = 0; x < 2; ++x) {
					for (int y = 0; y < 2; ++y) {
						if (x > 0 || y > 0) {
							frames[i].DrawOffset = new Point2I(8 * x, 8 * y);
							animation.AddFrame(new AnimationFrame(frames[i]));
						}
					}
				}
			}
			return this;
		}
		
		public AnimationBuilder MakeFlicker(int alternateDelayTicks, bool startOn = true) {

			Animation newAnimation = new Animation();

			for (int i = 0; i < animation.FrameCount; i++)  {
				AnimationFrame frame = animation.GetFrameAt(i);
				
				int beginSection	= frame.StartTime / (alternateDelayTicks * 2);
				int endSection		= frame.EndTime / (alternateDelayTicks * 2);
				if (frame.EndTime % (alternateDelayTicks * 2) == 0)
					endSection--;

				for (int section = beginSection; section <= endSection; section++) {
					int t = section * alternateDelayTicks * 2;

					if (frame.StartTime < t + alternateDelayTicks && frame.EndTime > t) {
						AnimationFrame newFrame = new AnimationFrame(frame);
						//newFrame.Sprite		= frame.Sprite;
						newFrame.StartTime	= Math.Max(frame.StartTime, t);
						newFrame.Duration	= Math.Min(frame.EndTime, t + alternateDelayTicks) - newFrame.StartTime;
						newAnimation.AddFrame(newFrame);
					}
				}
			}

			animation.ClearFrames();
			animation.AddFrameRange(newAnimation.GetFrames());
			return this;
		}

		public AnimationBuilder CreateClone(Animation reference) {
			animation = new Animation(reference);
			return this;
		}

		public AnimationBuilder MakeDynamic(int numSubStrips, Point2I offset) {
			Animation subStrip = animation;

			for (int i = 1; i < numSubStrips; i++) {
				subStrip.NextStrip = new Animation();
				subStrip = subStrip.NextStrip;
				subStrip.LoopMode = animation.LoopMode;

				for (int j = 0; j < animation.FrameCount; j++) {
					AnimationFrame frame = new AnimationFrame(animation.GetFrameAt(j));
					if (frame.HasSource) {
						frame.SourceIndex += offset * i;
					}
					subStrip.AddFrame(frame);
				}
			}
			animation = subStrip;
			return this;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SpriteSheet SpriteSheet {
			get { return source as SpriteSheet; }
			set { source = value; }
		}
		public SpriteSet SpriteSet {
			get { return source as SpriteSet; }
			set { source = value; }
		}

		public ISpriteSheet Source {
			get { return source; }
			set { source = value; }
		}

		public Animation Animation {
			get { return animation; }
			set { animation = value; }
		}

		public SpritePaletteArgs PaletteArgs {
			get { return paletteArgs; }
			set { paletteArgs = value; }
		}
	}
}
