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
	/** <summary>
	* A static class for game-related audio functions and management.
	* </summary> */
	public static class AudioSystem {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		// The maximum number of sounds that can play simultaneously.
		public const int MaxNumSounds	= 600;


		//-----------------------------------------------------------------------------
		// Variables
		//-----------------------------------------------------------------------------

		// Containment
		// The current song playing.
		private static Song currentSong;

		// Playback
		// The master volume of the game.
		private static float masterVolume;
		// The master pitch of the game.
		private static float masterPitch;
		// The master pan of the game.
		private static float masterPan;
		// True if the game is muted.
		private static bool masterMuted;

		// The volume of the game sounds.
		private static float soundVolume;
		// The pitch of the game sounds.
		private static float soundPitch;
		// The pan of the game sounds.
		private static float soundPan;
		// True if the game sounds are muted.
		private static bool soundMuted;

		// The volume of the game music.
		private static float musicVolume;
		// The pitch of the game music.
		private static float musicPitch;
		// The pan of the game music.
		private static float musicPan;
		// True if the game music is muted.
		private static bool musicMuted;
		

		//-----------------------------------------------------------------------------
		// Initialization / Uninitialization
		//-----------------------------------------------------------------------------

		// Initializes the audio manager.
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

		// Uninitializes the audio manager.
		public static void Uninitialize() {
			//sounds.Stop();

			// Containment
			currentSong		= null;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Settings

		// Gets or sets the master volume of the game between 0 and 1.
		public static float MasterVolume {
			get { return masterVolume; }
			set {
				masterVolume = GMath.Clamp(value, 0.0f, 1.0f);
				SoundEffect.MasterVolume = masterVolume;
				UpdateSounds();
				UpdateMusic();
			}
		}

		// Gets or sets the master pitch of the game between -1 and 1.
		public static float MasterPitch {
			get { return masterPitch; }
			set {
				masterPitch = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateSounds();
				UpdateMusic();
			}
		}

		// Gets or sets the master pan of the game between -1 and 1.
		public static float MasterPan {
			get { return masterPan; }
			set {
				masterPan = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateSounds();
				UpdateMusic();
			}
		}

		// Gets or sets if the game is muted.
		public static bool IsMasterMuted {
			get { return masterMuted; }
			set {
				masterMuted = value;
				UpdateSounds();
				UpdateMusic();
			}
		}

		// Gets or sets the default volume of the sound between 0 and 1.
		public static float SoundVolume {
			get { return soundVolume; }
			set {
				soundVolume = GMath.Clamp(value, 0.0f, 1.0f);
				UpdateSounds();
				UpdateMusic();
			}
		}

		// Gets or sets the default pitch of the sound between -1 and 1.
		public static float SoundPitch {
			get { return soundPitch; }
			set {
				soundPitch = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateSounds();
				UpdateMusic();
			}
		}

		// Gets or sets the default pan of the sound between -1 and 1.
		public static float SoundPan {
			get { return soundPan; }
			set {
				soundPan = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateSounds();
				UpdateMusic();
			}
		}

		// Gets or sets if the default sound will be muted.
		public static bool IsSoundMuted {
			get { return soundMuted; }
			set {
				soundMuted = value;
				UpdateMusic();
			}
		}

		// Gets or sets the default volume of the sound between 0 and 1.
		public static float MusicVolume {
			get { return musicVolume; }
			set {
				musicVolume = GMath.Clamp(value, 0.0f, 1.0f);
				UpdateMusic();
			}
		}

		// Gets or sets the default pitch of the sound between -1 and 1.
		public static float MusicPitch {
			get { return musicPitch; }
			set {
				musicPitch = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateMusic();
			}
		}

		// Gets or sets the default pan of the sound between -1 and 1.
		public static float MusicPan {
			get { return musicPan; }
			set {
				musicPan = GMath.Clamp(value, -1.0f, 1.0f);
				UpdateMusic();
			}
		}

		// Gets or sets if the default sound will be muted.
		public static bool IsMusicMuted {
			get { return musicMuted; }
			set {
				musicMuted = value;
				UpdateMusic();
			}
		}
		
		// Playback

		// Gets the current song playing.
		public static Song CurrentSong {
			get { return currentSong; }
		}

		// Returns true if the music in the game is playing.
		public static bool IsMusicPlaying {
			get {
				if (currentSong != null)
					return currentSong.IsPlaying;
				return false;
			}
		}

		// Returns true if the music in the game is paused.
		public static bool IsMusicPaused {
			get {
				if (currentSong != null)
					return currentSong.IsPaused;
				return false;
			}
		}

		// Returns true if the music in the game is stopped.
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

		// Called every step to update the audio manager.
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

		// Plays the specified song.
		public static void PlaySong(Song song, bool looped = true) {
			if (currentSong != null)
				currentSong.Stop();

			currentSong = song;
			song.Play(looped);
		}

		// Plays the specified song.
		public static void PlaySong(Song song, bool looped, float volume, float pitch = 0.0f, float pan = 0.0f) {
			if (currentSong != null)
				currentSong.Stop();

			currentSong = song;
			song.Play(looped, volume, pitch, pan);
		}

		// Plays the song with the specified name.
		public static void PlaySong(string name, bool looped = true) {
			PlaySong(Resources.GetSong(name), looped);
		}

		// Plays the song with the specified name.
		public static void PlaySong(string name, bool looped, float volume, float pitch = 0.0f, float pan = 0.0f) {
			PlaySong(Resources.GetSong(name), looped, volume, pitch, pan);
		}

		// Plays the specified sound effect.
		public static SoundInstance PlaySound(Sound sound, bool looped = false) {
			return sound.Play(looped);
		}

		// Plays the specified sound effect.
		public static SoundInstance PlaySound(Sound sound, bool looped, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f, bool muted = false) {
			return sound.Play(looped, volume, pitch, pan, muted);
		}

		// Plays the sound effect with the specified name.
		public static SoundInstance PlaySound(string name, bool looped = false) {
			return PlaySound(Resources.GetSound(name), looped);
		}

		// Plays the sound effect with the specified name.
		public static SoundInstance PlaySound(string name, bool looped, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f, bool muted = false) {
			return PlaySound(Resources.GetSound(name), looped, volume, pitch, pan, muted);
		}

		// Plays the specified sound effect.
		public static SoundInstance PlayRandomSound(params Sound[] soundList) {
			return PlaySound(soundList[GRandom.NextInt(soundList.Length)]);
		}

		// Plays the specified sound effect.
		public static SoundInstance PlayRandomSound(params string[] soundList) {
			return PlaySound(soundList[GRandom.NextInt(soundList.Length)]);
		}

		public static void LoopSoundWhileActive(Sound sound) {
			sound.LoopWhileActive();
		}

		// Stops the specified sound effect.
		public static void StopSound(Sound sound) {
			sound.Stop();
		}

		// Stops the sound effect with the specified name.
		public static void StopSound(string name) {
			StopSound(Resources.GetSound(name));
		}

		// Returns true if the song with the specified name is playing.
		public static bool IsSongPlaying(Song song) {
			return song.IsPlaying;
		}

		// Returns true if the song with the specified name is playing.
		public static bool IsSongPlaying(string name) {
			return IsSongPlaying(Resources.GetSong(name));
		}

		// Returns true if the sound with the specified name is playing.
		public static bool IsSoundPlaying(Sound sound) {
			return sound.IsPlaying;
		}

		// Returns true if the sound with the specified name is playing.
		public static bool IsSoundPlaying(string name) {
			return IsSoundPlaying(Resources.GetSound(name));
		}

		//-----------------------------------------------------------------------------
		// Sound Playback
		//-----------------------------------------------------------------------------

		// Updates the sounds.
		internal static void UpdateSounds() {
			foreach (KeyValuePair<string, Sound> entry in Resources.Sounds) {
				entry.Value.UpdateSoundInstances();
			}
		}

		// Stops every sound in the group.
		public static void StopAllSounds() {
			foreach (KeyValuePair<string, Sound> entry in Resources.Sounds) {
				entry.Value.Stop();
			}
		}

		// Pauses every sound in the group.
		public static void PauseAllSounds() {
			foreach (KeyValuePair<string, Sound> entry in Resources.Sounds) {
				entry.Value.Pause();
			}
		}

		// Resumes every sound in the group.
		public static void ResumeAllSounds() {
			foreach (KeyValuePair<string, Sound> entry in Resources.Sounds) {
				entry.Value.Resume();
			}
		}

		
		//-----------------------------------------------------------------------------
		// Music Playback
		//-----------------------------------------------------------------------------

		// Updates the music.
		internal static void UpdateMusic() {
			if (currentSong != null)
				currentSong.UpdateSoundInstance();
		}

		// Pauses the current playing song.
		public static void PauseMusic() {
			if (currentSong != null)
				currentSong.Pause();
		}

		// Resumes the current playing song.
		public static void ResumeMusic() {
			if (currentSong != null)
				currentSong.Pause();
		}

		// Stops the current song.
		public static void StopMusic() {
			if (currentSong != null)
				currentSong.Stop();
		}

	}
}
