using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Audio {
/** <summary>
 * A structure for storing a catagorized group of sounds.
 * </summary> */
public class SoundGroup {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The root sound group containing all sound groups. </summary> */
	private SoundGroup root;
	/** <summary> The parent sound group containing this group. </summary> */
	private SoundGroup parent;
	/** <summary> The name of the sound group. </summary> */
	private string name;

	// Settings
	/** <summary> The default volume of the sound group. </summary> */
	private double volume;
	/** <summary> The default pitch of the sound group. </summary> */
	private double pitch;
	/** <summary> The default pan of the sound group. </summary> */
	private double pan;
	/** <summary> True if the group is muted. </summary> */
	private bool muted;

	// Groups
	/** <summary> The list of sound groups contained in this group. </summary> */
	private Dictionary<string, SoundGroup> groups;
	/** <summary> The list of sounds contained in this group. </summary> */
	private Dictionary<string, Sound> sounds;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default sound. </summary> */
	public SoundGroup(string name, double volume = 1.0, double pitch = 0.0, double pan = 0.0, bool muted = false) {
		// Containment
		this.root			= null;
		this.parent			= null;
		this.name			= name;

		// Settings
		this.volume			= GMath.Clamp(volume, 0.0, 1.0);
		this.pitch			= GMath.Clamp(pitch, -1.0, 1.0);
		this.pan			= GMath.Clamp(pan, -1.0, 1.0);
		this.muted			= muted;

		// Groups
		this.groups			= new Dictionary<string, SoundGroup>();
		this.sounds			= new Dictionary<string, Sound>();
	}
	/** <summary> Constructs the default sound. </summary> */
	public SoundGroup(SoundGroup parent, string name, double volume = 1.0, double pitch = 0.0, double pan = 0.0, bool muted = false) {
		// Containment
		this.root			= parent.Root;
		this.parent			= parent;
		this.name			= name;

		// Settings
		this.volume			= GMath.Clamp(volume, 0.0, 1.0);
		this.pitch			= GMath.Clamp(pitch, -1.0, 1.0);
		this.pan			= GMath.Clamp(pan, -1.0, 1.0);
		this.muted			= muted;

		// Groups
		this.groups			= new Dictionary<string, SoundGroup>();
		this.sounds			= new Dictionary<string, Sound>();
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets the root sound group containing this sound group. </summary> */
	public SoundGroup Root {
		get { return (root ?? this); }
	}
	/** <summary> Gets the parent sound group containing this sound group. </summary> */
	public SoundGroup Parent {
		get { return parent; }
	}
	/** <summary> Gets the name of the sound group. </summary> */
	public string Name {
		get { return name; }
	}
	/** <summary> Gets the list of sounds contained in this the group. </summary> */
	public Sound[] Sounds {
		get {
			Sound[] soundList = new Sound[sounds.Count];
			sounds.Values.CopyTo(soundList, 0);
			return soundList;
		}
	}
	/** <summary> Gets the list of groups contained in this group. </summary> */
	public SoundGroup[] Groups {
		get {
			SoundGroup[] soundGroupList = new SoundGroup[groups.Count];
			groups.Values.CopyTo(soundGroupList, 0);
			return soundGroupList;
		}
	}

	#endregion
	//--------------------------------
	#region Settings

	/** <summary> Gets or sets the default volume of the sound between 0 and 1. </summary> */
	public double Volume {
		get { return volume; }
		set { volume = GMath.Clamp(value, 0.0, 1.0); UpdateSounds(); }
	}
	/** <summary> Gets or sets the default pitch of the sound between -1 and 1. </summary> */
	public double Pitch {
		get { return pitch; }
		set { pitch = GMath.Clamp(value, -1.0, 1.0); UpdateSounds(); }
	}
	/** <summary> Gets or sets the default pan of the sound between -1 and 1. </summary> */
	public double Pan {
		get { return pan; }
		set { pan = GMath.Clamp(value, -1.0, 1.0); UpdateSounds(); }
	}
	/** <summary> Gets or sets if the default sound will be muted. </summary> */
	public bool IsMuted {
		get { return muted; }
		set { muted = value; UpdateSounds(); }
	}

	#endregion
	//--------------------------------
	#region Playback
	
	/** <summary> Gets the default volume of the sound between 0 and 1. </summary> */
	public double GroupVolume {
		get { return GMath.Clamp((parent != null ? parent.GroupVolume : AudioSystem.MasterVolume) * volume, 0.0, 1.0); }
	}
	/** <summary> Gets the default pitch of the sound between -1 and 1. </summary> */
	public double GroupPitch {
		get { return GMath.Clamp((parent != null ? parent.GroupPitch : AudioSystem.MasterPitch) + pitch, -1.0, 1.0); }
	}
	/** <summary> Gets the default pan of the sound between -1 and 1. </summary> */
	public double GroupPan {
		get { return GMath.Clamp((parent != null ? parent.GroupPan : AudioSystem.MasterPan) + pan, -1.0, 1.0); }
	}
	/** <summary> Returns true if the default sound will be muted. </summary> */
	public bool GroupMuted {
		get { return (parent != null ? parent.GroupMuted : AudioSystem.IsMasterMuted) || muted; }
	}

	#endregion
	//--------------------------------
	#endregion
	//========== MANAGEMENT ==========
	#region Management
	//--------------------------------
	#region Groups

	/** <summary> Gets the specified subgroup. </summary> */
	public SoundGroup GetGroup(string group) {
		if (group.Length != 0) {
			int index = group.IndexOf('/');
			if (index == -1) {
				if (groups.ContainsKey(group))
					return groups[group];
			}
			else if (groups.ContainsKey(group.Substring(0, index))) {
				return groups[group.Substring(0, index)].GetGroup(group.Substring(index + 1));
			}
		}
		return this;
	}
	/** <summary> Returns true if the specified group exists. </summary> */
	public bool GroupExists(string group) {
		if (group.Length != 0) {
			int index = group.IndexOf('/');
			if (index == -1) {
				return groups.ContainsKey(group);
			}
			else if (groups.ContainsKey(group.Substring(0, index))) {
				return groups[group.Substring(0, index)].GroupExists(group.Substring(index + 1));
			}
			return false;
		}
		return true;
	}
	/** <summary> Adds a subgroup to the sound group. </summary> */
	public void AddGroup(string group, double volume = 1.0, double pitch = 0.0, double pan = 0.0, bool muted = false) {
		if (group.Length != 0) {
			int index = group.IndexOf('/');
			if (index == -1) {
				groups.Add(group, new SoundGroup(this, group, volume, pitch, pan, muted));
			}
			else if (groups.ContainsKey(group.Substring(0, index))) {
				groups[group.Substring(0, index)].AddGroup(group.Substring(index + 1));
			}
		}
	}
	/** <summary> Adds a subgroup to the sound group. </summary> */
	public void AddGroup(string groupName, SoundGroup group) {
		if (groupName.Length != 0) {
			int index = groupName.IndexOf('/');
			if (index == -1) {
				group.name = groupName;
				group.parent = this;
				groups.Add(groupName, group);
			}
			else if (groups.ContainsKey(groupName.Substring(0, index))) {
				groups[groupName.Substring(0, index)].AddGroup(groupName.Substring(index + 1), group);
			}
		}
	}

	#endregion
	//--------------------------------
	#region Sounds

	/** <summary> Gets the sound from the specified subgroup. </summary> */
	public Sound GetSound(string sound) {
		if (sound.Length != 0) {
			int index = sound.IndexOf('/');
			if (index == -1) {
				if (sounds.ContainsKey(sound))
					return sounds[sound];
			}
			else if (groups.ContainsKey(sound.Substring(0, index))) {
				return groups[sound.Substring(0, index)].GetSound(sound.Substring(index + 1));
			}
		}
		return null;
	}
	/** <summary> Returns true if the specified sound exists. </summary> */
	public bool SoundExists(string sound) {
		if (sound.Length != 0) {
			int index = sound.IndexOf('/');
			if (index == -1) {
				return sounds.ContainsKey(sound);
			}
			else if (groups.ContainsKey(sound.Substring(0, index))) {
				return groups[sound.Substring(0, index)].SoundExists(sound.Substring(index + 1));
			}
		}
		return false;
	}
	/** <summary> Adds a sound to the sound group. </summary> */
	public void AddSound(string sound, SoundEffect soundEffect, string fileName, double volume = 1.0, double pitch = 0.0, double pan = 0.0, bool muted = false) {
		if (sound.Length != 0) {
			int index = sound.IndexOf('/');
			if (index == -1) {
				sounds.Add(sound, new Sound(soundEffect, fileName, this, sound, volume, pitch, pan, muted));
			}
			else if (groups.ContainsKey(sound.Substring(0, index))) {
				groups[sound.Substring(0, index)].AddSound(sound.Substring(index + 1), soundEffect, fileName, volume, pitch, pan, muted);
			}
		}
	}
	/** <summary> Adds a sound to the sound group. </summary> */
	public void AddSound(Sound sound) {
		if (sound != null) {
			sound.group = this;
			sounds.Add(sound.Name, sound);
		}
	}

	#endregion
	//--------------------------------
	#region Playback

	/** <summary> Updates every sound in the group. </summary> */
	internal void Update() {
		foreach (SoundGroup group in groups.Values) {
			group.Update();
		}
		foreach (Sound sound in sounds.Values) {
			sound.Update();
		}
	}
	/** <summary> Updates every sound instance in the group. </summary> */
	internal void UpdateSounds() {
		foreach (SoundGroup group in groups.Values) {
			group.UpdateSounds();
		}
		foreach (Sound sound in sounds.Values) {
			sound.UpdateSounds();
		}
	}
	/** <summary> Stops every sound in the group. </summary> */
	public void Stop() {
		foreach (SoundGroup group in groups.Values) {
			group.Stop();
		}
		foreach (Sound sound in sounds.Values) {
			sound.Stop();
		}
	}
	/** <summary> Pauses every sound in the group. </summary> */
	public void Pause() {
		foreach (SoundGroup group in groups.Values) {
			group.Pause();
		}
		foreach (Sound sound in sounds.Values) {
			sound.Pause();
		}
	}
	/** <summary> Resumes every sound in the group. </summary> */
	public void Resume() {
		foreach (SoundGroup group in groups.Values) {
			group.Resume();
		}
		foreach (Sound sound in sounds.Values) {
			sound.Resume();
		}
	}

	#endregion
	//--------------------------------
	#endregion
}
} // End namespace
