using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using XnaSong = Microsoft.Xna.Framework.Media.Song;

using ZeldaOracle.Common.Geometry;
using Song = ZeldaOracle.Common.Audio.Song;

namespace ZeldaOracle.Common.Audio {
	/**<summary>A structure for storing a song.</summary>*/
	public class Song {

		//========== CONSTANTS ===========
		#region Constants

		#endregion
		//=========== MEMBERS ============
		#region Members
	
		// Containment
		/**<summary>The base sound effect class contained by this song.</summary>*/
		private SoundEffect soundEffect;
		/**<summary>The sound effect instance currently playing.</summary>*/
		private SongInstance soundInstance;
		/**<summary>The name of the song.</summary>*/
		internal string name;
		/**<summary>The file path of the song.</summary>*/
		private string filePath;

		// Settings
		/**<summary>The default volume of the song.</summary>*/
		private float volume;
		/**<summary>The default pitch of the song.</summary>*/
		private float pitch;
		/**<summary>The default pan of the song.</summary>*/
		private float pan;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default song.</summary>*/
		public Song(SoundEffect song, string filePath, string name, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f) {
			// Containment
			this.soundEffect	= song;
			this.soundInstance	= null;
			this.name			= name;
			this.filePath		= filePath;

			// Settings
			this.volume			= volume;
			this.pitch			= pitch;
			this.pan			= pan;
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties
		//--------------------------------
		#region Containment

		/**<summary>Gets the sound effect contained by this song.</summary>*/
		public SoundEffect SoundEffect {
			get { return soundEffect; }
		}
		/**<summary>Gets the name of the song.</summary>*/
		public string Name {
			get { return name; }
		}
		/**<summary>Gets the file path of the song.</summary>*/
		public string FilePath {
			get { return filePath; }
		}

		#endregion
		//--------------------------------
		#region Settings

		/**<summary>Gets or sets the default volume of the sound between 0 and 1.</summary>*/
		public float Volume {
			get { return volume; }
			set {
				volume = GMath.Clamp(value, 0.0f, 1.0f);
				AudioSystem.UpdateMusic();
			}
		}
		/**<summary>Gets or sets the default pitch of the sound between -1 and 1.</summary>*/
		public float Pitch {
			get { return pitch; }
			set {
				pitch = GMath.Clamp(value, -1.0f, 1.0f);
				AudioSystem.UpdateMusic();
			}
		}
		/**<summary>Gets or sets the default pan of the sound between -1 and 1.</summary>*/
		public float Pan {
			get { return pan; }
			set {
				pan = GMath.Clamp(value, -1.0f, 1.0f);
				AudioSystem.UpdateMusic();
			}
		}

		#endregion
		//--------------------------------
		#region Playback

		/**<summary>Returns true if the song is currently playing.</summary>*/
		public bool IsPlaying {
			get {
				if (soundInstance != null)
					return soundInstance.IsPlaying;
				return false;
			}
		}
		/**<summary>Returns true if the song is currently paused.</summary>*/
		public bool IsPaused {
			get {
				if (soundInstance != null)
					return soundInstance.IsPaused;
				return false;
			}
		}
		/**<summary>Returns true if the song is currently stopped.</summary>*/
		public bool IsStopped {
			get {
				if (soundInstance != null)
					return soundInstance.IsStopped;
				return true;
			}
		}

		#endregion
		//--------------------------------
		#endregion
		//========== MANAGEMENT ==========
		#region Management
		//--------------------------------
		#region Playback

		/**<summary>Updates the sound instance.</summary>*/
		internal void UpdateSoundInstance() {
			if (soundInstance != null)
				soundInstance.Update();
		}
		/**<summary>Plays the song.</summary>*/
		public SongInstance Play(bool looped = false) {
			soundInstance = new SongInstance(soundEffect.CreateInstance(), this, looped);
			soundInstance.Play();
			return soundInstance;
		}
		/**<summary>Plays the song.</summary>*/
		public SongInstance Play(bool looped, float volume, float pitch = 0.0f, float pan = 0.0f) {
			soundInstance = new SongInstance(soundEffect.CreateInstance(), this, looped, volume, pitch, pan);
			soundInstance.Play();
			return soundInstance;
		}
		/**<summary>Stops the song.</summary>*/
		public void Stop() {
			if (soundInstance != null) {
				soundInstance.Stop();
				soundInstance = null;
			}
		}
		/**<summary>Resumes the song.</summary>*/
		public void Resume() {
			soundInstance.Resume();
		}
		/**<summary>Pauses the song.</summary>*/
		public void Pause() {
			soundInstance.Pause();
		}

		#endregion
		//--------------------------------
		#endregion
	}
} // End namespace
