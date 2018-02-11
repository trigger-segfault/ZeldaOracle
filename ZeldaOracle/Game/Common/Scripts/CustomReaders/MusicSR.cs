using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripts.Commands;

namespace ZeldaOracle.Common.Scripts.CustomReaders {

	public class MusicSR : ScriptReader {

		//private bool isCreatingMusic; // NOTE: Never used


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MusicSR() {
			
			//=====================================================================================
			// Begin creating music.
			/*AddCommand("Music", "", delegate(CommandParam parameters) {
				//isCreatingMusic = true;
			});*/
			//=====================================================================================
			// Finish creating music.
			/*AddCommand("End", "", delegate(CommandParam parameters) {
				//isCreatingMusic = false;
			});*/
			//=====================================================================================
			// Load a song.
			AddCommand("SONG", "string songName, float volume = 1, float pitch = 0, float pan = 0",
			delegate(CommandParam parameters) {
				string path = parameters.GetString(0);
				Song song	= Resources.LoadSong(Resources.MusicDirectory + path);
				song.name	= path.Substring(path.LastIndexOf('/') + 1);
				song.Volume	= parameters.GetFloat(1);
				song.Pitch	= parameters.GetFloat(2);
				song.Pan	= parameters.GetFloat(3);
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			//isCreatingMusic = false;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {
			//isCreatingMusic = false;
		}

		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new MusicSR();
		}
	}
}
