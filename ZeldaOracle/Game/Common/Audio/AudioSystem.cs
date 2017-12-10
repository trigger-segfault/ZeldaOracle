using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using XnaSong = Microsoft.Xna.Framework.Media.Song;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Content;
using Song = ZeldaOracle.Common.Audio.Song;

namespace ZeldaOracle.Common.Audio {
	/// <summary>A static class for game-related audio functions and management.</summary>
	public static class AudioSystem {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The maximum number of sounds that can play simultaneously.</summary>
		public const int MaxSoundInstances = 200;


		//-----------------------------------------------------------------------------
		// Variables
		//-----------------------------------------------------------------------------

		// Containment
		/// <summary>The current song playing.</summary>
		private static Song currentSong;

		// Playback
		/// <summary>The master volume of the game.</summary>
		private static float masterVolume;
		/// <summary>The master pitch of the game.</summary>
		private static float masterPitch;
		/// <summary>The master pan of the game.</summary>
		private static float masterPan;
		/// <summary>True if the game is muted.</summary>
		private static bool masterMuted;

		/// <summary>The volume of the game sounds.</summary>
		private static float soundVolume;
		/// <summary>The pitch of the game sounds.</summary>
		private static float soundPitch;
		/// <summary>The pan of the game sounds.</summary>
		private static float soundPan;
		/// <summary>True if the game sounds are muted.</summary>
		private static bool soundMuted;

		/// <summary>The volume of the game music.</summary>
		private static float musicVolume;
		/// <summary>The pitch of the game music.</summary>
		private static float musicPitch;
		/// <summary>The pan of the game music.</summary>
		private static float musicPan;
		/// <summary>True if the game music is muted.</summary>
		private static bool musicMuted;


		//-----------------------------------------------------------------------------
		// Initialization / Uninitialization
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the audio manager.</summary>
		public static void Initialize() {
			// Containment
			currentSong			= null;

			// Playback
			masterVolume		= 1.0f;
			masterPitch			= 0.0f;
			masterPan			= 0.0f;
			masterMuted			= false;

			soundVolume			= 1.0f;
			soundPitch			= 0.0f;
			soundPan			= 0.0f;
			soundMuted			= false;

			musicVolume			= 1.0f;
			musicPitch			= 0.0f;
			musicPan			= 0.0f;
			musicMuted			= false;
		}

		/// <summary>Uninitializes the audio manager.</summary>
		public static void Uninitialize() {
			//sounds.Stop();

			// Containment
			currentSong		= null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Settings -------------------------------------------------------------------

		/// <summary>Gets or sets the master volume of the game between 0 and 1.</summary>
		public static float MasterVolume {
			get { return masterVolume; }
			set {
				masterVolume = GMath.Clamp(value, 0.0f, 1.0f);
				SoundEffect.MasterVolume = masterVolume;
				UpdateSounds();
				UpdateMusic();
			}
		}

		/// <summary>Gets or sets the master pitch of the game between -1 and 1.</summary>
		public static float MasterPitch {
			get { return masterPitch; }
			set {
				masterPitch = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateSounds();
				UpdateMusic();
			}
		}

		/// <summary>Gets or sets the master pan of the game between -1 and 1.</summary>
		public static float MasterPan {
			get { return masterPan; }
			set {
				masterPan = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateSounds();
				UpdateMusic();
			}
		}

		/// <summary>Gets or sets if the game is muted.
		public static bool IsMasterMuted {
			get { return masterMuted; }
			set {
				masterMuted = value;
				UpdateSounds();
				UpdateMusic();
			}
		}

		/// <summary>Gets or sets the default volume of the sound between 0 and 1.</summary>
		public static float SoundVolume {
			get { return soundVolume; }
			set {
				soundVolume = GMath.Clamp(value, 0.0f, 1.0f);
				UpdateSounds();
				UpdateMusic();
			}
		}

		/// <summary>Gets or sets the default pitch of the sound between -1 and 1.</summary>
		public static float SoundPitch {
			get { return soundPitch; }
			set {
				soundPitch = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateSounds();
				UpdateMusic();
			}
		}

		/// <summary>Gets or sets the default pan of the sound between -1 and 1.</summary>
		public static float SoundPan {
			get { return soundPan; }
			set {
				soundPan = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateSounds();
				UpdateMusic();
			}
		}

		/// <summary>Gets or sets if the default sound will be muted.</summary>
		public static bool IsSoundMuted {
			get { return soundMuted; }
			set {
				soundMuted = value;
				UpdateMusic();
			}
		}

		/// <summary>Gets or sets the default volume of the sound between 0 and 1.</summary>
		public static float MusicVolume {
			get { return musicVolume; }
			set {
				musicVolume = GMath.Clamp(value, 0.0f, 1.0f);
				UpdateMusic();
			}
		}

		/// <summary>Gets or sets the default pitch of the sound between -1 and 1.</summary>
		public static float MusicPitch {
			get { return musicPitch; }
			set {
				musicPitch = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateMusic();
			}
		}

		/// <summary>Gets or sets the default pan of the sound between -1 and 1.</summary>
		public static float MusicPan {
			get { return musicPan; }
			set {
				musicPan = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateMusic();
			}
		}

		/// <summary>Gets or sets if the default sound will be muted.</summary>
		public static bool IsMusicMuted {
			get { return musicMuted; }
			set {
				musicMuted = value;
				UpdateMusic();
			}
		}
		
		// Playback -------------------------------------------------------------------

		/// <summary>Gets the current song playing.</summary>
		public static Song CurrentSong {
			get { return currentSong; }
		}

		/// <summary>Returns true if the music in the game is playing.</summary>
		public static bool IsMusicPlaying {
			get {
				if (currentSong != null)
					return currentSong.IsPlaying;
				return false;
			}
		}

		/// <summary>Returns true if the music in the game is paused.</summary>
		public static bool IsMusicPaused {
			get {
				if (currentSong != null)
					return currentSong.IsPaused;
				return false;
			}
		}

		/// <summary>Returns true if the music in the game is stopped.</summary>
		public static bool IsMusicStopped {
			get {
				if (currentSong != null)
					return currentSong.IsStopped;
				return true;
			}
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		/// <summary>Called every step to update the audio manager.</summary>
		public static void Update(GameTime gameTime) {
			if (currentSong != null)
				currentSong.UpdateSoundInstance();

			foreach (KeyValuePair<string, Sound> entry in Resources.Sounds) {
				entry.Value.Update();
			}
		}


		//-----------------------------------------------------------------------------
		// Playback
		//-----------------------------------------------------------------------------

		/// <summary>Plays the specified song.</summary>
		public static void PlaySong(Song song, bool looped = true) {
			if (currentSong != null)
				currentSong.Stop();

			currentSong = song;
			song.Play(looped);
		}

		/// <summary>Plays the specified song.</summary>
		public static void PlaySong(Song song, bool looped, float volume, float pitch = 0.0f, float pan = 0.0f) {
			if (currentSong != null)
				currentSong.Stop();

			currentSong = song;
			song.Play(looped, volume, pitch, pan);
		}

		/// <summary>Plays the song with the specified name.</summary>
		public static void PlaySong(string name, bool looped = true) {
			PlaySong(Resources.GetSong(name), looped);
		}

		/// <summary>Plays the song with the specified name.</summary>
		public static void PlaySong(string name, bool looped, float volume, float pitch = 0.0f, float pan = 0.0f) {
			PlaySong(Resources.GetSong(name), looped, volume, pitch, pan);
		}

		/// <summary>Plays the specified sound effect.</summary>
		public static SoundInstance PlaySound(Sound sound, bool looped = false) {
			return sound.Play(looped);
		}

		/// <summary>Plays the specified sound effect.</summary>
		public static SoundInstance PlaySound(Sound sound, bool looped, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f, bool muted = false) {
			return sound.Play(looped, volume, pitch, pan, muted);
		}

		/// <summary>Plays the sound effect with the specified name.</summary>
		public static SoundInstance PlaySound(string name, bool looped = false) {
			return PlaySound(Resources.GetSound(name), looped);
		}

		/// <summary>Plays the sound effect with the specified name.</summary>
		public static SoundInstance PlaySound(string name, bool looped, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f, bool muted = false) {
			return PlaySound(Resources.GetSound(name), looped, volume, pitch, pan, muted);
		}

		/// <summary>Plays the specified sound effect.</summary>
		public static SoundInstance PlayRandomSound(params Sound[] soundList) {
			return PlaySound(soundList[GRandom.NextInt(soundList.Length)]);
		}

		/// <summary>Plays the specified sound effect.</summary>
		public static SoundInstance PlayRandomSound(params string[] soundList) {
			return PlaySound(soundList[GRandom.NextInt(soundList.Length)]);
		}

		/// <summary></summary>
		public static void LoopSoundWhileActive(Sound sound) {
			sound.LoopWhileActive();
		}

		/// <summary>Stops the specified sound effect.</summary>
		public static void StopSound(Sound sound) {
			sound.Stop();
		}

		/// <summary>Stops the sound effect with the specified name.</summary>
		public static void StopSound(string name) {
			StopSound(Resources.GetSound(name));
		}

		/// <summary>Returns true if the song with the specified name is playing.</summary>
		public static bool IsSongPlaying(Song song) {
			return song.IsPlaying;
		}

		/// <summary>Returns true if the song with the specified name is playing.</summary>
		public static bool IsSongPlaying(string name) {
			return IsSongPlaying(Resources.GetSong(name));
		}

		/// <summary>Returns true if the sound with the specified name is playing.</summary>
		public static bool IsSoundPlaying(Sound sound) {
			return sound.IsPlaying;
		}

		/// <summary>Returns true if the sound with the specified name is playing.</summary>
		public static bool IsSoundPlaying(string name) {
			return IsSoundPlaying(Resources.GetSound(name));
		}

		//-----------------------------------------------------------------------------
		// Sound Playback
		//-----------------------------------------------------------------------------

		/// <summary>Updates the sounds.</summary>
		internal static void UpdateSounds() {
			foreach (KeyValuePair<string, Sound> entry in Resources.Sounds) {
				entry.Value.UpdateSoundInstances();
			}
		}

		/// <summary>Stops every sound in the group.</summary>
		public static void StopAllSounds() {
			foreach (KeyValuePair<string, Sound> entry in Resources.Sounds) {
				entry.Value.Stop();
			}
		}

		/// <summary>Pauses every sound in the group.</summary>
		public static void PauseAllSounds() {
			foreach (KeyValuePair<string, Sound> entry in Resources.Sounds) {
				entry.Value.Pause();
			}
		}

		/// <summary>Resumes every sound in the group.</summary>
		public static void ResumeAllSounds() {
			foreach (KeyValuePair<string, Sound> entry in Resources.Sounds) {
				entry.Value.Resume();
			}
		}


		//-----------------------------------------------------------------------------
		// Music Playback
		//-----------------------------------------------------------------------------

		/// <summary>Updates the music.</summary>
		internal static void UpdateMusic() {
			if (currentSong != null)
				currentSong.UpdateSoundInstance();
		}

		/// <summary>Pauses the current playing song.</summary>
		public static void PauseMusic() {
			if (currentSong != null)
				currentSong.Pause();
		}

		/// <summary>Resumes the current playing song.</summary>
		public static void ResumeMusic() {
			if (currentSong != null)
				currentSong.Pause();
		}

		/// <summary>Stops the current song.</summary>
		public static void StopMusic() {
			if (currentSong != null)
				currentSong.Stop();
		}

	}
}
