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
 * </summary> */
public class Sound {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members
	
	// Containment
	/** <summary> The sound effect class contained by this sound. </summary> */
	private SoundEffect soundEffect;
	/** <summary> The name of the sound effect. </summary> */
	internal string name;
	/** <summary> The file path of the sound. </summary> */
	private string filePath;

	// Settings
	/** <summary> The default volume of the sound effect. </summary> */
	private float volume;
	/** <summary> The default pitch of the sound effect. </summary> */
	private float pitch;
	/** <summary> The default pan of the sound effect. </summary> */
	private float pan;
	/** <summary> True if the sound effect is muted. </summary> */
	private bool muted;

	// Sound Instances
	/** <summary> The list of sound instances currently active. </summary> */
	private List<SoundInstance> sounds;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default sound. </summary> */
	public Sound(SoundEffect soundEffect, string filePath, string name, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f, bool muted = false) {
		// Containment
		this.soundEffect	= soundEffect;
		this.name			= name;
		this.filePath		= filePath;

		// Settings
		this.volume			= volume;
		this.pitch			= pitch;
		this.pan			= pan;
		this.muted			= muted;

		// Sound Instances
		this.sounds			= new List<SoundInstance>();
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Information

	/** <summary> Gets the sound effect contained by this sound. </summary> */
	public SoundEffect SoundEffect {
		get { return soundEffect; }
	}
	/** <summary> Gets the name of the sound. </summary> */
	public string Name {
		get { return name; }
	}
	/** <summary> Gets the file path of the sound. </summary> */
	public string FilePath {
		get { return filePath; }
	}
	/** <summary> Gets the list of sound instances of this sound. </summary> */
	public SoundInstance[] Instances {
		get {
			SoundInstance[] soundInstanceList = new SoundInstance[sounds.Count];
			sounds.CopyTo(soundInstanceList, 0);
			return soundInstanceList;
		}
	}

	#endregion
	//--------------------------------
	#region Settings

	/** <summary> Gets or sets the default volume of the sound between 0 and 1. </summary> */
	public float Volume {
		get { return volume; }
		set { volume = GMath.Clamp(value, 0.0f, 1.0f); UpdateSoundInstances(); }
	}
	/** <summary> Gets or sets the default pitch of the sound between -1 and 1. </summary> */
	public float Pitch {
		get { return pitch; }
		set { pitch = GMath.Clamp(value, -1.0f, 1.0f); UpdateSoundInstances(); }
	}
	/** <summary> Gets or sets the default pan of the sound between -1 and 1. </summary> */
	public float Pan {
		get { return pan; }
		set { pan = GMath.Clamp(value, -1.0f, 1.0f); UpdateSoundInstances(); }
	}
	/** <summary> Gets or sets if the sound is muted. </summary> */
	public bool IsMuted {
		get { return muted; }
		set { muted = value; UpdateSoundInstances(); }
	}

	#endregion
	//--------------------------------
	#region Playback

	/** <summary> Returns true if the sound is currently playing. </summary> */
	public bool IsPlaying {
		get {
			for (int i = 0; i < sounds.Count; i++) {
				if (sounds[i].IsPlaying)
					return true;
			}
			return false;
		}
	}
	/** <summary> Returns true if the sound is currently paused. </summary> */
	public bool IsPaused {
		get {
			for (int i = 0; i < sounds.Count; i++) {
				if (sounds[i].IsPaused)
					return true;
			}
			return false;
		}
	}
	/** <summary> Returns true if the sound is currently stopped. </summary> */
	public bool IsStopped {
		get {
			for (int i = 0; i < sounds.Count; i++) {
				if (sounds[i].IsStopped)
					return true;
			}
			return false;
		}
	}

	#endregion
	//--------------------------------
	#endregion
	//========== MANAGEMENT ==========
	#region Management
	//--------------------------------
	#region Playback

	/** <summary> Updates the sound. </summary> */
	internal void Update() {
		for (int i = 0; i < sounds.Count; i++) {
			if (sounds[i].IsStopped) {
				sounds.RemoveAt(i);
				i--;
			}
		}
	}
	/** <summary> Updates the sound instances. </summary> */
	internal void UpdateSoundInstances() {
		for (int i = 0; i < sounds.Count; i++) {
			sounds[i].Update();
		}
	}
	/** <summary> Plays the sound. </summary> */
	public SoundInstance Play(bool looped = false) {
		SoundInstance soundInstance = new SoundInstance(soundEffect.CreateInstance(), this, looped, volume, pitch, pan, muted);
		soundInstance.Play();
		sounds.Add(soundInstance);
		if (sounds.Count > 8) {
			if (!sounds[0].IsStopped)
				sounds[0].Stop();
			sounds.RemoveAt(0);
		}
		
		return soundInstance;
	}
	/** <summary> Plays the sound. </summary> */
	public SoundInstance Play(bool looped, float volume, float pitch = 0.0f, float pan = 0.0f, bool muted = false) {
		SoundInstance soundInstance = new SoundInstance(soundEffect.CreateInstance(), this, looped, volume, pitch, pan, muted);
		soundInstance.Play();
		sounds.Add(soundInstance);
		if (sounds.Count > 8) {
			if (!sounds[0].IsStopped)
				sounds[0].Stop();
			sounds.RemoveAt(0);
		}

		return soundInstance;
	}
	/** <summary> Stops the sound. </summary> */
	public void Stop() {
		for (int i = 0; i < sounds.Count; i++) {
			sounds[i].Stop();
		}
	}
	/** <summary> Resumes the sound. </summary> */
	public void Resume() {
		for (int i = 0; i < sounds.Count; i++) {
			sounds[i].Resume();
		}
	}
	/** <summary> Pauses the sound. </summary> */
	public void Pause() {
		for (int i = 0; i < sounds.Count; i++) {
			sounds[i].Pause();
		}
	}

	#endregion
	//--------------------------------
	#endregion
}
} // End namespace
