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
public class SoundsSR : ScriptReader {

	//=========== MEMBERS ============
	#region Members

	private bool creatingSounds;

	#endregion
	//========== PROPERTIES ==========
	#region Properties


	#endregion
	//=========== OVERRIDE ===========
	#region Override

	/** <summary> Begins reading the script. </summary> */
	protected override void BeginReading() {
		creatingSounds = false;
	}
	/** <summary> Ends reading the script. </summary> */
	protected override void EndReading() {
		creatingSounds = false;
	}
	/** <summary> Reads a line in the script as a command. </summary> */
	protected override bool ReadCommand(string command, List<string> args) {

		if (command == "sounds") {
			creatingSounds = true;
		}

		else if (command == "end") {
			creatingSounds = false;
		}

		else if (creatingSounds) {
			// Add a sound to the sound group
			// @sound [soundPath] [soundVolume] [soundPitch] [soundPan] [soundMuted]
			if (command == "sound") {
				Sound sound = Resources.LoadSound(Resources.SoundDirectory + args[0]);
				sound.name = args[0].Substring(args[0].LastIndexOf('/') + 1);
				sound.Volume = Single.Parse(args[1]);
				sound.Pitch = Single.Parse(args[2]);
				sound.Pan = Single.Parse(args[3]);
				if (args.Count > 4)
					sound.IsMuted = Boolean.Parse(args[4]);
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
