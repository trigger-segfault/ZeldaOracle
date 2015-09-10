using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using XnaSong = Microsoft.Xna.Framework.Media.Song;
using XnaPlaylist = Microsoft.Xna.Framework.Media.Playlist;

using ZeldaOracle.Common.Geometry;
using Song = ZeldaOracle.Common.Audio.Song;
using Playlist = ZeldaOracle.Common.Audio.Playlist;

namespace ZeldaOracle.Common.Audio {
/** <summary>
 * A structure for storing a song.
 * </summary> */
public class Song {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members
	
	// Containment
	/** <summary> The base song class contained by this song. </summary> */
	private XnaSong song;
	/** <summary> The name of the song. </summary> */
	internal string name;
	/** <summary> The file path of the song. </summary> */
	private string filePath;

	// Details
	/** <summary> The title of the song. </summary> */
	private string title;
	/** <summary> The album of the song. </summary> */
	private string album;
	/** <summary> The artist of the song. </summary> */
	private string artist;

	// Settings
	/** <summary> The default volume of the song. </summary> */
	private double volume;
	/** <summary> The default pitch of the song. </summary> */
	private double pitch;
	/** <summary> The default pan of the song. </summary> */
	private double pan;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default song. </summary> */
	public Song(XnaSong song, string filePath, string name, string title = "", string album = "", string artist = "", double volume = 1.0, double pitch = 0.0, double pan = 0.0) {
		// Containment
		this.song			= song;
		this.name			= name;
		this.filePath		= filePath;

		// Details
		this.title			= title;
		this.album			= album;
		this.artist			= artist;

		// Settings
		this.volume			= volume;
		this.pitch			= pitch;
		this.pan			= pan;

		if (title.Length == 0)
			title = song.Name;
		if (album.Length == 0)
			album = song.Album.Name;
		if (artist.Length == 0)
			artist = song.Artist.Name;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets the base song contained by this song. </summary> */
	public XnaSong BaseSong {
		get { return song; }
	}
	/** <summary> Gets the name of the song. </summary> */
	public string Name {
		get { return name; }
	}
	/** <summary> Gets the file path of the song. </summary> */
	public string FilePath {
		get { return filePath; }
	}

	#endregion
	//--------------------------------
	#region Details

	/** <summary> Gets or sets the title of the song. </summary> */
	public string Title {
		get { return title; }
		set { title = value; }
	}
	/** <summary> Gets or sets the album of the song. </summary> */
	public string Album {
		get { return album; }
		set { album = value; }
	}
	/** <summary> Gets or sets the artist of the song. </summary> */
	public string Artist {
		get { return artist; }
		set { artist = value; }
	}

	#endregion
	//--------------------------------
	#region Settings

	/** <summary> Gets or sets the default volume of the sound between 0 and 1. </summary> */
	public double Volume {
		get { return volume; }
		set {
			volume = GMath.Clamp(value, 0.0, 1.0);
			AudioSystem.UpdateMusic();
		}
	}
	/** <summary> Gets or sets the default pitch of the sound between -1 and 1. </summary> */
	public double Pitch {
		get { return pitch; }
		set {
			pitch = GMath.Clamp(value, -1.0, 1.0);
			AudioSystem.UpdateMusic();
		}
	}
	/** <summary> Gets or sets the default pan of the sound between -1 and 1. </summary> */
	public double Pan {
		get { return pan; }
		set {
			pan = GMath.Clamp(value, -1.0, 1.0);
			AudioSystem.UpdateMusic();
		}
	}

	#endregion
	//--------------------------------
	#region Playback

	/** <summary> Returns true if the song is currently playing. </summary> */
	public bool IsPlaying {
		get {
			if (MediaPlayer.State == MediaState.Playing)
				return (song == MediaPlayer.Queue[0]);
			return false;
		}
	}
	/** <summary> Returns true if the song is currently paused. </summary> */
	public bool IsPaused {
		get {
			if (MediaPlayer.State == MediaState.Paused)
				return (song == MediaPlayer.Queue[0]);
			return false;
		}
	}
	/** <summary> Returns true if the song is currently stopped. </summary> */
	public bool IsStopped {
		get {
			if (MediaPlayer.State != MediaState.Stopped)
				return (song != MediaPlayer.Queue[0]);
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

	/** <summary> Plays the song. </summary> */
	public void Play(bool looped = false) {
		AudioSystem.PlaySong(name, looped);
	}
	/** <summary> Plays the song. </summary> */
	public void Play(bool looped, double volume, double pitch = 0.0, double pan = 0.0) {
		AudioSystem.PlaySong(name, looped, volume, pitch, pan);
	}
	/** <summary> Stops the song. </summary> */
	public void Stop() {
		
	}
	/** <summary> Resumes the song. </summary> */
	public void Resume() {
		
	}
	/** <summary> Pauses the song. </summary> */
	public void Pause() {
		
	}

	#endregion
	//--------------------------------
	#endregion
}
} // End namespace
