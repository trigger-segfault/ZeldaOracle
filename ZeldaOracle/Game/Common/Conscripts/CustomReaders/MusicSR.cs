using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Conscripts.Commands;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Conscripts.CustomReaders {
	public class MusicSR : ConscriptRunner {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MusicSR() {
			
			//=====================================================================================
			AddCommand("MUSIC",
				"string songName, float volume = 1, float pitch = 0, float pan = 0",
			delegate(CommandParam parameters) {
				string path		= parameters.GetString(0);
				Music music		= Resources.LoadSong(Resources.MusicDirectory + path);
				music.name		= path.Substring(path.LastIndexOf('/') + 1);
				music.Volume	= parameters.GetFloat(1);
				music.Pitch		= parameters.GetFloat(2);
				music.Pan		= parameters.GetFloat(3);
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void OnBeginReading() {
			
		}

		/// <summary>Ends reading the script.</summary>
		protected override void OnEndReading() {
			OnBeginReading();
		}
	}
}
