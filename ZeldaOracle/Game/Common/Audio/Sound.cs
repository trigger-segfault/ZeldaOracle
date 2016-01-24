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
	 * A structure for storing a sound effect.
	 * </summary>
	 */
	public class Sound {

		// Containment
		// The sound effect class contained by this sound.
		private SoundEffect soundEffect;
		// The name of the sound effect.
		internal string name;
		// The file path of the sound.
		private string filePath;

		// Settings
		// The default volume of the sound effect.
		private float volume;
		// The default pitch of the sound effect.
		private float pitch;
		// The default pan of the sound effect.
		private float pan;
		// True if the sound effect is muted.
		private bool muted;

		private int maxInstances;

		// Sound Instances
		// The list of sound instances currently active.
		private List<SoundInstance> sounds;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		// Constructs the default sound.
		public Sound(SoundEffect soundEffect, string filePath, string name,
			float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f, bool isMuted = false)
		{
			// Containment
			this.soundEffect	= soundEffect;
			this.name			= name;
			this.filePath		= filePath;

			// Settings
			this.volume			= volume;
			this.pitch			= pitch;
			this.pan			= pan;
			this.muted			= isMuted;
			this.maxInstances	= 1;

			// Sound Instances
			this.sounds			= new List<SoundInstance>();
		}


		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		// Updates the sound.
		internal void Update() {
			for (int i = 0; i < sounds.Count; i++) {
				SoundInstance sound = sounds[i];

				if (sound.LoopWhileActive) {
					if (!sound.IsActive)
						sound.Stop();
					else
						sound.IsActive = false;
				}

				if (sound.IsStopped) {
					sounds.RemoveAt(i);
					i--;
				}
			}
		}

		// Updates the sound instances.
		internal void UpdateSoundInstances() {
			for (int i = 0; i < sounds.Count; i++) {
				sounds[i].Update();
			}
		}

		// Remove any extra sound instances currently playing.
		private void RemoveExtraInstances() {
			while (sounds.Count > maxInstances) {
				if (!sounds[0].IsStopped)
					sounds[0].Stop();
				sounds.RemoveAt(0);
			}
		}


		//-----------------------------------------------------------------------------
		// Playback
		//-----------------------------------------------------------------------------

		// Plays the sound.
		public SoundInstance Play(bool looped = false) {
			SoundInstance soundInstance = new SoundInstance(soundEffect.CreateInstance(), this, looped, volume, pitch, pan, muted);
			soundInstance.Play();
			sounds.Add(soundInstance);
			RemoveExtraInstances();
			return soundInstance;
		}

		// Plays the sound.
		public SoundInstance Play(bool looped, float volume, float pitch = 0.0f, float pan = 0.0f, bool muted = false) {
			SoundInstance soundInstance = new SoundInstance(soundEffect.CreateInstance(), this, looped, volume, pitch, pan, muted);
			soundInstance.Play();
			sounds.Add(soundInstance);
			RemoveExtraInstances();
			return soundInstance;
		}

		public void LoopWhileActive() {

			for (int i = 0; i < sounds.Count; i++) {
				if (sounds[i].IsStopped) {
					sounds.RemoveAt(i);
					i--;
				}
				else
					sounds[i].IsActive = true;
			}

			if (sounds.Count == 0) {
				SoundInstance soundInstance = Play(true);
				soundInstance.LoopWhileActive = true;
				soundInstance.IsActive = true;
			}
		}

		// Stops the sound.
		public void Stop() {
			for (int i = 0; i < sounds.Count; i++)
				sounds[i].Stop();
		}

		// Resumes the sound.
		public void Resume() {
			for (int i = 0; i < sounds.Count; i++)
				sounds[i].Resume();
		}

		// Pauses the sound.
		public void Pause() {
			for (int i = 0; i < sounds.Count; i++)
				sounds[i].Pause();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// Information ----------------------------------------------------------------

		// Gets the sound effect contained by this sound.
		public SoundEffect SoundEffect {
			get { return soundEffect; }
		}

		// Gets the name of the sound.
		public string Name {
			get { return name; }
		}

		// Gets the file path of the sound.
		public string FilePath {
			get { return filePath; }
		}

		// Gets the list of sound instances of this sound.
		public SoundInstance[] Instances {
			get {
				SoundInstance[] soundInstanceList = new SoundInstance[sounds.Count];
				sounds.CopyTo(soundInstanceList, 0);
				return soundInstanceList;
			}
		}

		// Settings -------------------------------------------------------------------

		// Gets or sets the default volume of the sound between 0 and 1.
		public float Volume {
			get { return volume; }
			set { volume = GMath.Clamp(value, 0.0f, 1.0f); UpdateSoundInstances(); }
		}

		// Gets or sets the default pitch of the sound between -1 and 1.
		public float Pitch {
			get { return pitch; }
			set { pitch = GMath.Clamp(value, -1.0f, 1.0f); UpdateSoundInstances(); }
		}

		// Gets or sets the default pan of the sound between -1 and 1.
		public float Pan {
			get { return pan; }
			set { pan = GMath.Clamp(value, -1.0f, 1.0f); UpdateSoundInstances(); }
		}

		// Gets or sets if the sound is muted.
		public bool IsMuted {
			get { return muted; }
			set { muted = value; UpdateSoundInstances(); }
		}

		// Gets or sets the maximum number of sound instances playing at once.
		public int MaxInstances {
			get { return maxInstances; }
			set {
				maxInstances = Math.Max(0, value);
				while (sounds.Count > maxInstances) {
					if (!sounds[0].IsStopped)
						sounds[0].Stop();
					sounds.RemoveAt(0);
				}
			}
		}
		
		// Playback -------------------------------------------------------------------

		// Returns true if the sound is currently playing.
		public bool IsPlaying {
			get {
				for (int i = 0; i < sounds.Count; i++) {
					if (sounds[i].IsPlaying)
						return true;
				}
				return false;
			}
		}

		// Returns true if the sound is currently paused.
		public bool IsPaused {
			get {
				for (int i = 0; i < sounds.Count; i++) {
					if (sounds[i].IsPaused)
						return true;
				}
				return false;
			}
		}

		// Returns true if the sound is currently stopped.
		public bool IsStopped {
			get {
				for (int i = 0; i < sounds.Count; i++) {
					if (sounds[i].IsStopped)
						return true;
				}
				return false;
			}
		}

	}
}
