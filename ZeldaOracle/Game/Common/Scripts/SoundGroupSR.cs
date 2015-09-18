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
public class SoundGroupSR : ScriptReader {

	//=========== MEMBERS ============
	#region Members

	/** <summary> The current sound group being created. </summary> */
	private SoundGroup group;

	#endregion
	//========== PROPERTIES ==========
	#region Properties


	#endregion
	//=========== OVERRIDE ===========
	#region Override

	/** <summary> Begins reading the script. </summary> */
	protected override void BeginReading() {
		group	= null;
	}
	/** <summary> Ends reading the script. </summary> */
	protected override void EndReading() {
		group	= null;
	}
	/** <summary> Reads a line in the script as a command. </summary> */
	protected override bool ReadCommand(string command, List<string> args) {
		// Create a new sound group to then define sound for.
		// @group [groupName]
		// root can be used as the group name when specifying the main group
		if (command == "group") {
			if (args[0] == "root") {
				group = Resources.RootSoundGroup;
			}
			else {
				group = new SoundGroup(args[0].Substring(args[0].LastIndexOf('/') + 1));
				Resources.RootSoundGroup.AddGroup(args[0], group);
			}
		}
		else if (group != null) {

			// Finish creating the current sound group.
			// @end
			if (command == "end") {
				group = null;
			}
			
			// Set the volume of the sound group
			// @volume [volume 0.0 - 1.0]
			else if (command == "volume") {
				group.Volume = Double.Parse(args[0]);
			}

			// Set the pitch of the sound group
			// @pitch [pitch -1.0 - 1.0]
			else if (command == "pitch") {
				group.Pitch = Double.Parse(args[0]);
			}

			// Set the pan of the sound group
			// @pan [pan -1.0 - 1.0]
			else if (command == "pan") {
				group.Pan = Double.Parse(args[0]);
			}

			// Set if the sound group is muted
			// @muted [muted]
			else if (command == "muted") {
				group.IsMuted = Boolean.Parse(args[0]);
			}

			// Add a sound to the sound group
			// @sound [soundPath] [soundVolume] [soundPitch] [soundPan] [soundMuted]
			else if (command == "sound") {
				Sound sound = Resources.LoadSound(Resources.SoundDirectory + args[0]);
				sound.name = args[0].Substring(args[0].LastIndexOf('/') + 1);
				sound.Volume = Double.Parse(args[1]);
				sound.Pitch = Double.Parse(args[2]);
				sound.Pan = Double.Parse(args[3]);
				sound.IsMuted = Boolean.Parse(args[4]);

				group.AddSound(sound);
			}

			// Invalid command
			else {
				return false;
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
