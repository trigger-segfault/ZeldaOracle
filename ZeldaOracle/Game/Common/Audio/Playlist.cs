using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using XnaSong = Microsoft.Xna.Framework.Media.Song;
using XnaPlaylist = Microsoft.Xna.Framework.Media.Playlist;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using Song = ZeldaOracle.Common.Audio.Song;
using Playlist = ZeldaOracle.Common.Audio.Playlist;

namespace ZeldaOracle.Common.Audio {
/** <summary>
 * A structure for storing a playlist of songs.
 * </summary> */
public class Playlist {

	//========== CONSTANTS ===========
	#region Constants

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The list of songs to play. </summary> */
	private List<string> songs;
	/** <summary> The name of the playlist. </summary> */
	private string name;
	/** <summary> The index of the current song. </summary> */
	private int currentIndex;

	// Settings
	/** <summary> True if the playlist should shuffle. </summary> */
	private bool shuffle;
	/** <summary> True if the playlist should automatically play the next song in the list. </summary> */
	private bool autoplay;
	/** <summary> True if the playlist should restart after all of the songs have played. </summary> */
	private bool loop;
	/** <summary> The default volume of the songs. </summary> */
	private double volume;
	/** <summary> The default pitch of the songs. </summary> */
	private double pitch;
	/** <summary> The default pan of the songs. </summary> */
	private double pan;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default playlist. </summary> */
	public Playlist(string name = "") {
		// Containment
		this.songs			= new List<string>();
		this.name			= name;
		this.currentIndex	= -1;

		// Settings
		this.shuffle		= false;
		this.autoplay		= true;
		this.loop			= true;
		this.volume			= 1.0;
		this.pitch			= 0.0;
		this.pan			= 0.0;
	}
	/** <summary> Constructs the default playlist. </summary> */
	public Playlist(string name, bool shuffle, bool autoplay, bool loop) {
		// Containment
		this.songs			= new List<string>();
		this.name			= name;
		this.currentIndex	= -1;

		// Settings
		this.shuffle		= shuffle;
		this.autoplay		= autoplay;
		this.loop			= loop;
		this.volume			= 1.0;
		this.pitch			= 0.0;
		this.pan			= 0.0;
	}
	/** <summary> Constructs the default playlist. </summary> */
	public Playlist(string name, bool shuffle, bool autoplay, bool loop, double volume, double pitch = 0.0, double pan = 0.0) {
		// Containment
		this.songs			= new List<string>();
		this.name			= name;
		this.currentIndex	= -1;

		// Settings
		this.shuffle		= shuffle;
		this.autoplay		= autoplay;
		this.loop			= loop;
		this.volume			= GMath.Clamp(volume, 0.0, 1.0);
		this.pitch			= GMath.Clamp(pitch, -1.0, 1.0);
		this.pan			= GMath.Clamp(pan, -1.0, 1.0);
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets the list of song names in the playlist. </summary> */
	public List<string> Songs {
		get { return songs; }
	}
	/** <summary> Gets the current song in the playlist. </summary> */
	public Song CurrentSong {
		get { return (currentIndex != -1 ? Resources.GetSong(songs[currentIndex]) : null); }
	}
	/** <summary> Gets the current index of the song in the playlist. </summary> */
	public int CurrentIndex {
		get { return currentIndex; }
		set { currentIndex = value; }
	}
	/** <summary> Gets the song at the specified index in the playlist. </summary> */
	public Song this[int index] {
		get { return Resources.GetSong(songs[index]); }
	}
	/** <summary> Gets the number of songs in the playlist. </summary> */
	public int NumSongs {
		get { return songs.Count; }
	}
	/** <summary> Gets or sets the name of the playlist. </summary> */
	public string Name {
		get { return name; }
		internal set { name = value; }
	}

	#endregion
	//--------------------------------
	#region Settings

	/** <summary> Gets or sets if the playlist should shuffle songs. </summary> */
	public bool Shuffle {
		get { return shuffle; }
		set { shuffle = value; }
	}
	/** <summary> Gets or sets if the playlist should automatically play the next song. </summary> */
	public bool Autoplay {
		get { return autoplay; }
		set { autoplay = value; }
	}
	/** <summary> Gets or sets if the playlist should restart after all the songs have played. </summary> */
	public bool Loop {
		get { return loop; }
		set { loop = value; }
	}
	/** <summary> Gets or sets the default volume of the songs between 0 and 1. </summary> */
	public double Volume {
		get { return volume; }
		set {
			volume = GMath.Clamp(value, 0.0, 1.0);
			AudioSystem.UpdateMusic();
		}
	}
	/** <summary> Gets or sets the default pitch of the songs between -1 and 1. </summary> */
	public double Pitch {
		get { return pitch; }
		set {
			pitch = GMath.Clamp(value, -1.0, 1.0);
			AudioSystem.UpdateMusic();
		}
	}
	/** <summary> Gets or sets the default pan of the songs between -1 and 1. </summary> */
	public double Pan {
		get { return pan; }
		set {
			pan = GMath.Clamp(value, -1.0, 1.0);
			AudioSystem.UpdateMusic();
		}
	}

	#endregion
	//--------------------------------
	#endregion
	//========== MANAGEMENT ==========
	#region Management
	//--------------------------------
	#region Playback

	/** <summary> Adds the song to the playlist. </summary> */
	public void AddSong(string name) {
		songs.Add(name);
	}
	/** <summary> Gets the song at the specified index in the playlist. </summary> */
	public Song GetSong(int index) {
		return Resources.GetSong(songs[index]);
	}
	/** <summary> Randomizes the order of the songs in the list. </summary> */
	internal void ShuffleSongs() {
		List<string> shuffledSongs = new List<string>();
		while (songs.Count > 0) {
			int index = GRandom.NextInt(songs.Count);
			shuffledSongs.Add(songs[index]);
			songs.RemoveAt(index);
		}
		songs = shuffledSongs;
	}

	#endregion
	//--------------------------------
	#endregion
}
} // End namespace
