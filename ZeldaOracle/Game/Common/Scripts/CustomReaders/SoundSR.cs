using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Scripts.Commands;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Scripts.CustomReaders {

	public class SoundSR : ScriptReader {
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SoundSR() {
			
			// Sound <name> <path> <volume=1> <pitch=0> <pan=0> <muted=false>
			AddCommand("Sound",
				"string name, string path, float volume = 1, float pitch = 0, float pan = 0, bool muted = false",
			delegate(CommandParam parameters) {
				string name = parameters.GetString(0);
				string path = parameters.GetString(1);
				Sound sound	= Resources.LoadSound(name, Resources.SoundDirectory + path);
				sound.name	= name;
				if (parameters.ChildCount > 2)
					sound.Volume = parameters.GetFloat(2);
				if (parameters.ChildCount > 3)
					sound.Pitch = parameters.GetFloat(3);
				if (parameters.ChildCount > 4)
					sound.Pan = parameters.GetFloat(4);
				if (parameters.ChildCount > 5)
					sound.IsMuted = parameters.GetBool(5);
			});
		}

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {
		}
		
		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new SoundSR();
		}
		
	}
}