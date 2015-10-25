using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Content.ResourceBuilders {

	public class AnimationBuilder {
		private Animation	animation;
		private SpriteSheet	sheet;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public AnimationBuilder() {
			animation = null;
			sheet = null;
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

		public AnimationBuilder InsertFrameStrip(int time, int duration, int sheetX, int sheetY, int length, int offsetX = 0, int offsetY = 0, int relX = 1, int relY = 0) {
			for (int i = 0; i < length; ++i)
				InsertFrame(time + (duration * i), duration, sheetX + (i * relX), sheetY + (i * relY), offsetX, offsetY);
			return this;
		}

		public AnimationBuilder AddFrameStrip(int duration, int sheetX, int sheetY, int length, int offsetX = 0, int offsetY = 0, int relX = 1, int relY = 0) {
			return InsertFrameStrip(animation.Duration, duration, sheetX, sheetY, length, offsetX, offsetY, relX, relY);
		}

		public AnimationBuilder AddFrame(int duration, int sheetX, int sheetY, int offsetX = 0, int offsetY = 0) {
			Sprite spr = new Sprite(sheet, sheetX, sheetY, offsetX, offsetY);
			return AddFrame(duration, spr);
		}

		public AnimationBuilder AddFrame(int duration, Sprite sprite) {
			return InsertFrame(animation.Duration, duration, sprite);
		}

		public AnimationBuilder AddPart(int sheetX, int sheetY, int offsetX = 0, int offsetY = 0) {
			return AddPart(new Sprite(sheet, sheetX, sheetY, offsetX, offsetY));
		}

		public AnimationBuilder AddPart(Sprite sprite) {
			//assert(m_strip->getNumFrames() > 0);
			AnimationFrame prevFrame = animation.Frames[animation.Frames.Count - 1];
			return InsertFrame(prevFrame.StartTime, prevFrame.Duration, sprite);
		}

		public AnimationBuilder AddPart(int duration, int sheetX, int sheetY, int offsetX = 0, int offsetY = 0) {
			return AddPart(duration, new Sprite(sheet, sheetX, sheetY, offsetX, offsetY));
		}

		public AnimationBuilder AddPart(int duration, Sprite sprite) {
			//assert(m_strip->getNumFrames() > 0);
			AnimationFrame prevFrame = animation.Frames[animation.Frames.Count - 1];
			return InsertFrame(prevFrame.StartTime, duration, sprite);
		}

		public AnimationBuilder InsertFrame(int time, int duration, int sheetX, int sheetY, int offsetX = 0, int offsetY = 0) {
			return InsertFrame(time, duration, new Sprite(sheet, sheetX, sheetY, offsetX, offsetY));
		}

		public AnimationBuilder InsertFrame(int time, int duration, Sprite sprite) {
			animation.AddFrame(time, duration, sprite);
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
			else {
				animation.NextStrip = new Animation();
				animation = animation.NextStrip;
			}
			return this;
		}

		
		//-----------------------------------------------------------------------------
		// Midifications
		//-----------------------------------------------------------------------------

		public AnimationBuilder RepeatPreviousFrames(int numFrames, int numRepeats) {
			int start = animation.Frames.Count - numFrames;
			for (int i = 0; i < numRepeats; i++) {
				for (int j = 0; j < numFrames; j++) {
					AnimationFrame frame = new AnimationFrame(animation.Frames[start + j]);
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

		public AnimationBuilder SetSheet(SpriteSheet sheet) {
			this.sheet = sheet;
			return this;
		}

		public AnimationBuilder Offset(int x, int y) {
			for (Animation anim = animation; anim != null; anim = anim.NextStrip) {
				for (int i = 0; i < anim.Frames.Count; i++)
					anim.Frames[i].Sprite.DrawOffset += new Point2I(x, y);
			}
			return this;
		}

		public AnimationBuilder ShiftSourcePositions(int x, int y) {
			for (Animation anim = animation; anim != null; anim = anim.NextStrip) {
				for (int i = 0; i < anim.Frames.Count; i++)
					anim.Frames[i].Sprite.SourceRect += new Point2I(x, y) * (SpriteSheet.CellSize + SpriteSheet.Spacing);
			}
			return this;
		}

		public AnimationBuilder MakeQuad() {
			int numFrames = animation.Frames.Count;
			AnimationFrame[] frames = new AnimationFrame[numFrames];

			for (int i = 0; i < numFrames; ++i)
				frames[i] = new AnimationFrame(animation.Frames[i]);

			for (int i = 0; i < numFrames; ++i) {
				for (int x = 0; x < 2; ++x) {
					for (int y = 0; y < 2; ++y) {
						if (x > 0 || y > 0) {
							frames[i].Sprite.DrawOffset = new Point2I(8 * x, 8 * y);
							animation.AddFrame(new AnimationFrame(frames[i]));
						}
					}
				}
			}
			return this;
		}
		
		public AnimationBuilder MakeFlicker(int alternateDelayTicks, bool startOn = true) {

			Animation newAnimation = new Animation();

			for (int i = 0; i < animation.Frames.Count; i++)  {
				AnimationFrame frame = animation.Frames[i];
				
				int beginSection	= frame.StartTime / (alternateDelayTicks * 2);
				int endSection		= frame.EndTime / (alternateDelayTicks * 2);
				if (frame.EndTime % (alternateDelayTicks * 2) == 0)
					endSection--;

				for (int section = beginSection; section <= endSection; section++) {
					int t = section * alternateDelayTicks * 2;

					if (frame.StartTime < t + alternateDelayTicks && frame.EndTime > t) {
						AnimationFrame newFrame = new AnimationFrame();
						newFrame.Sprite		= frame.Sprite;
						newFrame.StartTime	= Math.Max(frame.StartTime, t);
						newFrame.Duration	= Math.Min(frame.EndTime, t + alternateDelayTicks) - newFrame.StartTime;
						newAnimation.AddFrame(newFrame);
					}
				}
			}

			animation.Frames = newAnimation.Frames;
			return this;
		}

		public AnimationBuilder CreateClone(Animation reference) {
			animation = new Animation(reference);
			return this;
		}

		public AnimationBuilder MakeDynamic(int numSubStrips, int offsetX, int offsetY) {
			Animation subStrip = animation;
			Point2I offset = new Point2I(offsetX, offsetY);

			for (int i = 1; i < numSubStrips; i++) {
				subStrip.NextStrip = new Animation();
				subStrip = subStrip.NextStrip;
				subStrip.LoopMode = animation.LoopMode;

				for (int j = 0; j < animation.Frames.Count; j++) {
					AnimationFrame frame = new AnimationFrame(animation.Frames[j]);
					frame.Sprite.SourceRect = new Rectangle2I(
						frame.Sprite.SourceRect.Point + (i * ((sheet.CellSize + sheet.Spacing) * offset)),
						frame.Sprite.SourceRect.Size
					);
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
			get { return sheet; }
			set { sheet = value; }
		}
		
		public Animation Animation {
			get { return animation; }
			set { animation = value; }
		}

	}
}
