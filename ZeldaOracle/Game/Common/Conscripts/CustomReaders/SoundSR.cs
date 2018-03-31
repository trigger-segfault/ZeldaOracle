using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Conscripts.Commands;
using ZeldaOracle.Common.Content;

namespace ZeldaOracle.Common.Conscripts.CustomReaders {

	public class SoundSR : ConscriptRunner {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SoundSR() {

			// Sound <name> <path> [volume=1] [pitch=0] [pan=0] [muted=false] [instances=1]
			AddCommand("SOUND",
				"string name, string path, float volume = 1, float pitch = 0, float pan = 0, bool muted = false, int instances = 1",
			delegate(CommandParam parameters) {
				string name = parameters.GetString(0);
				string path = parameters.GetString(1);
				Sound sound	= Resources.LoadSound(name, Resources.SoundDirectory + path);
				sound.name	= name;
				sound.Volume = parameters.GetFloat(2);
				sound.Pitch = parameters.GetFloat(3);
				sound.Pan = parameters.GetFloat(4);
				sound.IsMuted = parameters.GetBool(5);
				sound.MaxInstances = parameters.GetInt(6);
			});
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