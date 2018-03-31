using Microsoft.Xna.Framework.Audio;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Audio {
	/// <summary>A structure for storing music.</summary>
	public class Music : ZeldaAPI.Music, ISound {
		// Containment
		/// <summary>The base sound effect class contained by this music.</summary>
		private SoundEffect soundEffect;
		/// <summary>The sound effect instance currently playing.</summary>
		private SoundInstance soundInstance;
		/// <summary>The name of the music.</summary>
		internal string name;
		/// <summary>The file path of the music.</summary>
		private string filePath;

		// Settings
		/// <summary>The default volume of the music.</summary>
		private float volume;
		/// <summary>The default pitch of the music.</summary>
		private float pitch;
		/// <summary>The default pan of the music.</summary>
		private float pan;
		/// <summary>True if the music is muted.</summary>
		private bool muted;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the music.</summary>
		public Music(SoundEffect music, string filePath, string name,
			float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f,
			bool muted = false)
		{
			// Containment
			this.soundEffect	= music;
			this.soundInstance	= null;
			this.name			= name;
			this.filePath		= filePath;

			// Settings
			this.volume			= volume;
			this.pitch			= pitch;
			this.pan			= pan;
			this.muted			= false;
		}


		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		/// <summary>Updates the music instance.</summary>
		internal void UpdateSoundInstance() {
			soundInstance?.Update();
		}

		/// <summary>Plays the music.</summary>
		public SoundInstance Play(bool looped = false) {
			soundInstance = new SoundInstance(soundEffect.CreateInstance(),
				this, looped);
			soundInstance.Play();
			return soundInstance;
		}

		/// <summary>Plays the music.</summary>
		public SoundInstance Play(bool looped, float volume, float pitch = 0.0f,
			float pan = 0.0f, bool muted = false)
		{
			soundInstance = new SoundInstance(soundEffect.CreateInstance(),
				this, looped, volume, pitch, pan, muted);
			soundInstance.Play();
			return soundInstance;
		}

		/// <summary>Stops the music.</summary>
		public void Stop() {
			if (soundInstance != null) {
				soundInstance.Stop();
				soundInstance = null;
			}
		}

		/// <summary>Resumes the music.</summary>
		public void Resume() {
			soundInstance.Resume();
		}

		/// <summary>Pauses the music.</summary>
		public void Pause() {
			soundInstance.Pause();
		}
		

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override string ToString() {
			return name;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Containment ----------------------------------------------------------------

		/// <summary>Gets the sound effect contained by this music.</summary>
		public SoundEffect SoundEffect {
			get { return soundEffect; }
		}

		/// <summary>Gets the name of the music.</summary>
		public string Name {
			get { return name; }
		}

		/// <summary>Gets the file path of the music.</summary>
		public string FilePath {
			get { return filePath; }
		}

		// Settings -------------------------------------------------------------------

		/// <summary>Gets or sets the default volume of the music between 0 and 1.</summary>
		public float Volume {
			get { return volume; }
			set {
				volume = GMath.Clamp(value, 0.0f, 1.0f);
				UpdateSoundInstance();
			}
		}

		/// <summary>Gets or sets the default pitch of the music between -1 and 1.</summary>
		public float Pitch {
			get { return pitch; }
			set {
				pitch = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateSoundInstance();
			}
		}

		/// <summary>Gets or sets the default pan of the music between -1 and 1.</summary>
		public float Pan {
			get { return pan; }
			set {
				pan = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateSoundInstance();
			}
		}

		/// <summary>Gets or sets if the music is muted.</summary>
		public bool IsMuted {
			get { return muted; }
			set {
				muted = value;
				UpdateSoundInstance();
			}
		}

		// Playback -------------------------------------------------------------------

		/// <summary>Returns true if the music is currently playing.</summary>
		public bool IsPlaying {
			get { return soundInstance?.IsPlaying ?? false; }
		}

		/// <summary>Returns true if the music is currently paused.</summary>
		public bool IsPaused {
			get { return soundInstance?.IsPaused ?? false; }
		}

		/// <summary>Returns true if the music is currently stopped.</summary>
		public bool IsStopped {
			get { return soundInstance?.IsStopped ?? true; }
		}
	}
}
