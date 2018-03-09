using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.API;

namespace ZeldaAPI {
	/// <summary>Access to the current game save.</summary>
	public interface Game {
		/// <summary>True if the game allows 2 extra heart containers,
		/// and starts with 4 heart containers instead of 3.</summary>
		bool IsAdvancedGame { get; set; }

		/// <summary>Gets the variables for the game.</summary>
		Variables Vars { get; }
	}
}
