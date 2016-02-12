using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {

	// TODO: Handle playing sprites.
	public class AnimationPlayer {
		private Sprite		sprite;			// The sprite to play.
		private Animation	animation;		// The animation to play.
		private Animation	subStrip;		// The actual sub-animation-strip that's playing.
		private int			subStripIndex;	// The index of the sub-animation-strip.
		private bool		isPlaying;		// True if the playback is rolling.
		private float		timer;			// The playback time in ticks.
		private float		speed;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public AnimationPlayer() {
			sprite			= null;
			animation		= null;
			subStrip		= null;
			subStripIndex	= 0;
			isPlaying		= false;
			timer			= 0.0f;
			speed			= 1.0f;
		}


		//-----------------------------------------------------------------------------
		// Playback
		//-----------------------------------------------------------------------------
		
		public void SetAnimation(Animation animation) {
			if (this.animation == animation)
				return;

			this.animation	= animation;
			this.subStrip	= GetSubStrip(subStripIndex);
			this.sprite		= null;

			if (animation != null) {
				if (subStrip.LoopMode == LoopMode.Clamp && timer > subStrip.Duration) {
					timer = subStrip.Duration;
				}
				else if (subStrip.LoopMode == LoopMode.Reset && timer >= subStrip.Duration) {
					timer		= 0.0f;
					isPlaying	= false;
				}
				else if (subStrip.LoopMode == LoopMode.Repeat && timer >= subStrip.Duration) {
					if (subStrip.Duration > 0)
						timer %= subStrip.Duration;
					else
						timer = 0.0f;
				}
			}
			else {
				timer		= 0.0f;
				isPlaying	= false;
			}
		}

		public void SetSprite(Sprite sprite) {
			this.sprite		= sprite;
			this.animation	= null;
			this.subStrip	= null;
			this.timer		= 0.0f;
			// TODO: this.isPlaying = false; ??
		}
		
		// Play the given sprite as a repeated animation with no duration.
		public void Play(Sprite sprite) {
			this.sprite		= sprite;
			this.animation	= null;
			this.subStrip	= null;
			this.isPlaying	= true;
			this.timer		= 0.0f;
		}
		
		// Play the given animation strip from the beginning.
		public void Play(Animation animation) {
			this.animation	= animation;
			this.sprite		= null;
			this.subStrip	= GetSubStrip(subStripIndex);
			this.isPlaying	= true;
			this.timer		= 0.0f;
		}

		// Play the animation from the beginning.
		public void Play() {
			isPlaying	= true;
			timer		= 0.0f;
		}
		
		// Stop the animation and rewind it to the beginning.
		public void Stop() {
			isPlaying	= false;
			timer		= 0.0f;
		}

		// Pause the animation's playback.
		public void Pause() {
			Pause(true);
		}
		
		// Resume the animation's playback.
		public void Resume() {
			Pause(false);
		}

		// Pause (true) or unpause (false) the animation's playback.
		public void Pause(bool isPaused) {
			isPlaying = !isPaused;
		}

		// Fast-forward the playback time to the end of the animation.
		public void SkipToEnd() {
			if (animation != null)
				timer = animation.Duration;
			else if (sprite != null)
				timer = 0.0f;
		}

		// Stop playing and clear (nullify) the current animation and sprite.
		public void Clear() {
			sprite		= null;
			animation	= null;
			subStrip	= null;
			isPlaying	= false;
			timer		= 0.0f;
		}

		// Update the animation over the elapsed frames.
		public void Update() {
			if (isPlaying && subStrip != null) {
				timer += (1.0f * speed);
				
				if (subStrip.LoopMode == LoopMode.Reset && timer >= subStrip.Duration) {
					//Stop();
				}
				else if (subStrip.LoopMode == LoopMode.Repeat && timer >= subStrip.Duration) {
					// Loop back to the beginning.
					if (subStrip.Duration > 0)
						timer %= subStrip.Duration;
					else
						timer = 0.0f;
				}
				else if (subStrip.LoopMode == LoopMode.Clamp && timer > subStrip.Duration) {
					// Hang on the last frame.
					timer = subStrip.Duration;
				}	
			}
		}
		

		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private Animation GetSubStrip(int index) {
			if (animation == null)
				return null;

			Animation subStrip = animation;
			int i;
			for (i = 0; subStrip != null && i < index; i++)
				subStrip = subStrip.NextStrip;
			if (i != index || subStrip == null)
				return animation; // The index doesn't exist, return the base animation.
			return subStrip;
		}



		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// Gets or sets whether the animation is playing.
		public bool IsPlaying {
			get { return isPlaying; }
			set { isPlaying = value; }
		}
		
		// Returns true if the animation is done playing.
		public bool IsDone {
			get {
				if (subStrip == null || !isPlaying || subStrip.LoopMode == LoopMode.Repeat)
					return false;
				return (timer >= subStrip.Duration);
			}
		}
		
		// Get or set the playback speed (default is 1).
		public float Speed {
			get { return speed; }
			set { speed = value; }
		}

		// Get or set the animation playback time.
		public float PlaybackTime {
			get { return timer; }
			set { timer = value; }
		}

		// Get the sub-strip that's currently active.
		public Animation SubStrip {
			get { return subStrip; }
		}

		// Gets or sets the sub-strip index of the animation to play.
		public int SubStripIndex {
			get { return subStripIndex; }
			set {
				if (value != subStripIndex) {
					subStripIndex = value;
					subStrip = GetSubStrip(value);
				}
			}
		}
		
		// Get or set the current animation strip.
		public Animation Animation {
			get { return animation; }
			set { SetAnimation(value); }
		}
	}
}
