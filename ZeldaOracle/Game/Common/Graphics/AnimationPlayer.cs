using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {

	// TODO: Handle playing sprites.
	public class AnimationPlayer {
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
		
		// Play the animation from the beginning.
		public void Play() {
			isPlaying	= true;
			timer		= 0.0f;
		}
		
		// Play the given animation strip from the beginning.
		public void Play(Animation animation) {
			this.animation = animation;
			subStrip	= GetSubStrip(subStripIndex);
			isPlaying	= true;
			timer		= 0.0f;
		}
		
		// Stop the animation and rewind it to the beginning.
		public void Stop() {
			isPlaying = false;
			timer = 0.0f;
		}

		// Pause (true) or unpause (false) the animation's playback.
		public void Pause(bool isPaused) {
			isPlaying = !isPaused;
		}

		// Pause the animation's playback.
		public void Pause() {
			Pause(true);
		}
		
		// Resume the animation's playback.
		public void Resume() {
			Pause(false);
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
			Animation subStrip = animation;
			int i;
			for (i = 0; subStrip != null && i < index; i++)
				subStrip = subStrip.NextStrip;

			if (i != index || subStrip == null)
				return animation; // The index doesn't exist.
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

		// Get the sub-strip that's currently active.
		public Animation SubStrip {
			get { return subStrip; }
		}
		
		// Get or set the playback speed (default is 1).
		public float Speed {
			get { return speed; }
			set { speed = value; }
		}

		// Get or set the animation playback time.
		public float PlaybackTime {
			get { return timer; }
			set {
				timer = value;
			}
		}
		
		// Get or set the current animation strip.
		public Animation Animation {
			get { return animation; }
			set {
				if (animation != value) {
					animation = value;

					if (animation == null) {
						subStrip = null;
						timer = 0.0f;
					}
					else {
						subStrip = GetSubStrip(subStripIndex);

						if (subStrip.LoopMode == LoopMode.Clamp && timer > subStrip.Duration) {
							timer = subStrip.Duration;
						}
						if (subStrip.LoopMode == LoopMode.Reset && timer >= subStrip.Duration) {
							timer = 0;
							isPlaying = false;
						}
						else if (subStrip.LoopMode == LoopMode.Repeat && timer >= subStrip.Duration) {
							if (subStrip.Duration > 0)
								timer %= subStrip.Duration;
							else
								timer = 0.0f;
						}
					}
				}
			}
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
	}
}
