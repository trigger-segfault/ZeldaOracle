using System;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Main {

	#if WINDOWS || XBOX

	// A static class for the entry point of the program.
	static class Program {

		// The entry point of the program.
		static void Main(string[] args) {

			// Creates and runs the game.
			using (ZeldaGame game = new ZeldaGame())
			{
				game.Run();
			}
		}
	}
	#endif

} // End namespace
