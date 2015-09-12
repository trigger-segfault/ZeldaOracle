using System;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Main {

	#if WINDOWS || XBOX

	// A static class for the entry point of the program.
	static class Program {

		// The entry point of the program.
		static void Main(string[] args) {

			// Creates and runs the game.
			using (GameBase game = new GameBase()) {
				game.Run();
			}
		}
	}
	#endif

} // End namespace
