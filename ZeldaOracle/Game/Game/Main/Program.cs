using System;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Main {
	#if WINDOWS || XBOX
	/// <summary>A static class for the entry point of the program.</summary>
	static class Program {
		/// <summary>The entry point of the program.</summary>
		static void Main(string[] args){

			// Creates and runs the game.
			using (GameBase game = new GameBase(args)) {
				game.Run();
			}
		}
	}
	#endif

} // End namespace
