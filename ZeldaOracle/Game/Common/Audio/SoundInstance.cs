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

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The sound effect instance class contained by this sound. </summary> */
	private SoundEffectInstance soundInstance;
	/** <summary> The sound class of the sound instance. </summary> */
	private Sound sound;

	// Settings
	/** <summary> The default volume of the sound effect. </summary> */
	private double volume;
	/** <summary> The default pitch of the sound effect. </summary> */
	private double pitch;
	/** <summary> The default pan of the sound effect. </summary> */
	private double pan;
	/** <summary> True if the sound effect is muted. </summary> */
	private bool muted;
	/** <summary> True if the sound effect is looped. </summary> */
	private bool looped;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default sound. </summary> */
	public SoundInstance(SoundEffectInstance soundInstance, Sound sound, bool looped = false, double volume = 1.0, double pitch = 0.0, double pan = 0.0, bool muted = false) {
		// Containment
		this.soundInstance	= soundInstance;
		this.sound			= sound;

		// Settings
		this.volume			= GMath.Clamp(volume, 0.0, 1.0);
		this.pitch			= GMath.Clamp(pitch, -1.0, 1.0);
		this.pan			= GMath.Clamp(pan, -1.0, 1.0);
		this.muted			= false;
		this.looped			= looped;

		this.soundInstance.Volume	= (float)GMath.Clamp(this.sound.Group.GroupVolume * this.volume, 0.0, 1.0);
		//this.soundInstance.Pitch	= (float)GMath.Clamp(this.sound.Group.GroupPitch + this.pitch, -1.0, 1.0);
		//this.soundInstance.Pan		= (float)GMath.Clamp(this.sound.Group.GroupPan + this.pan, -1.0, 1.0);
		//this.soundInstance.IsLooped	= this.looped;

		this.soundInstance.IsLooped = looped;
		if (this.sound.Group.GroupMuted || this.muted)
			this.soundInstance.Volume	= 0.0f;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets the sound effect instance contained by this sound instance. </summary> */
	public SoundEffectInstance SoundEffectInstance {
		get { return soundInstance; }
	}
	/** <summary> Gets the sound containing this sound instance. </summary> */
	public Sound Sound {
		get { return sound; }
	}

	#endregion
	//--------------------------------
	#region Settings

	/** <summary> Gets or sets the default volume of the sound between 0 and 1. </summary> */
	public double Volume {
		get { return volume; }
		set { volume = GMath.Clamp(value, 0.0, 1.0); Update(); }
	}
	/** <summary> Gets or sets the default pitch of the sound between -1 and 1. </summary> */
	public double Pitch {
		get { return pitch; }
		set { pitch = GMath.Clamp(value, -1.0, 1.0); Update(); }
	}
	/** <summary> Gets or sets the default pan of the sound between -1 and 1. </summary> */
	public double Pan {
		get { return pan; }
		set { pan = GMath.Clamp(value, -1.0, 1.0); Update(); }
	}
	/** <summary> Gets or sets if the sound is muted. </summary> */
	public bool IsMuted {
		get { return muted; }
		set { muted = value; Update(); }
	}
	/** <summary> Gets if the sound is looped. </summary> */
	public bool IsLooped {
		get { return looped; }
	}

	#endregion
	//--------------------------------
	#region Playback

	/** <summary> Returns true if the sound is currently playing. </summary> */
	public bool IsPlaying {
		get { return soundInstance.State == SoundState.Playing; }
	}
	/** <summary> Returns true if the sound is currently paused. </summary> */
	public bool IsPaused {
		get { return soundInstance.State == SoundState.Paused; }
	}
	/** <summary> Returns true if the sound is currently stopped. </summary> */
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

	/** <summary> Updates the sound. </summary> */
	internal void Update() {
		soundInstance.Volume	= (float)GMath.Clamp(sound.Group.GroupVolume * volume, 0.0, 1.0);
		//soundInstance.Pitch		= (float)GMath.Clamp(sound.Group.GroupPitch + pitch, -1.0, 1.0);
		//soundInstance.Pan		= (float)GMath.Clamp(sound.Group.GroupPan + pan, -1.0, 1.0);
		//soundInstance.IsLooped	= looped;

		if (sound.Group.GroupMuted || muted)
			soundInstance.Volume	= 0.0f;
	}
	/** <summary> Plays the sound. </summary> */
	public void Play() {
		soundInstance.Play();
	}
	/** <summary> Resumes the sound. </summary> */
	public void Resume() {
		soundInstance.Resume();
	}
	/** <summary> Stops the sound. </summary> */
	public void Stop() {
		soundInstance.Stop();
	}
	/** <summary> Pauses the sound. </summary> */
	public void Pause() {
		soundInstance.Pause();
	}

	#endregion
	//--------------------------------
	#endregion
}
} // End namespace
