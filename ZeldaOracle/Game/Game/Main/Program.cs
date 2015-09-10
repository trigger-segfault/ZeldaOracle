using System;

namespace GameFramework.MyGame.Main {
#if WINDOWS || XBOX
/** <summary>
 * A static class for the entry point of the program.
 * </summary> */
static class Program {

	//========= ENTRY POINT ==========
	#region Entry Point

	/** <summary> The entry point of the program. </summary> */
	static void Main(string[] args) {

		// Creates and runs the game
		using (GameBase game = new GameBase())
		{
			game.Run();
		}
	}

	#endregion
}
#endif
} // End namespace
