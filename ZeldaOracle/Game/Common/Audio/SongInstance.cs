using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Audio {
	/**<summary>A structure for storing a sound effect instance for a song.</summary>*/
	public class SongInstance {

		//========== CONSTANTS ===========
		#region Constants

		#endregion
		//=========== MEMBERS ============
		#region Members

		// Containment
		/**<summary>The sound effect instance class contained by this sound.</summary>*/
		private SoundEffectInstance soundInstance;
		/**<summary>The song class of the sound instance.</summary>*/
		private Song song;

		// Settings
		/**<summary>The default volume of the sound effect.</summary>*/
		private float volume;
		/**<summary>The default pitch of the sound effect.</summary>*/
		private float pitch;
		/**<summary>The default pan of the sound effect.</summary>*/
		private float pan;
		/**<summary>True if the sound effect is muted.</summary>*/
		private bool muted;
		/**<summary>True if the sound effect is looped.</summary>*/
		private bool looped;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default sound.</summary>*/
		public SongInstance(SoundEffectInstance soundInstance, Song song, bool looped = false, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f) {
			// Containment
			this.soundInstance	= soundInstance;
			this.song			= song;

			// Settings
			this.volume			= GMath.Clamp(volume, 0.0f, 1.0f);
			this.pitch			= GMath.Clamp(pitch, -1.0f, 1.0f);
			this.pan			= GMath.Clamp(pan, -1.0f, 1.0f);
			this.muted			= false;
			this.looped			= looped;

			this.soundInstance.Volume	= GMath.Clamp(AudioSystem.MusicVolume * volume, 0.0f, 1.0f);
			this.soundInstance.Pitch	= GMath.Clamp(AudioSystem.MasterPitch + AudioSystem.MusicPitch + pitch, -1.0f, 1.0f);
			this.soundInstance.Pan		= GMath.Clamp(AudioSystem.MasterPan + AudioSystem.MusicPan + pan, -1.0f, 1.0f);
			this.soundInstance.IsLooped	= this.looped;

			if (AudioSystem.IsMasterMuted || AudioSystem.IsMusicMuted || this.muted)
				this.soundInstance.Volume	= 0.0f;
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties
		//--------------------------------
		#region Containment

		/**<summary>Gets the sound effect instance contained by this sound instance.</summary>*/
		public SoundEffectInstance SoundEffectInstance {
			get { return soundInstance; }
		}
		/**<summary>Gets the song containing this sound instance.</summary>*/
		public Song Song {
			get { return song; }
		}

		#endregion
		//--------------------------------
		#region Settings

		/**<summary>Gets or sets the default volume of the sound between 0 and 1.</summary>*/
		public float Volume {
			get { return volume; }
			set { volume = GMath.Clamp(value, 0.0f, 1.0f); Update(); }
		}
		/**<summary>Gets or sets the default pitch of the sound between -1 and 1.</summary>*/
		public float Pitch {
			get { return pitch; }
			set { pitch = GMath.Clamp(value, -1.0f, 1.0f); Update(); }
		}
		/**<summary>Gets or sets the default pan of the sound between -1 and 1.</summary>*/
		public float Pan {
			get { return pan; }
			set { pan = GMath.Clamp(value, -1.0f, 1.0f); Update(); }
		}
		/**<summary>Gets or sets if the sound is muted.</summary>*/
		public bool IsMuted {
			get { return muted; }
			set { muted = value; Update(); }
		}
		/**<summary>Gets if the sound is looped.</summary>*/
		public bool IsLooped {
			get { return looped; }
		}

		#endregion
		//--------------------------------
		#region Playback

		/**<summary>Returns true if the sound is currently playing.</summary>*/
		public bool IsPlaying {
			get { return soundInstance.State == SoundState.Playing; }
		}
		/**<summary>Returns true if the sound is currently paused.</summary>*/
		public bool IsPaused {
			get { return soundInstance.State == SoundState.Paused; }
		}
		/**<summary>Returns true if the sound is currently stopped.</summary>*/
		public bool IsStopped {
			get { return soundInstance.State == SoundState.Stopped; }
		}

		#endregion
		//--------------------------------
		#endregion
		//========== MANAGEMENT ==========
		#region Management
		//--------------------------------
		#region Playback

		/**<summary>Updates the sound.</summary>*/
		internal void Update() {
			soundInstance.Volume	= GMath.Clamp(AudioSystem.MusicVolume * volume, 0.0f, 1.0f);
			soundInstance.Pitch		= GMath.Clamp(AudioSystem.MasterPitch + AudioSystem.MusicPitch + pitch, -1.0f, 1.0f);
			soundInstance.Pan		= GMath.Clamp(AudioSystem.MasterPan + AudioSystem.MusicPan + pan, -1.0f, 1.0f);

			if (AudioSystem.IsMasterMuted || AudioSystem.IsMusicMuted || this.muted)
				soundInstance.Volume	= 0.0f;
		}
		/**<summary>Plays the sound.</summary>*/
		public void Play() {
			soundInstance.Play();
		}
		/**<summary>Resumes the sound.</summary>*/
		public void Resume() {
			soundInstance.Resume();
		}
		/**<summary>Stops the sound.</summary>*/
		public void Stop() {
			soundInstance.Stop();
		}
		/**<summary>Pauses the sound.</summary>*/
		public void Pause() {
			soundInstance.Pause();
		}

		#endregion
		//--------------------------------
		#endregion
	}
} // End namespace
