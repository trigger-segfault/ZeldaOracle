using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Audio {
	/** <summary>
	 * A structure for storing a sound effect instance.
	 * </summary> */
	public class SoundInstance {

		// Containment
		// The sound effect instance class contained by this sound.
		private SoundEffectInstance soundInstance;
		// The sound class of the sound instance.
		private Sound sound;

		// Settings
		// The default volume of the sound effect.
		private float volume;
		// The default pitch of the sound effect.
		private float pitch;
		// The default pan of the sound effect.
		private float pan;
		// True if the sound effect is muted.
		private bool muted;
		// True if the sound effect is looped.
		private bool looped;

		private bool loopWhileActive;

		private bool isActive;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Constructs the default sound.
		public SoundInstance(SoundEffectInstance soundInstance, Sound sound, bool looped = false, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f, bool muted = false) {
			// Containment
			this.soundInstance	= soundInstance;
			this.sound			= sound;

			// Settings
			this.volume			= GMath.Clamp(volume, 0.0f, 1.0f);
			this.pitch			= GMath.Clamp(pitch, -1.0f, 1.0f);
			this.pan			= GMath.Clamp(pan, -1.0f, 1.0f);
			this.muted			= false;
			this.looped			= looped;
			this.loopWhileActive = false;

			this.soundInstance.Volume	= GMath.Clamp(AudioSystem.SoundVolume * volume, 0.0f, 1.0f);
			this.soundInstance.Pitch	= GMath.Clamp(AudioSystem.MasterPitch + AudioSystem.SoundPitch + pitch, -1.0f, 1.0f);
			this.soundInstance.Pan		= GMath.Clamp(AudioSystem.MasterPan + AudioSystem.SoundPan + pan, -1.0f, 1.0f);
			this.soundInstance.IsLooped	= this.looped;

			if (AudioSystem.IsMasterMuted || AudioSystem.IsSoundMuted || this.muted)
				this.soundInstance.Volume	= 0.0f;
		}


		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		// Updates the sound.
		internal void Update() {
			soundInstance.Volume	= GMath.Clamp(AudioSystem.SoundVolume * volume, 0.0f, 1.0f);
			soundInstance.Pitch		= GMath.Clamp(AudioSystem.MasterPitch + AudioSystem.SoundPitch + pitch, -1.0f, 1.0f);
			soundInstance.Pan		= GMath.Clamp(AudioSystem.MasterPan + AudioSystem.SoundPan + pan, -1.0f, 1.0f);

			if (AudioSystem.IsMasterMuted || AudioSystem.IsSoundMuted || this.muted)
				soundInstance.Volume	= 0.0f;
		}


		//-----------------------------------------------------------------------------
		// Playback
		//-----------------------------------------------------------------------------

		// Plays the sound.
		public void Play() {
			soundInstance.Play();
		}

		// Resumes the sound.
		public void Resume() {
			soundInstance.Resume();
		}

		// Stops the sound.
		public void Stop() {
			soundInstance.Stop();
		}

		// Pauses the sound.
		public void Pause() {
			soundInstance.Pause();
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// Management

		// Gets the sound effect instance contained by this sound instance.
		public SoundEffectInstance SoundEffectInstance {
			get { return soundInstance; }
		}

		// Gets the sound containing this sound instance.
		public Sound Sound {
			get { return sound; }
		}
		
		// Settings

		// Gets or sets the default volume of the sound between 0 and 1.
		public float Volume {
			get { return volume; }
			set { volume = GMath.Clamp(value, 0.0f, 1.0f); Update(); }
		}

		// Gets or sets the default pitch of the sound between -1 and 1.
		public float Pitch {
			get { return pitch; }
			set { pitch = GMath.Clamp(value, -1.0f, 1.0f); Update(); }
		}

		// Gets or sets the default pan of the sound between -1 and 1.
		public float Pan {
			get { return pan; }
			set { pan = GMath.Clamp(value, -1.0f, 1.0f); Update(); }
		}

		// Gets or sets if the sound is muted.
		public bool IsMuted {
			get { return muted; }
			set { muted = value; Update(); }
		}

		// Gets if the sound is looped.
		public bool IsLooped {
			get { return looped; }
		}

		// Gets if the sound is only looped while it is active.
		public bool LoopWhileActive {
			get { return loopWhileActive; }
			set {
				loopWhileActive = value;
				if (value)
					isActive = true;
			}
		}

		// Gets or sets whether the sound is currently active (for loop-while-active).
		public bool IsActive {
			get { return isActive; }
			set { isActive = value; }
		}

		// Playback

		// Returns true if the sound is currently playing.
		public bool IsPlaying {
			get { return soundInstance.State == SoundState.Playing; }
		}

		// Returns true if the sound is currently paused.
		public bool IsPaused {
			get { return soundInstance.State == SoundState.Paused; }
		}

		// Returns true if the sound is currently stopped.
		public bool IsStopped {
			get { return soundInstance.State == SoundState.Stopped; }
		}
	}
}
