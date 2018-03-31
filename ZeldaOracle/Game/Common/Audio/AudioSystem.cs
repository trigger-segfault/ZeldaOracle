using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Content;

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
		/// <summary>The currently playing music.</summary>
		private static Music currentMusic;

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
			currentMusic		= null;

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
			// Containment
			currentMusic		= null;
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		/// <summary>Called every step to update the audio manager.</summary>
		public static void Update(GameTime gameTime) {
			currentMusic?.UpdateSoundInstance();

			foreach (var pair in Resources.GetDictionary<Sound>()) {
				pair.Value.Update();
			}
		}


		//-----------------------------------------------------------------------------
		// Playback
		//-----------------------------------------------------------------------------

		// Music ----------------------------------------------------------------------

		/// <summary>Plays the specified music.</summary>
		public static void PlayMusic(Music music, bool looped = true) {
			currentMusic?.Stop();

			currentMusic = music;
			music.Play(looped);
		}

		/// <summary>Plays the specified music.</summary>
		public static void PlayMusic(Music music, bool looped, float volume, float pitch = 0.0f, float pan = 0.0f) {
			currentMusic?.Stop();

			currentMusic = music;
			music.Play(looped, volume, pitch, pan);
		}

		/// <summary>Plays the music with the specified name.</summary>
		public static void PlayMusic(string name, bool looped = true) {
			PlayMusic(Resources.Get<Music>(name), looped);
		}

		/// <summary>Plays the music with the specified name.</summary>
		public static void PlayMusic(string name, bool looped, float volume, float pitch = 0.0f, float pan = 0.0f) {
			PlayMusic(Resources.Get<Music>(name), looped, volume, pitch, pan);
		}

		// Sound ----------------------------------------------------------------------

		/// <summary>Plays the specified sound effect.</summary>
		public static SoundInstance PlaySound(Sound sound, bool looped = false) {
			return sound.Play(looped);
		}

		/// <summary>Forces the specified sound effect to play.</summary>
		public static SoundInstance ForcePlaySound(Sound sound, bool looped = false) {
			return sound.ForcePlay(looped);
		}

		/// <summary>Forces the specified sound effect to play.</summary>
		public static SoundInstance PlaySound(Sound sound, bool looped, float volume, float pitch = 0.0f, float pan = 0.0f, bool muted = false) {
			return sound.Play(looped, volume, pitch, pan, muted);
		}

		/// <summary>Plays the specified sound effect.</summary>
		public static SoundInstance ForcePlaySound(Sound sound, bool looped, float volume, float pitch = 0.0f, float pan = 0.0f, bool muted = false) {
			return sound.ForcePlay(looped, volume, pitch, pan, muted);
		}

		/// <summary>Plays the sound effect with the specified name.</summary>
		public static SoundInstance PlaySound(string name, bool looped = false) {
			return PlaySound(Resources.Get<Sound>(name), looped);
		}

		/// <summary>Forces the sound effect with the specified name to play.</summary>
		public static SoundInstance ForcePlaySound(string name, bool looped = false) {
			return ForcePlaySound(Resources.Get<Sound>(name), looped);
		}

		/// <summary>Plays the sound effect with the specified name.</summary>
		public static SoundInstance PlaySound(string name, bool looped, float volume, float pitch = 0.0f, float pan = 0.0f, bool muted = false) {
			return PlaySound(Resources.Get<Sound>(name), looped, volume, pitch, pan, muted);
		}

		/// <summary>Forces the sound effect with the specified name to play.</summary>
		public static SoundInstance ForcePlaySound(string name, bool looped, float volume, float pitch = 0.0f, float pan = 0.0f, bool muted = false) {
			return ForcePlaySound(Resources.Get<Sound>(name), looped, volume, pitch, pan, muted);
		}

		/// <summary>Stops the specified sound effect.</summary>
		public static void StopSound(Sound sound) {
			sound.Stop();
		}

		/// <summary>Stops the sound effect with the specified name.</summary>
		public static void StopSound(string name) {
			StopSound(Resources.Get<Sound>(name));
		}

		// Random Sound ---------------------------------------------------------------

		/// <summary>Plays the specified sound effect.</summary>
		public static SoundInstance PlayRandomSound(params Sound[] soundList) {
			return PlaySound(soundList[GRandom.NextInt(soundList.Length)]);
		}

		/// <summary>Forces the specified sound effect to play.</summary>
		public static SoundInstance ForcePlayRandomSound(params Sound[] soundList) {
			return ForcePlaySound(soundList[GRandom.NextInt(soundList.Length)]);
		}

		/// <summary>Plays the specified sound effect.</summary>
		public static SoundInstance PlayRandomSound(params string[] soundList) {
			return PlaySound(soundList[GRandom.NextInt(soundList.Length)]);
		}

		/// <summary>Forces the specified sound effect to play.</summary>
		public static SoundInstance ForcePlayRandomSound(params string[] soundList) {
			return ForcePlaySound(soundList[GRandom.NextInt(soundList.Length)]);
		}

		// Loop While Active Sound ----------------------------------------------------

		/// <summary>Play the sound looped if it is not already playing. The sound will
		/// automatically stop if this function is not called again before the next
		/// audio system update step.</summary>
		public static void LoopSoundWhileActive(Sound sound) {
			sound.LoopWhileActive();
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the music with the specified name is playing.</summary>
		public static bool IsMusicPlaying(Music music) {
			return music.IsPlaying;
		}

		/// <summary>Returns true if the music with the specified name is playing.</summary>
		public static bool IsMusicPlaying(string name) {
			return IsMusicPlaying(Resources.Get<Music>(name));
		}

		/// <summary>Returns true if the sound with the specified name is playing.</summary>
		public static bool IsSoundPlaying(Sound sound) {
			return sound.IsPlaying;
		}

		/// <summary>Returns true if the sound with the specified name is playing.</summary>
		public static bool IsSoundPlaying(string name) {
			return IsSoundPlaying(Resources.Get<Sound>(name));
		}


		//-----------------------------------------------------------------------------
		// All Sounds
		//-----------------------------------------------------------------------------

		/// <summary>Updates the sounds.</summary>
		internal static void UpdateSounds() {
			foreach (var pair in Resources.GetDictionary<Sound>()) {
				pair.Value.UpdateSoundInstances();
			}
		}

		/// <summary>Stops every sound in the game.</summary>
		public static void StopAllSounds() {
			foreach (var pair in Resources.GetDictionary<Sound>()) {
				pair.Value.Stop();
			}
		}

		/// <summary>Pauses every sound in the game.</summary>
		public static void PauseAllSounds() {
			foreach (var pair in Resources.GetDictionary<Sound>()) {
				pair.Value.Pause();
			}
		}

		/// <summary>Resumes every sound in the game.</summary>
		public static void ResumeAllSounds() {
			foreach (var pair in Resources.GetDictionary<Sound>()) {
				pair.Value.Resume();
			}
		}


		//-----------------------------------------------------------------------------
		// Current Music
		//-----------------------------------------------------------------------------

		/// <summary>Updates the music.</summary>
		internal static void UpdateMusic() {
			currentMusic?.UpdateSoundInstance();
		}

		/// <summary>Pauses the current playing music.</summary>
		public static void PauseMusic() {
			currentMusic?.Pause();
		}

		/// <summary>Resumes the current playing music.</summary>
		public static void ResumeMusic() {
			currentMusic?.Pause();
		}

		/// <summary>Stops the current music.</summary>
		public static void StopMusic() {
			currentMusic?.Stop();
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

		/// <summary>Gets the current music playing.</summary>
		public static Music CurrentMusic {
			get { return currentMusic; }
		}

		/// <summary>Returns true if the music in the game is playing.</summary>
		public static bool IsCurrentMusicPlaying {
			get { return currentMusic?.IsPlaying ?? false; }
		}

		/// <summary>Returns true if the music in the game is paused.</summary>
		public static bool IsCurrentMusicPaused {
			get { return currentMusic?.IsPaused ?? false; }
		}

		/// <summary>Returns true if the music in the game is stopped.</summary>
		public static bool IsCurrentMusicStopped {
			get { return currentMusic?.IsStopped ?? true; }
		}
	}
}
