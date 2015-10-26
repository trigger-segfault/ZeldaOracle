using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
public class MusicSR : ScriptReader {

	//=========== MEMBERS ============
	#region Members

	/** <summary> True if music is being created instead of a playlist. </summary> */
	private bool creatingMusic;

	#endregion
	//========== PROPERTIES ==========
	#region Properties


	#endregion
	//=========== OVERRIDE ===========
	#region Override

	/** <summary> Begins reading the script. </summary> */
	protected override void BeginReading() {
		creatingMusic	= false;
	}
	/** <summary> Ends reading the script. </summary> */
	protected override void EndReading() {
		creatingMusic	= false;
	}
	/** <summary> Reads a line in the script as a command. </summary> */
	protected override bool ReadCommand(string command, List<string> args) {
		// Create a new sound group to then define sound for.
		// @group [groupName]
		// root can be used as the group name when specifying the main group
		if (command == "music") {
			creatingMusic = true;
		}
		else if (creatingMusic) {

			// Finish creating the music.
			// @end
			if (command == "end") {
				creatingMusic = false;
			}

			// Load a song
			// @song [songName] [songVolume] [songPitch] [songPan]
			else if (command == "song") {
				Song song = Resources.LoadSong(Resources.MusicDirectory + args[0]);
				song.name = args[0].Substring(args[0].LastIndexOf('/') + 1);
				song.Volume = Single.Parse(args[1]);
				song.Pitch = Single.Parse(args[2]);
				song.Pan = Single.Parse(args[3]);
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
