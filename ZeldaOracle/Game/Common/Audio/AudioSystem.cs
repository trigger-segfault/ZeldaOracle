using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using XnaSong = Microsoft.Xna.Framework.Media.Song;
using XnaPlaylist = Microsoft.Xna.Framework.Media.Playlist;

using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Content;
using Song = ZeldaOracle.Common.Audio.Song;
using Playlist = ZeldaOracle.Common.Audio.Playlist;

namespace ZeldaOracle.Common.Audio {
/** <summary>
* A static class for game-related audio functions and management.
* </summary> */
public static class AudioSystem {
	
	//========== CONSTANTS ===========
	#region Constants

	/** <summary> The maximum number of sounds that can play simultaneously. </summary> */
	public const int MaxNumSounds	= 600;

	#endregion
	//========== VARIABLES ===========
	#region Variables

	// Containment
	/** <summary> The current song playing. </summary> */
	private static Song currentSong;
	/** <summary> The current playlist. </summary> */
	private static Playlist playlist;

	// Visualization
	/** <summary> The visualization data for the currently playing song. </summary> */
	private static VisualizationData visData	= null;
	/** <summary> The power of the current position in the song. </summary> */
	private static double visPower;

	// Playback
	/** <summary> The master volume of the game. </summary> */
	private static double masterVolume;
	/** <summary> The master pitch of the game. </summary> */
	private static double masterPitch;
	/** <summary> The master pan of the game. </summary> */
	private static double masterPan;
	/** <summary> True if the game is muted. </summary> */
	private static bool masterMuted;

	/** <summary> The volume of the game music. </summary> */
	private static double musicVolume;
	/** <summary> The pitch of the game music. </summary> */
	private static double musicPitch;
	/** <summary> The pan of the game music. </summary> */
	private static double musicPan;
	/** <summary> True if the game music is muted. </summary> */
	private static bool musicMuted;
	/** <summary> True if the music has started playing. </summary> */
	private static bool musicStarted;
	/** <summary> The playback state of the media player. </summary> */
	private static MediaState mediaPlayerState;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Initializes the audio manager. </summary> */
	public static void Initialize() {
		// Containment
		currentSong			= null;
		playlist			= null;

		// Visualization
		visData				= new VisualizationData();
		visPower			= 0.0;

		// Playback
		masterVolume		= 1.0;
		masterPitch			= 0.0;
		masterPan			= 0.0;
		masterMuted			= false;
		musicVolume			= 1.0;
		musicPitch			= 0.0;
		musicPan			= 0.0;
		musicMuted			= false;

		musicStarted		= false;
		mediaPlayerState	= MediaState.Stopped;

		// Setup
		MediaPlayer.IsVisualizationEnabled	= true;

		// Events
		MediaPlayer.MediaStateChanged	+= OnMediaStateChanged;// new EventHandler<EventArgs>((sender, e) => { mediaPlayerState = MediaPlayer.State; });
	}
	/** <summary> Uninitializes the audio manager. </summary> */
	public static void Uninitialize() {
		MediaPlayer.Stop();
		//sounds.Stop();

		// Events
		MediaPlayer.MediaStateChanged	-= OnMediaStateChanged;

		// Visualization
		visData			= null;

		// Containment
		currentSong		= null;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Visualization

	/** <summary> Gets the list of sample data from the currently playing song. </summary> */
	public static float[] Samples {
		get { return visData.Samples.ToArray(); }
	}
	/** <summary> Gets the list of frequency data from the currently playing song. </summary> */
	public static float[] Frequencies {
		get { return visData.Frequencies.ToArray(); }
	}
	/** <summary> Gets the power of the currently playing song. </summary> */
	public static double VisPower {
		get { return visPower; }
	}

	#endregion
	//--------------------------------
	#region Settings

	/** <summary> Gets or sets the master volume of the game between 0 and 1. </summary> */
	public static double MasterVolume {
		get { return masterVolume; }
		set {
			masterVolume = value;
			UpdateSounds();
			UpdateMusic();
		}
	}
	/** <summary> Gets or sets the master pitch of the game between -1 and 1. </summary> */
	public static double MasterPitch {
		get { return masterPitch; }
		set {
			masterPitch = value;
			UpdateSounds();
			UpdateMusic();
		}
	}
	/** <summary> Gets or sets the master pan of the game between -1 and 1. </summary> */
	public static double MasterPan {
		get { return masterPan; }
		set {
			masterPan = value;
			UpdateSounds();
			UpdateMusic();
		}
	}
	/** <summary> Gets or sets if the game is muted. </summary> */
	public static bool IsMasterMuted {
		get { return masterMuted; }
		set {
			masterMuted = value;
			UpdateSounds();
			UpdateMusic();
		}
	}

	/** <summary> Gets or sets the default volume of the sound between 0 and 1. </summary> */
	public static double SoundVolume {
		get { return Resources.RootSoundGroup.Volume; }
		set { Resources.RootSoundGroup.Volume = value; }
	}
	/** <summary> Gets or sets the default pitch of the sound between -1 and 1. </summary> */
	public static double SoundPitch {
		get { return Resources.RootSoundGroup.Pitch; }
		set { Resources.RootSoundGroup.Pitch = value; }
	}
	/** <summary> Gets or sets the default pan of the sound between -1 and 1. </summary> */
	public static double SoundPan {
		get { return Resources.RootSoundGroup.Pan; }
		set { Resources.RootSoundGroup.Pan = value; }
	}
	/** <summary> Gets or sets if the default sound will be muted. </summary> */
	public static bool SoundIsMuted {
		get { return Resources.RootSoundGroup.IsMuted; }
		set { Resources.RootSoundGroup.IsMuted = value; }
	}

	/** <summary> Gets or sets the default volume of the sound between 0 and 1. </summary> */
	public static double MusicVolume {
		get { return musicVolume; }
		set {
			musicVolume = value;
			UpdateMusic();
		}
	}
	/** <summary> Gets or sets the default pitch of the sound between -1 and 1. </summary> */
	public static double MusicPitch {
		get { return musicPitch; }
		set {
			musicPitch = value;
			UpdateMusic();
		}
	}
	/** <summary> Gets or sets the default pan of the sound between -1 and 1. </summary> */
	public static double MusicPan {
		get { return musicPan; }
		set {
			musicPan = value;
			UpdateMusic();
		}
	}
	/** <summary> Gets or sets if the default sound will be muted. </summary> */
	public static bool MusicIsMuted {
		get { return musicMuted; }
		set {
			musicMuted = value;
			UpdateMusic();
		}
	}

	#endregion
	//--------------------------------
	#region Playback

	/** <summary> Gets the current song playing. </summary> */
	public static Song CurrentSong {
		get { return currentSong; }
	}
	/** <summary> Gets the current playlist playing. </summary> */
	public static Playlist Playlist {
		get { return playlist; }
	}
	/** <summary> Returns true if the music in the game is playing. </summary> */
	public static bool IsMusicPlaying {
		get { return mediaPlayerState == MediaState.Playing; }
	}
	/** <summary> Returns true if the music in the game is paused. </summary> */
	public static bool IsMusicPaused {
		get { return mediaPlayerState == MediaState.Paused; }
	}
	/** <summary> Returns true if the music in the game is stopped. </summary> */
	public static bool IsMusicStopped {
		get { return mediaPlayerState == MediaState.Stopped; }
	}

	#endregion
	//--------------------------------
	#endregion
	//============ EVENTS ============
	#region Events

	/** <summary> Called when the media state has been changed. </summary> */
	public static void OnMediaStateChanged(object sender, EventArgs e) {
		mediaPlayerState = MediaPlayer.State;
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating
	/** <summary> Called every step to update the audio manager. </summary> */
	public static void Update(GameTime gameTime) {

		// Update the current song and playlist
		if (IsMusicStopped && playlist != null && !musicStarted) {
			if (playlist.NumSongs > 0) {
				if (playlist.CurrentIndex + 1 == playlist.NumSongs && playlist.Loop) {
					if (playlist.Shuffle)
						playlist.ShuffleSongs();
					playlist.CurrentIndex = 0;
					currentSong = playlist[0];
					MediaPlayer.Play(currentSong.BaseSong);
					UpdateMusic();
				}
				else if (playlist.CurrentIndex + 1 < playlist.NumSongs && playlist.Autoplay) {
					playlist.CurrentIndex++;
					currentSong = playlist[playlist.CurrentIndex];
					MediaPlayer.Play(currentSong.BaseSong);
					UpdateMusic();
				}
			}
		}
		else if (musicStarted) {
			if (!IsMusicStopped)
				musicStarted = false;
		}
		

		// Load the visualization data
		//MediaPlayer.GetVisualizationData(visData);
		Resources.RootSoundGroup.Update();

		visPower = 0;

		/*float[] samples = Samples;

		for (int i = 0; i < samples.Length; i++) {
			visPower += GMath.Abs(samples[i]);
		}
		visPower /= (double)samples.Length;*/
	}


	#endregion
	//=========== PLAYBACK ===========
	#region Playback

	/** <summary> Plays the song with the specified name. </summary> */
	public static void PlaySong(string name, bool looped = false) {
		if (currentSong == Resources.GetSong(name) && mediaPlayerState == MediaState.Playing)
			return;
		if (mediaPlayerState != MediaState.Stopped)
			MediaPlayer.Stop();

		playlist = new Playlist("", false, true, looped);
		playlist.AddSong(name);
		playlist.CurrentIndex = 0;
		currentSong = Resources.GetSong(name);
		MediaPlayer.Play(currentSong.BaseSong);
		UpdateMusic();
		musicStarted = true;
	}
	/** <summary> Plays the song with the specified name. </summary> */
	public static void PlaySong(string name, bool looped, double volume, double pitch = 0.0, double pan = 0.0) {
		if (currentSong == Resources.GetSong(name) && mediaPlayerState == MediaState.Playing)
			return;
		if (mediaPlayerState != MediaState.Stopped)
			MediaPlayer.Stop();

		playlist = new Playlist("", false, true, looped, volume, pitch, pan);
		playlist.AddSong(name);
		playlist.CurrentIndex = 0;
		currentSong = Resources.GetSong(name);
		MediaPlayer.Play(currentSong.BaseSong);
		UpdateMusic();
		musicStarted = true;
	}
	/** <summary> Starts playling the specified playlist. </summary> */
	public static void StartPlaylist(Playlist playlist) {
		if (mediaPlayerState != MediaState.Stopped)
			MediaPlayer.Stop();

		AudioSystem.playlist	= playlist;
		if (playlist.NumSongs > 0) {
			if (playlist.Shuffle)
				playlist.ShuffleSongs();
			playlist.CurrentIndex = 0;
			currentSong = playlist[0];
			MediaPlayer.Play(currentSong.BaseSong);
			UpdateMusic();
		}
		else {
			currentSong = null;
		}
		musicStarted = true;
	}
	/** <summary> Starts the next song in the playlist. </summary> */
	public static void NextSong() {
		if (mediaPlayerState != MediaState.Stopped)
			MediaPlayer.Stop();

		if (playlist.NumSongs > 0) {
			playlist.CurrentIndex = (playlist.CurrentIndex + 1) % playlist.NumSongs;
			currentSong = playlist[playlist.CurrentIndex];
			MediaPlayer.Play(currentSong.BaseSong);
			UpdateMusic();
		}
		else {
			currentSong = null;
		}
		musicStarted = true;
	}
	/** <summary> Plays the sound effect with the specified name. </summary> */
	public static SoundInstance PlaySound(string name, bool looped = false) {
		return Resources.RootSoundGroup.GetSound(name).Play(looped);
	}
	/** <summary> Plays the sound effect with the specified name. </summary> */
	public static SoundInstance PlaySound(string name, bool looped, double volume, double pitch = 0.0, double pan = 0.0, bool muted = false) {
		return Resources.RootSoundGroup.GetSound(name).Play(looped, volume, pitch, pan, muted);
	}
	/** <summary> Stops the sound effect with the specified name. </summary> */
	public static void StopSound(string name) {
		Resources.RootSoundGroup.GetSound(name).Stop();
	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management
	//--------------------------------
	#region Sound Groups


	#endregion
	//--------------------------------
	#region Sounds

	/** <summary> Gets the sound effect with the specified name. </summary> */
	public static Sound GetSound(string name) {
		return Resources.RootSoundGroup.GetSound(name);
	}

	#endregion
	//--------------------------------
	#region Songs


	#endregion
	//--------------------------------
	#region Sound Playback

	/** <summary> Updates the sound instances. </summary> */
	internal static void UpdateSounds() {
		Resources.RootSoundGroup.UpdateSounds();
	}
	/** <summary> Stops every sound in the group. </summary> */
	public static void StopAllSounds() {
		Resources.RootSoundGroup.Stop();
	}
	/** <summary> Pauses every sound in the group. </summary> */
	public static void PauseAllSounds() {
		Resources.RootSoundGroup.Pause();
	}
	/** <summary> Resumes every sound in the group. </summary> */
	public static void ResumeAllSounds() {
		Resources.RootSoundGroup.Resume();
	}

	#endregion
	//--------------------------------
	#region Music Playback

	/** <summary> Updates the music. </summary> */
	internal static void UpdateMusic() {
		if (!IsMusicStopped && currentSong != null && playlist != null)
			MediaPlayer.Volume = (float)(masterVolume * musicVolume * playlist.Volume * currentSong.Volume);
		//else if (!IsMusicStopped && currentSong != null)
		//	MediaPlayer.Volume = (float)(masterVolume * musicVolume * currentSong.Volume);
	}
	/** <summary> Pauses the current playing song. </summary> */
	public static void PauseMusic() {
		if (mediaPlayerState == MediaState.Playing)
			MediaPlayer.Pause();
	}
	/** <summary> Resumes the current playing song. </summary> */
	public static void ResumeMusic() {
		if (mediaPlayerState == MediaState.Paused)
			MediaPlayer.Resume();
	}
	/** <summary> Stops the current song. </summary> */
	public static void StopMusic() {
		if (mediaPlayerState != MediaState.Stopped)
			MediaPlayer.Stop();
		musicStarted = false;
	}

	#endregion
	//--------------------------------
	#endregion
}
} // End namespace
