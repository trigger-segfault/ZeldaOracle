using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>A single frame with a sprite and time for an animation.</summary>
	public class AnimationFrame {
		/// <summary>Start time in ticks.</summary>
		private int startTime;
		/// <summary>Duration in ticks.</summary>
		private int duration;
		/// <summary>The sprite used in the frame.</summary>
		private ISprite sprite;
		/// <summary>The draw offset of the frame.</summary>
		private Point2I drawOffset;
		/// <summary>The source of the sprite.</summary>
		private ISpriteSheet source;
		/// <summary>The source index of the sprite.</summary>
		private Point2I sourceIndex;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public AnimationFrame() {
			this.startTime		= 0;
			this.duration		= 0;
			this.sprite			= null;
			this.drawOffset		= Point2I.Zero;
			this.source         = null;
			this.sourceIndex    = Point2I.Zero;
		}

		public AnimationFrame(int startTime, int duration, ISprite sprite) {
			this.startTime		= startTime;
			this.duration		= duration;
			this.sprite			= sprite;
			this.drawOffset		= Point2I.Zero;
			this.source			= null;
			this.sourceIndex	= Point2I.Zero;
		}

		public AnimationFrame(int startTime, int duration, ISprite sprite, Point2I drawOffset) {
			this.startTime		= startTime;
			this.duration		= duration;
			this.sprite			= sprite;
			this.drawOffset		= drawOffset;
			this.source			= null;
			this.sourceIndex	= Point2I.Zero;
		}

		public AnimationFrame(int startTime, int duration, ISpriteSheet source, Point2I sourceIndex) {
			this.startTime		= startTime;
			this.duration		= duration;
			this.sprite			= source.GetSprite(sourceIndex);
			this.drawOffset		= Point2I.Zero;
			this.source			= source;
			this.sourceIndex	= sourceIndex;
		}

		public AnimationFrame(int startTime, int duration, ISpriteSheet source, Point2I sourceIndex, Point2I drawOffset) {
			this.startTime		= startTime;
			this.duration		= duration;
			this.sprite			= source.GetSprite(sourceIndex);
			this.drawOffset		= drawOffset;
			this.source			= source;
			this.sourceIndex	= sourceIndex;
		}

		public AnimationFrame(AnimationFrame copy) {
			this.startTime		= copy.startTime;
			this.duration		= copy.duration;
			this.sprite			= copy.sprite;
			this.drawOffset		= copy.drawOffset;
			this.source			= copy.source;
			this.sourceIndex	= copy.sourceIndex;
		}


		//-----------------------------------------------------------------------------
		// Source
		//-----------------------------------------------------------------------------

		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the start time in ticks.</summary>
		public int StartTime {
			get { return startTime; }
			set { startTime = value; }
		}

		/// <summary>Gets the end time in ticks.</summary>
		public int EndTime {
			get { return (startTime + duration); }
		}

		/// <summary>Gets or sets the duration time in ticks.</summary>
		public int Duration {
			get { return duration; }
			set { duration = value; }
		}

		/// <summary>Gets or sets the sprite of the frame.</summary>
		public ISprite Sprite {
			get { return sprite; }
			set {
				sprite = value;
				source = null;
				sourceIndex = Point2I.Zero;
			}
		}

		/// <summary>Gets or sets the draw offset of the frame.</summary>
		public Point2I DrawOffset {
			get { return drawOffset; }
			set { drawOffset = value; }
		}

		/// <summary>Returns true if the animation frame has a sprite sheet source.</summary>
		public bool HasSource {
			get { return source != null; }
		}

		/// <summary>Gets or sets the source of the sprite.</summary>
		public ISpriteSheet Source {
			get { return source; }
			set {
				source = value;
				if (value == null)
					sourceIndex = Point2I.Zero;
				else
					sprite = source.GetSprite(sourceIndex);
			}
		}

		/// <summary>Gets or sets the source index of the sprite.</summary>
		public Point2I SourceIndex {
			get { return sourceIndex; }
			set {
				if (source != null) {
					sourceIndex = value;
					sprite = source.GetSprite(value);
				}
			}
		}
	}
}
