using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaPlaylist = Microsoft.Xna.Framework.Media.Playlist;

using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Scripts {
/** <summary>
 * Script reader for sprite sheets. The script can contain
 * information for multiple sprite sheets with corresponding
 * images in the content folder.
 *
 * FORMAT:
 *
 * @spritesheet [name]
 * @size [width] [height]
 * @sprite [frame_x] [frame_y] [frame_width] [frame_height] [offset_x] [offset_y] [source_width] [source_height]
 * @end
 *
 * Note: All declared sprites must be within the
 * @spritesheet and @end commands.
 * </summary> */
public class PlaylistSR : ScriptReader {

	//=========== MEMBERS ============
	#region Members

	/** <summary> True if music is being created instead of a playlist. </summary> */
	private bool creatingMusic;
	/** <summary> The current playlist being created. </summary> */
	private Playlist playlist;

	#endregion
	//========== PROPERTIES ==========
	#region Properties


	#endregion
	//=========== OVERRIDE ===========
	#region Override

	/** <summary> Begins reading the script. </summary> */
	protected override void BeginReading() {
		creatingMusic	= false;
		playlist		= null;
	}
	/** <summary> Ends reading the script. </summary> */
	protected override void EndReading() {
		creatingMusic	= false;
		playlist		= null;
	}
	/** <summary> Reads a line in the script as a command. </summary> */
	protected override bool ReadCommand(string command, List<string> args) {
		// Create a new sound group to then define sound for.
		// @group [groupName]
		// root can be used as the group name when specifying the main group
		if (command == "music") {
			playlist = null;
			creatingMusic = true;
		}
		else if (command == "playlist") {
			creatingMusic = false;
			playlist = new Playlist(args[0]);
			Resources.AddPlaylist(playlist);
		}
		else if (playlist != null) {

			// Finish creating the current playlist.
			// @end
			if (command == "end") {
				playlist = null;
				creatingMusic = false;
			}

			// Set the volume of the playlist
			// @volume [volume 0.0 - 1.0]
			else if (command == "volume") {
				playlist.Volume = Double.Parse(args[0]);
			}

			// Set the pitch of the playlist
			// @pitch [pitch -1.0 - 1.0]
			else if (command == "pitch") {
				playlist.Pitch = Double.Parse(args[0]);
			}

			// Set the pan of the playlist
			// @pan [pan -1.0 - 1.0]
			else if (command == "pan") {
				playlist.Pan = Double.Parse(args[0]);
			}

			// Set if the playlist is looped
			// @loop [looped]
			else if (command == "loop") {
				playlist.Loop = Boolean.Parse(args[0]);
			}

			// Add a song to the playlist
			// @song [songName]
			else if (command == "song") {
				playlist.AddSong(args[0]);
			}

			// Invalid command
			else {
				return false;
			}
		}
		else if (creatingMusic) {

			// Finish creating the music.
			// @end
			if (command == "end") {
				playlist = null;
				creatingMusic = false;
			}

			// Set the volume of the music
			// @volume [volume 0.0 - 1.0]
			else if (command == "volume") {
				AudioSystem.MusicVolume = Double.Parse(args[0]);
			}

			// Set the pitch of the music
			// @pitch [pitch -1.0 - 1.0]
			else if (command == "pitch") {
				AudioSystem.MusicPitch = Double.Parse(args[0]);
			}

			// Set the pan of the music
			// @pan [pan -1.0 - 1.0]
			else if (command == "pan") {
				AudioSystem.MusicPan = Double.Parse(args[0]);
			}

			// Set if the music is muted
			// @muted [muted]
			else if (command == "muted") {
				AudioSystem.MusicIsMuted = Boolean.Parse(args[0]);
			}

			// Load a song
			// @song [songName] [songTitle] [songAlbum] [songArtist] [songVolume] [songPitch] [songPan]
			else if (command == "song") {
				Song song = Resources.LoadSong(Resources.MusicDirectory + args[0]);
				song.name = args[0].Substring(args[0].LastIndexOf('/') + 1);
				song.Title = args[1];
				song.Album = args[2];
				song.Artist = args[3];
				song.Volume = Double.Parse(args[4]);
				song.Pitch = Double.Parse(args[5]);
				song.Pan = Double.Parse(args[6]);
			}
		}

		// Invalid command
		else {
			return false;
		}

		return true;
	}

	#endregion
}
} // end namespace
