using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Common.Graphics {

	/// <summary>An AnimationPlayer can play either an animation or a sprite. Sprites
	/// are considered to be endlessly looped animations with a zero-length duration.</summary>
	public class AnimationPlayer {

		/// <summary>The sprite to play.</summary>
		private ISprite     sprite;
		/// <summary>The actual sub-animation-strip when playing an animation.</summary>	
		private Animation	subStrip;
		/// <summary>The index of the sub-animation-strip.</summary>
		private int			subStripIndex;
		/// <summary>True if the playback is rolling.</summary>
		private bool		isPlaying;
		/// <summary>The playback time in ticks.</summary>
		private float		timer;
		/// <summary>The scalar for how fast playback occurs.</summary>
		private float		speed;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		/// <summary>Constructs the animation player.</summary>
		public AnimationPlayer() {
			sprite			= null;
			subStrip		= null;
			subStripIndex	= 0;
			isPlaying		= false;
			timer			= 0.0f;
			speed			= 1.0f;
		}


		//-----------------------------------------------------------------------------
		// Playback
		//-----------------------------------------------------------------------------

		/// <summary>Change to the given animation without interrupting playback.</summary>
		public void SetAnimation(Animation animation) {
			if (sprite == animation)
				return;

			this.sprite		= animation;
			this.subStrip	= GetSubStrip(subStripIndex);

			if (animation != null) {
				// Change the animation and adjust the playback time
				// based on the new animation's loop mode and duration.
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

		/// <summary>Change to the given sprite without interrupting playback.</summary>
		public void SetSprite(ISprite sprite) {
			if (sprite is Animation) {
				SetAnimation(sprite as Animation);
			}
			else {
				this.sprite     = sprite;
				this.subStrip   = null;
				this.timer      = 0.0f;
				// TODO: this.isPlaying = false; ??
			}
		}

		/// <summary>Play the sprite or animation.</summary>
		public void Play(ISprite sprite) {
			if (sprite is Animation) {
				Animation animation = (Animation) sprite;
				this.sprite     = animation;
				this.subStrip   = GetSubStrip(subStripIndex);
				this.isPlaying  = true;
				this.timer      = 0.0f;
			}
			else {
				this.sprite     = sprite;
				this.subStrip   = null;
				this.isPlaying  = true;
				this.timer      = 0.0f;
			}
		}

		/// <summary>Play the animation from the beginning.</summary>
		public void Play() {
			isPlaying	= true;
			timer		= 0.0f;
		}

		/// <summary>Stop the animation and rewind it to the beginning.</summary>
		public void Stop() {
			isPlaying	= false;
			timer		= 0.0f;
		}

		/// <summary>Pause the animation's playback.</summary>
		public void Pause() {
			Pause(true);
		}

		/// <summary>Resume the animation's playback.</summary>
		public void Resume() {
			Pause(false);
		}

		/// <summary>Pause (true) or unpause (false) the animation's playback.</summary>
		public void Pause(bool isPaused) {
			isPlaying = !isPaused;
		}

		/// <summary>Fast-forward the playback time to the end of the animation.</summary>
		public void SkipToEnd() {
			if (Animation != null)
				timer = Animation.Duration;
			else if (sprite != null)
				timer = 0.0f;
		}

		/// <summary>Stop playing and clear (nullify) the current animation and sprite.</summary>
		public void Clear() {
			sprite		= null;
			subStrip	= null;
			isPlaying	= false;
			timer		= 0.0f;
		}

		/// <summary>Update the animation over the elapsed frames.</summary>
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

		/// <summary>Gets the substrip at the specified index of the playing animation.</summary>
		private Animation GetSubStrip(int index) {
			if (Animation == null)
				return null;

			Animation subStrip = Animation;
			int i;
			for (i = 0; subStrip != null && i < index; i++)
				subStrip = subStrip.NextStrip;
			if (i != index || subStrip == null)
				return Animation; // The index doesn't exist, return the base animation.
			return subStrip;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Get or set the playback speed (default is 1).</summary>
		public float Speed {
			get { return speed; }
			set { speed = value; }
		}

		/// <summary>Get or set the animation playback time.</summary>
		public float PlaybackTime {
			get { return timer; }
			set { timer = value; }
		}

		/// <summary>Gets or sets the sub-strip index of the animation to play.</summary>
		public int SubStripIndex {
			get { return subStripIndex; }
			set {
				if (value != subStripIndex) {
					subStripIndex = value;
					subStrip = GetSubStrip(value);
				}
			}
		}

		/// <summary>Gets or sets whether the animation is playing.</summary>
		public bool IsPlaying {
			get { return isPlaying; }
		}

		/// <summary>Returns true if the animation is done playing.</summary>
		public bool IsDone {
			get {
				if (subStrip == null || !isPlaying || subStrip.LoopMode == LoopMode.Repeat)
					return false;
				return (timer >= subStrip.Duration);
			}
		}

		/// <summary>Get the sub-strip that's currently active.</summary>
		public Animation SubStrip {
			get { return subStrip; }
		}

		/// <summary>Return the sprite.</summary>
		public ISprite Sprite {
			get { return sprite; }
		}

		/// <summary>Return a sprite animation representing this player's
		/// sprite or sub-animation-strip (or NULL).</summary>
		public ISprite SpriteOrSubStrip {
			get {
				if (subStrip != null)
					return subStrip;
				return sprite;
			}
		}

		/// <summary>Get or set the current animation strip.</summary>
		public Animation Animation {
			get { return sprite as Animation; }
		}
	}
}
